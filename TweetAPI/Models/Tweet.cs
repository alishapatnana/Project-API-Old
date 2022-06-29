using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace TweetAPI.Models
{
    public class Tweet
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfDefault]
        public ObjectId Id { get; set; }
        public int TweetID { get; set; }
        public string LoginID { get; set; }
        public string Photo { get; set; }
        public string HandleName { get; set; }
        public string Message { get; set; }
        public string Tag { get; set; }
        public int? Like { get; set; }
        public string Comment { get; set; }
        public DateTime Time { get; set; }
    }
}
