# 🍽️ RecipeBook – Féléves feladat fejlesztési terv

**.NET MAUI alkalmazás | 2025/26/2 félév**

---

## Tartalomjegyzék

1. [Összefoglalás](#1-összefoglalás)
2. [Követelmények teljesítése](#2-követelmények-teljesítése)
3. [Adatmodell](#3-adatmodell)
4. [Projekt struktúra](#4-projekt-struktúra)
5. [Navigáció](#5-navigáció)
6. [ViewModelek részletezése](#6-viewmodelyek-részletezése)
7. [Képernyőtervek](#7-képernyőtervek)
8. [Hibakezelési terv](#8-hibakezelési-terv)
9. [Tesztelési terv](#9-tesztelési-terv)
10. [Fejlesztési ütemterv](#10-fejlesztési-ütemterv)
11. [NuGet csomagok](#11-nuget-csomagok)
12. [TheMealDB API](#12-themealdb-api)

---

## 1. Összefoglalás

A **RecipeBook** egy .NET MAUI alapú mobil receptkönyv alkalmazás. A felhasználó recepteket kereshet a TheMealDB online API-n keresztül, kedvenceket menthet el helyi adatbázisba, saját recepteket hozhat létre (fotóval együtt), és recepteket oszthat meg más alkalmazásokba.

| Tulajdonság | Érték |
|---|---|
| Technológia | .NET MAUI (C#) |
| Adatbázis | SQLite – sqlite-net-pcl |
| API | TheMealDB (ingyenes, regisztráció nélkül) |
| MVVM | CommunityToolkit.Mvvm |
| Extra funkciók | Kamera + Megosztás + Hálózat detektálás |

---

## 2. Követelmények teljesítése

### 2.1 MVVM szerkezet ✅

Az alkalmazás a `CommunityToolkit.Mvvm` csomagot használja. Minden Page-hez dedikált ViewModel tartozik, amely `[ObservableProperty]` és `[RelayCommand]` attribútumokat alkalmaz. A View és a ViewModel között kizárólag data binding és command binding kommunikál, a code-behind fájlokban üzleti logika nem szerepel.

| Page | ViewModel |
|---|---|
| SearchPage | SearchViewModel |
| RecipeDetailPage | RecipeDetailViewModel |
| FavoritesPage | FavoritesViewModel |
| MyRecipesPage | MyRecipesViewModel |
| MyRecipeEditPage | MyRecipeEditViewModel |

### 2.2 Oldalak (minimum 3 interaktív Page) ✅

Az alkalmazás **5 interaktív Page**-ből áll – mindegyikkel érdemben interaktálhat a felhasználó:

| # | Page | Leírás |
|---|---|---|
| 1 | **SearchPage** | Receptek keresése API-ból, találati lista, hálózat figyelmeztetés |
| 2 | **RecipeDetailPage** | Recept teljes részletei, Mentés és Megosztás gomb |
| 3 | **FavoritesPage** | Mentett receptek listája, törlés, megosztás |
| 4 | **MyRecipesPage** | Saját receptek listája, szerkesztés és törlés |
| 5 | **MyRecipeEditPage** | Saját recept létrehozása / szerkesztése, kamera |

### 2.3 CRUD műveletek ✅

A `MyRecipe` entitáson teljes CRUD implementálva, minden művelet felhasználói inputhoz kötött:

| Művelet | Hol | Felhasználói interakció |
|---|---|---|
| **Create** | MyRecipeEditPage – Mentés gomb | Kitölti a szövegmezőket, opcionálisan fotót készít, majd ment |
| **Read** | MyRecipesPage – scrollozható lista | Listában látja az összes saját receptet, rákoppint a részletekre |
| **Update** | MyRecipeEditPage – meglévő recept megnyitása | Módosítja a kívánt mezőket, újra ment |
| **Delete** | MyRecipesPage – törlés gomb / swipe | Swipe-to-delete vagy gomb, megerősítő dialógus jelenik meg |

### 2.4 Extra funkciók ✅

#### 📷 Kamera (1.)
Saját recept hozzáadásakor a felhasználó fotót készíthet az ételről a `MyRecipeEditPage`-en a `MediaPicker.CapturePhotoAsync()` metódussal. A kép helyi elérési útja a SQLite adatbázisban tárolódik, a listában és a részletes nézetben megjelenik.

#### 📤 Szöveg megosztása (2.)
A `RecipeDetailPage`-en és a `FavoritesPage`-en a felhasználó megoszthatja a receptet a `Share.RequestAsync()` segítségével. A megosztott szöveg tartalmazza a recept nevét és hozzávalóit, amelyet SMS-ben, emailben vagy bármely más telepített alkalmazáson keresztül el lehet küldeni.

#### 🌐 Hálózat detektálás és kezelés (3.)
A `SearchPage` indításakor és minden kereséskor az alkalmazás ellenőrzi a hálózati kapcsolatot a `Connectivity.NetworkAccess` property-vel. Kapcsolat hiányában:
- A keresőmező letiltódik
- Figyelmeztető banner jelenik meg az oldal tetején
- A felhasználó a **Kedvencek** tab-ra irányítható, ahol az offline elérhető mentett receptek böngészhetők
- Hálózat visszatérésekor (`Connectivity.ConnectivityChanged` esemény) az oldal automatikusan feloldódik

---

## 3. Adatmodell

### 3.1 MyRecipe (SQLite – lokális)

```csharp
[Table("MyRecipes")]
public class MyRecipe
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [NotNull]
    public string Name { get; set; } = string.Empty;

    public string Ingredients { get; set; } = string.Empty;  // sortörésekkel elválasztva

    public string Instructions { get; set; } = string.Empty;

    public string? PhotoPath { get; set; }  // lokális fájlútvonal

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? UpdatedAt { get; set; }
}
```

### 3.2 FavoriteRecipe (SQLite – lokális)

```csharp
[Table("FavoriteRecipes")]
public class FavoriteRecipe
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [NotNull]
    public string MealId { get; set; } = string.Empty;  // TheMealDB azonosító

    public string Name { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string ThumbnailUrl { get; set; } = string.Empty;

    public string Ingredients { get; set; } = string.Empty;

    public string Instructions { get; set; } = string.Empty;

    public DateTime SavedAt { get; set; } = DateTime.Now;
}
```

### 3.3 API válasz modellek (nem perzisztált)

```csharp
// TheMealDB API válasz
public class MealResponse
{
    public List<Meal>? Meals { get; set; }
}

public class Meal
{
    public string IdMeal { get; set; } = string.Empty;
    public string StrMeal { get; set; } = string.Empty;
    public string StrCategory { get; set; } = string.Empty;
    public string StrInstructions { get; set; } = string.Empty;
    public string StrMealThumb { get; set; } = string.Empty;
    public string? StrIngredient1 { get; set; }
    public string? StrIngredient2 { get; set; }
    // ... StrIngredient3–StrIngredient20
}
```

---

## 4. Projekt struktúra

```
RecipeBook/
├── Models/
│   ├── MyRecipe.cs
│   ├── FavoriteRecipe.cs
│   └── Meal.cs                    # API válasz modell
│
├── ViewModels/
│   ├── BaseViewModel.cs           # IsBusy, Title – közös alap
│   ├── SearchViewModel.cs
│   ├── RecipeDetailViewModel.cs
│   ├── FavoritesViewModel.cs
│   ├── MyRecipesViewModel.cs
│   └── MyRecipeEditViewModel.cs
│
├── Views/
│   ├── SearchPage.xaml / .cs
│   ├── RecipeDetailPage.xaml / .cs
│   ├── FavoritesPage.xaml / .cs
│   ├── MyRecipesPage.xaml / .cs
│   └── MyRecipeEditPage.xaml / .cs
│
├── Services/
│   ├── DatabaseService.cs         # SQLite CRUD műveletek
│   ├── MealApiService.cs          # TheMealDB API hívások
│   └── ConnectivityService.cs     # Hálózat figyelés
│
├── Converters/
│   ├── BoolToColorConverter.cs
│   └── NullToVisibilityConverter.cs
│
├── Resources/
│   ├── Styles/
│   │   └── Styles.xaml
│   └── Images/
│
├── AppShell.xaml / .cs
└── MauiProgram.cs                 # DI konfiguráció
```

---

## 5. Navigáció

Az alkalmazás **Shell alapú navigációt** használ `TabBar`-ral az alsó navigációs sávban.

```
AppShell
├── TabBar
│   ├── 🔍 Keresés Tab
│   │   └── SearchPage
│   │       └── → RecipeDetailPage  (push navigáció)
│   │
│   ├── ❤️ Kedvencek Tab
│   │   └── FavoritesPage
│   │       └── → RecipeDetailPage  (push navigáció)
│   │
│   └── 📝 Saját receptek Tab
│       └── MyRecipesPage
│           └── → MyRecipeEditPage  (push navigáció, új és meglévő esetén is)
```

**Navigációs diagram:**

```
[SearchPage] ──────────────────────────────────┐
     │  keresési találatra koppint               │
     ▼                                           │
[RecipeDetailPage] ◄────────────────────────────┤
     ▲  kedvencre koppint                        │
     │                                           │
[FavoritesPage] ────────────────────────────────┘

[MyRecipesPage]
     │  "+" gomb  →  [MyRecipeEditPage] (új)
     │  szerkesztés → [MyRecipeEditPage] (meglévő)
```

---

## 6. ViewModelek részletezése

### SearchViewModel

```csharp
// Properties
[ObservableProperty] string searchQuery = string.Empty;
[ObservableProperty] ObservableCollection<Meal> searchResults = new();
[ObservableProperty] bool isLoading = false;
[ObservableProperty] bool isOffline = false;
[ObservableProperty] bool hasNoResults = false;

// Commands
[RelayCommand] async Task SearchAsync();
[RelayCommand] async Task NavigateToDetailAsync(Meal meal);
[RelayCommand] void CheckConnectivity();
```

### RecipeDetailViewModel

```csharp
// Properties
[ObservableProperty] Meal? meal;
[ObservableProperty] FavoriteRecipe? favoriteRecipe;  // ha API-ból jön
[ObservableProperty] MyRecipe? myRecipe;               // ha saját recept
[ObservableProperty] bool isSaved = false;
[ObservableProperty] List<string> ingredientList = new();

// Commands
[RelayCommand] async Task ToggleFavoriteAsync();
[RelayCommand] async Task ShareRecipeAsync();
[RelayCommand] void GoBack();
```

### FavoritesViewModel

```csharp
// Properties
[ObservableProperty] ObservableCollection<FavoriteRecipe> favorites = new();
[ObservableProperty] bool isEmpty = false;

// Commands
[RelayCommand] async Task LoadFavoritesAsync();
[RelayCommand] async Task DeleteFavoriteAsync(FavoriteRecipe recipe);
[RelayCommand] async Task ShareRecipeAsync(FavoriteRecipe recipe);
[RelayCommand] async Task NavigateToDetailAsync(FavoriteRecipe recipe);
```

### MyRecipesViewModel

```csharp
// Properties
[ObservableProperty] ObservableCollection<MyRecipe> myRecipes = new();
[ObservableProperty] bool isEmpty = false;

// Commands
[RelayCommand] async Task LoadRecipesAsync();
[RelayCommand] async Task DeleteRecipeAsync(MyRecipe recipe);
[RelayCommand] async Task NavigateToEditAsync(MyRecipe? recipe);  // null = új recept
```

### MyRecipeEditViewModel

```csharp
// Properties
[ObservableProperty] string name = string.Empty;
[ObservableProperty] string ingredients = string.Empty;
[ObservableProperty] string instructions = string.Empty;
[ObservableProperty] string? photoPath;
[ObservableProperty] bool isEditing = false;  // új vs. meglévő

// Commands
[RelayCommand] async Task SaveAsync();
[RelayCommand] async Task TakePhotoAsync();
[RelayCommand] async Task PickPhotoAsync();
[RelayCommand] void Cancel();
```

---

## 7. Képernyőtervek

### SearchPage

```
┌─────────────────────────────────┐
│  🔍 Recept keresés              │
├─────────────────────────────────┤
│ ⚠️ Nincs internetkapcsolat       │  ← csak offline esetén
├─────────────────────────────────┤
│ [ Keresés...              🔍 ]  │
├─────────────────────────────────┤
│ ┌─────────────────────────────┐ │
│ │ 🖼️  Chicken Tikka Masala    │ │
│ │     Chicken | 🇮🇳           │ │
│ └─────────────────────────────┘ │
│ ┌─────────────────────────────┐ │
│ │ 🖼️  Pasta Carbonara         │ │
│ │     Pasta | 🇮🇹             │ │
│ └─────────────────────────────┘ │
│              ...                │
├─────────────────────────────────┤
│  🔍 Keresés  ❤️ Kedvencek  📝 Saját │
└─────────────────────────────────┘
```

### RecipeDetailPage

```
┌─────────────────────────────────┐
│ ←  Chicken Tikka Masala    ❤️ 📤 │
├─────────────────────────────────┤
│                                 │
│         [ Borítókép ]           │
│                                 │
├─────────────────────────────────┤
│ Kategória: Chicken              │
├─────────────────────────────────┤
│ Hozzávalók                      │
│  • Chicken – 500g               │
│  • Tomato – 2 db                │
│  • ...                          │
├─────────────────────────────────┤
│ Elkészítés                      │
│  1. Marinálj...                 │
│  2. Süsd meg...                 │
│  ...                            │
└─────────────────────────────────┘
```

### FavoritesPage

```
┌─────────────────────────────────┐
│  ❤️ Kedvenc receptek            │
├─────────────────────────────────┤
│ ┌─────────────────────────────┐ │
│ │ 🖼️  Chicken Tikka Masala    │ │
│ │     Chicken  📤  🗑️         │ │
│ └─────────────────────────────┘ │
│ ┌─────────────────────────────┐ │
│ │ 🖼️  Pasta Carbonara         │ │
│ │     Pasta    📤  🗑️         │ │
│ └─────────────────────────────┘ │
│              ...                │
│                                 │
│  (üres állapot:)                │
│  ❤️ Még nincsenek              │
│     mentett receptjeid          │
│                                 │
├─────────────────────────────────┤
│  🔍 Keresés  ❤️ Kedvencek  📝 Saját │
└─────────────────────────────────┘
```

### MyRecipesPage

```
┌─────────────────────────────────┐
│  📝 Saját receptek          ➕  │
├─────────────────────────────────┤
│ ┌─────────────────────────────┐ │
│ │ 🖼️  Anyukám palacsintája    │ │
│ │     2025.04.10.   ✏️  🗑️   │ │
│ └─────────────────────────────┘ │
│ ┌─────────────────────────────┐ │
│ │ 🖼️  Gulyásleves             │ │
│ │     2025.04.15.   ✏️  🗑️   │ │
│ └─────────────────────────────┘ │
│              ...                │
│                                 │
│  (üres állapot:)                │
│  📝 Még nincs saját recepted   │
│  [ + Első recept hozzáadása ]   │
│                                 │
├─────────────────────────────────┤
│  🔍 Keresés  ❤️ Kedvencek  📝 Saját │
└─────────────────────────────────┘
```

### MyRecipeEditPage

```
┌─────────────────────────────────┐
│ ←  Új recept                    │
├─────────────────────────────────┤
│                                 │
│  [ 📷 Fotó készítése ]          │
│  [ 🖼️  Galériából választás ]   │
│       (vagy meglévő fotó)       │
│                                 │
│  Név *                          │
│  ┌───────────────────────────┐  │
│  │ Recept neve...            │  │
│  └───────────────────────────┘  │
│                                 │
│  Hozzávalók *                   │
│  ┌───────────────────────────┐  │
│  │ Pl. 2 tojás               │  │
│  │ 100g liszt                │  │
│  └───────────────────────────┘  │
│                                 │
│  Elkészítés                     │
│  ┌───────────────────────────┐  │
│  │ Lépések...                │  │
│  └───────────────────────────┘  │
│                                 │
│  [ Mégse ]        [ 💾 Mentés ] │
└─────────────────────────────────┘
```

---

## 8. Hibakezelési terv

| Helyzet | Hol fordul elő | Kezelés |
|---|---|---|
| Nincs internetkapcsolat | SearchPage | Banner figyelmeztetés, keresőmező letiltva, Kedvencek tab ajánlása |
| API hiba / timeout | MealApiService | Try-catch, felhasználóbarát hibaüzenet (Toast / Alert), üres lista megjelenítése |
| Üres keresési találat | SearchPage | "Nem található ilyen recept" felirat a lista helyén |
| Kamera engedély megtagadva | MyRecipeEditPage | Alert üzenet: "Kamera engedély szükséges", recept mentése fotó nélkül is lehetséges |
| SQLite írási/olvasási hiba | DatabaseService | Try-catch minden DB műveletnél, hibanapló konzolra, felhasználónak Alert |
| Kötelező mező üres mentéskor | MyRecipeEditPage | Validáció mentés előtt, hibaüzenet a mező mellett, mentés megakadályozva |
| Törlés megerősítése | MyRecipesPage, FavoritesPage | `DisplayAlert` megerősítő dialógus megjelenítése törlés előtt |
| Nincs mentett recept | FavoritesPage / MyRecipesPage | "Még nincsenek mentett receptek" placeholder szöveg és ikon |

---

## 9. Tesztelési terv

### 9.1 Funkcionális tesztek

| Teszteset | Elvárt eredmény |
|---|---|
| Keresés érvényes szóra (pl. "chicken") | Találati lista megjelenik képekkel |
| Keresés nem létező szóra | "Nem található" üzenet jelenik meg |
| Recept mentése kedvencekbe | Megjelenik a FavoritesPage-en |
| Ugyanaz a recept kétszer mentve | Csak egyszer szerepel (duplikáció elkerülése) |
| Saját recept létrehozása minden mezővel | Megjelenik a MyRecipesPage-en |
| Saját recept létrehozása csak névvel | Mentés sikeres, opcionális mezők üresek |
| Saját recept mentése üres névvel | Validációs hiba, mentés nem történik |
| Saját recept szerkesztése | Módosítások elmentésre kerülnek |
| Saját recept törlése (megerősítve) | Eltűnik a listából |
| Saját recept törlése (megszakítva) | Nem törlődik |
| Recept megosztása | Megnyílik a rendszer megosztási panel |
| Fotó készítése recepthez | Fotó megjelenik az editáló felületen |
| Alkalmazás offline indítása | Figyelmeztető banner, keresés letiltva |
| Hálózat visszatérése | Keresés automatikusan újra engedélyeződik |

### 9.2 Navigációs tesztek

| Teszteset | Elvárt eredmény |
|---|---|
| Találati elemre koppintás | RecipeDetailPage megnyílik |
| Vissza gomb RecipeDetailPage-ről | Visszanavigál az előző oldalra |
| Tab váltás | Megfelelő tab tartalma jelenik meg |
| "+" gomb MyRecipesPage-en | Üres MyRecipeEditPage nyílik |
| Meglévő receptre koppintás szerkesztéshez | Kitöltött MyRecipeEditPage nyílik |

### 9.3 Szélső esetek

| Teszteset | Elvárt eredmény |
|---|---|
| Nagyon hosszú recept neve | Szöveg csonkítva jelenik meg (ellipsis) |
| Sok találat (20+ elem) | Lista scrollozható, nem fagy |
| Alkalmazás háttérbe küldése és visszatérés | Állapot megmarad |
| Kamera engedély megtagadva | Hibaüzenet, az app nem crashel |

---

## 10. Fejlesztési ütemterv

| Hét | Feladat | Eredmény |
|---|---|---|
| **1. hét** | Projekt létrehozása, NuGet csomagok, Shell + TabBar navigáció | Navigálható app alap |
| **2. hét** | Adatmodellek, DatabaseService, MyRecipes CRUD teljes implementálása | Saját receptek kezelése kész |
| **3. hét** | MealApiService (TheMealDB), SearchPage + SearchViewModel | Online keresés működik |
| **4. hét** | RecipeDetailPage, FavoritesPage, mentés/törlés SQLite-ba | Kedvencek funkció kész |
| **5. hét** | Kamera integráció (fotó készítés + megjelenítés), Megosztás funkció | Extra funkciók kész |
| **6. hét** | Hálózat detektálás, offline mód, hibakezelés, validáció | Robusztus működés |
| **7. hét** | UI finomítás, tesztelés, bugok javítása, Moodle beadás | Végleges verzió |

---

## 11. NuGet csomagok

| Csomag | Verzió | Felhasználás |
|---|---|---|
| `CommunityToolkit.Mvvm` | 8.x | MVVM alap, `[ObservableProperty]`, `[RelayCommand]` |
| `CommunityToolkit.Maui` | 9.x | Toast üzenetek, egyéb UI segédletek |
| `sqlite-net-pcl` | 1.9.x | SQLite adatbázis kezelése |
| `SQLitePCLRaw.bundle_green` | 2.x | SQLite platform függőség |

---

## 12. TheMealDB API

Az alkalmazás a [TheMealDB](https://www.themealdb.com/api.php) ingyenes API-ját használja, amely nem igényel regisztrációt vagy API kulcsot.

| Endpoint | Felhasználás |
|---|---|
| `GET /search.php?s={név}` | Recept keresése név alapján |
| `GET /lookup.php?i={id}` | Recept teljes adatai ID alapján |
| `GET /random.php` | Véletlenszerű recept lekérése |

**Alap URL:** `https://www.themealdb.com/api/json/v1/1/`

### Példa API hívás (MealApiService.cs)

```csharp
public async Task<List<Meal>> SearchMealsAsync(string query)
{
    try
    {
        var response = await _httpClient.GetFromJsonAsync<MealResponse>(
            $"search.php?s={Uri.EscapeDataString(query)}");
        return response?.Meals ?? new List<Meal>();
    }
    catch (HttpRequestException)
    {
        return new List<Meal>();
    }
}
```

---

*Készítette: [Hallgató neve] | 2025/26/2 félév*
