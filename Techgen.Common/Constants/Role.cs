namespace Techgen.Models.Enum
{
    public static class Role
    {
        public const string User = "User";
        public const string Admin = "Admin";
        public const string Moderator = "Moderator";

        public const string Admins = Admin + ", " + Moderator;
    }
}
