using Treasury.Api.Dtos;

namespace Treasury.Api.Services;

public static class RestoreRequestValidator
{
    //Я хотел преобразовать входящий массив damaged в HashSet но по итогу не сказали у нас Damaged может 
    //ли быть с повтор символами(эталонный не повторяется а damaged содержит все символы эталонного, но по факту если damaged = ААА
    //а эталонный "ABC", то damaged всё равно подходит под условие)

    //Поэтому просто обозначил что Damaged если содержит повторы то выплёвываем как исключени/ошибка валидации

    // Возвращает ошибки по полям. Пустой словарь означает, что запрос корректен
    public static Dictionary<string, string[]> Validate(RestoreRequest request)
    {
        var errors = new Dictionary<string, string[]>();
        var referenceErrors = new List<string>();
        var damagedErrors = new List<string>(); //Списки для обработки бизнес логки. Если есть ошибки при вводе данных то выводим все ошибки
        var inReference = new bool[26];

        if (string.IsNullOrEmpty(request.Reference))
        {
            referenceErrors.Add("Поле reference обязательно.");
        }
        else
        {
            bool invalid = false, duplicate = false; //флаги
            foreach (var c in request.Reference)
            {
                if (c < 'A' || c > 'Z') { //смотрим по кодам
                    invalid = true;
                    continue;
                }
                if (inReference[c - 'A']) duplicate = true;
                inReference[c - 'A'] = true;
            }

            if (invalid) referenceErrors.Add("reference должен содержать только латинские буквы A-Z.");
            if (duplicate) referenceErrors.Add("reference не должен содержать повторяющихся символов.");
        }

        if (request.Damaged is null)
        {
            damagedErrors.Add("Поле damaged обязательно.");
        }
        else
        {
            var seen = new bool[26];
            bool invalid = false, duplicate = false, unknown = false;
            foreach (var c in request.Damaged)
            {
                if (c < 'A' || c > 'Z'){ 
                    invalid = true; continue;
                }
                if (seen[c - 'A']) duplicate = true;

                seen[c - 'A'] = true;

                if (!inReference[c - 'A']) unknown = true;
            }

            if (invalid) damagedErrors.Add("damaged должен содержать только латинские буквы A-Z.");
            if (duplicate) damagedErrors.Add("damaged не должен содержать повторяющихся символов.");
            if (unknown && referenceErrors.Count == 0)
                damagedErrors.Add("damaged содержит символ, которого нет в reference.");
        }

        if (referenceErrors.Count > 0) errors["reference"] = referenceErrors.ToArray();
        if (damagedErrors.Count > 0) errors["damaged"] = damagedErrors.ToArray();
        return errors;
    }
}