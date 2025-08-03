using cabapi.DTOs;
using cabapi.Models;
using Compunet.YoloSharp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.Fonts;

namespace cabapi.Controllers;

[ApiController]
[Route("api/inferencias")]
public class InferenciasController : ControllerBase
{
    private readonly CABDB _db;
    private readonly YoloPredictor _yolo;
    private readonly string _modelPath = Path.Combine("best.onnx");

    private readonly Dictionary<string, string> _wasteMapping = new()
    {
        // Orgánicos
        ["banana"] = "organico",
        ["apple"] = "organico",
        ["orange"] = "organico",
        ["food"] = "organico",
        ["organic"] = "organico",

        // Valorizables (Reciclables)
        ["bottle"] = "valorizable",
        ["plastic"] = "no_valorizable",
        ["cardboard"] = "valorizable",
        ["metal"] = "no_valorizable",
        ["can"] = "valorizable",
        ["paper"] = "no_valorizable",
        ["glass"] = "no_valorizable",

        // No valorizables
        ["trash"] = "no_valorizable",
        ["waste"] = "no_valorizable",
        ["general"] = "no_valorizable"
    };

    public InferenciasController(CABDB db)
    {
        _db = db;

        if (System.IO.File.Exists(_modelPath))
        {
            _yolo = new YoloPredictor(_modelPath);
        }
        else
        {
            throw new FileNotFoundException($"Modelo ONNX no encontrado: {_modelPath}");
        }
    }

    [Consumes("multipart/form-data")]
    [HttpPost]
    public async Task<IActionResult> Predecir([FromForm] InferenciaDTO request)
    {
        try
        {
            if (request.Imagen == null)
            {
                return BadRequest(new { error = "No se encontró imagen en la petición", success = false });
            }

            if (request.ClasificadorId <= 0)
            {
                return BadRequest(new { error = "El id del clasificador es inválido", success = false });
            }

            byte[] imageBytes;
            using (var memoryStream = new MemoryStream())
            {
                await request.Imagen.CopyToAsync(memoryStream);
                imageBytes = memoryStream.ToArray();
            }

            var frame = Image.Load<Rgb24>(imageBytes);
            if (frame == null)
            {
                return BadRequest(new { error = "No se pudo decodificar la imagen", success = false });
            }

            var originalPath = Path.Combine("original.jpg");
            await frame.SaveAsJpegAsync(originalPath);

            var results = await _yolo.DetectAsync(frame);
            var detectedObjects = results?.Count ?? 0;

            if (detectedObjects == 0)
            {
                return Ok(new { 
                    message = "Sin objetos detectados",
                    wasteType = "no_valorizable",
                    detectedObjects = 0,
                    success = true,
                });
            }

            var detectedClasses = new List<string>();

            if (results != null)
            {
                foreach (var detection in results)
                {
                    if (detection.Confidence > 0.5f)
                    {
                        var className = detection.Name.ToString().ToLower();
                        detectedClasses.Add(className);
                    }
                }
            }

            var wasteType = "no_valorizable";
            foreach (var detectedClass in detectedClasses)
            {
                foreach (var mapping in _wasteMapping)
                {
                    if (detectedClass.Contains(mapping.Key))
                    {
                        wasteType = mapping.Value;
                        break;
                    }
                }
                if (wasteType != "no_valorizable")
                    break;
            }

            var deteccion = new Deteccion
            {
                Tipo = wasteType,
                FechaHora = DateTime.Now,
                ClasificadorId = request.ClasificadorId
            };

            _db.Detecciones.Add(deteccion);
            await _db.SaveChangesAsync();

            return Ok(wasteType);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = $"Error interno del servidor: {ex.Message}", success = false });
        }
    }
}