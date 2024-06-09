namespace OPAQUE.Net.Types
{
    public class StartClientLoginParams
    {
        public string password { get; set; }
        
        public StartClientLoginParams(string password)
        {
            this.password = password;
        }
    }
}
