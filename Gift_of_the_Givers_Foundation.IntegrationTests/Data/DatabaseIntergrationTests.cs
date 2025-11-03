using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;


namespace Gift_of_the_Givers_Foundation.Tests.IntegrationTests
{
    public class DatabaseIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public DatabaseIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task DonationWorkflow_CreatesDonationAndUpdatesDatabase()
        {
            // Arrange
            var donationData = new
            {
                DonationType = "Food",
                Description = "Integration test - real database impact",
                Quantity = 10,
                Location = "Test Location",
                Unit = "kg",
                Urgency = "High"
            };

            // Act: Submit donation (this hits REAL database)
            var response = await _client.PostAsJsonAsync("/Donation/Donate", donationData);

            // Assert: Verify database was actually updated
            response.EnsureSuccessStatusCode();

            // This tests that the transaction completed and data is persistent
            var donationsResponse = await _client.GetAsync("/Donation/Index");
            donationsResponse.EnsureSuccessStatusCode();

            var content = await donationsResponse.Content.ReadAsStringAsync();
            Assert.Contains("Integration test - real database impact", content);
        }

        public Task InitializeAsync() => Task.CompletedTask;
        public Task DisposeAsync() => Task.CompletedTask;
    }
}