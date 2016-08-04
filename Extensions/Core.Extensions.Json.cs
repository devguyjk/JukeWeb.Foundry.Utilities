using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace JukeWeb.Foundry.Utilities.Extensions
{
    public static partial class ExtensionMethods
    {
        public static JsonpResult ToJsonp(this JsonResult json)
        {
            return new JsonpResult { ContentEncoding = json.ContentEncoding, ContentType = json.ContentType, Data = json.Data, JsonRequestBehavior = json.JsonRequestBehavior };
        }
    }
}
