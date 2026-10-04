using Microsoft.EntityFrameworkCore;
using OnlineClassManagementSystem.Database.Models;
using OnlineClassManagementSystem.Domain.models.TeachingTracking;
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
            var query = _db.TblTeachingTrackings
                .AsNoTracking()
                .Include(x => x.SubClass)
                .Include(x => x.Teacher)
                .AsQueryable();

            if (model.SubClassId.HasValue && model.SubClassId.Value > 0)
            {
                query = query.Where(x => x.SubClassId == model.SubClassId.Value);
            }

            if (model.TeacherId.HasValue && model.TeacherId.Value > 0)
            {
                query = query.Where(x => x.TeacherId == model.TeacherId.Value);
            }

            List<TeachingTrackingModel> trackingList = await query
                .Select(x => new TeachingTrackingModel
                {
                    TrackId = x.TrackId,
                    SubClassId = x.SubClassId,
                    ClassName = x.SubClass != null ? x.SubClass.ClassName : null,
                    TeacherId = x.TeacherId,
                    TeacherName = x.Teacher != null ? x.Teacher.Username : null,
                    TopicTaught = x.TopicTaught,
                    DateTaught = x.DateTaught,
                    Duration = x.Duration,
                    Remarks = x.Reamrks,
                    CreatedDateTime = x.CreatedDateTime,
                    ModifiedDateTime = x.ModifiedDateTime,
                    CreatedBy = x.CreatedBy,
                    ModifiedBy = x.ModifiedBy
                })
                .ToListAsync();

            return new TeachingTrackingListResponseModel
            {
                IsSuccess = true,
                Message = "Teaching tracking records fetched successfully",
                TeachingTrackingList = trackingList
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
            var tracking = await _db.TblTeachingTrackings
                .AsNoTracking()
                .Include(x => x.SubClass)
                .Include(x => x.Teacher)
                .FirstOrDefaultAsync(x => x.TrackId == model.TrackId);

            if (tracking is null)
            {
                return new TeachingTrackingEditResponseModel
                {
                    IsSuccess = false,
                    Message = "Teaching tracking record is not found"
                };
            }

            return new TeachingTrackingEditResponseModel
            {
                IsSuccess = true,
                Message = "Teaching tracking fetched successfully",
                TrackId = tracking.TrackId,
                SubClassId = tracking.SubClassId,
                ClassName = tracking.SubClass?.ClassName,
                TeacherId = tracking.TeacherId,
                TeacherName = tracking.Teacher?.Username,
                TopicTaught = tracking.TopicTaught,
                DateTaught = tracking.DateTaught,
                Duration = tracking.Duration,
                Remarks = tracking.Reamrks,
                CreatedDateTime = tracking.CreatedDateTime,
                ModifiedDateTime = tracking.ModifiedDateTime,
                CreatedBy = tracking.CreatedBy,
                ModifiedBy = tracking.ModifiedBy
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

        if (string.IsNullOrWhiteSpace(model.TopicTaught))
        {
            return new TeachingTrackingCreateResponseModel
            {
                IsSuccess = false,
                Message = "TopicTaught is required"
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

            TblTeachingTracking tracking = new()
            {
                SubClassId = model.SubClassId,
                TeacherId = model.TeacherId,
                TopicTaught = model.TopicTaught,
                DateTaught = model.DateTaught == default ? DateTime.Now : model.DateTaught,
                Duration = model.Duration,
                Reamrks = model.Remarks,
                CreatedDateTime = DateTime.Now,
                CreatedBy = model.CreatedBy
            };

            _db.TblTeachingTrackings.Add(tracking);
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
            var tracking = await _db.TblTeachingTrackings
                .FirstOrDefaultAsync(x => x.TrackId == id);

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

            if (!string.IsNullOrWhiteSpace(model.TopicTaught))
            {
                tracking.TopicTaught = model.TopicTaught;
            }

            if (model.DateTaught.HasValue && model.DateTaught.Value != default)
            {
                tracking.DateTaught = model.DateTaught.Value;
            }

            if (model.Duration.HasValue)
            {
                tracking.Duration = model.Duration.Value;
            }

            if (model.Remarks is not null)
            {
                tracking.Reamrks = model.Remarks;
            }

            if (model.ModifiedBy.HasValue)
            {
                tracking.ModifiedBy = model.ModifiedBy.Value;
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
            var tracking = await _db.TblTeachingTrackings
                .FirstOrDefaultAsync(x => x.TrackId == model.TrackId);

            if (tracking is null)
            {
                return new TeachingTrackingDeleteResponseModel
                {
                    IsSuccess = false,
                    Message = "TeachingTracking doesn't exist"
                };
            }

            _db.TblTeachingTrackings.Remove(tracking);
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
