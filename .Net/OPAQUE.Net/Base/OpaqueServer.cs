using OPAQUE.Net.Base.Exceptions;
using OPAQUE.Net.Base.Handles;
using OPAQUE.Net.Base.Helpers;
using OPAQUE.Net.Base.Results;

namespace OPAQUE.Net.Factory
{
    public static partial class OpaqueFactory
    {
        public interface IOpaqueServer
        {
            /// <summary>
            /// Creates the server setup
            /// </summary>
            /// <param name="serverSetup">The server setup</param>
            /// <returns>True if succeeded, false otherwise. Out parameter will not be null if succeeded</returns>
            bool CreateSetup(out string? serverSetup, out Exception? e);
            /// <summary>
            /// Gets the public key for a server setup
            /// </summary>
            /// <param name="serverSetup">String from <see cref="CreateSetup(out string?)"/></param>
            /// <param name="publicKey">The public key for the server setup</param>
            /// <returns>True if succeeded, false otherwise. Out parameter will not be null if succeeded</returns>
            bool GetPublicKey(string serverSetup, out string? publicKey);
            /// <summary>
            /// Creates a registration response
            /// </summary>
            /// <param name="serverSetup">String from <see cref="CreateSetup(out string?)"/></param>
            /// <param name="userIdentifier">Current user identifier</param>
            /// <param name="registrationRequest"><see cref="StartClientRegistrationResult.RegistrationRequest"/> from <see cref="OpaqueClient.StartRegistration(string, out StartClientRegistrationResult?)"/></param>
            /// <param name="registrationResponse">The resposne from the registration</param>
            /// <returns>True if succeeded, false otherwise. Out parameter will not be null if succeeded</returns>
            bool CreateRegistrationResponse(string serverSetup, string userIdentifier, string registrationRequest, out string? registrationResponse);
            /// <summary>
            /// Starts the login process for the server
            /// </summary>
            /// <param name="serverSetup">String from <see cref="CreateSetup(out string?)"/></param>
            /// <param name="startLoginRequest"><see cref="StartClientLoginResult.StartLoginRequest"/> from <see cref="OpaqueClient.StartLogin(string, out StartClientLoginResult?)"/></param>
            /// <param name="userIdentifier">Current user identifier</param>
            /// <param name="registrationRecord"><see cref="FinishClientRegistrationResult.RegistrationRecord"/> from <see cref="OpaqueClient.FinishRegistration(string, string, string, string?, string?, out FinishClientRegistrationResult?)"/></param>
            /// <param name="result">A <see cref="StartServerLoginResult"/> filled with data if successful</param>
            /// <returns>True if succeeded, false otherwise. Out parameter will not be null if succeeded</returns>
            bool StartLogin(string serverSetup, string startLoginRequest, string userIdentifier, string? registrationRecord, out StartServerLoginResult? result);
            /// <summary>
            /// Starts the login process for the server
            /// </summary>
            /// <param name="serverSetup">String from <see cref="CreateSetup(out string?)"/></param>
            /// <param name="startLoginRequest"><see cref="StartClientLoginResult.StartLoginRequest"/> from <see cref="OpaqueClient.StartLogin(string, out StartClientLoginResult?)"/></param>
            /// <param name="userIdentifier">Current user identifier</param>
            /// <param name="registrationRecord"><see cref="FinishClientRegistrationResult.RegistrationRecord"/> from <see cref="OpaqueClient.FinishRegistration(string, string, string, string?, string?, out FinishClientRegistrationResult?)"/></param>
            /// <param name="clientIdentitiy">Current client identifier</param>
            /// <param name="serverIdentity">Current server identifier</param>
            /// <param name="result">A <see cref="StartServerLoginResult"/> filled with data if successful</param>
            /// <returns>True if succeeded, false otherwise. Out parameter will not be null if succeeded</returns>
            bool StartLogin(string serverSetup, string startLoginRequest, string userIdentifier, string? registrationRecord, string? clientIdentitiy, string? serverIdentity, out StartServerLoginResult? result);
            /// <summary>
            /// Finishes the login process for the server
            /// </summary>
            /// <param name="serverLoginState"><see cref="StartServerLoginResult.ServerLoginState"/> from <see cref="StartLogin(string, string, string, string?, string?, string?, out StartServerLoginResult?)"/></param>
            /// <param name="finishLoginRequest"><see cref="FinishClientLoginResult.FinishLoginRequest"/> from <see cref="OpaqueClient.FinishLogin(string, string, string, string?, string?, out FinishClientLoginResult?)"/></param>
            /// <param name="serverSessionKey">The generated session key</param>
            /// <returns>True if succeeded, false otherwise. Out parameter will not be null if succeeded</returns>
            bool FinishLogin(string serverLoginState, string finishLoginRequest, out string? serverSessionKey);
        }

        private abstract partial class OpaqueServer : IOpaqueServer
        {
            public OpaqueServer() { }

            /// <summary>
            /// Creates the server setup
            /// </summary>
            /// <param name="serverSetup">The server setup</param>
            /// <returns>True if succeeded, false otherwise. Out parameter will not be null if succeeded</returns>
            public bool CreateSetup(out string? serverSetup, out Exception? e)
            {
                serverSetup = FunctionHelper.TryExecute(CreateServerSetup, out e)?.GetAndRelease();
                return !string.IsNullOrEmpty(serverSetup);
            }

            /// <summary>
            /// Gets the public key for a server setup
            /// </summary>
            /// <param name="serverSetup">String from <see cref="CreateSetup(out string?)"/></param>
            /// <param name="publicKey">The public key for the server setup</param>
            /// <returns>True if succeeded, false otherwise. Out parameter will not be null if succeeded</returns>
            public bool GetPublicKey(string serverSetup, out string? publicKey)
            {
                StringParamIsEmptyException.ThrowIfEmpty(serverSetup, nameof(serverSetup));

                publicKey = FunctionHelper.TryExecute(() => GetServerPublicKey(serverSetup))?.GetAndRelease();
                return !string.IsNullOrEmpty(publicKey);
            }

            /// <summary>
            /// Creates a registration response
            /// </summary>
            /// <param name="serverSetup">String from <see cref="CreateSetup(out string?)"/></param>
            /// <param name="userIdentifier">Current user identifier</param>
            /// <param name="registrationRequest"><see cref="StartClientRegistrationResult.RegistrationRequest"/> from <see cref="OpaqueClient.StartRegistration(string, out StartClientRegistrationResult?)"/></param>
            /// <param name="registrationResponse">The resposne from the registration</param>
            /// <returns>True if succeeded, false otherwise. Out parameter will not be null if succeeded</returns>
            public bool CreateRegistrationResponse(string serverSetup, string userIdentifier, string registrationRequest, out string? registrationResponse)
            {
                StringParamIsEmptyException.ThrowIfEmpty(serverSetup, nameof(serverSetup));
                StringParamIsEmptyException.ThrowIfEmpty(userIdentifier, nameof(userIdentifier));
                StringParamIsEmptyException.ThrowIfEmpty(registrationRequest, nameof(registrationRequest));

                registrationResponse = FunctionHelper.TryExecute(() => CreateServerRegistrationResponse(serverSetup, userIdentifier,
                    registrationRequest))?.GetAndRelease();

                return !string.IsNullOrEmpty(registrationResponse);
            }

            /// <summary>
            /// Starts the login process for the server
            /// </summary>
            /// <param name="serverSetup">String from <see cref="CreateSetup(out string?)"/></param>
            /// <param name="startLoginRequest"><see cref="StartClientLoginResult.StartLoginRequest"/> from <see cref="OpaqueClient.StartLogin(string, out StartClientLoginResult?)"/></param>
            /// <param name="userIdentifier">Current user identifier</param>
            /// <param name="registrationRecord"><see cref="FinishClientRegistrationResult.RegistrationRecord"/> from <see cref="OpaqueClient.FinishRegistration(string, string, string, string?, string?, out FinishClientRegistrationResult?)"/></param>
            /// <param name="result">A <see cref="StartServerLoginResult"/> filled with data if successful</param>
            /// <returns>True if succeeded, false otherwise. Out parameter will not be null if succeeded</returns>
            public bool StartLogin(string serverSetup, string startLoginRequest, string userIdentifier, string? registrationRecord, out StartServerLoginResult? result)
            {
                return StartLogin(serverSetup, startLoginRequest, userIdentifier, registrationRecord, "", "", out result);
            }

            /// <summary>
            /// Starts the login process for the server
            /// </summary>
            /// <param name="serverSetup">String from <see cref="CreateSetup(out string?)"/></param>
            /// <param name="startLoginRequest"><see cref="StartClientLoginResult.StartLoginRequest"/> from <see cref="OpaqueClient.StartLogin(string, out StartClientLoginResult?)"/></param>
            /// <param name="userIdentifier">Current user identifier</param>
            /// <param name="registrationRecord"><see cref="FinishClientRegistrationResult.RegistrationRecord"/> from <see cref="OpaqueClient.FinishRegistration(string, string, string, string?, string?, out FinishClientRegistrationResult?)"/></param>
            /// <param name="clientIdentitiy">Current client identifier</param>
            /// <param name="serverIdentity">Current server identifier</param>
            /// <param name="result">A <see cref="StartServerLoginResult"/> filled with data if successful</param>
            /// <returns>True if succeeded, false otherwise. Out parameter will not be null if succeeded</returns>
            public bool StartLogin(string serverSetup, string startLoginRequest, string userIdentifier,
                string? registrationRecord, string? clientIdentitiy, string? serverIdentity, out StartServerLoginResult? result)
            {
                StringParamIsEmptyException.ThrowIfEmpty(serverSetup, nameof(serverSetup));
                StringParamIsEmptyException.ThrowIfEmpty(startLoginRequest, nameof(startLoginRequest));
                StringParamIsEmptyException.ThrowIfEmpty(userIdentifier, nameof(userIdentifier));

                result = FunctionHelper.TryExecute(() => StartServerLogin(serverSetup, startLoginRequest,
                    userIdentifier, registrationRecord ?? "", clientIdentitiy ?? "", serverIdentity ?? ""))?.GetAndRelease();

                return result != null;
            }

            /// <summary>
            /// Finishes the login process for the server
            /// </summary>
            /// <param name="serverLoginState"><see cref="StartServerLoginResult.ServerLoginState"/> from <see cref="StartLogin(string, string, string, string?, string?, string?, out StartServerLoginResult?)"/></param>
            /// <param name="finishLoginRequest"><see cref="FinishClientLoginResult.FinishLoginRequest"/> from <see cref="OpaqueClient.FinishLogin(string, string, string, string?, string?, out FinishClientLoginResult?)"/></param>
            /// <param name="serverSessionKey">The generated session key</param>
            /// <returns>True if succeeded, false otherwise. Out parameter will not be null if succeeded</returns>
            public bool FinishLogin(string serverLoginState, string finishLoginRequest, out string? serverSessionKey)
            {
                StringParamIsEmptyException.ThrowIfEmpty(serverLoginState, nameof(serverLoginState));
                StringParamIsEmptyException.ThrowIfEmpty(finishLoginRequest, nameof(finishLoginRequest));

                serverSessionKey = FunctionHelper.TryExecute(() => FinishServerLogin(serverLoginState, finishLoginRequest))?.GetAndRelease();
                return !string.IsNullOrEmpty(serverSessionKey);
            }

            protected abstract StringHandle CreateServerSetup();

            protected abstract StringHandle GetServerPublicKey(string secret);

            protected abstract StringHandle CreateServerRegistrationResponse(string serverSetup, string userIdentifier, string registrationRequest);

            protected abstract StartServerLoginResultHandle? StartServerLogin(string serverSetup, string startLoginRequest,
                string userIdentifier, string? registrationRecord, string? clientIdentity, string? serverIdentity);

            protected abstract StringHandle FinishServerLogin(string serverLoginState, string finishLoginRequest);
        }
    }
}
