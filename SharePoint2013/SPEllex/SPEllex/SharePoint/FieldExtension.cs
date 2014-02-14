using System;
using System.Linq;
using Microsoft.SharePoint;

namespace SPEllex.SharePoint
{
    public static class FieldExtension
    {
        public static bool Contains(this SPFieldCollection collection, Predicate<SPField> predicate)
        {
            return predicate != null && collection.Cast<SPField>().Any(field => predicate(field));
        }

        public static bool IsBuiltInField(this SPField field)
        {
            return SPBuiltInFieldId.Contains(field.Id);
        }

        public static bool IsFileLeafRefField(this SPField field)
        {
            return (field.Id == SPBuiltInFieldId.FileLeafRef);
        }

        public static bool IsTitleField(this SPField field)
        {
            return (field.Id == SPBuiltInFieldId.Title);
        }
    }
}