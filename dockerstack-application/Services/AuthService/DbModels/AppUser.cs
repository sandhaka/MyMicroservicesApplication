namespace AuthService.DbModels
{
    public class AppUser
    {
        public string id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string name_full { get; set; }
        public int level { get; set; }
    }
}