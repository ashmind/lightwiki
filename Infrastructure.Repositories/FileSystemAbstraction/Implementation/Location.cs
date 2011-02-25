using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace AshMind.LightWiki.Infrastructure.Repositories.FileSystemAbstraction.Implementation {
    internal class Location : ILocation {
        public Location(string path) {
            this.Path = path;            
        }

        public IEnumerable<ILocation> GetLocations(bool recursive) {
            return Directory.EnumerateDirectories(this.Path, "*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                            .Select(path => new Location(path));
        }

        public IEnumerable<IFile> GetFiles() {
            return Directory.EnumerateFiles(this.Path)
                            .Select(name => new File(name, this));
        }

        public IFile GetFile(string name, bool nullUnlessExists = true) {
            var path = System.IO.Path.Combine(this.Path, name);
            if (nullUnlessExists && !System.IO.File.Exists(path))
                return null;

            return new File(path, this);
        }

        public ILocation GetLocation(string name, ActionIfMissing actionIfMissing = ActionIfMissing.ReturnNull) {
            var path = System.IO.Path.Combine(this.Path, name);
            return new FileSystem().GetLocation(path, actionIfMissing);
        }

        public bool IsHidden() {
            return new File(this.Path, this).IsHidden();
        }

        public void SetHidden(bool value) {
            new File(this.Path, this).SetHidden(value);
        }

        public string Name {
            get { return System.IO.Path.GetFileName(this.Path); }
        }

        public string Path { get; private set; }

        public override string ToString() {
            return this.Path;
        }
    }
}
