using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace GravataOnlineAuth.Common
{
    public class MyLog
    {
        log4net.Repository.ILoggerRepository logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
        ILog logger = LogManager.GetLogger(typeof(Program));
        public MyLog()
        {
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
        }
        public void GerarLog(string metodo, string mensagem)
        {
            logger.Info("Método: " + metodo + " - Mensagem:" + mensagem);
            
        }

        public void GerarLog(string metodo, string mensagem, Exception ex)
        {
            logger.Error("Método: " + metodo + " - Mensagem:" + mensagem, ex);            
        }

    }
}
