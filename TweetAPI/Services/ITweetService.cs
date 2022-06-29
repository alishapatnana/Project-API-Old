using System.Collections.Generic;
using TweetAPI.Models;

namespace TweetAPI.Services
{
    public interface ITweetService
    {
        public IEnumerable<Tweet> GetAllTweets();
        public IEnumerable<Tweet> GetAllTweetsOfUser(string userName);
        public Tweet UpdateTweet(int id, Tweet tweet);
        public bool Delete(int id);
        public bool LikeIncrement(int id);
        public Tweet CreateTweet(Tweet tweet);
        public Reply AddReply(Reply reply);
    }
}
