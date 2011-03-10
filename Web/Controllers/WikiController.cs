using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web.Mvc;

using AshMind.Extensions;

using AshMind.LightWiki.Domain;
using AshMind.LightWiki.Domain.Services;
using AshMind.LightWiki.Domain.Services.Syntax;
using AshMind.LightWiki.Infrastructure.Interfaces;
using AshMind.LightWiki.Web.Syntax;
using AshMind.LightWiki.Web.ViewModels;

namespace AshMind.LightWiki.Web.Controllers {
    [HandleError]
    public class WikiController : Controller {
        private readonly IRepository<WikiPage> repository;
        private readonly IWikiPageHierarchyProvider hierarchyProvider;
        private readonly IWikiSyntax syntax;
        private readonly HtmlWikiOutputFormat htmlWikiOutput;

        public WikiController(
            IRepository<WikiPage> repository,
            IWikiPageHierarchyProvider hierarchyProvider,
            IWikiSyntax syntax,
            HtmlWikiOutputFormat htmlWikiOutput
        ) {
            Contract.Requires<ArgumentNullException>(repository != null);
            Contract.Requires<ArgumentNullException>(hierarchyProvider != null);
            Contract.Requires<ArgumentNullException>(syntax != null);
            Contract.Requires<ArgumentNullException>(htmlWikiOutput != null);

            this.repository = repository;
            this.hierarchyProvider = hierarchyProvider;
            this.syntax = syntax;
            this.htmlWikiOutput = htmlWikiOutput;
        }

        public ActionResult Main(string slug) {
            var page = this.repository.Load(slug);
            if (page == null) {
                page = WikiPage.Create(slug, "Light wiki is light!");
                this.repository.Save(page);
            }

            var html = this.syntax.Convert(page.Text, htmlWikiOutput);
            var roots = this.repository.Query().ToList();
            roots.RemoveAll(roots.SelectMany(p => this.hierarchyProvider.GetSubPages(p)).ToArray());

            return View(new MainViewModel(
                new WikiPageViewModel(page, html),
                new WikiPageHierarchyModel(roots, this.hierarchyProvider.GetSubPages)
            ));
        }
    }
}
