using HW_6.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HW_6
{
    public class CmsUrlConstraint: IRouteConstraint
    {
        private readonly IConfiguration configuration;
        public CmsUrlConstraint(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public bool Match(HttpContext httpContext, IRouter route, string routeKey, RouteValueDictionary values, RouteDirection routeDirection)
        {
            if (values[routeKey] != null){
                var permalink = values[routeKey].ToString();
                var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                optionsBuilder.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
                var dbContext = new ApplicationDbContext(optionsBuilder.Options);
                var page = dbContext.Pages.FirstOrDefault(p => p.Url == permalink);
                if(page != null)
                {
                    httpContext.Items["cmspage"] = page;
                    return true;
                }
            }
            return false;
        }
    }
}
