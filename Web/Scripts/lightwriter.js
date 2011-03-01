(function($) {   
    $.fn.lightwriter = function() {
        this.each(enlighten);
        return this;
    };
    
    function enlighten() {
        var target = $(this);
        var surface = overlay(target)
                            .html(addMarkers(target.val()))
                            .appendTo(target.parent());

        $('c', surface[0]).live('click', surfaceCharacterClick);
    }

    var lastClickedCharacter;
    function surfaceCharacterClick() {
        if (lastClickedCharacter)
            lastClickedCharacter.css({ 'background-color' : 'inherit' });
        
        $(this).css({ 'background-color' : 'palegoldenrod' });
        lastClickedCharacter = $(this);
    }
    
    function addMarkers(text) {
        var result = [];
        result.push("<line>");
        for (var i = 0; i < text.length; i++) {
            var c = text.charAt(i);
            
            result.push("<c>");
            result.push(text.charAt(i));
            result.push("</c>");
            
            if (c === '\n' || (c === '\r' && text.charAt(i + 1) !== '\n')) {
                result.push("</line><line>");
            }
        }
        result.push("</line>");
        
        return result.join('');
    }
    
    function overlay(target) {
        var surface = $('<div></div>');
        var offset = target.offset();
        return surface.css({
            position: 'absolute',
            left:     offset.left,
            top:      offset.top,
            width:    target.width() + 'px',
            height:   target.height() + 'px',
            cursor:   'text'
        });
    }
})(jQuery);