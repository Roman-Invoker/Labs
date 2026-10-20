# Звіт. Самостійна робота №8

**Тема:** Dictionary<TKey, TValue> та HashSet<T> у бібліотеках .NET
**Виконав:** _[ПІБ студента]_

---

## 1. Результати аналізу open-source проєктів

Проаналізовано три приклади (Dictionary, HashSet, комбінований) та один допоміжний.

| № | Колекція | Проєкт | Місце в коді |
|---|----------|--------|--------------|
| 1 | `Dictionary<TKey, TValue>` | AutoMapper Enum Extensions | `src/AutoMapper.Extensions.EnumMapping/Internal/EnumMappingFeature.cs`, `CustomMapExpressionFactory.cs` |
| 2 | `HashSet<T>` | ASP.NET Core Benchmarks | `src/BenchmarksApps/TLS/Kestrel/Program.cs` |
| 3 | `Dictionary` + `HashSet` разом | Selenium (.NET) | клас `Preferences`, `dotnet/src/webdriver/Firefox/Preferences.cs` |
| 4 | `Dictionary` з custom comparer (допоміжний) | ASP.NET Core Benchmarks | `src/BenchmarksApps/Samples/Template/Startup.cs` |

Посилання:

- AutoMapper Enum Extensions: <https://github.com/AutoMapper/AutoMapper.Extensions.EnumMapping> (посилання складено за назвою архіву проєкту, перевірте, що воно відкривається)
- ASP.NET Core Benchmarks: <https://github.com/aspnet/Benchmarks>
- Selenium, пул-реквест із розглядом класу `Preferences`: <https://github.com/SeleniumHQ/selenium/pull/14646/files> (код у версії з пул-реквесту: [Preferences.cs](https://github.com/RenderMichael/selenium/blob/55d9383909e7a3dc7adb35c96f4cfaf37f8abd62/dotnet/src/webdriver/Firefox/Preferences.cs))

### Приклад 1. `Dictionary<TKey, TValue>`: таблиця відповідності значень enum (AutoMapper Enum Extensions)

**Код** ([`EnumMappingFeature.cs`](https://github.com/AutoMapper/AutoMapper.Extensions.EnumMapping/blob/master/src/AutoMapper.Extensions.EnumMapping/Internal/EnumMappingFeature.cs), рядки 16 і 53; [`CustomMapExpressionFactory.cs`](https://github.com/AutoMapper/AutoMapper.Extensions.EnumMapping/blob/master/src/AutoMapper.Extensions.EnumMapping/Internal/CustomMapExpressionFactory.cs), рядок 25):

```csharp
protected readonly Dictionary<TSource, GetDestinationObject<TDestination>> EnumValueMappingsOverride = new();
...
var enumValueMappings = new Dictionary<TSource, GetDestinationObject<TDestination>>();
...
// CustomMapExpressionFactory.ConvertEnumValue
if (!_enumValueMappings.TryGetValue(source, out var getDestinationObject))
{
    throw new AutoMapperMappingException($"Value {source} of type {source.GetType().FullName} not supported");
}
return getDestinationObject.GetDestinationFunc.Invoke();
```

| Питання | Аналіз |
|---------|--------|
| **Контекст** | Розширення AutoMapper відображає значення одного `enum` на значення іншого. Для цього будується таблиця «значення вихідного enum → результат перетворення». Користувач може перевизначити окремі пари (`EnumValueMappingsOverride`), а решта береться за замовчуванням (за ім'ям або значенням). |
| **Чому Dictionary** | Дані мають природну форму «ключ → значення»: за значенням вихідного enum потрібно швидко знайти, у що його перетворити. `List<KeyValuePair>` вимагав би лінійного пошуку під час **кожного** перетворення. Індексація масивом була б можлива лише для суцільних числових значень, а enum часто має «дірки». |
| **Складність** | `TryGetValue`, `ContainsKey`, додавання через індексатор: O(1) у середньому (O(n) у найгіршому випадку при колізіях). Побудова таблиці: O(n) за кількістю значень enum. |
| **Особливі вимоги** | Ключ - значення `enum` (`struct`), тому використовується стандартний comparer, власний не потрібен. Для відсутнього ключа застосовано `TryGetValue`, а не індексатор: так виняток із зрозумілим повідомленням формується самим кодом, а не `KeyNotFoundException`. Валідація перевіряє повноту таблиці через `_enumValueMappings.ContainsKey(...)` (файл `EnumMappingValidationRuntimeFeature.cs`, рядок 39). |

### Приклад 2. `HashSet<T>`: унікальні з'єднання та дозволені хости (ASP.NET Core Benchmarks)

**Код** ([`TLS/Kestrel/Program.cs`](https://github.com/aspnet/Benchmarks/blob/main/src/BenchmarksApps/TLS/Kestrel/Program.cs), рядки 54, 61-69, 197-198):

```csharp
var connectionIds = new HashSet<string>();
...
var allowedHosts = new HashSet<string>();
foreach (var endpoint in listeningEndpoints.Split([';'], StringSplitOptions.RemoveEmptyEntries))
{
    var urlPrefix = UrlPrefix.Create(endpoint);
    allowedHosts.Add(urlPrefix.Host);
}
...
options.AllowedHosts = allowedHosts.ToArray();
...
connectionIds.Add(context.Connection.Id);
Console.WriteLine($"[stats] unique connections established: {connectionIds.Count}; ...");
```

| Питання | Аналіз |
|---------|--------|
| **Контекст** | Тестовий TLS-сервер на Kestrel. У режимі статистики для кожного запиту записується ідентифікатор з'єднання, щоб порахувати **кількість унікальних з'єднань**. Окремо з адрес, на яких слухає сервер, збирається набір дозволених хостів для `HostFilteringOptions`. |
| **Чому HashSet** | Потрібна лише відповідь на питання «чи вже бачили цей елемент?» і кількість різних елементів. Через `HashSet` один запис з'єднання не рахується двічі, хоч запитів у ньому багато, а в `List` довелось би перед кожним додаванням шукати елемент лінійно (O(n)) або дедублікувати вручну. Значення (пари) тут не потрібні, тому `Dictionary` був би зайвим. |
| **Складність** | `Add` і `Contains`: O(1) у середньому. `Count`: O(1). `ToArray()`: O(n). |
| **Особливі вимоги** | Comparer стандартний, для `string` він чутливий до регістру. **Потокобезпека:** middleware виконується для кожного запиту, а запити можуть оброблятися паралельно, тоді як `HashSet` не є потокобезпечним. У діагностичному коді бенчмарку це прийнятне спрощення, а в реальному застосунку потрібен `lock` або `ConcurrentDictionary<string, byte>`. |

### Приклад 3. `Dictionary` і `HashSet` разом: налаштування профілю Firefox (Selenium)

**Код** (клас `Preferences`, версія з [PR #14646](https://github.com/SeleniumHQ/selenium/pull/14646/files)):

```csharp
private readonly Dictionary<string, string> preferences = new Dictionary<string, string>();
private readonly HashSet<string> immutablePreferences = new HashSet<string>();
...
foreach (KeyValuePair<string, JsonNode?> pref in defaultImmutablePreferences)
{
    this.SetPreferenceValue(pref.Key, pref.Value);
    this.immutablePreferences.Add(pref.Key);
}
...
private bool IsSettablePreference(string preferenceName)
{
    return !this.immutablePreferences.Contains(preferenceName);
}
```

| Питання | Аналіз |
|---------|--------|
| **Контекст** | Клас готує налаштування профілю браузера Firefox для Selenium WebDriver. Є налаштування, які можна змінювати, і «заморожені» (`frozen`), які перевизначати заборонено. |
| **Чому саме так** | `Dictionary<string, string> preferences` зберігає **пари** «назва налаштування → значення», тому йому потрібні значення. А для заморожених налаштувань потрібно лише знати, **чи входить назва в їх набір**, тож достатньо `HashSet<string>`. Автор пул-реквесту пояснює, що значення заморожених налаштувань і так лежать у словнику `preferences` (звідси й повідомлення про помилку), тому дублювати їх у другому словнику немає сенсу: у попередній версії це був `Dictionary<string, string>`, який замінили на `HashSet<string>` з ключами. Це хороша ілюстрація правила: **якщо значення не потрібні, беріть множину, а не словник**. |
| **Складність** | `preferences[key] = ...` і `immutablePreferences.Contains(key)`: O(1) у середньому. |
| **Особливі вимоги** | Імена налаштувань порівнюються стандартним (чутливим до регістру) comparer, що відповідає форматові Firefox. |
| **Примітка** | Пул-реквест на момент перегляду мав статус «Closed», тому наведено **версію коду з пул-реквесту**; актуальний стан файлу в основній гілці Selenium я не перевіряв. Перед здачею варто відкрити `Preferences.cs` у репозиторії й порівняти. |

### Допоміжний приклад. Dictionary з custom comparer (ASP.NET Core Benchmarks)

[`Samples/Template/Startup.cs`](https://github.com/aspnet/Benchmarks/blob/main/src/BenchmarksApps/Samples/Template/Startup.cs):

```csharp
var data = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);
for (var i = 0; i < count; i++)
{
    data.TryAdd("Id_", i);
}
```

Це бенчмарк вартості вставки в словник із нечутливим до регістру comparer (`TryAdd` не кидає виняток на дублікат). Показує, що **comparer є частиною вартості**: порівняння без урахування регістру коштує дорожче за порядкове.

### Спостереження про вибір колекції (AutoMapper Enum Extensions)

У методі `Reverse()` (файл `EnumMappingFeature.cs`, рядки 167-169) із ключів словника будується `List`, у якому потім виконується `sourceValues.Contains(...)`, тобто лінійний пошук O(n). Списки там дуже короткі (кілька значень enum), тому це виправдано простотою, але зі зростанням розміру `HashSet` був би кращим вибором.

---

## 2. Порівняльна таблиця складності операцій

Складність наведено для **середнього випадку**; у `Dictionary` і `HashSet` у найгіршому випадку (багато колізій хешів) O(n).

| Операція | `List<T>` | `Dictionary<TKey,TValue>` | `HashSet<T>` | Коли використовувати |
|----------|-----------|---------------------------|--------------|----------------------|
| Додавання | O(1) амортизовано (у кінець); O(n) у середину | O(1) | O(1) | `List`: потрібен порядок і доступ за індексом. `Dictionary`: потрібно прив'язати значення до ключа. `HashSet`: збираємо унікальні елементи. |
| Пошук за значенням | O(n) | O(n) через `ContainsValue` (для звичайного пошуку не призначений) | O(1) | Якщо часто шукаєте за значенням, використовуйте `HashSet`; не використовуйте для цього `List` на великих обсягах. |
| Пошук за ключем | N/A (за індексом O(1)) | O(1) | N/A | Швидкий доступ за ідентифікатором: кеші, довідники, індекси. |
| Видалення | O(n) | O(1) | O(1) | Часті видалення з великої колекції: `Dictionary`/`HashSet`; `List` лише для малих обсягів. |
| Перевірка наявності | O(n) (`Contains`) | O(1) (`ContainsKey`) | O(1) (`Contains`) | Часті перевірки «чи є елемент?»: `HashSet` або ключі `Dictionary`. |

Додатково: операції над множинами `UnionWith`, `IntersectWith`, `ExceptWith` у `HashSet` лінійні за розміром колекцій (O(n) або O(n + m)), а в `List` для цього довелося б писати вкладені цикли O(n·m).

---

## 3. Висновки про вибір колекції

### Коли Dictionary краще за List?

- Коли потрібен **швидкий доступ за ключем** (ідентифікатор, ім'я, слово): O(1) замість O(n). У сценарії 1 кеш метаданих шукає файл за іменем.
- Коли дані природно мають вигляд «пара ключ-значення» (довідники, кеші, лічильники, таблиці відповідності, як `EnumValueMappings` в AutoMapper).
- Коли ключі **унікальні**, а колекція велика й часто читається.
- `List` кращий, якщо важливий **порядок**, дозволені дублікати або потрібен доступ за індексом, а обсяг невеликий.

### Коли HashSet є оптимальним вибором?

- Коли потрібно **зберігати лише унікальні елементи** і швидко перевіряти їх наявність (унікальні з'єднання в Kestrel, множина заморожених налаштувань у Selenium).
- Коли потрібні **теоретико-множинні операції**: об'єднання, перетин, різниця (спільні теги статей, пошук за кількома словами).
- Коли значення до ключа не потрібні (тоді `Dictionary` зайвий).
- Не підходить, якщо важливий порядок елементів або доступ за індексом.

### Підводні камені

1. **Узгодженість `Equals` і `GetHashCode`.** Якщо об'єкти рівні, їх хеш-коди мусять збігатись; інакше елемент «загубиться» в колекції. Саме тому `PersonNameComparer` реалізує обидва методи на основі одного поля.
2. **Змінювані ключі.** Якщо після додавання змінити поле, що входить у хеш, об'єкт не буде знайдено. Ключами краще робити незмінні значення (`string`, `int`, `enum`).
3. **Comparer впливає і на коректність, і на швидкість.** Без нього рядки порівнюються з урахуванням регістру (`"Hello"` і `"HELLO"` це різні ключі). Нечутливий до регістру comparer змінює результат пошуку й трохи дорожчий.
4. **Потокобезпечність.** `Dictionary` і `HashSet` не потокобезпечні: для паралельного доступу потрібні `lock` або `ConcurrentDictionary`.
5. **Порядок обходу не гарантовано.** Не покладайтеся на нього. У демонстрації елементи сортуються перед виводом.
6. **Зміна під час обходу.** Додавання чи видалення елементів у `foreach` кидає `InvalidOperationException`.
7. **Виняток за відсутнім ключем.** Індексатор `dict[key]` кидає `KeyNotFoundException`, а `Add` з наявним ключем кидає `ArgumentException`. Тому `TryGetValue`, `TryAdd` і `ContainsKey` часто надійніші.
8. **Пам'ять і колізії.** Хеш-таблиці займають більше пам'яті, ніж `List`/масив, а за великої кількості колізій хешів операції деградують до O(n).
9. **Повернення внутрішніх колекцій назовні.** Якщо метод повертає внутрішній `HashSet`, зовнішній код може змінити стан класу. У проєкті `TagManager` і `SearchIndex` повертають **копії**.

---

## 4. Приклади з власного коду

Проєкт `IndependentWork8` реалізує всі три сценарії з завдання.

### Сценарій 1: `FileMetadataCache` (Dictionary)

```csharp
private readonly Dictionary<string, FileMetadata> _cache =
    new Dictionary<string, FileMetadata>(StringComparer.OrdinalIgnoreCase);

public void Add(FileMetadata metadata) => _cache[metadata.FileName] = metadata;
public FileMetadata? Get(string fileName) => _cache.TryGetValue(fileName, out var m) ? m : null;
public bool Remove(string fileName) => _cache.Remove(fileName);
public bool Contains(string fileName) => _cache.ContainsKey(fileName);
```

- Ключ - ім'я файлу, і comparer `OrdinalIgnoreCase` робить пошук нечутливим до регістру (`REPORT.DOCX` знаходить `report.docx`).
- `Add` додає або замінює запис; `Get` повертає `null`, а не кидає виняток, для відсутнього файлу.

### Сценарій 2: `TagManager` (Dictionary + HashSet)

```csharp
private readonly Dictionary<string, HashSet<string>> _articleTags = new();

public HashSet<string> GetAllUniqueTags()
{
    var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    foreach (var tags in _articleTags.Values) result.UnionWith(tags);       // об'єднання
    return result;
}

public HashSet<string> FindCommonTags(string id1, string id2)
{
    var common = GetTags(id1);
    common.IntersectWith(GetTags(id2));                                     // перетин
    return common;
}

public HashSet<string> FindTagsOnlyIn(string id, string otherId)
{
    var only = GetTags(id);
    only.ExceptWith(GetTags(otherId));                                      // різниця
    return only;
}
```

- `HashSet` не допускає дублікатів, тому повторне додавання `"CSharp"` після `"csharp"` нічого не змінює.
- Використано всі три операції: `UnionWith`, `IntersectWith`, `ExceptWith`.

### Сценарій 3: `SearchIndex` (Dictionary + HashSet)

```csharp
private readonly Dictionary<string, HashSet<int>> _invertedIndex =
    new Dictionary<string, HashSet<int>>(StringComparer.OrdinalIgnoreCase);

public HashSet<int> SearchMultiple(params string[] words)
{
    HashSet<int> result = Search(words[0]);
    for (int i = 1; i < words.Length && result.Count > 0; i++)
        result.IntersectWith(Search(words[i]));          // документи, що містять УСІ слова
    return result;
}
```

Чому `Dictionary<string, HashSet<int>>` зручна основа інвертованого індексу: `Dictionary` за O(1) знаходить слово, а `HashSet<int>` зберігає документи без дублікатів (слово може повторюватись у тексті) і дозволяє знайти документи з кількома словами одним перетином множин. Пошук за кількома словами не потребує перегляду текстів.

### Додатково: `HashSet<Person>` з custom comparer

```csharp
var people = new HashSet<Person>(new PersonNameComparer());
people.Add(new Person("John", 25));   // true
people.Add(new Person("John", 30));   // false: те саме ім'я
```

### Приклад виводу

Повний вивід програми наведено в [README.md](README.md).
