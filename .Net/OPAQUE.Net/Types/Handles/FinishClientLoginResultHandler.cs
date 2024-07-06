using OPAQUE.Net.Types.Results;
using System.Runtime.InteropServices;

namespace OPAQUE.Net.Types.Handles
{
    public class FinishClientLoginResultHandler : BaseHandle<FinishClientLoginResult>
    {
        protected override void DoRelease()
        {
            free_finish_client_login_result(handle);
        }

        protected override FinishClientLoginResult? GetValue()
        {
            string? request = get_finish_client_login_result_request(handle).GetAndRelease();
            string? sessionKey = get_finish_client_login_result_session_key(handle).GetAndRelease();
            string? exportKey = get_finish_client_login_result_export_key(handle).GetAndRelease();
            string? publicKey = get_finish_client_login_result_public_key(handle).GetAndRelease();

            if (!string.IsNullOrEmpty(request) && !string.IsNullOrEmpty(sessionKey) && !string.IsNullOrEmpty(exportKey) && !string.IsNullOrEmpty(publicKey))
            {
                return new FinishClientLoginResult(request, sessionKey, exportKey, publicKey);
            }

            return null;
        }

        [DllImport("opaque")]
        private static extern void free_finish_client_login_result(IntPtr handle);

        [DllImport("opaque")]
        private static extern StringHandle get_finish_client_login_result_request(IntPtr handle);
        [DllImport("opaque")]
        private static extern StringHandle get_finish_client_login_result_session_key(IntPtr handle);
        [DllImport("opaque")]
        private static extern StringHandle get_finish_client_login_result_export_key(IntPtr handle);
        [DllImport("opaque")]
        private static extern StringHandle get_finish_client_login_result_public_key(IntPtr handle);
    }
}
