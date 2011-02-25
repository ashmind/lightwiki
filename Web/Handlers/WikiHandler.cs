using System;
using System.Collections.Generic;
using System.Linq;

using AspComet.Eventing;

using AshMind.LightWiki.Domain;
using AshMind.LightWiki.Infrastructure.Interfaces;

namespace AshMind.LightWiki.Web.Handlers {
    public class WikiHandler {
        private readonly IRepository<WikiPage> repository;

        public WikiHandler(IRepository<WikiPage> repository) {
            this.repository = repository;
        }

        public void ProcessChange(PublishingEvent @event) {
            var data = (IDictionary<string, object>)@event.Message.data;
            this.repository.Save(new WikiPage {
                Slug = (string)data["page"],
                Text = (string)data["message"]
            });
        }
    }
}