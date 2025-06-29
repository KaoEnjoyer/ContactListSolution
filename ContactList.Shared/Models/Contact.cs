using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ContactList.Shared.Models
{
    public class Contact
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string Phone { get; set; }

        public DateTime? BirthDate { get; set; }

        public string Category { get; set; }
        public string? Subcategory { get; set; }

        public string? AppUserId { get; set; }
        public User? AppUser { get; set; }

    }
}
