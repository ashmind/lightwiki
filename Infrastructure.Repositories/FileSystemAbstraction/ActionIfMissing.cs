using System;
using System.Collections.Generic;
using System.Linq;

namespace AshMind.LightWiki.Infrastructure.Repositories.FileSystemAbstraction {
    public enum ActionIfMissing {
        ReturnNull,
        ReturnAsIs,
        ThrowException,
        CreateNew
    }
}
