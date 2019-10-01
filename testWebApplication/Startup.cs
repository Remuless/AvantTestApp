using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using testWebApplication.Models;
using Newtonsoft.Json;
using testWebApplication;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.OpenApi.Models;

namespace testWebApplication
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
			services.AddDistributedMemoryCache();

			services.AddSession();
			services.AddControllers();

            services.AddMvc(options => options.EnableEndpointRouting = false);

            services.AddSingleton<IProjectRepository, ProjectRepository>();

			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
			});

			services.AddMvcCore().AddApiExplorer();


		}

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSession();

			//app.UseEndpoints(endpoints =>
			//{
			//	endpoints.MapDefaultControllerRoute();
			//});

			app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseHttpsRedirection();

            app.UseAuthorization();
			
			app.UseSwagger();

			app.UseSwaggerUI( c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
			}
			);

			app.UseRouting();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapDefaultControllerRoute();
			});

			app.Run(async (context) =>
			{
				for (int i = 0; i < 3; i++)
				{
					if (context.Session.Keys.Contains("project" + i))
					{
						Project project = context.Session.Get<Project>("project" + i);
						await context.Response.WriteAsync($"DeserializeObject {project.Name}! ");
					}
					else
					{
						Project project = new Project { Name = "Project" + i, startdate = DateTime.Today.AddDays(-i) };
						context.Session.Set<Project>("project" + i, project);
						await context.Response.WriteAsync("SerializeObject! ");
						await context.Response.WriteAsync(project.Name + " " + 
							project.startdate + " " + 
							project.enddate + " " + 
							project.status.ToString() + " " + 
							project.ID.ToString());
					}
				}
			});
		}
    }
}
