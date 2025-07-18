using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduSetu.Application.Common.DTOs;

public enum ErrorCode
{
    None = 0,
    BadRequest = 400,
    Unauthorized = 401,
    Forbidden = 403,
    NotFound = 404,
    Conflict = 409,
    InternalServerError = 500,
    ServiceUnavailable = 503
}

public class AppError
{
    public AppError(ErrorCode code, string message)
    {
        Code = code;
        Message = message;
    }

    public ErrorCode Code { get; set; }
    public string Message { get; set; }
}

public class AppResult
{
    #region # Init

    public AppResult()
    {
        HasError = false;
        Errors = [];
    }

    #endregion

    public bool HasError { get; protected set; }

    public Collection<AppError> Errors { get; } = new Collection<AppError>();

    public virtual bool Validate<T>(T model) where T : class
    {
        if (model is null)
        {
            Failure(ErrorCode.BadRequest, $"{nameof(model)} can't be empty.");
        }
        else
        {
            List<ValidationResult> validationResults = new List<ValidationResult>();
            ValidationContext validationContext = new ValidationContext(model, null, null);

            bool isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);
            if (isValid)
            {
                Success();
            }
            else
            {
                foreach (ValidationResult validationResult in validationResults)
                {
                    Failure(ErrorCode.BadRequest, validationResult.ErrorMessage ?? "Unknown Error!!");
                }
            }
        }
        return HasError;
    }

    public virtual void Success()
    {
        HasError = false;
        Errors.Clear();
    }

    public virtual void Failure(ErrorCode code, string message)
    {
        HasError = true;
        Errors.Add(new AppError(code, message));
    }
}

public class AppResult<T> : AppResult
{
    #region # Init

    public AppResult() : base() { }

    #endregion

    public T? Payload { get; protected set; }

    public void Success(T? payload)
    {
        base.Success();
        Payload = payload;
    }
}