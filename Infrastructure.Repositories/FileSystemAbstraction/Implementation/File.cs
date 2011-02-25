using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AshMind.LightWiki.Infrastructure.Repositories.FileSystemAbstraction.Implementation {
    internal class File : IFile {
        private static readonly IDictionary<FileLockMode, FileShare> fileShare = new Dictionary<FileLockMode, FileShare> {
            { FileLockMode.None,        FileShare.ReadWrite },
            { FileLockMode.Read,        FileShare.Write },
            { FileLockMode.Write,       FileShare.Read },
            { FileLockMode.ReadWrite,   FileShare.None },
        };

        private static readonly IDictionary<FileOpenMode, FileMode> fileMode = new Dictionary<FileOpenMode, FileMode> {
            { FileOpenMode.Append,       FileMode.Append },
            { FileOpenMode.ReadOrWrite,  FileMode.OpenOrCreate },
            { FileOpenMode.Recreate,     FileMode.Create }
        };

        private static readonly IDictionary<FileOpenMode, FileAccess> fileAccess = new Dictionary<FileOpenMode, FileAccess> {
            { FileOpenMode.Append,       FileAccess.Write },
            { FileOpenMode.ReadOrWrite,  FileAccess.ReadWrite },
            { FileOpenMode.Recreate,     FileAccess.Write }
        };

        private readonly string path;

        public File(string path, Location location) {
            this.path = path;
            this.Location = location;
        }
        
        public bool IsHidden() {
            return System.IO.File.GetAttributes(this.path).HasFlag(FileAttributes.Hidden);
        }

        public void SetHidden(bool value) {
            var attributes = System.IO.File.GetAttributes(this.path);
            if (value) {
                attributes |= FileAttributes.Hidden;
            }
            else {
                attributes &= ~FileAttributes.Hidden;
            }

            System.IO.File.SetAttributes(this.path, attributes);
        }

        public Stream Read(FileLockMode lockMode) {
            return System.IO.File.Open(this.path, FileMode.Open, FileAccess.Read, fileShare[lockMode]);
        }

        public Stream Open(FileLockMode lockMode, FileOpenMode openMode) {
            return System.IO.File.Open(
                this.path,
                fileMode[openMode],
                fileAccess[openMode],
                fileShare[lockMode]
            );
        }

        public DateTimeOffset GetLastWriteTime() {
            return System.IO.File.GetLastWriteTimeUtc(this.path);
        }

        public string Name {
            get { return System.IO.Path.GetFileName(this.path); }
        }

        public string NameWithoutExtension {
            get { return System.IO.Path.GetFileNameWithoutExtension(this.path); }
        }

        public string Path {
            get { return this.path; }
        }

        public bool Exists {
            get { return System.IO.File.Exists(this.path); }
        }

        public ILocation Location { get; private set; }
    }
}
