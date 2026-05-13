# akbars.web

Кроссплатформенная веб-версия service desk.

## Назначение

Проект сделан как веб-альтернатива старому WPF-клиенту, чтобы система открывалась:

- на Windows
- на macOS
- на Linux
- на мобильных устройствах

## Технологии

- ASP.NET Core MVC
- .NET 8
- PostgreSQL
- server-rendered UI

## Возможности

- вход по логину и паролю
- панели по ролям
- создание заявок сотрудником
- назначение исполнителя диспетчером
- смена статусов исполнителем
- управление ролями и справочниками администратором

## Быстрый запуск

1. Установить `.NET 8 SDK`
2. Поднять PostgreSQL
3. Проверить строку подключения в [appsettings.json](/Users/murat/projects/Study/di-med/akbars2/akbars.web/appsettings.json)
4. Выполнить:

```bash
dotnet restore
dotnet run
```

## Локальная БД через Docker

```bash
docker run -d \
  --name akbars-postgres-local \
  -e POSTGRES_DB=akbars \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=change-me \
  -p 5432:5432 \
  -v "$(pwd)/db/init.sql:/docker-entrypoint-initdb.d/init.sql:ro" \
  postgres:16
```

## Тестовые логины

- `admin` / `admin123`
- `dispatcher` / `dispatcher123`
- `executor` / `executor123`
- `employee` / `employee123`

## Важные файлы

- [Program.cs](/Users/murat/projects/Study/di-med/akbars2/akbars.web/Program.cs)
- [appsettings.json](/Users/murat/projects/Study/di-med/akbars2/akbars.web/appsettings.json)
- [db/init.sql](/Users/murat/projects/Study/di-med/akbars2/akbars.web/db/init.sql)
- [Controllers](/Users/murat/projects/Study/di-med/akbars2/akbars.web/Controllers)
- [wwwroot/css/site.css](/Users/murat/projects/Study/di-med/akbars2/akbars.web/wwwroot/css/site.css)

## Открытие

- локально: `http://localhost:5099`
- с телефона в одной сети: `http://<ip_компьютера>:5099`
