using System.Collections.Generic;
using Microsoft.SharePoint.Administration;

namespace SPEllex.Server
{
    public static class ServerExtension
    {
        // Fields
        private const string WssAdministration = "WSS_Administration";

        // Methods
        public static bool IsWebFrontEndServer(this SPServer server)
        {
            if (server.Role == SPServerRole.Invalid) goto End;
            using (IEnumerator<SPServiceInstance> enumerator = server.ServiceInstances.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    SPServiceInstance current = enumerator.Current;
                    if (((current is SPWebServiceInstance) && (current.Name != WssAdministration)) &&
                        (current.Status != SPObjectStatus.Disabled))
                    {
                        return true;
                    }
                }
            }
            End:
            return false;
        }
    }
}