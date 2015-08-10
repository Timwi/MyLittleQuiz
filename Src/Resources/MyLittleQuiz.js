$(function ()
{
    var content = $('#content');

    function ensureBackground()
    {
        if (!$('#background').length)
            $('<div id="background">').prependTo(content);
    }

    transitions = {

        r1_showContestants: function (p)
        {
            ensureBackground();
            $('#r1-contestants, .r1-contestant').remove();
            var c = $('<div id="r1-contestants">').appendTo(content);
            for (var i = 0; i < p.contestants.length; i++)
            {
                var div = $('<div class="contestant">')
                    .attr('data-num', p.contestants[i].Round1Number)
                    .css('transition-delay', Math.random() * .75 + 's')
                    .appendTo(c);
                if (p.contestants[i].Round1Correct > 0)
                    div.addClass('correct');
                if (p.contestants[i].Round1Wrong > 0)
                    div.addClass('wrong');
            }
            findBestValue(100, function (fs) { c.css('font-size', fs + 'px'); return c.height() < content.height() ? -1 : 1; });
            $('.contestant').addClassDelay('in', 100);
        },

        r1_select: function (p)
        {
            ensureBackground();
            $('#r1-contestants').addClass('out');
            $('.r1-contestant').remove();

            var c = p.contestants[p.selected];
            var div = $('<div class="r1-contestant" id="r1-contestant-name">').append($('<span class="inner">').text(c.Name)).appendTo(content).addClassDelay('in', 100);
            var span = div.find('.inner');
            $('<div class="r1-contestant" id="r1-contestant-roll">').text(c.Roll).appendTo(content).addClassDelay('in', 100);

            if (span.width() > div.width())
                span.css('transform', 'scale(' + (div.width() / span.width()) + ', 1)');
        }

    };
});
