using AutoFixture;
using Catalog.API.Data;
using Catalog.API.Data.Repositories;
using Catalog.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Catalog.UnitTests.Data.Repositories;
public class EFPlateRepositoryTests : IDisposable
{
    private readonly Fixture _fixture = new();
    private readonly ApplicationDbContext _db;
    private readonly EFPlateRepository _sut;

    public EFPlateRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "Test1")
            .Options;
        _db = new(options);
        _sut = new(_db);
    }

    [Fact]
    public async Task GetPlates_ReturnsAllPlates()
    {
        var plates = _fixture.CreateMany<Plate>(100);
        await SetupPlatesDbSetAsync(plates);

        var result = await _sut.GetPlatesAsync();
        Assert.Equal(plates, result);
    }

    private async Task SetupPlatesDbSetAsync(IEnumerable<Plate> plates)
    {
        await _db.Plates.AddRangeAsync(plates);
        await _db.SaveChangesAsync();
    }

    public void Dispose()
    {
        _db.Dispose();
    }
}
