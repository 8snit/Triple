using System;
using System.Data;

namespace Triple.Api.EntityFramework
{
    public class DbContextScopeFactory : IDbContextScopeFactory
    {
        private readonly IDbContextFactory _dbContextFactory;

        public DbContextScopeFactory()
        {
            this._dbContextFactory = null;
        }

        //public DbContextScopeFactory(IDbContextFactory dbContextFactory)
        //{
        //    this._dbContextFactory = dbContextFactory;
        //}

        public IDbContextScope Create(DbContextScopeOption joiningOption = DbContextScopeOption.JoinExisting)
        {
            return new DbContextScope(joiningOption, false, null, this._dbContextFactory);
        }

        public IDbContextReadOnlyScope CreateReadOnly(
            DbContextScopeOption joiningOption = DbContextScopeOption.JoinExisting)
        {
            return new DbContextReadOnlyScope(joiningOption, null, this._dbContextFactory);
        }

        public IDbContextScope CreateWithTransaction(IsolationLevel isolationLevel)
        {
            return new DbContextScope(DbContextScopeOption.ForceCreateNew, false, isolationLevel, this._dbContextFactory);
        }

        public IDbContextReadOnlyScope CreateReadOnlyWithTransaction(IsolationLevel isolationLevel)
        {
            return new DbContextReadOnlyScope(DbContextScopeOption.ForceCreateNew, isolationLevel,
                this._dbContextFactory);
        }

        public IDisposable SuppressAmbientContext()
        {
            return new AmbientContextSuppressor();
        }
    }
}
