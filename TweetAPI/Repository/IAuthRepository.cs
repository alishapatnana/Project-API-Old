using System.Collections.Generic;
using TweetAPI.Dto;
using TweetAPI.Models;

namespace TweetAPI.Repository
{
    public interface IAuthRepository
    {
        public List<UserDto> GetAll();
        public UserDto GetByLoginId(string id);
        public bool InsertUser(UserDto newUser);
        public bool UpdateUser(string id, UserDto updatedBook);
        public bool DeleteUser(string id);
        public bool Login(LoginUser userDetails);
        public UserDto GetByMailId(string mailId);
        public List<UserDto> SearchByLoginId(string id);
    }
}
