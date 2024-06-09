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
            result = start_client_registration(password)?.GetAndRelease();
            return result != null;
        }

        public bool FinishRegistration(string password, string registrationResponse, string clientRegistrationState, 
            string? clientIdentifier, string? serverIdentifier, out FinishClientRegistrationResult? result)
        {
            result = finish_client_registration(password, registrationResponse, clientRegistrationState, clientIdentifier, serverIdentifier).GetAndRelease();
            return result != null;
        }

        public bool StartLogin(string password, out StartClientLoginResult? result)
        {
            result = start_client_login(password).GetAndRelease();
            return result != null;
        }

        public bool FinishLogin(string clientLoginState, string loginResponse, string password, string? clientIdentifier, 
            string? serverIdentifier, out FinishClientLoginResult? result)
        {
            result = finish_client_login(clientLoginState, loginResponse, password, clientIdentifier, serverIdentifier)?.GetAndRelease();
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
        private static extern FinishClientLoginResultHandler? finish_client_login(string clientLoginState, string loginResponse, string password, 
            string? clientIdentifier, string? serverIdentifier);
    }
}
