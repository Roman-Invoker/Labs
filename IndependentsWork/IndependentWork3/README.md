# Звіт з аналізу інкапсуляції в Open-Source проєкті

**Самостійна робота №3** · Об'єктно-орієнтоване програмування
**Тема:** Аналіз інкапсуляції в open-source проєктах. Практики валідації полів.
**Виконав:** _[ПІБ студента]_

---

## 1. Обраний проєкт

| | |
|---|---|
| **Назва** | ASP.NET Core Benchmarks (`aspnet/Benchmarks`) |
| **Посилання на GitHub** | <https://github.com/aspnet/Benchmarks> |
| **Мова / платформа** | C#, .NET, ASP.NET Core |
| **Призначення** | Набір тестових застосунків для вимірювання продуктивності ASP.NET Core (сценарії TechEmpower та інші) |

Проєкт містить понад 250 файлів `.cs`: веб-застосунки (MVC, Razor Pages, Blazor, Minimal API), моделі даних, шар доступу до БД та допоміжні класи. Тому в ньому є достатньо різних за призначенням класів для аналізу інкапсуляції.

### Проаналізовані файли

| № | Клас | Файл у репозиторії |
|---|------|--------------------|
| 1 | `Todo` | [src/BenchmarksApps/TodosApi/Todo.cs](https://github.com/aspnet/Benchmarks/blob/main/src/BenchmarksApps/TodosApi/Todo.cs) |
| 2 | `Address` | [src/BenchmarksApps/BlazorBlazingPizza/Model/Address.cs](https://github.com/aspnet/Benchmarks/blob/main/src/BenchmarksApps/BlazorBlazingPizza/Model/Address.cs) |
| 3 | `FortunesRazorParameters` | [src/BenchmarksApps/TechEmpower/BlazorSSR/FortunesRazorParameters.cs](https://github.com/aspnet/Benchmarks/blob/main/src/BenchmarksApps/TechEmpower/BlazorSSR/FortunesRazorParameters.cs) |

Для розділу про валідацію використано також:

- [src/BenchmarksApps/TodosApi/ValidationFilter.cs](https://github.com/aspnet/Benchmarks/blob/main/src/BenchmarksApps/TodosApi/ValidationFilter.cs)
- [src/BenchmarksApps/TechEmpower/Minimal/Database/Db.cs](https://github.com/aspnet/Benchmarks/blob/main/src/BenchmarksApps/TechEmpower/Minimal/Database/Db.cs)
- [src/BenchmarksApps/TechEmpower/PlatformBenchmarks/Data/CachedWorld.cs](https://github.com/aspnet/Benchmarks/blob/main/src/BenchmarksApps/TechEmpower/PlatformBenchmarks/Data/CachedWorld.cs)

---

## 2. Аналіз інкапсуляції

### Клас: `Todo`

- **Опис класу:** модель завдання (елемент списку справ) у демонстраційному Web API. Зберігається в БД і передається клієнту у форматі JSON.
- **Поля:** явних приватних полів у класі немає. Усі дані зберігаються у **автоматичних властивостях**, для яких компілятор сам створює приховані приватні поля. Публічних полів немає взагалі, а це правильна практика: зовні доступні лише властивості.
- **Модифікатори доступу:** клас оголошено як `internal sealed partial`. Він видимий лише в межах збірки (`internal`), а успадковувати його не можна (`sealed`). Це приклад інкапсуляції на рівні всього класу.

  ```csharp
  [DataRecordMapper]
  internal sealed partial class Todo : IValidatable
  {
      public int Id { get; set; }
      public string Title { get; set; } = default!;
      public DateOnly? DueBy { get; set; }
      public bool IsComplete { get; set; }
  ```

- **Властивості:** усі чотири властивості автоматичні з відкритими `get` і `set`. Властивостей лише для читання чи лише для запису тут немає. Відкритий `set` потрібен, щоб бібліотека відображення даних (`Nanorm`) і десеріалізатор JSON могли заповнювати об'єкт.
- **Індексатори/Оператори:** немає.
- **Зауваження:** кожен `set` приймає будь-яке значення, тому клас сам не гарантує коректного стану. Правила перевірки винесено в окремий метод `Validate` (див. розділ 3).

### Клас: `Address`

- **Опис класу:** модель адреси доставки в застосунку піцерії (Blazor). Описує одержувача та місце доставки.
- **Поля:** явних полів немає, як і в `Todo`: дані зберігаються в автоматичних властивостях.
- **Властивості:** сім публічних автоматичних властивостей (`Id`, `Name`, `Line1`, `Line2`, `City`, `Region`, `PostalCode`) з `get` і `set`. Властивість `Line2` не позначена `[Required]`, тобто є необов'язковою.

  ```csharp
  public class Address
  {
      public int Id { get; set; }

      [Required, MaxLength(100)]
      public string Name { get; set; }

      [Required, MaxLength(100)]
      public string Line1 { get; set; }

      [MaxLength(100)]
      public string Line2 { get; set; }
      ...
  ```

- **Індексатори/Оператори:** немає.
- **Зауваження:** клас є простим контейнером даних (DTO / модель EF). Інкапсуляція тут мінімальна: усе відкрито, а обмеження описано **декларативно атрибутами** (розділ 3).

### Клас: `FortunesRazorParameters`

- **Опис класу:** допоміжна структура, що передає модель (список `Fortune`) у Razor-компонент як набір параметрів. Реалізує інтерфейс `IReadOnlyDictionary<string, object?>`.
- **Поля:** є приватні елементи: константа `ModelKeyName` і поле `_modelKvp`. Структура оголошена як `readonly struct`, тому її стан не можна змінити після створення.

  ```csharp
  internal readonly struct FortunesRazorParameters(List<Fortune> model)
      : IReadOnlyDictionary<string, object?>
  {
      private const string ModelKeyName = nameof(FortunesParameters.Rows);

      private readonly KeyValuePair<string, object?> _modelKvp = new(ModelKeyName, model);
  ```

- **Властивості:** `Keys`, `Values`, `Count` оголошені лише з `get` (**read-only**) та ініціалізуються один раз:

  ```csharp
  public IEnumerable<string> Keys { get; } = [ModelKeyName];
  public IEnumerable<object?> Values { get; } = [model];
  public int Count { get; } = 1;
  ```

  Це повна протилежність до `Todo` і `Address`: після створення об'єкта змінити ці дані неможливо.
- **Індексатори/Оператори:** є **індексатор** лише для читання, який повертає модель за ключем `Rows`, а для будь-якого іншого ключа повертає `null`:

  ```csharp
  public object? this[string key] => KeyIsModel(key) ? model : null;
  ```

  Він потрібен, щоб структура поводилась як словник параметрів, який очікує Razor. Приватний метод `KeyIsModel` приховує, як саме порівнюється ключ:

  ```csharp
  private static bool KeyIsModel(string key) => ModelKeyName.Equals(key, StringComparison.Ordinal);
  ```

  Вкладена структура `Enumerator` теж приватна, а її метод `Reset` навмисно кидає `NotSupportedException`.

### Додатково: перевантажений оператор

У класі `CachedWorld` (див. [файл](https://github.com/aspnet/Benchmarks/blob/main/src/BenchmarksApps/TechEmpower/PlatformBenchmarks/Data/CachedWorld.cs)) є **неявний оператор перетворення** з `World` у `CachedWorld`. Він дає змогу писати `CachedWorld c = world;`, при цьому логіка копіювання полів ховається всередині класу:

```csharp
public static implicit operator CachedWorld(World world) =>
    new CachedWorld { Id = world.Id, RandomNumber = world.RandomNumber };
```

### Порівняння трьох класів

| Ознака | `Todo` | `Address` | `FortunesRazorParameters` |
|--------|--------|-----------|---------------------------|
| Тип | `internal sealed class` | `public class` | `internal readonly struct` |
| Приватні поля | лише приховані (автовластивості) | лише приховані (автовластивості) | є (`_modelKvp`, константа) |
| Публічні поля | немає | немає | немає |
| Read-only властивості | немає | немає | `Keys`, `Values`, `Count` |
| Автоматичні властивості | так | так | так (з `get`) |
| Індексатор | немає | немає | є, лише для читання |
| Валідація | метод `Validate` | атрибути | не потрібна |

---

## 3. Практики валідації у властивостях

> **Важливе спостереження.** У проаналізованому коді **немає валідації всередині `set`-аксесорів**: усі властивості мають вигляд `{ get; set; }` без тіла (це перевірено пошуком по всіх файлах `.cs` проєкту). Проєкт використовує інші, більш сучасні для ASP.NET Core механізми, які описано нижче.

### Приклад 1. Метод `Validate` та інтерфейс `IValidatableObject` (клас `Todo`)

Клас реалізує інтерфейс, що містить метод `Validate`. Він перевіряє одразу кілька властивостей і повертає **всі знайдені помилки** через `yield return`:

```csharp
public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
{
    if (string.IsNullOrEmpty(Title))
    {
        yield return new ValidationResult($"A value is required for {nameof(Title)}.", new[] { nameof(Title) });
        yield break;
    }
    if (DueBy.HasValue && DueBy.Value < DateOnly.FromDateTime(DateTime.UtcNow))
    {
        yield return new ValidationResult($"{nameof(DueBy)} cannot be in the past.", new[] { nameof(DueBy) });
    }
}
```

Перевіряється, що `Title` не порожній і що дата виконання `DueBy` не в минулому. Метод викликає не сам клас, а `ValidationFilter` (див. приклад 4).

### Приклад 2. Атрибути валідації `DataAnnotations` (клас `Address`)

Правила описано декларативно, тобто прямо над властивістю:

```csharp
[Required, MaxLength(100)]
public string Name { get; set; }

[Required, MaxLength(20)]
public string PostalCode { get; set; }
```

`[Required]` забороняє `null` та порожнє значення, а `[MaxLength(n)]` обмежує довжину рядка. Ці атрибути використовують і ASP.NET Core (для перевірки введених даних), і Entity Framework (довжина стовпця в БД).

### Приклад 3. Перевірка аргументів із викиданням винятку (клас `Db`)

У конструкторі класу доступу до БД використано **охоронну умову** (guard clause):

```csharp
public Db(AppSettings appSettings)
{
    ArgumentException.ThrowIfNullOrEmpty(appSettings.ConnectionString);

    _dbProviderFactory = Npgsql.NpgsqlFactory.Instance;
    _connectionString = appSettings.ConnectionString;
}
```

Об'єкт не буде створено, якщо рядок підключення порожній. У цьому ж класі інший підхід: параметр не відхиляється, а **обмежується допустимим діапазоном**:

```csharp
count = Math.Clamp(count, 1, 500);
```

### Приклад 4. Централізована валідація (`ValidationFilter`)

Фільтр викликає `Validate`, збирає помилки в словник і замість виконання обробника повертає клієнту відповідь **400 Bad Request** з описом проблем:

```csharp
var results = validatable.Validate(validationContext);

if (results.Any())
{
    ...
    var errors = MapToFinalErrorsResult(workingErrors);
    var problem = TypedResults.ValidationProblem(errors);
    return ValueTask.FromResult((object?)problem);
}
```

### Висновки щодо валідації

| Підхід | Переваги | Недоліки |
|--------|----------|----------|
| `IValidatableObject.Validate` | можна перевіряти кілька властивостей разом; повертає всі помилки одразу | клас не захищений від некоректного стану, бо перевірку треба викликати окремо |
| Атрибути `DataAnnotations` | коротко, читається одразу біля поля; одне правило працює і для API, і для БД | складні правила (залежність між полями) атрибутами не описати |
| `ThrowIfNullOrEmpty` у конструкторі | некоректний об'єкт неможливо створити | підходить для програмних помилок, а не для даних користувача |
| `Math.Clamp` | не ламає роботу; проста реалізація | приховує помилку: некоректне значення замінюється мовчки |

---

## 4. Загальні висновки

1. **Інкапсуляція в проєкті реалізована на різних рівнях.** Публічних полів немає взагалі, а доступ до даних іде через властивості. Клас `Todo` ще й закритий модифікаторами `internal sealed`, а `FortunesRazorParameters` є незмінною структурою з приватним станом і read-only API.
2. **Рівень «суворості» залежить від ролі класу.** Моделі даних (`Todo`, `Address`) навмисно прості: відкриті `get` і `set` потрібні фреймворкам для заповнення об'єктів. Службові класи (`FortunesRazorParameters`, `Db`) захищені значно сильніше: поля приховані, стан незмінний.
3. **Валідація винесена за межі властивостей.** Замість перевірки в `set` автори обрали атрибути, метод `Validate` і спільний фільтр. Це доречно для веб-API: усі помилки повертаються клієнту одним списком, а правила не розкидані по класу. Недолік у тому, що сам об'єкт `Todo` можна створити з порожнім `Title`, а перевірка відбудеться лише тоді, коли її викличе фільтр.
4. **Для навчального коду з простим доменом** (наприклад, `Product` із самостійної №2) краще перевіряти значення вже в конструкторі чи `set`-аксесорі, тоді некоректний об'єкт створити неможливо. У великих проєктах із фреймворками цей підхід часто замінюють зовнішньою валідацією, як у `aspnet/Benchmarks`.

---

## Скріншот

> Тут має бути скріншот сторінки одного з проаналізованих файлів на GitHub (наприклад, `Todo.cs`) із видимою адресою в браузері.
> Збережіть зображення як `screenshots/example.png`, і воно з'явиться нижче.

![Скріншот проаналізованого файлу на GitHub](screenshots/example.png)
