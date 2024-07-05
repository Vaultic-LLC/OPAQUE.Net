using OPAQUE.Net.Base.Results;

namespace OPAQUE.Net.Base.Handles
{
    public class StartClientRegistrationResultHandle : BaseHandle<StartClientRegistrationResult>
    {
        protected override void DoRelease()
        {
            FreeStartClientRegistrationResult(handle);
        }

        protected override StartClientRegistrationResult? GetValue()
        {
            string? state = GetStateClientRegistrationResultState(handle).GetAndRelease();
            string? request = GetStartClientRegistrationResultRequest(handle).GetAndRelease();

            if (!string.IsNullOrEmpty(state) && !string.IsNullOrEmpty(request))
            {
                return new StartClientRegistrationResult(state, request);
            }

            return null;
        }

        protected virtual void FreeStartClientRegistrationResult(nint handle) { }

        protected virtual StringHandle GetStateClientRegistrationResultState(nint handle) => new StringHandle();
        protected virtual StringHandle GetStartClientRegistrationResultRequest(nint handle) => new StringHandle();
    }
}
