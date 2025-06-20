namespace Infrastructure.Services;

public static class FilterDefaultSettings
{
    public static void FilterDefaultSettingsMethod(ref int pageNumber, ref int pageSize)
    { 
        if (pageNumber <= 0) pageNumber = 1;
        if (pageSize <= 0) pageSize = 10;
    }
}