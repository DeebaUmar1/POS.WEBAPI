using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using POS.Models.DTO;
using POS.Models.Entities;
using POS.Services;
using POS.Services.UserServices;
using POS.WebApi.Controllers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace POS.Tests
{
    [TestFixture]
    public class AuthenticationControllerTests
    {
        private Mock<IMapper> _mapperMock;
        private Mock<IUserService> _userServiceMock;
        private Mock<ILogger<AuthenticationController>> _loggerMock;
        private IConfiguration _configuration;
        private AuthenticationController _controller;

        [SetUp]
        public void Setup()
        {
            _mapperMock = new Mock<IMapper>();
            _userServiceMock = new Mock<IUserService>();
            _loggerMock = new Mock<ILogger<AuthenticationController>>();
            _configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            {
                {"JWT:Secret", "ByYM000OLlMQG6VVVp1OH7Xzyr7gHuw1qvUC5dcGt3SNM"},
                {"JWT:ValidIssuer", "your_issuer"},
                {"JWT:ValidAudience", "your_audience"}
            }).Build();

            _controller = new AuthenticationController(_configuration, _userServiceMock.Object, _mapperMock.Object, _loggerMock.Object);
        }
        [Test]
        public async Task Login_ShouldReturnOk_WhenUserIsValid()
        {
            // Arrange
            var loginDto = new LoginDTO { name = "testuser", password = "Password123" };
            var user = new User { name = "testuser", role = UserRole.Cashier };
            _userServiceMock.Setup(x => x.Login(loginDto.name, loginDto.password)).ReturnsAsync(user);

            _mapperMock.Setup(m => m.Map<LoginDTO>(user)).Returns(loginDto);

            // Act
            var result = await _controller.Login(loginDto.name, loginDto.password) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
        }

        [Test]
        public async Task Register_ShouldReturnOk_WhenUserIsRegistered()
        {
            // Arrange
            var registerDto = new RegisterDTO { name = "newuser", email = "newuser@example.com", password = "Password123" };
            var user = new User { name = "newuser", Email = "newuser@example.com", role = UserRole.Cashier };
            _mapperMock.Setup(m => m.Map<User>(registerDto)).Returns(user);
            _userServiceMock.Setup(x => x.RegisterUserAsync(user)).ReturnsAsync(true);

            // Act
            var result = await _controller.Register(registerDto) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual("User registered successfully as a Cashier", result.Value);
        }
    }
}