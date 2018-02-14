using System.Collections.Generic;

namespace Server.Validation.Util
{
    public static class NullValueChecker
    {
        public static void CheckValueForNull(this string stringValue, string attributeName, ref List<string> attributeNames)
        {
            if (string.IsNullOrEmpty(stringValue))
                attributeNames.Add(attributeName);
        }

        public static void CheckValueForDefault(this int intValue, string attributeName,
            ref List<string> attributeNames)
        {
            if (intValue == default(int))
                attributeNames.Add(attributeName);
        }
    }
}