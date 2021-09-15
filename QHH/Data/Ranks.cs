namespace QHH.Data
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using QHH.Data.Context;
    using QHH.Data.Models;

    public class Ranks
    {
        private readonly QHHDbContext context;

        public Ranks(QHHDbContext context)
        {
            context = context;
        }

        public async Task<List<Rank>> GetRanksAsync(ulong id)
        {
            var ranks = await this.context.Ranks
                .AsQueryable()
                .Where(x => x.ServerId == id)
                .ToListAsync();

            return await Task.FromResult(ranks);
        }

        public async Task AddRankAsync(ulong id, ulong roleId)
        {
            var server = await this.context.Servers
                .FindAsync(id);

            if (server == null)
                this.context.Add(new Server { Id = id });

            this.context.Add(new Rank { RoleId = roleId, ServerId = id });
            await this.context.SaveChangesAsync();
        }

        public async Task RemoveRankAsync(ulong id, ulong roleId)
        {
            var rank = await this.context.Ranks
                .AsQueryable()
                .Where(x => x.RoleId == roleId)
                .FirstOrDefaultAsync();

            this.context.Remove(rank);
            await this.context.SaveChangesAsync();
        }

        public async Task ClearRanksAsync(List<Rank> ranks)
        {
            this.context.RemoveRange(ranks);
            await this.context.SaveChangesAsync();
        }
    }
}