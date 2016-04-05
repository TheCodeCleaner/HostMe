using System;
using System.IO;
using System.Linq;
using System.Reflection;
using log4net;
using MicroService4Net;
using Newtonsoft.Json;

namespace HostMe
{
    class Program
    {
        private static readonly ILog _logger = Logger.GetLogger();

        static void Main(string[] args)
        {
            Configuration configuration = null;
            var defaultPort = 80;
            var configPath = PathNormalizer.NormaliePath("config.json");

            if (args.Count() != 0)
            {
                var argsString = args.Aggregate((arg, current) => current + "," + arg);
                _logger.Info("args = " + argsString);
            }
            else
                _logger.Info("No args passed");


            if (File.Exists(configPath))
            {
                _logger.Info("Config found in " + configPath);
                var jsonConfig = File.ReadAllText(configPath);
                _logger.Info("The Config is: \n" + jsonConfig);
                try
                {
                    configuration = JsonConvert.DeserializeObject<Configuration>(jsonConfig);
                    _logger.Info("Configuration parsed");

                    if (configuration.Port == 0)
                    {
                        _logger.Info("No configuration port found. using port " + defaultPort);
                        configuration.Port = defaultPort;
                    }
                }
                catch (Exception exception)
                {
                    _logger.Error("Exception occured in configuratin parsing", exception);
                }
            }

            if (configuration == null)
            {
                _logger.Info("Using default configuration. Port = 80; Path = the exe path...");
                configuration = new Configuration { Port = defaultPort };
            }

            var port = configuration.Port;
            StaticContentController.SiteRootPath = configuration.Path;

            var serviceName = Assembly.GetEntryAssembly().GetName().Name + "_Port_" + port;
            var microService = new MicroService(port, serviceName, serviceName);
            microService.Run(args);
        }
    }
}
