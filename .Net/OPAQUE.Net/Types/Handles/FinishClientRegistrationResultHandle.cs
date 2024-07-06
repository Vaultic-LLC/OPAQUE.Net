using OPAQUE.Net.Types.Results;
using System.Runtime.InteropServices;

namespace OPAQUE.Net.Types.Handles
{
    internal class FinishClientRegistrationResultHandle : BaseHandle<FinishClientRegistrationResult>
    {
        protected override void DoRelease()
        {
            free_finish_client_registration_result(handle);
        }

        protected override FinishClientRegistrationResult? GetValue()
        {
            string? record = get_finish_client_registration_result_record(handle).GetAndRelease();
            string? exportKey = get_finish_client_registration_result_export_key(handle).GetAndRelease();
            string? publicKey = get_finish_client_registration_result_public_key(handle).GetAndRelease();

            if (!string.IsNullOrEmpty(record) && !string.IsNullOrEmpty(exportKey) && !string.IsNullOrEmpty(publicKey))
            {
                return new FinishClientRegistrationResult(record, exportKey, publicKey);
            }

            return null;
        }

        [DllImport("opaque")]
        private static extern void free_finish_client_registration_result(IntPtr handle);

        [DllImport("opaque")]
        private static extern StringHandle get_finish_client_registration_result_record(IntPtr handle);
        [DllImport("opaque")]
        private static extern StringHandle get_finish_client_registration_result_export_key(IntPtr handle);
        [DllImport("opaque")]
        private static extern StringHandle get_finish_client_registration_result_public_key(IntPtr handle);
    }
}
