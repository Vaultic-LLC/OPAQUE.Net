namespace OPAQUE.Net.Types.Results
{
    public class FinishClientRegistrationResult
    {
        public string RegistrationRecord { get; private set; }
        public string ExportKey { get; private set; }
        public string ServerStaicPublicKey { get; private set; }

        public FinishClientRegistrationResult(string registrationRecord, string exportKey, string serverStaicPublicKey)
        {
            RegistrationRecord = registrationRecord;
            ExportKey = exportKey;
            ServerStaicPublicKey = serverStaicPublicKey;
        }
    }
}
