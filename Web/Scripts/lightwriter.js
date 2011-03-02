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

        this._cursor = new lightwriter.cursor(this._surface);
        
        var thatData = { that : this };
        $('c', this._surface[0]).live('click', thatData, this._surfaceCharacterClick);
        this._surface.click(function(e) {
            e.preventDefault();
        });
        this._element.blur(thatData, this._elementBlur)
                     .keypress(thatData, this._elementKeypress)
                     .keyup(thatData, this._elementKeyup);
    }

    lightwriter.cursor = function(surface) {
        this._surface = surface;
        this.element = $("<div class='lightwriter-cursor'></div>").appendTo(surface).hide();

        var that = this;
        this.blinker = window.setInterval(function() {
            if (!that.moved && that.visible)
                that.element.toggle();
                
            that.moved = false;
        }, 600);
    };

    lightwriter.cursor.prototype = {
        elementBefore : function() {
            if (this.anchorLocation === 'before')
                return this.anchor;

            return this.anchor.prev();
        },

        elementAfter : function() {
            if (this.anchorLocation === 'after')
                return this.anchor;

            return this.anchor.next();
        },
        
        moveForward : function() {
            this.moveAfter(this.elementAfter());
        },

        moveBefore : function(character) {
            this._moveTo('before', character);
        },

        moveAfter : function(character) {
            this._moveTo('after', character);
        },
        
        moveToTheStart: function() {
            this.moveBefore(this._surface.find(':first'));
        },

        moveToTheEnd : function() {
            this.moveAfter(this._surface.find(':last'));
        },

        _moveTo : function(cursorLocation, character) {
            var position = {};
            if (cursorLocation === 'after') {
                var next = character.next('c');
                if (next.length > 0) {
                    position = next.position();
                }
                else {
                    position = character.position();
                    position.left += character.width();
                } 
            }
            else {
                var prev = character.prev('c');
                if (prev.length > 0) {
                    this.moveAfter(prev);
                    return;
                }

                position = character.position();
            }

            this.element.css({
                top : position.top + 'px',
                left : position.left + 'px'
            });

            this.anchor = character;
            this.anchorLocation = (cursorLocation === 'after') ? 'before' : 'after';
            this.moved = true;
            this.element.show();            
        },
        
        hide : function () {
            this.visible = false;
            this.element.hide();
        },
        
        show : function () {
            this.visible = true;
        }
    };

    lightwriter.prototype = {
        keyHandlers : {
            /* backspace */ 8 : function() {
                var cursor = this._cursor;
                var after = cursor.elementAfter();
                cursor.elementBefore().remove();
                if (after.length > 0) {
                    cursor.moveBefore(after);
                }
                else {
                    cursor.moveToTheEnd();
                }
            },
            
            /* enter */ 13 : function() {
                var cursor = this._cursor;
                cursor.elementBefore().after('<br/>');
                cursor.moveForward();
            },
            
            /* end */ 35 : function() {
                this._cursor.moveToTheEnd();
            },
            
            /* home */ 36 : function() {
                this._cursor.moveToTheStart();
            },

            /* left */ 37 : function() {
                this._cursor.moveBefore(this._cursor.elementBefore());
            },

            /* right */ 39 : function() {
                this._cursor.moveAfter(this._cursor.elementAfter());
            },

            /* delete */ 46 : function() {
                var cursor = this._cursor;
                var before = cursor.elementBefore();
                cursor.elementAfter().remove();
                cursor.moveAfter(before);
            }
        },

        _focused : false,

        focus : function() {
            this._focused = true;
            this._element.focus();
            this._cursor.show();
        },

        _elementBlur : function(e) {
            var that = e.data.that;
            that._focused = false;
            that._cursor.hide();
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

            var newCharacter = String.fromCharCode(e.which);
            if (newCharacter === ' ')
                newCharacter = '&nbsp;';
            
            newCharacter = $('<c>' + newCharacter + '</c>');
            var before = that._cursor.elementBefore();
            if (before.length > 0) {
                before.after(newCharacter);
            }
            else {
                that._cursor.elementAfter().before(newCharacter);
            }
            that._cursor.moveAfter(newCharacter);
        },

        _surfaceCharacterClick : function(e) {
            var that = e.data.that;
            that.focus();
            
            var character = $(this);
            var position = character.position();
            var width = character.width();

            var x = e.pageX - that._surface.offset().left;

            if (x < position.left + (width / 2)) {
                that._cursor.moveBefore(character);
            }
            else {
                that._cursor.moveAfter(character);
            }
        },
    
        _addMarkers : function(text) {
            var result = [];
            for (var i = 0; i < text.length; i++) {
                var c = text.charAt(i);
            
                result.push("<c>");
                result.push(c !== ' ' ? c : '&nbsp;');
                result.push("</c>");
                
                if (c === '\n' || (c === '\r' && text.charAt(i + 1) !== '\n')) {
                    result.push("<br/>");
                }
            }
        
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