using CollegeERP.Application.DTOs;
using CollegeERP.Application.Interfaces;
using CollegeERP.Domain.Entities;
using CollegeERP.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CollegeERP.Application.Services;

public class ExamService : IExamService
{
    private readonly IGenericRepository<Exam> _examRepo;
    private readonly IGenericRepository<ExamResult> _resultRepo;

    public ExamService(IGenericRepository<Exam> examRepo, IGenericRepository<ExamResult> resultRepo)
    { _examRepo = examRepo; _resultRepo = resultRepo; }

    public async Task<IEnumerable<ExamDto>> GetAllAsync()
    {
        var exams = await _examRepo.Query()
            .Include(e => e.Course).Include(e => e.ExamResults)
            .OrderByDescending(e => e.Date).ToListAsync();
        return exams.Select(MapToDto);
    }

    public async Task<ExamDto?> GetByIdAsync(int id)
    {
        var e = await _examRepo.Query().Include(e => e.Course).Include(e => e.ExamResults)
            .FirstOrDefaultAsync(e => e.Id == id);
        return e == null ? null : MapToDto(e);
    }

    public async Task<IEnumerable<ExamDto>> GetByCourseAsync(int courseId)
    {
        var exams = await _examRepo.Query()
            .Include(e => e.Course).Include(e => e.ExamResults)
            .Where(e => e.CourseId == courseId)
            .OrderByDescending(e => e.Date).ToListAsync();
        return exams.Select(MapToDto);
    }

    public async Task<ExamDto> CreateAsync(CreateExamRequest request)
    {
        var exam = new Exam
        {
            CourseId = request.CourseId, Title = request.Title,
            ExamType = Enum.Parse<ExamType>(request.ExamType),
            TotalMarks = request.TotalMarks, Date = request.Date,
            Semester = request.Semester, AcademicYear = request.AcademicYear
        };
        await _examRepo.AddAsync(exam);
        return (await GetByIdAsync(exam.Id))!;
    }

    public async Task<ExamDto?> UpdateAsync(int id, CreateExamRequest request)
    {
        var e = await _examRepo.GetByIdAsync(id);
        if (e == null) return null;
        e.CourseId = request.CourseId; e.Title = request.Title;
        e.ExamType = Enum.Parse<ExamType>(request.ExamType);
        e.TotalMarks = request.TotalMarks; e.Date = request.Date;
        e.Semester = request.Semester; e.AcademicYear = request.AcademicYear;
        e.UpdatedAt = DateTime.UtcNow;
        await _examRepo.UpdateAsync(e);
        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var e = await _examRepo.GetByIdAsync(id);
        if (e == null) return false;
        await _examRepo.DeleteAsync(e);
        return true;
    }

    public async Task<bool> RecordResultsAsync(int examId, List<RecordResultRequest> results)
    {
        var exam = await _examRepo.GetByIdAsync(examId);
        if (exam == null) return false;

        foreach (var r in results)
        {
            var existing = await _resultRepo.Query()
                .FirstOrDefaultAsync(er => er.ExamId == examId && er.StudentId == r.StudentId);

            var grade = CalculateGrade(r.MarksObtained, exam.TotalMarks);

            if (existing != null)
            {
                existing.MarksObtained = r.MarksObtained;
                existing.Grade = grade;
                existing.Remarks = r.Remarks;
                await _resultRepo.UpdateAsync(existing);
            }
            else
            {
                await _resultRepo.AddAsync(new ExamResult
                {
                    ExamId = examId, StudentId = r.StudentId,
                    MarksObtained = r.MarksObtained, Grade = grade, Remarks = r.Remarks
                });
            }
        }
        return true;
    }

    public async Task<IEnumerable<ExamResultDto>> GetResultsAsync(int examId)
    {
        var results = await _resultRepo.Query()
            .Include(r => r.Exam).ThenInclude(e => e.Course)
            .Include(r => r.Student)
            .Where(r => r.ExamId == examId)
            .OrderBy(r => r.Student.FirstName).ToListAsync();
        return results.Select(MapResultToDto);
    }

    public async Task<IEnumerable<ExamResultDto>> GetStudentResultsAsync(int studentId)
    {
        var results = await _resultRepo.Query()
            .Include(r => r.Exam).ThenInclude(e => e.Course)
            .Include(r => r.Student)
            .Where(r => r.StudentId == studentId)
            .OrderByDescending(r => r.Exam.Date).ToListAsync();
        return results.Select(MapResultToDto);
    }

    private static string CalculateGrade(double marks, int total)
    {
        var pct = (marks / total) * 100;
        return pct switch
        {
            >= 90 => "A+", >= 80 => "A", >= 70 => "B+",
            >= 60 => "B", >= 50 => "C", >= 40 => "D", _ => "F"
        };
    }

    private static ExamDto MapToDto(Exam e) => new()
    {
        Id = e.Id, CourseId = e.CourseId, CourseName = e.Course?.Name ?? "",
        CourseCode = e.Course?.Code ?? "", Title = e.Title,
        ExamType = e.ExamType.ToString(), TotalMarks = e.TotalMarks,
        Date = e.Date, Semester = e.Semester, AcademicYear = e.AcademicYear,
        ResultCount = e.ExamResults?.Count ?? 0
    };

    private static ExamResultDto MapResultToDto(ExamResult r) => new()
    {
        Id = r.Id, ExamId = r.ExamId, ExamTitle = r.Exam?.Title ?? "",
        CourseName = r.Exam?.Course?.Name ?? "", StudentId = r.StudentId,
        StudentName = r.Student != null ? $"{r.Student.FirstName} {r.Student.LastName}" : "",
        EnrollmentNumber = r.Student?.EnrollmentNumber ?? "",
        MarksObtained = r.MarksObtained, TotalMarks = r.Exam?.TotalMarks ?? 0,
        Grade = r.Grade, Remarks = r.Remarks
    };
}
