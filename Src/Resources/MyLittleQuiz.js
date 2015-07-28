$(function ()
{
    var content = $('#content');

    transitions = {

        r1_showContestants: function (p)
        {
            $('#r1contestants').remove();
            var c = $('<div id="r1contestants">').appendTo(content);
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
            $('#r1contestants').addClass('out');
        }

    };
});
