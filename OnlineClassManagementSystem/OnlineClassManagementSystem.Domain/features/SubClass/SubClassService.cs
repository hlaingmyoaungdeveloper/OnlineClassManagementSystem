using OnlineClassManagementSystem.Database.Models;
using OnlineClassManagementSystem.Domain.models;
using System;
using System.Collections.Generic;
using System.Linq;
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

    public SubClassListResponseModel GetSubClasses(SubClassListRequestModel model)
    {
       
        return new SubClassListResponseModel();
    }


}
