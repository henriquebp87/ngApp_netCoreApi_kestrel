using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace ngApp_netCoreApi_kestrel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly string _contentRootPath;

        public FilesController(IWebHostEnvironment environment)
        {
            _contentRootPath = environment.ContentRootPath;
        }

        [Produces(typeof(IEnumerable<WeatherForecast>))]
        public IActionResult ReadFiles()
        {
            var path = $"{_contentRootPath}\\readfiles";
            var output = new List<WeatherForecast>();

            if (!Directory.Exists(path)) return NoContent();

            var files = Directory.EnumerateFiles(path);

            foreach (var fileName in files)
            {
                var file = new System.IO.StreamReader(fileName);
                string line;

                while ((line = file.ReadLine()) != null)
                {
                    var parts = line.Split(';');
                    var date = Convert.ToDateTime(parts[0]);
                    var tempC = Convert.ToInt32(parts[1]);
                    var summary = parts[2];

                    output.Add(new WeatherForecast
                    {
                        Date = date,
                        TemperatureC = tempC,
                        Summary = summary
                    });
                }

                file.Close();
            }

            return Ok(output);
        }
    }
}
