using ReplyTestKovtun.Models;

namespace ReplyTestKovtun.DTOs;

public class AssignCustomFieldValueDto
{
    public string Name { get; set; } = string.Empty;
    public FieldType Type { get; set; }
    public string Value { get; set; } = string.Empty;
}
