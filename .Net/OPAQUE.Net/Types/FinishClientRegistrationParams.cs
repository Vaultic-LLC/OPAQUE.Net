namespace OPAQUE.Net.Types
{
    public class FinishClientRegistrationParams
    {
        public string password { get; set; }
        public string registrationResponse { get; set; }
        public string clientRegistrationState { get; set; }
        public CustomIdentifiers? identifiers { get; set; }

        public FinishClientRegistrationParams(string password, string registrationResponse, string clientRegistrationState)
        {
            this.password = password;
            this.registrationResponse = registrationResponse;
            this.clientRegistrationState = clientRegistrationState;
        }

        public FinishClientRegistrationParams(string password, string registrationResponse, string clientRegistrationState, CustomIdentifiers? identifiers) : this(password, registrationResponse, clientRegistrationState)
        {
            this.identifiers = identifiers;
        }
    }
}
