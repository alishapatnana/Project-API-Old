using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TweetAPI.Models;
using TweetAPI.RabbitQueue;
using TweetAPI.Services;

namespace TweetAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TweetController : ControllerBase
    {
        private readonly ITweetService _tweetService;
        private static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(TweetController));
        private readonly IBus _busControl;
        public TweetController(ITweetService tweetService, IBus busControl)
        {
            _tweetService = tweetService;
            _busControl = busControl;
        }

        [HttpGet("all")]
        public IActionResult GetAllTweets()
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(400);
            }
            else
            {
                var tweetsData = _tweetService.GetAllTweets();
                if (tweetsData != null)
                {
                    _log4net.Info("Retrived all tweets");
                    _busControl.Send(Queue.Processing, tweetsData);
                    return Ok(tweetsData);
                }
                else
                {
                    _log4net.Info("Error Procesing the Request");
                    _busControl.Send(Queue.Processing, "Error Procesing the Request");
                    return BadRequest();
                }
            }
        }

        [HttpGet("username")]
        public IActionResult GetTweetsOfUser(string username)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(400);
            }
            else
            {
                var tweetsData = _tweetService.GetAllTweetsOfUser(username);
                if (tweetsData != null)
                {
                    _log4net.Info("Get Tweets by User Name");
                    _busControl.Send(Queue.Processing, username);
                    return Ok(tweetsData);
                }
                else
                {
                    _log4net.Info("Error Processing the Request");
                    _busControl.Send(Queue.Processing, "Error Processing the Request");
                    return BadRequest();
                }
            }
        }

        [HttpPut("update")]
        public IActionResult UpdateTweet(int id, [FromBody] Tweet tweets)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(400);
            }
            else
            {
                var tweet = _tweetService.UpdateTweet(id, tweets);
                if (tweet != null)
                {
                    _log4net.Info("Updated Tweet");
                    _busControl.Send(Queue.Processing, tweets);
                    return Ok(tweet);
                }
                else
                {
                    _log4net.Info("Error Processing the Request");
                    _busControl.Send(Queue.Processing, "Error Processing the Request");
                    return BadRequest();
                }
            }
        }

        [HttpDelete("Delete")]
        public IActionResult DeleteTweet(int id)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(400);
            }
            else
            {
                _tweetService.Delete(id);
                _log4net.Info("Deleted Tweet");
                _busControl.Send(Queue.Processing, id);
                return Ok();
            }

        }

        [HttpPost("Like")]
        public IActionResult LikeIncrement(int id)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(400);
            }
            else
            {
                _tweetService.LikeIncrement(id);
                _log4net.Info("Liked Tweet");
                _busControl.Send(Queue.Processing, id);
                return Ok();
            }
        }

        [HttpPost("add")]
        public IActionResult AddTweet(Tweet tweet)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(400);
            }
            else
            {
                var result = _tweetService.CreateTweet(tweet);
                if (result != null)
                {
                    _log4net.Info("Added Tweet");
                    _busControl.Send(Queue.Processing, tweet);
                    return Ok(result);
                }
                else
                {
                    _log4net.Info("Error Processing the Request");
                    _busControl.Send(Queue.Processing, "Error Processing the Request");
                    return BadRequest();
                }
            }
        }
        [HttpPost("addReply")]
        public IActionResult AddReply(Reply reply)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(400);
            }
            else
            {
                var result = _tweetService.AddReply(reply);
                if (result != null)
                {
                    _log4net.Info("Added Reply");
                    _busControl.Send(Queue.Processing, reply);
                    return Ok(result);
                }
                else
                {
                    _log4net.Info("Error Processing the Request");
                    _busControl.Send(Queue.Processing, "Error Processing the Request");
                    return BadRequest();
                }
            }
        }
    }
}
