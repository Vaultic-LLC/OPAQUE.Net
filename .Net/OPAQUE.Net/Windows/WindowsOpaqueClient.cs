using OPAQUE.Net.Windows.Handles;
using System.Runtime.InteropServices;

namespace OPAQUE.Net.Factory
{
    public static partial class OpaqueFactory
    {
        private class WindowsOpaqueClient : OpaqueClient
        {
            public WindowsOpaqueClient() { }

            protected override WindowsStartClientRegistrationResultHandle? StartClientRegistration(string password)
            {
                return start_client_registration(password);
            }

            protected override WindowsFinishClientRegistrationResultHandle? FinishClientRegistration(string password, string registrationResponse, string clientRegistrationState, string? clientIdentifier, string? serverIdentifier)
            {
                return finish_client_registration(password, registrationResponse, clientRegistrationState, clientIdentifier, serverIdentifier);
            }

            protected override WindowsStartClientLoginResultHandle? StartClientLogin(string password)
            {
                return start_client_login(password);
            }

            protected override WindowsFinishClientLoginResultHandler? FinishClientLogin(string clientLoginState, string serverLoginResponse, string password, string? clientIdentifier, string? serverIdentifier)
            {
                return finish_client_login(clientLoginState, serverLoginResponse, password, clientIdentifier, serverIdentifier);
            }

            [DllImport("opaque")]
            private static extern WindowsStartClientRegistrationResultHandle? start_client_registration(string password);

            [DllImport("opaque")]
            private static extern WindowsFinishClientRegistrationResultHandle finish_client_registration(string password, string registrationResponse,
                string clientRegistrationState, string? clientIdentifier, string? serverIdentifier);

            [DllImport("opaque")]
            private static extern WindowsStartClientLoginResultHandle start_client_login(string password);

            [DllImport("opaque")]
            private static extern WindowsFinishClientLoginResultHandler? finish_client_login(string clientLoginState, string serverLoginResponse, string password,
                string? clientIdentifier, string? serverIdentifier);
        }
    }
}
