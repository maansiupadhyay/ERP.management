using CollegeERP.Application.DTOs;

namespace CollegeERP.Application.Interfaces;

public interface IAttendanceService
{
    Task<AttendanceSessionDto> CreateSessionAsync(CreateAttendanceSessionRequest request);
    Task<bool> MarkAttendanceAsync(MarkAttendanceRequest request);
    Task<bool> CloseSessionAsync(int sessionId);
    Task<IEnumerable<AttendanceSessionDto>> GetSessionsByDateAsync(DateTime date, int? facultyId = null);
    Task<IEnumerable<AttendanceSessionDto>> GetSessionsByCourseAsync(int courseId);
    Task<AttendanceReportDto> GetAttendanceReportAsync(int courseId, int? studentId = null);
    Task<StudentAttendanceSummaryDto> GetStudentAttendanceSummaryAsync(int studentId);
    Task<IEnumerable<AttendanceRecordDto>> GetSessionRecordsAsync(int sessionId);
}
