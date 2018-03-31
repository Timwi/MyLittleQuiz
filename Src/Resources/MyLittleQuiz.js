/// <reference path="Global.js" />
$(function ()
{
    var content = $('#content');

    var preloadableImages = ["BonBon.svg", "Derpy.png", "Horseshoe.svg", "Logo.png", "Lyra.png", "Minuette.png", "Muffin1_sm.png", "Muffin2_sm.png", "Muffin3_sm.png",
        "Octavia.png", "Roseluck.png", "SweetieBelle.png", "Trophy.png"];

    var preloadableJingles = ["Round1CorrectAnswer", "Round1WrongAnswer", "Present", "Tada", "Swoosh", "PresentSet", "Round3CorrectAnswer", "Round3WrongAnswer",
        "Round1Start", "Round2Start", "Round3Start", "Round4Start", "WinnerAndOutro"];

    function clearScreen(bgClass, except)
    {
        // There are some items we want to ensure are pre-buffered (mostly images).
        // Make sure that there is always an ‘img’ element for each such image.
        for (var i = 0; i < preloadableImages.length; i++)
        {
            var p = $('#preloadable' + i);
            if (!p.length)
                content.append("<img src='/files/MyLittleQuiz/" + preloadableImages[i] + "' id='preloadable" + i + "' class='preloadable'>");
        }
        // Create an audio element for every jingle
        for (i = 0; i < preloadableJingles.length; i++)
        {
            var q = $('#preloadableAudio' + i);
            if (!q.length)
                $(document.body).append("<audio src='/files/MyLittleQuiz/GeneratedMusic/" + preloadableJingles[i] + ".ogg' id='preloadableAudio" + i + "'>");
        }
        // Pre-buffer the intro video if we are likely to need it soon
        if (bgClass == 'setup' || bgClass == 'intro')
        {
            if (!$('#intro').length)
                $('<video id="intro" src="/files/MyLittleQuiz/' + $('#content').data('package') + '/Intro.mp4">').appendTo(content);
        }

        $('.away.out').remove();
        $('#intro').removeClass('visible');
        var consider = $('.away');
        if (except)
            consider = consider.not(except);
        consider.removeClass('in').addClass('out');
        if (!$('#background').length)
            $('<div id="background">').prependTo(content);
        $('#background').removeClass().addClass(bgClass);
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
        clearScreen('r2');
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

    function r4_addBackground(div, iwidth, height, hue, positionTop, xFactor)
    {
        xFactor = xFactor || .5;
        var owidth = div.outerWidth();
        var lf = (owidth - iwidth) * xFactor;
        var llf = lf - height / 4;
        var rg = lf + iwidth;
        var rrg = rg + height / 4;
        var strw = content.height() / 250;
        var svg = '';
        svg += '<svg xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" viewBox="0 -' + strw + ' ' + owidth + ' ' + (height + strw) + '">';
        svg += '    <defs>';
        svg += '        <linearGradient id="lg">';
        svg += '            <stop offset="0"   style="stop-color:hsl(' + hue + ', 33%, 20%);stop-opacity:1;" />';
        svg += '            <stop offset="0.2" style="stop-color:hsl(' + hue + ', 15%, 47%);stop-opacity:1;" />';
        svg += '            <stop offset="0.3" style="stop-color:hsl(' + hue + ', 17%, 60%);stop-opacity:1;" />';
        svg += '            <stop offset="0.9" style="stop-color:hsl(' + hue + ', 50%, 13%);stop-opacity:1;" />';
        svg += '            <stop offset="1"   style="stop-color:hsl(' + hue + ', 33%, 20%);stop-opacity:1;" />';
        svg += '        </linearGradient>';
        svg += '        <linearGradient xlink:href="#lg" id="lqh" x1="0" y1="0" x2=".1" y2="1" />';
        svg += '    </defs>';
        svg += '    <line x1="0" y1="' + (height / 2 - strw / 2) + '" x2="' + owidth + '" y2="' + (height / 2 - strw / 2) + '" stroke="hsl(' + hue + ', 17%, 40%)" stroke-width="' + strw + '" />';
        svg += '    <line x1="0" y1="' + (height / 2 + strw / 2) + '" x2="' + owidth + '" y2="' + (height / 2 + strw / 2) + '" stroke="hsl(' + hue + ', 50%, 13%)" stroke-width="' + strw + '" />';
        svg += '    <path style="fill:url(#lqh)" d="M ' + lf + ',0 ' + rg + ',0 ' + rrg + ',' + (height / 2) + ' ' + rg + ',' + height + ' ' + lf + ',' + height + ' ' + llf + ',' + (height / 2) + ' z" />';
        svg += '    <line x1="' + llf + '" y1="' + (height / 2) + '" x2="' + lf + '" y2="0" stroke="hsl(' + hue + ', 25%, 73%)" stroke-width="' + strw + '" />';
        svg += '    <line x1="' + lf + '" y1="0" x2="' + rg + '" y2="0" stroke="hsl(' + hue + ', 16%, 40%)" stroke-width="' + strw + '" />';
        svg += '    <line x1="' + rg + '" y1="0" x2="' + rrg + '" y2="' + (height / 2) + '" stroke="hsl(' + hue + ', 25%, 27%)" stroke-width="' + strw + '" />';
        svg += '    <line x1="' + rrg + '" y1="' + (height / 2) + '" x2="' + rg + '" y2="' + height + '" stroke="hsl(' + hue + ', 50%, 13%)" stroke-width="' + strw + '" />';
        svg += '    <line x1="' + rg + '" y1="' + height + '" x2="' + lf + '" y2="' + height + '" stroke="hsl(' + hue + ', 50%, 13%)" stroke-width="' + strw + '" />';
        svg += '    <line x1="' + lf + '" y1="' + height + '" x2="' + llf + '" y2="' + (height / 2) + '" stroke="hsl(' + hue + ', 20%, 33%)" stroke-width="' + strw + '" />';
        svg += '</svg>';
        div.css({
            'background-image': "url('data:image/svg+xml," + svg.replace(/#/g, '%23') + "')",
            'background-repeat': 'no-repeat',
            'background-position': positionTop ? '50% 0%' : '50% 50%',
            'background-size': 'fit'
        });
    }

    musics = {
        Music1: { url: '/files/MyLittleQuiz/GeneratedMusic/Music1.ogg', fadeIn: 0, fadeOut: 1, volume: .3 },
        Music2: { url: '/files/MyLittleQuiz/GeneratedMusic/Music2.ogg', fadeIn: 0, fadeOut: 1, volume: .3 },
        Music3: { url: '/files/MyLittleQuiz/GeneratedMusic/Music3.ogg', fadeIn: 0, fadeOut: 1, volume: .3 },
        Music4: { url: '/files/MyLittleQuiz/GeneratedMusic/Music4.ogg', fadeIn: 0, fadeOut: 1, volume: .3 },
    };

    jingleUrl = function (p) { return '/files/MyLittleQuiz/GeneratedMusic/' + p + '.ogg'; };

    transitions = {

        blank: function (p)
        {
            clearScreen(p.bgclass || 'blank');
        },

        setup: function (p)
        {
            $('#content').data('package', p.graphicsPackage);
            clearScreen('setup');
            div = $('<div class="setup phrase away">').appendTo(content).addClassDelay('in');

            var msgs = [
                'The administrator pony is setting stuff up...',
                'Ponying something up...',
                'Now I’m awoken and I’m setting something up!',
                'Isn’t it great to be set up?',
                'Fillies and Gentlecolts! Setup in progress!',
                'While I’m setting this up, will you read my fanfic?',
                'Silly foals with silly dreams, together while I set this up...',
                'Pegasus, fly! Fly far while I set this up here...',
                'Setting up the Magic of Friendship...',
                'Dear Princess Celestia, I’m setting this up.',
                'Take me to the place where everything’s set up',
                'Let’s get this party set up!',
                'I can set this up in 10 seconds flat...',
                'It needs... about 20% more setup.',
                'Until I’ve set this up, join the herd!',
                'I watch it for the setup.',
                'Can you do that? Can you set it up twice?',
                'What fun is there in ever setting it up?',
                'Human beings fascinate me, setting up the way they do...',
                'Don’t set up at night.'
            ];
            var msg = msgs[Math.floor(Math.random() * msgs.length)];
            div.html(msg);
            findBestValue(100, function (fs)
            {
                div.css('font-size', fs + 'px');
                if (div.outerWidth() > content.width() * 2 / 3)
                    return 1;
                if (div.outerHeight() > content.height() * 3 / 5)
                    return 1;
                return -1;
            });
        },

        intro: function (p)
        {
            $('#content').data('package', p.graphicsPackage);
            clearScreen('intro');
            $('#content *').not('video').remove();
            $('#intro').addClass('visible');
            $('#intro')[0].pause();
            $('#intro')[0].currentTime = 0;
            $('#intro')[0].play();
        },

        //#region QUESTION/ANSWER (showQ, showA, showQA)
        showQ: function (p)
        {
            $('#qa').remove();
            clearScreen(p.round);

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
        r1_intro: function (p)
        {
            $('.r1-intro').remove();
            clearScreen('r1');

            var w = content.width();
            var h = content.height();
            for (var i = 0; i < 50; i++)
            {
                var tr = 'ease-in-out ' + (1 + 2 * Math.random()) + 's';
                var transition = 'transition: transform ' + tr + ', left ' + tr + ', top ' + tr + ', opacity linear ' + (Math.random()) + 's;';
                var angle = Math.random() * 2 * Math.PI;
                var muffin = $('<div class="away muffin muffin' + (1 + Math.floor(Math.random() * 3)) + '">').appendTo(content)[0];
                muffin.style =
                    'left:' + (w / 2 + 2 * w * Math.cos(angle)) + 'px;' +
                    'top:' + (h / 2 + 2 * w * Math.sin(angle)) + 'px;' +
                    'transform: translate(-50%, -50%) rotate(' + (3600 * Math.random()) + 'deg);' +
                    transition;
                window.setTimeout(function (m, tr)
                {
                    return function ()
                    {
                        m.style =
                            'left:' + (w * Math.random()) + 'px;' +
                            'top:' + (h * Math.random()) + 'px;' +
                            'transform: translate(-50%, -50%) rotate(' + (360 * Math.random()) + 'deg);' +
                            tr;
                    }
                }(muffin, transition), 500 + 60 * i);
            }

            var derpyHeight = content.height() * 95 / 100;
            var derpyWidth = derpyHeight * 6750 / 3931;
            if (derpyWidth > content.width())
            {
                derpyWidth = content.width();
                derpyHeight = derpyWidth * 3931 / 6750;
            }
            var derpy = $('<div class="r1-intro away" id="derpy">')
                .addClassDelay('in')
                .appendTo(content);

            var div1 = $('<div id="r1-intro-title" class="r1-intro static away">')
                .text('Round 1')
                .addClassDelay('in')
                .appendTo(content)
                .css({ left: content.width() - derpyWidth });
        },

        r1_showContestants: function (p)
        {
            $('#r1-contestants').remove();
            clearScreen('r1');
            var c = $('<div id="r1-contestants" class="full-flow away">').appendTo(content);
            for (var i = 0; i < p.contestants.length; i++)
            {
                var div = $('<div class="contestant">')
                    .attr('data-num', p.contestants[i].Number)
                    .css('transition-delay', Math.random() * .75 + 's')
                    .appendTo(c);
                if (p.contestants[i].HasCorrect)
                    div.addClass('correct');
                if (p.contestants[i].HasWrong)
                    div.addClass('wrong');
            }
            findBestValue(100, function (fs) { c.css('font-size', fs + 'px'); return c.outerHeight() < content.height() ? -1 : 1; });
            $('.contestant').addClassDelay('in');
        },

        r1_select: function (p)
        {
            $('.r1-contestant').remove();
            clearScreen('r1');

            var span = $('<span class="inner">').text(p.contestant.Name);
            var div = $('<div class="r1-contestant away static" id="r1-contestant-name">').append(span).appendTo(content).addClassDelay('in');
            $('<div class="r1-contestant away" id="r1-contestant-roll">').text(p.contestant.Roll).appendTo(content).addClassDelay('in');

            if (span.width() > div.width())
                span.css('transform', 'scale(' + (div.width() / span.width()) + ', 1)');
        },
        //#endregion

        //#region ROUND 2 (Categories)
        r2_intro: function (p)
        {
            $('.r2-intro').remove();
            clearScreen('r2');

            $('<div class="r2-intro r2-intro-title away static" id="r2-intro-title">').addClassDelay('in').appendTo(content)
                .append($('<span>').text('Round 2'));
            $('<div class="r2-intro away" id="minuette">').addClassDelay('in').appendTo(content);
            $('<div class="r2-intro r2-intro-title away static" id="r2-intro-subtitle">').addClassDelay('in').appendTo(content)
                .append($('<span>').text('Categories'));
            $('<div class="r2-intro away" id="roseluck">').addClassDelay('in').appendTo(content);
        },

        r2_showContestants: function (p)
        {
            $('#r2-contestants').remove();
            clearScreen('r2');

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
        r3_intro: function (p)
        {
            $('.r3-intro').remove();
            clearScreen('r3');

            $('<div class="r3-intro away static" id="r3-intro-title">').addClassDelay('in').appendTo(content)
                .append($('<span>').text('Round 3'));
            $('<div class="r3-intro away" id="lyra">').addClassDelay('in').appendTo(content);
            $('<div class="r3-intro away" id="bonbon">').addClassDelay('in').appendTo(content);
        },

        r3_showContestants: function (p)
        {
            $('#r3-contestants').remove();
            clearScreen('r3');

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
            clearScreen('r3');

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

            $('#r3-teams>.contestant').each(function ()
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
            clearScreen('r3');

            var div = $('<div id="r3-set" class="away">').appendTo(content).addClassDelay('in');
            var inner = $('<div class="name static">').appendTo(div).html(p.set);
            findBestValue(100, function (fs)
            {
                div.css('font-size', fs + 'px');
                return inner.outerHeight() < div.height() ? -1 : 1;
            });
        },

        r3_play_transition: function (p)
        {
            transitions.r3_play(p, true);
        },

        r3_play: function (p, doTransition)
        {
            if (p.answers.length < 1)
                return;

            var alreadyFontSize = null;
            if ($('#r3-play').length)
                alreadyFontSize = parseFloat($('#r3-play').css('font-size'));

            $('#r3-play').remove();
            clearScreen('r3', '#r3-bid,#r3-strikes');

            var div = $('<table id="r3-play" class="away static">');
            if ($('#r3-bid').length)
                div.insertBefore($('#r3-bid'));
            else
                div.appendTo(content);
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
                    $('<div class="ans' + (i < p.answers.length ? ((p.answers[i] === null ? ' wrong' : '') + (i < p.answers.length - 1 || !doTransition ? ' in' : '')) : ' invisible') + '">').data('index', i).append(
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

            if (doTransition)
            {
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
            }
        },

        r3_reveal: function (p)
        {
            $('#r3-reveal').remove();
            clearScreen('r3');

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
                    if (div.outerHeight() > content.height())
                        return 1;
                    if (table.outerWidth() + parseFloat(div.css('padding-left')) + parseFloat(div.css('padding-right')) > content.width())
                        return 1;
                    return -1;
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
        r4_intro: function (p)
        {
            $('.r4-intro').remove();
            clearScreen('r4');

            var span1 = $('<span>').text('Round 4');
            var div1 = $('<div id="r4-intro-title" class="r4-intro r4-style static away">')
                .append(span1)
                .addClassDelay('in')
                .appendTo(content);

            findBestValue(100, function (fs)
            {
                div1.css('font-size', fs + 'px');
                if (span1.outerWidth() > content.width() * 1 / 2)
                    return 1;
                return -1;
            });

            var factor = .8;
            r4_addBackground(div1, content.width() / 2, div1.height(), 0, false, factor);
            div1.css('padding-left', (content.width() - span1.outerWidth()) * factor);

            var span2 = $('<span>').text('Sudden Death');
            var div2 = $('<div id="r4-intro-subtitle" class="r4-intro r4-style static away">')
                .append(span2)
                .addClassDelay('in')
                .appendTo(content);

            findBestValue(100, function (fs)
            {
                div2.css('font-size', fs + 'px');
                if (span2.outerWidth() > content.width() * 1 / 2)
                    return 1;
                return -1;
            });

            factor = .9;
            r4_addBackground(div2, content.width() / 2, div2.height(), 60, false, factor);
            div2.css('padding-left', (content.width() - span2.outerWidth()) * factor);

            // Octavia
            $('<div class="r4-intro away" id="octavia">').addClassDelay('in').appendTo(content);
        },

        r4_showContestants: function (p)
        {
            $('#r4-contestants').remove();
            clearScreen('r4');

            var div = $('<div id="r4-contestants" class="static away">').appendTo(content);
            var innerDiv = $('<div class="r4-style away">').appendTo(div).addClassDelay('in', 500);
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
            var allHave = true;
            for (var i = 0; i < p.answers.length; i++)
            {
                if (p.answers[i].length > cols)
                    cols = p.answers[i].length;
                else if (p.answers[i].length < p.answers[0].length)
                    allHave = false;
            }
            if (allHave && p.answers[0].length >= p.minAnswers)
                cols++;
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
                if (div.outerHeight() > content.height() / 3)
                    return 1;
                if (table.outerWidth() > content.width() * 4 / 5)
                    return 1;
                return -1;
            });

            r4_addBackground(innerDiv, table.outerWidth(), table.outerHeight(), 30, true);
        },
        //#endregion

        'congratulations': function (p)
        {
            $('.congratulations').remove();
            clearScreen('congratulations');

            var stripes = $('<div class="congratulations away stripes">').appendTo(content).addClassDelay('in');
            for (var i = 0; i < 10; i++)
                $('<div class="congratulations stripe">').appendTo(stripes).css({
                    transform: 'translateX(-50%) rotate(' + (i * 360 / 10) + 'deg)'
                });

            $('<div class="congratulations away" id="trophy">').appendTo(content);
            var winnerdiv = $('<div class="congratulations static away" id="winner-name">').appendTo(content)
                .append($('<div class="inner">')
                    .append($('<span class="name">').text(p.winner)));

            var nameWidth = $('#winner-name .name').width();
            if (nameWidth > winnerdiv.width())
                $('#winner-name .inner').css({ transform: 'scaleX(' + (winnerdiv.width() / nameWidth) + ')' });

            $('<div class="congratulations away" id="logo">').appendTo(content).addClassDelay('in');
        }
    };
});
