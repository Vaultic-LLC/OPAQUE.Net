using OPAQUE.Net.Base.Results;

namespace OPAQUE.Net.Base.Handles
{
    public class StartServerLoginResultHandle : BaseHandle<StartServerLoginResult>
    {
        protected override void DoRelease()
        {
            FreeStartServerLoginResult(handle);
        }

        protected override StartServerLoginResult? GetValue()
        {
            string? state = GetStartServerLoginResponseState(handle).GetAndRelease();
            string? response = GetStartServerLoginResponseResponse(handle).GetAndRelease();

            if (!string.IsNullOrEmpty(state) && !string.IsNullOrEmpty(response))
            {
                return new StartServerLoginResult(state, response);
            }

            return null;
        }

        protected virtual void FreeStartServerLoginResult(nint handle) { }

        protected virtual StringHandle GetStartServerLoginResponseState(nint handle)=> new StringHandle();
        protected virtual StringHandle GetStartServerLoginResponseResponse(nint handle)=> new StringHandle();
    }
}
