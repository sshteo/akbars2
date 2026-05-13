# Akbars2 Schema Notes

## Expected tables
- `users`: `id`, `last_name`, `first_name`, `middle_name`, `email`, `phone`, `department`, `login`, `password_hash`, `role_id`
- `roles`: `id`, `name`, `description`
- `tickets`: `id`, `created_at`, `updated_at`, `short_description`, `detailed_description`, `priority_id`, `type_id`, `status_id`, `author_id`
- `priorities`: `id`, `name`, `sla_hours`
- `statuses`: `id`, `name`, `description`
- `ticket_types`: `id`, `name`

## Optional compatibility columns
- `tickets.assignee_id` or `tickets.executor_id`
- `tickets.completed_at`
- `ticket_history` or `ticket_histories` with columns:
  - `id`, `ticket_id`, `changed_at`, `changed_by`, `field_name`, `old_value`, `new_value`

The application now detects these optional fields at runtime and adapts ticket assignment/history behavior.

## Seed expectations
- Roles:
  - `1` — Сотрудник
  - `2` — Исполнитель
  - `3` — Диспетчер
  - `4` — Администратор
- Statuses should contain names that map to:
  - new
  - in progress
  - completed
  - cancelled

Russian names such as `Новая`, `В работе`, `Выполнена`, `Отменена` are supported.

## Password migration
- Legacy plain-text values in `users.password_hash` are still accepted once.
- After a successful login, the app rewrites the stored value into PBKDF2 format:
  - `pbkdf2$iterations$salt$hash`

## Operations added by the upgraded app
- employee creates ticket
- dispatcher assigns/reassigns executor
- executor updates status
- admin changes user roles
- admin adds priorities, statuses, and ticket types
