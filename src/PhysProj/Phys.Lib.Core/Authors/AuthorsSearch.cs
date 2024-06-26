﻿using Phys.Lib.Db.Authors;

namespace Phys.Lib.Core.Authors
{
    internal class AuthorsSearch : IAuthorsSearch
    {
        private readonly IAuthorsDb db;

        public AuthorsSearch(IAuthorsDb db)
        {
            this.db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public AuthorDbo? FindByCode(string code)
        {
            ArgumentNullException.ThrowIfNull(nameof(code));

            return db.Find(new AuthorsDbQuery { Code = code }).FirstOrDefault();
        }

        public List<AuthorDbo> Find(string? search = null)
        {
            return db.Find(new AuthorsDbQuery { Search = search, Limit = 20 });
        }

        public List<AuthorDbo> FindByCodes(List<string> codes)
        {
            ArgumentNullException.ThrowIfNull(codes);

            return db.Find(new AuthorsDbQuery { Codes = codes });
        }
    }
}
