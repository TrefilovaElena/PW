using PW.DataModel.Entities;
using PW.ViewModels;
using System.Collections.Generic;

namespace PW.Services
{
    public interface IUserService
    {
        User Authenticate(string email, string password);
        List<AutocomleteSelectModel> GetUserBySearchString(int currentUser, string searchStr, int count);
        (User,string) Create(User user, string password);
        User GetUserById(int userId);

    }
}