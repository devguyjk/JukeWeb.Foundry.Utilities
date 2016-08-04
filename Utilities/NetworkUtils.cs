using System;
using System.IO;
using System.DirectoryServices.ActiveDirectory;
using System.DirectoryServices;  
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using JukeWeb.Foundry.Exceptions;

namespace JukeWeb.Foundry.Utilities
{
    public class NetworkUtils
    {

        public static StringCollection GetDomainList()
        {
            StringCollection domainList = new StringCollection();
            try
            {
                DirectoryEntry en = new DirectoryEntry("LDAP://");
                // Search for objectCategory type "Domain"
                DirectorySearcher srch = new DirectorySearcher("objectCategory=Domain");
                SearchResultCollection coll = srch.FindAll();
                // Enumerate over each returned domain.
                foreach (SearchResult rs in coll)
                {
                    ResultPropertyCollection resultPropColl = rs.Properties;
                    foreach (object domainName in resultPropColl["name"])
                    {
                        domainList.Add(domainName.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new GeneralException(ex.Message, ex);
            }
            return domainList;
        }

        public static string GetCurrentDomainName()
        {
            Forest currentForest = null;
            try
            {
                currentForest = Forest.GetCurrentForest();
                return currentForest.Name;
            }
            catch
            {
                return "";
            }
        }

        public static string GetMachineName()
        {
            return GetMachineName(false);
        }

        public static string GetMachineName(bool fullName)
        {
            string domain = (string.IsNullOrEmpty((GetCurrentDomainName())))?"":"."+GetCurrentDomainName();
            return (fullName)?string.Format("{0}{1}", Environment.MachineName, domain):Environment.MachineName;
        }
    }   
}
