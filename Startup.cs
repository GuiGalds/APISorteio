using FluentAssertions.Common;
using Microsoft.EntityFrameworkCore;
using SorteioAPI.Controllers;

namespace APISorteio
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddDbContext<SorteioDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("SorteioDb")));
            services.AddSingleton<GeradorNumerosSorteio>();
            services.AddScoped<SorteioService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }


    }
}
