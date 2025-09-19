using EduSetu.Application.Common.DTOs;
using Microsoft.AspNetCore.Components;
using System.Collections.ObjectModel;

namespace EduSetu.Components;

public class BaseComponent : ComponentBase
{
    private readonly int _delayInMilliseconds = 100;

    #region # Loader

    protected bool isLoading = false;

    protected async Task ShowLoaderAsync()
    {
        if (isLoading)
        {
            return;
        }

        isLoading = true;

        await InvokeAsync(StateHasChanged);
        await Task.Delay(_delayInMilliseconds);
    }

    protected async Task HideLoaderAsync()
    {
        if (!isLoading)
        {
            return;
        }

        isLoading = false;

        await InvokeAsync(StateHasChanged);
        await Task.Delay(_delayInMilliseconds);
    }


    #endregion

    #region # Error

    protected Collection<AppError> errors = new();
    protected bool showErrorModal = false;

    protected void ShowErrorModal(Collection<AppError> appErrors)
    {
        ArgumentNullException.ThrowIfNull(appErrors);

        errors = appErrors;
        showErrorModal = true;
        StateHasChanged();
    }

    protected void HideErrorModal()
    {
        showErrorModal = false;
        errors.Clear();
        StateHasChanged();
    }

    #endregion
}

