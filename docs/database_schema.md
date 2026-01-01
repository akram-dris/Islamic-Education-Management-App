## Database Schema (DBML)

### Quran & Islamic Education Management App (MVP)

```dbml
Project quran_school {
  database_type: "PostgreSQL"
  Note: "MVP schema based on BRD, User Stories, and API specification"
}

///////////////////////
// ENUMS
///////////////////////

Enum user_role {
  ADMIN
  TEACHER
  STUDENT
  PARENT
}

Enum attendance_status {
  PRESENT
  ABSENT
}

///////////////////////
// COMMON METADATA (Auditing & Soft Delete)
///////////////////////
// All tables include:
// created_at timestamp [not null]
// created_by uuid
// last_modified_at timestamp
// last_modified_by uuid
// is_deleted boolean [not null, default: false]

///////////////////////
// USERS & RELATIONSHIPS
///////////////////////

Table users {
  id uuid [pk]
  username varchar [not null]
  password_hash varchar [not null]
  full_name varchar [not null]
  role user_role [not null]
  
  // Metadata
  created_at timestamp [not null]
  created_by uuid
  last_modified_at timestamp
  last_modified_by uuid
  is_deleted boolean [not null, default: false]
}

Table parent_students {
  id uuid [not null]
  parent_id uuid [pk, not null]
  student_id uuid [pk, not null]

  // Metadata
  created_at timestamp [not null]
  created_by uuid
  last_modified_at timestamp
  last_modified_by uuid
  is_deleted boolean [not null, default: false]
}

Ref: parent_students.parent_id > users.id [delete: restrict]
Ref: parent_students.student_id > users.id [delete: cascade]

///////////////////////
// ACADEMIC STRUCTURE
///////////////////////

Table classes {
  id uuid [pk]
  name varchar [not null]

  // Metadata
  created_at timestamp [not null]
  created_by uuid
  last_modified_at timestamp
  last_modified_by uuid
  is_deleted boolean [not null, default: false]
}

Table subjects {
  id uuid [pk]
  name varchar [not null]

  // Metadata
  created_at timestamp [not null]
  created_by uuid
  last_modified_at timestamp
  last_modified_by uuid
  is_deleted boolean [not null, default: false]
}

Table allocations {
  id uuid [pk]
  teacher_id uuid [not null]
  class_id uuid [not null]
  subject_id uuid [not null]

  indexes {
    (teacher_id, class_id, subject_id) [unique]
  }

  // Metadata
  created_at timestamp [not null]
  created_by uuid
  last_modified_at timestamp
  last_modified_by uuid
  is_deleted boolean [not null, default: false]
}

Ref: allocations.teacher_id > users.id
Ref: allocations.class_id > classes.id
Ref: allocations.subject_id > subjects.id

///////////////////////
// STUDENT ENROLLMENT
///////////////////////

Table enrollments {
  id uuid [pk]
  student_id uuid [not null]
  class_id uuid [not null]

  indexes {
    (student_id, class_id) [unique]
  }

  // Metadata
  created_at timestamp [not null]
  created_by uuid
  last_modified_at timestamp
  last_modified_by uuid
  is_deleted boolean [not null, default: false]
}

Ref: enrollments.student_id > users.id
Ref: enrollments.class_id > classes.id

///////////////////////
// ASSIGNMENTS & SUBMISSIONS
///////////////////////

Table assignments {
  id uuid [pk]
  allocation_id uuid [not null]
  title varchar [not null]
  description text
  due_date date [not null]

  // Metadata
  created_at timestamp [not null]
  created_by uuid
  last_modified_at timestamp
  last_modified_by uuid
  is_deleted boolean [not null, default: false]
}

Ref: assignments.allocation_id > allocations.id

Table submissions {
  id uuid [pk]
  assignment_id uuid [not null]
  student_id uuid [not null]
  file_url varchar [not null]
  grade numeric
  remarks text

  indexes {
    (assignment_id, student_id) [unique]
  }

  // Metadata
  created_at timestamp [not null]
  created_by uuid
  last_modified_at timestamp
  last_modified_by uuid
  is_deleted boolean [not null, default: false]
}

Ref: submissions.assignment_id > assignments.id
Ref: submissions.student_id > users.id

///////////////////////
// ATTENDANCE
///////////////////////

Table attendance_sessions {
  id uuid [pk]
  allocation_id uuid [not null]
  session_date date [not null]

  indexes {
    (allocation_id, session_date) [unique]
  }

  // Metadata
  created_at timestamp [not null]
  created_by uuid
  last_modified_at timestamp
  last_modified_by uuid
  is_deleted boolean [not null, default: false]
}

Ref: attendance_sessions.allocation_id > allocations.id

Table attendance_records {
  id uuid [pk]
  attendance_session_id uuid [not null]
  student_id uuid [not null]
  status attendance_status [not null]

  indexes {
    (attendance_session_id, student_id) [unique]
  }

  // Metadata
  created_at timestamp [not null]
  created_by uuid
  last_modified_at timestamp
  last_modified_by uuid
  is_deleted boolean [not null, default: false]
}

Ref: attendance_records.attendance_session_id > attendance_sessions.id
Ref: attendance_records.student_id > users.id
```