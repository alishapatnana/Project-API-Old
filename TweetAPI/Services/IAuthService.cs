using System.Collections.Generic;
using TweetAPI.Dto;
using TweetAPI.Models;
namespace TweetAPI.Services
{
    public interface IAuthService
    {
        public UserDto CreateUser(RegisterUser newUser);
        public string GetUser(LoginUser userDetails);
        public UserDto ForgetPassword(ForgetPasswordUser fpUser);
        public List<UserDto> GetAllUsers();
        public List<UserDto> GetByUserName(string loginId);
    }
}
