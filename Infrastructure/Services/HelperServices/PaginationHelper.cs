namespace Infrastructure.Services.HelperServices;

public static class PaginationHelper
{
    public static void Normalize(ref int pageNumber, ref int pageSize)
    { 
        if (pageNumber <= 0) pageNumber = 1;
        if (pageSize <= 0) pageSize = 10;
    }
}