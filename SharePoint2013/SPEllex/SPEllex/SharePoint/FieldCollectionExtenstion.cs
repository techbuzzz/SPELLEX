using System;
using System.Collections;
using System.Globalization;
using Microsoft.SharePoint;

namespace SPEllex.SharePoint
{
    public static class FieldCollectionExtenstion
    {
        public static SPField TryGetFieldByDisplayName(this SPFieldCollection fields, string title, int culture)
        {
            SPField field = null;
            CultureInfo cultureInfo = CultureInfo.GetCultureInfo(culture);
            IEnumerator enumerator = fields.GetEnumerator();
            SPField current;
            while (enumerator.MoveNext())
            {
                current = (SPField) enumerator.Current;
                string valueForUICulture = current.TitleResource.GetValueForUICulture(cultureInfo);
                if (title.Equals(valueForUICulture, StringComparison.OrdinalIgnoreCase))
                {
                    goto Success;
                }
            }
            return null;
            Success:
            field = current;

            return field;
        }

        public static SPField TryGetFieldByInternalName(this SPFieldCollection collection, string internalName)
        {
            SPField field = null;
            IEnumerator enumerator = collection.GetEnumerator();

            SPField current;
            while (enumerator.MoveNext())
            {
                current = (SPField) enumerator.Current;
                if (current.InternalName == internalName)
                {
                    goto Success;
                }
            }
            return null;
            Success:
            field = current;
            return field;
        }
    }
}