using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using SimpleInjector;

using YAB.Core.EventReactor;
using YAB.Core.Events;
using YAB.Plugins;

namespace YAB.Services.Common
{
    public static class PluginLoader
    {
        private const string PluginsDirectory = "/extensions/";

        public static void LoadAllPlugins(this Container container)
        {
            Console.WriteLine(Directory.GetCurrentDirectory());
            var pluginFiles = Directory.GetFiles(Directory.GetCurrentDirectory() + PluginsDirectory, "*.dll", SearchOption.AllDirectories);

            var loadedAssemblies = new List<Assembly>();

            var loadedEvents = new List<Type>();
            var loadedEventReactors = new List<Type>();
            var loadedBackgroundTasks = new List<Type>();

            foreach (var plugin in pluginFiles)
            {
                try
                {
                    var asm = Assembly.LoadFile(plugin);
                    if (asm != null)
                    {
                        if (loadedAssemblies.Any(a => a.FullName == asm.FullName))
                        {
                            continue;
                        }
                        else
                        {
                            loadedAssemblies.Add(asm);
                        }

                        var modules = asm.GetTypes().Where(t => !t.IsAbstract && !t.IsGenericType && typeof(IPluginModule).IsAssignableFrom(t));
                        foreach (var module in modules)
                        {
                            var moduleInstance = (IPluginModule)Activator.CreateInstance(module);
                            moduleInstance.RegisterBackgroundTasks((t) =>
                            {
                                container.Register(t);
                                loadedBackgroundTasks.Add(t);
                            });

                            moduleInstance.RegisterEventReactors((t) =>
                            {
                                container.Register(t);
                                loadedEventReactors.Add(t);
                            });

                            moduleInstance.RegisterPluginEvents((t) =>
                            {
                                container.Register(t);
                                loadedEvents.Add(t);
                            });
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

            container.Options.AllowOverridingRegistrations = false;
            container.Collection.Register(typeof(IBackgroundTask), loadedBackgroundTasks);
            container.Collection.Register(typeof(IEventReactor), loadedEventReactors);
            container.Collection.Register(typeof(IEventBase), loadedEvents);
        }
    }
}