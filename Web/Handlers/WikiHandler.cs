using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

using AspComet.Eventing;

using AshMind.Extensions;

using AshMind.LightWiki.Domain;
using AshMind.LightWiki.Domain.Services;
using AshMind.LightWiki.Infrastructure.Interfaces;

namespace AshMind.LightWiki.Web.Handlers {
    public class WikiHandler {
        #region DynamicDictionaryWrapper Class
        private class DynamicDictionaryWrapper : DynamicObject {
            private readonly IDictionary<string, object> dictionary;

            public DynamicDictionaryWrapper(IDictionary<string, object> dictionary) {
                this.dictionary = dictionary;
            }

            public override bool TryGetMember(GetMemberBinder binder, out object result) {
                return dictionary.TryGetValue(binder.Name, out result);
            }

            public override bool TrySetMember(SetMemberBinder binder, object value) {
                dictionary[binder.Name] = value;
                return true;
            }
        }
        #endregion

        private readonly IRepository<WikiPage> repository;
        private readonly WikiPageUpdater updater;

        public WikiHandler(
            IRepository<WikiPage> repository,
            WikiPageUpdater updater
        ) {
            this.repository = repository;
            this.updater = updater;
        }

        public void ProcessEvent(PublishingEvent @event) {
            dynamic data = new DynamicDictionaryWrapper((IDictionary<string, object>)@event.Message.data);
            if (data.action != "change")
                return;

            this.ProcessChange(@event.Channel, data);
        }

        private void ProcessChange(string channel, dynamic data) {
            var page = this.repository.Load(channel.SubstringAfterLast("/"));
            updater.Update(page, data.patch);            
        }
    }
}