using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System.Collections.Generic;
using TweetAPI.Models;

namespace TweetAPI.Repository
{
    public class TweetRepository : ITweetRepository
    {
        MongoClient _client;
        MongoServer _server;
        MongoDatabase _db;

        [System.Obsolete]
        public TweetRepository()
        {
            _client = new MongoClient("mongodb://localhost:27017");
            _server = _client.GetServer();
            _db = _server.GetDatabase("TweetAppDB");
        }

        public bool Delete(int id)
        {
            var res = Query<Tweet>.EQ(e => e.TweetID, id);
            var operation = _db.GetCollection<Tweet>("TweetsData").Remove(res);
            return true;
        }

        public IEnumerable<Tweet> GetAllTweets()
        {
            var result = _db.GetCollection<Tweet>("TweetsData").FindAll();
            return result;
        }
        public IEnumerable<Tweet> GetAllTweetsOfUser(string userName)
        {
            var res = Query<Tweet>.EQ(p => p.LoginID, userName);
            return _db.GetCollection<Tweet>("TweetsData").Find(res);
        }
        public void LikeIncrement(int id)
        {
            var res = Query<Tweet>.EQ(t => t.TweetID, id);
            var tweet = _db.GetCollection<Tweet>("TweetsData").FindOne(res);
            tweet.Like++;
            var operation = Update<Tweet>.Replace(tweet);
            _db.GetCollection<Tweet>("TweetsData").Update(res, operation);
        }
        public Tweet CreateTweet(Tweet tweet)
        {
            var result = _db.GetCollection<Tweet>("TweetsData").FindAll();
            tweet.Time = System.DateTime.Now;
            tweet.Like = 0;
            tweet.TweetID = (int)(result.Count() + 1);
            _db.GetCollection<Tweet>("TweetsData").Save(tweet);
            return tweet;
        }
        public Tweet UpdateTweet(int id, Tweet tweet)
        {
            var res = Query<Tweet>.EQ(t => t.TweetID, id);
            var operation = Update<Tweet>.Replace(tweet);
            _db.GetCollection<Tweet>("TweetsData").Update(res, operation);
            var result = Query<Tweet>.EQ(p => p.TweetID, id);
            return _db.GetCollection<Tweet>("TweetsData").FindOne(res);
        }
        public Reply AddReply(Reply reply)
        {
            var result = _db.GetCollection<Reply>("TweetReplies").FindAll();
            reply.DateTime = System.DateTime.Now;
            _db.GetCollection<Reply>("TweetReplies").Save(reply);
            return reply;
        }
    }
}
