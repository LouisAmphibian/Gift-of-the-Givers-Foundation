using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Gift_of_the_Givers_Foundation.Tests.IntegrationTests
{
    public class HttpClientIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public HttpClientIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task ApiEndpoints_ReturnCorrectContentTypeAndHeaders()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act & Assert - test multiple endpoints
            var endpoints = new[] { "/", "/Home/Index", "/Account/Login" };

            foreach (var endpoint in endpoints)
            {
                var response = await client.GetAsync(endpoint);
                response.EnsureSuccessStatusCode();

                // Test real HTTP headers
                Assert.Equal("text/html; charset=utf-8",
                    response.Content.Headers.ContentType.ToString());
                Assert.True(response.Headers.Contains("X-Content-Type-Options"));
            }
        }

        [Fact]
        public async Task FormSubmissions_WithInvalidData_ReturnValidationErrors()
        {
            // Arrange
            var client = _factory.CreateClient();
            var invalidDonation = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("DonationType", ""), // Invalid - empty
            new KeyValuePair<string, string>("Quantity", "-5"),   // Invalid - negative
            new KeyValuePair<string, string>("Location", "")      // Invalid - empty
        });

            // Act
            var response = await client.PostAsync("/Donation/Donate", invalidDonation);

            // Assert - should return validation errors (not crash)
            Assert.Equal(HttpStatusCode.OK, response.StatusCode); // Returns form with errors
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("field-validation-error", content); // MVC validation CSS class
        }
    }
}