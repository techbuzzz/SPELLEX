using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Administration;

namespace SPEllex.SharePoint.Utils
{
    public class CustomJob
    {
        public void DeleteJobAndSettings<T>(SPWebApplication webApplication)
        {
            using (IEnumerator<SPJobDefinition> enumerator = webApplication.JobDefinitions.GetEnumerator())
            {
                SPJobDefinition current;
                while (enumerator.MoveNext())
                {
                    current = enumerator.Current;
                    if (current is T)
                    {
                        goto Label_Exist;
                    }
                }
                return;
            Label_Exist:
                current.Delete();
            }
        }
    }
}
