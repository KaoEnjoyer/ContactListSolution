
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ContactList.Shared.Models
{

    //https://www.youtube.com/watch?v=sHuuo9L3e5c
    public class User 
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Password { get; set; }

        public string Login { get; set; }

        public string Email { get; set; }

        public string Token { get; set; }

        public string PasswordFormat { get; set; }

    }
}
