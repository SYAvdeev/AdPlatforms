using AdPlatforms.Application.Contracts;

namespace AdPlatforms.Application.Abstractions;

public interface IAdvertisingService
{
    Task<IEnumerable<AdvertisingDto>> SearchForLocationAsync(string location, CancellationToken cancellationToken);
    Task LoadFromFileAsync(Stream fileStream, CancellationToken cancellationToken);
}