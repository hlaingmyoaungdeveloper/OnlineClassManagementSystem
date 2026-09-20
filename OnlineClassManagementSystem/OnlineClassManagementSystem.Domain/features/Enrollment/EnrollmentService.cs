using Microsoft.EntityFrameworkCore;
using OnlineClassManagementSystem.Database.Models;
using OnlineClassManagementSystem.Domain.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineClassManagementSystem.Domain.features.Enrollment;

public class EnrollmentService
{
    private readonly AppDbContext _db;

    public EnrollmentService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<EnrollmentListResponseModel> GetEnrollmentsAsync(EnrollmentListRequestModel model)
    {
        try
        {
            List<EnrollmentModel> enrollments = await _db.TblEnrollments
                 .AsNoTracking()
                 .Where(x => !x.IsDelete)
                 .Select(x => new EnrollmentModel
                 {
                     EnrollmentId = x.EnrollmentId,
                     ClassId = x.ClassId,
                     ClassName = x.Class.ClassName,
                     StudentId = x.StudentId,
                     StudentName = x.Student.Username,
                     EnrollDate = x.EnrollDate,
                     IsDelete = x.IsDelete
                 }).ToListAsync();

            return new EnrollmentListResponseModel()
            {
                IsSuccess = true,
                Message = "Enrollments Fetched Successfully",
                EnrollmentList = enrollments
            };
        }
        catch (Exception ex)
        {
            return new EnrollmentListResponseModel
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }

    public async Task<EnrollmentEditResponseModel> GetEnrollmentAsync(EnrollmentEditRequestModel model)
    {
        try
        {
            var enrollment = await _db.TblEnrollments
                .AsNoTracking()
                .Include(x => x.Class)
                .Include(x => x.Student)
                .FirstOrDefaultAsync(x => !x.IsDelete && x.EnrollmentId == model.EnrollmentId);

            if (enrollment is null)
            {
                return new EnrollmentEditResponseModel()
                {
                    IsSuccess = false,
                    Message = "Enrollment is not found",
                };
            }

            return new EnrollmentEditResponseModel()
            {
                IsSuccess = true,
                Message = "Enrollment Fetched Successfully",
                EnrollmentId = enrollment.EnrollmentId,
                ClassId = enrollment.ClassId,
                ClassName = enrollment.Class?.ClassName,
                StudentId = enrollment.StudentId,
                StudentName = enrollment.Student?.Username,
                EnrollDate = enrollment.EnrollDate
            };
        }
        catch (Exception ex)
        {
            return new EnrollmentEditResponseModel
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }

    public async Task<EnrollmentCreateResponseModel> CreateEnrollmentAsync(EnrollmentCreateRequestModel model)
    {
        if (model.ClassId <= 0)
        {
            return new EnrollmentCreateResponseModel
            {
                IsSuccess = false,
                Message = "ClassId is required"
            };
        }

        if (model.StudentId <= 0)
        {
            return new EnrollmentCreateResponseModel
            {
                IsSuccess = false,
                Message = "StudentId is required"
            };
        }

        try
        {
            var subClass = await _db.TblSubClasses
                .FirstOrDefaultAsync(x => !x.IsDelete && x.SubClassId == model.ClassId);

            if (subClass is null)
            {
                return new EnrollmentCreateResponseModel
                {
                    IsSuccess = false,
                    Message = "SubClass does not exist"
                };
            }

            bool isStudentExist = await _db.TblUsers
                .AsNoTracking()
                .AnyAsync(x => !x.IsDelete && x.UserId == model.StudentId);

            if (!isStudentExist)
            {
                return new EnrollmentCreateResponseModel
                {
                    IsSuccess = false,
                    Message = "Student does not exist"
                };
            }

            bool isAlreadyEnrolled = await _db.TblEnrollments
                .AsNoTracking()
                .AnyAsync(x => !x.IsDelete && x.ClassId == model.ClassId && x.StudentId == model.StudentId);

            if (isAlreadyEnrolled)
            {
                return new EnrollmentCreateResponseModel
                {
                    IsSuccess = false,
                    Message = "Student is already enrolled in this class"
                };
            }

            if ((subClass.StudentCount ?? 0) >= subClass.StudentLimit)
            {
                return new EnrollmentCreateResponseModel
                {
                    IsSuccess = false,
                    Message = "Class student limit reached"
                };
            }

            TblEnrollment enrollment = new()
            {
                ClassId = model.ClassId,
                StudentId = model.StudentId,
                EnrollDate = model.EnrollDate ?? DateTime.Now,
                CreatedDateTime = DateTime.Now,
                ModifiedDateTime = DateTime.Now,
                IsDelete = false
            };

            _db.TblEnrollments.Add(enrollment);
            subClass.StudentCount = (subClass.StudentCount ?? 0) + 1;

            int result = await _db.SaveChangesAsync();

            return new EnrollmentCreateResponseModel
            {
                IsSuccess = result > 0,
                Message = result > 0 ? "Successfully created Enrollment" : "Failed to create Enrollment"
            };
        }
        catch (Exception ex)
        {
            return new EnrollmentCreateResponseModel
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }

    public async Task<EnrollmentPatchResponseModel> PatchEnrollmentAsync(int id, EnrollmentPatchRequestModel model)
    {
        try
        {
            var enrollment = await _db.TblEnrollments.FirstOrDefaultAsync(x => !x.IsDelete && x.EnrollmentId == id);

            if (enrollment is null)
            {
                return new EnrollmentPatchResponseModel
                {
                    IsSuccess = false,
                    Message = "Enrollment doesn't exist"
                };
            }

            int targetClassId = model.ClassId ?? enrollment.ClassId;
            int targetStudentId = model.StudentId ?? enrollment.StudentId;

            if (model.ClassId != null && model.ClassId != enrollment.ClassId)
            {
                var newSubClass = await _db.TblSubClasses.FirstOrDefaultAsync(x => !x.IsDelete && x.SubClassId == model.ClassId);
                if (newSubClass is null)
                {
                    return new EnrollmentPatchResponseModel
                    {
                        IsSuccess = false,
                        Message = "New SubClass does not exist"
                    };
                }

                if ((newSubClass.StudentCount ?? 0) >= newSubClass.StudentLimit)
                {
                    return new EnrollmentPatchResponseModel
                    {
                        IsSuccess = false,
                        Message = "New SubClass student limit reached"
                    };
                }

                var oldSubClass = await _db.TblSubClasses.FirstOrDefaultAsync(x => !x.IsDelete && x.SubClassId == enrollment.ClassId);
                if (oldSubClass is not null && (oldSubClass.StudentCount ?? 0) > 0)
                {
                    oldSubClass.StudentCount--;
                }

                newSubClass.StudentCount = (newSubClass.StudentCount ?? 0) + 1;
                enrollment.ClassId = model.ClassId.Value;
            }

            if (model.StudentId != null && model.StudentId != enrollment.StudentId)
            {
                bool isStudentExist = await _db.TblUsers
                    .AsNoTracking()
                    .AnyAsync(x => !x.IsDelete && x.UserId == model.StudentId);

                if (!isStudentExist)
                {
                    return new EnrollmentPatchResponseModel
                    {
                        IsSuccess = false,
                        Message = "Student does not exist"
                    };
                }

                enrollment.StudentId = model.StudentId.Value;
            }

            if (model.ClassId != null || model.StudentId != null)
            {
                bool isDuplicate = await _db.TblEnrollments
                    .AnyAsync(x => !x.IsDelete && x.ClassId == targetClassId && x.StudentId == targetStudentId && x.EnrollmentId != id);

                if (isDuplicate)
                {
                    return new EnrollmentPatchResponseModel
                    {
                        IsSuccess = false,
                        Message = "Enrollment for this student in this class already exists"
                    };
                }
            }

            if (model.EnrollDate != null)
            {
                enrollment.EnrollDate = model.EnrollDate;
            }

            enrollment.ModifiedDateTime = DateTime.Now;

            int result = await _db.SaveChangesAsync();
            return new EnrollmentPatchResponseModel
            {
                IsSuccess = result > 0,
                Message = result > 0 ? "Successfully updated Enrollment" : "Failed to update Enrollment"
            };
        }
        catch (Exception ex)
        {
            return new EnrollmentPatchResponseModel
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }

    public async Task<EnrollmentDeleteResponseModel> DeleteEnrollmentAsync(EnrollmentDeleteRequestModel model)
    {
        try
        {
            var enrollment = await _db.TblEnrollments.FirstOrDefaultAsync(x => !x.IsDelete && x.EnrollmentId == model.EnrollmentId);

            if (enrollment is null)
            {
                return new EnrollmentDeleteResponseModel
                {
                    IsSuccess = false,
                    Message = "Enrollment doesn't exist"
                };
            }

            var subClass = await _db.TblSubClasses.FirstOrDefaultAsync(x => !x.IsDelete && x.SubClassId == enrollment.ClassId);
            if (subClass is not null && (subClass.StudentCount ?? 0) > 0)
            {
                subClass.StudentCount--;
            }

            enrollment.IsDelete = true;
            enrollment.ModifiedDateTime = DateTime.Now;

            int result = await _db.SaveChangesAsync();

            return new EnrollmentDeleteResponseModel
            {
                IsSuccess = result > 0,
                Message = result > 0 ? "Successfully deleted Enrollment" : "Failed to delete Enrollment"
            };
        }
        catch (Exception ex)
        {
            return new EnrollmentDeleteResponseModel
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }
}
