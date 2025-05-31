namespace assignment_mvc_carrental.Classes
{
    public class UserRole
    {
        public int Id { get; set; }
        public string Role { get; set; } = "";



        public static UserRole AdminRole => new UserRole { Id = 1, Role = "AdminRole" };
        public static UserRole CustomerRole => new UserRole { Id = 2, Role = "CustomerRole" };
    }
}
