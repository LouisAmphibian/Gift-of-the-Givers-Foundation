using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;


namespace Gift_of_the_Givers_Foundation.Tests.IntegrationTests
{
    public class IncidentWorkflowIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public IncidentWorkflowIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task ReportIncident_CompleteWorkflow_StoresAndDisplaysIncident()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Simulate login first (if required)
            // ... login logic ...

            var incidentData = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("Title", "Critical Flood in Area X"),
            new KeyValuePair<string, string>("Description", "Heavy flooding reported in Area X, urgent assistance needed"),
            new KeyValuePair<string, string>("Location", "Area X, City Y"),
            new KeyValuePair<string, string>("IncidentType", "Flood"),
            new KeyValuePair<string, string>("Severity", "Critical")
        });

            // Act 1: Submit incident report
            var reportResponse = await client.PostAsync("/Incident/Report", incidentData);

            // Assert 1: Should redirect after successful submission
            Assert.Equal(HttpStatusCode.Redirect, reportResponse.StatusCode);

            // Act 2: Check if incident appears in the list
            var incidentsResponse = await client.GetAsync("/Incident/Index");
            incidentsResponse.EnsureSuccessStatusCode();

            var content = await incidentsResponse.Content.ReadAsStringAsync();

            // Assert 2: Verify the incident was stored and displayed
            Assert.Contains("Critical Flood in Area X", content);
            Assert.Contains("Critical", content); // Severity
        }
    }
}