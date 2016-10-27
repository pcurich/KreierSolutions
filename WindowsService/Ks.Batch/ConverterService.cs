using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ks.Batch
{
    public class ConverterService
    {
        private FileSystemWatcher _watcher;

        public bool Start()
        {
            using (var writer = new StreamWriter("log.txt", true))
            {
                try
                {

                    _watcher = new FileSystemWatcher(@"C:\KsService\dir", "*_in.txt");
                    _watcher.Created += FileCreated;
                    _watcher.IncludeSubdirectories = false;
                    _watcher.EnableRaisingEvents = true;

                    writer.WriteLine("Ok");
                }
                catch (Exception e)
                {
                    writer.WriteLine(e.Message.ToString());
                }
            }
            return true;
        }

        private void FileCreated(object sender, FileSystemEventArgs e)
        {
            using (var writer = new StreamWriter("FileCreated.txt", true))
            {
                var content = File.ReadAllText(e.FullPath);
                writer.WriteLine(content);
                
                var upperContent = content.ToUpperInvariant();
                writer.WriteLine(upperContent);
                
                var dir = Path.GetDirectoryName(e.FullPath);
                writer.WriteLine(dir);
                
                var convertedFileName = Path.GetFileName(e.FullPath) + ".converted";
                writer.WriteLine(convertedFileName);

                if (dir != null)
                {
                    var convertedPath = Path.Combine(dir, convertedFileName);
                    writer.WriteLine(convertedPath);
                    File.WriteAllText(convertedPath, upperContent);
                }
            }
            
        }

        public bool Stop()
        {
            _watcher.Dispose();
            return true;
        }
    }
}
