using Microsoft.AspNetCore.Http;
using System;
using Microsoft.AspNetCore.Mvc;
using basicapp.Services;
using System.Collections;
using System.Collections.Generic;

namespace basicapp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IFileService fileService;

        public HomeController(IFileService _fileService)
        {
            fileService = _fileService;
        }

        public ActionResult<IEnumerable<JsonResult>> Post([FromForm] IFormFile file)
        {
            if (!fileService.IsValidFileType(file))
                return BadRequest("Invalid file type.");

            IEnumerable emails = fileService.ProcessFile(file);
            return Ok(emails);
        }
    }
}
