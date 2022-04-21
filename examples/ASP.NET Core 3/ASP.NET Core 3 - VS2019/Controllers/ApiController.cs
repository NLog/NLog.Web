using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ASP.NetCore3_NLog_Web_Example.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogTestsController : ControllerBase
    {
        private readonly ILogger<LogTestsController> _logger;

        public LogTestsController(ILogger<LogTestsController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult> TestLog([FromBody]string request)
        {
            try
            {
                await Task.Delay(5).ConfigureAwait(false);
                throw new ApplicationException("Test log message");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Test error logging");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
