﻿using Phys.Lib.Db.Files;

namespace Phys.Lib.Core.Files
{
    public interface IFilesSearch
    {
        List<FileDbo> Find(string? search = null);

        List<FileDbo> FindByCodes(IEnumerable<string> codes);

        FileDbo? FindByCode(string code);
    }
}
