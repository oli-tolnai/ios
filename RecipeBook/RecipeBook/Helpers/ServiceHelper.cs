using Microsoft.Maui;
using Microsoft.Extensions.DependencyInjection;

namespace RecipeBook.Helpers;

public static class ServiceHelper
{
    public static T GetService<T>() where T : class
    {
        return IPlatformApplication.Current?.Services.GetService<T>()
            ?? throw new InvalidOperationException($"Service not found: {typeof(T).Name}");
    }
}
