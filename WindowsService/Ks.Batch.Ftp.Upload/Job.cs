using Quartz;
using System;
using System.Configuration;
using System.IO;
using System.Threading;

namespace Ks.Batch.Ftp.Upload
{
    public class Job : IJob
    {
        private string _sysName;
        private string _host;
        private string _username;
        private string _password;
        private string _root;
        private string _path;
        private string _moved;
        private string _ext;

        public Job()
        {
            _sysName = ConfigurationManager.AppSettings["SysName"];
            _host = ConfigurationManager.AppSettings["Host"];
            _username = ConfigurationManager.AppSettings["User"];
            _password = ConfigurationManager.AppSettings["Password"];
            _root = ConfigurationManager.AppSettings["Root"];
            _path = ConfigurationManager.AppSettings["Path"];
            _moved = ConfigurationManager.AppSettings["Moved"];
            _ext = ConfigurationManager.AppSettings["Ext"];
        }


        public void Execute(IJobExecutionContext context)
        {
            string[] files = Directory.GetFiles(_path, _ext);

            foreach (var file in files)
            {
                Thread.Sleep(1000 * 3); //3 Sec because is not atomic

                var ftpClient = new FtpClient(hostIP: _host, userName: _username, password: _password);

                var result = ftpClient.Upload(string.Format("{0}/{1}", _root, file.Replace(_path, "")), file);

                if (result)
                {
                    string destiny = Path.Combine(_path, _moved, file.Replace(_path, ""));
                    File.Move(file, destiny);
                }
            }
        }
    }
}
