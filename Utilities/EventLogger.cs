using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Configuration;

namespace JukeWeb.Foundry.Utilities
{
	/// Summary description for Class1.
	public class EventLogger
	{
		public static void WriteWarning(string message, string source)
		{
		//	WriteWarning(string.Format(" / Source:{0}", message));
		}

		public static void WriteWarning(string message)
		{
		//	WriteLog(ConfigHelper.EventLogSource, message, EventLogEntryType.Warning);
		}

		public static void WriteInfo(string message)
		{
			//WriteLog(ConfigHelper.EventLogSource, message, EventLogEntryType.Information);
		}

		public static void WriteLog(string source, string message, EventLogEntryType type)
		{
            //    if (!EventLog.SourceExists(source))
            //        EventLog.CreateEventSource(source, "Application");

            //EventLog.WriteEntry(source, message, type);		
		}
	}
}
