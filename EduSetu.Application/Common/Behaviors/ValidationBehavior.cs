using FluentValidation;
using MediatR;

namespace EduSetu.Application.Common.Behaviors;

/// <summary>
/// Pipeline behavior that automatically validates requests using FluentValidation
/// </summary>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            Console.WriteLine($"DEBUG: ValidationBehavior - Request type: {typeof(TRequest).Name}");
            
            if (_validators.Any())
            {
                Console.WriteLine($"DEBUG: Found {_validators.Count()} validators");
                ValidationContext<TRequest> context = new ValidationContext<TRequest>(request);

                FluentValidation.Results.ValidationResult[] validationResults = await Task.WhenAll(
                    _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

                List<FluentValidation.Results.ValidationFailure> failures = validationResults
                    .Where(r => r.Errors.Count != 0)
                    .SelectMany(r => r.Errors)
                    .ToList();

                if (failures.Count != 0)
                {
                    Console.WriteLine($"DEBUG: Validation failed with {failures.Count} errors");
                    foreach (var failure in failures)
                    {
                        Console.WriteLine($"DEBUG: Validation error - Property: {failure.PropertyName}, Error: {failure.ErrorMessage}");
                    }
                    throw new ValidationException(failures);
                }
                
                Console.WriteLine("DEBUG: Validation passed");
            }
            else
            {
                Console.WriteLine("DEBUG: No validators found");
            }

            Console.WriteLine("DEBUG: Calling next handler");
            return await next();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"DEBUG: Exception in ValidationBehavior: {ex.Message}");
            Console.WriteLine($"DEBUG: Stack trace: {ex.StackTrace}");
            throw;
        }
    }
}
