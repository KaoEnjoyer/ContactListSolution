using ContactList.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactList.Users.Responses
{
    public class SearchUsersResponse
    {
        public List<User> Users { get; set; }

        public int CurrentUser { get; set; }

        public int All { get; set; }

    }
}
