
using Microsoft.AspNetCore.Identity;

namespace ContactList.Shared.Models
{

    //https://www.youtube.com/watch?v=sHuuo9L3e5c
    public class User : IdentityUser
    {
        // Add your custom properties here
        public string FullName { get; set; }



        // You can override base IdentityUser properties if needed
        // public override string Email { get; set; }
    }
}
