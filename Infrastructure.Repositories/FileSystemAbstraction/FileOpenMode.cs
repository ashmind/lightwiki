using System;
using System.Collections.Generic;
using System.Linq;

namespace AshMind.LightWiki.Infrastructure.Repositories.FileSystemAbstraction {
    public enum FileOpenMode {
        ReadOrWrite,
        Append,
        Recreate
    }
}
