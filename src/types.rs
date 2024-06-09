use crate::csharp;
use libc::c_char;
use serde::{Deserialize, Serialize};

#[derive(Debug, Serialize, Deserialize)]
pub struct CustomIdentifiers {
    pub client: Option<String>,
    pub server: Option<String>,
}

impl CustomIdentifiers {
    pub fn new(client_param: Option<String>, server_param: Option<String>) -> CustomIdentifiers {
        CustomIdentifiers {
            client: client_param,
            server: server_param,
        }
    }
}

#[derive(Debug, Serialize, Deserialize)]
pub struct CreateServerRegistrationResponseParams {
    #[serde(rename = "serverSetup")]
    pub server_setup: String,
    #[serde(rename = "userIdentifier")]
    pub user_identifier: String,
    #[serde(rename = "registrationRequest")]
    pub registration_request: String,
}

#[derive(Debug, Serialize, Deserialize)]
pub struct StartServerLoginParams {
    #[serde(rename = "serverSetup")]
    pub server_setup: String,
    #[serde(rename = "registrationRecord")]
    pub registration_record: Option<String>,
    #[serde(rename = "startLoginRequest")]
    pub start_login_request: String,
    #[serde(rename = "userIdentifier")]
    pub user_identifier: String,
    pub identifiers: Option<CustomIdentifiers>,
}

#[derive(Debug, Serialize, Deserialize)]
pub struct StartServerLoginResult {
    #[serde(rename = "serverLoginState")]
    pub server_login_state: String,
    #[serde(rename = "loginResponse")]
    pub login_response: String,
}

#[no_mangle]
pub extern "C" fn get_start_server_login_response_state(
    ptr: *mut StartServerLoginResult,
) -> *const c_char {
    let result = unsafe {
        assert!(!ptr.is_null());
        &mut *ptr
    };

    csharp::rust_string_to_csharp_string_handle(result.server_login_state.clone())
}

#[no_mangle]
pub extern "C" fn get_start_server_login_response_response(
    ptr: *mut StartServerLoginResult,
) -> *const c_char {
    let result = unsafe {
        assert!(!ptr.is_null());
        &mut *ptr
    };

    csharp::rust_string_to_csharp_string_handle(result.login_response.clone())
}

#[no_mangle]
pub extern "C" fn free_start_server_login_result(ptr: *mut StartServerLoginResult) {
    if ptr.is_null() {
        return;
    }

    unsafe {
        drop(Box::from_raw(ptr));
    }
}

#[derive(Debug, Serialize, Deserialize)]
pub struct FinishServerLoginParams {
    #[serde(rename = "serverLoginState")]
    pub server_login_state: String,
    #[serde(rename = "finishLoginRequest")]
    pub finish_login_request: String,
}

#[derive(Debug, Serialize, Deserialize)]
pub struct StartClientLoginResult {
    #[serde(rename = "clientLoginState")]
    pub client_login_state: String,
    #[serde(rename = "startLoginRequest")]
    pub start_login_request: String,
}

#[no_mangle]
pub extern "C" fn get_start_client_login_result_state(
    ptr: *mut StartClientLoginResult,
) -> *const c_char {
    let result = unsafe {
        assert!(!ptr.is_null());
        &mut *ptr
    };

    csharp::rust_string_to_csharp_string_handle(result.client_login_state.clone())
}

#[no_mangle]
pub extern "C" fn get_start_client_login_result_request(
    ptr: *mut StartClientLoginResult,
) -> *const c_char {
    let result = unsafe {
        assert!(!ptr.is_null());
        &mut *ptr
    };

    csharp::rust_string_to_csharp_string_handle(result.start_login_request.clone())
}

#[no_mangle]
pub extern "C" fn free_start_client_login_result(ptr: *mut StartServerLoginResult) {
    if ptr.is_null() {
        return;
    }

    unsafe {
        drop(Box::from_raw(ptr));
    }
}

#[derive(Debug, Serialize, Deserialize)]
pub struct FinishClientLoginParams {
    #[serde(rename = "clientLoginState")]
    pub client_login_state: String,
    #[serde(rename = "loginResponse")]
    pub login_response: String,
    pub password: String,
    pub identifiers: Option<CustomIdentifiers>,
}

#[derive(Debug, Serialize, Deserialize)]
pub struct FinishClientLoginResult {
    #[serde(rename = "finishLoginRequest")]
    pub finish_login_request: String,
    #[serde(rename = "sessionKey")]
    pub session_key: String,
    #[serde(rename = "exportKey")]
    pub export_key: String,
    #[serde(rename = "serverStaticPublicKey")]
    pub server_static_public_key: String,
}

#[no_mangle]
pub extern "C" fn get_finish_client_login_result_request(
    ptr: *mut FinishClientLoginResult,
) -> *const c_char {
    let result = unsafe {
        assert!(!ptr.is_null());
        &mut *ptr
    };

    csharp::rust_string_to_csharp_string_handle(result.finish_login_request.clone())
}

#[no_mangle]
pub extern "C" fn get_finish_client_login_result_session_key(
    ptr: *mut FinishClientLoginResult,
) -> *const c_char {
    let result = unsafe {
        assert!(!ptr.is_null());
        &mut *ptr
    };

    csharp::rust_string_to_csharp_string_handle(result.session_key.clone())
}

#[no_mangle]
pub extern "C" fn get_finish_client_login_result_export_key(
    ptr: *mut FinishClientLoginResult,
) -> *const c_char {
    let result = unsafe {
        assert!(!ptr.is_null());
        &mut *ptr
    };

    csharp::rust_string_to_csharp_string_handle(result.export_key.clone())
}

#[no_mangle]
pub extern "C" fn get_finish_client_login_result_public_key(
    ptr: *mut FinishClientLoginResult,
) -> *const c_char {
    let result = unsafe {
        assert!(!ptr.is_null());
        &mut *ptr
    };

    csharp::rust_string_to_csharp_string_handle(result.server_static_public_key.clone())
}

#[no_mangle]
pub extern "C" fn free_finish_client_login_result(ptr: *mut StartServerLoginResult) {
    if ptr.is_null() {
        return;
    }

    unsafe {
        drop(Box::from_raw(ptr));
    }
}

#[derive(Debug, Serialize, Deserialize)]
pub struct StartClientRegistrationResult {
    #[serde(rename = "clientRegistrationState")]
    pub client_registration_state: String,
    #[serde(rename = "registrationRequest")]
    pub registration_request: String,
}

#[no_mangle]
pub extern "C" fn get_start_client_registration_result_state(
    ptr: *mut StartClientRegistrationResult,
) -> *const c_char {
    let result = unsafe {
        assert!(!ptr.is_null());
        &mut *ptr
    };

    csharp::rust_string_to_csharp_string_handle(result.client_registration_state.clone())
}

#[no_mangle]
pub extern "C" fn get_start_client_registration_result_request(
    ptr: *mut StartClientRegistrationResult,
) -> *const c_char {
    let result = unsafe {
        assert!(!ptr.is_null());
        &mut *ptr
    };

    csharp::rust_string_to_csharp_string_handle(result.registration_request.clone())
}

#[no_mangle]
pub extern "C" fn free_start_client_registration_result(ptr: *mut StartServerLoginResult) {
    if ptr.is_null() {
        return;
    }

    unsafe {
        drop(Box::from_raw(ptr));
    }
}

#[derive(Debug, Serialize, Deserialize)]
pub struct FinishClientRegistrationParams {
    pub password: String,
    #[serde(rename = "registrationResponse")]
    pub registration_response: String,
    #[serde(rename = "clientRegistrationState")]
    pub client_registration_state: String,
    pub identifiers: Option<CustomIdentifiers>,
}

#[derive(Debug, Serialize, Deserialize)]
pub struct FinishClientRegistrationResult {
    #[serde(rename = "registrationRecord")]
    pub registration_record: String,
    #[serde(rename = "exportKey")]
    pub export_key: String,
    #[serde(rename = "serverStaticPublicKey")]
    pub server_static_public_key: String,
}

#[no_mangle]
pub extern "C" fn get_finish_client_registration_result_record(
    ptr: *mut FinishClientRegistrationResult,
) -> *const c_char {
    let result = unsafe {
        assert!(!ptr.is_null());
        &mut *ptr
    };

    csharp::rust_string_to_csharp_string_handle(result.export_key.clone())
}

#[no_mangle]
pub extern "C" fn get_finish_client_registration_result_export_key(
    ptr: *mut FinishClientRegistrationResult,
) -> *const c_char {
    let result = unsafe {
        assert!(!ptr.is_null());
        &mut *ptr
    };

    csharp::rust_string_to_csharp_string_handle(result.registration_record.clone())
}

#[no_mangle]
pub extern "C" fn get_finish_client_registration_result_public_key(
    ptr: *mut FinishClientRegistrationResult,
) -> *const c_char {
    let result = unsafe {
        assert!(!ptr.is_null());
        &mut *ptr
    };

    csharp::rust_string_to_csharp_string_handle(result.server_static_public_key.clone())
}

#[no_mangle]
pub extern "C" fn free_finish_client_registration_result(ptr: *mut StartServerLoginResult) {
    if ptr.is_null() {
        return;
    }

    unsafe {
        drop(Box::from_raw(ptr));
    }
}
