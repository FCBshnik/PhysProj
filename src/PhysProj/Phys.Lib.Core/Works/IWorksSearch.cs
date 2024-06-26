﻿using Phys.Lib.Db.Works;

namespace Phys.Lib.Core.Works
{
    public interface IWorksSearch
    {
        List<WorkDbo> Find(string? search = null);

        List<WorkDbo> FindByAuthor(string authorCode);

        List<WorkDbo> FindCollected(string subWorkCode);

        WorkDbo? FindByCode(string code);

        List<WorkDbo> FindByCodes(ICollection<string> codes);
    }
}
