var lightwiki = {
    _connected : false,
    _subscription : null,
    _settings : null,
    
    _server : {
        text :    '',
        revision: 0,
        pending : false
    },
    
    _log : function() {
        if (console)
            console.log.apply(console, arguments);
    },
    
    setup : function(settings) {
        settings.revisionContainer = $(settings.revisionContainer);
        
        this._settings = settings;
        this._editor = settings.editor;
        this._patcher = new diff_match_patch();

        this._server.text = settings.editor.getText();
        this._server.revision = settings.serverRevision;
        
        $.cometd.configure({ url: settings.cometUrl });
        $.cometd.addListener('/meta/connect', this, this['/meta/connect']);
        $.cometd.addListener('/meta/disconnect', this, this['/meta/disconnect']);

        $.cometd.handshake();
    },
    
    save : function() {
        var text = this._editor.getText();
        if (text === this._server.text)
            return;
        
        this._settings.revisionContainer.text(this._server.revision + '+');
        this._sendChanges(text);
    },
    
    _sendChanges : function(text) {
        if (!this._connected || this._server.pending) {
            return;
        }

        var last = this._server.text;
        var patch = this._patcher.patch_toText(
            this._patcher.patch_make(last, text)
        );
        
        $('body').addClass('sending');
        $.cometd.publish('/wiki/' + this._settings.page + '/change', {
            revision: this._server.revision,
            patch:    patch
        });
        this._log("Local sent patch for ", this._server.revision, ":", patch, this._getTextStateForLog(text));
        this._server.pending = { text : text };    
    },
    
    '/meta/connect' : function(message) {
        var wasConnected = this._connected;
        this._connected = message.successful;

        if (this._connected && !wasConnected) {
            $('body').addClass('connected');

            $.cometd.batch(this, function() {
                this._unsubscribe();
                this._subscription = $.cometd.subscribe(
                    '/wiki/' + this._settings.page + '/sync',
                    this, this['/wiki/?/sync']
                );
                $.cometd.publish();
            });
        }
        else if (wasConnected && !this._connected) {
            $('body').removeClass('connected');
            this._unsubscribe();
        }
    },
    
    '/wiki/?/sync' : function(message) {
        var revision = message.data.revision;
        if (message.data.patch) {
            this._log(
                "Server ",
                (message.data.isreply ? "replied with" : "sent"),
                " patch ",
                revision.from, "=>", revision.to,
                ":", message.data.patch
            );
        }
        else if (message.data.isreply) {
            this._log("Server confirmed change ", revision.from, "=>", revision.to);
        }
        else {
            this._log("Invalid message ", revision.from, "=>", revision.to);
        }
        
        if (revision.from != this._server.revision) {
            this._log("Local ignored changes for ", revision.from, " since it has ", this._server.revision);
            
            if (revision.from > this._server.revision) {
                $.cometd.publish('/wiki/' + this._settings.page + '/resync', {
                    revision: this._server.revision
                });
                this._server.pending = true;
                this._log("Local sent resync request for ", this._server.revision);
            }

            return;
        }
        
        if (message.data.patch) {
            if (this._server.pending && !message.data.isreply) {
                this._log("Local ignored changes for ", revision.from, " since it is waiting for reply.");
                return;
            }
            
            var current = this._editor.getText();

            var patch = this._patcher.patch_fromText(message.data.patch);
            var newServerText = this._patcher.patch_apply(patch, this._server.text)[0];

            var patchSinceThisRevision = this._patcher.patch_make(this._server.text, current);
        
            var patched = this._patcher.patch_apply(patchSinceThisRevision, newServerText)[0];
            if (patched !== current)
                this._editor.setText(patched);
        
            this._server.text = newServerText;
            this._log("Local merged changes ", revision.from, "=>", revision.to, this._getTextStateForLog(patched));
        }
        else if (message.data.isreply) {
            this._server.text = this._server.pending.text;
            this._log("Local accepted changes ", revision.from, "=>", revision.to, { serverText : this._server.text });
        }
        else {
            throw "Invalid message"; 
        }

        this._settings.viewer.setHtml(message.data.html);
        this._server.revision = revision.to;
        this._settings.revisionContainer.text(this._server.revision);
        if (message.data.isreply)
            this._clearPending();
    },
    
    '/meta/disconnect' : function (message) {
        if (!message.successful)
            return;
            
        this._connected = false;
        this._unsubscribe();
    },
    
    _getTextStateForLog : function(clientText) {
        return { serverText : this._server.text, clientText : clientText };
    },
    
    _clearPending : function () {
        this._server.pending = false;
        $('body').removeClass('sending');
    },
    
    _unsubscribe : function() {
        if (this._subscription)
            $.cometd.unsubscribe(this._subscription);

        this._subscription = null;
    }
};