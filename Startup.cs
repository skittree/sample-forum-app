using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Task3.Store;
using Task3.Configuration;
using Task3.Services;
using Task3.Store.Roles;

namespace Task3
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
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

            services.AddIdentity<IdentityUser, IdentityRole>(options =>
                options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            var mapper = mapperConfig.CreateMapper();

            services.AddScoped<ISectionService, SectionService>();
            services.AddScoped<ITopicService, TopicService>();
            services.AddScoped<IMessageService, MessageService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddSingleton(mapper);
            services.AddControllersWithViews();
            InitializeRoles(services);
            InitializeAdmin(services);
            InitializeModerators(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
        private void InitializeRoles(IServiceCollection services)
        {
            using (var serviceProvider = services.BuildServiceProvider())
            {
                try
                {
                    var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

                    foreach (var role in Roles.AllRoles)
                    {
                        if (roleManager.Roles.Any(x => x.Name == role))
                        {
                            continue;
                        }

                        var result = roleManager.CreateAsync(
                            new IdentityRole
                            {
                                Id = Guid.NewGuid().ToString(),
                                Name = role
                            }).Result;
                    }
                }
                catch (Exception)
                {
                    return;
                }
            }
        }

        private void InitializeAdmin(IServiceCollection services)
        {
            using (var serviceProvider = services.BuildServiceProvider())
            {
                try
                {
                    var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();

                    if (userManager.Users.Any(x => x.UserName == "admin"))
                    {
                        return;
                    }

                    var identityResult = userManager.CreateAsync(
                        new IdentityUser
                        {
                            Id = Guid.NewGuid().ToString(),
                            Email = "admin",
                            UserName = "admin"
                        },
                        "P@ssw0rd").Result;

                    var adminUser = userManager.Users.FirstOrDefault(x => x.UserName == "admin");
                    var result = userManager.AddToRoleAsync(adminUser, "Admin").Result;
                }
                catch (Exception)
                {
                    return;
                }
            }
        }

        private void InitializeModerators(IServiceCollection services)
        {
            using (var serviceProvider = services.BuildServiceProvider())
            {
                try
                {
                    var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();

                    for (int i = 1; i < 4; i++)
                    {
                        string name = "moderator" + i.ToString();
                        if (userManager.Users.Any(x => x.UserName == name))
                        {
                            continue;
                        }

                        var identityResult = userManager.CreateAsync(
                            new IdentityUser
                            {
                                Id = Guid.NewGuid().ToString(),
                                Email = name,
                                UserName = name
                            },
                            "P@ssw0rd").Result;

                        var moderatorUser = userManager.Users.FirstOrDefault(x => x.UserName == name);
                        var result = userManager.AddToRoleAsync(moderatorUser, "Moderator").Result;
                    }
                }
                catch (Exception)
                {
                    return;
                }
            }
        }
    }
}
