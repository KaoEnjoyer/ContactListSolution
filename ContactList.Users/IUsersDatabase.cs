using ContactList.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContactList.Users.Requests;
namespace ContactList.Users
{
    public interface IUsersDatabase
    {
        User Login(LoginRequest loginRequest);

        List<User> SelectAll(SearchUsersRequest searchUsersRequest);

        User Select(string id);

        int SelectCount(SearchUsersRequest searchUsersRequest);

        User Edit(int id, User user);

        void Delete(int  id);

        User Add(RegisterRequest registerRequest);

    }
}
