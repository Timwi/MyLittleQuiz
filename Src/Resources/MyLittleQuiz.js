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
            var c = $('<div id="r1-contestants" class="away">').appendTo(content);
            for (var i = 0; i < p.contestants.length; i++)
            {
                if (p.contestants[i].Round1Correct > 1 || p.contestants[i].Round1Wrong > 1)
                    continue;
                var div = $('<div class="contestant">')
                    .attr('data-num', i + 1)
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
            var div = $('<div class="r1-contestant away static" id="r1-contestant-name">').append(span).appendTo(content).addClassDelay('in', 100);
            $('<div class="r1-contestant away" id="r1-contestant-roll">').text(c.Roll).appendTo(content).addClassDelay('in', 100);

            if (span.width() > div.width())
                span.css('transform', 'scale(' + (div.width() / span.width()) + ', 1)');
        },

        r1_showQ: function (p)
        {
            $('#qa').remove();
            clearScreen();
            ensureBackground();

            var qa = $('<div id="qa" class="static away">').appendTo(content);

            var q = p.question;
            qa.data('type', q[':type']);

            var qdecor = $('<div class="decor">').text('?');
            var qdiv = $('<div class="question qa">').addClassDelay('in', 100).append(qdecor);

            var adecor = $('<div class="decor">').text('✗');
            var adiv = $('<div class="answer qa">').append(adecor);

            switch (q[':type'])
            {
                case 'SimpleQuestion':
                    qdiv.append($('<div class="text">').html(q.QuestionText));
                    adiv.append($('<div class="text">').html(q.Answer));
                    break;

                case 'NOfQuestion':
                    qdiv.append($('<div class="text">').html(q.QuestionText));
                    var ul = $('<ul class="text">').appendTo(adiv);
                    for (var i = 0; i < q.Answers.length; i++)
                        ul.append($('<li>').css('transition-delay', (i / 4) + 's').addClass('opt-' + i).append($('<div class="inner">').append($('<span class="inner">').html(q.Answers[i]))));
                    break;
            }

            qa.append(qdiv).append(adiv);

            // Overall font size
            findBestValue(20, function (fs) { qa.css('font-size', fs + 'px'); return qa.outerHeight() < content.height() ? -1 : 1; });

            // Question “?” decor
            findBestValue(20, function (fs) { qdecor.css('font-size', fs + 'px'); return qdecor.outerHeight() < qdiv.outerHeight() ? -1 : 1; });
            // Answer “✗”/“✓” decor
            findBestValue(20, function (fs) { adecor.css('font-size', fs + 'px'); return adecor.outerHeight() < adiv.outerHeight() ? -1 : 1; });
        },

        r1_showA: function (p)
        {
            var qa = $('#qa');
            if (!qa.length)
                return;

            var a = p.answer;
            if (':value' in a)
                a = a[':value'];

            $('#qa .answer').addClass(a === false ? 'wrong in' : 'correct in');
            if (a !== false)
                $('#qa .answer .decor').text('✓');

            switch (qa.data('type'))
            {
                case 'SimpleQuestion':
                    break;

                case 'NOfQuestion':
                    if (a !== false)
                        for (var i = 0; i < a.length; i++)
                            $('#qa .answer .text li.opt-' + a[i]).addClass('sel');
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
