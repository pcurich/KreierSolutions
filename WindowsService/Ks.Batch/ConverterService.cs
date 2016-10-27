﻿using System;
using System.IO;
using System.Threading;
using Topshelf.Logging;

namespace Ks.Batch
{
    public class ConverterService
    {
        private static readonly LogWriter Log = HostLogger.Get<ConverterService>();
        private FileSystemWatcher _watcher;

        #region Start, Stop, Pause,Continue

        public bool Start()
        {
            _watcher = new FileSystemWatcher(@"c:\temp2\a", "*_in.txt");
            _watcher.Created += FileCreated;
            _watcher.IncludeSubdirectories = false;
            _watcher.EnableRaisingEvents = true;

            return true;
        }

        public bool Stop()
        {
            _watcher.Dispose();
            return true;
        }

        public bool Pause()
        {
            _watcher.EnableRaisingEvents = false;
            return true;
        }

        public bool Continue()
        {
            _watcher.EnableRaisingEvents = true;
            return true;
        }

        #endregion

        private void FileCreated(object sender, FileSystemEventArgs e)
        {
            Thread.Sleep(500);

            Log.InfoFormat("Starting conversion of '{0}'", e.FullPath);

            if (e.FullPath.Contains("bad_in"))
            {
                throw new NotSupportedException("Cannot convert");
            }

            var content = File.ReadAllText(e.FullPath);
            var upperContent = content.ToUpperInvariant();
            var dir = Path.GetDirectoryName(e.FullPath);
            var convertedFileName = Path.GetFileName(e.FullPath) + ".converted";
            if (dir != null)
            {
                var convertedPath = Path.Combine(dir, convertedFileName);
                File.WriteAllText(convertedPath, upperContent);
            }
        }

        public void CustomCommand(int commandNumber)
        {
            //128-255
            Log.InfoFormat("Starting Convertion of '{0}' ",commandNumber);
        }
    }
}
