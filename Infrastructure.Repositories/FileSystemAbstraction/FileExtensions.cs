using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AshMind.LightWiki.Infrastructure.Repositories.FileSystemAbstraction {
    public static class FileExtensions {
        public static string ReadAllText(this IFile file) {
            using (var stream = file.Read(FileLockMode.Write))
            using (var reader = new StreamReader(stream)) {
                return reader.ReadToEnd();
            }
        }

        public static IEnumerable<string> ReadAllLines(this IFile file) {
            using (var stream = file.Read(FileLockMode.Write))
            using (var reader = new StreamReader(stream)) {
                var line = reader.ReadLine();
                while (line != null) {
                    yield return line;
                    line = reader.ReadLine();
                }
            }
        }

        public static void AppendAllText(this IFile file, string contents) {
            using (var stream = file.Open(FileLockMode.Write, FileOpenMode.Append))
            using (var writer = new StreamWriter(stream, new UTF8Encoding(false, true))) {
                writer.Write(contents);
            }
        }

        public static void WriteAllText(this IFile file, string contents) {
            using (var stream = file.Open(FileLockMode.Write, FileOpenMode.Recreate))
            using (var writer = new StreamWriter(stream, new UTF8Encoding(false, true))) {
                writer.Write(contents);
            }
        }
    }
}
