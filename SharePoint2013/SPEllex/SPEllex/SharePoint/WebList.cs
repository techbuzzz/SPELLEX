using System;
using System.Collections.Generic;
using Microsoft.SharePoint;

namespace SPEllex.SharePoint
{
    public class WebList : List<SPWeb>, IDisposable
    {
        public WebList()
        {
        }

        public WebList(List<SPWeb> list)
            : base(list)
        {
        }

        public void Dispose()
        {
            foreach (SPWeb web in this)
            {
                try
                {
                    web.Dispose();
                }
                catch (Exception ex)
                {
                    // Logging Extension at now was not created
                    throw;
                }
            }
        }
    }
}