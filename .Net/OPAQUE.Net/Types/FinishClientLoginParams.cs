namespace OPAQUE.Net.Types
{
    public class FinishClientLoginParams
    {
        public string clientLoginState { get; set; }
        public string loginResponse { get; set; }
        public string password { get; set; }
        public CustomIdentifiers? identifiers { get; set; }

        public FinishClientLoginParams(string clientLoginState, string loginResponse, string password)
        {
            this.clientLoginState = clientLoginState;
            this.loginResponse = loginResponse;
            this.password = password;
        }

        public FinishClientLoginParams(string clientLoginState, string loginResponse, string password, CustomIdentifiers? identifiers) : this(clientLoginState, loginResponse, password)
        {
            this.identifiers = identifiers;
        }
    }
}
