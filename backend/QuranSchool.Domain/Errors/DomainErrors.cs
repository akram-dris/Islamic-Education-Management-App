using QuranSchool.Domain.Abstractions;

namespace QuranSchool.Domain.Errors;

public static class DomainErrors
{
    public static class User
    {
        public static readonly Error NotFound = Error.NotFound("User.NotFound", "User not found.");
        public static readonly Error DuplicateUsername = Error.Conflict("User.DuplicateUsername", "Username is already taken.");
        public static readonly Error AlreadyLinked = Error.Conflict("User.AlreadyLinked", "Student is already linked to this parent.");
        public static readonly Error ParentNotFound = Error.NotFound("User.ParentNotFound", "Parent not found.");
        public static readonly Error StudentNotFound = Error.NotFound("User.StudentNotFound", "Student not found.");
        public static readonly Error TeacherNotFound = Error.NotFound("User.TeacherNotFound", "Teacher not found.");
        public static readonly Error NotAuthorized = Error.Forbidden("User.NotAuthorized", "You are not authorized to perform this action.");
        public static readonly Error EmptyUsername = Error.Validation("User.EmptyUsername", "Username cannot be empty.");
        public static readonly Error EmptyPassword = Error.Validation("User.EmptyPassword", "Password cannot be empty.");
        public static readonly Error EmptyFullName = Error.Validation("User.EmptyFullName", "Full name cannot be empty.");
        public static readonly Error EmptyParentId = Error.Validation("User.EmptyParentId", "Parent ID cannot be empty.");
        public static readonly Error EmptyStudentIdForLink = Error.Validation("User.EmptyStudentIdForLink", "Student ID for linking cannot be empty.");
    }

    public static class Auth
    {
        public static readonly Error InvalidCredentials = Error.Unauthorized("Auth.InvalidCredentials", "Invalid username or password.");
    }

    public static class Class
    {
        public static readonly Error NotFound = Error.NotFound("Class.NotFound", "Class not found.");
        public static readonly Error DuplicateName = Error.Conflict("Class.DuplicateName", "A class with this name already exists.");
        public static readonly Error EmptyName = Error.Validation("Class.EmptyName", "Class name cannot be empty.");
    }

    public static class Subject
    {
        public static readonly Error NotFound = Error.NotFound("Subject.NotFound", "Subject not found.");
        public static readonly Error DuplicateName = Error.Conflict("Subject.DuplicateName", "A subject with this name already exists.");
        public static readonly Error EmptyName = Error.Validation("Subject.EmptyName", "Subject name cannot be empty.");
    }

    public static class Allocation
    {
        public static readonly Error NotFound = Error.NotFound("Allocation.NotFound", "Allocation not found.");
        public static readonly Error Duplicate = Error.Conflict("Allocation.Duplicate", "This teacher is already allocated to this class and subject.");
        public static readonly Error EmptyTeacherId = Error.Validation("Allocation.EmptyTeacherId", "Teacher ID cannot be empty.");
        public static readonly Error EmptyClassId = Error.Validation("Allocation.EmptyClassId", "Class ID cannot be empty.");
        public static readonly Error EmptySubjectId = Error.Validation("Allocation.EmptySubjectId", "Subject ID cannot be empty.");
    }

    public static class Enrollment
    {
        public static readonly Error NotFound = Error.NotFound("Enrollment.NotFound", "Enrollment not found.");
        public static readonly Error Duplicate = Error.Conflict("Enrollment.Duplicate", "Student is already enrolled in this class.");
        public static readonly Error EmptyStudentId = Error.Validation("Enrollment.EmptyStudentId", "Student ID cannot be empty.");
        public static readonly Error EmptyClassId = Error.Validation("Enrollment.EmptyClassId", "Class ID cannot be empty.");
    }

    public static class Assignment
    {
        public static readonly Error NotFound = Error.NotFound("Assignment.NotFound", "Assignment not found.");
        public static readonly Error PastDueDate = Error.Validation("Assignment.PastDueDate", "The submission deadline has passed.");
        public static readonly Error EmptyTitle = Error.Validation("Assignment.EmptyTitle", "Assignment title cannot be empty.");
        public static readonly Error EmptyAllocationId = Error.Validation("Assignment.EmptyAllocationId", "Allocation ID cannot be empty.");
    }

    public static class Submission
    {
        public static readonly Error NotFound = Error.NotFound("Submission.NotFound", "Submission not found.");
        public static readonly Error AlreadySubmitted = Error.Conflict("Submission.AlreadySubmitted", "You have already submitted this assignment.");
        public static readonly Error EmptyAssignmentId = Error.Validation("Submission.EmptyAssignmentId", "Assignment ID cannot be empty.");
        public static readonly Error EmptyStudentId = Error.Validation("Submission.EmptyStudentId", "Student ID cannot be empty.");
        public static readonly Error EmptyFileUrl = Error.Validation("Submission.EmptyFileUrl", "File URL cannot be empty.");
        public static readonly Error InvalidGrade = Error.Validation("Submission.InvalidGrade", "Grade must be between 0 and 100.");
    }

    public static class AttendanceSession
    {
        public static readonly Error NotFound = Error.NotFound("AttendanceSession.NotFound", "Attendance session not found.");
        public static readonly Error EmptyAllocationId = Error.Validation("AttendanceSession.EmptyAllocationId", "Allocation ID cannot be empty.");
        public static readonly Error InvalidDate = Error.Validation("AttendanceSession.InvalidDate", "Invalid session date.");
    }

    public static class AttendanceRecord
    {
        public static readonly Error NotFound = Error.NotFound("AttendanceRecord.NotFound", "Attendance record not found.");
        public static readonly Error EmptySessionId = Error.Validation("AttendanceRecord.EmptySessionId", "Attendance session ID cannot be empty.");
        public static readonly Error EmptyStudentId = Error.Validation("AttendanceRecord.EmptyStudentId", "Student ID cannot be empty.");
    }
}
