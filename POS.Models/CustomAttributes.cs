using System.ComponentModel.DataAnnotations;

public class RoleValidationAttribute : ValidationAttribute
{
    private static readonly HashSet<string> AllowedRoles = new HashSet<string>
    {
        "Cashier",
        "Admin"
    };

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var role = value as string;
        if (role == null || !AllowedRoles.Contains(role))
        {
            return new ValidationResult("Role must be either 'Cashier' or 'Admin'");
        }

        return ValidationResult.Success;
    }
}
