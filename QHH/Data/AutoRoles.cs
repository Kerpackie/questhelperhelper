using Microsoft.EntityFrameworkCore;
using QHH.Data.Context;
using QHH.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QHH.Data
{
    public class AutoRoles
    {
        private readonly QHHDbContext context;

        public AutoRoles(QHHDbContext context)
        {
            this.context = context;
        }

        public async Task<List<AutoRole>> GetAutoRolesAsync(ulong id)
        {
            var autoRoles = await this.context.AutoRoles
                .AsQueryable()
                .Where(x => x.ServerId == id)
                .ToListAsync();

            return await Task.FromResult(autoRoles);
        }

        public async Task AddAutoRoleAsync(ulong id, ulong roleId)
        {
            var server = await this.context.Servers
                .FindAsync(id);

            if (server == null)
            {
                this.context.Add(new Server { Id = id });
            }

            this.context.Add(new AutoRole { RoleId = roleId, ServerId = id });
            await this.context.SaveChangesAsync();
        }

        public async Task RemoveAutoRoleAsync(ulong id, ulong roleId)
        {
            var autoRole = await this.context.AutoRoles
                .AsQueryable()
                .Where(x => x.RoleId == roleId)
                .FirstOrDefaultAsync();

            this.context.Remove(autoRole);
            await this.context.SaveChangesAsync();
        }

        public async Task ClearAutoRolesAsync(List<AutoRole> autoRoles)
        {
            this.context.RemoveRange(autoRoles);
            await this.context.SaveChangesAsync();
        }
    }
}
