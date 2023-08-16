using System.ComponentModel.DataAnnotations;
using CommandDotNet;
using Empowered.Dataverse.Connection.Commands.Constants;

namespace Empowered.Dataverse.Connection.Commands.Arguments;

public class UpsertConnectionArguments : IArgumentModel, IValidatableObject
{
    public required ConnectionNameArguments ConnectionNameArguments { get; init; }
    public required ConnectionArguments ConnectionArguments { get; init; }

    [Option(Description = "Test the Dataverse connection after upsertion")]
    public bool TestConnection { get; init; } = true;
    
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var validationResults = new List<ValidationResult>();

        if (ConnectionArguments.DeviceCode && ConnectionArguments.Interactive)
        {
            var validationResult = new ValidationResult(ValidationErrors.DeviceCodeAndInteractive);
            validationResults.Add(validationResult);
        }

        // TODO: validate invalid connections
        // if (ConnectionType == ConnectionType.Unknown)
        // {
        //     validationResults.Add(new ValidationResult(ValidationErrors.UnknownConnectionType));
        // }

        return validationResults;
    }
}