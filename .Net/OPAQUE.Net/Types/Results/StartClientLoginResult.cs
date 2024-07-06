namespace OPAQUE.Net.Types.Results
{
    public class StartClientLoginResult
    {
        public string ClientLoginState { get; private set; }
        public string StartLoginRequest { get; private set; }

        public StartClientLoginResult(string clientLoginState, string startLoginRequest)
        {
            ClientLoginState = clientLoginState;
            StartLoginRequest = startLoginRequest;
        }
    }
}
