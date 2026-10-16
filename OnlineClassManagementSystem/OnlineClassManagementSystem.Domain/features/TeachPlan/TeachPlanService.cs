using Microsoft.EntityFrameworkCore;
using OnlineClassManagementSystem.Database.Models;
using OnlineClassManagementSystem.Shared.models.TeachPlan;
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
            var query = _db.TblTeachPlans
                .AsNoTracking()
                .Include(x => x.Subject)
                .Where(x => !x.IsDelete);

            if (model.SubjectId.HasValue && model.SubjectId.Value > 0)
            {
                query = query.Where(x => x.SubjectId == model.SubjectId.Value);
            }

            if (!string.IsNullOrWhiteSpace(model.Search))
            {
                var search = model.Search.Trim().ToLower();
                query = query.Where(x =>
                    (x.Topic != null && x.Topic.ToLower().Contains(search)) ||
                    (x.Subject.SubjectName != null && x.Subject.SubjectName.ToLower().Contains(search)) ||
                    x.TeachPlanId.ToString().Contains(search));
            }

            var totalCount = await query.CountAsync();

            bool isPaged = model.PageNo.HasValue || model.PageSize.HasValue;
            int pageNo = model.PageNo ?? 1;
            int pageSize = model.PageSize ?? 10;
            if (pageNo < 1) pageNo = 1;
            if (pageSize < 1) pageSize = 10;

            if (isPaged)
            {
                query = query
                    .OrderByDescending(x => x.TeachPlanId)
                    .Skip((pageNo - 1) * pageSize)
                    .Take(pageSize);
            }
            else
            {
                query = query.OrderByDescending(x => x.TeachPlanId);
            }

            List<TeachPlanModel> teachPlans = await query
                .Select(x => new TeachPlanModel
                {
                    Id = x.TeachPlanId,
                    SubjectId = x.SubjectId,
                    SubjectName = x.Subject.SubjectName,
                    Topic = x.Topic
                }).ToListAsync();

            return new TeachPlanListResponseModel
            {
                IsSuccess = true,
                Message = "TeachPlans Fetched Successfully",
                TeachPlanList = teachPlans,
                TotalCount = totalCount,
                PageNo = isPaged ? pageNo : 1,
                PageSize = isPaged ? pageSize : (totalCount > 0 ? totalCount : 10)
            };
        }
        catch (Exception ex)
        {
            return new TeachPlanListResponseModel
            {
                IsSuccess = false,
                Message = ex.Message,
                TeachPlanList = new List<TeachPlanModel>()
            };
        }
    }

    public async Task<TeachPlanEditResponseModel> GetTeachPlanAsync(TeachPlanEditRequestModel model)
    {
        try
        {
            var teachPlan = await _db.TblTeachPlans
                .Include(x => x.Subject)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => !x.IsDelete && x.TeachPlanId == model.Id);

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
                Id = teachPlan.TeachPlanId,
                SubjectId = teachPlan.SubjectId,
                SubjectName = teachPlan.Subject.SubjectName,
                Topic = teachPlan.Topic
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

            TblTeachPlan teachPlan = new()
            {
                SubjectId = model.SubjectId,
                Topic = model.Topic,
                CreatedDateTime = DateTime.Now,
                ModifiedDateTime = DateTime.Now
            };

            _db.TblTeachPlans.Add(teachPlan);
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
            var teachPlan = await _db.TblTeachPlans
                .FirstOrDefaultAsync(x => !x.IsDelete && x.TeachPlanId == id);

            if (teachPlan is null)
            {
                return new TeachPlanPatchResponseModel
                {
                    IsSuccess = false,
                    Message = "TeachPlan doesn't exist"
                };
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
            var teachPlan = await _db.TblTeachPlans
                .FirstOrDefaultAsync(x => !x.IsDelete && x.TeachPlanId == model.Id);

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
