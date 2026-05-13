# akbars2

Сервис-деск приложение для Ак Барс Мед.

В репозитории сейчас две версии клиента:

- `akbars/` — исходное WPF-приложение под Windows
- `akbars.web/` — новая кроссплатформенная веб-версия на ASP.NET Core MVC

Веб-версия открывается на Windows, macOS, Linux и мобильных устройствах через браузер.

## Состав репозитория

```text
akbars.sln          решение со старым desktop-проектом
akbars/             WPF-приложение (.NET Framework 4.8.1)
akbars.web/         ASP.NET Core MVC веб-приложение (.NET 8)
docs/               заметки по ожидаемой схеме БД
```

## Что реализовано в веб-версии

- cookie-аутентификация
- ролевой вход
- панель сотрудника
- панель исполнителя
- панель диспетчера
- административный контур
- работа с PostgreSQL
- адаптивная верстка для мобильных устройств

## Роли

Система ожидает такие роли:

- `1` — Сотрудник
- `2` — Исполнитель
- `3` — Диспетчер
- `4` — Администратор

## Тестовые учетные записи

После инициализации локальной БД доступны:

- `admin` / `admin123`
- `dispatcher` / `dispatcher123`
- `executor` / `executor123`
- `employee` / `employee123`

## Требования

Для веб-версии:

- .NET 8 SDK
- PostgreSQL 16+ или Docker

Для старого WPF-клиента:

- Windows
- .NET Framework 4.8.1
- Visual Studio с поддержкой WPF

## Быстрый запуск веб-версии

### Вариант 1. Через Docker

Поднять PostgreSQL:

```bash
docker run -d \
  --name akbars-postgres-local \
  -e POSTGRES_DB=akbars \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=change-me \
  -p 5432:5432 \
  -v "$(pwd)/akbars.web/db/init.sql:/docker-entrypoint-initdb.d/init.sql:ro" \
  postgres:16
```

Запустить веб-приложение:

```bash
cd akbars.web
dotnet restore
dotnet run
```

После запуска приложение доступно по адресу:

- `http://localhost:5099`

Для телефона в той же сети:

- `http://<ip_компьютера>:5099`

### Вариант 2. С локальным PostgreSQL

Если PostgreSQL уже установлен локально, в `akbars.web/appsettings.json` должна быть корректная строка подключения:

```json
{
  "ConnectionStrings": {
    "PostgresConnection": "Host=localhost;Port=5432;Database=akbars;Username=postgres;Password=change-me"
  }
}
```

Нужно:

1. создать базу `akbars`
2. применить SQL из `akbars.web/db/init.sql`
3. запустить `dotnet run` из папки `akbars.web`

## Инициализация базы

SQL лежит в:

- [akbars.web/db/init.sql](/Users/murat/projects/Study/di-med/akbars2/akbars.web/db/init.sql)

Он создает:

- `roles`
- `users`
- `priorities`
- `statuses`
- `ticket_types`
- `tickets`
- `ticket_history`

И заполняет:

- роли
- справочники
- тестовых пользователей
- несколько тестовых заявок

## Ожидаемая схема

Краткие заметки по схеме лежат в:

- [docs/schema-notes.md](/Users/murat/projects/Study/di-med/akbars2/docs/schema-notes.md)

## Запуск старой desktop-версии

Старый WPF-клиент остался в `akbars/`.

Он:

- работает только на Windows
- использует `App.config`
- подключается к той же PostgreSQL базе

Веб-версия добавлена рядом и не заменяет старый проект насильно.

## Основные файлы веб-версии

- [akbars.web/Program.cs](/Users/murat/projects/Study/di-med/akbars2/akbars.web/Program.cs)
- [akbars.web/appsettings.json](/Users/murat/projects/Study/di-med/akbars2/akbars.web/appsettings.json)
- [akbars.web/db/init.sql](/Users/murat/projects/Study/di-med/akbars2/akbars.web/db/init.sql)
- [akbars.web/Controllers/TicketsController.cs](/Users/murat/projects/Study/di-med/akbars2/akbars.web/Controllers/TicketsController.cs)
- [akbars.web/Controllers/AdminController.cs](/Users/murat/projects/Study/di-med/akbars2/akbars.web/Controllers/AdminController.cs)
- [akbars.web/wwwroot/css/site.css](/Users/murat/projects/Study/di-med/akbars2/akbars.web/wwwroot/css/site.css)

## Что уже проверено

- веб-приложение собирается на `.NET 8`
- сервер поднимается на `localhost:5099`
- вход под `admin / admin123` проходит успешно
- приложение подключается к локальной PostgreSQL в Docker

## Типовые проблемы

### Не удается подключиться к базе

Проверь:

- запущен ли PostgreSQL
- слушает ли он `localhost:5432`
- существует ли база `akbars`
- совпадают ли `Username` и `Password` в `appsettings.json`

### Не проходит логин

Проверь:

- есть ли пользователь в таблице `users`
- совпадает ли поле `login`
- корректен ли `password_hash`

Приложение поддерживает legacy plain-text пароль один раз, после успешного входа он переписывается в PBKDF2.

### Веб-приложение не стартует

Проверь:

- установлен ли `.NET 8 SDK`
- выполняется ли запуск из папки `akbars.web`
- не занят ли порт `5099`

## Статус проекта

Веб-контур уже пригоден для локального использования и демонстрации. Старый WPF-клиент сохранен как legacy-ветка интерфейса.
