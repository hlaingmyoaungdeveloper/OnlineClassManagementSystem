using Microsoft.AspNetCore.Mvc;
using OnlineClassManagementSystem.Domain.features.Enrollment;
using OnlineClassManagementSystem.Domain.models.Enrollment;
using System.Threading.Tasks;

namespace OnlineClassManagementSystem.MvcApp.Controllers
{
    public class EnrollmentController : Controller
    {
        private readonly EnrollmentService _enrollmentService;

        public EnrollmentController(EnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        // GET: /Enrollment/Index
        public async Task<IActionResult> Index(EnrollmentListRequestModel request)
        {
            var response = await _enrollmentService.GetEnrollmentsAsync(request);
            if (response.IsSuccess && response.EnrollmentList != null)
            {
                return View("EnrollmentList", response);
            }
            ViewBag.ErrorMessage = response.Message;
            return View("EnrollmentList", response);
        }

        // GET: /Enrollment/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View("EnrollmentCreate");
        }

        // POST: /Enrollment/Create
        [HttpPost]
        public async Task<IActionResult> Create(EnrollmentCreateRequestModel model)
        {
            var response = await _enrollmentService.CreateEnrollmentAsync(model);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = response.Message;
                return RedirectToAction("Index");
            }
            ViewBag.ErrorMessage = response.Message;
            return View("EnrollmentCreate", model);
        }

        // GET: /Enrollment/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var request = new EnrollmentEditRequestModel { EnrollmentId = id };
            var response = await _enrollmentService.GetEnrollmentAsync(request);
            if (!response.IsSuccess)
            {
                TempData["ErrorMessage"] = response.Message;
                return RedirectToAction("Index");
            }
            return View("EnrollmentEdit", response);
        }

        // POST: /Enrollment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EnrollmentPatchRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.EnrollmentId = id;
                return View("EnrollmentEdit", model);
            }
            var response = await _enrollmentService.PatchEnrollmentAsync(id, model);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = response.Message;
                return RedirectToAction("Index");
            }
            ViewBag.ErrorMessage = response.Message;
            ViewBag.EnrollmentId = id;
            return View("EnrollmentEdit", model);
        }

        // POST: /Enrollment/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var model = new EnrollmentUpdateStatusResquestModel { EnrollmentId = id, Status = "canceled" };
            var response = await _enrollmentService.UpdateEnrollmentStatusAsync(model);
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
