using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProviderPortal.Helpers
{
    public class LogOnHelper
    {
        public static bool CheckLogOn()
        {
            if (HttpContext.Current.Session["UserName"] != null && HttpContext.Current.Session["ProviderId"] != null)
            {
                if(HttpContext.Current.Session["UserName"].ToString() != "" && HttpContext.Current.Session["ProviderId"].ToString() != "")
                    return true;
            }
            return false;
        }
    }
}