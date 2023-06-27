using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;

namespace Homepage.Controllers;

public class DownloadController : Controller
{
    public IActionResult Index(string filename)
    {
        var file = $"./wwwroot/download/{filename}";
        if (!System.IO.File.Exists(file))
            return NotFound();
        
        var bytes = System.IO.File.ReadAllBytes(file);
        System.IO.File.Delete(file);
        
        var cd = new ContentDisposition
        {
            FileName = filename,
            Inline = false
        };
        Response.Headers.Add("Content-Disposition", cd.ToString());
        Response.Headers.Add("X-Content-Type-Options", "nosniff");
        return File(bytes, "application/octet-stream");
    }
}