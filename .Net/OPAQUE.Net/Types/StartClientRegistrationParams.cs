namespace OPAQUE.Net.Types
{
    public class StartClientRegistrationParams
    {
        public string password { get; set; }

        public StartClientRegistrationParams(string password)
        {
            this.password = password;
        }
    }
}
