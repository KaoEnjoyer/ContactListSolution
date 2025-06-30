using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ContactList.Shared.Models
{
    public class Contact
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public int Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public DateTime? BirthDate { get; set; }

        public string Category { get; set; }
        public string? Subcategory { get; set; }

        public string? AppUserId { get; set; }
        public User? AppUser { get; set; }

    }
}
