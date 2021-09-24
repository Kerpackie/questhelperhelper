namespace QHH.Modules
{
    using System;
    using Discord.Commands;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using QHH.Data;

    /// <inheritdoc />
    public abstract class QHHModuleBase : ModuleBase<SocketCommandContext>
    {
        public readonly DataAccessLayer DataAccessLayer;
        public readonly IConfiguration Configuration;
        private readonly IServiceScope scope;

        /// <summary>
        /// Initializes a new instance of the <see cref="QHHModuleBase"/> class.
        /// </summary>
        /// <param name="serviceProvider">Service provider interface.</param>
        /// <param name="configuration">Config file to be passed through.</param>
        protected QHHModuleBase(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            this.scope = serviceProvider.CreateScope();
            this.DataAccessLayer = this.scope.ServiceProvider.GetRequiredService<DataAccessLayer>();

            this.Configuration = configuration;
        }
    }
}
