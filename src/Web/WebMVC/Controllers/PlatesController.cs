﻿using RTCodingExercise.Microservices.Services.Catalog;

namespace RTCodingExercise.Microservices.Controllers;
public class PlatesController : Controller
{
    private readonly ICatalogService _catalogService;

    public PlatesController(ICatalogService catalogService)
    {
        _catalogService = catalogService;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken = default)
    {
        var plates = await _catalogService.GetPlatesAsync(cancellationToken);
        return View(plates);
    }

    [HttpGet]
    public IActionResult Add()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Add(NewPlate plate, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return View(plate);
        }

        var result = await _catalogService.AddPlateAsync(plate, cancellationToken);
        if (result.IsSuccess)
        {
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError(string.Empty, result.Message);
        return View(plate);
    }
}
