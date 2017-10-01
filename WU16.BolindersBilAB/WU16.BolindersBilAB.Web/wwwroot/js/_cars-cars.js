$(document).ready(function () {
    $('#fuelType').select2();
    $('#carType').select2();
    $('#gearbox').select2();

    $('.car-list-item').on('click', function () {
        var licenseNumber = $(this).data('id');
        location.href = '/bil/' + licenseNumber
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