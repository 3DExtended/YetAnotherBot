using System.Web.Mvc;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using SimpleInjector;

using YAB.Api.Contracts.BackgroundTasks;
using YAB.Plugins.Injectables;
using YAB.Plugins.Injectables.Options;
using YAB.Services.Common;

namespace YAB.ApiWithFrontend
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
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseCors(opt => opt.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ControllerBuilder.Current.DefaultNamespaces.Add("YAB.Api.Contracts.Controllers");

            // neccessary in order to inject IContainerAccessor into controller
            services.AddSingleton(typeof(IContainerAccessor), new ContainerAccessor(_container));
            services.AddCors();

            services
                .AddControllersWithViews()
                .AddNewtonsoftJson(opt =>
                {
                    opt.SerializerSettings.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All;
                });

            services.AddSimpleInjector(_container, options =>
            {
                options.AddAspNetCore()
                    .AddControllerActivation();

                options.AddLogging();
            });

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            InitializeContainer();
        }

        private void InitializeContainer()
        {
            var containerAccessor = new ContainerAccessor(_container);

            _container.RegisterInstance(new BotOptions());
            // _container.RegisterInstance(new TwitchOptions());
            // _container.RegisterSingleton<IBackgroundTasksManager, BackgroundTasksManager>();
            _container.Register<BackgroundTasksManager>(Lifestyle.Singleton);

            _container.RegisterInstance(new Lazy<IBackgroundTasksManager>(() => (IBackgroundTasksManager)_container.GetInstance(typeof(BackgroundTasksManager))));

            _container.RegisterInstance<IContainerAccessor>(containerAccessor);
            _container.RegisterInstance<IAvailablePluginsHelper>(new AvailablePluginsHelper());
            _container.RegisterSingleton<IPipelineStore, PipelineStore>();
            _container.RegisterSingleton(typeof(IEventSender), typeof(EventSenderInstantExecuter));

            _container.LoadAllPlugins();
        }
    }
}