$(function ()
{
    var content = $('#content');

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
            $('.contestants').remove();
            var cl = $('<div class="contestants left">').appendTo(content),
                cr = $('<div class="contestants right">').appendTo(content),
                numLeft = Math.ceil(p.contestants.length / 2),
                ps = [];
            for (var i = 0; i < p.contestants.length; i++)
            {
                ps[i] = $('<div class="contestant">')
                    .append($('<div class="name">').append($('<span class="inner">').text(p.contestants[i].Name)))
                    .append($('<div class="score">').text(p.contestants[i].Score))
                    .addClassDelay('in', 200 * i)
                    .appendTo(i < numLeft ? cl : cr);

                if (i === p.selected)
                    ps[i].addClassDelay('selected', 200 * i + 1000);
            }
            if (cl.height() > content.height())
                findBestValue(10, function (fs)
                {
                    cl.css('font-size', fs + 'vh');
                    cr.css('font-size', fs + 'vh');
                    return cl.height() > content.height() ? 1 : cl.height() < content.height() ? -1 : 0;
                });
            for (var i = 0; i < p.contestants.length; i++)
            {
                var inner = ps[i].find('.inner').width(),
                    outerE = ps[i].find('.name'),
                    outer = outerE.width();
                if (inner > outer)
                    outerE.css('transform', 'scale(' + (outer / inner) + ', 1)');
            }
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