using Mao.Web.Features.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Http.ExceptionHandling;

namespace Mao.Web.Features
{
    public class DefaultExceptionLogger : ExceptionLogger
    {
        private readonly AppSettings _appSettings;
        public DefaultExceptionLogger(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public override void Log(ExceptionLoggerContext context)
        {
            string logPath = HostingEnvironment.MapPath(_appSettings.LogPath);
            System.IO.Directory.CreateDirectory(logPath);
            string logFilePath = System.IO.Path.Combine(logPath, $"{DateTime.Today:yyyy-MM-dd}.txt");
            System.IO.File.AppendAllText(logFilePath, $@"
{DateTime.Now:HH:mm:ss}
{context.Exception.Message}
{context.Exception}
");
        }
    }
}