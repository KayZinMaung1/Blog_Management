namespace Blog.Models
{
    public class UserConstants
    {
        public static List<UserModel> Users = new List<UserModel>() { 
            new UserModel() { UserName = "admin", Email = "admin@gmail.com", Password = "admin123", Role="Administrator"},
            new UserModel() { UserName = "kay", Email = "kay@gmail.com", Password = "kay123", Role="seller"}
        };

    }
}
