using System.Web.Mvc;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

using SimpleInjector;

using YAB.Api.Contracts.BackgroundTasks;
using YAB.Plugins.Injectables;
using YAB.Plugins.Injectables.Options;
using YAB.Services.Common;

namespace YAB.Api
{
    public class Startup
    {
        private Container _container = new Container();

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSimpleInjector(_container);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "YAB.Api v1"));
            }

            app.UseCors(opt => opt.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ControllerBuilder.Current.DefaultNamespaces.Add("YAB.Api.Contracts.Controllers");

            // Necessary in order to inject IContainerAccessor into controller
            services.AddSingleton(typeof(IContainerAccessor), new ContainerAccessor(_container));
            services.AddCors();
            services
                .AddControllers()
                .AddNewtonsoftJson(opt =>
                {
                    opt.SerializerSettings.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All;
                });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "YAB.Api", Version = "v1" });
            });

            services.AddSimpleInjector(_container, options =>
            {
                options.AddAspNetCore()
                    .AddControllerActivation();

                options.AddLogging();
            });

            InitializeContainer();
        }

        private void InitializeContainer()
        {
            var containerAccessor = new ContainerAccessor(_container);

            _container.RegisterInstance(new BotOptions());
            // _container.RegisterInstance(new TwitchOptions());
            _container.Register<BackgroundTasksManager>(Lifestyle.Singleton);

            _container.RegisterInstance(new Lazy<IBackgroundTasksManager>(() => (IBackgroundTasksManager)_container.GetInstance(typeof(BackgroundTasksManager))));

            _container.RegisterInstance<IContainerAccessor>(containerAccessor);
            _container.RegisterInstance<IAvailablePluginsHelper>(new AvailablePluginsHelper());
            _container.RegisterSingleton<IPipelineStore, PipelineStore>();
            _container.RegisterSingleton<IFrontendLogging, FrontendLogging>();
            _container.RegisterSingleton(typeof(IEventSender), typeof(EventSenderInstantExecuter));

            _container.LoadAllPlugins();
        }
    }
}