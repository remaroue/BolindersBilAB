using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using WU16.BolindersBilAB.DAL.DataAccess;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using WU16.BolindersBilAB.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using WU16.BolindersBilAB.DAL.Repository;
using WU16.BolindersBilAB.DAL.Services;
using WU16.BolindersBilAB.DAL.Seeding;
using Microsoft.AspNetCore.Routing;
using WU16.BolindersBilAB.DAL.Seeding.Enums;
using WU16.BolindersBilAB.Web.Models;
using WU16.BolindersBilAB.BLL.Configuration;
using WU16.BolindersBilAB.BLL.Services;

namespace WU16.BolindersBilAB.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("ApplicationDbContext")));

            services.Configure<EmailServiceConfiguration>(Configuration.GetSection("EmailService"));
            services.Configure<ImageUploadConfiguration>(Configuration.GetSection("ImageUpload"));
            services.Configure<FtpServiceConfiguration>(Configuration.GetSection("FtpService"));

            #region Identity
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 2;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = true;
            });


            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.Cookie.Expiration = TimeSpan.FromDays(150);
                options.LoginPath = "/Account/Login"; // If the LoginPath is not set here, ASP.NET Core will default to /Account/Login
                options.LogoutPath = "/Account/Logout"; // If the LogoutPath is not set here, ASP.NET Core will default to /Account/Logout
                options.AccessDeniedPath = "/Home/Index"; // If the AccessDeniedPath is not set here, ASP.NET Core will default to /Account/AccessDenied
                options.SlidingExpiration = true;
            });
            #endregion

            services.AddScoped<EmailService>();
            services.AddScoped<CarSearchService>();
            services.AddScoped<ImageService>();
            services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
            services.AddScoped<CarListService>();
            services.AddScoped<CarBrandService>();
            services.AddScoped<LocationService>();
            services.AddScoped<CarService>();

            services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, UserManager<ApplicationUser> userManager, ApplicationDbContext _ctx)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseStaticFiles();
                app.UseBrowserLink();
            }
            else
            {
                app.UseStaticFiles(new StaticFileOptions()
                {
                    OnPrepareResponse = x =>
                    {
                        const int duration = 60 * 60 * 24 * 365;
                        x.Context.Request.Headers[HeaderNames.CacheControl]
                            = "public,max-age=" + duration;
                    }
                });
            }

            app.UseAuthentication();

            app.UseMvc(x => x.MapRoute("default", template: "{Controller=Home}/{Action=Index}/{Id?}"));

            //// New Seeder

            #region Seeding
            if (!userManager.Users.Any())
            {
                var user1 = new ApplicationUser { UserName = "jonkoping@bolindersbil.se", Email = "jonkoping@bolindersbil.se" };
                var user2 = new ApplicationUser { UserName = "varnamo@bolindersbil.se", Email = "varnamo@bolindersbil.se" };
                var user3 = new ApplicationUser { UserName = "goteborg@bolindersbil.se", Email = "goteborg@bolindersbil.se" };
                var user4 = new ApplicationUser { UserName = "admin@bolindersbil.se", Email = "admin@bolindersbil.se" };

                Task.WaitAll(userManager.CreateAsync(user1, "Admin1234"));
                Task.WaitAll(userManager.CreateAsync(user2, "Admin1234"));
                Task.WaitAll(userManager.CreateAsync(user3, "Admin1234"));
                Task.WaitAll(userManager.CreateAsync(user4, "Admin1234"));
            }

            if (!_ctx.Locations.Any())
            {
                var locations = new List<Location>
                {
                new Location{Name="Bolinders Bil Jönköping", Address="Lovsjövägen 33", City="Jönköping", Zip="55626", PhoneNumber="036-123456", Email="jonkoping@bolindersbil.se", Id="BB1"},
                new Location{Name="Bolinders Bil Värnamo", Address="Bultgatan 2", City="Värnamo", Zip="54452", PhoneNumber="0370-123456", Email="varnamo@bolindersbil.se", Id="BB2"},
                new Location{Name="Bolinders Bil Göteborg", Address="Industrivägen 1", City="Göteborg", Zip="55336", PhoneNumber="031-123456", Email="goteborg@bolindersbil.se", Id="BB3"}
                };

                var carBrands = new List<CarBrand>()
                {
                new CarBrand{BrandName="Volvo", ImageName="/images/carbrands/bmw-logo.png"},
                new CarBrand{BrandName="BMW", ImageName="/images/carbrands/ferrari-logo.png"},
                new CarBrand{BrandName="Audi", ImageName="/images/carbrands/koenigsegg-logo.png"},
                new CarBrand{BrandName="Ford", ImageName="/images/carbrands/saab-logo.png"},
                new CarBrand{BrandName="Mercedes-benz", ImageName="/images/carbrands/saab-logo.png"},
                new CarBrand{BrandName="Volkswagen", ImageName="/images/carbrands/volvo-logo.png"},
                };

                _ctx.Set<CarBrand>().AddRange(carBrands);
                _ctx.Set<Location>().AddRange(locations);
                _ctx.SaveChanges();
            }

            Seeder<Car>.SeedDbContext(_ctx, 1000, SeedDbContextSettings.LeaveIfExists);
            #endregion
        }
    }
}
