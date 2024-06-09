namespace OPAQUE.Net.Types
{
    public class FinishServerLoginParams
    {
        public string serverLoginState {  get; set; }
        public string finishLoginRequest { get; set; }

        public FinishServerLoginParams(string slr, string flr)
        {
            serverLoginState = slr;
            finishLoginRequest = flr;
        }
    }
}
