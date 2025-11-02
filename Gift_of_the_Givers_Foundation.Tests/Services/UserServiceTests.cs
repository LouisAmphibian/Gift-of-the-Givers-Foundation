using Xunit;
using Moq; //Mocking framework for creating fake dependencies
using Microsoft.EntityFrameworkCore;
using Gift_of_the_Givers_Foundation.Data;
using Gift_of_the_Givers_Foundation.Models;
using Gift_of_the_Givers_Foundation.Services;
using System.Threading.Tasks; // For async/await programming
using System.Linq; // For  LINQ operations (AsQueryable)

namespace Gift_of_the_Givers_Foundation.Tests;

public class UserServiceTests
{
    // Mocks - fake versions of my dependencies
    private readonly Mock<ApplicationDbContext> _mockContext; //fake database context
    private readonly UserService _userService; //The Real service I'm testing

    // Constructor
    public UserServiceTests()
    {
        //Create in-memory database options (fake database for testing)
        var options = new DbContextOptionsBuilder<ApplicationDbContext>() 
            .UseInMemoryDatabase(databaseName: "TestDatabase") // Create a temporary in-memory database
            .Options;

        //Create a mock/fake database context with the in-memory options
        _mockContext = new Mock<ApplicationDbContext>(options);

        //Create REAL UserService but inject the FAKE database context
        //This isolates the service for testing without touching a real database
        _userService = new UserService(_mockContext.Object);
    }

    [Fact]
    public async Task RegisterUserAsync_ValidUser_ReturnsUser()
    {
        // Arrange: create and initialize any objects and variables that are needed
        var user = new User
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            Role = "Donor"

            // A Note: UserID and PasswordHash will be set by the service
        };

        var password = "Password123";

        //CREATE a mock DbSet (fake database table for Users)
        var mockSet = new Mock<DbSet<User>>();
        //SETUP: "When the context.Users property is accessed, return a mock DbSet"
        _mockContext.Setup(m => m.Users).Returns(mockSet.Object);
        //SETUP: "When SaveChangesAsync is called, return 1 (simulating 1 record saved)"
        _mockContext.Setup(m => m.SaveChangesAsync(default)).ReturnsAsync(1);

        // Act: do something with the object that you are testing
        var result = await _userService.RegisterUserAsunc(user, password);

        // Assert: check that the expected behavior occurred
        Assert.NotNull(result);// Check that I got user object back(not null)
        Assert.Equal("John", result.FirstName); // Check that FirstName is set correctly
        // Verify that the password was hashed and stored correctly
        //BCrypt.Verify checks if the plain password matches the hashed version
        Assert.True(BCrypt.Net.BCrypt.Verify(password, result.PasswordHash));

        //TEST SUMMARY:
        // I tested that RegisterUserAsync successfully creates a user with hashed password
        // I used a fake database to avoid touching real data

    }


    [Fact]
    public async Task AuthenticateUserAsync_ValidCredentials_ReturnsUser()
    {
        // Arrange
        var email = "test@example.com";
        var password = "Password123";
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password); // Hash the test password


        // Create a test user with the hashed password that would exist in the database
        var user = new User
        {
            UserID = 1,
            Email = email,
            PasswordHash = passwordHash,
            FirstName = "Test",
            LastName = "User",
            Role = "Donor"
        };

        //Create fake database data - a list with the test user
        var data = new List<User> { user }.AsQueryable(); // Convert to IQueryable for EF
        // Create a mock DbSet (FAKE Users table)
        var mockSet = new Mock<DbSet<User>>();

        //SETUP the mock DbSet to behave like a real DbSet:
        //These lines make the mock DbSet work with LINQ queries
        mockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(data.Provider);
        mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(data.Expression);
        mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(data.ElementType);
        mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

        //SETUP: "When the context.Users property is accessed, return our mock DbSet with test data"
        _mockContext.Setup(m => m.Users).Returns(mockSet.Object);

        // Act
        var result = await _userService.AuthenticateUserAsync(email, password);

        // Assert
        Assert.NotNull(result); //Should return a user object
        Assert.Equal(email, result.Email); //Should return the correct user

        // TEST SUMMARY:
        //I tested that AuthenticateUserAsync finds users and verifies passwords correctly
        // We simulated a database with existing users and tested the authentication logic
    }

    [Fact]
    public async Task UserExistsAsync_ExistingEmail_ReturnsTrue()
    {
        // Arrange
        var email = "existing@example.com";
        var user = new User { Email = email };

        //Create fake database data
        var data = new List<User> { user }.AsQueryable();
        var mockSet = new Mock<DbSet<User>>();

        //SETUP mock DbSet LINQ behavior (same as previous test)
        mockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(data.Provider);
        mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(data.Expression);
        mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(data.ElementType);
        mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

        //SETUP: "When the context.Users property is accessed, return our mock DbSet with test data"
        _mockContext.Setup(m => m.Users).Returns(mockSet.Object);

        // Act
        var result = await _userService.UserExistAsync(email); 

        // Assert
        Assert.True(result); //Should return true since user exists
        
        // TEST SUMMARY:
        // I tested that UserExistsAsync correctly identifies when a user email already exists
        // This is important for preventing duplicate registrations
    }