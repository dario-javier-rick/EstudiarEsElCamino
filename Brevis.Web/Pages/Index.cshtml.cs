﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Brevis.Core;
using Brevis.Core.Models;
using Brevis.Web.Controllers;
using Brevis.Web.Views;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static Brevis.Web.ServiceResolver;

namespace Brevis.Web.Pages
{
    public class IndexModel : PageModel
    {
        public List<Tuple<string, string>> _progressCarreerTransformerImplementations = new List<Tuple<string, string>>();
        public string selectedProgressCarreer;

        public IProgressCarreerTransformer _progressCarreerTransformer;

        ProgressCarreerTransformerResolver _progressCarreerTransformerResolver;
        public IndexModel(ProgressCarreerTransformerResolver progressCarreerTransformerResolver)
        {
            _progressCarreerTransformerResolver = progressCarreerTransformerResolver;
        }

        public void OnGet()
        {
            _progressCarreerTransformerImplementations = Startup._progressCarreerTransformerImplementations;
        }


        [BindProperty]
        public IFormFile File { get; set; }
        public async Task<IActionResult> OnPostAsync(string selectedProgressCarreer)
        {
            //Getting file meta data
            var fileName = Path.GetFileName(File.FileName);
            var contentType = File.ContentType;

            // MVC Pattern
            var model = new StudyPath();
            var view = new StudyPathView(model);
            var controller = new StudyPathController(view);

            model.AttachObserver(controller);
            //model.AttachObserver(view);

            _progressCarreerTransformer = _progressCarreerTransformerResolver(selectedProgressCarreer); //Pass XLS, CSV, etc.
            var progressCarrer = _progressCarreerTransformer.Transform(File);

            // to do : return something
            //var subjects = controller.GetCriticalStudyPath(progressCarrer);
            var subjects = new List<Subject> { new Subject { Code = "A" } };

            model.Notify();

            //return new JsonResult(subjects);

            return RedirectToPage("Tree", new { subjects = subjects });
        }
    }
}
