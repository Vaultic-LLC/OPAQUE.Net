using OPAQUE.Net.Base.Handles;
using System.Runtime.InteropServices;

namespace OPAQUE.Net.Windows.Handles
{
    public class WindowsStartServerLoginResultHandle : StartServerLoginResultHandle
    {
        protected override void FreeStartServerLoginResult(nint handle)
        {
            free_start_server_login_result(handle);
        }

        protected override WindowsStringHandle GetStartServerLoginResponseState(nint handle)
        {
            return get_start_server_login_response_state(handle);
        }

        protected override WindowsStringHandle GetStartServerLoginResponseResponse(nint handle)
        {
            return get_start_server_login_response_response(handle);
        }

        [DllImport("opaque")]
        private static extern void free_start_server_login_result(IntPtr handle);

        [DllImport("opaque")]
        private static extern WindowsStringHandle get_start_server_login_response_state(IntPtr handle);
        [DllImport("opaque")]
        private static extern WindowsStringHandle get_start_server_login_response_response(IntPtr handle);
    }
}
