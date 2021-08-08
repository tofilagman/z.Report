using z.Report.Services;
using Microsoft.Extensions.DependencyInjection;
using z.Report.Options;
using System;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace z.Report
{
    public static class ReportServiceInjector
    {
        public static void AddReport(this IServiceCollection services, Action<IServiceProvider, ReportOptions> configure = null)
        {
            services.AddSingleton<IRenderService, RenderService>();

            services.TryAdd(new ServiceDescriptor(typeof(ReportOptions), provider =>
            {
                var option = new ReportOptions();
                configure?.Invoke(provider, option);
                return option;
            }, ServiceLifetime.Singleton));

        }
    }
}
