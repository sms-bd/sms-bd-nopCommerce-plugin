using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;

namespace Nop.Plugin.SMS.Net.bd
{
    public class PluginNopStartup : INopStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient(nameof(SmsNetBdProvider));
        }

        public void Configure(IApplicationBuilder application)
        {
            // no middleware required
        }

        public int Order => 500;
    }
}
