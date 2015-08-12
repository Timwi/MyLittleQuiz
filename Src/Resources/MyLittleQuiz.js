/// <reference path="Global.js" />
$(function ()
{
    var content = $('#content');

    function ensureBackground()
    {
        if (!$('#background').length)
            $('<div id="background">').prependTo(content);
    }

    function clearScreen()
    {
        $('.away').removeClass('in').addClass('out');
    }

    transitions = {

        r1_showContestants: function (p)
        {
            $('#r1-contestants, .r1-contestant').remove();
            ensureBackground();
            clearScreen();
            var c = $('<div id="r1-contestants">').appendTo(content);
            for (var i = 0; i < p.contestants.length; i++)
            {
                var div = $('<div class="contestant away">')
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
            $('.r1-contestant').remove();
            clearScreen();
            ensureBackground();

            var c = p.contestants[p.selected];
            var span = $('<span class="inner">').text(c.Name);
            var div = $('<div class="r1-contestant away" id="r1-contestant-name">').append(span).appendTo(content).addClassDelay('in', 100);
            $('<div class="r1-contestant away" id="r1-contestant-roll">').text(c.Roll).appendTo(content).addClassDelay('in', 100);

            if (span.width() > div.width())
                span.css('transform', 'scale(' + (div.width() / span.width()) + ', 1)');
        },

        r1_showQ: function (p)
        {
            $('#qa').remove();
            clearScreen();
            ensureBackground();

            var qa = $('<div id="qa" class="away">').appendTo(content);

            var q = p.question;
            qa.data('type', q[':type']);

            var qdiv = $('<div class="question qa">').addClassDelay('in', 100);
            var adiv = $('<div class="answer qa">');

            switch (q[':type'])
            {
                case 'SimpleQuestion':
                    qdiv.append($('<div class="text">').text(q.QuestionText));
                    adiv.append($('<div class="text">').text(q.Answer));
                    break;

                case 'NOfQuestion':
                    qdiv.append($('<div class="text">').text(q.QuestionText));
                    var ul = $('<ul class="text">').appendTo(adiv);
                    for (var i = 0; i < p.Answers.length; i++)
                        ul.append($('<li>').addClass('opt-' + i).text(p.Answers[i]));
                    break;
            }

            qa.append(qdiv).append(adiv);
            findBestValue(20, function (fs) { qa.css('font-size', fs + 'px'); return qa.height() < content.height() ? -1 : 1; });
        },

        r1_showA: function (p)
        {
            var qa = $('#qa');
            if (!qa.length)
                return;

            var a = p.answer;
            if (':value' in a)
                a = a[':value'];

            switch (qa.data('type'))
            {
                case 'SimpleQuestion':
                    $('#qa .answer').addClass(a === false ? 'wrong in' : 'correct in');
                    break;

                case 'NOfQuestion':
                    $('#qa .answer').addClass(a === false ? 'wrong in' : 'correct in');
                    if (a !== false)
                        for (var i = 0; i < a.length; i++)
                            $('#qa .answer .text opt-' + a[i]).addClass('sel');
                    break;
            }
        },

        r1_showQA: function (p)
        {
            transitions.r1_showQ(p);
            window.setTimeout(function () { transitions.r1_showA(p); }, 750);
        }

    };
});
