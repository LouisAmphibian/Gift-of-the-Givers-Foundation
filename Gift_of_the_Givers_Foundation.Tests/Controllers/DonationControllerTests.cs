using Microsoft.EntityFrameworkCore;
using Gift_of_the_Givers_Foundation.Data;
using Gift_of_the_Givers_Foundation.Controllers;
using Gift_of_the_Givers_Foundation.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

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
        _context.Donations.Add(new Donation { Id = 1, Amount = 100 });
        _context.SaveChanges();

        // Act
        var result = _controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<IEnumerable<Donation>>(viewResult.Model);
        Assert.Single(model);
    }
}
