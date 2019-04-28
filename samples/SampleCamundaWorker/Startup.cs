using System;
using Camunda.Worker;
using Camunda.Worker.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SampleCamundaWorker.Handlers;

namespace SampleCamundaWorker
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
            services.AddCamundaWorker(options =>
                {
                    options.WorkerId = "sampleWorker";
                    options.WorkerCount = 4;
                    options.BaseUri = new Uri("http://localhost:8080/engine-rest");
                })
                .AddHandler<SayHelloHandler>()
                .AddHandler<SayHelloGuestHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
        }
    }
}
