﻿using AutoFixture;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;
using Moq;
using Promotions.Api.Data;
using Promotions.Api.Data.Models;

namespace Promotions.UnitTests.Data;
public class OptionsPromotionsRepositoryTests
{
    /// <summary>
    /// The current date and time according to the injected <see cref="TimeProvider"/>.
    /// </summary>
    private readonly DateTime MockedDateTime = new(2024, 07, 14, 17, 32, 01, DateTimeKind.Utc);

    private readonly Fixture _fixture = new();
    private readonly List<Promotion> _promotions = [];
    private readonly OptionsPromotionsRepository _sut;

    public OptionsPromotionsRepositoryTests()
    {
        var optionsMonitor = new Mock<IOptionsMonitor<List<Promotion>>>();
        optionsMonitor
            .SetupGet(x => x.CurrentValue)
            .Returns(() => _promotions);

        _sut = new(optionsMonitor.Object, Mock.Of<ILogger<OptionsPromotionsRepository>>(), new FakeTimeProvider(MockedDateTime));
    }

    [Fact]
    public async Task GetCurrentPromotionByCodeAsync_WhenCodeNotAvailable_ReturnsNull()
    {
        var result = await _sut.GetCurrentPromotionByCodeAsync("TEST123");
        Assert.Null(result);
    }

    [Theory]
    [InlineData("TEST123")]
    [InlineData("Test123")]
    [InlineData("test123")]
    public async Task GetCurrentPromotionByCodeAsync_WhenPromotionAvailable_ReturnsPromotion(string promotionCode)
    {
        var validPeriodPromotionGenerator = _fixture
            .Build<Promotion>()
            .With(x => x.ValidFrom, MockedDateTime.AddDays(-12))
            .With(x => x.ValidUntil, MockedDateTime.AddDays(12));

        var promotion = validPeriodPromotionGenerator
            .With(x => x.Code, "TEST123")
            .Create();
        _promotions.AddRange(validPeriodPromotionGenerator.CreateMany(10));
        _promotions.Add(promotion);
        _promotions.AddRange(validPeriodPromotionGenerator.CreateMany(10));

        var result = await _sut.GetCurrentPromotionByCodeAsync(promotionCode);
        Assert.NotNull(result);
        Assert.Same(promotion, result);
    }

    /// <remarks>
    /// Tests execute relative to <see cref="MockedDateTime"/>.
    /// </remarks>
    [Theory]
    [InlineData("2024-07-01T17:32:01Z", "2024-07-12T17:32:00Z")] // Past
    [InlineData("2024-07-01T17:32:01Z", "2024-07-14T17:32:00Z")] // Past (boundary)
    [InlineData("2024-07-14T17:32:02Z", "2024-07-20T17:32:01Z")] // Future (boundary)
    [InlineData("2024-07-18T17:32:02Z", "2024-07-20T17:32:01Z")] // Future
    public async Task GetCurrentPromotionByCodeAsync_WhenOutsideValidPeriod_ReturnsNull(string validFrom, string validTo)
    {
        var promotion = new Promotion
        {
            Code = "TEST123",
            ValidFrom = DateTime.Parse(validFrom),
            ValidUntil = DateTime.Parse(validTo)
        };
        _promotions.Add(promotion);

        var result = await _sut.GetCurrentPromotionByCodeAsync("TEST123");
        Assert.Null(result);
    }
}
