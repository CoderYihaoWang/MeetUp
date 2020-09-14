using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetUp.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        private ILogger<ExceptionFilter> _logger;

        public ExceptionFilter(ILogger<ExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            _logger.LogCritical($"Caught in exception: {context.Exception.Message}", context.Exception);

            var result = new JsonResult("Something went wrong")
            {
                StatusCode = 500
            };

            context.Result = result;
        }
    }
}
