namespace OPAQUE.Net.Types.Results
{
    public class StartClientRegistrationResult
    {
        public string ClientRegistrationState { get; private set; }
        public string RegistrationRequest { get; private set; }

        public StartClientRegistrationResult(string clientRegistrationState, string registrationRequest)
        {
            ClientRegistrationState = clientRegistrationState;
            RegistrationRequest = registrationRequest;
        }
    }
}
