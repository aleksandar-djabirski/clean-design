using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Xunit;

namespace Claims.Tests
{
    public class ClaimsControllerTests
    {

        private readonly WebApplicationFactory<Program> _application;
        private readonly HttpClient _client;

        public ClaimsControllerTests()
        {
            _application = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(_ => { });
            _client = _application.CreateClient();
        }

        [Fact]
        public async Task Get_Claims()
        {
            var response = await _client.GetAsync("/Claims");

            response.EnsureSuccessStatusCode();

            //TODO: Apart from ensuring 200 OK being returned, what else can be asserted?
        }

        [Fact]
        public async Task Get_Invalid_Claims()
        {
            var invalidResponse = await _client.GetAsync("/Claims/invalidId");
            Assert.Equal(HttpStatusCode.NotFound, invalidResponse.StatusCode);
        }

        //TODO: Other endpoints?
        //TODO: Check expected results, when adding, deleting, error handling?
        //role based access in the future

        //[Fact]
        //public async Task Create_Delete_Claim_Test()
        //{
        //    // Create a new claim
        //    var newClaim = new ClaimDataTransferObject
        //    {
        //        DamageCost = 500,
        //        Name = "Test",
        //        CoverId = "1",
        //        Created = DateTime.Now,
        //        Type = ClaimType.Collision
        //    };

        //    var createContent = new StringContent(
        //        JsonSerializer.Serialize(newClaim),
        //        Encoding.UTF8,
        //        "application/json");

        //    var createResponse = await _client.PostAsync("/Claims", createContent);
        //    createResponse.EnsureSuccessStatusCode();

        //    var createdClaim = JsonSerializer.Deserialize<ClaimDataTransferObject>(
        //        await createResponse.Content.ReadAsStringAsync());

        //    Assert.NotNull(createdClaim);
        //    Assert.NotNull(createdClaim.Id); // Assuming the Id is set after creation.

        //    // Delete the created claim
        //    var deleteResponse = await _client.DeleteAsync($"/Claims/{createdClaim.Id}");
        //    deleteResponse.EnsureSuccessStatusCode();

        //    // Try to retrieve the deleted claim to ensure it's gone
        //    var retrieveResponse = await _client.GetAsync($"/Claims/{createdClaim.Id}");
        //    Assert.Equal(HttpStatusCode.NotFound, retrieveResponse.StatusCode);
        //}
    }
}
