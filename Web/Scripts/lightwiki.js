var lightwiki = {
    _connected : false,
    _subscription : null,
    _settings : null,
    
    _last : {
        text : '',
        revision: 0
    },
    _pendingPatches : {},
    
    setup : function(settings) {
        settings.throttleSend = settings.throttleSend || 1000;
        settings.editor = $(settings.editor);
        settings.revisionContainer = $(settings.revisionContainer);
        
        this._settings = settings;
        this._patcher = new diff_match_patch();

        this._last.text = settings.editor.html();
        
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
        that._settings.revisionContainer.text(that._last.revision + '+');

        if (!that._connected)
            return;

        var html = $(this).html();
        var last = that._last.text;

        var patch = that._patcher.patch_toText(
            that._patcher.patch_make(last, html)
        );
        
        $('body').addClass('sending');
        $.cometd.publish('/wiki/' + that._settings.page + '/change', {
            revision: that._last.revision,
            patch:    patch
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
        if (this._last.revision === message.data.revision - 1) {
            var editor = this._settings.editor;
            var html = editor.html();
            
            var patched = this._patcher.patch_apply(patch, html)[0];
            var revision = message.data.revision;

            patch = this._pendingPatches[revision];
            while (patch) {
                patched = this._patcher.patch_apply(patch, patched)[0];
                delete this._pendingPatches[revision];
                
                revision += 1;
                patch = this._pendingPatches[revision];
            }

            this._last.text = patched;
            this._last.revision = revision;
            if (html !== patched)
                editor.html(patched);
            
            this._settings.revisionContainer.text(revision);
        }
        else if (this._last.revision < message.data.revision - 1) {
            this._pendingPatches[message.data.revision] = patch;
        }
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