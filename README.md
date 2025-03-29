## API для управления событиями

### Общая информация
Этот API предназначен для управления событиями (Event) в системе ставок. Он позволяет создавать, изменять, завершать и удалять события, а также управлять подписчиками и деталями событий.

### Базовый URL
```
http://localhost:5283/api/events
```

## Методы API Событий

### Получение списка всех событий
**GET** `/all`

Возвращает список всех событий в системе.

Пример запроса:
```http
GET /api/events/all
```
Пример ответа:
```json
[
    {
        "id": "12345",
        "eventName": "Футбольный матч",
        "type": "Football",
        "homeTeam": "Команда А",
        "awayTeam": "Команда Б",
        "eventStartDate": "2025-03-11T18:00:00Z",
        "eventEndDate": "2025-03-11T20:00:00Z",
        "eventSubscribers": []
    }
]
```

### Получение информации о конкретном событии
**GET** `/{id}`

Позволяет получить информацию о событии по его `id`.

Пример запроса:
```http
GET /api/events/12345
```
Пример ответа:
```json
{
    "id": "12345",
    "eventName": "Футбольный матч",
    "type": "Football",
    "homeTeam": "Команда А",
    "awayTeam": "Команда Б",
    "eventStartDate": "2025-03-11T18:00:00Z",
    "eventEndDate": "2025-03-11T20:00:00Z",
    "eventSubscribers": []
}
```

### Создание нового события
**POST** `/create`

Создает новое событие.

Пример запроса:
```http
POST /api/events/create
Content-Type: application/json

{
    "eventName": "Футбольный матч",
    "type": "Football",
    "homeTeam": "Команда А",
    "awayTeam": "Команда Б",
    "eventStartDate": "2025-03-11T18:00:00Z",
    "eventEndDate": "2025-03-11T20:00:00Z"
}
```

Пример ответа:
```json
{
    "id": "67890",
    "eventName": "Футбольный матч",
    "type": "Football",
    "homeTeam": "Команда А",
    "awayTeam": "Команда Б",
    "eventStartDate": "2025-03-11T18:00:00Z",
    "eventEndDate": "2025-03-11T20:00:00Z",
    "eventSubscribers": []
}
```

### Завершение события
**POST** `/{id}/end`

Завершает событие и определяет его результат.

Пример запроса:
```http
POST /api/events/12345/end
```
Пример ответа:
```json
{
    "id": "12345",
    "eventName": "Футбольный матч",
    "homeScore": 2,
    "awayScore": 3,
    "result": "AwayWin"
}
```

### Отмена события
**POST** `/{id}/cancel`

Отменяет событие.

Пример запроса:
```http
POST /api/events/12345/cancel
```
Пример ответа:
```json
"Event with ID 12345 has been canceled."
```

### Подписка на событие
**POST** `/{id}/subscribe`

Добавляет подписчика к событию.

Пример запроса:
```http
POST /api/events/12345/subscribe
Content-Type: application/json

{
    "callbackUrl": "http://example.com/notify"
}
```
Пример ответа:
```json
"Successfully subscribed to event 12345."
```

### Удаление события
**DELETE** `/{id}/delete`

Удаляет событие и связанные с ним детали.

Пример запроса:
```http
DELETE /api/events/12345/delete
```
Ответ: `204 No Content`

### Получение деталей события
**GET** `/{id}/details`

Возвращает информацию о раундах события.

Пример запроса:
```http
GET /api/events/12345/details
```
Пример ответа:
```json
{
    "id": "12345",
    "eventName": "Футбольный матч",
    "eventRounds": [
        {
            "roundNumber": 1,
            "homeTeamScore": 1,
            "awayTeamScore": 0,
            "roundDateTime": "2025-03-11T18:15:00Z"
        }
    ]
}
```

### Обновление деталей события
**PATCH** `/{id}/details`

Позволяет обновить детали конкретного раунда события.

Пример запроса:
```http
PATCH /api/events/12345/details
Content-Type: application/json

{
    "roundNumber": 1,
    "homeTeamScore": 2,
    "awayTeamScore": 1
}
```
Пример ответа:
```json
{
    "roundNumber": 1,
    "homeTeamScore": 2,
    "awayTeamScore": 1,
    "roundDateTime": "2025-03-11T18:15:00Z"
}
```

### Добавление нового раунда в событие
**POST** `/{id}/details/create`

Позволяет добавить новый раунд в событие.

Пример запроса:
```http
POST /api/events/12345/details/create
Content-Type: application/json

{
    "roundNumber": 2,
    "homeTeamScore": 0,
    "awayTeamScore": 1
}
```
Пример ответа:
```json
{
    "id": "67890",
    "eventId": "12345",
    "roundNumber": 2,
    "homeTeamScore": 0,
    "awayTeamScore": 1,
    "roundDateTime": "2025-03-11T19:00:00Z"
}
```

## API для управления платежными картами

### Общая информация
Этот API предназначен для управления платежными картами пользователей. Он позволяет регистрировать новые карты, проводить платежи, пополнять баланс и получать информацию о карте.

### Базовый URL
```
http://localhost:5283/api/payment
```

## Методы API Платежей

### Регистрация новой карты
**POST** `/registerCard`

Регистрирует новую платежную карту для пользователя. Генерирует уникальный номер карты и CVV. Начальный баланс (`availableAmount`) можно указать, по умолчанию он равен 0.

Пример запроса:
```http
POST /api/payment/registerCard
Content-Type: application/json

{
    "cardOwnerName": "Иван Петров",
    "availableAmount": 5000.00
}
```
Пример ответа (успешная регистрация):
```json
{
    "cardOwnerName": "Иван Петров",
    "availableAmount": 5000.00,
    "cardNumber": "4815162342881995", // Сгенерированный номер
    "cvv": "789" // Сгенерированный CVV
}
```
Возможные ошибки:
*   `400 Bad Request`: Если данные в запросе некорректны (например, отсутствует `cardOwnerName`).

### Проведение платежа с карты
**POST** `/pay`

Осуществляет списание средств с указанной карты. Требует полные данные карты для верификации.

Пример запроса:
```http
POST /api/payment/pay
Content-Type: application/json

{
    "cardNumber": "4815162342881995",
    "cardOwnerName": "Иван Петров",
    "cvv": "789",
    "paymentAmount": 150.75
}
```
Пример ответа (успешный платеж):
```json
{
    "message": "Payment Successful",
    "availableAmount": 4849.25, // Оставшийся баланс
    "owner": "Иван Петров"
}
```
Возможные ошибки:
*   `400 Bad Request`: Некорректные данные запроса или недостаточно средств (`"Not enough money"`).
*   `404 Not Found`: Карта с указанными данными не найдена (`"Invalid Credentials"`).

### Пополнение баланса карты
**POST** `/addMoney`

Добавляет указанную сумму на баланс карты. Для идентификации карты используется только ее номер.

Пример запроса:
```http
POST /api/payment/addMoney
Content-Type: application/json

{
    "cardNumber": "4815162342881995",
    "paymentAmount": 1000.00
}
```
Пример ответа (успешное пополнение):
```json
{
    "message": "Money Added Successfully",
    "owner": "Иван Петров"
}
```
Возможные ошибки:
*   `400 Bad Request`: Некорректные данные запроса.
*   `404 Not Found`: Карта с указанным номером не найдена (`"Invalid Credentials"`).

### Получение информации о карте
**GET** `/info`

Возвращает информацию о владельце карты и доступном балансе. Требует полные данные карты для верификации. **Примечание:** Этот метод использует `GET`, но ожидает данные в теле запроса (`[FromBody]`).

Пример запроса:
```http
GET /api/payment/info
Content-Type: application/json

{
    "cardNumber": "4815162342881995",
    "cvv": "789",
    "cardOwnerName": "Иван Петров"
}
```
Пример ответа (успешное получение информации):
```json
{
    "cardOwnerName": "Иван Петров",
    "availableAmount": 5849.25 // Текущий баланс после пополнения
}
```
Возможные ошибки:
*   `400 Bad Request`: Некорректные данные запроса.
*   `404 Not Found`: Карта с указанными данными не найдена (`"Invalid Credentials"`).

## Заключение
Эти API предоставляют полный набор методов для управления событиями ставок и платежными картами пользователей. Используйте их для эффективного администрирования и функционирования системы ставок.