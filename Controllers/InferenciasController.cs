using Microsoft.AspNetCore.Mvc;
using cabapi.Models;
using OpenAI;
using OpenAI.Chat;

namespace cabapi.Controllers;

[ApiController]
[Route("api/inferencias")]
public class InferenciasController : ControllerBase
{
    private readonly CABDB _db;
    private readonly OpenAIClient _openAIClient;
    private readonly ILogger<InferenciasController> _logger;
    private readonly Dictionary<string, string> _wasteMapping = new()
    {
        ["organico"] = "Organico",
        ["organic"] = "Organico",
        ["compostable"] = "Organico",
        ["biodegradable"] = "Organico",

        ["valorizable"] = "Valorizable",
        ["reciclable"] = "Valorizable",
        ["recyclable"] = "Valorizable",

        ["no_valorizable"] = "NoValorizable",
        ["no_reciclable"] = "NoValorizable",
        ["basura"] = "NoValorizable",
        ["trash"] = "NoValorizable"
    };

    public InferenciasController(CABDB db, ILogger<InferenciasController> logger)
    {
        _db = db;
        _logger = logger;

        var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new InvalidOperationException("OpenAI API Key no configurada");
        }

        _openAIClient = new OpenAIClient(apiKey);
    }

    [Consumes("multipart/form-data")]
    [HttpPost("esp")]
    [DisableRequestSizeLimit]
    public async Task<IActionResult> ClasificarEsp32([FromForm] IFormFile imagen, [FromForm] int clasificadorId)
    {
        try
        {
            if (imagen == null)
            {
                return BadRequest(new { error = "No se encontró imagen en la petición", success = false });
            }

            if (clasificadorId <= 0)
            {
                return BadRequest(new { error = "El id " + clasificadorId + " del clasificador es inválido", success = false });
            }

            byte[] imageBytes;
            using (var memoryStream = new MemoryStream())
            {
                await imagen.CopyToAsync(memoryStream);
                imageBytes = memoryStream.ToArray();
            }

            var originalPath = Path.Combine("original.jpg");
            await System.IO.File.WriteAllBytesAsync(originalPath, imageBytes);

            var wasteType = await ClasificarConOpenAI(imageBytes);

            if (wasteType == Tipo.Error)
            {
                return Ok(new
                {
                    wasteType = Tipo.Error,
                    success = false,
                    shouldRetry = true,
                    confidence = "low",
                    timestamp = DateTime.UtcNow
                });
            }

            var deteccion = new Deteccion
            {
                Tipo = wasteType,
                FechaHora = DateTime.Now,
                ClasificadorId = clasificadorId
            };

            _db.Detecciones.Add(deteccion);
            await _db.SaveChangesAsync();

            return Ok(wasteType.ToString());
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = $"Error interno del servidor: {ex.Message}", success = false });
        }
    }

    [Consumes("multipart/form-data")]
    [HttpPost("android")]
    [DisableRequestSizeLimit]
    public async Task<IActionResult> ClasificarAndroid([FromForm] IFormFile imagen)
    {
        try
        {
            if (imagen == null)
            {
                return BadRequest(new { error = "No se encontró imagen en la petición", success = false });
            }

            byte[] imageBytes;
            using (var memoryStream = new MemoryStream())
            {
                await imagen.CopyToAsync(memoryStream);
                imageBytes = memoryStream.ToArray();
            }

            var wasteType = await ClasificarConOpenAI(imageBytes);

            if (wasteType == Tipo.Error)
            {
                return Ok(new
                {
                    wasteType = Tipo.Error,
                    success = false,
                    shouldRetry = true,
                    confidence = "low",
                    timestamp = DateTime.UtcNow
                });
            }

            return Ok(new
            {
                wasteType = wasteType,
                success = true,
                confidence = "high",
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = $"Error interno del servidor: {ex.Message}", success = false });
        }
    }

    private async Task<Tipo> ClasificarConOpenAI(byte[] imageBytes)
    {
        try
        {
            var systemPrompt = @"
Eres un experto clasificador de residuos. Tu tarea es analizar imágenes y clasificar el tipo de residuo presente.

INSTRUCCIONES IMPORTANTES:
1. Analiza cuidadosamente la imagen
2. Identifica el objeto o residuo principal en la imagen
3. Responde ÚNICAMENTE con una de estas tres palabras exactas:
   - 'Organico' para residuos orgánicos (comida, frutas, verduras, restos de comida, cáscaras, etc.)
   - 'Valorizable' para residuos reciclables (plástico, vidrio, metal, papel, cartón, latas, botellas, etc.)
   - 'NoValorizable' para residuos no reciclables (pañales, chicles, colillas, papel higiénico usado, etc.)
   - 'Error' si la imagen no es clara y no puedes determinar el tipo dada las condiciones de la imagen

EJEMPLOS:
- Cáscara de plátano → Organico
- Botella de plástico → Valorizable
- Pañal usado → NoValorizable
- Desconocido → NoValorizable
- Imagen borrosa → Error

Responde solo con la palabra correspondiente, sin explicaciones adicionales.";

            var userPrompt = "Clasifica el tipo de residuo en esta imagen según las categorías especificadas.";

            var chatClient = _openAIClient.GetChatClient("gpt-4o");

            var messages = new List<ChatMessage>
            {
                new SystemChatMessage(systemPrompt),
                new UserChatMessage(
                    ChatMessageContentPart.CreateTextPart(userPrompt),
                    ChatMessageContentPart.CreateImagePart(BinaryData.FromBytes(imageBytes), "image/jpeg")
                )
            };

            var response = await chatClient.CompleteChatAsync(messages, new ChatCompletionOptions
            {
                MaxOutputTokenCount = 10,
                Temperature = 0.1f
            });

            var classification = response.Value.Content[0].Text.Trim().ToLower();

            return Enum.Parse<Tipo>(classification, true);
        }
        catch
        {
            // En caso de error, retornar "error"
            return Tipo.Error;
        }
    }
}