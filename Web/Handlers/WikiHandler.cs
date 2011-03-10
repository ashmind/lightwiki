using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Dynamic;
using System.Linq;
using AshMind.LightWiki.Domain.Services.Concurrency;
using AspComet;
using AspComet.Eventing;

using AshMind.Extensions;

using AshMind.LightWiki.Domain;
using AshMind.LightWiki.Domain.Services;
using AshMind.LightWiki.Domain.Services.Syntax;
using AshMind.LightWiki.Web.Syntax;
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
        private readonly IWikiSyntax syntax;
        private readonly HtmlWikiOutputFormat htmlWikiOutput;

        public WikiHandler(
            IClientRepository clientRepository,
            IRepository<WikiPage> repository,
            WikiPageUpdater updater,
            IWikiSyntax syntax,
            HtmlWikiOutputFormat htmlWikiOutput
        ) {
            this.clientRepository = clientRepository;
            this.repository = repository;
            this.updater = updater;
            this.syntax = syntax;
            this.htmlWikiOutput = htmlWikiOutput;
        }

        public void ProcessEvent(PublishingEvent @event) {
            var action = @event.Channel.SubstringAfterLast("/");
            dynamic data = new DynamicDictionaryWrapper((IDictionary<string, object>)@event.Message.data);
            var channelPrefix = @event.Channel.SubstringBeforeLast("/");
            var page = this.repository.Load(channelPrefix.SubstringAfterLast("/"));

            if (action == "change") {
                this.ProcessChange(page, channelPrefix, @event.Message.clientId, data);
            }
            else if (action == "resync") {
                this.ProcessResync(page, channelPrefix, @event.Message.clientId, data);
            }
        }

        private void ProcessResync(WikiPage page, string channelPrefix, string clientId, dynamic data) {
            var sync = updater.GetUpdate(page, (int)data.revision);
            var author = this.clientRepository.GetByID(clientId);
            this.SendSyncMessage(
                new[] { author },
                channelPrefix + "/sync", true, (int)data.revision, sync.ResultingRevision, sync.Patch
            );
        }

        private void ProcessChange(WikiPage page, string channelPrefix, string clientId, dynamic data) {
            var authorRevision = (int)data.revision;
            var result = updater.ApplyUpdate(page, authorRevision, (string)data.patch);

            var syncChannel = channelPrefix + "/sync";

            var author = this.clientRepository.GetByID(clientId);
            this.SendSyncMessage(
                new[] {author},
                syncChannel, true, authorRevision, result.ResultingRevision, result.PatchForAuthor
            );

            if (!result.PatchForOthers.Any())
                return;

            this.SendSyncMessage(
                this.clientRepository.WhereSubscribedTo(syncChannel).Except(author),
                syncChannel, false, result.ResultingRevision.Number - 1, result.ResultingRevision, result.PatchForOthers
            );
        }

        private void SendSyncMessage(
            IEnumerable<IClient> clients,
            string channel, bool isReply,
            int fromRevisionNumber, WikiPageRevision toRevision,
            string patch
        ) {
            Contract.Requires<ArgumentException>(isReply || !string.IsNullOrEmpty(patch));

            var html = this.syntax.Convert(toRevision.Text, this.htmlWikiOutput);
            var message = new Message {
                channel = channel,
                data = new {
                    isreply = isReply,
                    revision = new {
                        from = fromRevisionNumber,
                        to = toRevision.Number
                    },
                    patch, html
                }
            };

            foreach (var client in clients) {
                client.Enqueue(message);
                client.FlushQueue();
            }
        }
    }
}