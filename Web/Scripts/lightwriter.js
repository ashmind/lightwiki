﻿(function($) {
    $.fn.textNodes = function() {
        var ret = [];
        this.contents().each(function recurse() {
            if (this.nodeType == 3) {
                ret.push(this);
            }
            else {
                $(this).contents().each(recurse);
            }
        });
        return $(ret);
    };
})(jQuery);

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
                            .addClass('lightwriter');

        this._html(this._element.val());

        this._cursor = new lightwriter.cursor(this._surface);
        this._selection = new lightwriter.selection(this._surface);
        this._history = new lightwriter.history();
        
        var thatData = { that : this };
        $('c', this._surface[0]).live('click', thatData, this._surfaceCharacterClick);

        /*var preventDefault = function(e) { e.preventDefault(); };
        this._surface.click(preventDefault);*/

        $(document).keypress(thatData, this._elementKeypress)
                   .keydown(thatData, this._elementKeydown);

        this._element.blur(thatData, this._elementBlur);
    }

    lightwriter.history = function() {
        this._stack = [];
    },

    lightwriter.history.prototype = {
        undo : function() {
            var undo = this._stack.pop();
            if (undo)
                undo();
        },

        record : function(undo) {
            this._stack.push(undo);
        }
    };

    lightwriter.cursor = function(surface) {
        this._surface = surface;
        this.element = $("<div class='lightwriter-cursor lightwriter-system'></div>").appendTo(surface).hide();

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
            var position;
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
    };

    lightwriter.selection.prototype = {
        _getRange : function () {
            return window.getSelection().getRangeAt(0);
        },
        
        removeAllElements : function () {
            this._getRange().deleteContents();
        },
        
        startElement : function() {
            return $(this._getRange().startContainer).closest('c');
        },

        endElement : function() {
            return $(this._getRange().endContainer).closest('c');
        },
        
        visible : function () {
            return !window.getSelection().isCollapsed;
        },
        
        hide : function () {
            window.getSelection().removeAllRanges();
        }
    };

    lightwriter.prototype = {
        keyHandlers : {
            /* backspace */ 8 : function() {
                var cursor = this._cursor;

                var anchor;                
                if (this._selection.visible()) {
                    anchor = this._selection.endElement().next();
                    this._deleteSelection();
                }
                else {                    
                    anchor = cursor.elementAfter();
                    cursor.elementBefore().remove();
                }
                
                /*this._cleanEmptyTags();*/
                if (anchor.length > 0) {
                    cursor.moveBefore(anchor);
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
                this._selection.hide();
                this._cursor.moveToTheEnd();
            },
            
            /* home */ 36 : function() {
                this._selection.hide();
                this._cursor.moveToTheStart();
            },

            /* left */ 37 : function() {
                if (this._selection.visible()) {
                    this._collapseSelectionTo('start');
                    return;
                }

                this._cursor.moveBefore(this._cursor.elementBefore());
            },

            /* right */ 39 : function() {
                if (this._selection.visible()) {
                    this._collapseSelectionTo('end');
                    return;
                }

                this._cursor.moveAfter(this._cursor.elementAfter());
            },

            /* delete */ 46 : function() {
                var cursor = this._cursor;
                var anchor;
                if (this._selection.visible()) {
                    anchor = this._selection.startElement().prev();
                    this._deleteSelection();
                }
                else {
                    anchor = cursor.elementBefore();
                    cursor.elementAfter().remove();
                }

                /*this._cleanEmptyTags();*/
                cursor.moveAfter(anchor);
            }
        },

        _focused : function() {
            return document.activeElement === this._element[0]
                || this._selection.visible();
        },

        focus : function() {
            this._surface.addClass('active');
            this._element.focus();
            if (!this._selection.visible())
                this._cursor.show();
        },

        _elementBlur : function(e) {
            var that = e.data.that;

            that._surface.removeClass('active');
            that._cursor.hide();
        },

        _elementKeydown : function(e) {
            var that = e.data.that;
            if (!that._focused())
                return;

            var handler = that.keyHandlers[e.which];
            if (handler) {
                e.preventDefault();
                handler.call(that);
            }
        },

        _elementKeypress : function(e) {
            var that = e.data.that;
            if (!that._focused())
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
        
        _surfaceCharacterClick : function(e) {
            var that = e.data.that;
            that.focus();
            
            var character = $(this);
            var half = that._whichHalf(character, e.pageX);

            if (half === 'left') {
                that._cursor.moveBefore(character);
            }
            else {
                that._cursor.moveAfter(character);
            }
        },

        _whichHalf : function(character, pageX) {
            character = $(character);

            var position = character.position();
            var width = character.width();
            var x = pageX - this._surface.offset().left;
            return (x < position.left + (width / 2))
                 ? 'left'
                 : 'right';
        },
    
        _html : function(html) {
            var whitespace = /^\s*$/;

            this._surface.html(html);
            this._surface.textNodes().each(function() {
                var node = $(this);
                var result = [];
                var text = node.text();
                if (whitespace.test(text))
                    return;                    

                for (var i = 0; i < text.length; i++) {
                    var c = text.charAt(i);
                    if (c === '\r' || c === '\n')
                        c = ' ';

                    result.push("<c>");
                    result.push(c);
                    result.push("</c>");
                }
        
                node.replaceWith(result.join(''));
            });
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
                'padding-bottom': target.css('padding-bottom')
            }).appendTo(target.parent());            
        },

        _deleteSelection : function() {
            this._selection.removeAllElements();
            this._selection.hide();
            this._cursor.show();
        },

        _collapseSelectionTo : function(to) {
            if (to === 'end') {
                this._cursor.moveAfter(this._selection.endElement());
            }
            else {
                this._cursor.moveBefore(this._selection.startElement());
            }
            this._selection.hide();
            this._cursor.show();
        },

        _cleanEmptyTags : function() {
            var whitespace = /^\s*$/;

            this._surface.find(':not(c,br,.lightwriter-system)').filter(function() {
                return whitespace.test($(this).text());
            }).remove();
            this._surface.find('ul:empty').remove();
        },

        _change : function(change) {
            change['do'].call(this);
            this._history.record(change.undo);
        },

        undo : function() {
            this._history.undo();
        }        
    };
})(jQuery);