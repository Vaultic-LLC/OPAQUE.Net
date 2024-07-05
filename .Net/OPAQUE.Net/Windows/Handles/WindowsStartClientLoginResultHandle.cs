using OPAQUE.Net.Base.Handles;
using System.Runtime.InteropServices;

namespace OPAQUE.Net.Windows.Handles
{
    public class WindowsStartClientLoginResultHandle : StartClientLoginResultHandle
    {
        protected override void FreeStateClientLoginResult(nint handle)
        {
            free_start_client_login_result(handle);
        }

        protected override WindowsStringHandle GetStartClientLoginResultState(nint handle)
        {
            return get_start_client_login_result_state(handle);
        }

        protected override WindowsStringHandle GetStartClientLoginResultRequest(nint handle)
        {
            return get_start_client_login_result_request(handle);
        }

        [DllImport("opaque.dll")]
        private static extern void free_start_client_login_result(IntPtr handle);

        [DllImport("opaque.dll")]
        private static extern WindowsStringHandle get_start_client_login_result_state(IntPtr handle);
        [DllImport("opaque.dll")]
        private static extern WindowsStringHandle get_start_client_login_result_request(IntPtr handle);
    }
}
