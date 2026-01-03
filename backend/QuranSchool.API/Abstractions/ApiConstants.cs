namespace QuranSchool.API.Abstractions;

public static class ApiRoutes
{
    private const string Base = "api";

    public static class Auth
    {
        public const string BaseRoute = $"{Base}/auth";
        public const string Login = "login";
    }

    public static class Users
    {
        public const string BaseRoute = $"{Base}/users";
        public const string Register = "register";
        public const string LinkStudent = "{parentId}/students";
    }

    public static class Classes
    {
        public const string BaseRoute = $"{Base}/classes";
    }

    public static class Subjects
    {
        public const string BaseRoute = $"{Base}/subjects";
    }

    public static class Allocations
    {
        public const string BaseRoute = $"{Base}/allocations";
    }

    public static class Enrollments
    {
        public const string BaseRoute = $"{Base}/enrollments";
    }

    public static class Assignments
    {
        public const string BaseRoute = $"{Base}/assignments";
        public const string My = "my";
    }

    public static class Submissions
    {
        public const string BaseRoute = $"{Base}/submissions";
        public const string Grade = "{id}/grade";
    }

    public static class Attendance
    {
        public const string BaseRoute = $"{Base}/attendance";
        public const string Mark = "mark";
        public const string My = "my";
    }

    public static class Parents
    {
        public const string BaseRoute = $"{Base}/parents";
        public const string Children = "children";
        public const string ChildAssignments = "children/{studentId}/assignments";
        public const string ChildAttendance = "children/{studentId}/attendance";
    }

    public static class Files
    {
        public const string BaseRoute = $"{Base}/files";
        public const string Upload = "upload";
    }
}

public static class RoleNames
{
    public const string Admin = "Admin";
    public const string Teacher = "Teacher";
    public const string Student = "Student";
    public const string Parent = "Parent";
    public const string TeacherOrAdmin = "Teacher,Admin";
}
