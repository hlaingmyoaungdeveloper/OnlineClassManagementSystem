using Microsoft.AspNetCore.Mvc;
using OnlineClassManagementSystem.Domain.features.SubClass;
using OnlineClassManagementSystem.Domain.models.SubClass;
using System.Threading.Tasks;

namespace OnlineClassManagementSystem.MvcApp.Controllers
{
    public class SubClassController : Controller
    {
        private readonly SubClassService _subClassService;

        public SubClassController(SubClassService subClassService)
        {
            _subClassService = subClassService;
        }

        [ActionName("Index")]
        public async Task<IActionResult> SubClassList(SubClassListRequestModel requestModel)
        {

            var response = await _subClassService.GetSubClassesAsync(requestModel);

            if (response.IsSuccess && response.SubClassList != null)
            {
                return View("SubClassList",response);
            }

            ViewBag.ErrorMessage = response.Message;
            return View("SubClassList",response);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View("SubClassCreate");
        }

        [HttpPost]
        public async Task<IActionResult> Create(SubClassCreateRequestModel requestModel)
        {

            var response = await _subClassService.CreateSubClassAsync(requestModel);

            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = response.Message;
                return RedirectToAction("Index");
            }

            ViewBag.ErrorMessage = response.Message;
            return View(requestModel);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var request = new SubClassEditRequestModel { SubClassId = id };
            var response = await _subClassService.GetSubClassAsync(request);

            if (!response.IsSuccess)
            {
                TempData["ErrorMessage"] = response.Message;
                return RedirectToAction("Index");
            }

            var editModel = new SubClassPatchRequestModel
            {
                ClassName = response.ClassName,
                Place = response.Place,
                OpenDate = response.OpenDate,
                OpenTime = response.OpenTime,
                StudentLimit = response.StudentLimit
            };

            ViewBag.SubClassId = id;

            return View("subclassedit", editModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SubClassPatchRequestModel requestModel)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.SubClassId = id;

                return View("subclassedit", requestModel);
            }

            var response = await _subClassService.PatchSubClassAsync(id, requestModel);

            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = response.Message;
                return RedirectToAction("Index");
            }

            ViewBag.ErrorMessage = response.Message;
            ViewBag.SubClassId = id;

            return View("subclassedit", requestModel);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var request = new SubClassDeleteRequestModel { SubClassId = id };
            var response = await _subClassService.DeleteSubClassAsync(request);

            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = response.Message;
            }
            else
            {
                TempData["ErrorMessage"] = response.Message;
            }

            return RedirectToAction("Index");
        }
    }
}