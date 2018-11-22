using PW.DataModel.Entities;

namespace PW.ViewModels
{
    public delegate void UserStateHandler(object sender, UserEventArgs e);

    public class UserEventArgs
    {
        // Сообщение
        public string Message { get; private set; }
        public User User { get; private set; }

        public UserEventArgs(User _user, string _mes)
        {
            Message = _mes;
            User = _user;
        }
    }
}
