using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using JukeWeb.Foundry.Utilities.Properties;

namespace JukeWeb.Foundry.Utilities
{
	public class ConfigHelper : ApplicationSettingsBase
	{
		private static Settings _settings = null;

		static ConfigHelper() 
		{
			_settings = new Settings();
		}

		public static string EventLogSource
		{
			get
			{
				return _settings.EventLogSource;
			}
		}

		public static string EventLogApplication
		{
			get
			{
				return _settings.EventLogApplication;
			}
		}

        public static string SMSMaxLength
        {
            get 
            {
                return _settings["SMSMaxLength"].ToString();
            }
        }
	}
}
