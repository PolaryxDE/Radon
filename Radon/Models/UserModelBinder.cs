using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Radon.Models;

internal class UserModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (!typeof(UserBase).IsAssignableFrom(bindingContext.ModelType))
        {
            return Task.CompletedTask;
        }

        if (!bindingContext.HttpContext.Items.ContainsKey("User"))
        {
            return Task.CompletedTask;
        }

        if (bindingContext.HttpContext.Items["User"] is not UserBase user)
        {
            return Task.CompletedTask;
        }

        bindingContext.Result = ModelBindingResult.Success(user);
        return Task.CompletedTask;
    }
}