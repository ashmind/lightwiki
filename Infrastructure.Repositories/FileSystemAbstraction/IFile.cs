using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace AshMind.LightWiki.Infrastructure.Repositories.FileSystemAbstraction {
    public interface IFile : IFileSystemElement {
        string NameWithoutExtension { get; }
        DateTimeOffset GetLastWriteTime();

        Stream Read(FileLockMode lockMode);
        Stream Open(FileLockMode lockMode, FileOpenMode openMode);

        bool Exists { get; }
        
        ILocation Location { get; }
    }
}
