using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MockIOT
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors(policy => {
                policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
            });
            app.UseRouting();

            var devices = new Dictionary<string, List<object>> {
                ["device1"] = new List<object> {
                    new { value = 1 },
                    new { value = 1 },
                    new { value = 1 },
                    new { value = 1 },
                    new { value = 1 },
                    new { value = 1 },
                    new { value = 1 },
                    new { value = 1 },
                    new { value = 2 },
                    new { value = 2 },
                    new { value = 3 }
                },
                ["device2"] = new List<object> {
                    new { min = 1, max = 6 },
                    new { min = 1, max = 6 },
                    new { min = 1, max = 6 },
                    new { min = 1, max = 6 },
                    new { min = 1, max = 6 },
                    new { min = 1, max = 6 },
                    new { min = 1, max = 6 },
                    new { min = -1, max = -6 },
                }
            };
            
            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", context => {
                    context.Response.Redirect("/index.html");
                    return Task.CompletedTask;
                });

                endpoints.MapGet("/api/{id}", async context =>
                {
                    var logger = context.RequestServices.GetRequiredService<ILogger<Startup>>();

                    logger.LogInformation("{RequestId} connected", context.TraceIdentifier);

                    var deviceId = context.Request.RouteValues["id"].ToString();
                    var deviceEvents = devices[deviceId];

                    context.Response.Headers.Add("Content-Type", "text/event-stream");
                    
                    foreach (var e in deviceEvents) {
                        if (context.RequestAborted.IsCancellationRequested) {
                            logger.LogInformation("{RequestId} disconnected", context.TraceIdentifier);
                            break;
                        }
                        
                        await Task.Delay(1000);
                        logger.LogInformation("Sent a message to {RequestId}", context.TraceIdentifier);
                        await context.Response.WriteAsync($"data: {System.Text.Json.JsonSerializer.Serialize(e)} \n\n");
                    }
                    await context.Response.CompleteAsync();
                });
            });
        }
    }
}
