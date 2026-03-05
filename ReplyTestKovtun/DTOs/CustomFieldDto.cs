using ReplyTestKovtun.Models;

namespace ReplyTestKovtun.DTOs;

public class CustomFieldDto
{
    public string Name { get; set; } = string.Empty;
    public FieldType Type { get; set; }
}
