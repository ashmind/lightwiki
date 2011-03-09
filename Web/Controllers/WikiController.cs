using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web.Mvc;

using AshMind.LightWiki.Domain;
using AshMind.LightWiki.Domain.Services.Syntax;
using AshMind.LightWiki.Infrastructure.Interfaces;
using AshMind.LightWiki.Web.Syntax;
using AshMind.LightWiki.Web.ViewModels;

namespace AshMind.LightWiki.Web.Controllers {
    [HandleError]
    public class WikiController : Controller {
        private readonly IRepository<WikiPage> repository;
        private readonly WikiSyntaxTransformer syntax;
        private readonly HtmlWikiOutputFormat htmlWikiOutput;

        public WikiController(
            IRepository<WikiPage> repository,
            WikiSyntaxTransformer syntax,
            HtmlWikiOutputFormat htmlWikiOutput
        ) {
            Contract.Requires<ArgumentNullException>(repository != null);
            Contract.Requires<ArgumentNullException>(syntax != null);
            Contract.Requires<ArgumentNullException>(htmlWikiOutput != null);

            this.repository = repository;
            this.syntax = syntax;
            this.htmlWikiOutput = htmlWikiOutput;
        }

        public ActionResult Main(string slug) {
            var page = this.repository.Load(slug);
            if (page == null) {
                page = WikiPage.Create(slug, "Light wiki is light!");
                this.repository.Save(page);
            }

            var html = this.syntax.Transform(page.Text, htmlWikiOutput);
            return View(new MainViewModel(
                new WikiPageViewModel(page, html),
                this.repository.Query()
            ));
        }
    }
}
