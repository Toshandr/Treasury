# Treasury API

ASP.NET Core Web API (.NET 10) для определения отсутствующих символов в повреждённом коде созвездия.

## Структура

```
Treasury/
  src/Treasury.Api/      API (Minimal API, DTO, сервис, валидатор)
  tests/Treasury.Tests/  unit- и интеграционные тесты
  k8s/                   манифесты Kubernetes
  Dockerfile
```

## Запуск

Требуется .NET 10 SDK. Для остальных способов: Docker и kubectl.

Локально:

```
dotnet run --project src/Treasury.Api
```

Swagger: http://localhost:5000/swagger (порт смотрите в логе запуска)

Тесты:

```
dotnet test
```

Docker:

```
docker build -t treasury-api:latest .
docker run --rm -p 8080:8080 treasury-api:latest
```

Swagger: http://localhost:8080/swagger

Kubernetes (образ должен быть доступен кластеру; для minikube: `minikube image load treasury-api:latest`, для kind: `kind load docker-image treasury-api:latest`):

```
kubectl apply -f k8s/
kubectl port-forward svc/treasury-api 8080:80
```

Swagger: http://localhost:8080/swagger

## Эндпоинт

`POST /api/constellations/restore`

Запрос:

```json
{ "reference": "DRAGON", "damaged": "GOD" }
```

Ответ 200:

```json
{ "missingSymbols": "ANR", "isComplete": false }
```

Если символы не потеряны:

```json
{ "missingSymbols": "", "isComplete": true }
```

Некорректные данные: 400 Bad Request.

## Валидация

- `reference` обязателен, не пустой, только A-Z, без повторов.
- `damaged` обязателен (может быть пустой строкой), только A-Z, без повторов, все символы есть в `reference`.

## Алгоритм

Два массива флагов на 26 букв. Один проход по `reference`, один по `damaged`, затем проход по алфавиту A-Z: буква есть в эталоне, но нет в повреждённом коде, значит она отсутствует. Обход по алфавиту сразу даёт отсортированный результат, отдельная сортировка не нужна. Сложность O(n + m), это минимум, так как нужно прочитать вход. Дополнительная память O(1). `Contains()` и `IndexOf()` не используются.
