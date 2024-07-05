using OPAQUE.Net.Base.Exceptions;
using OPAQUE.Net.Base.Handles;
using OPAQUE.Net.Base.Helpers;
using OPAQUE.Net.Base.Results;

namespace OPAQUE.Net.Factory
{
    public static partial class OpaqueFactory
    {
        public interface IOpaqueClient
        {
            /// <summary>
            /// Starts the registration process
            /// </summary>
            /// <param name="password">User's Password</param>
            /// <param name="result">A <see cref="StartClientRegistrationResult"/> filled with data if successful</param>
            /// <returns>True if succeeded, false otherwise. Out parameter will not be null if succeeded</returns>
            bool StartRegistration(string password, out StartClientRegistrationResult? result);
            /// <summary>
            /// Finishes the registration process for the client side
            /// </summary>
            /// <param name="password">Users's Password</param>
            /// <param name="registrationResponse">Out parameter from <see cref="OpaqueServer.CreateRegistrationResponse(string, string, string, out string?)"/></param>
            /// <param name="clientRegistrationState"><see cref="StartClientRegistrationResult.ClientRegistrationState"/> from <see cref="StartRegistration(string, out StartClientRegistrationResult?)"/></param>
            /// <param name="result">A <see cref="FinishClientRegistrationResult"/> filled with data if successful</param>
            /// <returns>True if succeeded, false otherwise. Out parameter will not be null if succeeded</returns>
            bool FinishRegistration(string password, string registrationResponse, string clientRegistrationState, out FinishClientRegistrationResult? result);
            /// <summary>
            /// Finishes the registration process for the client side
            /// </summary>
            /// <param name="password">Users's Password</param>
            /// <param name="registrationResponse">Out parameter from <see cref="OpaqueServer.CreateRegistrationResponse(string, string, string, out string?)"/></param>
            /// <param name="clientRegistrationState"><see cref="StartClientRegistrationResult.ClientRegistrationState"/> from <see cref="StartRegistration(string, out StartClientRegistrationResult?)"/></param>
            /// <param name="clientIdentifier">Current client identifier</param>
            /// <param name="serverIdentifier">Current server identifier</param>
            /// <param name="result">A <see cref="FinishClientRegistrationResult"/> filled with data if successful</param>
            /// <returns>True if succeeded, false otherwise. Out parameter will not be null if succeeded</returns>
            bool FinishRegistration(string password, string registrationResponse, string clientRegistrationState, string? clientIdentifier, string? serverIdentifier, out FinishClientRegistrationResult? result);
            /// <summary>
            /// Starts the login process
            /// </summary>
            /// <param name="password">Users's current password</param>
            /// <param name="result">A <see cref="StartClientLoginResult"/> filled with data if successful</param>
            /// <returns>True if succeeded, false otherwise. Out parameter will not be null if succeeded</returns>
            bool StartLogin(string password, out StartClientLoginResult? result);
            /// <summary>
            /// Finishes the login process for the client
            /// </summary>
            /// <param name="clientLoginState"><see cref="StartClientLoginResult.ClientLoginState"/> from <see cref="StartLogin(string, out StartClientLoginResult?)"/></param>
            /// <param name="serverLoginResponse"><see cref="StartServerLoginResult.LoginResponse"/> from <see cref="OpaqueServer.StartLogin(string, string, string, string?, string?, string?, out StartServerLoginResult?)"/></param>
            /// <param name="password">Users's current password</param>
            /// <param name="result">A <see cref="FinishClientLoginResult"/> filled with data if successful</param>
            /// <returns>True if succeeded, false otherwise. Out parameter will not be null if succeeded</returns>
            bool FinishLogin(string clientLoginState, string serverLoginResponse, string password, out FinishClientLoginResult? result);
            /// <summary>
            /// Finishes the login process for the client
            /// </summary>
            /// <param name="clientLoginState"><see cref="StartClientLoginResult.ClientLoginState"/> from <see cref="StartLogin(string, out StartClientLoginResult?)"/></param>
            /// <param name="serverLoginResponse"><see cref="StartServerLoginResult.LoginResponse"/> from <see cref="OpaqueServer.StartLogin(string, string, string, string?, string?, string?, out StartServerLoginResult?)"/></param>
            /// <param name="password">Users's current password</param>
            /// <param name="clientIdentifier">Current client identifier</param>
            /// <param name="serverIdentifier">Current server identifier</param>
            /// <param name="result">A <see cref="FinishClientLoginResult"/> filled with data if successful</param>
            /// <returns>True if succeeded, false otherwise. Out parameter will not be null if succeeded</returns>
            bool FinishLogin(string clientLoginState, string serverLoginResponse, string password, string? clientIdentifier, string? serverIdentifier, out FinishClientLoginResult? result);
        }

        private abstract class OpaqueClient : IOpaqueClient
        {
            protected OpaqueClient() { }

            /// <summary>
            /// Starts the registration process
            /// </summary>
            /// <param name="password">User's Password</param>
            /// <param name="result">A <see cref="StartClientRegistrationResult"/> filled with data if successful</param>
            /// <returns>True if succeeded, false otherwise. Out parameter will not be null if succeeded</returns>
            public bool StartRegistration(string password, out StartClientRegistrationResult? result)
            {
                StringParamIsEmptyException.ThrowIfEmpty(password, nameof(password));

                result = FunctionHelper.TryExecute(() => StartClientRegistration(password))?.GetAndRelease();
                return result != null;
            }

            /// <summary>
            /// Finishes the registration process for the client side
            /// </summary>
            /// <param name="password">Users's Password</param>
            /// <param name="registrationResponse">Out parameter from <see cref="OpaqueServer.CreateRegistrationResponse(string, string, string, out string?)"/></param>
            /// <param name="clientRegistrationState"><see cref="StartClientRegistrationResult.ClientRegistrationState"/> from <see cref="StartRegistration(string, out StartClientRegistrationResult?)"/></param>
            /// <param name="result">A <see cref="FinishClientRegistrationResult"/> filled with data if successful</param>
            /// <returns>True if succeeded, false otherwise. Out parameter will not be null if succeeded</returns>
            public bool FinishRegistration(string password, string registrationResponse, string clientRegistrationState, out FinishClientRegistrationResult? result)
            {
                return FinishRegistration(password, registrationResponse, clientRegistrationState, "", "", out result);
            }

            /// <summary>
            /// Finishes the registration process for the client side
            /// </summary>
            /// <param name="password">Users's Password</param>
            /// <param name="registrationResponse">Out parameter from <see cref="OpaqueServer.CreateRegistrationResponse(string, string, string, out string?)"/></param>
            /// <param name="clientRegistrationState"><see cref="StartClientRegistrationResult.ClientRegistrationState"/> from <see cref="StartRegistration(string, out StartClientRegistrationResult?)"/></param>
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

                result = FunctionHelper.TryExecute(() => FinishClientRegistration(password, registrationResponse,
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

                result = FunctionHelper.TryExecute(() => StartClientLogin(password))?.GetAndRelease();
                return result != null;
            }

            /// <summary>
            /// Finishes the login process for the client
            /// </summary>
            /// <param name="clientLoginState"><see cref="StartClientLoginResult.ClientLoginState"/> from <see cref="StartLogin(string, out StartClientLoginResult?)"/></param>
            /// <param name="serverLoginResponse"><see cref="StartServerLoginResult.LoginResponse"/> from <see cref="OpaqueServer.StartLogin(string, string, string, string?, string?, string?, out StartServerLoginResult?)"/></param>
            /// <param name="password">Users's current password</param>
            /// <param name="result">A <see cref="FinishClientLoginResult"/> filled with data if successful</param>
            /// <returns>True if succeeded, false otherwise. Out parameter will not be null if succeeded</returns>
            public bool FinishLogin(string clientLoginState, string serverLoginResponse, string password, out FinishClientLoginResult? result)
            {
                return FinishLogin(clientLoginState, serverLoginResponse, password, "", "", out result);
            }

            /// <summary>
            /// Finishes the login process for the client
            /// </summary>
            /// <param name="clientLoginState"><see cref="StartClientLoginResult.ClientLoginState"/> from <see cref="StartLogin(string, out StartClientLoginResult?)"/></param>
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

                result = FunctionHelper.TryExecute(() => FinishClientLogin(clientLoginState, serverLoginResponse,
                    password, clientIdentifier ?? "", serverIdentifier ?? ""))?.GetAndRelease();

                return result != null;
            }

            protected abstract StartClientRegistrationResultHandle? StartClientRegistration(string password);

            protected abstract FinishClientRegistrationResultHandle? FinishClientRegistration(string password, string registrationResponse,
                string clientRegistrationState, string? clientIdentifier, string? serverIdentifier);

            protected abstract StartClientLoginResultHandle? StartClientLogin(string password);

            protected abstract FinishClientLoginResultHandler? FinishClientLogin(string clientLoginState, string serverLoginResponse, string password,
                string? clientIdentifier, string? serverIdentifier);
        }
    }
}
