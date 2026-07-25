# Biblioteka PPP

Biblioteka PPP is a C#/ASP.NET Core MVC web application for library business process automation, developed during the third year of vocational studies at ITS, as part of the Applied Programming Practicum course, from requirements analysis and data modeling through implementation.

The application is designed for three main user groups:

- Public visitors, who can search the library catalog and open detailed material pages.
- Library members, who can register, log in, view their personal data, memberships, and loans, and rate materials they have already borrowed and returned.
- Librarians, who manage member records, memberships, loans, returns, and member accounts.

## Key Features

- Public catalog search with filters for title, author, UDK classification, library branch, publisher, publication place/year, and loan availability.
- Detailed material pages with cover images, authors, descriptions, publishing data, ISBN, UDK, member rating controls, and librarian loan actions.
- Member registration and login, with validation that each registered library member can have only one platform account.
- Member-facing views for personal data, membership periods, and loans grouped by selected membership.
- Librarian workflows for member lookup by unique member number (JCB), new member enrollment, membership opening, loan creation, loan return, and member account removal.
- Business-rule validation for memberships and loans, including active membership checks, overdue-loan checks, a maximum of 10 active loans, duplicate active-loan prevention, and automatic copy status updates.
- AJAX-enhanced UI interactions for rating updates and modal-based librarian workflows such as membership checks, loan creation, and loan returns.

## Tech Stack

- Backend: C#, ASP.NET Core MVC (.NET 8)
- Data layer: Microsoft SQL Server, Entity Framework Core 9, repository classes, and DTO-like business objects
- Frontend: Razor views, HTML5, CSS3, Bootstrap 5, JavaScript, jQuery, AJAX
- Tooling: Visual Studio/.NET CLI, Git/GitHub, SQL Server Management Studio

## Architecture Notes

The implementation follows a layered MVC structure. Controllers handle request flow and role-specific screens, repositories isolate database access and business rules, and business objects shape EF entities for Razor views. Protected member and librarian workflows are guarded by custom authorization filters.

The database model covers authors, materials, editions, branches, physical copies, members, memberships, loans, user accounts, librarian accounts, and read-material ratings. `Database/Biblioteka.sql` contains the SQL Server database dump with schema, relationships, constraints, and sample data. `Database/unosPodataka.sql` provides reset/sample-data logic, including helper database logic for copy inventory numbers and loan status updates.
