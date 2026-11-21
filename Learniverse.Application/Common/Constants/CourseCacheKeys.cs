using System.Globalization;

namespace Learniverse.Application.Common.Constants;

public static class CourseCacheKeys
{
    public const string PublicListTag = "courses:public-list";

    public static string PublicList(int pageNumber, int pageSize)
    {
        return string.Create(
            CultureInfo.InvariantCulture,
            $"courses:public-list:v1:page:{pageNumber}:size:{pageSize}");
    }
}
