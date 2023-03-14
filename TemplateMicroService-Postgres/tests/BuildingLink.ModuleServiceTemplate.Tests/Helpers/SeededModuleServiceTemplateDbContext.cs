using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BuildingLink.ModuleServiceTemplate.Data;
using Microsoft.EntityFrameworkCore;

namespace BuildingLink.ModuleServiceTemplate.Tests.Helpers
{
    public static class SeededModuleServiceTemplateDbContext
    {
        public static CodeFirstDbContext
           BuildModuleServiceTemplateDbContext()
        {
            var dbContext = new CodeFirstDbContext(
                new DbContextOptionsBuilder<CodeFirstDbContext>()
                    .UseInMemoryDatabase($"ModuleServiceTemplateDb-{Guid.NewGuid():N}")
                    .Options);

            return dbContext;
        }

        public static async Task<CodeFirstDbContext>
            BuildModuleServiceTemplateDbContextAsync<TEntity>(IEnumerable<TEntity> entities)
            where TEntity : class
        {
            var dbContext = BuildModuleServiceTemplateDbContext();

            await dbContext.Set<TEntity>().AddRangeAsync(entities);
            await dbContext.SaveChangesAsync();

            return dbContext;
        }
    }
}
