using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;
using RecipeBook.Models;
using RecipeBook.Services;

namespace RecipeBook.ViewModels;

public partial class MyRecipeEditViewModel : BaseViewModel, IQueryAttributable
{
    private readonly DatabaseService _databaseService;

    private int _recipeId;

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private string ingredients = string.Empty;

    [ObservableProperty]
    private string instructions = string.Empty;

    [ObservableProperty]
    private string photoPath = string.Empty;

    [ObservableProperty]
    private bool isEditing;

    [ObservableProperty]
    private string validationError = string.Empty;

    [ObservableProperty]
    private bool hasPhoto;

    [ObservableProperty]
    private bool hasValidationError;

    public MyRecipeEditViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;
        Title = "Új recept";
    }

    partial void OnPhotoPathChanged(string value)
    {
        HasPhoto = !string.IsNullOrWhiteSpace(value);
    }

    partial void OnValidationErrorChanged(string value)
    {
        HasValidationError = !string.IsNullOrWhiteSpace(value);
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        var idValue = ReadQueryValue(query, "id");
        if (int.TryParse(idValue, out var id) && id > 0)
        {
            _recipeId = id;
            _ = LoadForEditAsync();
            return;
        }

        ResetForCreate();
    }

    [RelayCommand]
    public async Task SaveAsync()
    {
        ValidationError = string.Empty;

        if (string.IsNullOrWhiteSpace(Name))
        {
            ValidationError = "A név megadása kötelező.";
            return;
        }

        var model = new MyRecipe
        {
            Id = _recipeId,
            Name = Name.Trim(),
            Ingredients = Ingredients.Trim(),
            Instructions = Instructions.Trim(),
            PhotoPath = string.IsNullOrWhiteSpace(PhotoPath) ? null : PhotoPath
        };

        await _databaseService.SaveMyRecipeAsync(model);
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    public async Task TakePhotoAsync()
    {
        if (!MediaPicker.Default.IsCaptureSupported)
        {
            await Shell.Current.DisplayAlert("Kamera nem elérhető", "Az eszközön nem érhető el a kamera funkció.", "OK");
            await OfferGalleryFallbackAsync();
            return;
        }

        var cameraPermission = await Permissions.CheckStatusAsync<Permissions.Camera>();
        if (cameraPermission != PermissionStatus.Granted)
        {
            cameraPermission = await Permissions.RequestAsync<Permissions.Camera>();
        }

        if (cameraPermission != PermissionStatus.Granted)
        {
            await Shell.Current.DisplayAlert("Engedély szükséges", "A kamera használatához engedélyezd a kamera hozzáférést.", "OK");
            await OfferGalleryFallbackAsync();
            return;
        }

        try
        {
            var photo = await MediaPicker.Default.CapturePhotoAsync();
            if (photo is null)
            {
                return;
            }

            PhotoPath = await SaveFileToAppDataAsync(photo);
        }
        catch (FeatureNotSupportedException)
        {
            await Shell.Current.DisplayAlert("Kamera nem támogatott", "A készülék nem támogatja a fotózást ebben az alkalmazásban.", "OK");
            await OfferGalleryFallbackAsync();
        }
        catch (PermissionException)
        {
            await Shell.Current.DisplayAlert("Engedély hiba", "A kamera engedély hiányzik vagy tiltva van.", "OK");
            await OfferGalleryFallbackAsync();
        }
        catch
        {
            await Shell.Current.DisplayAlert("Hiba", "Nem sikerült fotót készíteni.", "OK");
            await OfferGalleryFallbackAsync();
        }
    }

    [RelayCommand]
    public async Task PickPhotoAsync()
    {
        try
        {
            var photo = await MediaPicker.Default.PickPhotoAsync();
            if (photo is null)
            {
                return;
            }

            PhotoPath = await SaveFileToAppDataAsync(photo);
        }
        catch
        {
            await Shell.Current.DisplayAlert("Hiba", "Nem sikerült képet kiválasztani.", "OK");
        }
    }

    [RelayCommand]
    public async Task CancelAsync()
    {
        await Shell.Current.GoToAsync("..");
    }

    private async Task LoadForEditAsync()
    {
        var existing = await _databaseService.GetMyRecipeByIdAsync(_recipeId);
        if (existing is null)
        {
            ResetForCreate();
            return;
        }

        IsEditing = true;
        Title = "Recept szerkesztése";
        Name = existing.Name;
        Ingredients = existing.Ingredients;
        Instructions = existing.Instructions;
        PhotoPath = existing.PhotoPath ?? string.Empty;
        ValidationError = string.Empty;
    }

    private void ResetForCreate()
    {
        _recipeId = 0;
        IsEditing = false;
        Title = "Új recept";
        Name = string.Empty;
        Ingredients = string.Empty;
        Instructions = string.Empty;
        PhotoPath = string.Empty;
        ValidationError = string.Empty;
    }

    private static async Task<string> SaveFileToAppDataAsync(FileResult file)
    {
        var extension = Path.GetExtension(file.FileName);
        var fileName = $"recipe_{DateTime.Now:yyyyMMdd_HHmmss}_{Guid.NewGuid():N}{extension}";
        var destination = Path.Combine(FileSystem.AppDataDirectory, fileName);

        await using var sourceStream = await file.OpenReadAsync();
        await using var targetStream = File.Create(destination);
        await sourceStream.CopyToAsync(targetStream);

        return destination;
    }

    private static string? ReadQueryValue(IDictionary<string, object> query, string key)
    {
        if (!query.TryGetValue(key, out var value) || value is null)
        {
            return null;
        }

        return Uri.UnescapeDataString(value.ToString() ?? string.Empty);
    }

    private async Task OfferGalleryFallbackAsync()
    {
        var openGallery = await Shell.Current.DisplayAlert(
            "Galéria",
            "Szeretnél inkább képet választani a galériából?",
            "Igen",
            "Nem");

        if (openGallery)
        {
            await PickPhotoAsync();
        }
    }
}
