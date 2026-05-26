# RecipeBook projektvédési leírás

Ez a leírás arra készült, hogy a projekt bemutatásakor gyorsan el tudd magyarázni, mit tud az alkalmazás, hogyan épül fel, és a fontosabb funkciók hol vannak megvalósítva a kódban.

## Rövid bemutatás

A RecipeBook egy egyszerű .NET MAUI receptes alkalmazás. A cél nem egy túlkomplikált, látványos app volt, hanem egy működő MVP, amelyben a fő funkciók használhatók Androidon és Windowson is.

Az alkalmazás fő funkciói:

- receptek keresése online a TheMealDB API alapján
- recept részleteinek megnyitása
- online receptek kedvencek közé mentése
- kedvencek listázása, megnyitása, törlése és megosztása
- saját receptek létrehozása, szerkesztése, törlése
- saját recepthez fotó készítése vagy kép választása galériából
- adatok helyi mentése SQLite adatbázisba
- recept megosztása szövegfájllal és ha van kép, akkor képpel együtt

## Projekt felépítése

A projekt egyszerű MVVM szerkezetet használ.

Fontosabb mappák:

- `Models`: adatmodellek, például API recept, kedvenc recept, saját recept
- `Services`: API hívás, SQLite adatbázis, megosztás
- `ViewModels`: az oldalak logikája és parancsai
- `Views`: XAML oldalak, vagyis a felhasználói felület
- `Helpers`: közös konstansok, például navigációs route nevek

Fontos fájlok:

- `RecipeBook/RecipeBook/MauiProgram.cs`: dependency injection regisztrációk
- `RecipeBook/RecipeBook/AppShell.xaml`: tabos fő navigáció
- `RecipeBook/RecipeBook/AppShell.xaml.cs`: részletoldalak route regisztrációja
- `RecipeBook/RecipeBook/App.xaml.cs`: light mode erőltetése
- `RecipeBook/RecipeBook/Resources/Styles/Styles.xaml`: alap stílusok, gombok, TabBar színek

## Architektúra

Az alkalmazás MVVM mintát követ.

A View feladata:

- megjeleníti az adatokat
- XAML bindingokon keresztül kapcsolódik a ViewModelhez
- gombnyomáskor commandokat hív

A ViewModel feladata:

- kezeli az oldal állapotát
- betölti az adatokat
- commandokat tartalmaz, például keresés, törlés, mentés, navigáció

A Service réteg feladata:

- API kommunikáció
- SQLite adatbázis kezelés
- megosztási logika

Ez azért jó, mert a UI nincs összekeverve az adatbázis vagy API logikával. Ha a tanár megkérdezi, hogy hol van az üzleti logika, akkor a válasz: főleg a `ViewModels` és `Services` mappákban.

## Dependency Injection

A service-ek és ViewModel-ek a `MauiProgram.cs` fájlban vannak regisztrálva.

Itt történik például:

- `HttpClient` regisztrálása
- `DatabaseService` regisztrálása
- `MealApiService` regisztrálása
- `RecipeShareService` regisztrálása
- az oldalakhoz tartozó ViewModel-ek regisztrálása

Ez azért hasznos, mert az oldalak nem maguk hozzák létre kézzel a service-eket, hanem a MAUI DI konténere adja át őket. Így tisztább és jobban karbantartható a kód.

## Navigáció

A fő navigáció Shell alapú.

Fő tabok:

- `Keresés`
- `Kedvencek`
- `Saját`

Ez az `AppShell.xaml` fájlban van megadva.

A külön megnyíló oldalak route alapján működnek:

- `recipe-detail`: recept részletoldal
- `my-recipe-edit`: saját recept létrehozás/szerkesztés

A route nevek az `Helpers/AppRoutes.cs` fájlban vannak konstansként tárolva. A regisztráció az `AppShell.xaml.cs` fájlban történik.

Példa magyarázat:

Ha a keresési listából megnyitok egy receptet, akkor a `SearchViewModel` összerak egy route-ot a recept azonosítójával, majd meghívja a `Shell.Current.GoToAsync(...)` metódust.

## Keresés

A keresés a `SearchPage` oldalon történik, a logikája pedig a `SearchViewModel.cs` fájlban van.

Fontosabb részek:

- `SearchQuery`: a keresőmező tartalma
- `SearchResults`: a találati lista
- `SearchAsync`: keresés indítása
- `OpenDetailAsync`: kiválasztott recept részletoldalának megnyitása
- `CheckConnectivity`: internetkapcsolat ellenőrzése

Az API hívást nem közvetlenül a ViewModel végzi, hanem a `MealApiService`.

A keresésnél az alkalmazás ellenőrzi, hogy van-e internet. Ha nincs internet, akkor figyelmeztető üzenetet jelenít meg, mert a TheMealDB API csak online érhető el.

A találatokból név alapján történik egy egyszerű szűrés is, hogy a keresés jobban azt adja vissza, amit a felhasználó vár.

## TheMealDB API

Az API hívások a `Services/MealApiService.cs` fájlban vannak.

Használt végpontok:

- `search.php?s=...`: receptek keresése név alapján
- `lookup.php?i=...`: konkrét recept lekérése azonosító alapján

Fontos metódusok:

- `SearchMealsAsync(string query)`
- `GetMealByIdAsync(string mealId)`

Az API válasz modelljei a `Models/Meal.cs` fájlban vannak.

A `Meal` modellben szerepelnek:

- recept azonosító
- recept neve
- kategória
- elkészítési leírás
- kép URL
- `StrIngredient1` - `StrIngredient20`
- `StrMeasure1` - `StrMeasure20`

A hozzávalók szöveggé alakítását a `BuildIngredientsText()` metódus végzi.

## Recept részletoldal

A recept részleteinek megjelenítése a `RecipeDetailPage` oldalon történik.

A logika a `RecipeDetailViewModel.cs` fájlban van.

Fontosabb funkciók:

- API recept betöltése azonosító alapján
- kedvenc recept betöltése helyi adatbázisból
- saját recept betöltése helyi adatbázisból
- kedvencekhez adás vagy eltávolítás
- recept megosztása

A részletoldal több forrásból is tud adatot betölteni:

- `source=api`: online API-ból jövő recept
- `source=favorite`: kedvencekből nyitott recept
- `source=my`: saját receptként létrehozott recept

Ez azért van, mert ugyanaz a részletoldal újrahasznosítható több helyről.

## Kedvencek

A kedvencek oldala a `FavoritesPage`.

A logika a `FavoritesViewModel.cs` fájlban van.

Fontosabb metódusok:

- `LoadFavoritesAsync`: kedvencek betöltése SQLite-ból
- `DeleteFavoriteAsync`: kedvenc törlése
- `ShareRecipeAsync`: kedvenc recept megosztása
- `NavigateToDetailAsync`: kedvenc megnyitása részletoldalon

A kedvenc receptek a `FavoriteRecipe` modell alapján kerülnek mentésre.

A duplikált kedvencek elkerülése a `DatabaseService.AddFavoriteIfNotExistsAsync(...)` metódusban van megoldva. Ez először ellenőrzi, hogy a `MealId` alapján létezik-e már a recept a kedvencek között. Ha igen, akkor nem menti el újra.

## Saját receptek

A saját receptek listája a `MyRecipesPage` oldalon van.

A logika a `MyRecipesViewModel.cs` fájlban található.

Fontosabb metódusok:

- `LoadRecipesAsync`: saját receptek betöltése
- `NavigateToEditAsync`: új recept létrehozása vagy meglévő szerkesztése
- `NavigateToDetailAsync`: saját recept részleteinek megnyitása
- `DeleteRecipeAsync`: saját recept törlése

A saját receptek a `MyRecipe` modell alapján kerülnek mentésre.

## Saját recept létrehozása és szerkesztése

A létrehozó/szerkesztő oldal a `MyRecipeEditPage`.

A logika a `MyRecipeEditViewModel.cs` fájlban van.

Fontosabb metódusok:

- `SaveAsync`: recept mentése
- `LoadForEditAsync`: meglévő recept betöltése szerkesztéshez
- `ResetForCreate`: új recept oldal alaphelyzetbe állítása
- `TakePhotoAsync`: fotó készítése kamerával
- `PickPhotoAsync`: kép kiválasztása galériából

Mentés előtt van egyszerű validáció: a recept neve kötelező. Ha nincs megadva név, akkor a ViewModel hibaüzenetet állít be.

Fotózásnál az app először kamerát próbál használni. Ha nincs kamera, nincs engedély, vagy hiba történik, akkor felajánlja a galériából választást.

A kiválasztott vagy készített képet az app a saját alkalmazás adatmappájába másolja. Ez azért jobb, mint az eredeti külső fájlra hivatkozni, mert így később is megmarad az alkalmazás számára.

## SQLite adatbázis

A helyi adatmentés a `Services/DatabaseService.cs` fájlban van.

Az app SQLite adatbázist használ:

- adatbázis fájl neve: `recipebook.db3`
- helye: `FileSystem.AppDataDirectory`

Táblák:

- `MyRecipe`: saját receptek
- `FavoriteRecipe`: kedvencek

Fontosabb metódusok:

- `GetMyRecipesAsync`
- `GetMyRecipeByIdAsync`
- `SaveMyRecipeAsync`
- `DeleteMyRecipeAsync`
- `GetFavoritesAsync`
- `GetFavoriteByMealIdAsync`
- `AddFavoriteIfNotExistsAsync`
- `DeleteFavoriteAsync`
- `IsFavoriteAsync`

Az adatbázis inicializálása a `DatabaseService` konstruktorában indul el. A táblákat a `CreateTableAsync` hozza létre, ha még nem léteznek.

A `MauiProgram.cs` fájlban szerepel a `SQLitePCL.Batteries_V2.Init();` hívás is, ami az SQLite működéséhez szükséges.

## Megosztás

A receptmegosztás a `Services/RecipeShareService.cs` fájlban van.

A megosztás működése:

- összeállít egy receptszöveget
- létrehoz egy `.txt` fájlt a cache mappában
- ha van kép, akkor azt is előkészíti megosztáshoz
- online kép esetén letölti a képet cache-be
- helyi kép esetén átmásolja cache-be
- `ShareMultipleFilesRequest` segítségével megosztja a fájlokat

Ha a kép előkészítése nem sikerül, akkor az app nem omlik össze, hanem legalább a szöveges receptet megpróbálja megosztani.

Ez egy tudatos egyszerűsítés: a lényeg az, hogy a megosztás működjön, ne pedig az, hogy minden eszközön minden megosztó alkalmazás ugyanúgy kezelje a képet.

## Kamera és engedélyek

A kamera kezelése a `MyRecipeEditViewModel.TakePhotoAsync` metódusban van.

Itt történik:

- ellenőrzés, hogy támogatott-e a fotókészítés
- kamera engedély ellenőrzése
- kamera engedély kérése
- fotó készítése `MediaPicker.Default.CapturePhotoAsync()` segítségével
- hiba esetén galéria fallback

Androidon a szükséges engedélyek az `Platforms/Android/AndroidManifest.xml` fájlban vannak.

iOS-en a kamera és fotó szöveges engedélykérései az `Platforms/iOS/Info.plist` fájlban vannak.

## Light mode

Az alkalmazás light mode-ra van kényszerítve az `App.xaml.cs` fájlban:

```csharp
UserAppTheme = AppTheme.Light;
```

Ennek az oka az volt, hogy Androidon dark mode mellett néhány alapértelmezett szín rosszul látszott. Mivel a projekt célja egy egyszerű működő MVP volt, ezért egyszerűbb és stabilabb megoldás volt light mode-ot használni, mint teljes dark/light témakezelést készíteni.

## Gomb animációs probléma

Androidon előjött egy olyan hiba, hogy listában lévő gomboknál több gomb egyszerre tűnt lenyomottnak.

Ez a Command állapotkezelés miatt történhetett listás elemeknél. A megoldás az lett, hogy a listaelemekhez tartozó parancsoknál engedélyezve lett a párhuzamos command végrehajtás:

```csharp
[RelayCommand(AllowConcurrentExecutions = true)]
```

Ez több helyen szerepel:

- `SearchViewModel.OpenDetailAsync`
- `FavoritesViewModel.DeleteFavoriteAsync`
- `FavoritesViewModel.ShareRecipeAsync`
- `FavoritesViewModel.NavigateToDetailAsync`
- `MyRecipesViewModel.DeleteRecipeAsync`
- `MyRecipesViewModel.NavigateToEditAsync`
- `MyRecipesViewModel.NavigateToDetailAsync`

Fontos: ez nem azért van, hogy ténylegesen sok művelet fusson egyszerre, hanem hogy a CommunityToolkit által generált command ne tiltsa le egyszerre a listában lévő azonos commandhoz kötött gombokat.

## Android és Windows közötti eltérések

A projekt .NET MAUI alkalmazás, tehát ugyanaz a kódbázis fut Androidon és Windowson is.

Viszont a MAUI natív vezérlőket használ. Ez azt jelenti, hogy bizonyos elemek platformonként kicsit máshogy nézhetnek ki vagy helyezkedhetnek el.

Példa:

- Androidon a tab navigáció általában alul jelenik meg
- Windowson a Shell/TabBar megjelenése eltérhet

Ez nem külön Android és Windows kódot jelent, hanem ugyanaz a MAUI Shell más natív megjelenéssel renderelődik.

## Bemutatási forgatókönyv

Ezt érdemes bemutatni a tanárnak:

1. Indítsd el az alkalmazást.
2. Mutasd meg az alsó navigációt: `Keresés`, `Kedvencek`, `Saját`.
3. A keresőben keress rá például arra, hogy `egg` vagy `potato`.
4. Nyiss meg egy találatot a részletoldalon.
5. Mutasd meg a hozzávalókat, elkészítést és a képet.
6. Add hozzá kedvencekhez.
7. Menj át a `Kedvencek` oldalra.
8. Nyisd meg ugyanazt a receptet kedvencekből.
9. Próbáld ki a megosztást.
10. Töröld a kedvencet.
11. Menj át a `Saját` oldalra.
12. Hozz létre egy saját receptet.
13. Adj hozzá képet kamerával vagy galériából.
14. Mentsd el.
15. Nyisd meg a saját recept részleteit.
16. Szerkeszd a receptet.
17. Töröld a saját receptet.
18. Ha van idő, zárd be és nyisd újra az appot, majd mutasd meg, hogy az SQLite-ba mentett adatok megmaradnak.

## Tipikus tanári kérdések és válaszok

### Hol van megvalósítva a keresés?

A keresés UI-ja a `Views/SearchPage.xaml` fájlban van. A logika a `ViewModels/SearchViewModel.cs` fájlban található. Az online API hívást a `Services/MealApiService.cs` végzi.

### Milyen API-t használ az app?

TheMealDB API-t használ. A `search.php?s=...` végponttal keres recepteket, a `lookup.php?i=...` végponttal pedig egy konkrét recept részleteit kéri le.

### Hol van az adatbázis kezelés?

A `Services/DatabaseService.cs` fájlban. Itt van a SQLite kapcsolat, a táblák létrehozása, a saját receptek és kedvencek CRUD műveletei.

### Hol vannak a modellek?

A `Models` mappában. A fontosabb modellek: `Meal`, `MealResponse`, `FavoriteRecipe`, `MyRecipe`.

### Hogyan vannak mentve a kedvencek?

A kedvencek SQLite adatbázisba kerülnek `FavoriteRecipe` rekordként. Mentés előtt az app ellenőrzi, hogy a `MealId` alapján létezik-e már ugyanaz a recept, így nem jön létre duplikált kedvenc.

### Hol van megoldva a saját receptek CRUD-ja?

A listaoldal logikája a `MyRecipesViewModel.cs` fájlban van. A létrehozás és szerkesztés logikája a `MyRecipeEditViewModel.cs` fájlban van. A tényleges adatbázis műveletek a `DatabaseService.cs` fájlban vannak.

### Hogyan működik a navigáció?

Shell navigációt használ az app. A fő tabok az `AppShell.xaml` fájlban vannak. A részletoldal és szerkesztőoldal route-jai az `AppShell.xaml.cs` fájlban vannak regisztrálva. A route nevek az `AppRoutes.cs` fájlban vannak konstansként.

### Miért MVVM-et használtál?

Azért, hogy a UI és a logika külön legyen választva. A XAML oldalak főleg megjelenítenek, a ViewModel-ek kezelik az állapotot és a commandokat, a service-ek pedig az API-t, adatbázist és megosztást.

### Hol történik a fotó készítése?

A `MyRecipeEditViewModel.TakePhotoAsync` metódusban. Ez ellenőrzi a kamera támogatást és engedélyt, majd a MAUI `MediaPicker` segítségével készít fotót. Ha ez nem működik, akkor galéria választást ajánl fel.

### Hol van megoldva a megosztás?

A `RecipeShareService.cs` fájlban. A service létrehoz egy szövegfájlt a recept adataiból, és ha van kép, akkor azt is hozzáadja a megosztáshoz.

### Miért van light mode-ra állítva az app?

Azért, mert Androidon dark mode-ban néhány alapértelmezett szín rosszul látszott. Mivel MVP alkalmazás volt a cél, a stabil és egyszerű megoldás a light mode használata lett.

### Miért néz ki kicsit máshogy Androidon és Windowson?

Mert a .NET MAUI natív platformvezérlőket használ. Ugyanaz a kód fut, de a rendszer a saját platformos megjelenésének megfelelően rendereli a Shellt, TabBart és más vezérlőket.

### Miért nincs automata teszt?

A feladatnál az volt a cél, hogy egyszerű, működő MVP készüljön. A tesztelés manuálisan történik, a fő felhasználói folyamatok végigkattintásával.

## Mit érdemes hangsúlyozni védéskor?

- Egy kódbázis fut Androidon és Windowson is.
- Az app nem csak memóriában tárol, hanem SQLite adatbázisba ment.
- A keresés valódi külső API-ból dolgozik.
- A saját receptek offline is megmaradnak.
- A kedvencek duplikáció ellen védve vannak.
- A kamera és galéria használata platformengedélyekkel van kezelve.
- A megosztás nem csak sima szöveg, hanem fájl alapú, és képet is tud mellékelni.
- A projekt egyszerű, de a fő rétegek külön vannak választva.

## Ha valami nem működik bemutatás közben

Ha az API nem ad választ:

- mondd el, hogy külső online API-t használ az app
- internetkapcsolat szükséges a kereséshez
- a már mentett kedvencek és saját receptek ettől még SQLite-ból működnek

Ha a kamera nem nyílik meg:

- mondd el, hogy a kamera eszköz- és engedélyfüggő
- az app ezért galéria fallbacket is tartalmaz

Ha Windows és Android kinézet eltér:

- mondd el, hogy MAUI natív megjelenítést használ
- ez ugyanaz a közös kód, csak platformonként eltérő natív UI-val

Ha a tanár a designra kérdez:

- mondd el, hogy a feladat MVP jellegű volt
- a fő cél a működő funkciók elkészítése volt, nem egy végleges designrendszer
