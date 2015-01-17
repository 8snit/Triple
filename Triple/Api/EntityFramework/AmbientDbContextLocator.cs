using System.Data.Entity;

namespace Triple.Api.EntityFramework
{
    public class AmbientDbContextLocator : IAmbientDbContextLocator
    {
        public TDbContext Get<TDbContext>() where TDbContext : DbContext
        {
            var ambientDbContextScope = DbContextScope.GetAmbientScope();
            return ambientDbContextScope == null ? null : ambientDbContextScope.DbContexts.Get<TDbContext>();
        }
    }
}
