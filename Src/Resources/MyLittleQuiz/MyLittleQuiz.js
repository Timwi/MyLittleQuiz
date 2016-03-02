/// <reference path="Global.js" />
$(function ()
{
    var content = $('#content');

    function clearScreen(except)
    {
        $('.away.out').remove();
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

    function r4_addBackground(div, iwidth, height, hue, positionTop)
    {
        var owidth = div.outerWidth();
        var lf = owidth / 2 - iwidth / 2;
        var llf = lf - height / 4;
        var rg = lf + iwidth;
        var rrg = rg + height / 4;
        var strw = content.height() / 250;
        var svg = '';
        svg += '<svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" viewBox="0 -' + strw + ' ' + owidth + ' ' + (height + strw) + '">';
        svg += '<defs>';
        svg += '<linearGradient id="lg">';
        svg += '<stop offset="0"   style="stop-color:hsl(' + hue + ', 33%, 20%);stop-opacity:1;" />';
        svg += '<stop offset="0.2" style="stop-color:hsl(' + hue + ', 15%, 47%);stop-opacity:1;" />';
        svg += '<stop offset="0.3" style="stop-color:hsl(' + hue + ', 17%, 60%);stop-opacity:1;" />';
        svg += '<stop offset="0.9" style="stop-color:hsl(' + hue + ', 50%, 13%);stop-opacity:1;" />';
        svg += '<stop offset="1"   style="stop-color:hsl(' + hue + ', 33%, 20%);stop-opacity:1;" />';
        svg += '</linearGradient>';
        svg += '<linearGradient xlink:href="#lg" id="lqh" x1="0" y1="0" x2=".1" y2="1" />';
        svg += '</defs>';
        svg += '<line x1="0" y1="' + (height / 2 - strw / 2) + '" x2="' + owidth + '" y2="' + (height / 2 - strw / 2) + '" stroke="hsl(' + hue + ', 17%, 40%)" stroke-width="' + strw + '" />';
        svg += '<line x1="0" y1="' + (height / 2 + strw / 2) + '" x2="' + owidth + '" y2="' + (height / 2 + strw / 2) + '" stroke="hsl(' + hue + ', 50%, 13%)" stroke-width="' + strw + '" />';
        svg += '<path style="fill:url(#lqh)" d="M ' + lf + ',0 ' + rg + ',0 ' + rrg + ',' + (height / 2) + ' ' + rg + ',' + height + ' ' + lf + ',' + height + ' ' + llf + ',' + (height / 2) + ' z" />';
        svg += '<line x1="' + llf + '" y1="' + (height / 2) + '" x2="' + lf + '"  y2="0"              stroke="hsl(' + hue + ', 25%, 73%)" stroke-width="' + strw + '" />';
        svg += '<line x1="' + lf + '"  y1="0"              x2="' + rg + '"  y2="0"              stroke="hsl(' + hue + ', 16%, 40%)" stroke-width="' + strw + '" />';
        svg += '<line x1="' + rg + '"  y1="0"              x2="' + rrg + '" y2="' + (height / 2) + '" stroke="hsl(' + hue + ', 25%, 27%)" stroke-width="' + strw + '" />';
        svg += '<line x1="' + rrg + '" y1="' + (height / 2) + '" x2="' + rg + '"  y2="' + height + '"     stroke="hsl(' + hue + ', 50%, 13%)" stroke-width="' + strw + '" />';
        svg += '<line x1="' + rg + '"  y1="' + height + '"     x2="' + lf + '"  y2="' + height + '"     stroke="hsl(' + hue + ', 50%, 13%)" stroke-width="' + strw + '" />';
        svg += '<line x1="' + lf + '"  y1="' + height + '"     x2="' + llf + '" y2="' + (height / 2) + '" stroke="hsl(' + hue + ', 20%, 33%)" stroke-width="' + strw + '" />';
        svg += '</svg>';
        div.css({
            'background-image': "url('data:image/svg+xml," + svg.replace(/#/g, '%23') + "')",
            'background-repeat': 'no-repeat',
            'background-position': positionTop ? '50% 0%' : '50% 50%',
            'background-size': 'fit'
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
            var qdiv = $('<div class="question qa ' + p.round + '-style away">').addClassDelay('in', 500).append(qdecor);

            var adecor = $('<div class="decor">').text('✗');
            var adiv = $('<div class="answer qa ' + p.round + '-style away invisible">').append(adecor);

            var qtext, atext;

            switch (q[':type'])
            {
                case 'SimpleQuestion':
                    qtext = $('<div class="text">').html(q.QuestionText).appendTo(qdiv);
                    atext = $('<div class="text">').html(q.Answer).appendTo(adiv);
                    break;
            }

            qa.append(qdiv).append(adiv);

            // Overall font size
            findBestValue(20, function (fs) { qa.css('font-size', fs + 'px'); return qa.outerHeight() < content.height() ? -1 : 1; });

            // Question “?” decor
            findBestValue(20, function (fs) { qdecor.css('font-size', fs + 'px'); return qdecor.outerHeight() < qdiv.height() ? -1 : 1; });
            // Answer “✗”/“✓” decor
            findBestValue(20, function (fs) { adecor.css('font-size', fs + 'px'); return adecor.outerHeight() < adiv.height() ? -1 : 1; });

            if (p.round === 'r4')
                r4_addBackground(qdiv, qtext.innerWidth(), qtext.innerHeight(), 240);
        },

        showA: function (p)
        {
            var qa = $('#qa');
            if (!qa.length)
                return;

            var a = p.answer;
            if (':value' in a)
                a = a[':value'];

            $('#qa .answer').removeClass('invisible wrong correct').addClass(a === false ? 'wrong in' : 'correct in');
            $('#qa .answer .decor').text(a === false ? '✗' : '✓');

            if (p.round === 'r4')
            {
                var atext = $('#qa .answer .text');
                r4_addBackground($('div.qa.answer'), atext.innerWidth(), atext.innerHeight(), a === false ? 0 : 110);
            }

            switch (qa.data('type'))
            {
                case 'SimpleQuestion':
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
            $('#r3-contestants').remove();
            clearScreen();

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

            $('#r3-play').remove();
            clearScreen('#r3-bid,#r3-strikes');

            var div = $('<table id="r3-play" class="away static">').appendTo(content);
            if ('remaining' in p)
            {
                if (!$('#r3-bid').length)
                    content.append($('<div id="r3-bid" class="away static">').append($('<span class="number">')).addClassDelay('in'));
                $('#r3-bid .number').text(p.remaining);
            }
            if ('strikes' in p && p.strikes > 0)
            {
                if (!$('#r3-strikes').length)
                    content.append($('<div id="r3-strikes" class="away static">').addClassDelay('in'));
                var already = $('#r3-strikes .strike');
                if (already.length > p.strikes)
                    already.slice(p.strikes).remove();
                for (var i = already.length; i < p.strikes; i++)
                    $('#r3-strikes').append($('<span class="strike">').text('✗').addClassDelay('in'));
            }

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
                    $('<div class="ans' + (i < p.answers.length ? ((p.answers[i] === null ? ' wrong' : '') + (i < p.answers.length - 1 ? ' in' : '')) : ' invisible') + '">').data('index', i).append(
                        $('<div class="answer">').append(
                            $('<span class="inner">').html(i < p.answers.length && p.answers[i] !== null ? p.answers[i] : p.answers[0]))));
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
            if (p.answers[p.answers.length - 1] !== null)
            {
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
            }
            else
                elem.removeClass('in').addClassDelay('in');
        },

        r3_reveal: function (p)
        {
            $('#r3-reveal').remove();
            clearScreen();

            var div = $('<div id="r3-reveal" class="static away">').appendTo(content).addClassDelay('in');
            div.append($('<h1>').html(p.set));
            var table = $('<table>').appendTo(div);

            var bestFontSize = null;
            var bestTr = null;
            for (var cols = 1; cols <= 4; cols++)
            {
                var tr = $('<tr>').appendTo(table);
                var itemsPerCol = Math.ceil(p.remaining.length / cols);
                for (var col = 0; col < cols; col++)
                {
                    var td = $('<td>').appendTo(tr);
                    for (var i = itemsPerCol * col; i < itemsPerCol * (col + 1) && i < p.remaining.length; i++)
                        td.append($('<div>').html(p.remaining[i]));
                }

                var fontSize = findBestValue(100, function (fs)
                {
                    div.css('font-size', fs + 'px');
                    return div.outerHeight() < content.height() ? -1 : 1;
                });
                if (bestFontSize === null || bestFontSize < fontSize)
                {
                    bestFontSize = fontSize;
                    bestTr = tr;
                }

                table.empty();
            }
            div.css('font-size', bestFontSize + 'px');
            table.append(bestTr);
        },
        //#endregion

        //#region ROUND 4 (Final/Sudden Death)
        r4_showContestants: function (p)
        {
            $('#r4-contestants').remove();
            clearScreen();

            var div = $('<div id="r4-contestants" class="static away">')
                        .appendTo(content);
            var innerDiv = $('<div class="r4-style away">')
                        .appendTo(div)
                        .addClassDelay('in', 500);
            var table = $('<table>').appendTo(innerDiv);

            var rows = p.answers.length;
            var trs = div.find('tr');
            for (var i = Math.max(rows, trs.length) - 1; i >= 0; i--)
            {
                if (i >= rows)
                    $(trs.get(i)).remove();
                else if (i >= trs.length)
                    $('<tr>').appendTo(table);
            }
            trs = table.find('tr');

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
                    if (j >= cols)
                        $(tds.get(j)).remove();
                    else if (j >= tds.length)
                        $('<td>').appendTo(tr);
                }
            }

            var turn = 0;
            for (var i = 1; turn === 0 && i < p.answers.length; i++)
                if (p.answers[i].length < p.answers[0].length)
                    turn = i;

            for (var i = 0; i < rows; i++)
            {
                var tr = $(trs.get(i));
                var tds = tr.find('td');
                for (var j = 0; j < cols; j++)
                {
                    var td = $(tds.get(j));
                    if (j === 0)
                        td.text(p.contestants[i].Name);
                    else if (i === turn && j - 1 === p.answers[i].length)
                        td.addClass('turn');
                    else
                        td.addClass(j - 1 >= p.answers[i].length ? 'none' : p.answers[i][j - 1] ? 'correct' : 'wrong');
                }
            }

            findBestValue(100, function (fs)
            {
                div.css('font-size', fs + 'px');
                return div.outerHeight() < content.height() / 3 ? -1 : 1;
            });

            r4_addBackground(innerDiv, table.outerWidth(), table.outerHeight(), 30, true);
        },
        //#endregion
    };
});
