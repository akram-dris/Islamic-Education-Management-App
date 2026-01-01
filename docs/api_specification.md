# API Specification

## Quran & Islamic Education Management App (MVP)

**Base URL:** `/api`  
**Authentication:** JWT (Bearer Token)

All endpoints except login require:

```
Authorization: Bearer <token>
```

---

## 1. Authentication

### Login

**POST** `/auth/login`

**Body**

```json
{
  "username": "string",
  "password": "string"
}
```

**Response**

```json
{
  "token": "jwt",
  "user": {
    "id": "uuid",
    "fullName": "string",
    "role": "Admin | Teacher | Student | Parent"
  }
}
```

---

## 2. Users (Admin Only)

### Register User

**POST** `/users/register`

```json
{
  "username": "string",
  "password": "string",
  "fullName": "string",
  "role": "Admin | Teacher | Student | Parent"
}
```

---

### Get Users

**GET** `/users`

---

### Update User

**PUT** `/users/{userId}`

```json
{
  "fullName": "string",
  "password": "string (optional)"
}
```

---

### Delete User (Soft Delete)

**DELETE** `/users/{userId}`

---

## 3. Parent ↔ Student Relationship (Admin Only)

### Link Student to Parent

**POST** `/users/{parentId}/students`

```json
{
  "studentId": "uuid"
}
```

---

## 4. Academic Structure (Admin Only)

### Classes

**POST** `/classes`
**PUT** `/classes/{classId}`
**DELETE** `/classes/{classId}`

### Subjects

**POST** `/subjects`
**PUT** `/subjects/{subjectId}`
**DELETE** `/subjects/{subjectId}`

---

## 5. Teacher Allocation (Admin Only)

### Assign Teacher to Class & Subject

**POST** `/allocations`
**PUT** `/allocations/{allocationId}`
**DELETE** `/allocations/{allocationId}`

---

## 6. Student Enrollment (Admin Only)

### Enroll Student in Class

**POST** `/enrollments`
**DELETE** `/enrollments/{enrollmentId}`

---

## 7. Teacher API

### Get My Teaching Allocations

**GET** `/allocations` (Filtered by authenticated teacher)

---

### Assignments

**POST** `/assignments`
**PUT** `/assignments/{assignmentId}`
**DELETE** `/assignments/{assignmentId}`

---

### View Assignment Submissions

**GET** `/submissions/assignment/{assignmentId}`

---

### Grade Submission

**POST** `/submissions/{id}/grade`

```json
{
  "grade": 0,
  "remarks": "string (optional)"
}
```

---

### Record Attendance

**POST** `/attendance/mark`

```json
{
  "allocationId": "uuid",
  "date": "YYYY-MM-DD",
  "records": [
    {
      "studentId": "uuid",
      "status": "Present | Absent"
    }
  ]
}
```

**PUT** `/attendance/{sessionId}`
**DELETE** `/attendance/{sessionId}`

---

## 8. Student API

### Get My Assignments

**GET** `/assignments/my`

---

### Upload Assignment File

**POST** `/files/upload`  
`multipart/form-data`

---

### Submit Assignment

**POST** `/submissions`

```json
{
  "assignmentId": "uuid",
  "fileUrl": "string"
}
```

---

### Get My Attendance

**GET** `/attendance/my`

---

## 9. Parent API (Read-Only)

### Get My Children

**GET** `/parents/children`

---

### View Child Assignments

**GET** `/parents/children/{studentId}/assignments`

---

### View Child Attendance

**GET** `/parents/children/{studentId}/attendance`

---

## 10. Common Behavior

- **Soft Delete**: All entities use soft delete. `DELETE` requests mark `IsDeleted = true`.
- **Auditing**: All entities track `CreatedAt`, `CreatedBy`, `LastModifiedAt`, `LastModifiedBy`.
- **UUIDs**: All primary and foreign keys use UUIDs.