using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TweetAPI.Dto;
using TweetAPI.Models;

namespace TweetAPI.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IMongoCollection<UserDto> _userCollection;
        public AuthRepository(IOptions<TweetAppDatabaseSettings> tweetAppDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                tweetAppDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                tweetAppDatabaseSettings.Value.DatabaseName);

            _userCollection = mongoDatabase.GetCollection<UserDto>(
                tweetAppDatabaseSettings.Value.UserCollectionName);
        }

        public List<UserDto> GetAll() => _userCollection.Find(_ => true).ToList();

        public List<UserDto> SearchByLoginId(string id)
        {
            return _userCollection.Find(
                new BsonDocument { { "LoginID", new BsonDocument { { "$regex", id }, { "$options", "i" } } } }).ToList();
        }
        public UserDto GetByLoginId(string id)
        {
            var userDto = _userCollection.AsQueryable<UserDto>().FirstOrDefault(x => x.LoginID == id);
            return userDto;
        }
        public UserDto GetByMailId(string mailId)
        {
            var userDto = _userCollection.AsQueryable<UserDto>().FirstOrDefault(x => x.Email == mailId);
            return userDto;
        }
        public bool InsertUser(UserDto newUser)
        {
            _userCollection.InsertOne(newUser);
            return true;
        }
        public bool UpdateUser(string id, UserDto updatedBook)
        {
            var userDto = _userCollection.AsQueryable<UserDto>().FirstOrDefault(x => x.LoginID == id);
            if (userDto != null)
            {
                userDto.FirstName = updatedBook.FirstName;
                userDto.LastName = updatedBook.LastName;
                userDto.Password = updatedBook.Password;
                userDto.Email = updatedBook.Email;
                userDto.ContactNumber = updatedBook.ContactNumber;
                _userCollection.ReplaceOne(x => x.LoginID == id, userDto);
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool DeleteUser(string id)
        {
            var userDto = _userCollection.AsQueryable<UserDto>().FirstOrDefault(x => x.LoginID == id);
            if (userDto != null)
            {
                _userCollection.DeleteOne(x => x.LoginID == id);
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool Login(LoginUser userDetails)
        {
            var userDto = _userCollection.AsQueryable<UserDto>().FirstOrDefault(x => x.LoginID == userDetails.LoginID);
            if (userDto != null && userDto.LoginID == userDetails.LoginID && userDto.Password == userDetails.Password)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
