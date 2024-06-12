# Opaque

Secure password based client-server authentication without the server ever obtaining knowledge of the password.

A C# implementation of the [OPAQUE protocol](https://datatracker.ietf.org/doc/draft-irtf-cfrg-opaque/) forked from [serenity-key/opaque](https://github.com/serenity-kit/opaque).

## Benefits

- Never accidentally log passwords
- Security against pre-computation attacks upon server compromise
- Foundation for encrypted backups and end-to-end encryption apps
- Original repo had a [penetration test and whitebox security review](https://7asecurity.com/reports/pentest-report-opaque.pdf) done through OTF’s [Red Team Lab](https://www.opentech.fund/labs/red-team-lab/), via 7ASecurity

## Documentation

In depth documentation can be found at [https://opaque-auth.com/](https://opaque-auth.com/) (written for the original repo)

## Install

```sh
npm install @serenity-kit/opaque
```

### Usage

The API is exposed through the `OpaqueServer` and `OpaqueClient` objects.
```cs
OpaqueServer server = new OpaqueServer();
OpaqueClient client = new OpaqueClient();
```

### Server Setup

The server setup is a one-time operation. It is used to generate the server's long-term private key. The result is a 171 long string. Only store it in a secure location and make sure you have it available in your application e.g. via an environment variable.

You can generate a new server setup on the fly:

```cs
server.CreateSetup(out string? serverSetup);
```

Keep in mind that changing the serverSetup will invalidate all existing password files.

### Registration Flow

```cs
// client
string password = "sup-krah.42-UOI"; // user password
if (!client.StartRegistration(
    password, 
    out StartClientRegistrationResult? startClientRegistrationResult))
{
    // handle failed 
}
```

```cs
// server
string userIdentifier = "20e14cd8-ab09-4f4b-87a8-06d2e2e9ff68"; // userId/email/username
if (!server.CreateRegistrationResponse(
    serverSetup, 
    userIdentifier, 
    startClientRegistrationResult.RegistrationRequest, 
    out string? registrationResponse))
{
    // handle failed
}
```

```cs
// client
if (!client.FinishRegistration(
    password, 
    registrationResponse, 
    startClientRegistrationResult.ClientRegistrationState, 
    out FinishClientRegistrationResult? finishClientRegistrationResult))
{
    // handle failed
}
// send finishClientRegistrationResult.RegistrationRecord to server and create user account
```

### Login Flow

```cs
// client
string password = "sup-krah.42-UOI"; // user password
if (!client.StartLogin(
    password, 
    out StartClientLoginResult? startClientLoginResult))
{
    // handle failed
}
```

```cs
// server
string userIdentifier = "20e14cd8-ab09-4f4b-87a8-06d2e2e9ff68"; // userId/email/username
string registrationRecord; // retrieve original registration record

if (!server.StartLogin(
    serverSetup, 
    startClientLoginResult.StartLoginRequest, 
    userIdentifier, 
    registrationRecord, 
    out StartServerLoginResult? startServerLoginResult))
{
    // handle failed
}
```

```cs
// client
if (!client.FinishLogin(
    startClientLoginResult.ClientLoginState, 
    startServerLoginResult.LoginResponse, 
    password, 
    out FinishClientLoginResult? finishClientLoginResult))
{
    // handle failed
}
```

```cs
// server

if (!server.FinishLogin(
    startServerLoginResult.ServerLoginState, 
    finishClientLoginResult.FinishLoginRequest,
    out string? sessionKey))
{
    // handle failed
}
```

## Advanced usage

### ExportKey

After the initial registration flow as well as every login flow, the client has access to a private key only available to the client. This is the `exportKey`. The key is not available to the server and it is stable. Meaning if you log in multiple times your `exportKey` will stay the same.

#### Example usage

**Registration**

```cs
// client
if (!client.FinishRegistration(
    password, 
    registrationResponse, 
    startClientRegistrationResult.ClientRegistrationState, 
    out FinishClientRegistrationResult? finishClientRegistrationResult))
{
    // handle failed
}

finishClientRegistrationResult.ExportKey;
```

**Login**

```cs
// client
if (!client.FinishLogin(
    startClientLoginResult.ClientLoginState, 
    startServerLoginResult.LoginResponse, 
    password, 
    out FinishClientLoginResult? finishClientLoginResult))
{
    // handle failed
}

finishClientLoginResult.ExportKey;
```

### Server Static Public Key

The result of `client.FinishRegistration` and `client.FinishLogin` also contains a property `ServerStaticPublicKey`. It can be used to verify the authenticity of the server.

It's recommended to verify the server static public key in the application layer e.g. hard-code it into the application code and verify it's correctness.

#### Example usage

**Server**

The `ServerStaticPublicKey` can be extracted using the following CLI command:

```sh
npx @serenity-kit/opaque@latest get-server-public-key "<server setup string>"
```

Alternatively the functionality is exposed via

```cs
if (!server.GetPublicKey(
    serverSetupString, 
    out string? publicKey))
{
    // handle failed
}
```

**Client**

Registration

```cs
// client
if (!client.FinishRegistration(
    password, 
    registrationResponse, 
    startClientRegistrationResult.ClientRegistrationState, 
    out FinishClientRegistrationResult? finishClientRegistrationResult))
{
    // handle failed
}

finishClientRegistrationResult.ServerStaticPublicKey;
```

Login

```cs
// client
if (!client.FinishLogin(
    startClientLoginResult.ClientLoginState, 
    startServerLoginResult.LoginResponse, 
    password, 
    out FinishClientLoginResult? finishClientLoginResult))
{
    // handle failed
}

finishClientLoginResult.ServerStaticPublicKey;
```

### Identifiers

By default the server-side sets a `userIdentifier` during the registration and login process. This `userIdentifier` does not even need to be exposed to be exposed to a client.

`client.FinishRegistration`, `server.StartLogin` and `client.FinishLogin` all have overloads that take a optional string value for `clientIdentifier` and `serverIdentifier`.

```cs
client.FinishRegistration(
    password, 
    registrationResponse, 
    clientRegistrationState, 
    clientIdentifier, 
    serverIdentifier, 
    out FinishClientRegistrationResult? result);

server.StartLogin(
    serverSetup, 
    startLoginRequest, 
    userIdentifier, 
    registrationRecord, 
    clientIdentitiy, 
    serverIdentity, 
    out StartServerLoginResult? result);

client.FinishLogin(
    clientLoginState, 
    serverLoginResponse, 
    password, 
    clientIdentifier, 
    serverIdentifier, 
    out FinishClientLoginResult? result);
```

The identifiers will be public, but cryptographically bound to the registration record.

Once provided in the `client.FinishRegistration` function call, the identical identifiers must be provided in the `server.StartLogin` and `client.FinishLogin` function call. Otherwise the login will result in an error.

#### Example Registration

```cs
// client
if (!client.FinishRegistration(
    password, 
    registrationResponse, 
    startClientRegistrationResult.ClientRegistrationState, 
    "jane@example.com", 
    "mastodon.example.com", 
    out FinishClientRegistrationResult? result))
{
    // handle failed
}
// send registrationRecord to server and create user account
```

#### Example Login

```cs
// server
if (!server.StartLogin(
    serverSetup, 
    startClientLoginResult.StartLoginRequest, 
    userIdentifier, 
    registrationRecord, 
    "jane@example.com", 
    "mastodon.example.com", 
    out StartServerLoginResult? startServerLoginResult))
{
    // handle failed
}
```

```cs
// client
if (!client.FinishLogin(
    startClientLoginResult.ClientLoginState, 
    startServerLoginResult.LoginResponse, 
    password, 
    "jane@example.com", 
    "mastodon.example.com", 
    out FinishClientLoginResult? finishClientLoginResult))
{
    // handle failed
}
```
