using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Gift_of_the_Givers_Foundation.Controllers;
using Gift_of_the_Givers_Foundation.Models;
using Gift_of_the_Givers_Foundation.Data;

namespace Gift_of_the_Givers_Foundation.Tests.Controllers
{
    public class DonationControllerTests
    {
        private readonly Mock<ApplicationDbContext> _mockContext;
        private readonly DonationController _donationController;

        //Constructor
        public DonationControllerTests()
        {
            //Create mock database context
            _mockContext = new Mock<ApplicationDbContext>();

            //Create real controller but inject fake database context
            _donationController = new DonationController(_mockContext.Object);

            //Mock HttpContext for the controller (controller need HTTP contect)
            var httpContext = new DefaultHttpContext();
            httpContext.Session= new Mock<ISession>().Object; //Mock session 
            _donationController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        [Fact]
        public async Task Donate_ValidDonation_ReturnsRedirect()
        {
            // Arrange - Setup test data and mock behaviors
            // Create a test donation (this is what the user would submit)
            var donation = new Donation
            {
                DonationType = "Food",
                Description = "Test donation",
                Quantity = 10,
                Location = "Test location"

                // Note : DonationID, DonatedByUserID, DonationDate, Status will be set by the controller
            };

            // Create a mock DbSet for Donations
            var mockSet = new Mock<DbSet<Donation>>();

            //Setup: When context.Donations is accessed, return mock DbSet
            _mockContext.Setup(m => m.Donations).Returns(mockSet.Object);

            //Setup: "When SaveChangesAsync is called, return 1 (1 record saved)"
            _mockContext.Setup(m => m.SaveChangesAsync(default)).ReturnsAsync(1);

            //Mock the session to simulate a logged-in user
            var httpContext = new DefaultHttpContext();
            httpContext.Session = new MockSession();
            httpContext.Session.SetInt32("UserID", 1); //Simulate user with ID 1 logged in

            //Apply the mock session to the controller
            _donationController.ControllerContext.HttpContext = httpContext;

            //ACT Phase - Call the actual  method we're testing
            var result = await _donationController.Donate(donation);


            //ASSERT 
            //ASSERT Phase - Verify the results
            //Check that the result is a redirect 
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);

            //Verify it redirects to the correct action
            Assert.Equal("Index", redirectResult.ActionName);

            //Verify it redirects to the correct controller
            Assert.Equal("Home", redirectResult.ControllerName);

            // TEST SUMMARY:
            // I tested that when a valid donation is submitted by a logged-in user,
            // the controller saves it and redirects to Home/Index

        }

        [Fact]
        public async Task Index_ReturnsViewWithDonations()
        {
            //Arrange
            //Create fake donation data that would come from the database
            var data = new List<Donation>
            {
                new Donation { DonationID = 1, DonationType = "Test 1"},
                new Donation { DonationID = 2, DonationType = "Test 2"}
            }.AsQueryable(); //Convert to IQueryable for E.F

            //Create a mock DbSet AND setup LINQ behavior 
            var mockSet = new Mock<DbSet<Donation>>();

            //SETUP the mock DbSet to work with LINQ queries:
            mockSet.As<IQueryable<Donation>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<Donation>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<Donation>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<Donation>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            //SETUP: "When context.Donations is accessed, return mock DbSet"
            _mockContext.Setup(m => m.Donations).Returns(mockSet.Object);

            // SETUP: "When Include() is called on Donations, still return our mock DbSet"
            // This handles the .Include(d => d.DonatedByUser) in your actual controller
            _mockContext.Setup(m => m.Donations.Include(It.IsAny<string>())).Returns(mockSet.Object);

            //ACT
            var result = await _donationController.Index();

            //ASSERT
            var viewResult = Assert.IsType<ViewResult>(result); //Check that result is a ViewResult
            var model = Assert.IsAssignableFrom<IEnumerable<Donation>>(viewResult.Model); //Check that the model is collection of Donations

            Assert.Equal(2, model.Count()); //Verify that we have 2 donations in the model

            // TEST SUMMARY:
            // We tested that the Index method returns a view with all donations
            // We simulated a database with 2 donations and verified they're returned
        }
    }

    //Mock session class for simulating user sessions in tests
    public class MockSession : indexer
    {
        private readonly Dictionary<string, byte[]> _storage = new Dictionary<string, byte[]>();

        public string Id =>  throw new System.NotImplementedException();
        public bool IsAvailable =>  throw new System.NotImplementedException();
        public public IEnumerable<string> Keys => _storage.Keys;

        public void Clear() => _storage.Clear();

        public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public void Remove(string key) => _storage.Remove(key);
        public void Set(string key, byte[] value) => _storage[key] = value;
        public bool TryGetValue(string key, out byte[] value) => _storage.TryGetValue(key, out value);
    }
}