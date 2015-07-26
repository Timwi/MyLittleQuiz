var transitions;
$(function ()
{
    var content = $('#content');

    $.fn.addClassDelay = function (cls, delay)
    {
        var t = this;
        window.setTimeout(function () { t.addClass(cls); }, delay);
        return t;
    };

    function findBestValue(startingValue, evaluator, threshold, preferHigh)
    {
        startingValue = +startingValue;
        threshold = threshold || .1;
        preferHigh = !!preferHigh;

        var negative = startingValue < 0;
        var low = negative ? startingValue : 0;
        var high = startingValue == 0 ? 1 : (negative ? 0 : startingValue);
        if (negative)
        {
            while (evaluator(low) > 0)
            {
                high = low;
                low *= 2;
            }
        }
        else
        {
            while (evaluator(high) < 0)
            {
                low = high;
                high *= 2;
            }
        }

        while (high - low > threshold)
        {
            var mid = (high + low) / 2;
            if (evaluator(mid) < 0)
                low = mid;
            else
                high = mid;
        }
        var finalValue = preferHigh ? high : low;
        evaluator(finalValue);
        return finalValue;
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

        showContestants: function (p)
        {
            $('.welcome, .qa .q-or-a').addClass('out').removeClass('in');
            $('.contestant').remove();
            for (var i = 0; i < p.contestants.length; i++)
            {
                var div = $('<div class="contestant">')
                    .append($('<div class="name">').append($('<span class="inner">').text(p.contestants[i].Name)))
                    .append($('<div class="score">').text(p.contestants[i].Score))
                    .appendTo(content)
                    .addClassDelay('in', 200 * i);
                if (i === p.selected)
                    div.addClassDelay('selected', 200 * i + 1000);
            }
            alignContestants();
        },

        select: function (p) { $($('.contestant').removeClass('selected')[p.index]).addClass('selected'); },
        unselect: function () { $('.contestant').removeClass('selected'); },

        showQuestion: function (p)
        {
            $('.welcome, .qa').remove();
            $('.contestant').removeClass('in');

            var qa = $('<div class="qa">')
                .append($('<div class="q-or-a question">')
                    .addClassDelay('in', 100)
                    .append($('<span class="inner">').text(p.question)))
                .append($('<div class="q-or-a answer">')
                    .append($('<span class="inner">').text(p.answer)))
                .appendTo(content);

            if (qa.outerHeight() > $(window).height() * .9)
            {
                findBestValue(parseInt(qa.css('font-size')), function (val)
                {
                    qa.css('font-size', val + 'px');
                    return (qa.outerHeight() > $(window).height() * .9) ? 1 : -1;
                });
            }

            if ('correct' in p)
                window.setTimeout(function () { transitions[p.correct ? 'correct' : 'wrong']() }, 750);
        },

        correct: function () { $('.qa .q-or-a.answer').addClass('in correct'); },
        wrong: function () { $('.qa .q-or-a.answer').addClass('in wrong'); }
    };
});