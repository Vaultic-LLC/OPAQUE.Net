using OPAQUE.Net.Base.Handles;
using System.Runtime.InteropServices;

namespace OPAQUE.Net.Windows.Handles
{
    public class WindowsFinishClientLoginResultHandler : FinishClientLoginResultHandler
    {
        protected override void FreeFinishClientLoginResult(nint handle)
        {
            free_finish_client_login_result(handle);
        }

        protected override WindowsStringHandle GetFinishClientLoginResultRequest(nint handle)
        {
            return get_finish_client_login_result_request(handle);
        }

        protected override WindowsStringHandle GetFinishClientLoginResultSessionKey(nint handle)
        {
            return get_finish_client_login_result_session_key(handle);
        }

        protected override WindowsStringHandle GetFinishClientLoginResultExportKey(nint handle)
        {
            return get_finish_client_login_result_export_key(handle);
        }

        protected override WindowsStringHandle GetFinishClientLoginResultPublicKey(nint handle)
        {
            return get_finish_client_login_result_public_key(handle);
        }

        [DllImport("opaque.dll")]
        private static extern void free_finish_client_login_result(IntPtr handle);

        [DllImport("opaque.dll")]
        private static extern WindowsStringHandle get_finish_client_login_result_request(IntPtr handle);
        [DllImport("opaque.dll")]
        private static extern WindowsStringHandle get_finish_client_login_result_session_key(IntPtr handle);
        [DllImport("opaque.dll")]
        private static extern WindowsStringHandle get_finish_client_login_result_export_key(IntPtr handle);
        [DllImport("opaque.dll")]
        private static extern WindowsStringHandle get_finish_client_login_result_public_key(IntPtr handle);
    }
}
