using Microsoft.EntityFrameworkCore;
using QHH.Data.Context;
using QHH.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QHH.Data
{
    public class DataAccessLayer
    {
        private readonly QHHDbContext dbContext;

        public DataAccessLayer(QHHDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<FAQ> GetFAQ(string faqName)
        {
            return await this.dbContext.FAQs
                .AsQueryable()
                .FirstOrDefaultAsync(x => x.Name == faqName);
        }

        public async Task<IEnumerable<FAQ>> GetFAQs()
        {
            return await this.dbContext.FAQs
                .AsQueryable()
                .ToListAsync();
        }

        public async Task CreateFaq(string name, ulong ownerId, string content)
        {
            var faq = await this.dbContext.FAQs
                .AsQueryable()
                .FirstOrDefaultAsync(x => x.Name == name);

            if (faq != null)
            {
                return;
            }

            this.dbContext.Add(new FAQ
            {
                Name = name,
                OwnerId = ownerId,
                Content = content,
            });

            await this.dbContext.SaveChangesAsync();
        }

        public async Task EditFAQContent(string faqName, string content)
        {
            var faq = await this.dbContext.FAQs
                .AsQueryable()
                .FirstOrDefaultAsync(x => x.Name == faqName);

            if (faqName == null)
            {
                return;
            }

            faq.Content = content;
            await this.dbContext.SaveChangesAsync();
        }

        public async Task EditFAQOwner(string faqName, ulong ownerId)
        {
            var faq = await this.dbContext.FAQs
                .AsQueryable()
                .FirstOrDefaultAsync(x => x.Name == faqName);

            if (faq == null)
            {
                return;
            }

            faq.OwnerId = ownerId;
            await this.dbContext.SaveChangesAsync();
        }

        public async Task DeleteFAQ(string faqName)
        {
            var faq = await this.dbContext.FAQs
                .AsQueryable()
                .FirstOrDefaultAsync(x => x.Name == faqName);

            if (faq == null)
            {
                return;
            }

            this.dbContext.Remove(faq);
            await this.dbContext.SaveChangesAsync();
        }
    }
}
