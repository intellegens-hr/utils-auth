using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UtilsAuth.Core.Api.Models.Users;
using UtilsAuth.DbContext.Models;
using UtilsAuth.Demo.Api.DbContext;
using UtilsAuth.Extensions;

namespace UtilsAuth.Demo.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //    app.UseSwagger();
            //    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "UtilsAuth.Demo.Api v1"));
            //}

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var context = new AppDbContext();
            context.Database.Migrate();

            services.AddAutoMapper(c =>
            {
                c.CreateMap<UserDb, UserDto>()
                .ForMember(x => x.Id, x => x.MapFrom(s => s.Id))
                .ForMember(x => x.UserName, x => x.MapFrom(s => s.UserName))
                .ForMember(x => x.Email, x => x.MapFrom(s => s.Email));
            });

            services.AddDbContext<AppDbContext>();
            services.AddLogging();
            // services.AddControllers();
            services.AddMvcCore().AddUtilsAuthAuthenticationControllerAssemblyPart();
            services.AddUtilsAuth<AppDbContext>(new Configuration());

            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new OpenApiInfo { Title = "UtilsAuth.Demo.Api", Version = "v1" });
            //});
        }
    }
}