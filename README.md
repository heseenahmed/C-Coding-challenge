# C-Coding-challenge
Due to the limited time available for the interview assignment, I focused on implementing the required functionality and clean code style rather than building a fully layered clean architecture.
The core business logic is already placed inside a Service layer, and the controller only handles HTTP input/output  keeping a clear separation of concerns.
In a real production environment, I would extend this structure into a full Clean Architecture setup, adding dedicated layers for Repositories, CQRS Command/Query Handlers, DTOs, and Validation, to make the solution more scalable and maintainable.
