using OPAQUE.Net.Types;
using OPAQUE.Net.Types.Handles;
using System.Runtime.InteropServices;

namespace OPAQUE.Net
{
    public class OpaqueServer
    {
        public OpaqueServer() { }

        public string? SetupServer()
        {
            using StringHandle handle = create_server_setup();
            return handle.Value;
        }

        public string? GetPublicKey(string secret)
        {
            using StringHandle handle = get_server_public_key(secret);
            return handle.Value;
        }

        public string? CreateRegistrationResponse(string serverSetup, string userIdentifier, string registrationRequeset)
        {
            using StringHandle handle = create_server_registration_response(serverSetup, userIdentifier, registrationRequeset);
            return handle.Value;
        }

        public object StartServerLogin()
        {
            return start_server_login(args);
        }

        public object FinishServerLogin(FinishServerLoginParams args)
        {
            return finish_server_login(args);
        }

        [DllImport("opaque.dll")]
        private static extern StringHandle create_server_setup();

        [DllImport("opaque.dll")]
        private static extern StringHandle get_server_public_key(string secret);

        [DllImport("opaque.dll")]
        private static extern StringHandle create_server_registration_response(string serverSetup, string userIdentifier, string registrationRequest);

        [DllImport("opaque.dll")]
        private static extern object start_server_login(StartServerLoginParams args);

        [DllImport("opaque.dll")]
        private static extern object finish_server_login(FinishServerLoginParams args);
    }
}
