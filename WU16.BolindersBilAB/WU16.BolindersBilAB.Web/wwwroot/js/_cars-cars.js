$(document).ready(function () {
    $('#fuelType').select2();
    $('#carType').select2();
    $('#gearbox').select2();

    $('.card').on('click', function () {
        var url = $(this).data('url');
        location.href = url;
    });
    var timer;
    $(window).on('scroll', function () {
        if (timer)
        {
            clearTimeout(timer);
        }
        timer = setTimeout(function () {
            sessionStorage.scrollTop = $(this).scrollTop();
        }, 100);
    });

    if (sessionStorage.scrollTop !== "undefined")
    {
        $(window).scrollTop(sessionStorage.scrollTop);
    }
});