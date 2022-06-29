using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using TweetAPI.Controllers;
using TweetAPI.Models;
using TweetAPI.RabbitQueue;
using TweetAPI.Repository;
using TweetAPI.Services;

namespace TweetAPITests
{
    public class TweetControllerTest
    {
        private TweetController _tweetController;
        private Mock<ITweetService> _mockTweetService;
        private Mock<ITweetRepository> _mockTweetRepo;
        Tweet tweet;
        private Mock<IBus> _bus;

        [SetUp]
        public void Setup()
        {
            _bus = new Mock<IBus>();
            _mockTweetRepo = new Mock<ITweetRepository>();
            _mockTweetService = new Mock<ITweetService>();
            _tweetController = new TweetController(_mockTweetService.Object,_bus.Object);
            tweet = new Tweet()
            {
                TweetID = 2,
                LoginID = "@John",
                Photo = "profiles/tweet.jpg",
                HandleName = "@Ram23",
                Message = "Hello",
                Comment = "Cool",
                Tag = "@Ram",
                Like = 10,
                Time = DateTime.Parse("2021-12-11T18:30:00.000+00:00")
            };
        }

        [Test]
        public void AddTweet_ReturnsSuccess()
        {
            _mockTweetService.Setup(x => x.CreateTweet(tweet)).Returns(tweet);
            _mockTweetRepo.Setup(x => x.CreateTweet(tweet)).Returns(tweet);

            var result = _tweetController.AddTweet(tweet);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public void AddTweet_ReturnsBadRequest()
        {
            _mockTweetService.Setup(x => x.CreateTweet(tweet)).Returns((Tweet)null);
            _mockTweetRepo.Setup(x => x.CreateTweet(tweet)).Returns((Tweet)null);

            var result = _tweetController.AddTweet(tweet);
            Assert.That(result, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public void UpdateTweet_ReturnsSuccess()
        {
            int id = 2;
            _mockTweetService.Setup(x => x.UpdateTweet(id, tweet)).Returns(tweet);
            _mockTweetRepo.Setup(x => x.UpdateTweet(id, tweet)).Returns(tweet);

            var result = _tweetController.UpdateTweet(id, tweet);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public void UpdateTweet_ReturnsBadRequest()
        {
            int id = 2;
            _mockTweetService.Setup(x => x.UpdateTweet(id, tweet)).Returns((Tweet)null);
            _mockTweetRepo.Setup(x => x.UpdateTweet(id, tweet)).Returns((Tweet)null);

            var result = _tweetController.UpdateTweet(id, tweet);
            Assert.That(result, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public void DeleteTweet_ReturnsSuccess()
        {
            int id = 2;
            _mockTweetService.Setup(x => x.Delete(id)).Returns(true);
            _mockTweetRepo.Setup(x => x.Delete(id)).Returns(true);

            var result = _tweetController.DeleteTweet(id);
            Assert.That(result, Is.InstanceOf<OkResult>());
        }

        [Test]
        public void DeleteTweet_ReturnsFailure()
        {
            int id = 2;
            _mockTweetService.Setup(x => x.Delete(id)).Returns(false);
            _mockTweetRepo.Setup(x => x.Delete(id)).Returns(false);

            var result = _tweetController.DeleteTweet(id);
            Assert.That(result, Is.InstanceOf<OkResult>());
        }

        [Test]
        public void LikeIncrement_Success()
        {
            int id = 2;
            var result = _tweetController.LikeIncrement(id) as StatusCodeResult;
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public void GetTweetsOfUser_Success()
        {
            string username = "@john";
            Tweet[] tweets = new Tweet[]
            {
                new Tweet(){ TweetID = 2,LoginID = "@John",Photo = "profiles/tweet.jpg",HandleName = "@Ram23",Message = "Hello",Comment = "Cool",Tag = "@Ram",Like = 10,Time = DateTime.Parse("2021-12-11T18:30:00.000+00:00")},
                new Tweet(){ TweetID = 2,LoginID = "@John",Photo = "profiles/tweet.jpg",HandleName = "@Ram23",Message = "Hello",Comment = "Cool",Tag = "@Ram",Like = 10,Time = DateTime.Parse("2021-12-11T18:30:00.000+00:00")}
            };
            _mockTweetService.Setup(x => x.GetAllTweetsOfUser(username)).Returns(tweets);
            _mockTweetRepo.Setup(x => x.GetAllTweetsOfUser(username)).Returns(tweets);

            var result = _tweetController.GetTweetsOfUser(username);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public void GetTweetsOfUser_Failure()
        {
            string username = "@john";
            _mockTweetService.Setup(x => x.GetAllTweetsOfUser(username)).Returns(null as IEnumerable<Tweet>);
            _mockTweetRepo.Setup(x => x.GetAllTweetsOfUser(username)).Returns(null as IEnumerable<Tweet>);

            var result = _tweetController.GetTweetsOfUser(username);
            Assert.That(result, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public void GetAllTweets_Success()
        {
            Tweet[] tweets = new Tweet[]
            {
                new Tweet(){ TweetID = 2,LoginID = "@Ram",Photo = "profiles/tweet.jpg",HandleName = "@Ram23",Message = "Hello",Comment = "Cool",Tag = "@Ram",Like = 10,Time = DateTime.Parse("2021-12-11T18:30:00.000+00:00")},
                new Tweet(){ TweetID = 2,LoginID = "@John",Photo = "profiles/tweet.jpg",HandleName = "@Ram23",Message = "Hello",Comment = "Cool",Tag = "@Ram",Like = 10,Time = DateTime.Parse("2021-12-11T18:30:00.000+00:00")}
            };
            _mockTweetService.Setup(x => x.GetAllTweets()).Returns(tweets);
            _mockTweetRepo.Setup(x => x.GetAllTweets()).Returns(tweets);

            var result = _tweetController.GetAllTweets();
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }
    }
}