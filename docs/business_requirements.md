# Business Requirements Document (BRD)

## Quran & Islamic Education Management App (MVP)

### 1. Product Overview

The goal is to develop a structured academic management system using Flutter for mobile, specifically tailored for teaching Quran, Arabic Language, and Islamic Education. The system aims to replace manual paperwork with a digital workflow for assignments, grading, and attendance.

### 2. Primary Goals

- **Centralization**: Consolidate student data, grades, and attendance in one system.
- **Digitalization**: Move homework submission and grading to a digital format (text/image/PDF).
- **Transparency**: Provide parents with passive read-only access to their child's progress.
- **Efficiency**: Reduce administrative burden on teachers and noise for parents (no chat/notifications).

### 3. Target Audience

- **Admins**: Managing the school structure (users, classes, subjects).
- **Teachers**: Managing academic activities (assignments, grades, attendance).
- **Students**: Consuming content and submitting work.
- **Parents**: Monitoring child performance.

### 4. Scope & Features

#### In-Scope (MVP)

- **Authentication**: Role-based login for Admin, Teacher, Student, Parent.
- **Academic Management**:
  - Class and Subject creation.
  - Teacher assignment to specific Classes and Subjects.
  - Student-Parent linking.
- **Workflow**:
  - Teacher publishes Assignments.
  - Student submits work (Text, Image, PDF).
  - Teacher grades and provides optional remarks.
- **Attendance**:
  - Teacher records session attendance.
  - Read-only view for Students/Parents.
- **Data Views**:
  - Admin view of all system data.
  - Teacher view of assigned classes/subjects.
  - Student/Parent view of own records.

#### Out-of-Scope (V1)

- Real-time messaging or chat.
- Push notifications.
- Audio recording/submission/evaluation.
- Quran memorization specific tracking (Ayah/Surah logging).
- Online exams/Quizzes.
- Financial/Tuition management.
- Web interface (Mobile only).

### 5. Success Metrics

- Successful creation of all user roles by Admin.
- End-to-end completion of an assignment lifecycle (Create -> Submit -> Grade).
- Accurate attendance records viewable by parents.
