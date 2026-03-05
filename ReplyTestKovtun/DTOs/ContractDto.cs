namespace ReplyTestKovtun.DTOs;

public class ContractDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public List<DynamicFieldDto> DynamicFields { get; set; } = new();
}
