using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Gift_of_the_Givers_Foundation.Tests.IntegrationTests
{
    public class SecurityIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public SecurityIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/Donation/Index")]
        [InlineData("/Incident/Index")]
        [InlineData("/Donation/Donate")]
        [InlineData("/Incident/Report")]
        public async Task ProtectedRoutes_WithoutAuthentication_RedirectToLogin(string protectedRoute)
        {
            // Arrange - client that doesn't automatically follow redirects
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });

            // Act - try to access protected route without logging in
            var response = await client.GetAsync(protectedRoute);

            // Assert - should redirect to login page
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Contains("/Account/Login", response.Headers.Location.OriginalString);
        }

        [Fact]
        public async Task AdminRoutes_NonAdminUser_ReturnsForbidden()
        {
            // Arrange - login as regular user (not admin)
            var client = _factory.CreateClient();
            // ... login as regular user logic ...

            // Act - try to access admin-only functionality
            var response = await client.PostAsync("/Donation/UpdateStatus?id=1&status=Approved", null);

            // Assert - should be forbidden
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
}