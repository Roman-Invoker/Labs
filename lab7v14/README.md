# Лабораторна робота №7 — Generics, IRepository з where-обмеженнями та тестами

Варіант 14. Сутність **Course**: `Title` (string), `Credits` (int), `TeacherId` (int).

## Структура

- `lab12v14` — консольний проєкт
  - `IEntity` — інтерфейс сутності з властивістю `int Id { get; set; }`.
  - `Course` — сутність за варіантом, реалізує `IEntity`.
  - `IRepository<T>` — узагальнений інтерфейс з обмеженням `where T : IEntity`;
    методи `GetById`, `GetAll`, `Add`, `Update`, `Delete`.
  - `InMemoryRepository<T>` — реалізація, що зберігає дані у `List<T>`.
    `GetById` для неіснуючого Id повертає `null`; `Update` і `Delete` для неіснуючого Id
    кидають `KeyNotFoundException`; `Add` з наявним Id кидає `InvalidOperationException`.
  - `Program.cs` — демонстрація роботи (Add, GetById, Update, Delete).
- `lab12v14.Tests` — xUnit-тести для кожного методу репозиторію, включно з неіснуючими Id.

## Запуск

```bash
dotnet run --project lab12v14
dotnet test lab12v14.Tests
```
