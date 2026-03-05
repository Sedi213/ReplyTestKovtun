namespace ReplyTestKovtun.Models;

public class Contract
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public Guid UserId { get; set; }
    public ICollection<ContractDynamicField> DynamicFields { get; set; } = new List<ContractDynamicField>();
}
