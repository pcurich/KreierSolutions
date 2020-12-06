using System.Configuration;
using System.IO;
using System.Threading;
using Topshelf.Logging;

namespace Ks.Batch.Ftp.Upload
{
    public class Watcher
    {
        private static readonly LogWriter Log = HostLogger.Get<Watcher>();
        private static string _host;
        private static string _username;
        private static string _password;
        private static string _root;
        private static string _path;
        private static string _moved;

        public static void FileCreated(object sender, FileSystemEventArgs e)
        {
            _host = ConfigurationManager.AppSettings["Host"];
            _username = ConfigurationManager.AppSettings["User"];
            _password = ConfigurationManager.AppSettings["Password"];
            _root = ConfigurationManager.AppSettings["Root"];
            _path = ConfigurationManager.AppSettings["Path"];
            _moved = ConfigurationManager.AppSettings["Moved"];

            Thread.Sleep(1000 * 3); //3 Sec because is not atomic

            var ftpClient = new FtpClient(hostIP: _host, userName: _username, password: _password);

            var result = ftpClient.Upload(string.Format("{0}/{1}", _root, e.Name), e.FullPath);

            if (result) {
                string destiny = Path.Combine(_path,_moved, e.Name);
                File.Move(e.FullPath, destiny);
            }
        }
    }
}
