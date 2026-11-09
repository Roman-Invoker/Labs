# Самостійна робота №16 (ООП)

**Тема:** Колекції в практиці ООП: інвертований індекс і вимірювання продуктивності
**Проєкт:** `InvertedIndexDemo`
**Звіт (реалізація, таблиця вимірювань, висновки):** [REPORT.md](REPORT.md)

## Що реалізовано

| Клас | Призначення |
|---|---|
| `Document` | документ (`Id`, `Title`, `Content`) |
| `TextProcessor` | `Tokenize` (регістр + пунктуація через `\w+`) та `RemoveStopWords` |
| `InvertedIndex` | `Dictionary<string, HashSet<int>>` + `Dictionary<int, Document>`; `AddDocument`, `SearchSingleWord`, `SearchMultipleWords` (AND, `IntersectWith`), `SearchAnyWord` (OR, `UnionWith`), `DocumentCount`, `UniqueWords` |
| `PerformanceTest` | порівняння лінійного пошуку та індексу на 10 / 100 / 1000 / 10000 документів |

Особливості:

- Обидва підходи до пошуку використовують **однакову нормалізацію** (`TextProcessor.Tokenize`), тому порівняння коректне; кількості знайдених документів перевіряються на збіг.
- `\w+` не вважає `#` літерою, тому `C#` перетворюється на `c`. Запити токенізуються так само, тому `c# async` знаходить документ 4.
- Пошуки AND/OR працюють з **копіями** множин, тож внутрішній індекс не псується.
- Дані для вимірювань генеруються з фіксованим `Seed = 42`.

## Структура

```text
InvertedIndexDemo/
├── InvertedIndexDemo.csproj
├── Document.cs
├── TextProcessor.cs
├── InvertedIndex.cs
├── PerformanceTest.cs
├── Program.cs
├── README.md
├── REPORT.md
└── docs/            # сюди — скриншот запуску
```

## Запуск

Потрібен [.NET SDK 8.0](https://dotnet.microsoft.com/download) або новіший.

```bash
cd InvertedIndexDemo
dotnet run                      # демонстрація запитів + вимірювання
dotnet run -- --interactive     # інтерактивний режим пошуку (введення з клавіатури)
```

### Приклад виводу

> Час вимірювань залежить від комп'ютера; наведені значення отримано на одноядерному хмарному середовищі.
> Кількість унікальних слів (37) відрізняється від «45» в тексті завдання: це лише ілюстративне значення з методички, а наш токенізатор рахує слова саме так.

```text
Loaded 5 documents
Index contains 37 unique words

Query: "programming language"
--- Search Results (AND) ---
[1] Introduction to C#
  C# is a modern object-oriented programming language developed by Microsoft....
[2] Java Basics
  Java is a popular programming language known for its portability and performance....
[3] Python Overview
  Python is a high-level programming language with simple syntax....

Found 3 documents

Query: "c# async"
--- Search Results (AND) ---
[4] C# Advanced Features
  C# supports LINQ, async/await, and many modern programming paradigms....

Found 1 documents

Query: "java python"
--- Search Results (OR) ---
[2] Java Basics
  Java is a popular programming language known for its portability and performance....
[3] Python Overview
  Python is a high-level programming language with simple syntax....
[5] Comparing Languages
  C#, Java, and Python are among the most popular programming languages today....

Found 3 documents

Query: "kotlin"
--- Search Results (AND) ---

Found 0 documents

=== Performance: лінійний пошук vs інвертований індекс (слово "programming") ===
Linear search: 488622 ticks
Inverted Index: 231471 ticks
Linear found: 1, Index found: 1

Середній час одного запиту (повторні вимірювання):
    10 док. | лінійний:     100.27 мкс | індекс:     1.96 мкс | прискорення:       51x | побудова індексу:     0.18 мс | знайдено 1/1 (збігаються)
   100 док. | лінійний:     625.41 мкс | індекс:     0.98 мкс | прискорення:      641x | побудова індексу:     0.83 мс | знайдено 10/10 (збігаються)
  1000 док. | лінійний:    6318.66 мкс | індекс:     3.25 мкс | прискорення:     1944x | побудова індексу:     9.04 мс | знайдено 51/51 (збігаються)
 10000 док. | лінійний:   66166.22 мкс | індекс:    39.40 мкс | прискорення:     1679x | побудова індексу:   161.60 мс | знайдено 559/559 (збігаються)
```

### Скриншот запуску

> 📸 _Встав сюди скриншот реального запуску `dotnet run`._
>
> `![Приклад запуску](docs/screenshot.png)`

## Відповіді на контрольні запитання

1. **Чому `Dictionary<string, HashSet<int>>` добре підходить для інвертованого індексу?**
   Словник дає перехід від слова до документів за O(1), а `HashSet<int>` не допускає дублікатів ідентифікаторів і дозволяє швидко виконувати операції над множинами.
2. **Які переваги `HashSet<T>` в AND/OR-пошуку?**
   AND — це перетин (`IntersectWith`), OR — об'єднання (`UnionWith`). Вони вбудовані й працюють ефективно, без вкладених циклів.
3. **Чому при вимірюванні важлива однакова нормалізація?**
   Якщо лінійний пошук і індекс по-різному обробляють регістр і пунктуацію, вони знайдуть різні документи, і порівняння часу втрачає сенс, бо порівнюються неоднакові задачі.
4. **Які обмеження простого індексу без ранжування, стемінгу й фраз?**
   Немає впорядкування за релевантністю; «language» і «languages» — різні слова; не можна шукати точну фразу; індекс не оновлюється при видаленні чи зміні документа й займає додаткову пам'ять.
