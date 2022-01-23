namespace API.Extensions;

public static class HttpExtentions
{
    public static void AddPaginationHeader(this HttpResponse response,int currentPage,int itemsPerPage, int totalItems, int totalPages){
        var pagnationHeader = new PaginationHeader(currentPage,itemsPerPage,totalItems,totalPages);
        var options = new JsonSerializerOptions{
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        response.Headers.Add("Pagination", JsonSerializer.Serialize(pagnationHeader, options));
        response.Headers.Add("Access-control-expose-headers","Pagination");
    }
}