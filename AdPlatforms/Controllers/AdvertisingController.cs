using AdPlatforms.Application.Abstractions;
using AdPlatforms.Application.Exceptions;
using AdPlatforms.Contracts;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AdPlatforms.Controllers;

public class AdvertisingController(IAdvertisingService _advertisingService, IMapper _mapper) : Controller
{
    /// <summary>
    /// Метод поиска списка рекламных площадок для заданной локации
    /// </summary>
    [HttpGet("search-for-location")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<AdvertisingResponse>>> SearchForLocation(string location, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _advertisingService.SearchForLocationAsync(location, cancellationToken);
            return Ok(result.Select(_mapper.Map<AdvertisingResponse>).ToList());
        }
        catch (NoFileException)
        {
            return BadRequest(new { message = "Файл с рекламой не был загружен" });
        }
        catch (BadLocationException)
        {
            return BadRequest(new { message = "Некорректное значение параметра location" });
        }
    }

    /// <summary>
    /// Метод загрузки рекламных площадок из файла
    /// </summary>
    [HttpPost("load-from-file")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> LoadFromFileAsync(IFormFile? file, CancellationToken cancellationToken)
    {
        if (file == null)
        {
            return BadRequest(new { message = "Файл не был прикреплен" });
        }

        if (file.Length == 0)
        {
            return BadRequest(new { message = "Файл пуст" });
        }
        
        try
        {
            await _advertisingService.LoadFromFileAsync(file.OpenReadStream(), cancellationToken);
            return Ok(new { message = "Файл обработан" });
        }
        catch (BadFileFormatException)
        {
            return BadRequest(new { message = "Некорректный формат файла" });
        }
    }
}