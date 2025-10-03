using AdPlatforms.Application.Abstractions;
using AdPlatforms.Application.Contracts;
using AdPlatforms.Application.Exceptions;
using AdPlatforms.Contracts;
using AdPlatforms.Controllers;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AdPlatforms.Tests.Web;

public class SearchForLocationTests
{
    private readonly Mock<IAdvertisingService> _serviceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly AdvertisingController _controller;

    public SearchForLocationTests()
    {
        _serviceMock = new Mock<IAdvertisingService>();
        _mapperMock = new Mock<IMapper>();
        _controller = new AdvertisingController(_serviceMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task ShouldReturnOk_WhenServiceReturnsData()
    {
        var location = "moscow";
        var dto = new AdvertisingDto { Name = "Ad1" };
        var response = new AdvertisingResponse { Name = "Ad1" };

        _serviceMock
            .Setup(s => s.SearchForLocationAsync(location, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AdvertisingDto> { dto });

        _mapperMock
            .Setup(m => m.Map<AdvertisingResponse>(dto))
            .Returns(response);

        var result = await _controller.SearchForLocation(location, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var actual = Assert.IsAssignableFrom<List<AdvertisingResponse>>(okResult.Value);

        Assert.Single(actual);
        Assert.Equal("Ad1", actual[0].Name);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_WhenNoFileException()
    {
        _serviceMock
            .Setup(s => s.SearchForLocationAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NoFileException());

        var result = await _controller.SearchForLocation("test", CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Файл с рекламой не был загружен", badRequest.Value?.GetType().GetProperty("message")?.GetValue(badRequest.Value));
    }

    [Fact]
    public async Task ShouldReturnBadRequest_WhenBadLocationException()
    {
        _serviceMock
            .Setup(s => s.SearchForLocationAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new BadLocationException());

        var result = await _controller.SearchForLocation("bad", CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Некорректное значение параметра location", badRequest.Value?.GetType().GetProperty("message")?.GetValue(badRequest.Value));
    }
}
