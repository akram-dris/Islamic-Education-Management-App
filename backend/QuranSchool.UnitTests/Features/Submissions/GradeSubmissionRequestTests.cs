using FluentAssertions;
using QuranSchool.API.Controllers;
using QuranSchool.Application.Features.Auth.Login;
using QuranSchool.Application.Features.Attendance.Mark;
using QuranSchool.Application.Features.Users.GetAll;
using QuranSchool.Domain.Enums;
using Xunit;

namespace QuranSchool.UnitTests.Features.Submissions;

public class GradeSubmissionRequestTests
{
    [Fact]
    public void GradeSubmissionRequest_ShouldSetProperties()
    {
        // Arrange
        var remarks = "Good job";
        var grade = 10m;

        // Act
        var request = new GradeSubmissionRequest(grade, remarks);

        // Assert
        request.Grade.Should().Be(grade);
        request.Remarks.Should().Be(remarks);
    }

    [Fact]
    public void GradeSubmissionRequest_Equality_ShouldWork()
    {
        // Arrange
        var request1 = new GradeSubmissionRequest(10m, "feedback");
        var request2 = new GradeSubmissionRequest(10m, "feedback");
        var request3 = new GradeSubmissionRequest(9m, "feedback");
        var request4 = new GradeSubmissionRequest(10m, "other");

        // Assert
        request1.Should().Be(request2);
        request1.Should().NotBe(request3);
        request1.Should().NotBe(request4);
        request1.Grade.Should().Be(10m);
        request1.Remarks.Should().Be("feedback");
        request1.GetHashCode().Should().Be(request2.GetHashCode());
    }
}

public class CreateSubmissionRequestTests
{
    [Fact]
    public void CreateSubmissionRequest_ShouldSetProperties()
    {
        // Arrange
        var assignmentId = Guid.NewGuid();
        var fileUrl = "http://test.com/file.pdf";

        // Act
        var request = new CreateSubmissionRequest(assignmentId, fileUrl);

        // Assert
        request.AssignmentId.Should().Be(assignmentId);
        request.FileUrl.Should().Be(fileUrl);
    }

    [Fact]
    public void CreateSubmissionRequest_Equality_ShouldWork()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request1 = new CreateSubmissionRequest(id, "url");
        var request2 = new CreateSubmissionRequest(id, "url");
        var request3 = new CreateSubmissionRequest(id, "url2");

        // Assert
        request1.Should().Be(request2);
        request1.Should().NotBe(request3);
        request1.GetHashCode().Should().Be(request2.GetHashCode());
    }
}

public class UpdateRequestTests
{
    [Fact]
    public void UpdateAllocationRequest_ShouldWork()
    {
        var id = Guid.NewGuid();
        var req1 = new UpdateAllocationRequest(id, id, id);
        var req2 = new UpdateAllocationRequest(id, id, id);
        req1.Should().Be(req2);
        req1.TeacherId.Should().Be(id);
        req1.ClassId.Should().Be(id);
        req1.SubjectId.Should().Be(id);
        req1.GetHashCode().Should().Be(req2.GetHashCode());
    }

    [Fact]
    public void UpdateAssignmentRequest_ShouldWork()
    {
        var date = DateOnly.FromDateTime(DateTime.Now);
        var req1 = new UpdateAssignmentRequest("T", "D", date);
        var req2 = new UpdateAssignmentRequest("T", "D", date);
        req1.Should().Be(req2);
        req1.Title.Should().Be("T");
        req1.Description.Should().Be("D");
        req1.DueDate.Should().Be(date);
        req1.GetHashCode().Should().Be(req2.GetHashCode());
    }

    [Fact]
    public void UpdateAttendanceRequest_ShouldWork()
    {
        var date = DateOnly.FromDateTime(DateTime.Now);
        var records = new List<AttendanceRecordDto>();
        var req1 = new UpdateAttendanceRequest(date, records);
        
        req1.Date.Should().Be(date);
        req1.Records.Should().BeSameAs(records);
    }

    [Fact]
    public void UpdateClassRequest_ShouldWork()
    {
        var req1 = new UpdateClassRequest("N");
        var req2 = new UpdateClassRequest("N");
        req1.Should().Be(req2);
        req1.Name.Should().Be("N");
        req1.GetHashCode().Should().Be(req2.GetHashCode());
    }

    [Fact]
    public void UpdateSubjectRequest_ShouldWork()
    {
        var req1 = new UpdateSubjectRequest("N");
        var req2 = new UpdateSubjectRequest("N");
        req1.Should().Be(req2);
        req1.Name.Should().Be("N");
        req1.GetHashCode().Should().Be(req2.GetHashCode());
    }

    [Fact]
    public void UpdateUserRequest_ShouldWork()
    {
        var req1 = new UpdateUserRequest("F", "P");
        var req2 = new UpdateUserRequest("F", "P");
        req1.Should().Be(req2);
        req1.FullName.Should().Be("F");
        req1.Password.Should().Be("P");
        req1.GetHashCode().Should().Be(req2.GetHashCode());
    }

    [Fact]
    public void LoginResponse_ShouldWork()
    {
        var resp1 = new LoginResponse("T", "U", "F", "R");
        var resp2 = new LoginResponse("T", "U", "F", "R");
        resp1.Should().Be(resp2);
        resp1.Token.Should().Be("T");
        resp1.Username.Should().Be("U");
        resp1.FullName.Should().Be("F");
        resp1.Role.Should().Be("R");
        resp1.GetHashCode().Should().Be(resp2.GetHashCode());
    }
    
    [Fact]
    public void UserResponse_ShouldWork()
    {
        var id = Guid.NewGuid();
        var resp1 = new UserResponse(id, "U", "F", "R");
        var resp2 = new UserResponse(id, "U", "F", "R");
        resp1.Should().Be(resp2);
        resp1.Id.Should().Be(id);
        resp1.Username.Should().Be("U");
        resp1.FullName.Should().Be("F");
        resp1.Role.Should().Be("R");
        resp1.GetHashCode().Should().Be(resp2.GetHashCode());
    }
}