var lightwiki = {
    _connected : false,
    _subscription : null,
    _settings : null,
    
    _server : {
        text : '',
        revision: 0
    },
    _sent : {
        text : ''
    },
    
    setup : function(settings) {
        settings.syncInterval = settings.syncInterval || 1000;
        settings.editor = $(settings.editor);
        settings.revisionContainer = $(settings.revisionContainer);
        
        this._settings = settings;
        this._editor = settings.editor;
        this._patcher = new diff_match_patch();

        this._server.text = settings.editor.html();
        this._server.revision = settings.serverRevision;
        this._sent.text = this._server.text;
        
        $.cometd.configure({ url: settings.cometUrl });
        $.cometd.addListener('/meta/connect', this, this['/meta/connect']);
        $.cometd.addListener('/meta/disconnect', this, this['/meta/disconnect']);

        $.cometd.handshake();

        var that = this;
        window.setInterval(function() { that._timer(); }, settings.syncInterval);
    },
    
    _timer : function() {
        var html = this._editor.html();
        if (html === this._sent.text)
            return;
        
        this._settings.revisionContainer.text(this._server.revision + '+');

        if (!this._connected)
            return;

        var last = this._server.text;
        var patch = this._patcher.patch_toText(
            this._patcher.patch_make(last, html)
        );
        
        $('body').addClass('sending');
        $.cometd.publish('/wiki/' + this._settings.page + '/change', {
            revision: this._server.revision,
            patch:    patch
        });
        this._sent.text = html;

        window.setTimeout(
            function() { $('body').removeClass('sending'); },
            500
        );
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
        var patch = this._patcher.patch_fromText(message.data.patch);
        this._server.text = this._patcher.patch_apply(patch, this._server.text)[0];
        this._server.revision = message.data.revision;
        this._sent.text = this._server.text;
        
        var current = this._editor.html();
        var patched = this._patcher.patch_apply(patch, current)[0];
        if (patched !== current)
            this._editor.html(patched);
        
        this._settings.revisionContainer.text(this._server.revision);
    },
    
    '/meta/disconnect' : function (message) {
        if (!message.successful)
            return;
            
        this._connected = false;
        this._unsubscribe();
    },
    
    _unsubscribe : function() {
        if (this._subscription)
            $.cometd.unsubscribe(this._subscription);

        this._subscription = null;
    }
};