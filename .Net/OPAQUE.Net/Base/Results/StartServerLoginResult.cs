namespace OPAQUE.Net.Base.Results
{
    public class StartServerLoginResult
    {
        public string ServerLoginState { get; private set; }
        public string LoginResponse { get; private set; }

        public StartServerLoginResult(string serverLoginState, string loginResponse)
        {
            ServerLoginState = serverLoginState;
            LoginResponse = loginResponse;
        }
    }
}
