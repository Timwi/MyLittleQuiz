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
                    prevMusic.attr('id', '');
                    var fadeOutCounter = 10;
                    var fadeOutIntervalId = window.setInterval(function()
                    {
                        fadeOutCounter--;
                        prevMusic[0].volume = fadeOutCounter/10;
                        if (fadeOutCounter === 0)
                        {
                            window.clearInterval(fadeOutIntervalId);
                            prevMusic.remove();
                        }
                    }, 100);
                }

                currentMusic = data.music;
                var newMusic = $('<audio id="music" src="'+currentMusic+'">').appendTo(document.body);
                newMusic[0].volume = 0;
                newMusic[0].play();
                var fadeInCounter = 0;
                var fadeInIntervalId = window.setInterval(function()
                {
                    fadeInCounter++;
                    if (fadeInCounter === 10 || newMusic.data('cancel'))
                        window.clearInterval(fadeInIntervalId);
                    newMusic[0].volume = fadeInCounter/10;
                }, 100);
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
