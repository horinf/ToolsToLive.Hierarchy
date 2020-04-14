using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using ToolsToLive.Hierarchy.Interfaces;

namespace ToolsToLive.Hierarchy
{
    public static class DependencyInjectionExtensions
    {
        /// <summary>
        /// Adds hierarchy tools.
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <param name="configurationSection">Configuration section with settings.</param>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection AddHierarchyToolsForIntId<T>(this IServiceCollection services, IConfigurationSection configurationSection) where T : class, IHierarchyItem<T, int, int?>
        {
            services.AddOptions();
            services.Configure<HierarchyOptions>(configurationSection);
            services.AddSingleton(x => x.GetRequiredService<IOptions<HierarchyOptions>>().Value);

            services.AddSingleton<IHierarchyTools<T, int, int?>, HierarchyToolsForIntId<T>>();

            return services;
        }

        /// <summary>
        /// Adds hierarchy tools.
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <param name="configurationSection">Configuration section with settings.</param>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection AddHierarchyToolsForStringId<T>(this IServiceCollection services, IConfigurationSection configurationSection) where T : class, IHierarchyItem<T, string, string>
        {
            services.AddOptions();
            services.Configure<HierarchyOptions>(configurationSection);
            services.AddSingleton(x => x.GetRequiredService<IOptions<HierarchyOptions>>().Value);

            services.AddSingleton<IHierarchyTools<T, string, string>, HierarchyToolsForStringId<T>>();

            return services;
        }

        /// <summary>
        /// Adds hierarchy tools.
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <param name="setupAction">Action to configure settings.</param>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection AddHierarchyToolsForIntId<T>(this IServiceCollection services, Action<HierarchyOptions> setupAction) where T : class, IHierarchyItem<T, int, int?>
        {
            var settings = new HierarchyOptions();
            setupAction(settings);

            services.AddSingleton(settings);

            services.AddSingleton<IHierarchyTools<T, int, int?>, HierarchyToolsForIntId<T>>();

            return services;
        }

        /// <summary>
        /// Adds hierarchy tools.
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <param name="setupAction">Action to configure settings.</param>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection AddHierarchyToolsForStringId<T>(this IServiceCollection services, Action<HierarchyOptions> setupAction) where T : class, IHierarchyItem<T, string, string>
        {
            var settings = new HierarchyOptions();
            setupAction(settings);

            services.AddSingleton(settings);

            services.AddSingleton<IHierarchyTools<T, string, string>, HierarchyToolsForStringId<T>>();

            return services;
        }
    }
}
