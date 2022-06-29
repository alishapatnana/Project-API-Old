using System;
using System.Collections.Generic;
using TweetAPI.Controllers;
using TweetAPI.Models;
using TweetAPI.Repository;
using TweetAPI.Services;
using TweetAPI.Dto;
using Moq;
using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using TweetAPI.RabbitQueue;

namespace TweetAPITests
{
    public class AuthControllerTest
    {
        private AuthController _authController;
        private Mock<IAuthService> _mockAuthService;
        private Mock<IAuthRepository> _mockAuthRepo;
        private Mock<IBus> _bus;

        [SetUp]
        public void Setup()
        {
            _mockAuthRepo = new Mock<IAuthRepository>();
            _mockAuthService = new Mock<IAuthService>();
            _bus = new Mock<IBus>();
            _authController = new AuthController(_mockAuthService.Object,_bus.Object);

        }

        [Test]
        public void RegisterUserValidTest()
        {
            RegisterUser rUser = new RegisterUser() { };
            UserDto userDto = new UserDto() { };
            _mockAuthService.Setup(x => x.CreateUser(rUser)).Returns(userDto);
            _mockAuthRepo.Setup(x => x.InsertUser(userDto)).Returns(true);

            var result = _authController.Register(rUser);
            Assert.That(result, Is.InstanceOf<OkResult>());
        }

        [Test]
        public void RegisterUserInvalidTest()
        {
            RegisterUser rUser = new RegisterUser() { };
            UserDto userDto = null;
            _mockAuthService.Setup(x => x.CreateUser(rUser)).Returns(userDto);
            _mockAuthRepo.Setup(x => x.InsertUser(userDto)).Returns(false);

            var result = _authController.Register(rUser);
            Assert.That(result, Is.InstanceOf<BadRequestResult>());
        }

        [Test]
        public void LoginUserValidTest()
        {
            LoginUser lUser = new LoginUser() { LoginID = "nicole", Password = "nicole" };
            _mockAuthService.Setup(x => x.GetUser(lUser))
                .Returns("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjE2NTI1MjkyOTYsImlzcyI6Ik1haW51c2VyIiwiYXVkIjoiTWFpbnVzZXIifQ.RU7gdqJutpcjAlE_lFzHTaVVdHkokHXuGLdKEpgOXiI");
            _mockAuthRepo.Setup(x => x.Login(lUser)).Returns(true);

            var result = _authController.Login(lUser);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public void LoginUserInvalidTest()
        {
            LoginUser lUser = null;
            string token  = null;
            _mockAuthService.Setup(x => x.GetUser(lUser)).Returns(token);
            _mockAuthRepo.Setup(x => x.Login(lUser)).Returns(false);

            var result = _authController.Login(lUser);
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }
        [Test]
        public void GetAllUsersValidTest()
        {
            List<UserDto> userDtos = new List<UserDto>()
            {
                new UserDto(){ ID = "627f6119cf22e7036ac2a54d",FirstName = "Nicole",LastName = "Martin",Email = "nicole@gmail.com",
                                LoginID = "nicole",Password = "nicole",ContactNumber = "9885017232" }
            };
            _mockAuthService.Setup(x => x.GetAllUsers()).Returns(userDtos);
            _mockAuthRepo.Setup(x => x.GetAll()).Returns(userDtos);

            var result = _authController.Get();
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }
        [Test]
        public void GetAllUsersInvalidTest()
        {
            List<UserDto> userDtos = new List<UserDto>();
            _mockAuthService.Setup(x => x.GetAllUsers()).Returns(userDtos);
            _mockAuthRepo.Setup(x => x.GetAll()).Returns(userDtos);

            var result = _authController.Get();
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }
        [Test]
        public void GetByUserNameValidTest()
        {
            string loginId = "nicole";
            List<UserDto> userList = new List<UserDto>();
            UserDto userDto = new UserDto()
            {
                ID = "627f6119cf22e7036ac2a54d",
                FirstName = "Nicole",
                LastName = "Martin",
                Email = "nicole@gmail.com",
                LoginID = "nicole",
                Password = "nicole",
                ContactNumber = "9885017232"
            };
            userList.Add(userDto);

            _mockAuthService.Setup(x => x.GetByUserName(loginId)).Returns(userList);
            _mockAuthRepo.Setup(x => x.SearchByLoginId(loginId)).Returns(userList);

            var result = _authController.GetByUserName(loginId);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }
        [Test]
        public void GetByUserNameInvalidTest()
        {
            string loginId = null;
            List<UserDto> userDto = null;
            _mockAuthService.Setup(x => x.GetByUserName(loginId)).Returns(userDto);
            _mockAuthRepo.Setup(x => x.SearchByLoginId(loginId)).Returns(userDto);

            var result = _authController.GetByUserName(loginId);
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }
        [Test]
        public void ForgetPasswordValidTest()
        {
            ForgetPasswordUser fpUser = new ForgetPasswordUser()
            {
                LoginID = "nicole",
                Password = "nicole",
                ConfirmPassword = "nicole"
            };
            UserDto userDto = new UserDto()
            {
                ID = "627f6119cf22e7036ac2a54d",
                FirstName = "Nicole",
                LastName = "Martin",
                Email = "nicole@gmail.com",
                LoginID = "nicole",
                Password = "nicole",
                ContactNumber = "9885017232"
            };

            _mockAuthService.Setup(x => x.ForgetPassword(fpUser)).Returns(userDto);
            _mockAuthRepo.Setup(x => x.UpdateUser(fpUser.LoginID,userDto)).Returns(true);

            var result = _authController.ForgetPassword(fpUser);
            Assert.That(result, Is.InstanceOf<OkResult>());
        }
        [Test]
        public void ForgetPasswordInvalidTest()
        {
            ForgetPasswordUser fpUser = null;
            string loginId = null;
            UserDto userDto = null;

            _mockAuthService.Setup(x => x.ForgetPassword(fpUser)).Returns(userDto);
            _mockAuthRepo.Setup(x => x.UpdateUser(loginId, userDto)).Returns(false);

            var result = _authController.ForgetPassword(fpUser);
            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }
    }
}
