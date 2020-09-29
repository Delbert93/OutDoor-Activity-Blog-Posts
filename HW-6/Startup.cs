using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HW_6.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Sqlite;

namespace HW_6
{
    //we are started
    public class Startup
    {
        private CmsUrlConstraint cmsUrlConstraint;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<OutdoorUser, IdentityRole>()
                 .AddDefaultTokenProviders()
                 .AddDefaultUI()
                 .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddScoped<Microsoft.AspNetCore.Identity.IUserClaimsPrincipalFactory<OutdoorUser>, AppClaimsPrincipalFactory>();


            services.AddAuthorization(options =>
            {
                options.AddPolicy(MyIdentityData.BlogPolicy_Add, policy => policy.RequireRole(MyIdentityData.AdminRoleName, MyIdentityData.EditorRoleName, MyIdentityData.ContributorRoleName));
                options.AddPolicy(MyIdentityData.BlogPolicy_Edit, policy => policy.RequireRole(MyIdentityData.AdminRoleName, MyIdentityData.EditorRoleName));
                options.AddPolicy(MyIdentityData.BlogPolicy_Delete, policy => policy.RequireRole(MyIdentityData.AdminRoleName));
            });

            
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, UserManager<OutdoorUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {

            MyIdentityData.SeedData(userManager, roleManager);
            cmsUrlConstraint = new CmsUrlConstraint(configuration);

            app.Use(async (context, next) =>
            {
                await next.Invoke();

                //After going down the pipeline check if we 404'd. 
                if (context.Response.StatusCode == StatusCodes.Status404NotFound)
                {
                    await context.Response.WriteAsync("<h1>Woops! This page doesn't exist!</h1> <br> <h3>What are you trying to do here anyway?? Go back to the previous page before it's too late...<h3>");
                }
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "CmsRoute",
                    template: "{*permalink}",
                    defaults: new { controller = "Content", action = "Index" },
                    constraints: new { permalink = cmsUrlConstraint }
                    );
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        } 
    }
}
