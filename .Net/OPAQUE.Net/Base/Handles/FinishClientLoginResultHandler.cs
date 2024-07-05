using OPAQUE.Net.Base.Results;

namespace OPAQUE.Net.Base.Handles
{
    public class FinishClientLoginResultHandler : BaseHandle<FinishClientLoginResult>
    {
        protected override void DoRelease()
        {
            FreeFinishClientLoginResult(handle);
        }

        protected override FinishClientLoginResult? GetValue()
        {
            string? request = GetFinishClientLoginResultRequest(handle).GetAndRelease();
            string? sessionKey = GetFinishClientLoginResultSessionKey(handle).GetAndRelease();
            string? exportKey = GetFinishClientLoginResultExportKey(handle).GetAndRelease();
            string? publicKey = GetFinishClientLoginResultPublicKey(handle).GetAndRelease();

            if (!string.IsNullOrEmpty(request) && !string.IsNullOrEmpty(sessionKey) && !string.IsNullOrEmpty(exportKey) && !string.IsNullOrEmpty(publicKey))
            {
                return new FinishClientLoginResult(request, sessionKey, exportKey, publicKey);
            }

            return null;
        }

        protected virtual void FreeFinishClientLoginResult(nint handle) { }

        protected virtual StringHandle GetFinishClientLoginResultRequest(nint handle) => new StringHandle();
        protected virtual StringHandle GetFinishClientLoginResultSessionKey(nint handle) => new StringHandle();
        protected virtual StringHandle GetFinishClientLoginResultExportKey(nint handle) => new StringHandle();
        protected virtual StringHandle GetFinishClientLoginResultPublicKey(nint handle) => new StringHandle();
    }
}
