using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics.Contracts;

using AshMind.Web.Mvc.KeyModel;

using AshMind.LightWiki.Domain;
using AshMind.LightWiki.Infrastructure.Interfaces;

namespace AshMind.LightWiki.Web.Controllers {
    [HandleError]
    public class WikiController : Controller {
        private readonly IRepository<WikiPage> repository;

        public WikiController(IRepository<WikiPage> repository) {
            this.repository = repository;
        }

        public new ActionResult View(string slug) {
            var page = this.repository.Load(slug) ?? new WikiPage {
                Slug = slug,
                Text = "Light wiki is light!"
            };

            return View(page);
        }
    }
}
