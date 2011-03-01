(function($) {   
    $.fn.lightwriter = function() {
        this.each(function() {
            new lightwriter(this);
        });
        return this;
    };
    
    function lightwriter(element) {
        this._element = $(element);
        this._surface = this._overlay(this._element)
                            .addClass('lightwriter')
                            .html(this._addMarkers(this._element.val()));

        var that = this;
        this._cursor = {
            element: $("<div class='lightwriter-cursor'></div>").appendTo(this._surface)
        };

        var thatData = { that : this };
        $('c', this._surface[0]).live('click', thatData, this._surfaceCharacterClick);
        this._element.blur(thatData, this._elementBlur)
                     .keypress(thatData, this._elementKeypress)
                     .keyup(thatData, this._elementKeyup);
    }

    lightwriter.prototype = {
        keyHandlers : {
            /* backspace */ 8 : function() {
                this._cursor.anchor.prev().remove();
                this._updateCursorPosition();
            },

            /* left */ 37 : function() {
                this._moveCursorTo(this._cursor.anchor.prev());
            },

            /* right */ 39 : function() {
                this._moveCursorTo(this._cursor.anchor.next());
            }
        },

        _focused : false,

        focus : function() {
            this._focused = true;
            this._element.focus();

            var cursor = this._cursor;
            cursor.blinker = window.setInterval(function() {
                if (!cursor.moved)
                    cursor.element.toggle();
                cursor.moved = false;
            }, 600);
        },

        _elementBlur : function(e) {
            var that = e.data.that;
            that._focused = false;
            window.clearInterval(that._cursor.blinker);
        },

        _elementKeyup : function(e) {
            /*var that = e.data.that;
            if (!that._focused)
                return;

            var handler = that.keyHandlers[e.which];
                        
            e.preventDefault();
            if (handler)
                handler.call(that);*/
        },

        _elementKeypress : function(e) {
            var that = e.data.that;
            if (!that._focused)
                return;
            
            e.preventDefault();
            var handler = that.keyHandlers[e.keyCode];
            if (handler) {
                handler.call(that);
                return;
            }

            var newCharacter = $('<c>' + String.fromCharCode(e.which) + '</c>');
            that._cursor.anchor.before(newCharacter);
            that._updateCursorPosition();
        },

        _surfaceCharacterClick : function(e) {
            var that = e.data.that;
            that.focus();
            
            var character = $(this);
            var position = character.position();
            var width = character.width();

            var x = e.pageX - that._surface.offset().left;

            var anchor = (x < position + (width / 2))
                       ? character
                       : character.next();

            that._moveCursorTo(anchor);
        },

        _updateCursorPosition : function() {
            this._moveCursorTo(this._cursor.anchor);
        },

        _moveCursorTo : function(character) {
            var position = character.position();
            this._cursor.element.css({
                top : position.top + 'px',
                left : position.left + 'px'
            });

            this._cursor.anchor = character;
            this._cursor.moved = true;
            this._cursor.element.show();
        },
    
        _addMarkers : function(text) {
            var result = [];
            result.push("<p>");
            for (var i = 0; i < text.length; i++) {
                var c = text.charAt(i);
            
                result.push("<c>");
                result.push(c !== ' ' ? c : '&nbsp;');
                result.push("</c>");
            
                if (c === '\n' || (c === '\r' && text.charAt(i + 1) !== '\n')) {
                    result.push("</p><p>");
                }
            }
            result.push("</p>");
        
            return result.join('');
        },
    
        _overlay : function (target) {
            var surface = $('<div></div>');
            var position = target.position();
            return surface.css({
                position: 'absolute',
                left:     position.left + 'px',
                top:      position.top + 100 + 'px',
                width:    target.width() + 'px',
                height:   target.height() + 'px'
            }).appendTo(target.parent());
        }
    };
})(jQuery);