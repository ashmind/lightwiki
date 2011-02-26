using System;
using System.Collections.Generic;
using System.Linq;

using AspComet.Eventing;

using AshMind.LightWiki.Domain;
using AshMind.LightWiki.Domain.Services;
using AshMind.LightWiki.Infrastructure.Interfaces;

namespace AshMind.LightWiki.Web.Handlers {
    public class WikiHandler {
        private readonly IRepository<WikiPage> repository;
        private readonly WikiPageUpdater updater;

        public WikiHandler(
            IRepository<WikiPage> repository,
            WikiPageUpdater updater
        ) {
            this.repository = repository;
            this.updater = updater;
        }

        public void ProcessChange(PublishingEvent @event) {
            var data = (IDictionary<string, object>)@event.Message.data;
            var slug = (string)data["page"];

            var page = this.repository.Load(slug);
            var patch = (string)data["patch"];

            updater.Update(page, patch);
        }
    }
}