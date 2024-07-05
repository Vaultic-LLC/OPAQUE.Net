using OPAQUE.Net.Base.Handles;
using System.Runtime.InteropServices;

namespace OPAQUE.Net.Windows.Handles
{
    public class WindowsFinishClientRegistrationResultHandle : FinishClientRegistrationResultHandle
    {
        protected override void FreeFinishClientRegistrationResult(nint handle)
        {
            free_finish_client_registration_result(handle);
        }

        protected override WindowsStringHandle GetFinishClientRegistrationResultRecord(nint handle)
        {
            return get_finish_client_registration_result_record(handle);
        }

        protected override WindowsStringHandle GetFinishClientRegistrationResultExportKey(nint handle)
        {
            return get_finish_client_registration_result_export_key(handle);
        }

        protected override WindowsStringHandle GetFinishClientRegistrationResultPublicKey(nint handle)
        {
            return get_finish_client_registration_result_public_key(handle);
        }

        [DllImport("opaque.dll")]
        private static extern void free_finish_client_registration_result(IntPtr handle);

        [DllImport("opaque.dll")]
        private static extern WindowsStringHandle get_finish_client_registration_result_record(IntPtr handle);
        [DllImport("opaque.dll")]
        private static extern WindowsStringHandle get_finish_client_registration_result_export_key(IntPtr handle);
        [DllImport("opaque.dll")]
        private static extern WindowsStringHandle get_finish_client_registration_result_public_key(IntPtr handle);
    }
}
