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
        this._selection = new lightwriter.selection(this._surface);
        
        var thatData = { that : this };
        $('c', this._surface[0]).live('click', thatData, this._surfaceCharacterClick)
                                .live('mousedown', thatData, this._surfaceCharacterMousedown)
                                .live('mousemove', thatData, this._surfaceCharacterMousemove);

        var preventDefault = function(e) { e.preventDefault(); };
        this._surface.click(preventDefault)
                     .mousedown(preventDefault)
                     .mouseup(thatData, this._surfaceMouseup);

        this._element.blur(thatData, this._elementBlur)
                     .keypress(thatData, this._elementKeypress)
                     .keydown(thatData, this._elementKeydown);
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
            this.moveBefore(this._surface.find('c:first'));
        },

        moveToTheEnd : function() {
            this.moveAfter(this._surface.find('c:last'));
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

    lightwriter.selection = function(surface) {
        this._surface = surface;
        this.active = false;
    };

    lightwriter.selection.prototype = {
        startAt : function(character) {
            if (this._start) {
                this._eachBetween(
                    this._start, this._end,
                    function(element) {
                        element.removeClass('selected');
                    }
                );
            }

            this.active = true;
            this._start = character;
            this._start.addClass('selected');
            this._end = character;
        },

        expandTo : function(character) {
            if (character.hasClass('selected'))
                return;

            var scanResult = this._eachBetween(
                this._end, character,
                function(element) {
                    element.addClass('selected');
                }
            );

            if (!scanResult.swap) {
                this._end = character;
            }
            else {
                this._start = character;
            }
        },

        _eachBetween : function(first, second, action) {
            var between = false;
            var swap = false;
            this._surface.find('c').each(function() {
                if (this === first[0]) {
                    between = true;
                }
                else if (this === second[0] && !between) {
                    var temp = first;
                    first = second;
                    second = temp;

                    between = true;
                    swap = true;
                }

                if (between)
                    action($(this));

                return this !== second[0];
            });

            return { swap : swap };
        },

        stop : function() {
            this.active = false;
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

        _elementKeydown : function(e) {
            var that = e.data.that;
            if (!that._focused)
                return;

            var handler = that.keyHandlers[e.which];
            if (handler) {
                e.preventDefault();
                handler.call(that);
            }
        },

        _elementKeypress : function(e) {
            var that = e.data.that;
            if (!that._focused)
                return;
            
            e.preventDefault();
            /*var handler = that.keyHandlers[e.keyCode];
            if (handler) {
                handler.call(that);
                return;
            }*/

            if (that.keyHandlers[e.keyCode])
                return;

            var newCharacter = String.fromCharCode(e.which);            
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
        
        _surfaceCharacterMousedown : function(e) {
            var that = e.data.that;
            that._selection.startAt($(this));
            that._cursor.hide();
            e.preventDefault();
        },

        _surfaceCharacterMousemove : function(e) {
            var selection = e.data.that._selection;
            if (selection.active)
                selection.expandTo($(this));
        },

        _surfaceMouseup : function(e) {
            var that = e.data.that;

            that._selection.stop();
            that._cursor.show();
            e.preventDefault();
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

                if (c !== '\r' && c !== '\n') {
                    result.push("<c>");
                    result.push(c);
                    result.push("</c>");
                }
                else if (c === '\n' || (c === '\r' && text.charAt(i + 1) !== '\n')) {
                    result.push('<br/>');
                }
            }
        
            return result.join('');
        },
    
        _overlay : function (target) {
            var surface = $('<div></div>');
            var position = target.position();
            target.css({ opacity: 0 });
            return surface.css({
                position:       'absolute',
                left:           position.left + 'px',
                top:            position.top + 'px',
                width:          target.width() + 'px',
                height:         target.height() + 'px',

                'padding-left':   target.css('padding-left'),
                'padding-top':    target.css('padding-top'),
                'padding-right':  target.css('padding-right'),
                'padding-bottom': target.css('padding-bottom'),
            }).appendTo(target.parent());            
        }
    };
})(jQuery);