using Microsoft.AspNetCore.Mvc;
using OnlineClassManagementSystem.Domain.features.Timetable;
using OnlineClassManagementSystem.Domain.models.Timetable;
using System.Threading.Tasks;

namespace OnlineClassManagementSystem.MvcApp.Controllers
{
    public class ScheduleController : Controller
    {
        private readonly TimetableService _timetableService;

        public ScheduleController(TimetableService timetableService)
        {
            _timetableService = timetableService;
        }

        // GET: /Schedule/Index
        public async Task<IActionResult> Index(TimetableListResquestModel request)
        {
            var response = await _timetableService.GetTimetablesAsync(request);
            if (response.IsSuccess && response.TimetableList != null)
            {
                return View("ScheduleList", response);
            }
            ViewBag.ErrorMessage = response.Message;
            return View("ScheduleList", response);
        }

        // GET: /Schedule/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View("ScheduleCreate");
        }

        // POST: /Schedule/Create
        [HttpPost]
        public async Task<IActionResult> Create(TimetableCreateRequestModel model)
        {
            var response = await _timetableService.CreateTimetableAsync(model);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = response.Message;
                return RedirectToAction("Index");
            }
            ViewBag.ErrorMessage = response.Message;
            return View("ScheduleCreate", model);
        }

        // GET: /Schedule/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var request = new TimetableListByClassRequest { SubClassId = id };
            var response = await _timetableService.GetTimetablesByClassAsync(request);
            if (!response.IsSuccess)
            {
                TempData["ErrorMessage"] = response.Message;
                return RedirectToAction("Index");
            }
            // Assuming we edit the first matching schedule
            var schedule = response.TimetableList?[0];
            return View("ScheduleEdit", schedule);
        }

        // POST: /Schedule/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TimetablePatchRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ScheduleId = id;
                return View("ScheduleEdit", model);
            }
            var response = await _timetableService.PatchTimetableAsync(id, model);
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = response.Message;
                return RedirectToAction("Index");
            }
            ViewBag.ErrorMessage = response.Message;
            ViewBag.ScheduleId = id;
            return View("ScheduleEdit", model);
        }

        // POST: /Schedule/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var model = new TimetableDeleteRequestModel { TimetableId = id };
            var response = await _timetableService.DeleteTimetableAsync(model);
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
