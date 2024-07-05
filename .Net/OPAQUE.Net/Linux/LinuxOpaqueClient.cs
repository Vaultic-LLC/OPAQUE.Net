using OPAQUE.Net.Linux.Handles;
using System.Runtime.InteropServices;

namespace OPAQUE.Net.Factory
{
    public static partial class OpaqueFactory
    {
        private class LinuxOpaqueClient : OpaqueClient
        {
            public LinuxOpaqueClient() { }

            protected override LinuxStartClientRegistrationResultHandle? StartClientRegistration(string password)
            {
                return start_client_registration(password);
            }

            protected override LinuxFinishClientRegistrationResultHandle? FinishClientRegistration(string password, string registrationResponse, string clientRegistrationState, string? clientIdentifier, string? serverIdentifier)
            {
                return finish_client_registration(password, registrationResponse, clientRegistrationState, clientIdentifier, serverIdentifier);
            }

            protected override LinuxStartClientLoginResultHandle? StartClientLogin(string password)
            {
                return start_client_login(password);
            }

            protected override LinuxFinishClientLoginResultHandler? FinishClientLogin(string clientLoginState, string serverLoginResponse, string password, string? clientIdentifier, string? serverIdentifier)
            {
                return finish_client_login(clientLoginState, serverLoginResponse, password, clientIdentifier, serverIdentifier);
            }

            [DllImport("libopaque.so")]
            private static extern LinuxStartClientRegistrationResultHandle? start_client_registration(string password);

            [DllImport("libopaque.so")]
            private static extern LinuxFinishClientRegistrationResultHandle finish_client_registration(string password, string registrationResponse,
                string clientRegistrationState, string? clientIdentifier, string? serverIdentifier);

            [DllImport("libopaque.so")]
            private static extern LinuxStartClientLoginResultHandle start_client_login(string password);

            [DllImport("libopaque.so")]
            private static extern LinuxFinishClientLoginResultHandler? finish_client_login(string clientLoginState, string serverLoginResponse, string password,
                string? clientIdentifier, string? serverIdentifier);
        }
    }
}
