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

    transitions = {

    };
});