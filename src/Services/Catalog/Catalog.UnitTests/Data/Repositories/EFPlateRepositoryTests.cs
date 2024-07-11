using AutoFixture;
using Catalog.API.Data;
using Catalog.API.Data.Repositories;
using Catalog.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
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
        _db.Database.EnsureDeleted();
        _sut = new(_db, Mock.Of<ILogger<EFPlateRepository>>());
    }

    [Fact]
    public async Task GetPlatesAsync_WithDefaultPaging_ReturnsFirstPage()
    {
        var plates = CreateIncrementalPlates(90);
        await SetupPlatesDbSetAsync(plates);

        var result = await _sut.GetPlatesAsync();

        var pagingOptions = new PagingOptions();
        Assert.Equal(new PageInfo(pagingOptions, true), result.Paging);
        Assert.Equal(PagingOptions.DefaultItemsPerPage, result.Results.Count());

        var expectedPlates = plates.Take(20).ToList();
        Assert.Equal(expectedPlates, result.Results);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    public async Task GetPlatesAsync_WhenSubsequentPage_ReturnsCorrectPlates(int pageNumber)
    {
        var plates = CreateIncrementalPlates(90);
        await SetupPlatesDbSetAsync(plates);

        var pagingOptions = new PagingOptions()
        {
            Page = pageNumber
        };
        var result = await _sut.GetPlatesAsync(pagingOptions);

        Assert.Equal(new PageInfo(pagingOptions, true), result.Paging);
        Assert.Equal(pagingOptions.ItemsPerPage, result.Results.Count());

        var expectedPlates = plates
            .Skip((pageNumber - 1) * PagingOptions.DefaultItemsPerPage)
            .Take(20)
            .ToList();
        Assert.Equal(expectedPlates, result.Results);
    }

    [Fact]
    public async Task GetPlatesAsync_WhenLastPage_ReturnsCorrectResult()
    {
        var plates = CreateIncrementalPlates(90);
        await SetupPlatesDbSetAsync(plates);

        var pagingOptions = new PagingOptions()
        {
            Page = 5
        };
        var result = await _sut.GetPlatesAsync(pagingOptions);

        Assert.Equal(new PageInfo(pagingOptions, false), result.Paging);
        Assert.Equal(10, result.Results.Count());

        var expectedPlates = plates
            .Skip(80)
            .Take(10)
            .ToList();
        Assert.Equal(expectedPlates, result.Results);
    }

    [Fact]
    public async Task GetPlatesAsync_WhenSinglePage_ReturnsCorrectResult()
    {
        var plates = CreateIncrementalPlates(10);
        await SetupPlatesDbSetAsync(plates);

        var pagingOptions = new PagingOptions()
        {
            Page = 1
        };
        var result = await _sut.GetPlatesAsync(pagingOptions);

        Assert.Equal(new PageInfo(pagingOptions, false), result.Paging);
        Assert.Equal(10, result.Results.Count());

        var expectedPlates = plates.ToList();
        Assert.Equal(expectedPlates, result.Results);
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

    private IEnumerable<Plate> CreateIncrementalPlates(int count)
    {
        var plates = new List<Plate>();
        foreach (var i in Enumerable.Range(1, count))
        {
            var p = _fixture.Build<Plate>()
                .With(x => x.Id, Guid.Parse(string.Format("00000000-0000-0000-0000-{0:D12}", i)))
                .With(x => x.Registration, string.Format("TEST{0}", i))
                .With(x => x.Numbers, i)
                .Create();
            plates.Add(p);
        }
        return plates;
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
