/// <reference path="Global.js" />
$(function ()
{
    var content = $('#content');

    function clearScreen(except)
    {
        var consider = $('.away');
        if (except)
            consider = consider.not(except);
        consider.removeClass('in').addClass('out');
        if (!$('#background').length)
            $('<div id="background">').prependTo(content);
    }

    function twoColumnLayout(id, divClassName, num, nameFnc, fnc)
    {
        var c = $('<div class="two-col">').attr('id', id).appendTo(content).addClassDelay('in');
        if (divClassName !== null)
            c.addClass(divClassName);
        var c1 = $('<div class="col col-1 static">').appendTo(c);
        var c2 = $('<div class="col col-2 static">').appendTo(c);
        var cs = [];
        for (var i = 0; i < num; i++)
        {
            var elem = $('<div class="two-col-elem">').appendTo(i % 2 ? c2 : c1)
                .append($('<div class="name">').append($('<span class="inner">').text(nameFnc(i))))
                .data('index', i);
            if (fnc !== null)
                fnc(elem);
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

    function r2_setupCats(cats, used)
    {
        $('#r2-cats').remove();
        clearScreen();
        twoColumnLayout('r2-cats', 'away', cats.length,
            function (i) { return cats[i]; },
            function (div)
            {
                var i = div.data('index');
                var useds = $('<div class="useds">');
                var anyUnused = false;
                for (var j = 0; j < 5; j++)
                {
                    useds.append($('<div class="used ' + (used[i][j] ? 'yes' : 'no') + '">'));
                    if (!used[i][j])
                        anyUnused = true;
                }
                div.addClass('r2-cat').prepend($('<div class="decor">')).append(useds);
                if (!anyUnused)
                    div.addClass('all-used');
            });
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
            var qdiv = $('<div class="question qa">').addClassDelay('in').append(qdecor);

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
            $('#r1-contestants').remove();
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
            $('.contestant').addClassDelay('in');
        },

        r1_select: function (p)
        {
            $('.r1-contestant').remove();
            clearScreen();

            var c = p.contestants[p.selected];
            var span = $('<span class="inner">').text(c.Name);
            var div = $('<div class="r1-contestant away static" id="r1-contestant-name">').append(span).appendTo(content).addClassDelay('in');
            $('<div class="r1-contestant away" id="r1-contestant-roll">').text(c.Roll).appendTo(content).addClassDelay('in');

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
                function (div)
                {
                    var i = div.data('index');
                    div.append($('<div class="score">').text(p.contestants[i].Score))
                        .css('transition-delay', (p.noscores ? i * .3 : i * .1) + 's')
                        .addClass('r2-contestant');
                });
        },

        r2_showCats: function (p)
        {
            if (!$('#r2-cats:not(.out)').length)
                r2_setupCats(p.categories, p.used);
            $('#r2-cats').removeClass('presenting');

            if (!$('#r2-cats').hasClass('was-presenting'))
            {
                $('#r2-cats .r2-cat').each(function ()
                {
                    var div = $(this), i = div.data('index');
                    div.css('transition-delay', (i * .1) + 's');
                    if (p.selected === i)
                        window.setTimeout(function () { div.css('transition-delay', '').addClass('selected'); }, 1500 + i * 100);
                });
            }
        },

        r2_presentCat: function (p)
        {
            if (!$('#r2-cats:not(.out).presenting').length)
            {
                r2_setupCats(p.categories, p.used);
                $('#r2-cats').addClass('presenting was-presenting');
                $('#r2-cats .r2-cat')
                    .each(function () { var i = $(this).data('index'); $(this).css('transform-origin', (100 * (i % 2)) + '% ' + (Math.floor(i / 2) / Math.ceil(p.categories.length / 2 - 1) * 80 + 10) + '%'); })
                    .filter(function () { return $(this).data('index') > p.index; })
                    .addClass('hidden');
            }

            $('#r2-cats .r2-cat.present').removeClass('present');
            $('#r2-cats .r2-cat').filter(function () { return $(this).data('index') === p.index; }).removeClass('hidden').addClass('present');
        },

        r2_selectCat: function (p)
        {
            $('#r2-cats').removeClass('was-presenting');
            $('#r2-cats .r2-cat')
                .removeClass('selected')
                .filter(function () { return $(this).data('index') === p.selected; })
                .addClass('selected');
        },
        //#endregion

        //#region ROUND 3 (Set Poker)
        r3_showContestants: function (p)
        {
            clearScreen();

            $('#r3-contestants').remove();
            var div = $('<div id="r3-contestants" class="r3-display static away"></div>').appendTo(content).addClassDelay('in');

            for (var i = 0; i < p.contestants.length; i++)
                div.append($('<div class="contestant"></div>')
                    .css('transition-delay', (i * .1) + 's')
                    .append($('<div class="inner"></div>')
                        .append($('<span class="name"></span>').text(p.contestants[i].Name))));

            findBestValue(100, function (fs) { div.css('font-size', fs + 'px'); return div.outerHeight() < content.height() ? -1 : 1; });

            $('#r3-contestants>.contestant').each(function ()
            {
                var inner = $(this).find('.inner');
                var name = inner.find('.name');
                if (name.outerWidth() > inner.outerWidth())
                    inner.css('transform', 'scaleX(' + (inner.outerWidth() / name.outerWidth()) + ')');
            });
        },

        r3_showTeams: function (p)
        {
            $('#r3-teams').remove();
            clearScreen();

            var div = $('<div id="r3-teams" class="r3-display static away"></div>').appendTo(content).addClassDelay('in');

            for (var i = 0; i < p.teamA.Contestants.length; i++)
                div.append($('<div class="contestant team-a"></div>')
                    .css('transition-delay', (i * .1) + 's')
                    .append($('<div class="inner"></div>')
                        .append($('<span class="name"></span>').text(p.teamA.Contestants[i].Name))));
            for (var i = 0; i < p.teamB.Contestants.length; i++)
                div.append($('<div class="contestant team-b"></div>')
                    .css('transition-delay', ((i + 4) * .1) + 's')
                    .append($('<div class="inner"></div>')
                        .append($('<span class="name"></span>').text(p.teamB.Contestants[i].Name))));

            div.append($('<div class="score-box team-a"></div>').append($('<div class="score">').text(p.teamA.Score)));
            div.append($('<div class="score-box team-b"></div>').append($('<div class="score">').text(p.teamB.Score)));

            findBestValue(100, function (fs) { div.css('font-size', fs + 'px'); return div.outerHeight() < content.height() ? -1 : 1; });
            div.css({ bottom: 0 });

            $('#r3-contestants>.contestant').each(function ()
            {
                var inner = $(this).find('.inner');
                var name = inner.find('.name');
                if (name.outerWidth() > inner.outerWidth())
                    inner.css('transform', 'scaleX(' + (inner.outerWidth() / name.outerWidth()) + ')');
            });
        },

        r3_showSet: function (p)
        {
            $('#r3-set').remove();
            clearScreen();

            var div = $('<div id="r3-set" class="away">').appendTo(content).addClassDelay('in');
            var inner = $('<div class="name static">').appendTo(div).html(p.set);
            findBestValue(100, function (fs) { div.css('font-size', fs + 'px'); return inner.outerHeight() < div.outerHeight() ? -1 : 1; });
        },

        r3_play: function (p)
        {
            if (p.answers.length < 1)
                return;

            var alreadyFontSize = null;
            if ($('#r3-play').length)
                alreadyFontSize = parseFloat($('#r3-play').css('font-size'));

            $('#r3-play,#r3-bid').remove();
            clearScreen();

            var div = $('<table id="r3-play" class="away static">').appendTo(content);
            if ('remaining' in p)
                content.append($('<div id="r3-bid" class="away static">').append($('<span class="number">').text(p.remaining)).addClassDelay('in'));

            var num = p.tie ? 10 : 5;
            if (num < p.answers.length + 1 && (p.remaining > 0 || p.tie))
                num = p.answers.length + 1;
            else if (num < p.answers.length)
                num = p.answers.length;

            var prevTr = null;
            for (var i = 0; i < num; i++)
            {
                if (!p.tie || (i % 2 == 0))
                    prevTr = $('<tr>').appendTo(div);

                var td = $('<td>').append(
                    $('<div class="ans' + (i < p.answers.length ? '' : ' invisible') + '">').data('index', i).append(
                        $('<div class="answer">').append(
                            $('<span class="inner">').html(i < p.answers.length ? p.answers[i] : p.answers[0]))));
                if (!p.tie || p.teamAStarted)
                    td.appendTo(prevTr);
                else
                    td.prependTo(prevTr);
            }

            var fontSize = findBestValue(100, function (fs)
            {
                div.css('font-size', fs + 'px');
                if (div.outerHeight() > content.height() || div.outerWidth() > content.width())
                    return 1;
                var elems = $('#r3-play .ans');
                for (var i = 0; i < elems.length; i++)
                    if ($(elems[i]).find('.inner').width() > $(elems[i]).find('.answer').width())
                        return 1;
                return -1;
            });
            if (alreadyFontSize != null && alreadyFontSize != fontSize)
            {
                div.css('font-size', alreadyFontSize + 'px');
                div.animate({ fontSize: fontSize }, { duration: 2000, queue: false });
            }

            var elem = $('#r3-play .ans').filter(function (_, e) { return $(e).data('index') === p.answers.length - 1 });
            var width = elem.width();
            var height = elem.outerHeight();
            elem.removeClass('invisible').css({ width: '0%', height: height + 'px', opacity: 0 });
            var inner = elem.find('.answer');
            inner.css({ width: width + 'px' });

            elem.animate({ opacity: 1 }, { duration: 700, queue: false });
            elem.animate({ width: '100%' }, {
                duration: 1000,
                queue: false,
                done: function ()
                {
                    elem.css({ width: '', height: '' });
                    inner.css({ width: '' });
                }
            });
        },
        //#endregion

        //#region ROUND 4 (Final/Sudden Death)
        r4_showContestants: function (p)
        {
            clearScreen('#r4-contestants');

            var div = $('#r4-contestants');
            if (!div.length)
                div = $('<div id="r4-contestants" class="static away">')
                    .append('<table>')
                    .appendTo(content);
            var table = div.find('table');

            var rows = p.answers.length;
            var trs = div.find('tr');
            for (var i = Math.max(rows, trs.length) - 1; i >= 0; i--)
            {
                if (i >= rows.length)
                    $(trs.get(i)).remove();
                else if (i >= trs.length)
                    $('<tr>').appendTo(table);
            }
            trs = div.find('tr');

            var cols = p.minAnswers;
            for (var i = 0; i < p.answers.length; i++)
                if (p.answers[i].length > cols)
                    cols = p.answers[i].length;
            cols++; // contestant names

            for (var i = 0; i < trs.length; i++)
            {
                var tr = $(trs.get(i));
                var tds = tr.find('td');
                for (var j = Math.max(cols, tds.length) - 1; j >= 0; j--)
                {
                    if (j >= cols.length)
                        $(tds.get(j)).remove();
                    else if (j >= tds.length)
                        $('<td>').appendTo(tr);
                }
            }

            for (var i = 0; i < rows; i++)
            {
                var tr = $(trs.get(i));
                var tds = tr.find('td');
                for (var j = 0; j < cols; j++)
                {
                    var td = $(tds.get(j));
                    if (j === 0)
                        td.text(p.contestants[i].Name);
                    else
                        td.addClass(j - 1 >= p.answers[i].length ? 'none' : p.answers[i][j - 1] ? 'correct' : 'wrong');
                }
            }
        },
        //#endregion
    };
});
