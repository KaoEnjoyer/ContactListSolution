using Microsoft.EntityFrameworkCore;
using ContactList.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ContactList.Server.Data
{
    public class ContactListDbContext : IdentityDbContext<User>
    {
        public ContactListDbContext(DbContextOptions<ContactListDbContext> options)
            : base(options)
        {
        }

        public DbSet<Contact> Contacts { get; set; } = null!;
    }
}

