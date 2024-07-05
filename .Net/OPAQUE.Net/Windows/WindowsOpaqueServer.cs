using OPAQUE.Net.Windows.Handles;
using System.Runtime.InteropServices;

namespace OPAQUE.Net.Factory
{
    public static partial class OpaqueFactory
    {
        private class WindowsOpaqueServer : OpaqueServer
        {
            public WindowsOpaqueServer() { }

            protected override WindowsStringHandle CreateServerSetup()
            {
                return create_server_setup();
            }

            protected override WindowsStringHandle GetServerPublicKey(string secret)
            {
                return get_server_public_key(secret);
            }

            protected override WindowsStringHandle CreateServerRegistrationResponse(string serverSetup, string userIdentifier, string registrationRequest)
            {
                return create_server_registration_response(serverSetup, userIdentifier, registrationRequest);
            }

            protected override WindowsStartServerLoginResultHandle? StartServerLogin(string serverSetup, string startLoginRequest, string userIdentifier, string? registrationRecord, string? clientIdentity, string? serverIdentity)
            {
                return start_server_login(serverSetup, startLoginRequest, userIdentifier, registrationRecord, clientIdentity, serverIdentity);
            }

            protected override WindowsStringHandle FinishServerLogin(string serverLoginState, string finishLoginRequest)
            {
                return finish_server_login(serverLoginState, finishLoginRequest);
            }

            [DllImport("opaque.dll")]
            private static extern WindowsStringHandle create_server_setup();

            [DllImport("opaque.dll")]
            private static extern WindowsStringHandle get_server_public_key(string secret);

            [DllImport("opaque.dll")]
            private static extern WindowsStringHandle create_server_registration_response(string serverSetup, string userIdentifier, string registrationRequest);

            [DllImport("opaque.dll")]
            private static extern WindowsStartServerLoginResultHandle start_server_login(string serverSetup, string startLoginRequest,
                string userIdentifier, string? registrationRecord, string? clientIdentitiy, string? serverIdentity);

            [DllImport("opaque.dll")]
            private static extern WindowsStringHandle finish_server_login(string serverLoginState, string finishLoginRequest);
        }
    }
}
