using ReplyTestKovtun.Models;

namespace ReplyTestKovtun.Services;

public class ValidationService : IValidationService
{
    public void EnsureContractBelongsToUser(Contract contract, Guid userId)
    {
        if (contract.UserId != userId)
            throw new UnauthorizedAccessException("Contract does not belong to the current user.");
    }

    public bool ValidateDynamicFieldValue(FieldType type, string value) => type switch
    {
        // If validation rules become more complex in the future,
        // this logic can be refactored to use a strategy pattern
        // (e.g., IDynamicTypeParser per FieldType implementation)
        // to separate parsing/validation responsibility per type.
        FieldType.Integer => int.TryParse(value, out _),
        FieldType.Bool => bool.TryParse(value, out _),
        _ => true
    };
}
