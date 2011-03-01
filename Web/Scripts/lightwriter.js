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
        this._cursor.element = $("<div class='lightwriter-cursor'></div>").appendTo(this._surface);

        var thatData = { that : this };
        $('c', this._surface[0]).live('click', thatData, this._surfaceCharacterClick);
        this._element.blur(thatData, this._elementBlur)
                     .keypress(thatData, this._elementKeypress)
                     .keyup(thatData, this._elementKeyup);
    }

    lightwriter.prototype = {
        _cursor : {
            elementBefore : function() {
                if (this.anchorLocation === 'before')
                    return this.anchor;

                return this.anchor.prev();
            },

            elementAfter : function() {
                if (this.anchorLocation === 'after')
                    return this.anchor;

                return this.anchor.next();
            }
        },

        keyHandlers : {
            /* backspace */ 8 : function() {
                var after = this._cursor.elementAfter();
                this._cursor.elementBefore().remove();
                if (after.length > 0) {
                    this._moveCursorBefore(after);
                }
                else {
                    this._moveCursorToTheEnd();
                }
            },

            /* left */ 37 : function() {
                this._moveCursorBefore(this._cursor.elementBefore());
            },

            /* right */ 39 : function() {
                this._moveCursorAfter(this._cursor.elementAfter());
            },

            /* delete */ 46 : function() {
                var before = this._cursor.elementBefore();
                this._cursor.elementAfter().remove();
                this._moveCursorAfter(before);
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
            var before = that._cursor.elementBefore();
            if (before.length > 0) {
                before.after(newCharacter);
            }
            else {
                that._cursor.elementAfter().before(newCharacter);
            }
            that._moveCursorAfter(newCharacter);
        },

        _surfaceCharacterClick : function(e) {
            var that = e.data.that;
            that.focus();
            
            var character = $(this);
            var position = character.position();
            var width = character.width();

            var x = e.pageX - that._surface.offset().left;

            if (x < position + (width / 2)) {
                that._moveCursorBefore(character);
            }
            else {
                that._moveCursorAfter(character);
            }
        },

        _updateCursorPosition : function() {
            this._moveCursorTo(this._cursor.anchor);
        },

        _moveCursorBefore : function(character) {
            this._moveCursorTo(character, 'after');
        },

        _moveCursorAfter : function(character) {
            this._moveCursorTo(character, 'before');
        },

        _moveCursorToTheEnd : function() {
            this._moveCursorAfter(this._surface.find('c:last'));
        },

        _moveCursorTo : function(character, characterLocation) {
            var position = character.position();
            var left;
            if (characterLocation === 'before') {
                var next = character.next();
                left = next.length > 0
                     ? next.position().left
                     : position.left + character.width();
            }
            else {
                var prev = character.prev();
                if (prev.length > 0) {
                    this._moveCursorTo(prev, 'before');
                    return;
                }

                left = position.left;                
            }

            this._cursor.element.css({
                top : position.top + 'px',
                left : left + 'px'
            });

            this._cursor.anchor = character;
            this._cursor.anchorLocation = characterLocation;
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