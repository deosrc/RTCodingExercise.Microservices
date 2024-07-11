using AutoFixture;
using Catalog.API.Data;
using Catalog.API.Data.Repositories;
using Catalog.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
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
        _sut = new(_db, Mock.Of<ILogger<EFPlateRepository>>());
    }

    [Fact]
    public async Task GetPlatesAsync_ReturnsAllPlates()
    {
        var plates = _fixture.CreateMany<Plate>(100);
        await SetupPlatesDbSetAsync(plates);

        var result = await _sut.GetPlatesAsync();
        Assert.Equal(plates, result.Results);
    }

    [Fact]
    public async Task AddPlateAsync_WhenSuccessful_ReturnsSuccessAndNewPlate()
    {
        var plate = _fixture.Create<NewPlate>();

        var result = await _sut.AddPlateAsync(plate);

        Assert.True(result.IsSuccess);
        Assert.Equal("Success", result.Message);
        Assert.NotNull(result.Result);
        Assert.NotEqual(Guid.Empty, result.Result!.Id);

        Assert.Equal(plate.Registration, result.Result.Registration);
        Assert.Equal(plate.PurchasePrice, result.Result.PurchasePrice);
        Assert.Equal(plate.SalePrice, result.Result.SalePrice);
        Assert.Equal(plate.Letters, result.Result.Letters);
        Assert.Equal(plate.Numbers, result.Result.Numbers);
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
