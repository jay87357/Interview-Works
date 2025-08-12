using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using VueApp1.Server.Models;
using SixLabors.ImageSharp;


namespace VueApp1.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CaptchaController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            string code = CaptchaModel.GenerateCode(5);
            HttpContext.Session.SetString("CaptchaCode", code);

            var image = CaptchaModel.DrawCaptchaImage(code);
            var ms = new MemoryStream();
            image.SaveAsPng(ms);
            ms.Seek(0, SeekOrigin.Begin);

            return File(ms, "image/png");
        }
    }
}
