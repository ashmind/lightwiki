using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace AshMind.LightWiki.Infrastructure.Repositories.FileSystemAbstraction.Implementation {
    internal class FileSystem : IFileSystem {
        public IFile GetFile(string path, bool nullUnlessExists = true) {
            return this.GetLocation(Path.GetDirectoryName(path))
                       .GetFile(Path.GetFileName(path), nullUnlessExists);
        }
       
        public string BuildPath(params string[] parts) {
            return Path.Combine(parts);
        }

        public bool IsFileName(string pathOrFileName) {
            return pathOrFileName.IndexOfAny(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }) < 0;
        }

        public bool IsLocation(string path) {
            return Directory.Exists(path);
        }

        public ILocation GetLocation(string path, ActionIfMissing actionIfMissing = ActionIfMissing.ThrowException) {
            if (!Directory.Exists(path) && actionIfMissing != ActionIfMissing.ReturnAsIs) {
                if (actionIfMissing == ActionIfMissing.CreateNew) {
                    Directory.CreateDirectory(path);
                }
                else if (actionIfMissing == ActionIfMissing.ReturnNull) {
                    return null;
                }
                else if (actionIfMissing == ActionIfMissing.ThrowException) {
                    throw new FileNotFoundException("Location was not found.", path);
                }
            }

            return new Location(path);
        }
    }
}