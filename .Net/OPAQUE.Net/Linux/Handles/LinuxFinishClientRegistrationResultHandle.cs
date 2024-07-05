using OPAQUE.Net.Base.Handles;
using System.Runtime.InteropServices;

namespace OPAQUE.Net.Linux.Handles
{
    public class LinuxFinishClientRegistrationResultHandle : FinishClientRegistrationResultHandle
    {
        protected override void FreeFinishClientRegistrationResult(nint handle)
        {
            free_finish_client_registration_result(handle);
        }

        protected override LinuxStringHandle GetFinishClientRegistrationResultRecord(nint handle)
        {
            return get_finish_client_registration_result_record(handle);
        }

        protected override LinuxStringHandle GetFinishClientRegistrationResultExportKey(nint handle)
        {
            return get_finish_client_registration_result_export_key(handle);
        }

        protected override LinuxStringHandle GetFinishClientRegistrationResultPublicKey(nint handle)
        {
            return get_finish_client_registration_result_public_key(handle);
        }

        [DllImport("libopaque.so")]
        private static extern void free_finish_client_registration_result(IntPtr handle);

        [DllImport("libopaque.so")]
        private static extern LinuxStringHandle get_finish_client_registration_result_record(IntPtr handle);
        [DllImport("libopaque.so")]
        private static extern LinuxStringHandle get_finish_client_registration_result_export_key(IntPtr handle);
        [DllImport("libopaque.so")]
        private static extern LinuxStringHandle get_finish_client_registration_result_public_key(IntPtr handle);
    }
}
