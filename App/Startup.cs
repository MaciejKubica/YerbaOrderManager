using System.Text;
using System.Threading.Tasks;
using App.Data;
using App.Data.Entities;
using AspNetCore.Identity.SQLite.Dapper;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace App
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);            

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            services.AddAutoMapper();

            services.AddTransient<IDatabaseRepository, DatabaseRepository>();
           
            services.AddIdentity<StoreUserExtended, IdentityRole<int>>(options =>
            {
                options.User.RequireUniqueEmail = true;

                // Password settings
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 2;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+ ";
                options.User.RequireUniqueEmail = true;
            }).AddDefaultTokenProviders();


            // Identity Services
            services.AddTransient<IDBRepositoryConfiguration, DBRepositoryConfiguration>(s =>
            {
                return new DBRepositoryConfiguration(
                    Configuration["ConnectionString"], 
                    "Users",
                    "UserLogins",
                    "Roles",
                    "UserRoles",
                    "Claims",
                    false
                    );

            });

            services.AddTransient<IUserStore<StoreUserExtended>, UserStore<StoreUserExtended, IdentityRole<int>,int,int>>();
            services.AddTransient<IRoleStore<IdentityRole<int>>, RoleStore<IdentityRole<int>, int>>();
            services.AddTransient<IUserTable<StoreUserExtended, int>, UserTable<StoreUserExtended, int>>();
            services.AddTransient<IRoleTable<IdentityRole<int>, int>, RoleTable<IdentityRole<int>,int>>();
            services.AddTransient<IUserRolesTable<StoreUserExtended,int, int>, UserRolesTable<StoreUserExtended,int,int>>();
            services.AddTransient<IUserLoginsTable<StoreUserExtended, int>, UserLoginsTable<StoreUserExtended,int>>();
            services.AddTransient<IUserClaimsTable<StoreUserExtended, int>, UserClaimsTable<StoreUserExtended,int>>();

            services.AddAuthentication()
                .AddCookie()
                .AddJwtBearer(cfg =>
                {
                    cfg.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidIssuer = Configuration["Tokens:Issuer"],
                        ValidAudience = Configuration["Tokens:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Tokens:Key"]))
                    };
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                using (var scope = app.ApplicationServices.CreateScope())
                {
                    var seeder = scope.ServiceProvider.GetService<UserManager<StoreUserExtended>>();
                    //var reesult = seeder.CreateAsync(
                    //    new StoreUserExtended()
                    //    {
                    //        AccessFailedCount = 0,
                    //        BankAccount = "5432657483657843",
                    //        Email = "maxxior3@o2.pl",
                    //        Name = "Maciej",
                    //        OrderToken = false
                    //    }, "p@SSw0rd!").Result;
                }
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }
}
