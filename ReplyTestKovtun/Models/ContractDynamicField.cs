namespace ReplyTestKovtun.Models;

public class ContractDynamicField
{
    public Guid Id { get; set; }
    public Guid ContractId { get; set; }
    public Contract Contract { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public FieldType Type { get; set; }
    public string Value { get; set; } = string.Empty;
}
