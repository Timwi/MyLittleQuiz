$.fn.addClassDelay = function (cls, delay)
{
    var t = this;
    window.setTimeout(function () { t.addClass(cls); }, delay || 100);
    return t;
};

$(function ()
{
    var socket;
    var currentMusic = null;

    function newSocket()
    {
        socket = new WebSocket($(document.body).data('socket-url'));
        socket.onopen = function ()
        {
            console.log('socket open');
            socket.send("ping");
        };
        socket.onclose = function ()
        {
            console.log('socket close');
            window.setTimeout(newSocket, 1000);
        };
        socket.onmessage = function (msg)
        {
            console.log('socket message: ' + msg.data);
            var data = JSON.parse(msg.data);
            if ('method' in data)
            {
                if (data.method in transitions)
                    transitions[data.method](data.params);
                else
                    console.log('Undefined transition method: ' + data.method);
            }
            if ('music' in data && data.music !== currentMusic)
            {
                var prevMusic = $('#music');
                if (prevMusic.length)
                {
                    prevMusic.data('cancel', true);
                    var fadeOutTime = prevMusic.data('fadeout');
                    prevMusic.attr('id', '');
                    if (!fadeOutTime)
                        prevMusic.remove();
                    else
                    {
                        var fiCounter = 100;
                        for (var i = 1; i <= 100; i++)
                        {
                            (function (i2)
                            {
                                window.setTimeout(function ()
                                {
                                    prevMusic[0].volume = (100 - i2) / 100;
                                    fiCounter--;
                                    if (fiCounter === 0)
                                        prevMusic.remove();
                                }, fadeOutTime * 10 * i2);
                            })(i);
                        }
                    }
                }

                if (!(data.music in musics))
                    currentMusic = null;
                else
                {
                    currentMusic = data.music;
                    var inf = musics[currentMusic];
                    var newMusic = $('<audio id="music" src="' + inf.url + '">').appendTo(document.body);
                    newMusic[0].volume = 0;
                    newMusic[0].play();
                    newMusic.data('fadeout', inf.fadeOut);

                    // TODO: ability to specify music volume
                    var fullVolume = 1;

                    if (!inf.fadeIn)
                        newMusic[0].volume = fullVolume;
                    else
                    {
                        var foCounter = 100;
                        for (var i = 1; i <= 100; i++)
                        {
                            (function (i2)
                            {
                                window.setTimeout(function ()
                                {
                                    foCounter--;
                                    if (foCounter === 0)
                                        newMusic[0].volume = fullVolume;
                                    else
                                        newMusic[0].volume = fullVolume * i2 / 100;
                                }, inf.fadeIn * 10 * i2);
                            })(i);
                        }
                    }
                }
            }
        };
    }

    newSocket();

    $(document.body).keypress(function (e)
    {
        if (e.keyCode === 99)   // 'c'
        {
            $(document.body).css('cursor', 'default');
        }
        else if (e.keyCode === 114)   // 'r'
        {
            $(document.body).css('cursor', 'none');
        }
    });
});

function findBestValue(startingValue, evaluator, threshold, preferHigh)
{
    startingValue = +startingValue;
    threshold = threshold || .1;
    preferHigh = !!preferHigh;
    var iter = 0;

    var negative = startingValue < 0;
    var low = negative ? startingValue : 0;
    var high = startingValue == 0 ? 1 : (negative ? 0 : startingValue);
    if (negative)
    {
        while (evaluator(low) > 0)
        {
            iter++;
            if (iter > 1000) { alert('iteration failed'); return; }
            high = low;
            low *= 2;
        }
    }
    else
    {
        while (evaluator(high) < 0)
        {
            iter++;
            if (iter > 1000) { alert('iteration failed'); return; }
            low = high;
            high *= 2;
        }
    }

    while (high - low > threshold)
    {
        iter++;
        if (iter > 1000) { alert('iteration failed'); return; }
        var mid = (high + low) / 2;
        var ev = evaluator(mid);
        if (ev === 0)
            return mid;
        if (ev < 0)
            low = mid;
        else
            high = mid;
    }
    var finalValue = preferHigh ? high : low;
    evaluator(finalValue);
    return finalValue;
}
