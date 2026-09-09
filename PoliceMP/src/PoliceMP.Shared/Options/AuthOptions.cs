namespace PoliceMP.Shared.Options
{
    public class AuthOptions
    {
        public int MaxLoginAttempts { get; set; }
        public int MinPasswordLength { get; set; }
        public int MaxPasswordLength { get; set; }
        public int MinUsernameLength { get; set; }
        public int MaxUsernameLength { get; set; }
    }
}