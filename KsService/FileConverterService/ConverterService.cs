using System.IO;
using System.Xml.Schema;

namespace FileConverterService
{
    public class ConverterService
    {
        private FileSystemWatcher _watcher;

        public bool Start()
        {
            _watcher = new FileSystemWatcher(@"D:\KsService", "*_in.txt");
            _watcher.Created += FileCreated;
            _watcher.IncludeSubdirectories = false;
            _watcher.EnableRaisingEvents = true;
            return true;
        }

        private void FileCreated(object sender , FileSystemEventArgs e)
        {
            var content = File.ReadAllText(e.FullPath);
            var upperContent = content.ToUpperInvariant();
            var dir = Path.GetDirectoryName(e.FullPath);
            var convertedFileName = Path.GetFileName(e.FullPath) + ".converted";
            if (dir != null)
            {
                var convertedPath = Path.Combine(dir, convertedFileName);
                File.WriteAllText(convertedPath,upperContent);
            }
        }

        public bool Stop()
        {
            _watcher.Dispose();
            return true;
        }
    }
}