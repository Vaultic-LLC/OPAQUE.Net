using OPAQUE.Net.Helpers;
using OPAQUE.Net.Types.Exceptions;
using OPAQUE.Net.Types.Handles;
using OPAQUE.Net.Types.Results;
using System.Runtime.InteropServices;

namespace OPAQUE.Net
{
    public class OpaqueClient
    {
        public OpaqueClient() { }

        /// <summary>
        /// Starts the registration process
        /// </summary>
        /// <param name="password">User's Password</param>
        /// <param name="result">A <see cref="StartClientRegistrationResult"/> filled with data if successful</param>
        /// <returns>True if succeeded, false otherwise. Out parameter will not be null if succeeded</returns>
        public bool StartRegistration(string password, out StartClientRegistrationResult? result)
        {
            StringParamIsEmptyException.ThrowIfEmpty(password, nameof(password));

            result = FunctionHelper.TryExecute(() => start_client_registration(password))?.GetAndRelease();
            return result != null;
        }

        /// <summary>
        /// Finishes the registration process for the client side
        /// </summary>
        /// <param name="password">Users's Password</param>
        /// <param name="registrationResponse">Out parameter from <see cref="OpaqueServer.CreateRegistrationResponse(string, string, string, out string?)"/></param>
        /// <param name="clientRegistrationState"><see cref="StartClientRegistrationResult.ClientRegistrationState"/> from <see cref="OpaqueClient.StartRegistration(string, out StartClientRegistrationResult?)"/></param>
        /// <param name="clientIdentifier">Current client identifier</param>
        /// <param name="serverIdentifier">Current server identifier</param>
        /// <param name="result">A <see cref="FinishClientRegistrationResult"/> filled with data if successful</param>
        /// <returns>True if succeeded, false otherwise. Out parameter will not be null if succeeded</returns>
        public bool FinishRegistration(string password, string registrationResponse, string clientRegistrationState, 
            string? clientIdentifier, string? serverIdentifier, out FinishClientRegistrationResult? result)
        {
            StringParamIsEmptyException.ThrowIfEmpty(password, nameof(password));
            StringParamIsEmptyException.ThrowIfEmpty(registrationResponse, nameof(registrationResponse));
            StringParamIsEmptyException.ThrowIfEmpty(clientRegistrationState, nameof(clientRegistrationState));

            result = FunctionHelper.TryExecute(() => finish_client_registration(password, registrationResponse, 
                clientRegistrationState, clientIdentifier ?? "", serverIdentifier ?? ""))?.GetAndRelease();

            return result != null;
        }

        /// <summary>
        /// Starts the login process
        /// </summary>
        /// <param name="password">Users's current password</param>
        /// <param name="result">A <see cref="StartClientLoginResult"/> filled with data if successful</param>
        /// <returns>True if succeeded, false otherwise. Out parameter will not be null if succeeded</returns>
        public bool StartLogin(string password, out StartClientLoginResult? result)
        {
            StringParamIsEmptyException.ThrowIfEmpty(password, nameof(password));

            result = FunctionHelper.TryExecute(() => start_client_login(password))?.GetAndRelease();
            return result != null;
        }

        /// <summary>
        /// Finishes the login process for the client
        /// </summary>
        /// <param name="clientLoginState"><see cref="StartClientLoginResult.ClientLoginState"/> from <see cref="OpaqueClient.StartLogin(string, out StartClientLoginResult?)"/></param>
        /// <param name="serverLoginResponse"><see cref="StartServerLoginResult.LoginResponse"/> from <see cref="OpaqueServer.StartLogin(string, string, string, string?, string?, string?, out StartServerLoginResult?)"/></param>
        /// <param name="password">Users's current password</param>
        /// <param name="clientIdentifier">Current client identifier</param>
        /// <param name="serverIdentifier">Current server identifier</param>
        /// <param name="result">A <see cref="FinishClientLoginResult"/> filled with data if successful</param>
        /// <returns>True if succeeded, false otherwise. Out parameter will not be null if succeeded</returns>
        public bool FinishLogin(string clientLoginState, string serverLoginResponse, string password, string? clientIdentifier, 
            string? serverIdentifier, out FinishClientLoginResult? result)
        {
            StringParamIsEmptyException.ThrowIfEmpty(clientLoginState, nameof(clientLoginState));
            StringParamIsEmptyException.ThrowIfEmpty(serverLoginResponse, nameof(serverLoginResponse));
            StringParamIsEmptyException.ThrowIfEmpty(password, nameof(password));

            result = FunctionHelper.TryExecute(() => finish_client_login(clientLoginState, serverLoginResponse, 
                password, clientIdentifier ?? "", serverIdentifier ?? ""))?.GetAndRelease();

            return result != null;
        }

        [DllImport("opaque.dll")]
        private static extern StartClientRegistrationResultHandle? start_client_registration(string password);

        [DllImport("opaque.dll")]
        private static extern FinishClientRegistrationResultHandle finish_client_registration(string password, string registrationResponse, 
            string clientRegistrationState, string? clientIdentifier, string? serverIdentifier);

        [DllImport("opaque.dll")]
        private static extern StartClientLoginResultHandle start_client_login(string password);

        [DllImport("opaque.dll")]
        private static extern FinishClientLoginResultHandler? finish_client_login(string clientLoginState, string serverLoginResponse, string password, 
            string? clientIdentifier, string? serverIdentifier);
    }
}
