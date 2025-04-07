using OPAQUE.Net.Helpers;
using OPAQUE.Net.Types.Exceptions;
using OPAQUE.Net.Types.Handles;
using OPAQUE.Net.Types.Results;
using System.Runtime.InteropServices;

namespace OPAQUE.Net
{
    public class OpaqueServer
    {
        public OpaqueServer() { }

        /// <summary>
        /// Creates the server setup
        /// </summary>
        /// <param name="serverSetup">The server setup</param>
        /// <returns>True if succeeded, false otherwise. Out parameter will not be null if succeeded</returns>
        public bool CreateSetup(out string? serverSetup)
        {
            serverSetup = FunctionHelper.TryExecute(create_server_setup)?.GetAndRelease();
            return !string.IsNullOrEmpty(serverSetup);
        }

        /// <summary>
        /// Gets the public key for a server setup
        /// </summary>
        /// <param name="serverSetup">String from <see cref="OpaqueServer.CreateSetup(out string?)"/></param>
        /// <param name="publicKey">The public key for the server setup</param>
        /// <returns>True if succeeded, false otherwise. Out parameter will not be null if succeeded</returns>
        public bool GetPublicKey(string serverSetup, out string? publicKey)
        {
            StringParamIsEmptyException.ThrowIfEmpty(serverSetup, nameof(serverSetup));

            publicKey = FunctionHelper.TryExecute(() => get_server_public_key(serverSetup))?.GetAndRelease();
            return !string.IsNullOrEmpty(publicKey);
        }

        /// <summary>
        /// Creates a registration response
        /// </summary>
        /// <param name="serverSetup">String from <see cref="OpaqueServer.CreateSetup(out string?)"/></param>
        /// <param name="userIdentifier">Current user identifier</param>
        /// <param name="registrationRequest"><see cref="StartClientRegistrationResult.RegistrationRequest"/> from <see cref="OpaqueClient.StartRegistration(string, out StartClientRegistrationResult?)"/></param>
        /// <param name="registrationResponse">The resposne from the registration</param>
        /// <returns>True if succeeded, false otherwise. Out parameter will not be null if succeeded</returns>
        public bool CreateRegistrationResponse(string serverSetup, string userIdentifier, string registrationRequest, out string? registrationResponse)
        {
            StringParamIsEmptyException.ThrowIfEmpty(serverSetup, nameof(serverSetup));
            StringParamIsEmptyException.ThrowIfEmpty(userIdentifier, nameof(userIdentifier));
            StringParamIsEmptyException.ThrowIfEmpty(registrationRequest, nameof(registrationRequest));

            registrationResponse = FunctionHelper.TryExecute(() => create_server_registration_response(serverSetup, userIdentifier, 
                registrationRequest))?.GetAndRelease();

            return !string.IsNullOrEmpty(registrationResponse);
        }

        /// <summary>
        /// Starts the login process for the server
        /// </summary>
        /// <param name="serverSetup">String from <see cref="OpaqueServer.CreateSetup(out string?)"/></param>
        /// <param name="startLoginRequest"><see cref="StartClientLoginResult.StartLoginRequest"/> from <see cref="OpaqueClient.StartLogin(string, out StartClientLoginResult?)"/></param>
        /// <param name="userIdentifier">Current user identifier</param>
        /// <param name="registrationRecord"><see cref="FinishClientRegistrationResult.RegistrationRecord"/> from <see cref="OpaqueClient.FinishRegistration(string, string, string, string?, string?, out FinishClientRegistrationResult?)"/></param>
        /// <param name="result">A <see cref="StartServerLoginResult"/> filled with data if successful</param>
        /// <returns>True if succeeded, false otherwise. Out parameter will not be null if succeeded</returns>
        public bool StartLogin(string serverSetup, string startLoginRequest, string userIdentifier, string? registrationRecord, out StartServerLoginResult? result, out Exception? e)
        {
            return StartLogin(serverSetup, startLoginRequest, userIdentifier, registrationRecord, "", "", out result, out e);
        }

        /// <summary>
        /// Starts the login process for the server
        /// </summary>
        /// <param name="serverSetup">String from <see cref="OpaqueServer.CreateSetup(out string?)"/></param>
        /// <param name="startLoginRequest"><see cref="StartClientLoginResult.StartLoginRequest"/> from <see cref="OpaqueClient.StartLogin(string, out StartClientLoginResult?)"/></param>
        /// <param name="userIdentifier">Current user identifier</param>
        /// <param name="registrationRecord"><see cref="FinishClientRegistrationResult.RegistrationRecord"/> from <see cref="OpaqueClient.FinishRegistration(string, string, string, string?, string?, out FinishClientRegistrationResult?)"/></param>
        /// <param name="clientIdentitiy">Current client identifier</param>
        /// <param name="serverIdentity">Current server identifier</param>
        /// <param name="result">A <see cref="StartServerLoginResult"/> filled with data if successful</param>
        /// <returns>True if succeeded, false otherwise. Out parameter will not be null if succeeded</returns>
        public bool StartLogin(string serverSetup, string startLoginRequest, string userIdentifier, 
            string? registrationRecord, string? clientIdentitiy, string? serverIdentity, out StartServerLoginResult? result, out Exception? e)
        {
            StringParamIsEmptyException.ThrowIfEmpty(serverSetup, nameof(serverSetup));
            StringParamIsEmptyException.ThrowIfEmpty(startLoginRequest, nameof(startLoginRequest));
            StringParamIsEmptyException.ThrowIfEmpty(userIdentifier, nameof(userIdentifier));

            result = FunctionHelper.TryExecute(() => start_server_login(serverSetup, startLoginRequest, 
                userIdentifier, registrationRecord ?? "", clientIdentitiy ?? "", serverIdentity ?? ""), out e)?.GetAndRelease();

            return result != null;
        }

        /// <summary>
        /// Finishes the login process for the server
        /// </summary>
        /// <param name="serverLoginState"><see cref="StartServerLoginResult.ServerLoginState"/> from <see cref="OpaqueServer.StartLogin(string, string, string, string?, string?, string?, out StartServerLoginResult?)"/></param>
        /// <param name="finishLoginRequest"><see cref="FinishClientLoginResult.FinishLoginRequest"/> from <see cref="OpaqueClient.FinishLogin(string, string, string, string?, string?, out FinishClientLoginResult?)"/></param>
        /// <param name="serverSessionKey">The generated session key</param>
        /// <returns>True if succeeded, false otherwise. Out parameter will not be null if succeeded</returns>
        public bool FinishLogin(string serverLoginState, string finishLoginRequest, out string? serverSessionKey)
        {
            StringParamIsEmptyException.ThrowIfEmpty(serverLoginState, nameof(serverLoginState));
            StringParamIsEmptyException.ThrowIfEmpty(finishLoginRequest, nameof(finishLoginRequest));

            serverSessionKey = FunctionHelper.TryExecute(() => finish_server_login(serverLoginState, finishLoginRequest))?.GetAndRelease();
            return !string.IsNullOrEmpty(serverSessionKey);
        }

        [DllImport("opaque")]
        private static extern StringHandle create_server_setup();

        [DllImport("opaque")]
        private static extern StringHandle get_server_public_key(string secret);

        [DllImport("opaque")]
        private static extern StringHandle create_server_registration_response(string serverSetup, string userIdentifier, string registrationRequest);

        [DllImport("opaque")]
        private static extern StartServerLoginResultHandle start_server_login(string serverSetup, string startLoginRequest, 
            string userIdentifier, string? registrationRecord, string? clientIdentitiy, string? serverIdentity);

        [DllImport("opaque")]
        private static extern StringHandle finish_server_login(string serverLoginState, string finishLoginRequest);
    }
}
