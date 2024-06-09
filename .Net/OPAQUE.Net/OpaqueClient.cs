using OPAQUE.Net.Types;
using System.Runtime.InteropServices;

namespace OPAQUE.Net
{
    public class OpaqueClient
    {
        public OpaqueClient() { }

        public object StartClientRegistration(StartClientRegistrationParams args)
        {
            return start_client_registration(args);
        }

        public object FinishClientRegistration(FinishClientRegistrationParams args)
        {
            return finish_client_registration(args);
        }

        public object StartClientLogin(StartClientLoginParams args)
        {
            return start_client_login(args);
        }

        public object FinishClientLogin(FinishClientLoginParams args)
        {
            return finish_client_login(args);
        }

        [DllImport("opaque.dll")]
        private static extern object start_client_registration(StartClientRegistrationParams args);
        [DllImport("opaque.dll")]
        private static extern object finish_client_registration(FinishClientRegistrationParams args);
        [DllImport("opaque.dll")]
        private static extern object start_client_login(StartClientLoginParams args);
        [DllImport("opaque.dll")]
        private static extern object finish_client_login(FinishClientLoginParams args);
    }
}
