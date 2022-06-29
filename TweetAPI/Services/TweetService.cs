using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using TweetAPI.Models;
using TweetAPI.Repository;

namespace TweetAPI.Services
{
    public class TweetService : ITweetService
    {
        private readonly ITweetRepository _tweetRepo;
        private IConfiguration _config;
        private static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(TweetService));
        public TweetService(ITweetRepository tweetRepo, IConfiguration configuration)
        {
            _tweetRepo = tweetRepo;
            _config = configuration;
        }

        public Tweet CreateTweet(Tweet tweet)
        {
            var result = _tweetRepo.CreateTweet(tweet);
            _log4net.Info("Tweet Created Successfully");
            return result;
        }

        public bool Delete(int id)
        {
            _tweetRepo.Delete(id);
            _log4net.Info("Tweet Deleted Successfully");
            return true;
        }

        public IEnumerable<Tweet> GetAllTweets()
        {
            var result = _tweetRepo.GetAllTweets();
            if (result != null)
            {
                _log4net.Info("Retriveing all tweets Successfully");
                return result;
            }
            else
            {
                _log4net.Info("No Records to retrieve");
                return null;
            }
        }

        public IEnumerable<Tweet> GetAllTweetsOfUser(string userName)
        {
            var result = _tweetRepo.GetAllTweetsOfUser(userName);
            if (result != null)
            {
                _log4net.Info("Retrived all tweets by userName");
                return result;
            }
            else
            {
                _log4net.Info("User doesn't exists");
                return null;
            }
        }

        public bool LikeIncrement(int id)
        {
            _log4net.Info("Liked the tweet");
            _tweetRepo.LikeIncrement(id);
            return true;
        }

        public Tweet UpdateTweet(int id, Tweet tweet)
        {
            var result = _tweetRepo.UpdateTweet(id, tweet);
            if (result != null)
            {
                _log4net.Info("Updated the tweet");
                return result;
            }
            else
            {
                _log4net.Info("Tweet doesn't exists");
                return null;
            }
        }
        public Reply AddReply(Reply reply)
        {
            var result = _tweetRepo.AddReply(reply);
            if (result != null)
            {
                _log4net.Info("Added Reply to the tweet");
                return result;
            }
            else
            {
                _log4net.Error("Reply not Added");
                return null;
            }
        }
    }
}
