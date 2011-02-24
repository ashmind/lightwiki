using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics.Contracts;

using AshMind.Web.Mvc.KeyModel;

using AshMind.LightWiki.Domain;

namespace AshMind.LightWiki.Web.Controllers {
    [HandleError]
    public class WikiController : Controller {
        public new ActionResult View(string slug) {
            var page = new WikiPage {
                Slug = slug,
                Text = "Welcome to LightWiki!"
            };

            return View(page);
        }
    }
}
