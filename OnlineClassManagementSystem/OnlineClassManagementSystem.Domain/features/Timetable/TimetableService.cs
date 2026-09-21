using Microsoft.EntityFrameworkCore;
using OnlineClassManagementSystem.Database.Models;
using OnlineClassManagementSystem.Domain.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineClassManagementSystem.Domain.features.Timetable;

public class TimetableService
{
    private readonly AppDbContext _db;

    public TimetableService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<TimetableListResponseModel> GetTimetablesAsync(TimetableListResquestModel model)
    {
        try
        {
            List<TimetableModel> timetables = await _db.Schedules
                .AsNoTracking()
                .Include(x => x.Class)
                .Include(x => x.Teacher)
                .Include(x => x.Subject)
                .Where(x => !x.IsDelete)
                .Select(x => new TimetableModel
                {
                    SubClassId = x.ClassId,
                    ClassName = x.Class.ClassName,
                    TeacherId = x.TeacherId,
                    TeacherName = x.Teacher.Username,
                    SubjectId = x.SubjectId,
                    SubjectName = x.Subject.SubjectName,
                    DayOfWeek = x.DayOfWeek,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    Mode = x.Mode,
                    CreatedDateTime = x.CreatedDateTime
                })
                .ToListAsync();

            return new TimetableListResponseModel
            {
                IsSuccess = true,
                Message = "All timetables fetched successfully",
                TimetableList = timetables
            };
        }
        catch (Exception ex)
        {
            return new TimetableListResponseModel
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }

    public async Task<TimetableListByClassResponseModel> GetTimetablesByClassAsync(TimetableListByClassRequest model)
    {
        try
        {
            List<TimetableModel> timetables = await _db.Schedules
                .AsNoTracking()
                .Include(x => x.Class)
                .Include(x => x.Teacher)
                .Include(x => x.Subject)
                .Where(x => !x.IsDelete && x.ClassId == model.SubClassId)
                .Select(x => new TimetableModel
                {
                    SubClassId = x.ClassId,
                    ClassName= x.Class.ClassName,
                    TeacherId = x.TeacherId,
                    TeacherName = x.Teacher.Username,
                    SubjectId = x.SubjectId,
                    SubjectName = x.Subject.SubjectName,
                    DayOfWeek = x.DayOfWeek,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    Mode = x.Mode,
                    CreatedDateTime = x.CreatedDateTime
                })
                .ToListAsync();

            if (timetables.Count != 0)
            {
                return new TimetableListByClassResponseModel
                {
                    IsSuccess = true,
                    Message = "Timetables for Classs fetched successfully",
                    TimetableList = timetables
                };
            }
            return new TimetableListByClassResponseModel
            {
                IsSuccess = false,
                Message = "Timetables for Class doesn't have!",
                TimetableList = timetables
            };

        }
        catch (Exception ex)
        {
            return new TimetableListByClassResponseModel
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }

    public async Task<TimetableListByTeacherResponseModel> GetTimetablesByTeacherAsync(TimetableListByTeacherRequestModel model)
    {
        try
        {
            List<TimetableModel> timetables = await _db.Schedules
                .AsNoTracking()
                .Include(x => x.Class)
                .Include(x => x.Teacher)
                .Include(x => x.Subject)
                .Where(x => !x.IsDelete && x.TeacherId == model.TeacherId)
                .Select(x => new TimetableModel
                {
                    SubClassId = x.ClassId,
                    ClassName = x.Class.ClassName,
                    TeacherId = x.TeacherId,
                    TeacherName = x.Teacher.Username,
                    SubjectId = x.SubjectId,
                    SubjectName = x.Subject.SubjectName,
                    DayOfWeek = x.DayOfWeek,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    Mode = x.Mode,
                    CreatedDateTime = x.CreatedDateTime
                })
                .ToListAsync();

            if (timetables.Count != 0)
            {
                return new TimetableListByTeacherResponseModel
                {
                    IsSuccess = true,
                    Message = "Timetables for Teacher fetched successfully",
                    TimetableList = timetables
                };
            }
            return new TimetableListByTeacherResponseModel
            {
                IsSuccess = false,
                Message = "Timetables for Teacher doesn't have!",
                TimetableList = timetables
            };
        }
        catch (Exception ex)
        {
            return new TimetableListByTeacherResponseModel
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }

    public async Task<TimetableCreateResponseModel> CreateTimetableAsync(TimetableCreateRequestModel model)
    {
        if (model.SubClassId <= 0)
        {
            return new TimetableCreateResponseModel
            {
                IsSuccess = false,
                Message = "ClubId / ClassId is required"
            };
        }

        if (model.TeacherId <= 0)
        {
            return new TimetableCreateResponseModel
            {
                IsSuccess = false,
                Message = "TeacherId is required"
            };
        }

        if (model.SubjectId <= 0)
        {
            return new TimetableCreateResponseModel
            {
                IsSuccess = false,
                Message = "SubjectId is required"
            };
        }

        if (string.IsNullOrWhiteSpace(model.DayOfWeek))
        {
            return new TimetableCreateResponseModel
            {
                IsSuccess = false,
                Message = "DayOfWeek is required"
            };
        }

        if (model.StartTime >= model.EndTime)
        {
            return new TimetableCreateResponseModel
            {
                IsSuccess = false,
                Message = "StartTime must be earlier than EndTime"
            };
        }

        try
        {
            var subClass = await _db.TblSubClasses
                .AsNoTracking()
                .FirstOrDefaultAsync(x => !x.IsDelete && x.SubClassId == model.SubClassId);

            if (subClass is null)
            {
                return new TimetableCreateResponseModel
                {
                    IsSuccess = false,
                    Message = "Club / Class does not exist"
                };
            }

            var teacher = await _db.TblUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(x => !x.IsDelete && x.UserId == model.TeacherId);

            if (teacher is null)
            {
                return new TimetableCreateResponseModel
                {
                    IsSuccess = false,
                    Message = "Teacher does not exist"
                };
            }

            var subject = await _db.TblSubjects
                .AsNoTracking()
                .FirstOrDefaultAsync(x => !x.IsDelete && x.SubjectId == model.SubjectId);

            if (subject is null)
            {
                return new TimetableCreateResponseModel
                {
                    IsSuccess = false,
                    Message = "Subject does not exist"
                };
            }

            bool isTeacherOverlapped = await _db.Schedules
                .AsNoTracking()
                .AnyAsync(x => !x.IsDelete
                            && x.DayOfWeek == model.DayOfWeek
                            && x.TeacherId == model.TeacherId
                            && ((model.StartTime >= x.StartTime && model.StartTime < x.EndTime)
                             || (model.EndTime > x.StartTime && model.EndTime <= x.EndTime)
                             || (model.StartTime <= x.StartTime && model.EndTime >= x.EndTime)));

            if (isTeacherOverlapped)
            {
                return new TimetableCreateResponseModel
                {
                    IsSuccess = false,
                    Message = $"Teacher already has a timetable schedule during this time slot on {model.DayOfWeek}"
                };
            }

            Schedule schedule = new()
            {
                ClassId = model.SubClassId,
                TeacherId = model.TeacherId,
                SubjectId = model.SubjectId,
                DayOfWeek = model.DayOfWeek,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                Mode = string.IsNullOrWhiteSpace(model.Mode) ? "ClassRoom" : model.Mode,
                CreatedDateTime = DateTime.Now,
                ModifiedDateTime = DateTime.Now,
                IsDelete = false
            };

            _db.Schedules.Add(schedule);
            int result = await _db.SaveChangesAsync();

            return new TimetableCreateResponseModel
            {
                IsSuccess = result > 0,
                Message = result > 0 ? "Successfully created timetable schedule" : "Failed to create timetable schedule"
            };
        }
        catch (Exception ex)
        {
            return new TimetableCreateResponseModel
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }

    public async Task<TimetablePatchResponseModel> PatchTimetableAsync(int id, TimetablePatchRequestModel model)
    {
        try
        {
            var schedule = await _db.Schedules
                .FirstOrDefaultAsync(x => !x.IsDelete && x.Id == id);

            if (schedule is null)
            {
                return new TimetablePatchResponseModel
                {
                    IsSuccess = false,
                    Message = "Timetable schedule doesn't exist"
                };
            }

            if (model.SubClassId.HasValue && model.SubClassId.Value != schedule.ClassId)
            {
                var subClassExists = await _db.TblSubClasses
                    .AsNoTracking()
                    .AnyAsync(x => !x.IsDelete && x.SubClassId == model.SubClassId.Value);

                if (!subClassExists)
                {
                    return new TimetablePatchResponseModel
                    {
                        IsSuccess = false,
                        Message = "SubClass does not exist"
                    };
                }

                schedule.ClassId = model.SubClassId.Value;
            }

            if (model.TeacherId.HasValue && model.TeacherId.Value != schedule.TeacherId)
            {
                var teacherExists = await _db.TblUsers
                    .AsNoTracking()
                    .AnyAsync(x => !x.IsDelete && x.UserId == model.TeacherId.Value);

                if (!teacherExists)
                {
                    return new TimetablePatchResponseModel
                    {
                        IsSuccess = false,
                        Message = "Teacher does not exist"
                    };
                }

                schedule.TeacherId = model.TeacherId.Value;
            }

            if (model.SubjectId.HasValue && model.SubjectId.Value != schedule.SubjectId)
            {
                var subjectExists = await _db.TblSubjects
                    .AsNoTracking()
                    .AnyAsync(x => !x.IsDelete && x.SubjectId == model.SubjectId.Value);

                if (!subjectExists)
                {
                    return new TimetablePatchResponseModel
                    {
                        IsSuccess = false,
                        Message = "Subject does not exist"
                    };
                }

                schedule.SubjectId = model.SubjectId.Value;
            }

            if (!string.IsNullOrWhiteSpace(model.DayOfWeek))
            {
                schedule.DayOfWeek = model.DayOfWeek;
            }

            if (model.StartTime.HasValue)
            {
                schedule.StartTime = model.StartTime.Value;
            }

            if (model.EndTime.HasValue)
            {
                schedule.EndTime = model.EndTime.Value;
            }

            if (schedule.StartTime >= schedule.EndTime)
            {
                return new TimetablePatchResponseModel
                {
                    IsSuccess = false,
                    Message = "StartTime must be earlier than EndTime"
                };
            }

            if (!string.IsNullOrWhiteSpace(model.Mode))
            {
                schedule.Mode = model.Mode;
            }

            schedule.ModifiedDateTime = DateTime.Now;

            int result = await _db.SaveChangesAsync();

            return new TimetablePatchResponseModel
            {
                IsSuccess = result > 0,
                Message = result > 0 ? "Successfully updated timetable schedule" : "Failed to update timetable schedule"
            };
        }
        catch (Exception ex)
        {
            return new TimetablePatchResponseModel
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }

    public async Task<TimetableDeleteResponseModel> DeleteTimetableAsync(TimetableDeleteRequestModel model)
    {
        try
        {
            var schedule = await _db.Schedules
                .FirstOrDefaultAsync(x => !x.IsDelete && x.Id == model.TimetableId);

            if (schedule is null)
            {
                return new TimetableDeleteResponseModel
                {
                    IsSuccess = false,
                    Message = "Timetable schedule doesn't exist"
                };
            }

            schedule.IsDelete = true;
            schedule.ModifiedDateTime = DateTime.Now;

            int result = await _db.SaveChangesAsync();

            return new TimetableDeleteResponseModel
            {
                IsSuccess = result > 0,
                Message = result > 0 ? "Successfully deleted timetable schedule" : "Failed to delete timetable schedule"
            };
        }
        catch (Exception ex)
        {
            return new TimetableDeleteResponseModel
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }

   
}
