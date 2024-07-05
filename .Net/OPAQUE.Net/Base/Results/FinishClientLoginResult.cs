namespace OPAQUE.Net.Base.Results
{
    public class FinishClientLoginResult
    {
        public string FinishLoginRequest { get; private set; }
        public string SessionKey { get; private set; }
        public string ExportKey { get; private set; }
        public string ServerStaticPublicKey { get; private set; }

        public FinishClientLoginResult(string finishLoginRequest, string sessionKey, string exportKey, string serverStaticPublicKey)
        {
            FinishLoginRequest = finishLoginRequest;
            SessionKey = sessionKey;
            ExportKey = exportKey;
            ServerStaticPublicKey = serverStaticPublicKey;
        }
    }
}
