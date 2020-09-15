using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MeetUp.Filters
{
    public class NationalityFilter : Attribute, IAuthorizationFilter
    {
        private readonly string[] _nationalities;

        public NationalityFilter(string nationalities)
        {
            _nationalities = nationalities.Split(",");
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var nationality = context.HttpContext.User.FindFirst(c => c.Type == "Nationality")?.Value;

            if (!_nationalities.Any(n => n == nationality))
            {
                context.Result = new StatusCodeResult(403);
            }
        }
    }
}
