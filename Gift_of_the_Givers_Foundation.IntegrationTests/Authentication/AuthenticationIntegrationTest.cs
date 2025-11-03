using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Gift_of_the_Givers_Foundation.Tests.IntegrationTests
{
    public class AuthenticationIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public AuthenticationIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task FullLoginFlow_ValidCredentials_RedirectsToHomeAndMaintainsSession()
        {
            // Arrange
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            // Act 1: Login
            var loginData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("email", "test@example.com"),
                new KeyValuePair<string, string>("password", "Password123!")
            });

            var loginResponse = await client.PostAsync("/Account/Login", loginData);

            // Assert 1: Should redirect after successful login
            Assert.Equal(HttpStatusCode.Redirect, loginResponse.StatusCode);

            // Follow redirect
            client = _factory.CreateClient(); // New client that follows redirects
            var homeResponse = await client.GetAsync("/Home/Index");
            homeResponse.EnsureSuccessStatusCode();

            // Act 2: Try to access protected route with maintained session
            var donationResponse = await client.GetAsync("/Donation/Donate");

            // Assert 2: Should have access (not redirect to login)
            Assert.Equal(HttpStatusCode.OK, donationResponse.StatusCode);
        }
    }
}