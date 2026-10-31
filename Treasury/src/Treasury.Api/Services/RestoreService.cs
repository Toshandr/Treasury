using System.Text;
using Treasury.Api.Dtos;

namespace Treasury.Api.Services;

public class RestoreService : IRestoreService
{
    private const int AlphabetSize = 26;

    // O(n + m): один проход по каждой строке, затем фиксированный проход по 26 буквам.
    // Обход алфавита по порядку сразу даёт отсортированный результат, сортировка не нужна.
    public RestoreResponse Restore(string reference, string damaged)
    {
        var inReference = new bool[AlphabetSize];
        var inDamaged = new bool[AlphabetSize];

        foreach (var c in reference)
            inReference[c - 'A'] = true;

        foreach (var c in damaged)
            inDamaged[c - 'A'] = true;

        var missing = new StringBuilder();
        for (var i = 0; i < AlphabetSize; i++)
        {
            if (inReference[i] && !inDamaged[i])
                missing.Append((char)('A' + i));
        }

        return new RestoreResponse(missing.ToString(), missing.Length == 0);
    }
}
