using Microsoft.EntityFrameworkCore;
using OnlineClassManagementSystem.Database.Models;
using OnlineClassManagementSystem.Domain.models.SubClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineClassManagementSystem.Domain.features.SubClass;

public class SubClassService
{
    private readonly AppDbContext _db;

    public SubClassService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<SubClassListResponseModel> GetSubClassesAsync(SubClassListRequestModel model)
    {
        try
        {
            List<SubClassModel> subClasses = await _db.TblSubClasses
                 .AsNoTracking()
                 .Where(x => !x.IsDelete)
                 .Select(x => new SubClassModel
                 {
                     ClassName = x.ClassName,
                     Place = x.Place,
                     OpenDate = x.OpenDate,
                     OpenTime = x.OpenTime,
                     StudentLimit = x.StudentLimit,
                     StudentCount = x.StudentCount
                 }).ToListAsync();

            return new SubClassListResponseModel()
            {
                IsSuccess = true,
                Message = "SubClasses Fetched Successfully",
                SubClassList = subClasses
            };
        }
        catch (Exception ex)
        {
            return new SubClassListResponseModel
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }

    public async Task<SubClassEditResponseModel> GetSubClassAsync(SubClassEditRequestModel model)
    {
        try
        {
            var subClass = await _db.TblSubClasses
                .AsNoTracking()
                .FirstOrDefaultAsync(x => !x.IsDelete && x.SubClassId == model.SubClassId);

            if (subClass is null)
            {
                return new SubClassEditResponseModel()
                {
                    IsSuccess = false,
                    Message = "SubClass is not found",
                };
            }

            return new SubClassEditResponseModel()
            {
                IsSuccess = true,
                Message = "SubClass Fetched Successfully",
                ClassName = subClass.ClassName,
                Place = subClass.Place,
                OpenDate = subClass.OpenDate,
                OpenTime = subClass.OpenTime,
                StudentLimit = subClass.StudentLimit,
                StudentCount = subClass.StudentCount
            };
        }
        catch (Exception ex)
        {
            return new SubClassEditResponseModel
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }

    public async Task<SubClassCreateResponseModel> CreateSubClassAsync(SubClassCreateRequestModel model)
    {
        if (string.IsNullOrWhiteSpace(model.ClassName))
        {
            return new SubClassCreateResponseModel
            {
                IsSuccess = false,
                Message = "ClassName is required"
            };
        }

        if (string.IsNullOrWhiteSpace(model.Place))
        {
            return new SubClassCreateResponseModel
            {
                IsSuccess = false,
                Message = "Place is required"
            };
        }

        try
        {
            bool isClassNameExist = await _db.TblSubClasses
                .AsNoTracking()
                .AnyAsync(x => !x.IsDelete && x.ClassName == model.ClassName);

            if (isClassNameExist)
            {
                return new SubClassCreateResponseModel
                {
                    IsSuccess = false,
                    Message = "SubClass already exists"
                };
            }

            bool isPlaceAndTimeTaken = await _db.TblSubClasses
                .AsNoTracking()
                .AnyAsync(x => x.Place == model.Place
                            && x.OpenDate == model.OpenDate
                            && x.OpenTime == model.OpenTime);

            if (isPlaceAndTimeTaken)
            {
                return new SubClassCreateResponseModel
                {
                    IsSuccess = false,
                    Message = "Place, OpenDate, and OpenTime are already taken"
                };
            }

            TblSubClass subClass = new()
            {
                ClassName = model.ClassName,
                Place = model.Place,
                OpenDate = model.OpenDate,
                StudentLimit = model.StudentLimit,
                StudentCount = 0,
                OpenTime = model.OpenTime,
            };

            _db.TblSubClasses.Add(subClass);
            int result = await _db.SaveChangesAsync();

            return new SubClassCreateResponseModel
            {
                IsSuccess = result > 0,
                Message = result > 0 ? "Successfully created SubClass" : "Failed to create SubClass"
            };
        }
        catch (Exception ex)
        {
            return new SubClassCreateResponseModel
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }

    public async Task<SubClassPatchResponseModel> PatchSubClassAsync(int id, SubClassPatchRequestModel model)
    {
        try
        {
            var subClass = await _db.TblSubClasses.FirstOrDefaultAsync(x => !x.IsDelete && x.SubClassId == id);

            if (subClass is null)
            {
                return new SubClassPatchResponseModel
                {
                    IsSuccess = false,
                    Message = "SubClass doesn't exist"
                };
            }

            if (!string.IsNullOrWhiteSpace(model.ClassName) && model.ClassName != subClass.ClassName)
            {
                bool isClassNameExist = await _db.TblSubClasses
                    .AnyAsync(x => !x.IsDelete && x.ClassName == model.ClassName && x.SubClassId != id);

                if (isClassNameExist)
                {
                    return new SubClassPatchResponseModel
                    {
                        IsSuccess = false,
                        Message = "Class Name already exists"
                    };
                }
                subClass.ClassName = model.ClassName;
            }

            bool isPlaceChanged = !string.IsNullOrWhiteSpace(model.Place) && model.Place != subClass.Place;
            bool isDateChanged = model.OpenDate != default && model.OpenDate != subClass.OpenDate;
            bool isTimeChanged = !string.IsNullOrWhiteSpace(model.OpenTime) && model.OpenTime != subClass.OpenTime;

            if (isPlaceChanged || isDateChanged || isTimeChanged)
            {
                string proposedPlace = isPlaceChanged ? model.Place : subClass.Place;
                var proposedDate = isDateChanged ? model.OpenDate : subClass.OpenDate;
                var proposedTime = isTimeChanged ? model.OpenTime : subClass.OpenTime;

                bool isPlaceAndTimeTaken = await _db.TblSubClasses
                    .AnyAsync(x => !x.IsDelete && x.Place == proposedPlace
                                && x.OpenDate == proposedDate
                                && x.OpenTime == proposedTime
                                && x.SubClassId != id);

                if (isPlaceAndTimeTaken)
                {
                    return new SubClassPatchResponseModel
                    {
                        IsSuccess = false,
                        Message = "Another class already exists at the proposed place, date, and time."
                    };
                }

                if (isPlaceChanged) subClass.Place = model.Place;
                if (isDateChanged) subClass.OpenDate = (DateOnly)model.OpenDate;
                if (isTimeChanged) subClass.OpenTime = model.OpenTime;
            }

            if (model.StudentLimit != null)
            {
                if (model.StudentLimit < subClass.StudentCount)
                {
                    return new SubClassPatchResponseModel
                    {
                        IsSuccess = false,
                        Message = "Student limit cannot be less than current student count."
                    };
                }
                subClass.StudentLimit = (int)model.StudentLimit;
            }

            int result = await _db.SaveChangesAsync();
            return new SubClassPatchResponseModel
            {
                IsSuccess = result > 0,
                Message = result > 0 ? "Successfully updated SubClass" : "Failed to update SubClass"
            };
        }
        catch (Exception ex)
        {
            return new SubClassPatchResponseModel
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }

    public async Task<SubClassDeleteResponseModel> DeleteSubClassAsync(SubClassDeleteRequestModel model)
    {
        try
        {
            // Note: If you implement Soft Delete, add "&& x.IsDelete == false" to this query.
            var subClass = await _db.TblSubClasses.FirstOrDefaultAsync(x =>!x.IsDelete && x.SubClassId == model.SubClassId);

            if (subClass is null)
            {
                return new SubClassDeleteResponseModel
                {
                    IsSuccess = false,
                    Message = "SubClass doesn't exist"
                };
            }

            //var hasEnrollments = await _db.TblEnrollments.AnyAsync(x => x.SubClassId == model.SubClassId);

            //if (hasEnrollments)
            //{
            //    return new SubClassDeleteResponseModel
            //    {
            //        IsSuccess = false,
            //        Message = "Cannot delete SubClass because it has enrollments."
            //    };
            //}

            //// Hard delete
            //_db.TblSubClasses.Remove(subClass);

            subClass.IsDelete = true;

            int result = await _db.SaveChangesAsync();

            return new SubClassDeleteResponseModel
            {
                IsSuccess = result > 0,
                Message = result > 0 ? "Successfully deleted SubClass" : "Failed to delete SubClass"
            };
        }
        catch (Exception ex)
        {
            return new SubClassDeleteResponseModel
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }
}