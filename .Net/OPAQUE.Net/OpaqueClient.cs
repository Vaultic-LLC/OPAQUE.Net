using OPAQUE.Net.Types.Exceptions;
using OPAQUE.Net.Types.Handles;
using OPAQUE.Net.Types.Results;
using System.Runtime.InteropServices;

namespace OPAQUE.Net
{
    public class OpaqueClient
    {
        public OpaqueClient() { }

        public bool StartRegistration(string password, out StartClientRegistrationResult? result)
        {
            StringParamIsEmptyException.ThrowIfEmpty(password, nameof(password));

            result = start_client_registration(password)?.GetAndRelease();
            return result != null;
        }

        public bool FinishRegistration(string password, string registrationResponse, string clientRegistrationState, 
            string? clientIdentifier, string? serverIdentifier, out FinishClientRegistrationResult? result)
        {
            StringParamIsEmptyException.ThrowIfEmpty(password, nameof(password));
            StringParamIsEmptyException.ThrowIfEmpty(registrationResponse, nameof(registrationResponse));
            StringParamIsEmptyException.ThrowIfEmpty(clientRegistrationState, nameof(clientRegistrationState));

            result = finish_client_registration(password, registrationResponse, clientRegistrationState, clientIdentifier ?? "", serverIdentifier ?? "").GetAndRelease();
            return result != null;
        }

        public bool StartLogin(string password, out StartClientLoginResult? result)
        {
            StringParamIsEmptyException.ThrowIfEmpty(password, nameof(password));

            result = start_client_login(password).GetAndRelease();
            return result != null;
        }

        public bool FinishLogin(string clientLoginState, string serverLoginResponse, string password, string? clientIdentifier, 
            string? serverIdentifier, out FinishClientLoginResult? result)
        {
            StringParamIsEmptyException.ThrowIfEmpty(clientLoginState, nameof(clientLoginState));
            StringParamIsEmptyException.ThrowIfEmpty(serverLoginResponse, nameof(serverLoginResponse));
            StringParamIsEmptyException.ThrowIfEmpty(password, nameof(password));

            result = finish_client_login(clientLoginState, serverLoginResponse, password, clientIdentifier ?? "", serverIdentifier ?? "")?.GetAndRelease();
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
