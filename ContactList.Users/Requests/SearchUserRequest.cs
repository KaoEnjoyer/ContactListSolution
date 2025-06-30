using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactList.Users.Requests
{

    public class SearchUsersRequest
    {
        public string Email { get; set; }

        public string Login { get; set; }

        public int Id { get; set; }

        public string Sort { get; set; }

        public int From { get; set; }

        public int Take { get; set; }
    }

}
