using OPAQUE.Net.Base.Results;

namespace OPAQUE.Net.Base.Handles
{
    public class StartClientLoginResultHandle : BaseHandle<StartClientLoginResult>
    {
        protected override void DoRelease()
        {
            FreeStateClientLoginResult(handle);
        }

        protected override StartClientLoginResult? GetValue()
        {
            string? state = GetStartClientLoginResultState(handle).GetAndRelease();
            string? request = GetStartClientLoginResultRequest(handle).GetAndRelease();

            if (!string.IsNullOrEmpty(state) && !string.IsNullOrEmpty(request))
            {
                return new StartClientLoginResult(state, request);
            }

            return null;
        }

        protected virtual void FreeStateClientLoginResult(nint handle) { }

        protected virtual StringHandle GetStartClientLoginResultState(nint handle)=> new StringHandle();
        protected virtual StringHandle GetStartClientLoginResultRequest(nint handle)=> new StringHandle();
    }
}
