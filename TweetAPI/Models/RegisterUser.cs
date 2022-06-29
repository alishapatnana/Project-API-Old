using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TweetAPI.Models
{
    public class RegisterUser
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfDefault]
        public string? ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string LoginID { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string ContactNumber { get; set; }
    }
}
