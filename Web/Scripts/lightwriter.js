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
        this._cursor = $("<div class='lightwriter-cursor'></div>")
                            .appendTo(this._surface);

        window.setInterval(function() {
            that._cursor.toggle();
        }, 600);

        var that = this;
        $('c', this._surface[0]).live('click', { that : this }, this._surfaceCharacterClick);
    }

    lightwriter.prototype = {
        _surfaceCharacterClick : function(e) {
            var that = e.data.that;
            var character = $(this);
            var position = character.position();
            var width = character.width();

            var x = e.pageX - that._surface.offset().left;
            var left = x > position + (width / 2)
                     ? position.left
                     : position.left + width;

            e.data.that._cursor.css({
                top : position.top + 'px',
                left : left + 'px'
            });
        },
    
        _addMarkers : function(text) {
            var result = [];
            result.push("<p>");
            for (var i = 0; i < text.length; i++) {
                var c = text.charAt(i);
            
                result.push("<c>");
                result.push(text.charAt(i));
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