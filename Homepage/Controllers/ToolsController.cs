using Homepage.Util;
using Microsoft.AspNetCore.Mvc;

namespace Homepage.Controllers;

public class ToolsController : Controller
{
    private readonly ILogger<ToolsController> _logger;
    private readonly HomebankConverter _homebankConverter;
    private readonly FileManager _fileManager;

    #region Constructor

    public ToolsController(
        HomebankConverter homebankConverter,
        ILogger<ToolsController> logger,
        FileManager fileManager)
    {
        _homebankConverter = homebankConverter;
        _logger = logger;
        _fileManager = fileManager;
    }

    #endregion

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult HomebankConvert()
    {
        try
        {
            var file = Request.Form.Files.FirstOrDefault();
            if (file == null || file.Length == 0)
                return BadRequest();

            using var ms = new MemoryStream();
            file.CopyTo(ms);
            var result = _homebankConverter.Convert(ms.ToArray());
            var filename = $"{Path.GetFileNameWithoutExtension(file.FileName)}-homebank.csv";
            _fileManager.WriteForDownload(result, filename);
            Response.Headers.Add(Htmx.HtmxResponseHeaders.Keys.Redirect, $"/download/{filename}");
            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while converting Homebank file");
            return StatusCode(500);
        }
    }
}