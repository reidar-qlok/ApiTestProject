using ArtistApi.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq;
using System.Net.Http.Json;
using Xunit;

namespace TestProject10
{
    /*För att testa API:et utan att påverka den verkliga databasen, 
     * använder jag "mocking" eller en in-memory databas som 
     * Entity Framework Core's InMemory provider. Du själv kan skriva testfall för 
     * olika scenarier, som lyckade anrop, felaktig data, databasfel, osv.
    */
    public class UnitTest1
    {
        private readonly ArtistDbContext _mockContext;
        private readonly HttpClient _client;

        public UnitTest1()
        {
            _mockContext = new ArtistDbContext(new DbContextOptionsBuilder<ArtistDbContext>()
                .UseInMemoryDatabase("ArtistTestDb").Options);
            _client = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7146/") // Basadress för API:t
            };
        }
        [Fact]
        public async Task getitems_returnsitemsfromdatabase()
        {
            // arrange
            var expecteditems = new[]
            {
                new ArtistApi.Models.Artist { ArtistId = 1, ArtistName = "Beatles"},
                new ArtistApi.Models.Artist {ArtistId = 2, ArtistName  = "Credence"},
                new ArtistApi.Models.Artist { ArtistId = 3, ArtistName  = "The Script"},
                new ArtistApi.Models.Artist { ArtistId = 4, ArtistName  = "McFly"},
                new ArtistApi.Models.Artist { ArtistId = 5, ArtistName  = "Frank Sinatra"},
                new ArtistApi.Models.Artist { ArtistId = 6, ArtistName = "Steve Lacy"}
            };
            _mockContext.Artists.AddRange(expecteditems);
            _mockContext.SaveChanges();
            
            // act
            var response = await _client.GetAsync("/artists");

            // assert
            response.EnsureSuccessStatusCode();
            var responsestring = await response.Content.ReadAsStringAsync();
            var items = JsonConvert.DeserializeObject<List<ArtistApi.Models.Artist>>(responsestring);
            Assert.Equal(expecteditems.Length, items.Count);
        }
        [Fact]
        public async Task InsertItem_ReturnsItemFromDatabase()
        {
            var options = new DbContextOptionsBuilder<ArtistDbContext>()
           .UseInMemoryDatabase(databaseName: "TestDatabase")
           .Options;

            using (var context = new ArtistDbContext(options))
            {
                // Arrange - create a new artist
                var newArtist = new ArtistApi.Models.Artist { ArtistName = "Reidar's Singers"};
                // Act - insert the artist into the in-memory database
                context.Artists.Add(newArtist);
                context.SaveChanges();

                // Assert - retrieve the artist and verify it was inserted
                var artistFromMockDb = context.Artists.FirstOrDefault(n => n.ArtistName == "Reidar's Singers");
                Assert.NotNull(artistFromMockDb);
                Assert.Equal("Reidar's Singers", artistFromMockDb.ArtistName);
            }
        }
    }
}