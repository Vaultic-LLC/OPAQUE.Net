use argon2::{Algorithm, Argon2, ParamsBuilder, Version};
use opaque_ke::ciphersuite::CipherSuite;
use opaque_ke::errors::{InternalError, ProtocolError};
use opaque_ke::ksf::{self, Ksf};
use opaque_ke::rand::rngs::OsRng;
use opaque_ke::{
    ClientLogin, ClientLoginFinishParameters, ClientRegistration,
    ClientRegistrationFinishParameters, CredentialFinalization, CredentialRequest,
    CredentialResponse, Identifiers, RegistrationRequest, RegistrationResponse, ServerLogin,
    ServerLoginStartParameters, ServerRegistration, ServerSetup,
};

use base64::{engine::general_purpose as b64, Engine as _};
use generic_array::{ArrayLength, GenericArray};

mod csharp;
use libc::c_char;

mod types;

enum Error {
    Protocol {
        context: &'static str,
        error: ProtocolError,
    },
    Base64 {
        context: &'static str,
        error: base64::DecodeError,
    },
    Internal {
        context: &'static str,
        error: InternalError,
    },
}

fn from_base64_error(context: &'static str) -> impl Fn(base64::DecodeError) -> Error {
    move |error| Error::Base64 { context, error }
}

fn from_protocol_error(context: &'static str) -> impl Fn(ProtocolError) -> Error {
    move |error| Error::Protocol { context, error }
}

struct DefaultCipherSuite;

#[cfg(not(feature = "p256"))]
impl CipherSuite for DefaultCipherSuite {
    type OprfCs = opaque_ke::Ristretto255;
    type KeGroup = opaque_ke::Ristretto255;
    type KeyExchange = opaque_ke::key_exchange::tripledh::TripleDh;
    type Ksf = types::CustomKsf;
}

#[cfg(feature = "p256")]
impl CipherSuite for DefaultCipherSuite {
    type OprfCs = p256::NistP256;
    type KeGroup = p256::NistP256;
    type KeyExchange = opaque_ke::key_exchange::tripledh::TripleDh;
    type Ksf = types::CustomKsf;
}

const BASE64: b64::GeneralPurpose = b64::URL_SAFE_NO_PAD;

type JsResult<T> = Result<T, Error>;

const INVALID_KSF_COMBINATION_ERROR: Error = Error::Internal {
    context: "Invalid keyStretching (argon2id) combination",
    error: InternalError::KsfError,
};

impl Ksf for types::CustomKsf {
    fn hash<L: ArrayLength<u8>>(
        &self,
        input: GenericArray<u8, L>,
    ) -> Result<GenericArray<u8, L>, InternalError> {
        let mut output = GenericArray::default();
        self.argon
            .hash_password_into(&input, &[0; argon2::RECOMMENDED_SALT_LEN], &mut output)
            .map_err(|_| InternalError::KsfError)?;
        Ok(output)
    }
}

fn build_argon2_ksf(
    t_cost: u32,
    m_cost: u32,
    parallelism: u32,
) -> Result<Option<types::CustomKsf>, Error> {
    let mut param_builder = ParamsBuilder::default();
    param_builder.t_cost(t_cost);
    param_builder.m_cost(m_cost);
    param_builder.p_cost(parallelism);

    if let Ok(params) = param_builder.build() {
        let argon = Argon2::new(Algorithm::Argon2id, Version::V0x13, params);
        return Ok(Some(types::CustomKsf { argon }));
    }

    Err(INVALID_KSF_COMBINATION_ERROR)
}

fn get_custom_ksf(
    ksf_config: types::KeyStretchingFunctionConfig,
) -> Result<Option<types::CustomKsf>, Error> {
    match ksf_config {
        // https://www.ietf.org/archive/id/draft-irtf-cfrg-opaque-17.html#name-configurations
        // using the recommended parameters for Argon2id except we use 2^21-1 since 2^21 crashes in browsers
        types::KeyStretchingFunctionConfig::RfcDraftRecommended => {
            build_argon2_ksf(1, u32::pow(2, 21) - 1, 4)
        }
        // https://www.rfc-editor.org/rfc/rfc9106.html#section-4-6.2
        types::KeyStretchingFunctionConfig::MemoryConstrained => {
            build_argon2_ksf(3, u32::pow(2, 16), 4)
        }
        types::KeyStretchingFunctionConfig::Custom {
            iterations,
            memory,
            parallelism,
        } => build_argon2_ksf(iterations, memory, parallelism),
    }
}

fn base64_decode<T: AsRef<[u8]>>(context: &'static str, input: T) -> JsResult<Vec<u8>> {
    BASE64.decode(input).map_err(from_base64_error(context))
}

pub fn internal_create_server_setup() -> String {
    let mut rng: OsRng = OsRng;
    let setup = ServerSetup::<DefaultCipherSuite>::new(&mut rng);
    BASE64.encode(setup.serialize())
}

fn decode_server_setup(data: String) -> JsResult<ServerSetup<DefaultCipherSuite>> {
    base64_decode("serverSetup", data).and_then(|bytes| {
        ServerSetup::<DefaultCipherSuite>::deserialize(&bytes)
            .map_err(from_protocol_error("deserialize serverSetup"))
    })
}

fn internal_get_server_public_key(data: String) -> Result<String, Error> {
    let server_setup = decode_server_setup(data)?;
    let pub_key = server_setup.keypair().public().serialize();
    Ok(BASE64.encode(pub_key))
}

fn try_create_identifiers(
    csharp_client: *mut c_char,
    csharp_server: *mut c_char,
) -> Option<types::CustomIdentifiers> {
    let rust_client: String = csharp::csharp_string_to_rust_string(csharp_client);
    let rust_server: String = csharp::csharp_string_to_rust_string(csharp_server);

    if rust_client.is_empty() && rust_server.is_empty() {
        return None;
    }

    let optional_rust_client: Option<String> = if rust_client.is_empty() {
        None
    } else {
        Some(rust_client)
    };

    let optional_rust_server: Option<String> = if rust_server.is_empty() {
        None
    } else {
        Some(rust_server)
    };

    return Some(types::CustomIdentifiers::new(
        optional_rust_client,
        optional_rust_server,
    ));
}

fn try_create_ksf_config(
    csharp_config_type: *mut c_char,
    charp_config_iterations: *mut c_char,
    charp_config_memory: *mut c_char,
    charp_config_parallelism: *mut c_char,
) -> Result<types::KeyStretchingFunctionConfig, Error> {
    let ksf_config_type: String = csharp::csharp_string_to_rust_string(csharp_config_type);

    let ksf = match ksf_config_type.as_str() {
        "rfcDraftRecommended" => Ok(types::KeyStretchingFunctionConfig::RfcDraftRecommended),
        "memoryConstrained" => Ok(types::KeyStretchingFunctionConfig::MemoryConstrained),
        "custom" => {
            let iterations: String = csharp::csharp_string_to_rust_string(charp_config_iterations);
            if iterations.is_empty() {
                return Err(INVALID_KSF_COMBINATION_ERROR);
            }

            let memory: String = csharp::csharp_string_to_rust_string(charp_config_memory);
            if memory.is_empty() {
                return Err(INVALID_KSF_COMBINATION_ERROR);
            }

            let parallelism: String =
                csharp::csharp_string_to_rust_string(charp_config_parallelism);
            if parallelism.is_empty() {
                return Err(INVALID_KSF_COMBINATION_ERROR);
            }

            Ok(types::KeyStretchingFunctionConfig::Custom {
                iterations: iterations.parse().unwrap(),
                memory: memory.parse().unwrap(),
                parallelism: parallelism.parse().unwrap(),
            })
        }
        _ => Err(INVALID_KSF_COMBINATION_ERROR),
    };

    ksf
}

fn get_identifiers(idents: &Option<types::CustomIdentifiers>) -> Identifiers {
    Identifiers {
        client: idents
            .as_ref()
            .and_then(|idents| idents.client.as_ref().map(|val| val.as_bytes())),
        server: idents
            .as_ref()
            .and_then(|idents| idents.server.as_ref().map(|val| val.as_bytes())),
    }
}

fn internal_create_server_registration_response(
    params: types::CreateServerRegistrationResponseParams,
) -> Result<String, Error> {
    let server_setup = decode_server_setup(params.server_setup)?;
    let registration_request_bytes =
        base64_decode("registrationRequest", params.registration_request)?;
    let server_registration_start_result = ServerRegistration::<DefaultCipherSuite>::start(
        &server_setup,
        RegistrationRequest::deserialize(&registration_request_bytes)
            .map_err(from_protocol_error("deserialize registrationRequest"))?,
        params.user_identifier.as_bytes(),
    )
    .map_err(from_protocol_error("start server registration"))?;
    let registration_response_bytes = server_registration_start_result.message.serialize();
    Ok(BASE64.encode(registration_response_bytes))
}

fn internal_start_server_login(
    params: types::StartServerLoginParams,
) -> Result<types::StartServerLoginResult, Error> {
    let server_setup = decode_server_setup(params.server_setup)?;
    let registration_record_bytes = match params.registration_record {
        Some(pw) => base64_decode("registrationRecord", pw).map(Some),
        None => Ok(None),
    }?;
    let credential_request_bytes = base64_decode("startLoginRequest", params.start_login_request)?;

    let mut rng: OsRng = OsRng;

    let registration_record = match registration_record_bytes.as_ref() {
        Some(bytes) => Some(
            ServerRegistration::<DefaultCipherSuite>::deserialize(bytes)
                .map_err(from_protocol_error("deserialize registrationRecord"))?,
        ),
        None => None,
    };

    let start_params = ServerLoginStartParameters {
        identifiers: get_identifiers(&params.identifiers),
        context: None,
    };

    let server_login_start_result = ServerLogin::start(
        &mut rng,
        &server_setup,
        registration_record,
        CredentialRequest::deserialize(&credential_request_bytes)
            .map_err(from_protocol_error("deserialize startLoginRequest"))?,
        params.user_identifier.as_bytes(),
        start_params,
    )
    .map_err(from_protocol_error("start server login"))?;

    let login_response = BASE64.encode(server_login_start_result.message.serialize());
    let server_login_state = BASE64.encode(server_login_start_result.state.serialize());

    Ok(types::StartServerLoginResult {
        server_login_state,
        login_response,
    })
}

fn internal_finish_server_login(params: types::FinishServerLoginParams) -> Result<String, Error> {
    let credential_finalization_bytes =
        base64_decode("finishLoginRequest", params.finish_login_request)?;
    let state_bytes = base64_decode("serverLoginState", params.server_login_state)?;
    let state = ServerLogin::<DefaultCipherSuite>::deserialize(&state_bytes)
        .map_err(from_protocol_error("deserialize serverLoginState"))?;
    let server_login_finish_result = state
        .finish(
            CredentialFinalization::deserialize(&credential_finalization_bytes)
                .map_err(from_protocol_error("deserialize finishLoginRequest"))?,
        )
        .map_err(from_protocol_error("finish server login"))?;
    Ok(BASE64.encode(server_login_finish_result.session_key))
}

fn internal_start_client_login(password: String) -> Result<types::StartClientLoginResult, Error> {
    let mut client_rng = OsRng;
    let client_login_start_result =
        ClientLogin::<DefaultCipherSuite>::start(&mut client_rng, password.as_bytes())
            .map_err(from_protocol_error("start client login"))?;

    Ok(types::StartClientLoginResult {
        client_login_state: BASE64.encode(client_login_start_result.state.serialize()),
        start_login_request: BASE64.encode(client_login_start_result.message.serialize()),
    })
}

fn internal_finish_client_login(
    params: types::FinishClientLoginParams,
) -> Result<types::FinishClientLoginResult, Error> {
    let custom_ksf = get_custom_ksf(params.key_stretching_function_config)?;
    let credential_response_bytes = base64_decode("loginResponse", params.login_response)?;
    let state_bytes = base64_decode("clientLoginState", params.client_login_state)?;
    let state = ClientLogin::<DefaultCipherSuite>::deserialize(&state_bytes)
        .map_err(from_protocol_error("deserialize clientLoginState"))?;

    let finish_params = ClientLoginFinishParameters::new(
        None,
        get_identifiers(&params.identifiers),
        custom_ksf.as_ref(),
    );

    let result = state.finish(
        params.password.as_bytes(),
        CredentialResponse::deserialize(&credential_response_bytes)
            .map_err(from_protocol_error("deserialize loginResponse"))?,
        finish_params,
    );

    if result.is_err() {
        // Client-detected login failure
        return Ok(types::FinishClientLoginResult {
            finish_login_request: "".to_string(),
            session_key: "".to_string(),
            export_key: "".to_string(),
            server_static_public_key: "".to_string(),
        });
    }
    let client_login_finish_result = result.unwrap();

    Ok(types::FinishClientLoginResult {
        finish_login_request: BASE64.encode(client_login_finish_result.message.serialize()),
        session_key: BASE64.encode(client_login_finish_result.session_key),
        export_key: BASE64.encode(client_login_finish_result.export_key),
        server_static_public_key: BASE64.encode(client_login_finish_result.server_s_pk.serialize()),
    })
}

fn internal_start_client_registration(
    password: String,
) -> Result<types::StartClientRegistrationResult, Error> {
    let mut client_rng = OsRng;

    let client_registration_start_result =
        ClientRegistration::<DefaultCipherSuite>::start(&mut client_rng, password.as_bytes())
            .map_err(from_protocol_error("start client registration"))?;

    Ok(types::StartClientRegistrationResult {
        client_registration_state: BASE64
            .encode(client_registration_start_result.state.serialize()),
        registration_request: BASE64.encode(client_registration_start_result.message.serialize()),
    })
}

fn internal_finish_client_registration(
    params: types::FinishClientRegistrationParams,
) -> Result<types::FinishClientRegistrationResult, Error> {
    let custom_ksf = get_custom_ksf(params.key_stretching_function_config)?;

    let registration_response_bytes =
        base64_decode("registrationResponse", params.registration_response)?;
    let mut rng: OsRng = OsRng;
    let client_registration =
        base64_decode("clientRegistrationState", params.client_registration_state)?;
    let state = ClientRegistration::<DefaultCipherSuite>::deserialize(&client_registration)
        .map_err(from_protocol_error("deserialize clientRegistrationState"))?;

    let finish_params = ClientRegistrationFinishParameters::new(
        get_identifiers(&params.identifiers),
        custom_ksf.as_ref(),
    );

    let client_finish_registration_result = state
        .finish(
            &mut rng,
            params.password.as_bytes(),
            RegistrationResponse::deserialize(&registration_response_bytes)
                .map_err(from_protocol_error("deserialize registrationResponse"))?,
            finish_params,
        )
        .map_err(from_protocol_error("finish client registration"))?;

    let registration_record_bytes = client_finish_registration_result.message.serialize();

    Ok(types::FinishClientRegistrationResult {
        registration_record: BASE64.encode(registration_record_bytes),
        export_key: BASE64.encode(client_finish_registration_result.export_key),
        server_static_public_key: BASE64
            .encode(client_finish_registration_result.server_s_pk.serialize()),
    })
}

#[no_mangle]
pub fn finish_client_registration(
    csharp_password: *mut c_char,
    csharp_registration_response: *mut c_char,
    csharp_client_registration_state: *mut c_char,
    csharp_client_identifier: *mut c_char,
    csharp_server_identifeir: *mut c_char,
    csharp_config_type: *mut c_char,
    charp_config_iterations: *mut c_char,
    charp_config_memory: *mut c_char,
    charp_config_parallelism: *mut c_char,
) -> *mut types::FinishClientRegistrationResult {
    let rust_password: String = csharp::csharp_string_to_rust_string(csharp_password);
    let rust_registration_response: String =
        csharp::csharp_string_to_rust_string(csharp_registration_response);
    let rust_client_registration_state: String =
        csharp::csharp_string_to_rust_string(csharp_client_registration_state);
    let identifiers: Option<types::CustomIdentifiers> =
        try_create_identifiers(csharp_client_identifier, csharp_server_identifeir);

    let ksf_config = try_create_ksf_config(
        csharp_config_type,
        charp_config_iterations,
        charp_config_memory,
        charp_config_parallelism,
    );

    let result = match ksf_config {
        Ok(v) => {
            if let Ok(result) =
                internal_finish_client_registration(types::FinishClientRegistrationParams {
                    password: rust_password,
                    registration_response: rust_registration_response,
                    client_registration_state: rust_client_registration_state,
                    identifiers: identifiers,
                    key_stretching_function_config: v,
                })
            {
                return Box::into_raw(Box::new(result));
            }

            Box::into_raw(Box::new(types::FinishClientRegistrationResult {
                registration_record: "".to_string(),
                export_key: "".to_string(),
                server_static_public_key: "".to_string(),
            }))
        }
        Err(e) => Box::into_raw(Box::new(types::FinishClientRegistrationResult {
            registration_record: "".to_string(),
            export_key: "".to_string(),
            server_static_public_key: "".to_string(),
        })),
    };

    result
}

#[no_mangle]
pub fn create_server_setup() -> *mut c_char {
    csharp::rust_string_to_csharp_string_handle(internal_create_server_setup())
}

#[no_mangle]
pub fn get_server_public_key(data: *mut c_char) -> *mut c_char {
    let secret: String = csharp::csharp_string_to_rust_string(data);
    if let Ok(public_key) = internal_get_server_public_key(secret) {
        return csharp::rust_string_to_csharp_string_handle(public_key);
    }

    return csharp::rust_string_to_csharp_string_handle("".to_string());
}

#[no_mangle]
pub fn create_server_registration_response(
    csharp_server_setup: *mut c_char,
    csharp_user_identifier: *mut c_char,
    csharp_registration_request: *mut c_char,
) -> *mut c_char {
    let rust_server_setup: String = csharp::csharp_string_to_rust_string(csharp_server_setup);
    let rust_user_identifier: String = csharp::csharp_string_to_rust_string(csharp_user_identifier);
    let rust_registration_request: String =
        csharp::csharp_string_to_rust_string(csharp_registration_request);

    if let Ok(result) = internal_create_server_registration_response(
        types::CreateServerRegistrationResponseParams {
            server_setup: rust_server_setup,
            user_identifier: rust_user_identifier,
            registration_request: rust_registration_request,
        },
    ) {
        return csharp::rust_string_to_csharp_string_handle(result);
    }

    csharp::rust_string_to_csharp_string_handle("".to_string())
}

#[no_mangle]
pub fn start_server_login(
    csharp_server_setup: *mut c_char,
    csharp_start_login_request: *mut c_char,
    csharp_user_identifier: *mut c_char,
    csharp_registration_record: *mut c_char,
    csharp_client_identifier: *mut c_char,
    csharp_server_identifier: *mut c_char,
) -> *mut types::StartServerLoginResult {
    let rust_server_setup: String = csharp::csharp_string_to_rust_string(csharp_server_setup);
    let rust_start_login_request: String =
        csharp::csharp_string_to_rust_string(csharp_start_login_request);
    let rust_user_identifier: String = csharp::csharp_string_to_rust_string(csharp_user_identifier);

    let potentially_empty_registration_record =
        csharp::csharp_string_to_rust_string(csharp_registration_record);
    let rust_registration_record: Option<String> =
        if potentially_empty_registration_record.is_empty() {
            None
        } else {
            Some(potentially_empty_registration_record)
        };

    let identifiers: Option<types::CustomIdentifiers> =
        try_create_identifiers(csharp_client_identifier, csharp_server_identifier);

    if let Ok(result) = internal_start_server_login(types::StartServerLoginParams {
        server_setup: rust_server_setup,
        registration_record: rust_registration_record,
        start_login_request: rust_start_login_request,
        user_identifier: rust_user_identifier,
        identifiers: identifiers,
    }) {
        return Box::into_raw(Box::new(result));
    }

    Box::into_raw(Box::new(types::StartServerLoginResult {
        server_login_state: "".to_string(),
        login_response: "".to_string(),
    }))
}

#[no_mangle]
pub fn finish_server_login(
    csharp_server_login_state: *mut c_char,
    csharp_finish_login_request: *mut c_char,
) -> *mut c_char {
    let rust_server_login_state: String =
        csharp::csharp_string_to_rust_string(csharp_server_login_state);
    let rust_finish_login_request: String =
        csharp::csharp_string_to_rust_string(csharp_finish_login_request);

    if let Ok(result) = internal_finish_server_login(types::FinishServerLoginParams {
        server_login_state: rust_server_login_state,
        finish_login_request: rust_finish_login_request,
    }) {
        return csharp::rust_string_to_csharp_string_handle(result);
    }

    return csharp::rust_string_to_csharp_string_handle("".to_string());
}

#[no_mangle]
pub fn start_client_login(csharp_password: *mut c_char) -> *mut types::StartClientLoginResult {
    let rust_password: String = csharp::csharp_string_to_rust_string(csharp_password);
    if let Ok(result) = internal_start_client_login(rust_password) {
        return Box::into_raw(Box::new(result));
    }

    Box::into_raw(Box::new(types::StartClientLoginResult {
        client_login_state: "".to_string(),
        start_login_request: "".to_string(),
    }))
}

#[no_mangle]
pub fn finish_client_login(
    csharp_client_login_state: *mut c_char,
    csharp_login_response: *mut c_char,
    csharp_password: *mut c_char,
    csharp_client_identifier: *mut c_char,
    csharp_server_identifeir: *mut c_char,
    csharp_config_type: *mut c_char,
    charp_config_iterations: *mut c_char,
    charp_config_memory: *mut c_char,
    charp_config_parallelism: *mut c_char,
) -> *mut types::FinishClientLoginResult {
    let rust_client_login_state: String =
        csharp::csharp_string_to_rust_string(csharp_client_login_state);
    let rust_login_response: String = csharp::csharp_string_to_rust_string(csharp_login_response);
    let rust_password: String = csharp::csharp_string_to_rust_string(csharp_password);
    let identifiers: Option<types::CustomIdentifiers> =
        try_create_identifiers(csharp_client_identifier, csharp_server_identifeir);

    let ksf_config = try_create_ksf_config(
        csharp_config_type,
        charp_config_iterations,
        charp_config_memory,
        charp_config_parallelism,
    );

    let result = match ksf_config {
        Ok(v) => {
            if let Ok(result) = internal_finish_client_login(types::FinishClientLoginParams {
                client_login_state: rust_client_login_state,
                login_response: rust_login_response,
                password: rust_password,
                identifiers: identifiers,
                key_stretching_function_config: v,
            }) {
                return Box::into_raw(Box::new(result));
            }

            Box::into_raw(Box::new(types::FinishClientLoginResult {
                finish_login_request: "".to_string(),
                session_key: "".to_string(),
                export_key: "".to_string(),
                server_static_public_key: "".to_string(),
            }))
        }
        Err(e) => Box::into_raw(Box::new(types::FinishClientLoginResult {
            finish_login_request: "".to_string(),
            session_key: "".to_string(),
            export_key: "".to_string(),
            server_static_public_key: "".to_string(),
        })),
    };

    result
}

#[no_mangle]
pub fn start_client_registration(
    csharp_password: *mut c_char,
) -> *mut types::StartClientRegistrationResult {
    let rust_password: String = csharp::csharp_string_to_rust_string(csharp_password);
    if let Ok(result) = internal_start_client_registration(rust_password) {
        return Box::into_raw(Box::new(result));
    }

    Box::into_raw(Box::new(types::StartClientRegistrationResult {
        client_registration_state: "".to_string(),
        registration_request: "".to_string(),
    }))
}
