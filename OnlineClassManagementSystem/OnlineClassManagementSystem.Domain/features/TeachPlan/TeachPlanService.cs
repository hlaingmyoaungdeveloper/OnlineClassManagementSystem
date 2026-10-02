using Microsoft.EntityFrameworkCore;
using OnlineClassManagementSystem.Database.Models;
using OnlineClassManagementSystem.Domain.models.TeachPlan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineClassManagementSystem.Domain.features.TeachPlan;

public class TeachPlanService
{
    private readonly AppDbContext _db;

    public TeachPlanService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<TeachPlanListResponseModel> GetTeachPlansAsync(TeachPlanListRequestModel model)
    {
        try
        {
            List<TeachPlanModel> teachPlans = await _db.TeachPlans
                .AsNoTracking()
                .Where(x => !x.IsDelete)
                .Select(x => new TeachPlanModel
                {
                    Id = x.Id,
                    TeacherId = x.TeacherId,
                    ClassId = x.ClassId,
                    SubjectId = x.SubjectId,
                    Topic = x.Topic,
                    IsCompleted = x.IsCompleted,
                    CompletedDate = x.CompletedDate
                }).ToListAsync();

            return new TeachPlanListResponseModel
            {
                IsSuccess = true,
                Message = "TeachPlans Fetched Successfully",
                TeachPlanList = teachPlans
            };
        }
        catch (Exception ex)
        {
            return new TeachPlanListResponseModel
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }

    public async Task<TeachPlanEditResponseModel> GetTeachPlanAsync(TeachPlanEditRequestModel model)
    {
        try
        {
            var teachPlan = await _db.TeachPlans
                .AsNoTracking()
                .FirstOrDefaultAsync(x => !x.IsDelete && x.Id == model.Id);

            if (teachPlan is null)
            {
                return new TeachPlanEditResponseModel
                {
                    IsSuccess = false,
                    Message = "TeachPlan is not found"
                };
            }

            return new TeachPlanEditResponseModel
            {
                IsSuccess = true,
                Message = "TeachPlan Fetched Successfully",
                TeacherId = teachPlan.TeacherId,
                ClassId = teachPlan.ClassId,
                SubjectId = teachPlan.SubjectId,
                Topic = teachPlan.Topic,
                IsCompleted = teachPlan.IsCompleted,
                CompletedDate = teachPlan.CompletedDate
            };
        }
        catch (Exception ex)
        {
            return new TeachPlanEditResponseModel
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }

    public async Task<TeachPlanCreateResponseModel> CreateTeachPlanAsync(TeachPlanCreateRequestModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Topic))
        {
            return new TeachPlanCreateResponseModel
            {
                IsSuccess = false,
                Message = "Topic is required"
            };
        }

        try
        {
            bool isTeacherExists = await _db.TblUsers
                .AsNoTracking()
                .AnyAsync(x => x.UserId == model.TeacherId);

            if (!isTeacherExists)
            {
                return new TeachPlanCreateResponseModel
                {
                    IsSuccess = false,
                    Message = "Teacher does not exist"
                };
            }

            bool isClassExists = await _db.TblSubClasses
                .AsNoTracking()
                .AnyAsync(x => !x.IsDelete && x.SubClassId == model.ClassId);

            if (!isClassExists)
            {
                return new TeachPlanCreateResponseModel
                {
                    IsSuccess = false,
                    Message = "Class does not exist"
                };
            }

            bool isSubjectExists = await _db.TblSubjects
                .AsNoTracking()
                .AnyAsync(x => x.SubjectId == model.SubjectId);

            if (!isSubjectExists)
            {
                return new TeachPlanCreateResponseModel
                {
                    IsSuccess = false,
                    Message = "Subject does not exist"
                };
            }

            Database.Models.TeachPlan teachPlan = new()
            {
                TeacherId = model.TeacherId,
                ClassId = model.ClassId,
                SubjectId = model.SubjectId,
                Topic = model.Topic,
                IsCompleted = false,
                CreatedDateTime = DateTime.Now,
                ModifiedDateTime = DateTime.Now
            };

            _db.TeachPlans.Add(teachPlan);
            int result = await _db.SaveChangesAsync();

            return new TeachPlanCreateResponseModel
            {
                IsSuccess = result > 0,
                Message = result > 0 ? "Successfully created TeachPlan" : "Failed to create TeachPlan"
            };
        }
        catch (Exception ex)
        {
            return new TeachPlanCreateResponseModel
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }

    public async Task<TeachPlanPatchResponseModel> PatchTeachPlanAsync(int id, TeachPlanPatchRequestModel model)
    {
        try
        {
            var teachPlan = await _db.TeachPlans
                .FirstOrDefaultAsync(x => !x.IsDelete && x.Id == id);

            if (teachPlan is null)
            {
                return new TeachPlanPatchResponseModel
                {
                    IsSuccess = false,
                    Message = "TeachPlan doesn't exist"
                };
            }

            if (model.TeacherId.HasValue)
            {
                bool isTeacherExists = await _db.TblUsers
                    .AnyAsync(x => x.UserId == model.TeacherId.Value);

                if (!isTeacherExists)
                {
                    return new TeachPlanPatchResponseModel
                    {
                        IsSuccess = false,
                        Message = "Teacher does not exist"
                    };
                }
                teachPlan.TeacherId = model.TeacherId.Value;
            }

            if (model.ClassId.HasValue)
            {
                bool isClassExists = await _db.TblSubClasses
                    .AnyAsync(x => !x.IsDelete && x.SubClassId == model.ClassId.Value);

                if (!isClassExists)
                {
                    return new TeachPlanPatchResponseModel
                    {
                        IsSuccess = false,
                        Message = "Class does not exist"
                    };
                }
                teachPlan.ClassId = model.ClassId.Value;
            }

            if (model.SubjectId.HasValue)
            {
                bool isSubjectExists = await _db.TblSubjects
                    .AnyAsync(x => x.SubjectId == model.SubjectId.Value);

                if (!isSubjectExists)
                {
                    return new TeachPlanPatchResponseModel
                    {
                        IsSuccess = false,
                        Message = "Subject does not exist"
                    };
                }
                teachPlan.SubjectId = model.SubjectId.Value;
            }

            if (!string.IsNullOrWhiteSpace(model.Topic))
            {
                teachPlan.Topic = model.Topic;
            }

            if (model.IsCompleted.HasValue)
            {
                teachPlan.IsCompleted = model.IsCompleted.Value;
                if (model.IsCompleted.Value && teachPlan.CompletedDate is null)
                {
                    teachPlan.CompletedDate = DateTime.Now;
                }
            }

            if (model.CompletedDate.HasValue)
            {
                teachPlan.CompletedDate = model.CompletedDate.Value;
            }

            teachPlan.ModifiedDateTime = DateTime.Now;

            int result = await _db.SaveChangesAsync();
            return new TeachPlanPatchResponseModel
            {
                IsSuccess = result > 0,
                Message = result > 0 ? "Successfully updated TeachPlan" : "Failed to update TeachPlan"
            };
        }
        catch (Exception ex)
        {
            return new TeachPlanPatchResponseModel
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }

    public async Task<TeachPlanDeleteResponseModel> DeleteTeachPlanAsync(TeachPlanDeleteRequestModel model)
    {
        try
        {
            var teachPlan = await _db.TeachPlans
                .FirstOrDefaultAsync(x => !x.IsDelete && x.Id == model.Id);

            if (teachPlan is null)
            {
                return new TeachPlanDeleteResponseModel
                {
                    IsSuccess = false,
                    Message = "TeachPlan doesn't exist"
                };
            }

            teachPlan.IsDelete = true;
            teachPlan.ModifiedDateTime = DateTime.Now;

            int result = await _db.SaveChangesAsync();

            return new TeachPlanDeleteResponseModel
            {
                IsSuccess = result > 0,
                Message = result > 0 ? "Successfully deleted TeachPlan" : "Failed to delete TeachPlan"
            };
        }
        catch (Exception ex)
        {
            return new TeachPlanDeleteResponseModel
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }
}
