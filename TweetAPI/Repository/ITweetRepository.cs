using System.Collections.Generic;
using TweetAPI.Models;

namespace TweetAPI.Repository
{
    public interface ITweetRepository
    {
        public IEnumerable<Tweet> GetAllTweets();
        public IEnumerable<Tweet> GetAllTweetsOfUser(string userName);
        public Tweet UpdateTweet(int id, Tweet tweet);
        public void LikeIncrement(int id);
        public bool Delete(int id);
        public Tweet CreateTweet(Tweet tweet);
        public Reply AddReply(Reply reply);
    }
}
