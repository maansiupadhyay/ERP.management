using CollegeERP.Application.DTOs;

namespace CollegeERP.Application.Interfaces;

public interface IExamService
{
    Task<IEnumerable<ExamDto>> GetAllAsync();
    Task<ExamDto?> GetByIdAsync(int id);
    Task<IEnumerable<ExamDto>> GetByCourseAsync(int courseId);
    Task<ExamDto> CreateAsync(CreateExamRequest request);
    Task<ExamDto?> UpdateAsync(int id, CreateExamRequest request);
    Task<bool> DeleteAsync(int id);
    Task<bool> RecordResultsAsync(int examId, List<RecordResultRequest> results);
    Task<IEnumerable<ExamResultDto>> GetResultsAsync(int examId);
    Task<IEnumerable<ExamResultDto>> GetStudentResultsAsync(int studentId);
}
