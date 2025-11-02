using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;
using Gift_of_the_Givers_Foundation.Controllers;
using Gift_of_the_Givers_Foundation.Models;
using Gift_of_the_Givers_Foundation.Services;

namespace Gift_of_the_Givers_Foundation.Tests.Controllers
{
    public class AccountControllerTests
    {
        // Mocks - fakes versions of my dependencies
        private readonly Mock<IUserService> _mockUserService; //fake user service
        private readonly AccountController _accountController; //The Real controller I'm testing


        // Constructor - runs before each test
        public AccountControllerTests()
        {
            //Create a mock/fake user service
            _mockUserService = new Mock<IUserService>();

            //Create the real controller, injecting the fake user service
            //This isolates the controller for testing
            _accountController = new AccountController(_mockUserService.Object);

            // Mock HttpContext for authentication
            var httpContext = new DefaultHttpContext();
            _accountController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }


        [Fact] //single test case
        public async Task Login_Post_ValidCredentials_RedirectsToHomeIndex()
        {
            // Arrange
            //Creating a fake user that would come from the database
            var user = new User
            {
                UserID = 1,
                Email = "test@example.com",
                FirstName = "John",
                LastName = "Doe",
                Role = "Donor"
            };


            //SETUP the mock: "When AuthenticateUserAsync is called with these credentials, return this user"
            _mockUserService.Setup(x => x.AuthenticateUserAsync("test@example.com", "password"))
                .ReturnsAsync(user); //This makes the fake service return my test user


            // Mock the authentication system (this handles login cookies)
            var authServiceMock = new Mock<IAuthenticationService>();
            authServiceMock
                .Setup(_ => _.SignInAsync(
                    It.IsAny<HttpContext>(), //any HttpContext
                    It.IsAny<string>(), //any authentication scheme
                    It.IsAny<ClaimsPrincipal>(), //any user principal
                    It.IsAny<AuthenticationProperties>())) //any authentication properties
                .Returns(Task.CompletedTask); //just return a completed task


            //Mock the service provider (Dependency Injection container)
            var servicesProviderMock = new Mock<IServiceProvider>();
            servicesProviderMock
                .Setup(_ => _.GetService(typeof(IAuthenticationService))) // "When asked for IAuthenticationService"
                .Returns(authServices.Object); // "return my fake authentication service"


            //Tell the controller to use my fake service provider
            _accountController.ControllerContext.HttpContext.RequestServices = servicesProviderMock.Object;

            // Act
            var result = await _accountController.Login("test@example.com", "password");

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result); //Check that the result is a RedirectToActionResult
            Assert.Equal("Index", redirectResult.ActionName); //Verify it redirects to the correct action
            Assert.Equal("Home", redirectResult.ControllerName); //Verify it redirects to the correct controller

            // TEST SUMMARY:
            // I tested that with valid credentials, the user gets redirected to Home/Index
            // I used a fake user service and fake authentication to isolate the controller logic
        }

        [Fact]
        public async Task Register_ValidData_RedirectsToHome()
        {
            // Arrange
            var user = new User
            {
                UserID = 1,
                Email = "new@example.com",
                FirstName = "Jane",
                LastName = "Doe",
                Role = "Donor"
            };


            //1. "When checking if user exists , return false (user does not exist)"
            _mockUserService.Setup(x => x.UserExistsAsync("new@example.com"))
                .ReturnsAsync(false);

            //2. "When user registering any user with password, return my test user"
            _mockUserService.Setup(x => x.RegisterUserAsync(It.IsAny<User>(), "password"))
                .ReturnsAsync(user);

            //Mock authentication (same as login test)
            var authServiceMock = new Mock<IAuthenticationService>();
            authServiceMock
                .Setup(_ => _.SignInAsync(
                    It.IsAny<HttpContext>(),
                    It.IsAny<string>(),
                    It.IsAny<ClaimsPrincipal>(),
                    It.IsAny<AuthenticationProperties>()))
                .Returns(Task.CompletedTask);

            var servicesProviderMock = new Mock<IServiceProvider>();
            servicesProviderMock
                .Setup(_ => _.GetService(typeof(IAuthenticationService)))
                .Returns(authServices.Object);

            _accountController.ControllerContext.HttpContext.RequestServices = servicesProviderMock.Object;

            // Act
            var result = await _accountController.Register("Jane", "Doe", "new@example.com", "password", "Donor");


            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Home", redirectResult.ControllerName);

            // TEST SUMMARY:
            // I tested that with valid registration data, the user gets created and redirected to Home/Index
            // I verified the user service is called correctly and authentication work
        }
    }
}