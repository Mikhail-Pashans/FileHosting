using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace FileHosting.MVC.Helpers
{
    public class FileThrottleResult : FilePathResult
    {
        private readonly string _contentType;
        private readonly string _fileName;
        private readonly string _pathToFile;
        private readonly decimal _rate;

        public FileThrottleResult(string pathToFile, string fileName, decimal rate, string contentType)
            : base(pathToFile, contentType)
        {
            _pathToFile = pathToFile;
            _fileName = fileName;
            _rate = rate;
            _contentType = contentType;
        }

        protected override void WriteFile(HttpResponseBase response)
        {
            var bufferSize = (int) Math.Truncate(1024*_rate);
            var buffer = new byte[bufferSize];

            Stream outputStream = response.OutputStream;

            using (var stream = new FileStream(_pathToFile, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize))
            {
                response.AppendHeader("Cache-control", "private");
                response.AppendHeader("Content-Type", _contentType);
                response.AppendHeader("Content-Length", stream.Length.ToString(CultureInfo.InvariantCulture));
                response.AppendHeader("Content-Disposition", "attachment;filename=" + _fileName);
                response.Flush();

                while (true)
                {
                    if (!response.IsClientConnected)
                        break;

                    int bytesRead = stream.Read(buffer, 0, bufferSize);

                    if (bytesRead == 0)
                        break;

                    outputStream.Write(buffer, 0, bytesRead);
                    response.Flush();
                    Thread.Sleep(1000);
                }
            }
        }
    }
}