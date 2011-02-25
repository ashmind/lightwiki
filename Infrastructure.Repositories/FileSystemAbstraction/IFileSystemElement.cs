using System;
using System.Collections.Generic;
using System.Linq;

namespace AshMind.LightWiki.Infrastructure.Repositories.FileSystemAbstraction {
    public interface IFileSystemElement {
        string Name { get; }
        string Path { get; }

        bool IsHidden();
        void SetHidden(bool value);
    }
}
