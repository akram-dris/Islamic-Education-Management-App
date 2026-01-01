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
    }

    public static class Auth
    {
        public static readonly Error InvalidCredentials = Error.Unauthorized("Auth.InvalidCredentials", "Invalid username or password.");
    }

    public static class Class
    {
        public static readonly Error NotFound = Error.NotFound("Class.NotFound", "Class not found.");
        public static readonly Error DuplicateName = Error.Conflict("Class.DuplicateName", "A class with this name already exists.");
    }

    public static class Subject
    {
        public static readonly Error NotFound = Error.NotFound("Subject.NotFound", "Subject not found.");
        public static readonly Error DuplicateName = Error.Conflict("Subject.DuplicateName", "A subject with this name already exists.");
    }

    public static class Allocation
    {
        public static readonly Error NotFound = Error.NotFound("Allocation.NotFound", "Allocation not found.");
        public static readonly Error Duplicate = Error.Conflict("Allocation.Duplicate", "This teacher is already allocated to this class and subject.");
    }

    public static class Enrollment
    {
        public static readonly Error NotFound = Error.NotFound("Enrollment.NotFound", "Enrollment not found.");
        public static readonly Error Duplicate = Error.Conflict("Enrollment.Duplicate", "Student is already enrolled in this class.");
    }

    public static class Assignment
    {
        public static readonly Error NotFound = Error.NotFound("Assignment.NotFound", "Assignment not found.");
        public static readonly Error PastDueDate = Error.Validation("Assignment.PastDueDate", "The submission deadline has passed.");
    }

    public static class Submission
    {
        public static readonly Error NotFound = Error.NotFound("Submission.NotFound", "Submission not found.");
        public static readonly Error AlreadySubmitted = Error.Conflict("Submission.AlreadySubmitted", "You have already submitted this assignment.");
    }
}
