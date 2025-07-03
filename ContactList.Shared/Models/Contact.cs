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

        public Contact()
        {
            FirstName = "";
            LastName = "";
            Email = "";
            Phone = "";
            BirthDate = DateTime.Now;
            Category = "";
            Subcategory = "";
        }
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public DateTime? BirthDate { get; set; }
        public string Category { get; set; }
        public string? Subcategory { get; set; }

    }
}
