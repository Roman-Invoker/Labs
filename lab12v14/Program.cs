using System.Text;
using lab18v14.Helpers;
using lab18v14.Services;

// Лабораторна робота №12. Обробка IO/мережевих помилок та патерн Retry.
// Варіант 14: Оновлення профілю користувача.
//   FileProcessor.UpdateUserProfile  — FileNotFoundException перші 3 рази
//   NetworkClient.PostUserProfile    — HttpRequestException перші 2 рази
//   shouldRetry: повторювати для FileNotFoundException та HttpRequestException

Console.OutputEncoding = Encoding.UTF8;

const string profileJson = "{ \"id\": 14, \"name\": \"Роман\", \"role\": \"student\" }";
TimeSpan initialDelay = TimeSpan.FromMilliseconds(200);

// Повторюємо лише для винятків, зазначених у варіанті.
static bool ShouldRetry(Exception ex) => ex is FileNotFoundException or HttpRequestException;

// ---------------------------------------------------------------
Header("1. FileProcessor: 3 невдачі (FileNotFoundException), потім успіх");
var fileProcessor = new FileProcessor(failuresBeforeSuccess: 3);

// retryCount = 3 → до 4 спроб загалом. RetryHelper узагальнений (Func<T>),
// тому void-метод загортаємо в Func<bool>.
bool fileDone = RetryHelper.ExecuteWithRetry(
    () =>
    {
        fileProcessor.UpdateUserProfile("profiles/user14.json", profileJson);
        return true;
    },
    retryCount: 3,
    initialDelay: initialDelay,
    shouldRetry: ShouldRetry);
Console.WriteLine($"Результат: {fileDone}, викликів UpdateUserProfile: {fileProcessor.Calls}");

// ---------------------------------------------------------------
Header("2. NetworkClient: 2 невдачі (HttpRequestException), потім успіх");
var networkClient = new NetworkClient(failuresBeforeSuccess: 2);

bool posted = RetryHelper.ExecuteWithRetry(
    () => networkClient.PostUserProfile("https://api.example.com/profiles/14", profileJson),
    retryCount: 3,
    initialDelay: initialDelay,
    shouldRetry: ShouldRetry);
Console.WriteLine($"Результат: {posted}, викликів PostUserProfile: {networkClient.Calls}");

// ---------------------------------------------------------------
Header("3. shouldRetry: виняток, який НЕ входить до списку, не повторюється");
int unauthorizedCalls = 0;
try
{
    RetryHelper.ExecuteWithRetry<bool>(
        () =>
        {
            unauthorizedCalls++;
            throw new UnauthorizedAccessException("Немає прав на запис у файл профілю.");
        },
        retryCount: 3,
        initialDelay: initialDelay,
        shouldRetry: ShouldRetry);
}
catch (UnauthorizedAccessException ex)
{
    Console.WriteLine($"Піймано без повторів: {ex.Message} (викликів операції: {unauthorizedCalls})");
}

// ---------------------------------------------------------------
Header("4. Вичерпання спроб: retryCount = 1, а FileProcessor падає 3 рази");
var stubbornFileProcessor = new FileProcessor(failuresBeforeSuccess: 3);
try
{
    RetryHelper.ExecuteWithRetry(
        () =>
        {
            stubbornFileProcessor.UpdateUserProfile("profiles/user14.json", profileJson);
            return true;
        },
        retryCount: 1,
        initialDelay: initialDelay,
        shouldRetry: ShouldRetry);
}
catch (FileNotFoundException ex)
{
    Console.WriteLine($"Піймано після вичерпання спроб: {ex.Message}");
}

static void Header(string title)
{
    Console.WriteLine();
    Console.WriteLine(new string('=', 70));
    Console.WriteLine(title);
    Console.WriteLine(new string('=', 70));
}
