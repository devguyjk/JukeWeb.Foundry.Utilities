using System;
using System.Reflection;
using System.Configuration;
using System.Configuration.Assemblies;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Configuration;

using JukeWeb.Foundry.Exceptions;

namespace JukeWeb.Foundry.Utilities
{
    public class WCFUtils
    {
        public static bool EndpointExists(string endpointName)
        {
            return (GetEndpointElement(endpointName) != null);
        }

        public static ChannelEndpointElement GetEndpointElement(string endpointName)
        {
            try
            {
                ExeConfigurationFileMap map = new ExeConfigurationFileMap();
                map.ExeConfigFilename = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;

                Configuration config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
                ServiceModelSectionGroup group = ServiceModelSectionGroup.GetSectionGroup(config);

                if (group != null)
                {
                    ConfigurationSection css = group.Sections["client"];
                    if (css != null && css is ClientSection)
                    {
                        foreach (ChannelEndpointElement endpoint in ((ClientSection)css).Endpoints)
                        {
                            if (endpoint.Name.ToLower() == endpointName.ToLower())
                            {
                                return endpoint;
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new GeneralException(ex.Message, ex);
            }
        }
    }
}
