using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace TweetAPI.Models
{
    public class Reply
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfDefault]
        public ObjectId ID { get; set; }
        public int TweetID { get; set; }
        public string LoginID { get; set; }
        public DateTime DateTime { get; set; }
        public string Content { get; set; }
    }
}
