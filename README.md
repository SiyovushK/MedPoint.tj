🏥 Clinic Management System
Clinic Management System — это современное веб-приложение для управления медицинской клиникой, разработанное на ASP.NET Core с использованием Clean Architecture. Проект автоматизирует ключевые процессы: регистрацию пациентов и врачей, бронирование приемов, подтверждение электронной почты, чат в реальном времени со службой поддержки, управление расписанием врачей и обработку заказов.

Система решает проблему ручного ведения записей и делает процесс взаимодействия между врачами и пациентами более удобным, быстрым и безопасным.

⚙️ Функциональность
🔐 Регистрация и логин с подтверждением по email

👥 Разделение по ролям: Admin, Doctor, User

📅 Бронирование приёмов с проверкой доступности

📆 Расписание врачей и автоматическое завершение встреч

📬 Отправка email-уведомлений (SMTP)

💬 Онлайн-чат между клиентом и поддержкой (SignalR)

📊 Админ-панель со статистикой

🔁 Hangfire — фоновые задачи: напоминания, авто-завершения

🧾 Отзывы пациентов, управление заказами

🖼️ Загрузка и отображение фото профилей

✅ Обработка ошибок, валидация, логгирование

🛠️ Технологический стек
Backend: ASP.NET Core 8, C#, REST API

Architecture: Clean Architecture, SOLID

Database: PostgreSQL, Entity Framework Core, Fluent API

Auth: Identity, JWT Bearer, роли

Mapping: AutoMapper

Validation: FluentValidation

Real-Time: SignalR

Background Jobs: Hangfire

Email: SMTP (Gmail)

Docs: Swagger

Response Wrapper: Response<T>, PagedResponse<T>
