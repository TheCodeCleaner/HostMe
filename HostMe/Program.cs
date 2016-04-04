using System;
using System.IO;
using System.Linq;
using System.Reflection;
using MicroService4Net;
using Newtonsoft.Json;

namespace HostMe
{
    class Program
    {
        static void Main(string[] args)
        {
            Configuration configuration = null;
            var defaultPort = 80;
            var configPath = Common.NormaliePath("config.json");

            if (args.Count() != 0)
            {
                var argsString = args.Aggregate((arg, current) => current + "," + arg);
                Common.WriteLog("args = " + argsString);
            }
            else
                Common.WriteLog("No args passed");


            if (File.Exists(configPath))
            {
                Common.WriteLog("Config found in " + configPath);
                var jsonConfig = File.ReadAllText(configPath);
                Common.WriteLog("The Config is: \n" + jsonConfig);
                try
                {
                    configuration = JsonConvert.DeserializeObject<Configuration>(jsonConfig);
                    Common.WriteLog("Configuration parsed");

                    if (configuration.Port == 0)
                    {
                        Common.WriteLog("No configuration port found. using port " + defaultPort);
                        configuration.Port = defaultPort;
                    }
                }
                catch (Exception exception)
                {
                    Common.WriteLog("Exception occured in configuratin parsing: " + exception);
                }
            }

            if (configuration == null)
            {
                Common.WriteLog("Using default configuration. Port = 80; Path = the exe path...");
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
