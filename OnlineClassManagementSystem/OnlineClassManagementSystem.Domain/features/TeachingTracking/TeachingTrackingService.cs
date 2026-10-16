using Microsoft.EntityFrameworkCore;
using OnlineClassManagementSystem.Database.Models;
using OnlineClassManagementSystem.Shared.models.TeachingTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineClassManagementSystem.Domain.features.TeachingTracking;

public class TeachingTrackingService
{
    private readonly AppDbContext _db;

    public TeachingTrackingService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<TeachingTrackingListResponseModel> GetTeachingTrackingsAsync(TeachingTrackingListRequestModel model)
    {
        try
        {
            var query = _db.TblTeachTrackings
                .AsNoTracking()
                .Include(x => x.TeachPlan)
                .Where(x => !x.IsDelete);

            if (model.SubClassId.HasValue && model.SubClassId.Value > 0)
            {
                query = query.Where(x => x.SubClassId == model.SubClassId.Value);
            }

            if (model.TeacherId.HasValue && model.TeacherId.Value > 0)
            {
                query = query.Where(x => x.TeacherId == model.TeacherId.Value);
            }

            if (!string.IsNullOrWhiteSpace(model.Search))
            {
                var search = model.Search.Trim().ToLower();

                var matchingSubClassIds = await _db.TblSubClasses
                    .Where(c => c.ClassName.ToLower().Contains(search))
                    .Select(c => c.SubClassId)
                    .ToListAsync();

                var matchingTeacherIds = await _db.TblUsers
                    .Where(u => u.FullName.ToLower().Contains(search))
                    .Select(u => u.UserId)
                    .ToListAsync();

                query = query.Where(x =>
                    (x.Remarks != null && x.Remarks.ToLower().Contains(search)) ||
                    (x.TeachPlan.Topic != null && x.TeachPlan.Topic.ToLower().Contains(search)) ||
                    x.TrackingId.ToString().Contains(search) ||
                    matchingSubClassIds.Contains(x.SubClassId) ||
                    matchingTeacherIds.Contains(x.TeacherId));
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
                    .OrderByDescending(x => x.TrackingId)
                    .Skip((pageNo - 1) * pageSize)
                    .Take(pageSize);
            }
            else
            {
                query = query.OrderByDescending(x => x.TrackingId);
            }

            var trackings = await query.ToListAsync();
            var subClassIds = trackings.Select(x => x.SubClassId).Distinct().ToList();
            var teacherIds = trackings.Select(x => x.TeacherId).Distinct().ToList();

            var classNames = await _db.TblSubClasses
                .AsNoTracking()
                .Where(x => subClassIds.Contains(x.SubClassId))
                .ToDictionaryAsync(x => x.SubClassId, x => x.ClassName);
            var teacherNames = await _db.TblUsers
                .AsNoTracking()
                .Where(x => teacherIds.Contains(x.UserId))
                .ToDictionaryAsync(x => x.UserId, x => x.FullName);

            List<TeachingTrackingModel> trackingList = trackings
                .Select(x => new TeachingTrackingModel
                {
                    TrackId = x.TrackingId,
                    TeachPlanId = x.TeachPlanId,
                    TopicTaught = x.TeachPlan?.Topic,
                    SubClassId = x.SubClassId,
                    ClassName = classNames.GetValueOrDefault(x.SubClassId),
                    TeacherId = x.TeacherId,
                    TeacherName = teacherNames.GetValueOrDefault(x.TeacherId),
                    DateTaught = x.DateTaught,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    Remarks = x.Remarks
                })
                .ToList();

            return new TeachingTrackingListResponseModel
            {
                IsSuccess = true,
                Message = "Teaching tracking records fetched successfully",
                TeachingTrackingList = trackingList,
                TotalCount = totalCount,
                PageNo = isPaged ? pageNo : 1,
                PageSize = isPaged ? pageSize : (totalCount > 0 ? totalCount : 10)
            };
        }
        catch (Exception ex)
        {
            return new TeachingTrackingListResponseModel
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }

    public async Task<TeachingTrackingEditResponseModel> GetTeachingTrackingAsync(TeachingTrackingEditRequestModel model)
    {
        try
        {
            var tracking = await _db.TblTeachTrackings
                .AsNoTracking()
                .Include(x => x.TeachPlan)
                .FirstOrDefaultAsync(x => !x.IsDelete && x.TrackingId == model.TrackId);

            if (tracking is null)
            {
                return new TeachingTrackingEditResponseModel
                {
                    IsSuccess = false,
                    Message = "Teaching tracking record is not found"
                };
            }

            string? className = await _db.TblSubClasses
                .AsNoTracking()
                .Where(x => x.SubClassId == tracking.SubClassId)
                .Select(x => x.ClassName)
                .FirstOrDefaultAsync();
            string? teacherName = await _db.TblUsers
                .AsNoTracking()
                .Where(x => x.UserId == tracking.TeacherId)
                .Select(x => x.FullName)
                .FirstOrDefaultAsync();

            return new TeachingTrackingEditResponseModel
            {
                IsSuccess = true,
                Message = "Teaching tracking fetched successfully",
                TrackId = tracking.TrackingId,
                TeachPlanId = tracking.TeachPlanId,
                TopicTaught = tracking.TeachPlan.Topic,
                SubClassId = tracking.SubClassId,
                ClassName = className,
                TeacherId = tracking.TeacherId,
                TeacherName = teacherName,
                DateTaught = tracking.DateTaught,
                StartTime = tracking.StartTime,
                EndTime = tracking.EndTime,
                Remarks = tracking.Remarks
            };
        }
        catch (Exception ex)
        {
            return new TeachingTrackingEditResponseModel
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }

    public async Task<TeachingTrackingCreateResponseModel> CreateTeachingTrackingAsync(TeachingTrackingCreateRequestModel model)
    {
        if (model.SubClassId <= 0)
        {
            return new TeachingTrackingCreateResponseModel
            {
                IsSuccess = false,
                Message = "SubClassId is required and must be greater than 0"
            };
        }

        if (model.TeacherId <= 0)
        {
            return new TeachingTrackingCreateResponseModel
            {
                IsSuccess = false,
                Message = "TeacherId is required and must be greater than 0"
            };
        }

        if (model.TeachPlanId <= 0)
        {
            return new TeachingTrackingCreateResponseModel
            {
                IsSuccess = false,
                Message = "TeachPlanId is required and must be greater than 0"
            };
        }

        try
        {
            bool isSubClassExists = await _db.TblSubClasses
                .AsNoTracking()
                .AnyAsync(x => !x.IsDelete && x.SubClassId == model.SubClassId);

            if (!isSubClassExists)
            {
                return new TeachingTrackingCreateResponseModel
                {
                    IsSuccess = false,
                    Message = "SubClass does not exist"
                };
            }

            bool isTeacherExists = await _db.TblUsers
                .AsNoTracking()
                .AnyAsync(x => !x.IsDelete && x.UserId == model.TeacherId);

            if (!isTeacherExists)
            {
                return new TeachingTrackingCreateResponseModel
                {
                    IsSuccess = false,
                    Message = "Teacher does not exist"
                };
            }

            bool isTeachPlanExists = await _db.TblTeachPlans
                .AsNoTracking()
                .AnyAsync(x => !x.IsDelete && x.TeachPlanId == model.TeachPlanId);

            if (!isTeachPlanExists)
            {
                return new TeachingTrackingCreateResponseModel
                {
                    IsSuccess = false,
                    Message = "TeachPlan does not exist"
                };
            }

            TblTeachTracking tracking = new()
            {
                TeachPlanId = model.TeachPlanId,
                SubClassId = model.SubClassId,
                TeacherId = model.TeacherId,
                DateTaught = model.DateTaught == default ? DateOnly.FromDateTime(DateTime.Now) : model.DateTaught,
                StartTime = model.StartTime ?? default,
                EndTime = model.EndTime ?? default,
                Remarks = model.Remarks,
                CreatedDateTime = DateTime.Now
            };

            _db.TblTeachTrackings.Add(tracking);
            int result = await _db.SaveChangesAsync();

            return new TeachingTrackingCreateResponseModel
            {
                IsSuccess = result > 0,
                Message = result > 0 ? "Successfully created TeachingTracking" : "Failed to create TeachingTracking"
            };
        }
        catch (Exception ex)
        {
            return new TeachingTrackingCreateResponseModel
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }

    public async Task<TeachingTrackingPatchResponseModel> PatchTeachingTrackingAsync(int id, TeachingTrackingPatchRequestModel model)
    {
        try
        {
            var tracking = await _db.TblTeachTrackings
                .FirstOrDefaultAsync(x => !x.IsDelete && x.TrackingId == id);

            if (tracking is null)
            {
                return new TeachingTrackingPatchResponseModel
                {
                    IsSuccess = false,
                    Message = "TeachingTracking doesn't exist"
                };
            }

            if (model.SubClassId.HasValue && model.SubClassId.Value != tracking.SubClassId)
            {
                bool isSubClassExists = await _db.TblSubClasses
                    .AnyAsync(x => !x.IsDelete && x.SubClassId == model.SubClassId.Value);

                if (!isSubClassExists)
                {
                    return new TeachingTrackingPatchResponseModel
                    {
                        IsSuccess = false,
                        Message = "SubClass does not exist"
                    };
                }

                tracking.SubClassId = model.SubClassId.Value;
            }

            if (model.TeacherId.HasValue && model.TeacherId.Value != tracking.TeacherId)
            {
                bool isTeacherExists = await _db.TblUsers
                    .AnyAsync(x => !x.IsDelete && x.UserId == model.TeacherId.Value);

                if (!isTeacherExists)
                {
                    return new TeachingTrackingPatchResponseModel
                    {
                        IsSuccess = false,
                        Message = "Teacher does not exist"
                    };
                }

                tracking.TeacherId = model.TeacherId.Value;
            }

            if (model.TeachPlanId.HasValue && model.TeachPlanId.Value != tracking.TeachPlanId)
            {
                bool isTeachPlanExists = await _db.TblTeachPlans
                    .AnyAsync(x => !x.IsDelete && x.TeachPlanId == model.TeachPlanId.Value);

                if (!isTeachPlanExists)
                {
                    return new TeachingTrackingPatchResponseModel
                    {
                        IsSuccess = false,
                        Message = "TeachPlan does not exist"
                    };
                }

                tracking.TeachPlanId = model.TeachPlanId.Value;
            }

            if (model.DateTaught.HasValue && model.DateTaught.Value != default)
            {
                tracking.DateTaught = model.DateTaught.Value;
            }

            // Validate and update start/end times if both are provided
            if (model.StartTime.HasValue && model.EndTime.HasValue)
            {
                if (model.StartTime.Value > model.EndTime.Value)
                {
                    return new TeachingTrackingPatchResponseModel
                    {
                        IsSuccess = false,
                        Message = "StartTime cannot be later than EndTime."
                    };
                }
                tracking.StartTime = model.StartTime.Value;
                tracking.EndTime = model.EndTime.Value;
            }

            if (model.Remarks is not null)
            {
                tracking.Remarks = model.Remarks;
            }

            tracking.ModifiedDateTime = DateTime.Now;

            int result = await _db.SaveChangesAsync();

            return new TeachingTrackingPatchResponseModel
            {
                IsSuccess = result > 0,
                Message = result > 0 ? "Successfully updated TeachingTracking" : "Failed to update TeachingTracking"
            };
        }
        catch (Exception ex)
        {
            return new TeachingTrackingPatchResponseModel
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }

    public async Task<TeachingTrackingDeleteResponseModel> DeleteTeachingTrackingAsync(TeachingTrackingDeleteRequestModel model)
    {
        try
        {
            var tracking = await _db.TblTeachTrackings
                .FirstOrDefaultAsync(x => !x.IsDelete && x.TrackingId == model.TrackId);

            if (tracking is null)
            {
                return new TeachingTrackingDeleteResponseModel
                {
                    IsSuccess = false,
                    Message = "TeachingTracking doesn't exist"
                };
            }

            tracking.IsDelete = true;
            tracking.ModifiedDateTime = DateTime.Now;
            int result = await _db.SaveChangesAsync();

            return new TeachingTrackingDeleteResponseModel
            {
                IsSuccess = result > 0,
                Message = result > 0 ? "Successfully deleted TeachingTracking" : "Failed to delete TeachingTracking"
            };
        }
        catch (Exception ex)
        {
            return new TeachingTrackingDeleteResponseModel
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }
}
