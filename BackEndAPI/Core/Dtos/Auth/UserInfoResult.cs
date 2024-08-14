namespace BackEndAPI.Core.Dtos.Auth
{
    public class UserInfoResult
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string Lastname { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public DateTime CreateAt { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}
