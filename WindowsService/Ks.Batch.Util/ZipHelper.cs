using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Ks.Batch.Util
{
    public static class ZipHelper
    {
        public static void CreateZipFile(string directory, string pattern)
        {
            var files = Directory.EnumerateFiles(directory, "*.*", SearchOption.AllDirectories)
            .Where(s => s.EndsWith(".xlsx") || s.EndsWith(".txt")).ToList();

            var zipFile = Path.Combine(directory, pattern + ".zip");
            if (File.Exists(zipFile))
                File.Delete(zipFile);

            using (ZipArchive archive = ZipFile.Open(zipFile, ZipArchiveMode.Create))
            { 
                foreach (var fPath in files)
                {
                    archive.CreateEntryFromFile(fPath, Path.GetFileName(fPath));
                    File.Delete(fPath);
                }
            }
        }
    }
}
