var lightwiki = {
    _connected : false,
    _subscription : null,
    _settings : null,
    
    setup : function(settings) {
        settings.throttleSend = settings.throttleSend || 1000;
        settings.editor = $(settings.editor);
        
        this._settings = settings;
        
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

        $.cometd.publish('/wiki/change', { 
            page: that._settings.page,
            message: $(this).html()
        });
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

        if (comet.clientId == $.cometd.getClientId())
            return;

        var editor = this._settings.editor;
        if (editor.html() != comet.data.message)
            editor.html(comet.data.message);
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