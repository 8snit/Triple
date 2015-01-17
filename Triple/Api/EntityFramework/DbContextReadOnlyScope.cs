using System.Data;

namespace Triple.Api.EntityFramework
{
    public class DbContextReadOnlyScope : IDbContextReadOnlyScope
    {
        private readonly DbContextScope _internalScope;

        public DbContextReadOnlyScope(IDbContextFactory dbContextFactory = null)
            : this(DbContextScopeOption.JoinExisting, null, dbContextFactory)
        {
        }

        public DbContextReadOnlyScope(IsolationLevel isolationLevel, IDbContextFactory dbContextFactory = null)
            : this(DbContextScopeOption.ForceCreateNew, isolationLevel, dbContextFactory)
        {
        }

        public DbContextReadOnlyScope(DbContextScopeOption joiningOption, IsolationLevel? isolationLevel,
            IDbContextFactory dbContextFactory = null)
        {
            this._internalScope = new DbContextScope(joiningOption, true, isolationLevel, dbContextFactory);
        }

        public IDbContextCollection DbContexts
        {
            get
            {
                return this._internalScope.DbContexts;
            }
        }

        public void Dispose()
        {
            this._internalScope.Dispose();
        }
    }
}
