using EduSetu.Application.Common.DTOs;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduSetu.Application.Features.Authentication.Request;

public sealed record ParseLoginTokenRequest(ParseLoginTokenDto TokenData) : IRequest<ParseLoginTokenResponse>;

public sealed class ParseLoginTokenResponse : AppResult<LoginTokenData>;

internal sealed class ParseLoginTokenRequestValidator : AbstractValidator<ParseLoginTokenRequest>
{
    public ParseLoginTokenRequestValidator()
    {
        RuleFor(x => x.TokenData.Token)
            .NotEmpty()
            .WithMessage("Token is required.");
    }
}

internal sealed class ParseLoginTokenRequestHandler(
    ) : IRequestHandler<ParseLoginTokenRequest, ParseLoginTokenResponse>
{
    public Task<ParseLoginTokenResponse> Handle(ParseLoginTokenRequest request, CancellationToken cancellationToken)
    {
        ParseLoginTokenResponse result = new ParseLoginTokenResponse();

        if (string.IsNullOrWhiteSpace(request.TokenData.Token))
        {
            result.Failure(ErrorCode.BadRequest, "Token is required");
            return Task.FromResult(result);
        }

        string decodedToken = Uri.UnescapeDataString(request.TokenData.Token);

        string base64 = decodedToken.Replace("-", "+").Replace("_", "/");
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }

        byte[] tokenBytes = Convert.FromBase64String(base64);
        string tokenData = Encoding.UTF8.GetString(tokenBytes);
        string[] parts = tokenData.Split('|');

        if (parts.Length != 3)
        {
            result.Failure(ErrorCode.BadRequest, "Invalid token format");
            return Task.FromResult(result);
        }

        string email = parts[0];
        bool rememberMe = bool.Parse(parts[1]);
        long timestamp = long.Parse(parts[2]);
        DateTimeOffset createdAt = DateTimeOffset.FromUnixTimeSeconds(timestamp);

        if (DateTimeOffset.UtcNow.Subtract(createdAt).TotalMinutes > 5)
        {
            result.Failure(ErrorCode.Unauthorized, "Login token has expired");
            return Task.FromResult(result);
        }

        LoginTokenData loginTokenData = new LoginTokenData
        {
            Email = email,
            RememberMe = rememberMe,
            CreatedAt = createdAt
        };

        result.Success(loginTokenData);
        return Task.FromResult(result);
    }
}
