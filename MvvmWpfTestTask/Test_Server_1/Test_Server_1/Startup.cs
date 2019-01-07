using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Test_Server_1
{
    using System.IO;
    using Test_Server_1.Controllers;

    public class Startup
    {
        private static FileController _fController;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            _fController = FileController.GetInstance();
            _fController.Initialize();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();

            app.Map("/Add", AddNote);
            app.Map("/Edit", EditNote);
            app.Map("/Del", DeleteNote);
            app.Map("/Get", GetNote);
            app.Map("/Alive", Alive);
        }

        private static void Alive(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                await context.Response.WriteAsync("t");
            });
        }

        private static void AddNote(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                using(var reader = new StreamReader(context.Request.Body))
                {
                    var result = await _fController.AddNote(await reader.ReadToEndAsync());

                    if (result)
                        await context.Response.WriteAsync("t");
                    else
                        await context.Response.WriteAsync("f");
                }
            });
        }
        private static void EditNote(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                using (var reader = new StreamReader(context.Request.Body))
                {
                    var result = await _fController.EditNote(await reader.ReadToEndAsync());

                    if (result)
                        await context.Response.WriteAsync("t");

                    await context.Response.WriteAsync("f");
                }
            });
        }
        private static void DeleteNote(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                using (var reader = new StreamReader(context.Request.Body))
                {
                    bool result = false;

                    string json = await reader.ReadToEndAsync();
                    char comp = json.ElementAt(0);

                    json = json.Remove(0, 1);

                    if (comp == 't')
                        result = await _fController.DeleteNote(json, true);
                    else
                        result = await _fController.DeleteNote(json, false);
                    
                    if (result)
                        await context.Response.WriteAsync("t");
                    else
                        await context.Response.WriteAsync("f");

                }
            });
        }
        private static void GetNote(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                var result = await _fController.GetNotes();
                await context.Response.WriteAsync(result);
            });
        }
    }
}
