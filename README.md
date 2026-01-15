This project is a practical example of asynchronous messaging using RabbitMQ, implementing the Producer–Consumer pattern for user-related events.

The core idea is straightforward and architecturally sound:

when a user is created, an event is published by the Producer and processed asynchronously by a Consumer.

No tight coupling between services. No unnecessary synchronous calls. This is messaging done right.
