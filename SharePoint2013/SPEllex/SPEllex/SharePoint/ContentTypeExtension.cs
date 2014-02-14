using Microsoft.SharePoint;

namespace SPEllex.SharePoint
{
    public static class ContentTypeExtension
    {
        //need dictionary all ContentTypesID

        private const string String0 = "0x0120";
        private const string String1 = "0x0107";

        public static bool IsFolderContentType(this SPContentType contentType)
        {
            return Check(contentType.Id, String0);
        }

        public static bool IsMessageContentType(this SPContentType contentType)
        {
            return Check(contentType.Id, String1);
        }

        private static bool Check(SPContentTypeId id, string baseIdString)
        {
            var id2 = new SPContentTypeId(baseIdString);
            if (!(id == id2))
            {
                return id.IsChildOf(id2);
            }
            return true;
        }
    }
}