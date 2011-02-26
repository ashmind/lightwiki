var lightwiki = {
    _connected : false,
    _subscription : null,
    _settings : null,
    
    setup : function(settings) {
        settings.throttleSend = settings.throttleSend || 1000;
        settings.editor = $(settings.editor);
        
        this._settings = settings;
        this._patcher = new diff_match_patch();

        this._lastSyncData = settings.editor.html();
        
        $.cometd.configure({ url: settings.cometUrl });
        $.cometd.addListener('/meta/connect', this, this['/meta/connect']);
        $.cometd.addListener('/meta/disconnect', this, this['/meta/disconnect']);

        $.cometd.handshake();
        
        settings.editor.keyup({ that : this }, $.throttle(
            settings.throttleSend, this['editor.keyup']
        ));
    },
    
    'editor.keyup' : function(e) {
        var that = e.data.that;
        if (!that._connected)
            return;

        var html = $(this).html();
        var last = that._lastSyncData;

        var patch = that._patcher.patch_toText(
            that._patcher.patch_make(last, html)
        );

        $('body').addClass('sending');
        $.cometd.publish('/wiki/change', {
            by:     that._settings.unique,
            page:   that._settings.page,
            patch:  patch
        });

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

            this._unsubscribe();
            this._subscription = $.cometd.subscribe('/wiki/change', this, this['/wiki/change']);
        }
        else if (wasConnected && !this._connected) {
            $('body').removeClass('connected');
            this._unsubscribe();
        }
    },
    
    '/wiki/change' : function(comet) {
        if (comet.data.page != this._settings.page)
            return;
        
        var patch = this._patcher.patch_fromText(comet.data.patch);
        this._lastSyncData = this._patcher.patch_apply(patch, this._lastSyncData)[0];

        if (comet.data.by == this._settings.unique)
            return;
        
        var editor = this._settings.editor;
        var html = editor.html();
        
        var patched = this._patcher.patch_apply(patch, html)[0];
            
        if (html != patched)
            editor.html(patched);
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