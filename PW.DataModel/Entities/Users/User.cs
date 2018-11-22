using PW.DataModel.Enums;


namespace PW.DataModel.Entities
{
    public class User : AppUser
    {
        public UserType Type { get; set; }
    }
}