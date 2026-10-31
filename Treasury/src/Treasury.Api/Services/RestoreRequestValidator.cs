using Treasury.Api.Dtos;

namespace Treasury.Api.Services;

public static class RestoreRequestValidator
{
    // Возвращает текст ошибки или null, если запрос корректен
    public static string? Validate(RestoreRequest request)
    {
        if (string.IsNullOrEmpty(request.Reference))
            return "Поле reference обязательно.";

        if (request.Damaged is null)
            return "Поле damaged обязательно.";

        var inReference = new bool[26];
        foreach (var c in request.Reference)
        {
            if (c < 'A' || c > 'Z')
                return "reference должен содержать только латинские буквы A-Z.";
            if (inReference[c - 'A'])
                return "reference не должен содержать повторяющихся символов.";
            inReference[c - 'A'] = true;
        }

        var seen = new bool[26];
        foreach (var c in request.Damaged)
        {
            if (c < 'A' || c > 'Z')
                return "damaged должен содержать только латинские буквы A-Z.";
            if (!inReference[c - 'A'])
                return "damaged содержит символ, которого нет в reference.";
            if (seen[c - 'A'])
                return "damaged не должен содержать повторяющихся символов.";
            seen[c - 'A'] = true;
        }

        return null;
    }
}
