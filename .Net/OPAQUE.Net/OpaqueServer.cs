using OPAQUE.Net.Types.Handles;
using OPAQUE.Net.Types.Results;
using System.Runtime.InteropServices;

namespace OPAQUE.Net
{
    public class OpaqueServer
    {
        public OpaqueServer() { }

        public bool CreateSetup(out string? serverSetup)
        {
            serverSetup = create_server_setup().GetAndRelease();
            return !string.IsNullOrEmpty(serverSetup);
        }

        public bool GetPublicKey(string secret, out string? publicKey)
        {
            publicKey = get_server_public_key(secret).GetAndRelease();
            return !string.IsNullOrEmpty(publicKey);
        }

        public bool CreateRegistrationResponse(string serverSetup, string userIdentifier, string registrationRequest, out string? registrationResponse)
        {
            registrationResponse = create_server_registration_response(serverSetup, userIdentifier, registrationRequest).GetAndRelease();
            return !string.IsNullOrEmpty(registrationResponse);
        }

        public bool StartLogin(string serverSetup, string startLoginRequest, string userIdentifier, 
            string? registrationRecord, string? clientIdentitiy, string? serverIdentity, out StartServerLoginResult? result)
        {
            result = start_server_login(serverSetup, startLoginRequest, userIdentifier, registrationRecord ?? "", clientIdentitiy ?? "", serverIdentity ?? "").GetAndRelease();
            return result != null;
        }

        public bool FinishLogin(string serverLoginState, string finishLoginRequest, out string? serverSessionKey)
        {
            serverSessionKey = finish_server_login(serverLoginState, finishLoginRequest).GetAndRelease();
            return !string.IsNullOrEmpty(serverSessionKey);
        }

        [DllImport("opaque.dll")]
        private static extern StringHandle create_server_setup();

        [DllImport("opaque.dll")]
        private static extern StringHandle get_server_public_key(string secret);

        [DllImport("opaque.dll")]
        private static extern StringHandle create_server_registration_response(string serverSetup, string userIdentifier, string registrationRequest);

        [DllImport("opaque.dll")]
        private static extern StartServerLoginResultHandle start_server_login(string serverSetup, string startLoginRequest, 
            string userIdentifier, string? registrationRecord, string? clientIdentitiy, string? serverIdentity);

        [DllImport("opaque.dll")]
        private static extern StringHandle finish_server_login(string serverLoginState, string finishLoginRequest);
    }
}
