namespace QHH.Modules
{
    using System;
    using Discord.Commands;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using QHH.Data;

    public abstract class QHHModuleBase : ModuleBase<SocketCommandContext>
    {
        public readonly DataAccessLayer DataAccessLayer;
        public readonly IConfiguration Configuration;
        private readonly IServiceScope scope;

        protected QHHModuleBase(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            this.scope = serviceProvider.CreateScope();
            this.DataAccessLayer = this.scope.ServiceProvider.GetRequiredService<DataAccessLayer>();

            this.Configuration = configuration;
        }
    }
}
