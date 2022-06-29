using TweetAPI.Dto;
using TweetAPI.Models;
using TweetAPI.Repository;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;

namespace TweetAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private IConfiguration _config;
        private static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(AuthService));
        public AuthService(IAuthRepository authRepository, IConfiguration configuration)
        {
            _authRepository = authRepository;
            _config = configuration;
        }

        public UserDto CreateUser(RegisterUser newUser)
        {
            var user = _authRepository.GetByLoginId(newUser.LoginID);
            var userMail = _authRepository.GetByMailId(newUser.Email);
            if (user == null && userMail == null)
            {
                if (newUser.Password == newUser.ConfirmPassword)
                {
                    var userDto = new UserDto()
                    {
                        FirstName = newUser.FirstName,
                        LastName = newUser.LastName,
                        Email = newUser.Email,
                        Password = newUser.Password,
                        LoginID = newUser.LoginID,
                        ContactNumber = newUser.ContactNumber,
                    };
                    bool loginUser = _authRepository.InsertUser(userDto);
                    if (loginUser)
                    {
                        _log4net.Info("User registered successfully");
                        return userDto;
                    }
                    else
                    {
                        _log4net.Info("Error Processing the Request");
                        return null;
                    }
                }
                else
                {
                    _log4net.Info("Password and Confirm Password are not same");
                    return null;
                }

            }
            else
            {
                _log4net.Info("Login ID or Mail Id already exists");
                return null;
            }
        }
        public UserDto ForgetPassword(ForgetPasswordUser fpUser)
        {
            var userDto = _authRepository.GetByLoginId(fpUser.LoginID);
            if (userDto != null && fpUser.Password == fpUser.ConfirmPassword)
            {
                userDto.Password = fpUser.Password;
                bool updatedUser = _authRepository.UpdateUser(fpUser.LoginID, userDto);
                if (updatedUser)
                {
                    _log4net.Info("User Details are updated successfully");
                    return userDto;
                }
                else
                {
                    _log4net.Info("Error Processing the request");
                    return null;
                }

            }
            else
            {
                _log4net.Info("Not a valid user");
                return null;
            }
        }
        public List<UserDto> GetAllUsers()
        {
            List<UserDto> userDtos = _authRepository.GetAll();
            if (userDtos.Count > 0)
            {
                _log4net.Info("Successfully retrived all Users");
                return userDtos;
            }
            else
            {
                _log4net.Info("Error Processing the Request");
                return null;
            }
        }
        public string GetUser(LoginUser userDetails)
        {
                bool result = _authRepository.Login(userDetails);
                if (!result)
                {
                    _log4net.Info("User with given credentials does not exist");
                    return null;
                }
                return GenerateJSONWebToken(userDetails);
        }
        public List<UserDto> GetByUserName(string loginId)
        {
            try
            {
                var result = _authRepository.SearchByLoginId(loginId);
                if (result.Count <= 0)
                {
                    _log4net.Info("User does not exist");
                    return null;
                }
                return result;
            }
            catch (Exception e)
            {
                _log4net.Info("Error in the processing the request" + e.Message);
                return null;
            }
        }
        private string GenerateJSONWebToken(LoginUser userInfo)
        {
            _log4net.Info("Token Generation Started");
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                null,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token).ToString();
        }
    }
}
