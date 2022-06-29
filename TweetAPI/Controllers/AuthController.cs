using System;
using TweetAPI.Models;
using TweetAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TweetAPI.RabbitQueue;
namespace TweetAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(AuthController));
        private readonly IAuthService _authService;
        private readonly IBus _busControl;
        public AuthController(IAuthService authService, IBus busControl)
        {
            _authService = authService;
            _busControl = busControl;
        }
        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterUser newUser)
        {
            try
            {
                var resUser = _authService.CreateUser(newUser);
                if (resUser != null)
                {
                    _log4net.Info("User registered successfully");
                     _busControl.Send(Queue.Processing, newUser);
                    return Ok();
                }    
                else
                {
                    _log4net.Info("Error Processing the request of Register User");
                    _busControl.Send(Queue.Processing, "Error Processing the request of Register User");
                    return BadRequest();
                }      
            }
            catch (Exception e)
            {
                _log4net.Error("Unexpected error occured during registeration of user" + e.Message);
                return StatusCode(500);
            }
        }
        [HttpPost("loginId/forgetPassword")]
        public IActionResult ForgetPassword([FromBody] ForgetPasswordUser fpUser)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(400, "Invalid Request");
            }
            try
            {
                var userDetails = _authService.ForgetPassword(fpUser);
                if (userDetails != null)
                {
                    _log4net.Info("User successfully changed password");
                    _busControl.Send(Queue.Processing, fpUser);
                    return Ok();
                }
                else
                {
                    _log4net.Info("Error Processing the request of ForgetPassword");
                    _busControl.Send(Queue.Processing, "Error Processing the request of ForgetPassword");
                    return NotFound();
                }
            }
            catch (Exception e)
            {
                _log4net.Error("Unexpected error occured during the process of request" + e.Message);
                return BadRequest();
            }
        }
        [HttpGet("users/all")]
        public IActionResult Get()
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(400, "Invalid Request");
            }
            try
            {
               var userList = _authService.GetAllUsers();
                if(userList.Count > 0)
                {
                    _log4net.Info("Retriving All Users Successful");
                    _busControl.Send(Queue.Processing, userList);
                    return Ok(userList);
                }
                else
                {
                    _log4net.Info("Error Processing the request of get all users");
                    _busControl.Send(Queue.Processing, "Error Processing the request of get all users");
                    return NotFound();
                }  
            }
            catch(Exception e)
            {
                _log4net.Error("Unexpected error occured during the process of request" + e.Message);
                return BadRequest();
            }
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginUser user)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(400, "Invalid data given as credentials");
            }
            _log4net.Info("Login Initiated for user " + user);
            try
            {
                string token = _authService.GetUser(user);
                if (token == null)
                {
                    _log4net.Info("User does not exist");
                     _busControl.Send(Queue.Processing, "User does not exist");
                    return NotFound();
                }
                else
                {
                    _log4net.Info("Successfully logged In and token returned for user " + user);
                    _busControl.Send(Queue.Processing, user);
                    return Ok(new { token = token });
                }
            }
            catch (Exception e)
            {
                _log4net.Error("Unexpected error occured during login of user " + user + " with message" + e.Message);
                return StatusCode(500, "Unexpected error occured during login");
            }

        }
        [HttpGet("users/search/username")]
        public IActionResult GetByUserName(string loginId)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(400, "Invalid Request");
            }
            try
            {
                var userDetails = _authService.GetByUserName(loginId);
                if (userDetails != null)
                {
                    _log4net.Info("User Retrieved Successful");
                     _busControl.Send(Queue.Processing, loginId);
                    return Ok(userDetails);
                }
                else
                {
                    _log4net.Info("Error Processing the request of get by user name");
                    _busControl.Send(Queue.Processing, "Error Processing the request of get by user name");
                    return NotFound();
                }
            }
            catch (Exception e)
            {
                _log4net.Error("Unexpected error occured during the process of request" + e.Message);
                return BadRequest();
            }
        }
    }
}
