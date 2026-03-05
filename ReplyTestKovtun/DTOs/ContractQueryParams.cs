namespace ReplyTestKovtun.DTOs;

public class ContractQueryParams
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string SortBy { get; set; } = "createdAt";
    public bool SortDescending { get; set; } = false;
    public string? FilterName { get; set; }
    public string? FilterEmail { get; set; }
}
