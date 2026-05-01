import axios from 'axios';

const api = axios.create({
 baseURL: "https://erp-management-1-p6lp.onrender.com/api",
  headers: { 'Content-Type': 'application/json' },
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

api.interceptors.response.use(
  (res) => res,
  (err) => {
    if (err.response?.status === 401) {
      localStorage.removeItem('token');
      localStorage.removeItem('user');
      window.location.href = '/login';
    }
    return Promise.reject(err);
  }
);

// Auth
export const login = (data) => api.post('/auth/login', data);
export const register = (data) => api.post('/auth/register', data);
export const getMe = () => api.get('/auth/me');

// Dashboard
export const getAdminStats = () => api.get('/dashboard/admin');
export const getFacultyDashboard = (id) => api.get(`/dashboard/faculty/${id}`);
export const getStudentDashboard = (id) => api.get(`/dashboard/student/${id}`);

// Students
export const getStudents = () => api.get('/students');
export const getStudent = (id) => api.get(`/students/${id}`);
export const getStudentsByDept = (deptId) => api.get(`/students/department/${deptId}`);
export const getStudentsBySemester = (deptId, sem) => api.get(`/students/department/${deptId}/semester/${sem}`);
export const createStudent = (data) => api.post('/students', data);
export const updateStudent = (id, data) => api.put(`/students/${id}`, data);
export const deleteStudent = (id) => api.delete(`/students/${id}`);
export const getStudentAttendance = (id) => api.get(`/students/${id}/attendance`);

// Faculty
export const getFaculties = () => api.get('/faculty');
export const getFaculty = (id) => api.get(`/faculty/${id}`);
export const createFaculty = (data) => api.post('/faculty', data);
export const updateFaculty = (id, data) => api.put(`/faculty/${id}`, data);
export const deleteFaculty = (id) => api.delete(`/faculty/${id}`);

// Departments
export const getDepartments = () => api.get('/departments');
export const getDepartment = (id) => api.get(`/departments/${id}`);
export const createDepartment = (data) => api.post('/departments', data);
export const updateDepartment = (id, data) => api.put(`/departments/${id}`, data);
export const deleteDepartment = (id) => api.delete(`/departments/${id}`);

// Courses
export const getCourses = () => api.get('/courses');
export const getCourse = (id) => api.get(`/courses/${id}`);
export const getCoursesByDept = (deptId) => api.get(`/courses/department/${deptId}`);
export const createCourse = (data) => api.post('/courses', data);
export const updateCourse = (id, data) => api.put(`/courses/${id}`, data);
export const deleteCourse = (id) => api.delete(`/courses/${id}`);
export const assignFaculty = (data) => api.post('/courses/assign-faculty', data);

// Attendance
export const createSession = (data) => api.post('/attendance/sessions', data);
export const markAttendance = (data) => api.post('/attendance/mark', data);
export const closeSession = (id) => api.put(`/attendance/sessions/${id}/close`);
export const getSessionsByDate = (date, facultyId) => api.get('/attendance/sessions', { params: { date, facultyId } });
export const getSessionsByCourse = (courseId) => api.get(`/attendance/sessions/course/${courseId}`);
export const getSessionRecords = (sessionId) => api.get(`/attendance/sessions/${sessionId}/records`);
export const getAttendanceReport = (courseId, studentId) => api.get(`/attendance/report/${courseId}`, { params: { studentId } });
export const getStudentAttendanceSummary = (studentId) => api.get(`/attendance/student/${studentId}`);

// Timetable
export const getTimetables = () => api.get('/timetable');
export const getTimetableByFaculty = (id) => api.get(`/timetable/faculty/${id}`);
export const getTimetableByDeptSem = (deptId, sem) => api.get(`/timetable/department/${deptId}/semester/${sem}`);
export const createTimetable = (data) => api.post('/timetable', data);
export const updateTimetable = (id, data) => api.put(`/timetable/${id}`, data);
export const deleteTimetable = (id) => api.delete(`/timetable/${id}`);

// Exams
export const getExams = () => api.get('/exams');
export const getExam = (id) => api.get(`/exams/${id}`);
export const getExamsByCourse = (courseId) => api.get(`/exams/course/${courseId}`);
export const createExam = (data) => api.post('/exams', data);
export const updateExam = (id, data) => api.put(`/exams/${id}`, data);
export const deleteExam = (id) => api.delete(`/exams/${id}`);
export const recordResults = (examId, data) => api.post(`/exams/${examId}/results`, data);
export const getExamResults = (examId) => api.get(`/exams/${examId}/results`);
export const getStudentResults = (studentId) => api.get(`/exams/student/${studentId}/results`);

export default api;
