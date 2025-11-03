using Microsoft.EntityFrameworkCore;
using Gift_of_the_Givers_Foundation.Data;
using Gift_of_the_Givers_Foundation.Controllers;
using Gift_of_the_Givers_Foundation.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Http;
using Moq;


namespace Gift_of_the_Givers_Foundation.Tests.Controllers
{
    public class DonationControllerTests
    {
    private readonly DonationController _controller;
    private readonly ApplicationDbContext _context;

    public DonationControllerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "DonationTestDb")
            .Options;

        _context = new ApplicationDbContext(options);
        _controller = new DonationController(_context);
    }

    [Fact]
    public void Index_ReturnsViewWithDonations()
    {
        // Arrange
        _context.Donations.Add(new Donation
        {
            DonationID = 1,
            DonationType = "Food",
            Description = "Test food donation",
            Quantity = 100,
            Unit = "kg",
            Location = "Test Location",
            Urgency = "High",
            DonatedByUserID = 1,
            Status = "Pending"
        });
        _context.SaveChanges();

        // Act
        var result = _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<Donation>>(viewResult.Model);
        Assert.Single(model);
    }


    [Fact]
    public async Task Donate_ValidDonation_RedirectsToIndex()
    {
        // Arrange
        var donation = new Donation
        {
            DonationType = "Clothing",
            Description = "Winter clothes donation",
            Quantity = 50,
            Unit = "pieces",
            Location = "Cape Town",
            Urgency = "Medium",
            DonatedByUserID = 1,
            Status = "Pending"
        };

        // Mock HttpContext and Session
        var mockHttpContext = new Mock<HttpContext>();
        var mockSession = new Mock<ISession>();
        var userId = 1;

        // Setup session to return a user ID
        mockSession.Setup(_ => _.GetInt32(It.IsAny<string>())).Returns(userId);
        mockHttpContext.Setup(c => c.Session).Returns(mockSession.Object);

        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = mockHttpContext.Object
        };

        // Act
        var result = await _controller.Donate(donation);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("Home", redirectResult.ControllerName);
    
        // Verify donation was added to database
        var savedDonation = await _context.Donations.FirstOrDefaultAsync();
        Assert.NotNull(savedDonation);
        Assert.Equal("Clothing", savedDonation.DonationType);
    }

        [Fact]
        public async Task Donate_InvalidDonation_ReturnsViewWithErrors()
        {
            // Arrange - Create invalid donation (missing required fields)
            var donation = new Donation
            {
                // Missing required fields: DonationType, Description, Quantity, Location
                Quantity = -1 // Invalid quantity
            };

            // Mock HttpContext and Session
            var mockHttpContext = new Mock<HttpContext>();
            var mockSession = new Mock<ISession>();
            var userId = 1;

            mockSession.Setup(_ => _.GetInt32(It.IsAny<string>())).Returns(userId);
            mockHttpContext.Setup(c => c.Session).Returns(mockSession.Object);

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            // Simulate model state errors
            _controller.ModelState.AddModelError("DonationType", "Donation type is required");
            _controller.ModelState.AddModelError("Description", "Description is required");
            _controller.ModelState.AddModelError("Quantity", "Quantity must be at least 1");

            // Act - Use the correct method name "Donate" that matches your controller
            var result = await _controller.Donate(donation);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(_controller.ModelState.IsValid);
        }

        [Fact]
        public async Task Donate_WithoutUserSession_RedirectsToLogin()
        {
            // Arrange
            var donation = new Donation
            {
                DonationType = "Food",
                Description = "Test donation",
                Quantity = 10,
                Location = "Test Location"
            };

            // Mock HttpContext and Session with NO user ID (null)
            var mockHttpContext = new Mock<HttpContext>();
            var mockSession = new Mock<ISession>();

            // Setup session to return null (no user logged in)
            mockSession.Setup(_ => _.GetInt32(It.IsAny<string>())).Returns((int?)null);
            mockHttpContext.Setup(c => c.Session).Returns(mockSession.Object);

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            };

            // Act
            var result = await _controller.Donate(donation);

            // Assert - Should redirect to Login page when no user is logged in
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirectResult.ActionName);
            Assert.Equal("Account", redirectResult.ControllerName);
        }

    }
}

