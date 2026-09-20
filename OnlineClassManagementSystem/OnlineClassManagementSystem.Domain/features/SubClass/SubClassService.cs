using Microsoft.EntityFrameworkCore;
using OnlineClassManagementSystem.Database.Models;
using OnlineClassManagementSystem.Domain.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
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
               .FirstOrDefaultAsync(x => x.SubClassId == model.SubClassId);

            if (subClass is null)
            {
                return new SubClassEditResponseModel()
                {
                    Message = "SubClass is not found",
                };
            }
            return new SubClassEditResponseModel()
            {
                IsSuccess = true,
                Message = "SubClas Fetched Successfully",
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
            .AnyAsync(x => x.ClassName == model.ClassName);

            if (isClassNameExist)
            {
                return new SubClassCreateResponseModel
                {
                    IsSuccess = false,
                    Message = "SubClass is already exist"
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
                    Message = "Place and OpenDate and OpenTime are already exist"
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
                //CreatedDateTime = DateTime.Now,
                //ModifiedDateTime = DateTime.Now,
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
            var subClass = await _db.TblSubClasses.FirstOrDefaultAsync(x => x.SubClassId == id);
            if (subClass is null)
            {
                return new SubClassPatchResponseModel
                {
                    Message = "SubClass doesn't exist"
                };
            }
            if (!string.IsNullOrEmpty(model.ClassName))
            {
                subClass.ClassName = model.ClassName;
            }

            if (!string.IsNullOrEmpty(model.Place))
            {
                subClass.Place = model.Place;
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
                subClass.StudentLimit = model.StudentLimit;
            }


            if (model.OpenDate != null)
            {
                subClass.OpenDate = model.OpenDate;
            }


            if (model.OpenTime != null)
            {
                subClass.OpenTime = model.OpenTime;
            }

            //subClass.ModifiedDateTime = DateTime.Now;
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

}

