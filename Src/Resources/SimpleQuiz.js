var transitions;
$(function ()
{
    var content = $('#content');

    $.fn.addClassDelay = function (cls, delay)
    {
        var t = this;
        window.setTimeout(function () { t.addClass(cls); }, delay);
    };

    function findBestFontSize(obj, curValue, maxSize, getSize, setSize)
    {
        var low = 0;
        while (getSize() < maxSize)
        {
            low = curValue;
            setSize(curValue *= 2);
        }
        high = curValue;
        while (high - low > .1)
        {
            var mid = (high + low) / 2;
            setSize(mid);
            if (getSize() < maxSize)
                low = mid;
            else
                high = mid;
        }
        setSize(low);
    }

    function alignContestants()
    {
        var c = $('.contestant');
        var spacing = 1;
        var numLeft = Math.ceil(c.length / 2);
        c.each(function (i, t)
        {
            t = $(t);
            t.removeClass('left right').css({ top: (i < numLeft ? 3 + spacing * (i + 1) + 10 * i : 3 + spacing * (i - numLeft + 1) + 10 * (i - numLeft)) + 'vh' });
            t.addClass(i < numLeft ? 'left' : 'right');
            var outer = t.find('.name'), inner = outer.find('.inner');
            if (inner.width() > outer.width())
                findBestFontSize(inner, 5, outer.width(), function () { return inner.width(); }, function (s) { inner.css('fontSize', s + 'vh'); });
        });
    }

    transitions = {
        welcome: function (p)
        {
            content.empty();
            $('<div class="welcome">')
                .text("Welcome!")
                .appendTo(content)
                .addClassDelay('in', 100);
        },

        start: function (p)
        {
            $('.welcome, .question').addClass('out').removeClass('in');
            $('.contestant').remove();
            for (var i = 0; i < p.contestants.length; i++)
                $('<div class="contestant">')
                    .append($('<div class="name">').append($('<span class="inner">').text(p.contestants[i].Name)))
                    .append($('<div class="score">').text(p.contestants[i].Score))
                    .appendTo(content)
                    .addClassDelay('in', 200 * i);
            alignContestants();
        },

        select: function (p) { $($('.contestant').removeClass('selected')[p.index]).addClass('selected'); },
        unselect: function () { $('.contestant').removeClass('selected'); },

        showQuestion: function (p)
        {
            $('.question').remove();
            $('.contestant').removeClass('in');
            $('<div class="question">')
                .append($('<span class="inner">').text(p.question))
                .appendTo(content)
                .addClassDelay('in', 100);
        }
    };
});