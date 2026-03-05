namespace ReplyTestKovtun.DTOs;

public class BulkMergeItemDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public List<AssignCustomFieldValueDto>? DynamicFields { get; set; }
}
