Implemented CRUD operations using Entity Framework Core (not Dapper).

Used dependency injection and repository-style separation for maintainability.

Added pagination logic in the FetchUsersAsync method to handle large datasets efficiently.

All operations are async and handle cancellation tokens properly.

Note:
For the sake of the interview and limited time, I focused on the backend functionality and core logic.
In a production environment, I would expand this solution using Clean Architecture (with separate layers for Repositories, Commands/Queries, DTOs, and Validation).
