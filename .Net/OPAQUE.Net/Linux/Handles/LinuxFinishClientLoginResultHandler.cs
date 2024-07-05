using OPAQUE.Net.Base.Handles;
using System.Runtime.InteropServices;

namespace OPAQUE.Net.Linux.Handles
{
    public class LinuxFinishClientLoginResultHandler : FinishClientLoginResultHandler
    {
        protected override void FreeFinishClientLoginResult(nint handle)
        {
            free_finish_client_login_result(handle);
        }

        protected override LinuxStringHandle GetFinishClientLoginResultRequest(nint handle)
        {
            return get_finish_client_login_result_request(handle);
        }

        protected override LinuxStringHandle GetFinishClientLoginResultSessionKey(nint handle)
        {
            return get_finish_client_login_result_session_key(handle);
        }

        protected override LinuxStringHandle GetFinishClientLoginResultExportKey(nint handle)
        {
            return get_finish_client_login_result_export_key(handle);
        }

        protected override LinuxStringHandle GetFinishClientLoginResultPublicKey(nint handle)
        {
            return get_finish_client_login_result_public_key(handle);
        }

        [DllImport("libopaque.so")]
        private static extern void free_finish_client_login_result(IntPtr handle);

        [DllImport("libopaque.so")]
        private static extern LinuxStringHandle get_finish_client_login_result_request(IntPtr handle);
        [DllImport("libopaque.so")]
        private static extern LinuxStringHandle get_finish_client_login_result_session_key(IntPtr handle);
        [DllImport("libopaque.so")]
        private static extern LinuxStringHandle get_finish_client_login_result_export_key(IntPtr handle);
        [DllImport("libopaque.so")]
        private static extern LinuxStringHandle get_finish_client_login_result_public_key(IntPtr handle);
    }
}
