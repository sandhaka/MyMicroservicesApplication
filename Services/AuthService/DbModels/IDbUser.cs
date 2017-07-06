namespace AuthService.DbModels
{
    public interface IDbUser
    {
        string id { get; set; }
        string username { get; set; }
        string password { get; set; }
        string name_full { get; set; }
        int level { get; set; }
    }
}