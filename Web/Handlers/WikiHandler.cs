using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using AspComet;
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

        private readonly IClientRepository clientRepository;
        private readonly IRepository<WikiPage> repository;
        private readonly WikiPageUpdater updater;

        public WikiHandler(
            IClientRepository clientRepository,
            IRepository<WikiPage> repository,
            WikiPageUpdater updater
        ) {
            this.clientRepository = clientRepository;
            this.repository = repository;
            this.updater = updater;
        }

        public void ProcessEvent(PublishingEvent @event) {
            var action = @event.Channel.SubstringAfterLast("/");
            dynamic data = new DynamicDictionaryWrapper((IDictionary<string, object>)@event.Message.data);
            if (action != "change")
                return;

            this.ProcessChange(@event.Channel.SubstringBeforeLast("/"), data);
        }

        private void ProcessChange(string channelPrefix, dynamic data) {
            var page = this.repository.Load(channelPrefix.SubstringAfterLast("/"));
            var result = updater.Update(page, (int)data.revision, (string)data.patch);

            var syncChannel = channelPrefix + "/sync";
            var clients = this.clientRepository.WhereSubscribedTo(syncChannel);
            var message = new Message {
                channel = channelPrefix + "/sync",
                data = new {
                    revision = result.RevisionNumber,
                    patch = result.PatchToRevision
                }
            };
            foreach (var client in clients) {
                client.Enqueue(message);
                client.FlushQueue();
            }
        }
    }
}