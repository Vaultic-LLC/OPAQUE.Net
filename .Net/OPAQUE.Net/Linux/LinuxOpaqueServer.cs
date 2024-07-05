using OPAQUE.Net.Linux.Handles;
using System.Runtime.InteropServices;

namespace OPAQUE.Net.Factory
{
    public static partial class OpaqueFactory
    {
        private class LinuxOpaqueServer : OpaqueServer
        {
            public LinuxOpaqueServer() { }

            protected override LinuxStringHandle CreateServerSetup()
            {
                return create_server_setup();
            }

            protected override LinuxStringHandle GetServerPublicKey(string secret)
            {
                return get_server_public_key(secret);
            }

            protected override LinuxStringHandle CreateServerRegistrationResponse(string serverSetup, string userIdentifier, string registrationRequest)
            {
                return create_server_registration_response(serverSetup, userIdentifier, registrationRequest);
            }

            protected override LinuxStartServerLoginResultHandle? StartServerLogin(string serverSetup, string startLoginRequest, string userIdentifier, string? registrationRecord, string? clientIdentity, string? serverIdentity)
            {
                return start_server_login(serverSetup, startLoginRequest, userIdentifier, registrationRecord, clientIdentity, serverIdentity);
            }

            protected override LinuxStringHandle FinishServerLogin(string serverLoginState, string finishLoginRequest)
            {
                return finish_server_login(serverLoginState, finishLoginRequest);
            }

            [DllImport("libopaque.so")]
            private static extern LinuxStringHandle create_server_setup();

            [DllImport("libopaque.so")]
            private static extern LinuxStringHandle get_server_public_key(string secret);

            [DllImport("libopaque.so")]
            private static extern LinuxStringHandle create_server_registration_response(string serverSetup, string userIdentifier, string registrationRequest);

            [DllImport("libopaque.so")]
            private static extern LinuxStartServerLoginResultHandle start_server_login(string serverSetup, string startLoginRequest,
                string userIdentifier, string? registrationRecord, string? clientIdentitiy, string? serverIdentity);

            [DllImport("libopaque.so")]
            private static extern LinuxStringHandle finish_server_login(string serverLoginState, string finishLoginRequest);
        }
    }
}
