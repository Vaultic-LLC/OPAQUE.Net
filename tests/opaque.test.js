import * as opaqueP256 from "../build/p256";
import * as opaqueRistretto from "../build/ristretto";

const opaque =
  process.env.OPAQUE_BUILD === "p256" ? opaqueP256 : opaqueRistretto;

/**
 * @typedef {{client?:string;server?:string}} Identifiers
 */

/**
 * @param {string} userIdentifier
 * @param {string} password
 * @param {Identifiers|undefined} identifiers
 */
function setupAndRegister(userIdentifier, password, identifiers = undefined) {
  const serverSetup = opaque.server.createSetup();
  const { clientRegistrationState, registrationRequest } =
    opaque.client.startRegistration({ password });
  const { registrationResponse } = opaque.server.createRegistrationResponse({
    serverSetup,
    userIdentifier,
    registrationRequest,
  });
  const { registrationRecord, exportKey, serverStaticPublicKey } =
    opaque.client.finishRegistration({
      clientRegistrationState,
      registrationResponse,
      password,
      identifiers,
    });

  return {
    serverSetup,
    registrationRecord,
    exportKey,
    serverStaticPublicKey,
  };
}

beforeAll(async () => {
  await opaque.ready;
});

test("full registration & login flow", () => {
  const userIdentifier = "user123";
  const password = "hunter42";

  const {
    serverSetup,
    registrationRecord,
    exportKey: registrationExportKey,
    serverStaticPublicKey: registrationServerStaticPublicKey,
  } = setupAndRegister(userIdentifier, password);

  const { clientLoginState, startLoginRequest } = opaque.client.startLogin({
    password,
  });

  const { serverLoginState, loginResponse } = opaque.server.startLogin({
    serverSetup,
    userIdentifier,
    registrationRecord,
    startLoginRequest,
  });

  const loginResult = opaque.client.finishLogin({
    clientLoginState,
    loginResponse,
    password,
  });

  expect(loginResult).not.toBeUndefined();

  if (!loginResult) throw new TypeError(); // for typescript

  const {
    sessionKey: clientSessionKey,
    finishLoginRequest,
    exportKey: loginExportKey,
    serverStaticPublicKey: loginServerStaticPublicKey,
  } = loginResult;

  expect(registrationExportKey).toEqual(loginExportKey);
  expect(registrationServerStaticPublicKey).toEqual(loginServerStaticPublicKey);
  expect(loginServerStaticPublicKey).toEqual(
    opaque.server.getPublicKey(serverSetup)
  );

  const { sessionKey: serverSessionKey } = opaque.server.finishLogin({
    serverLoginState,
    finishLoginRequest,
  });

  expect(serverSessionKey).toEqual(clientSessionKey);
});

test("full registration & login with bad password", () => {
  const userIdentifier = "user123";

  const { serverSetup, registrationRecord } = setupAndRegister(
    userIdentifier,
    "hunter42"
  );
  const { clientLoginState, startLoginRequest } = opaque.client.startLogin({
    password: "hunter42",
  });

  const { loginResponse } = opaque.server.startLogin({
    serverSetup,
    userIdentifier,
    registrationRecord,
    startLoginRequest,
  });

  const loginResult = opaque.client.finishLogin({
    clientLoginState,
    loginResponse,
    password: "hunter23",
  });

  expect(loginResult).toBeUndefined();
});

test("full registration & login flow with mismatched custom client identifier on server login", () => {
  const userIdentifier = "user123";
  const client = "client123";
  const password = "hunter2";

  const { serverSetup, registrationRecord } = setupAndRegister(
    userIdentifier,
    password,
    { client }
  );

  const { clientLoginState, startLoginRequest } = opaque.client.startLogin({
    password,
  });

  const { loginResponse } = opaque.server.startLogin({
    serverSetup,
    userIdentifier,
    registrationRecord,
    startLoginRequest,
    identifiers: {
      client,
    },
  });

  const loginResult = opaque.client.finishLogin({
    clientLoginState,
    loginResponse,
    password,
    identifiers: {
      client: client + "abc",
    },
  });

  expect(loginResult).toBeUndefined();
});

test("full registration & login attempt with mismatched server identifier", () => {
  const userIdentifier = "client123";
  const password = "hunter2";

  const { serverSetup, registrationRecord } = setupAndRegister(
    userIdentifier,
    password,
    { server: "server-ident" }
  );

  const { clientLoginState, startLoginRequest } = opaque.client.startLogin({
    password,
  });

  const { loginResponse } = opaque.server.startLogin({
    serverSetup,
    registrationRecord,
    startLoginRequest,
    userIdentifier,
    identifiers: {
      server: "server-ident-abc",
    },
  });

  const loginResult = opaque.client.finishLogin({
    clientLoginState,
    loginResponse,
    password,
    identifiers: {
      server: "server-ident",
    },
  });

  expect(loginResult).toBeUndefined();
});

describe("client.startRegistration", () => {
  test("invalid argument type", () => {
    expect(() => {
      // @ts-expect-error intentional test of invalid input
      opaque.client.startRegistration();
    }).toThrow(
      "invalid type: unit value, expected struct StartClientRegistrationParams"
    );
    expect(() => {
      // @ts-expect-error intentional test of invalid input
      opaque.client.startRegistration(123);
    }).toThrow(
      "invalid type: floating point `123`, expected struct StartClientRegistrationParams"
    );
  });
  test("incomplete params object", () => {
    expect(() => {
      // @ts-expect-error intentional test of invalid input
      opaque.client.startRegistration({});
    }).toThrow("missing field `password`");
  });
});

describe("client.finishRegistration", () => {
  test("invalid argument type", () => {
    expect(() =>
      // @ts-expect-error intentional test of invalid input
      opaque.client.finishRegistration()
    ).toThrow(
      "invalid type: unit value, expected struct FinishClientRegistrationParams"
    );

    expect(() =>
      // @ts-expect-error intentional test of invalid input
      opaque.client.finishRegistration(123)
    ).toThrow(
      "invalid type: floating point `123`, expected struct FinishClientRegistrationParams"
    );
  });
  test("incomplete params object", () => {
    expect(() =>
      // @ts-expect-error intentional test of invalid input
      opaque.client.finishRegistration({})
    ).toThrow("Error: missing field `password`");
    expect(() =>
      // @ts-expect-error intentional test of invalid input
      opaque.client.finishRegistration({
        password: "hunter2",
        clientRegistrationState: "",
      })
    ).toThrow("Error: missing field `registrationResponse`");
    expect(() =>
      // @ts-expect-error intentional test of invalid input
      opaque.client.finishRegistration({
        password: "hunter2",
        registrationResponse: "",
      })
    ).toThrow("Error: missing field `clientRegistrationState`");
  });

  test("registrationResponse invalid", () => {
    const { clientRegistrationState } = opaque.client.startRegistration({
      password: "hunter2",
    });
    expect(() => {
      opaque.client.finishRegistration({
        password: "hunter2",
        registrationResponse: "",
        clientRegistrationState,
      });
    }).toThrow(
      'opaque protocol error at "deserialize registrationResponse"; Internal error encountered'
    );
  });

  test("registrationResponse encoding invalid", () => {
    const { clientRegistrationState } = opaque.client.startRegistration({
      password: "hunter2",
    });
    expect(() => {
      opaque.client.finishRegistration({
        password: "hunter2",
        registrationResponse: "a",
        clientRegistrationState,
      });
    }).toThrow(
      'base64 decoding failed at "registrationResponse"; Encoded text cannot have a 6-bit remainder.'
    );
  });

  test("clientRegistrationState invalid", () => {
    const { registrationRequest } = opaque.client.startRegistration({
      password: "hunter2",
    });
    const { registrationResponse } = opaque.server.createRegistrationResponse({
      userIdentifier: "user123",
      registrationRequest,
      serverSetup: opaque.server.createSetup(),
    });
    expect(() => {
      opaque.client.finishRegistration({
        password: "hunter2",
        registrationResponse,
        clientRegistrationState: "",
      });
    }).toThrow(
      'opaque protocol error at "deserialize clientRegistrationState"; Internal error encountered'
    );
  });

  test("clientRegistrationState encoding invalid", () => {
    const { registrationRequest } = opaque.client.startRegistration({
      password: "hunter2",
    });
    const { registrationResponse } = opaque.server.createRegistrationResponse({
      userIdentifier: "user123",
      registrationRequest,
      serverSetup: opaque.server.createSetup(),
    });
    expect(() => {
      opaque.client.finishRegistration({
        password: "hunter2",
        registrationResponse,
        clientRegistrationState: "a",
      });
    }).toThrow(
      'base64 decoding failed at "clientRegistrationState"; Encoded text cannot have a 6-bit remainder.'
    );
  });
});

describe("client.startLogin", () => {
  test("invalid argument type", () => {
    expect(() => {
      // @ts-expect-error intentional test of invalid input
      opaque.client.startLogin();
    }).toThrow(
      "invalid type: unit value, expected struct StartClientLoginParams"
    );
    expect(() => {
      // @ts-expect-error intentional test of invalid input
      opaque.client.startLogin(123);
    }).toThrow(
      "invalid type: floating point `123`, expected struct StartClientLoginParams"
    );
  });
  test("incomplete params object", () => {
    expect(() => {
      // @ts-expect-error intentional test of invalid input
      opaque.client.startLogin({});
    }).toThrow("missing field `password`");
  });
});

describe("client.finishLogin", () => {
  test("invalid argument type", () => {
    expect(() =>
      // @ts-expect-error intentional test of invalid input
      opaque.client.finishLogin()
    ).toThrow(
      "invalid type: unit value, expected struct FinishClientLoginParams"
    );

    expect(() =>
      // @ts-expect-error intentional test of invalid input
      opaque.client.finishLogin(123)
    ).toThrow(
      "invalid type: floating point `123`, expected struct FinishClientLoginParams"
    );
  });

  test("incomplete params object", () => {
    expect(() =>
      // @ts-expect-error intentional test of invalid input
      opaque.client.finishLogin({})
    ).toThrow("missing field `clientLoginState`");
    expect(() =>
      // @ts-expect-error intentional test of invalid input
      opaque.client.finishLogin({ clientLoginState: "" })
    ).toThrow("missing field `loginResponse`");
    expect(() =>
      // @ts-expect-error intentional test of invalid input
      opaque.client.finishLogin({ clientLoginState: "", loginResponse: "" })
    ).toThrow("missing field `password`");
  });

  test("clientLoginState invalid", () => {
    expect(() =>
      opaque.client.finishLogin({
        clientLoginState: "",
        loginResponse: "",
        password: "hunter2",
      })
    ).toThrow(
      'opaque protocol error at "deserialize clientLoginState"; Internal error encountered'
    );
  });

  test("clientLoginState encoding invalid", () => {
    expect(() =>
      opaque.client.finishLogin({
        clientLoginState: "a",
        loginResponse: "",
        password: "hunter2",
      })
    ).toThrow(
      'base64 decoding failed at "clientLoginState"; Encoded text cannot have a 6-bit remainder.'
    );
  });

  test("loginResponse invalid", () => {
    expect(() => {
      const { clientLoginState } = opaque.client.startLogin({
        password: "hunter2",
      });
      opaque.client.finishLogin({
        clientLoginState,
        loginResponse: "",
        password: "hunter2",
      });
    }).toThrow(
      'opaque protocol error at "deserialize loginResponse"; Internal error encountered'
    );
  });

  test("loginResponse encoding invalid", () => {
    expect(() => {
      const { clientLoginState } = opaque.client.startLogin({
        password: "hunter2",
      });
      opaque.client.finishLogin({
        clientLoginState,
        loginResponse: "a",
        password: "hunter2",
      });
    }).toThrow(
      'base64 decoding failed at "loginResponse"; Encoded text cannot have a 6-bit remainder.'
    );
  });
});

describe("server.createRegistrationResponse", () => {
  test("invalid params type", () => {
    expect(() =>
      // @ts-expect-error intentional test of invalid input
      opaque.server.createRegistrationResponse()
    ).toThrow(
      "invalid type: unit value, expected struct CreateServerRegistrationResponseParams"
    );
    expect(() =>
      // @ts-expect-error intentional test of invalid input
      opaque.server.createRegistrationResponse(123)
    ).toThrow(
      "invalid type: floating point `123`, expected struct CreateServerRegistrationResponseParams"
    );
    expect(() =>
      // @ts-expect-error intentional test of invalid input
      opaque.server.createRegistrationResponse("test")
    ).toThrow(
      'invalid type: string "test", expected struct CreateServerRegistrationResponseParams'
    );
  });

  test("incomplete params object", () => {
    expect(() =>
      // @ts-expect-error intentional test of invalid input
      opaque.server.createRegistrationResponse({})
    ).toThrow("missing field `serverSetup`");
    expect(() =>
      // @ts-expect-error intentional test of invalid input
      opaque.server.createRegistrationResponse({ serverSetup: "" })
    ).toThrow("missing field `userIdentifier`");
    expect(() =>
      // @ts-expect-error intentional test of invalid input
      opaque.server.createRegistrationResponse({
        serverSetup: "",
        userIdentifier: "",
      })
    ).toThrow("missing field `registrationRequest`");
  });

  test("serverSetup invalid", () => {
    expect(() => {
      const { registrationRequest } = opaque.client.startRegistration({
        password: "hunter2",
      });
      opaque.server.createRegistrationResponse({
        serverSetup: "abcd",
        userIdentifier: "user1",
        registrationRequest,
      });
    }).toThrow(
      'opaque protocol error at "deserialize serverSetup"; Internal error encountered'
    );
  });

  test("serverSetup decoding", () => {
    expect(() => {
      const { registrationRequest } = opaque.client.startRegistration({
        password: "hunter2",
      });
      opaque.server.createRegistrationResponse({
        serverSetup: "a",
        userIdentifier: "user1",
        registrationRequest,
      });
    }).toThrow(
      'base64 decoding failed at "serverSetup"; Encoded text cannot have a 6-bit remainder.'
    );
  });

  test("registrationRequest invalid", () => {
    expect(() => {
      const serverSetup = opaque.server.createSetup();
      opaque.server.createRegistrationResponse({
        serverSetup,
        userIdentifier: "user1",
        registrationRequest: "",
      });
    }).toThrow(
      'opaque protocol error at "deserialize registrationRequest"; Internal error encountered'
    );
  });

  test("registrationRequest decoding", () => {
    expect(() => {
      const serverSetup = opaque.server.createSetup();
      opaque.server.createRegistrationResponse({
        serverSetup,
        userIdentifier: "user1",
        registrationRequest: "a",
      });
    }).toThrow(
      'base64 decoding failed at "registrationRequest"; Encoded text cannot have a 6-bit remainder.'
    );
  });
});

describe("server.startLogin", () => {
  test("invalid params type", () => {
    expect(() =>
      // @ts-expect-error intentional test of invalid input
      opaque.server.startLogin()
    ).toThrow(
      "invalid type: unit value, expected struct StartServerLoginParams"
    );
    expect(() =>
      // @ts-expect-error intentional test of invalid input
      opaque.server.startLogin(123)
    ).toThrow(
      "invalid type: floating point `123`, expected struct StartServerLoginParams"
    );
    expect(() =>
      // @ts-expect-error intentional test of invalid input
      opaque.server.startLogin("test")
    ).toThrow(
      'invalid type: string "test", expected struct StartServerLoginParams'
    );
  });

  test("incomplete params object", () => {
    expect(() =>
      // @ts-expect-error intentional test of invalid input
      opaque.server.startLogin({})
    ).toThrow("missing field `serverSetup`");

    expect(() =>
      // @ts-expect-error intentional test of invalid input
      opaque.server.startLogin({ serverSetup: "" })
    ).toThrow("missing field `startLoginRequest`");
    expect(() =>
      // @ts-expect-error intentional test of invalid input
      opaque.server.startLogin({
        serverSetup: "",
        startLoginRequest: "",
      })
    ).toThrow("missing field `userIdentifier`");
  });

  test("serverSetup invalid", () => {
    expect(() =>
      // @ts-expect-error intentional test of invalid input
      opaque.server.startLogin({
        serverSetup: "",
        startLoginRequest: "",
        userIdentifier: "",
      })
    ).toThrow(
      'opaque protocol error at "deserialize serverSetup"; Internal error encountered'
    );
  });

  test("serverSetup encoding invalid", () => {
    expect(() =>
      // @ts-expect-error intentional test of invalid input
      opaque.server.startLogin({
        serverSetup: "a",
        startLoginRequest: "",
        userIdentifier: "",
      })
    ).toThrow(
      'base64 decoding failed at "serverSetup"; Encoded text cannot have a 6-bit remainder.'
    );
  });

  test("startLoginRequest invalid", () => {
    expect(() =>
      // @ts-expect-error intentional test of invalid input
      opaque.server.startLogin({
        serverSetup: opaque.server.createSetup(),
        startLoginRequest: "",
        userIdentifier: "",
      })
    ).toThrow(
      'opaque protocol error at "deserialize startLoginRequest"; Internal error encountered'
    );
  });

  test("startLoginRequest encoding invalid", () => {
    expect(() =>
      // @ts-expect-error intentional test of invalid input
      opaque.server.startLogin({
        serverSetup: opaque.server.createSetup(),
        startLoginRequest: "a",
        userIdentifier: "",
      })
    ).toThrow(
      'base64 decoding failed at "startLoginRequest"; Encoded text cannot have a 6-bit remainder.'
    );
  });

  test("dummy server login credential response", () => {
    const password = "hunter2";
    const serverSetup = opaque.server.createSetup();
    const { startLoginRequest, clientLoginState } = opaque.client.startLogin({
      password,
    });
    const { loginResponse } = opaque.server.startLogin({
      userIdentifier: "user1",
      serverSetup,
      startLoginRequest,
      registrationRecord: undefined,
    });
    const result = opaque.client.finishLogin({
      clientLoginState,
      loginResponse,
      password,
    });
    expect(result).toBeUndefined();
  });
});

describe("server.finishLogin", () => {
  test("invalid params type", () => {
    expect(() =>
      // @ts-expect-error intentional test of invalid input
      opaque.server.finishLogin()
    ).toThrow(
      "invalid type: unit value, expected struct FinishServerLoginParams"
    );
    expect(() =>
      // @ts-expect-error intentional test of invalid input
      opaque.server.finishLogin(123)
    ).toThrow(
      "invalid type: floating point `123`, expected struct FinishServerLoginParams"
    );
    expect(() =>
      // @ts-expect-error intentional test of invalid input
      opaque.server.finishLogin("test")
    ).toThrow(
      'invalid type: string "test", expected struct FinishServerLoginParams'
    );
  });

  test("incomplete params object", () => {
    expect(() =>
      // @ts-expect-error intentional test of invalid input
      opaque.server.finishLogin({ finishLoginRequest: "" })
    ).toThrow("missing field `serverLoginState`");

    expect(() =>
      // @ts-expect-error intentional test of invalid input
      opaque.server.finishLogin({ serverLoginState: "" })
    ).toThrow("missing field `finishLoginRequest`");
  });

  test("finishLoginRequest invalid", () => {
    const { startLoginRequest } = opaque.client.startLogin({
      password: "hunter2",
    });
    const { serverLoginState } = opaque.server.startLogin({
      registrationRecord: null,
      serverSetup: opaque.server.createSetup(),
      startLoginRequest,
      userIdentifier: "user123",
    });
    expect(() => {
      opaque.server.finishLogin({ serverLoginState, finishLoginRequest: "" });
    }).toThrow(
      'opaque protocol error at "deserialize finishLoginRequest"; Internal error encountered'
    );
  });

  test("finishLoginRequest encoding invalid", () => {
    const { startLoginRequest } = opaque.client.startLogin({
      password: "hunter2",
    });
    const { serverLoginState } = opaque.server.startLogin({
      registrationRecord: null,
      serverSetup: opaque.server.createSetup(),
      startLoginRequest,
      userIdentifier: "user123",
    });
    expect(() => {
      opaque.server.finishLogin({ serverLoginState, finishLoginRequest: "a" });
    }).toThrow(
      'base64 decoding failed at "finishLoginRequest"; Encoded text cannot have a 6-bit remainder'
    );
  });

  test("serverLoginState invalid", () => {
    const username = "user123";
    const password = "hunter2";
    const { serverSetup, registrationRecord } = setupAndRegister(
      username,
      password
    );
    const { startLoginRequest, clientLoginState } = opaque.client.startLogin({
      password,
    });
    const { loginResponse } = opaque.server.startLogin({
      registrationRecord,
      serverSetup,
      startLoginRequest,
      userIdentifier: username,
    });
    const result = opaque.client.finishLogin({
      clientLoginState,
      loginResponse,
      password,
    });
    expect(result).not.toBeUndefined();
    if (!result) throw new TypeError();
    const { finishLoginRequest } = result;
    expect(() => {
      opaque.server.finishLogin({ serverLoginState: "", finishLoginRequest });
    }).toThrow(
      'opaque protocol error at "deserialize serverLoginState"; Internal error encountered'
    );
  });

  test("serverLoginState encoding invalid", () => {
    const username = "user123";
    const password = "hunter2";
    const { serverSetup, registrationRecord } = setupAndRegister(
      username,
      password
    );
    const { startLoginRequest, clientLoginState } = opaque.client.startLogin({
      password,
    });
    const { loginResponse } = opaque.server.startLogin({
      registrationRecord,
      serverSetup,
      startLoginRequest,
      userIdentifier: username,
    });
    const result = opaque.client.finishLogin({
      clientLoginState,
      loginResponse,
      password,
    });
    expect(result).not.toBeUndefined();
    if (!result) throw new TypeError();
    const { finishLoginRequest } = result;
    expect(() => {
      opaque.server.finishLogin({ serverLoginState: "a", finishLoginRequest });
    }).toThrow(
      'base64 decoding failed at "serverLoginState"; Encoded text cannot have a 6-bit remainder.'
    );
  });
});
