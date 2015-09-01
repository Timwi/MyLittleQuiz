/// <reference path="Global.js" />
$(function ()
{
    var content = $('#content');

    function clearScreen()
    {
        $('.away').removeClass('in').addClass('out');
        if (!$('#background').length)
            $('<div id="background">').prependTo(content);
    }

    function twoColumnLayout(id, divClassName, num, nameFnc, fnc)
    {
        var c = $('<div class="two-col">').attr('id', id).appendTo(content).addClassDelay('in', 100);
        if (divClassName !== null)
            c.addClass(divClassName);
        var c1 = $('<div class="col col-1 static">').appendTo(c);
        var c2 = $('<div class="col col-2 static">').appendTo(c);
        var cs = [];
        for (var i = 0; i < num; i++)
        {
            var elem = $('<div class="two-col-elem">').appendTo(i % 2 ? c2 : c1)
                .append($('<div class="name">').append($('<span class="inner">').text(nameFnc(i))));
            if (fnc !== null)
                fnc(elem, i);
            cs.push(elem);
        }

        findBestValue(100, function (fs) { c.css('font-size', fs + 'px'); return c1.outerHeight() < content.height() && c2.outerHeight() < content.height() ? -1 : 1; });

        for (var i = 0; i < cs.length; i++)
        {
            var name = cs[i].find('.name .inner');
            if (name.width() > cs[i].width())
                cs[i].find('.name').css('transform', 'scale(' + (cs[i].width() / name.width()) + ', 1)');
        }
    }

    transitions = {

        //#region QUESTION/ANSWER (showQ, showA, showQA)
        showQ: function (p)
        {
            $('#qa').remove();
            clearScreen();

            var qa = $('<div id="qa" class="static full-flow away ' + p.round + '">').appendTo(content);

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

        showA: function (p)
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

        showQA: function (p)
        {
            transitions.showQ(p);
            window.setTimeout(function () { transitions.showA(p); }, 750);
        },
        //#endregion

        //#region ROUND 1 (Elimination)
        r1_showContestants: function (p)
        {
            $('#r1-contestants, .r1-contestant').remove();
            clearScreen();
            var c = $('<div id="r1-contestants" class="full-flow away">').appendTo(content);
            for (var i = 0; i < p.contestants.length; i++)
            {
                if (p.contestants[i].NumCorrect > 1 || p.contestants[i].NumWrong > 1)
                    continue;
                var div = $('<div class="contestant">')
                    .attr('data-num', i + 1)
                    .css('transition-delay', Math.random() * .75 + 's')
                    .appendTo(c);
                if (p.contestants[i].NumCorrect > 0)
                    div.addClass('correct');
                if (p.contestants[i].NumWrong > 0)
                    div.addClass('wrong');
            }
            findBestValue(100, function (fs) { c.css('font-size', fs + 'px'); return c.height() < content.height() ? -1 : 1; });
            $('.contestant').addClassDelay('in', 100);
        },

        r1_select: function (p)
        {
            $('.r1-contestant').remove();
            clearScreen();

            var c = p.contestants[p.selected];
            var span = $('<span class="inner">').text(c.Name);
            var div = $('<div class="r1-contestant away static" id="r1-contestant-name">').append(span).appendTo(content).addClassDelay('in', 100);
            $('<div class="r1-contestant away" id="r1-contestant-roll">').text(c.Roll).appendTo(content).addClassDelay('in', 100);

            if (span.width() > div.width())
                span.css('transform', 'scale(' + (div.width() / span.width()) + ', 1)');
        },
        //#endregion

        //#region ROUND 2 (Categories)
        r2_showContestants: function (p)
        {
            $('#r2-contestants').remove();
            clearScreen();

            twoColumnLayout('r2-contestants', p.noscores ? 'no-scores away' : 'away', p.contestants.length,
                function (i) { return p.contestants[i].Name; },
                function (div, i)
                {
                    div.append($('<div class="score">').text(p.contestants[i].Score))
                        .css('transition-delay', (p.noscores ? i * .3 : i * .1) + 's')
                        .addClass('r2-contestant');
                });

            window.setTimeout(function ()
            {
                $('.r2-contestant').css('transition-delay', '');
            }, 1500 + (p.noscores ? 300 : 100) * p.contestants.length);
        },

        r2_showCats: function (p)
        {
            $('#r2-cats').remove();
            clearScreen();

            twoColumnLayout('r2-cats', 'away', p.categories.length,
                function (i) { return p.categories[i]; },
                function (div, i)
                {
                    var used = $('<div class="useds">');
                    var anyUnused = false;
                    for (var j = 0; j < 5; j++)
                    {
                        used.append($('<div class="used ' + (p.used[i][j] ? 'yes' : 'no') + '">'));
                        if (!p.used[i][j])
                            anyUnused = true;
                    }
                    div.prepend($('<div class="decor">'))
                        .append(used)
                        .css('transition-delay', (i * .1) + 's')
                        .addClass('r2-cat');
                    if (!anyUnused)
                        div.addClass('all-used');
                    if (p.selected === i)
                        div.addClassDelay('selected', 1500 + i * 100);
                });
        },

        r2_selectCat: function (p)
        {
            $($('#r2-cats .r2-cat').removeClass('selected')[p.selected]).addClass('selected');
        },
        //#endregion
    };
});
