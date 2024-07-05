using OPAQUE.Net.Base.Results;

namespace OPAQUE.Net.Base.Handles
{
    public class FinishClientRegistrationResultHandle : BaseHandle<FinishClientRegistrationResult>
    {
        protected override void DoRelease()
        {
            FreeFinishClientRegistrationResult(handle);
        }

        protected override FinishClientRegistrationResult? GetValue()
        {
            string? record = GetFinishClientRegistrationResultRecord(handle).GetAndRelease();
            string? exportKey = GetFinishClientRegistrationResultExportKey(handle).GetAndRelease();
            string? publicKey = GetFinishClientRegistrationResultPublicKey(handle).GetAndRelease();

            if (!string.IsNullOrEmpty(record) && !string.IsNullOrEmpty(exportKey) && !string.IsNullOrEmpty(publicKey))
            {
                return new FinishClientRegistrationResult(record, exportKey, publicKey);
            }

            return null;
        }

        protected virtual void FreeFinishClientRegistrationResult(nint handle) { }

        protected virtual StringHandle GetFinishClientRegistrationResultRecord(nint handle) => new StringHandle();
        protected virtual StringHandle GetFinishClientRegistrationResultExportKey(nint handle)=> new StringHandle();
        protected virtual StringHandle GetFinishClientRegistrationResultPublicKey(nint handle)=> new StringHandle();
    }
}
