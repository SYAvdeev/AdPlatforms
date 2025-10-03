using AdPlatforms.Application.Abstractions;
using AdPlatforms.Application.Contracts;
using AdPlatforms.Application.Exceptions;
using AdPlatforms.Domain.Entities;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;

namespace AdPlatforms.Application.Services;

public class AdvertisingService(
    ITrieCacheBuilder<string, Advertising> _cacheBuilder,
    IMemoryCache _memoryCache, 
    IMapper _mapper)
    : IAdvertisingService
{
    private ITrieCache<string, Advertising> _trieCache;
    
    public async Task<IEnumerable<AdvertisingDto>> SearchForLocationAsync(
        string location, 
        CancellationToken cancellationToken)
    {
        if (_trieCache == null)
        {
            throw new NoFileException();
        }

        if (_memoryCache.TryGetValue(location, out IEnumerable<AdvertisingDto>? advertisingDtos))
        {
            return advertisingDtos!;
        }
        
        var children = location.Split('/', StringSplitOptions.RemoveEmptyEntries)
            .Select(l => l.Trim())
            .Where(s => !string.IsNullOrEmpty(s))
            .ToList();

        if (children.Count == 0)
        {
            throw new BadLocationException();
        }

        var advertisings = await _trieCache.GetAsync(
            children, 
            cancellationToken);
        
        advertisingDtos = advertisings.Select(_mapper.Map<AdvertisingDto>).ToList();
        _memoryCache.Set(location, advertisingDtos, TimeSpan.FromMinutes(60));
        
        return advertisingDtos;
    }

    public async Task LoadFromFileAsync(Stream fileStream, CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(fileStream);
        
        _cacheBuilder.StartNewBuild();

        string? line = await reader.ReadLineAsync(cancellationToken);
        while (line != null)
        {
            var parts = line.Split(":", 2);
            if (parts.Length != 2)
            {
                throw new BadFileFormatException();
            }

            var name = parts[0].Trim();
            var locations = parts[1]
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(l => l.Trim())
                .ToList();

            if (string.IsNullOrEmpty(name) || locations.Count == 0 || locations.Any(string.IsNullOrEmpty))
            {
                throw new BadFileFormatException();
            }

            foreach (var location in locations)
            {
                var children = location.Split('/', StringSplitOptions.RemoveEmptyEntries)
                    .Select(l => l.Trim())
                    .Where(s => !string.IsNullOrEmpty(s))
                    .ToList();

                if (children.Count == 0)
                {
                    throw new BadFileFormatException();
                }

                var advertising = new Advertising
                {
                    Location = location,
                    Name = name
                };

                await _cacheBuilder.AddAsync(children, advertising, cancellationToken);
                _memoryCache.Remove(location);
            }
            
            line = await reader.ReadLineAsync(cancellationToken);
        }
        
        _trieCache = _cacheBuilder.Build();
    }
}