using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace Triple.Api
{
    public class AppSettings
    {
        public static string LogDirectory
        {
            get
            {
                return ResolveDirectory("log", @"C:\inetpub\logs");
            }
        }

        public static string TemporaryDirectory
        {
            get
            {
                return ResolveDirectory("tmp", @"C:\inetpub\temp");
            }
        }

        private static string ResolveDirectory(string which, string defaultDirectoryName)
        {
            var key = string.Format("triple.{0}dir", which);
            try
            {
                var directoryName = ConfigurationManager.AppSettings[key];

                if (string.IsNullOrEmpty(directoryName))
                {
                    directoryName = defaultDirectoryName;
                }

                if (!directoryName.EndsWith("\\"))
                {
                    directoryName += "\\";
                }

                if (directoryName.StartsWith("~\\"))
                {
                    directoryName = directoryName.Replace("~\\", ConfigurationManager.AppSettings["triple.appdir"]);
                }

                if (!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }

                return directoryName;
            }
            catch (Exception ex)
            {
                throw new AppSettingsException(string.Format("directory configuration '{0}' not valid", key), ex);
            }
        }
    }
}
