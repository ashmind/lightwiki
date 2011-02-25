using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web.Mvc;

using AshMind.LightWiki.Domain;
using AshMind.LightWiki.Infrastructure.Interfaces;
using AshMind.LightWiki.Web.ViewModels;

namespace AshMind.LightWiki.Web.Controllers {
    [HandleError]
    public class WikiController : Controller {
        private readonly IRepository<WikiPage> repository;

        public WikiController(IRepository<WikiPage> repository) {
            Contract.Requires<ArgumentNullException>(repository != null);

            this.repository = repository;
        }

        public ActionResult Main(string slug) {
            var page = this.repository.Load(slug);
            if (page == null) {
                page = new WikiPage {
                    Slug = slug,
                    Text = "Light wiki is light!"
                };
                this.repository.Save(page);
            }

            return View(new MainViewModel(
                page,
                this.repository.Query().Select(p => p.Slug).ToArray()
            ));
        }
    }
}
