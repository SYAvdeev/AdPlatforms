using AdPlatforms.Application.Abstractions;
using AdPlatforms.Application.Exceptions;
using AdPlatforms.Controllers;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AdPlatforms.Tests.Web;

public class LoadFromFileAsyncTests
{
    private readonly Mock<IAdvertisingService> _serviceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly AdvertisingController _controller;

    public LoadFromFileAsyncTests()
    {
        _serviceMock = new Mock<IAdvertisingService>();
        _mapperMock = new Mock<IMapper>();
        _controller = new AdvertisingController(_serviceMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_WhenFileIsNull()
    {
        var result = await _controller.LoadFromFileAsync(null, CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Файл не был прикреплен", badRequest.Value?.GetType().GetProperty("message")?.GetValue(badRequest.Value));
    }

    [Fact]
    public async Task ShouldReturnBadRequest_WhenFileIsEmpty()
    {
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.Length).Returns(0);

        var result = await _controller.LoadFromFileAsync(fileMock.Object, CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Файл пуст", badRequest.Value?.GetType().GetProperty("message")?.GetValue(badRequest.Value));
    }

    [Fact]
    public async Task ShouldReturnOk_WhenServiceSucceeds()
    {
        var fileMock = new Mock<IFormFile>();
        var stream = new MemoryStream();
        fileMock.Setup(f => f.Length).Returns(10);
        fileMock.Setup(f => f.OpenReadStream()).Returns(stream);

        var result = await _controller.LoadFromFileAsync(fileMock.Object, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Файл обработан", okResult.Value?.GetType().GetProperty("message")?.GetValue(okResult.Value));

        _serviceMock.Verify(s => s.LoadFromFileAsync(stream, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ShouldReturnBadRequest_WhenBadFileFormatException()
    {
        var fileMock = new Mock<IFormFile>();
        var stream = new MemoryStream();
        fileMock.Setup(f => f.Length).Returns(10);
        fileMock.Setup(f => f.OpenReadStream()).Returns(stream);

        _serviceMock
            .Setup(s => s.LoadFromFileAsync(stream, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new BadFileFormatException());

        var result = await _controller.LoadFromFileAsync(fileMock.Object, CancellationToken.None);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Некорректный формат файла", badRequest.Value?.GetType().GetProperty("message")?.GetValue(badRequest.Value));
    }
}
