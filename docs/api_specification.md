 API Specification

## Quran & Islamic Education Management App (MVP)

**Base URL:** `/api/v1`  
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
    "role": "ADMIN | TEACHER | STUDENT | PARENT"
  }
}
```

---

## 2. Users (Admin Only)

Backed by: `users`

### Create User

**POST** `/users`

```json
{
  "username": "string",
  "password": "string",
  "fullName": "string",
  "role": "ADMIN | TEACHER | STUDENT | PARENT"
}
```

---

### Get Users

**GET** `/users?role=STUDENT|TEACHER|PARENT`

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

### Delete User

**DELETE** `/users/{userId}`

---

## 3. Parent ↔ Student Relationship (Admin Only)

Backed by: `parent_students`

### Link Student to Parent

**POST** `/parents/{parentId}/children`

```json
{
  "studentId": "uuid"
}
```

> One student can be linked to multiple parents.  
> Duplicate links are not allowed.

---

### Unlink Student from Parent

**DELETE** `/parents/{parentId}/children/{studentId}`

---

## 4. Academic Structure (Admin Only)

### Classes

Backed by: `classes`

**POST** `/classes`

```json
{
  "name": "string"
}
```

---

### Subjects

Backed by: `subjects`

**POST** `/subjects`

```json
{
  "name": "string"
}
```

---

## 5. Teacher Allocation (Admin Only)

Backed by: `allocations`

### Assign Teacher to Class & Subject

**POST** `/allocations`

```json
{
  "teacherId": "uuid",
  "classId": "uuid",
  "subjectId": "uuid"
}
```

> Combination of `(teacherId, classId, subjectId)` must be unique.

---

## 6. Student Enrollment (Admin Only)

Backed by: `enrollments`

### Enroll Student in Class

**POST** `/enrollments`

```json
{
  "studentId": "uuid",
  "classId": "uuid"
}
```

> A student may belong to multiple classes if needed.  
> Duplicate enrollments are not allowed.

---

## 7. Teacher API

Teacher access is restricted by `allocations`.

---

### Get My Teaching Allocations

**GET** `/teacher/my-classes`

**Response**

```json
[
  {
    "allocationId": "uuid",
    "classId": "uuid",
    "className": "string",
    "subjectId": "uuid",
    "subjectName": "string"
  }
]
```

---

### Create Assignment

Backed by: `assignments`

**POST** `/assignments`

```json
{
  "allocationId": "uuid",
  "title": "string",
  "description": "string",
  "dueDate": "YYYY-MM-DD"
}
```

---

### View Assignment Submissions

Backed by: `submissions`

**GET** `/assignments/{assignmentId}/submissions`

---

### Grade Submission

**POST** `/submissions/{submissionId}/grade`

```json
{
  "grade": 0,
  "remarks": "string (optional)"
}
```

> A submission can be graded only once.  
> Updating grade overwrites the previous value.

---

### Record Attendance

Backed by: `attendance_sessions`, `attendance_records`

**POST** `/attendance`

```json
{
  "allocationId": "uuid",
  "date": "YYYY-MM-DD",
  "records": [
    {
      "studentId": "uuid",
      "status": "PRESENT | ABSENT"
    }
  ]
}
```

Rules:

- Attendance is per **allocation + date**
    
- Duplicate sessions are rejected
    
- Each student appears once per session
    

---

## 8. Student API

Student access is limited to `student_id = authenticated user`.

---

### Get My Assignments

**GET** `/student/my-assignments`

Returns assignments across enrolled classes and allocations.

---

### Upload Assignment File

**POST** `/files/upload`  
`multipart/form-data`

**Response**

```json
{
  "fileUrl": "string"
}
```

---

### Submit Assignment

Backed by: `submissions`

**POST** `/submissions`

```json
{
  "assignmentId": "uuid",
  "fileUrl": "string"
}
```

> One submission per assignment per student.

---

### Get My Grades

**GET** `/student/my-grades`

---

### Get My Attendance

**GET** `/student/my-attendance`

---

## 9. Parent API (Read-Only)

Parent access is filtered via `parent_students`.

---

### Get My Children

**GET** `/parent/children`

---

### View Child Assignments

**GET** `/parent/children/{studentId}/assignments`

---

### View Child Attendance

**GET** `/parent/children/{studentId}/attendance`

---

## 10. Authorization Rules (Hard Constraints)

|Role|Permissions|
|---|---|
|Admin|Full system access|
|Teacher|Allocated classes only|
|Student|Own data only|
|Parent|Read-only linked children|

