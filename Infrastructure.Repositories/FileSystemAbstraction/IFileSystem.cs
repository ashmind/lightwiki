using System;
using System.Collections.Generic;
using System.Linq;

namespace AshMind.LightWiki.Infrastructure.Repositories.FileSystemAbstraction {
    public interface IFileSystem {
        bool IsFileName(string pathOrFileName);
        
        string BuildPath(params string[] parts);

        IFile GetFile(string path, bool nullUnlessExists = true);

        bool IsLocation(string path);
        ILocation GetLocation(string path, ActionIfMissing actionIfMissing = ActionIfMissing.ThrowException);
    }
}
