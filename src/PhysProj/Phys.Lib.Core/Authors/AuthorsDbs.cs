using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Phys.Lib.Db.Authors;
using Phys.Shared;

namespace Phys.Lib.Core.Authors
{
    internal class AuthorsDbs : IAuthorsDbs
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<AuthorsDbs> log;
        private readonly Lazy<IAuthorsDb> db;

        public string Name => "main";

        public AuthorsDbs(IEnumerable<IAuthorsDb> dbs, IConfiguration configuration, ILogger<AuthorsDbs> log)
        {
            this.configuration = configuration;
            this.log = log;

            db = new Lazy<IAuthorsDb>(() => GetDb(dbs));
        }

        public void Create(string code)
        {
            db.Value.Create(code);
        }

        public void Delete(string code)
        {
            db.Value.Delete(code);
        }

        public List<AuthorDbo> Find(AuthorsDbQuery query)
        {
            return db.Value.Find(query);
        }

        public void Update(string code, AuthorDbUpdate update)
        {
            db.Value.Update(code, update);
        }

        private IAuthorsDb GetDb(IEnumerable<IAuthorsDb> dbs)
        {
            var dbName = configuration.GetConnectionStringOrThrow("db");
            log.LogInformation($"use db '{dbName}' as {typeof(IAuthorsDb)}");
            return dbs.FirstOrDefault(db => db.Name == dbName && dbName != Name)
                ?? throw new PhysException($"authors db '{dbName}' not found");
        }
    }
}
