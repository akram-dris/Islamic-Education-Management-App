# 📚 Project Documentation

This folder contains the comprehensive documentation for the Education Management Assahaba. It serves as the single source of truth for requirements, architecture, and specifications.

## 📁 Document Index

| Document | Description |
| :--- | :--- |
| [**Business Requirements**](business_requirements.md) | High-level goals, scope, and functional requirements. |
| [**User Stories**](user_stories.md) | Persona-based requirements for Admin, Teacher, Student, and Parent. |
| [**Technical Architecture**](technical_architecture.md) | System-wide technology stack and high-level data flow. |
| [**Backend Architecture**](backend_architecture.md) | Deep dive into Clean Architecture and .NET implementation. |
| [**Database Schema**](database_schema.md) | DBML definition and relational structure. |
| [**API Specification**](api_specification.md) | REST API endpoints, request/response models, and auth rules. |

## 🛠️ Tools Used
*   **DBML**: For database modeling.
*   **Mermaid**: For architectural diagrams.
*   **Markdown**: For documentation formatting.

## 🔄 Maintaining Docs
Whenever a significant change is made to the codebase (e.g., adding a table, changing an API route):
1.  Update the corresponding Markdown file.
2.  Ensure consistency across the `database_schema.md` and `api_specification.md`.
3.  Reflect architectural changes in the `backend_architecture.md`.
