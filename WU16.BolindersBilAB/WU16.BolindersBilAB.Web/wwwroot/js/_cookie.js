$(document).ready(function () {
    getCookie();
});


function acceptCookie()
{
    document.cookie = "accept_cookie=1";
    $(".cookie-note").hide(200);
}

function getCookie()
{
    if (document.cookie.indexOf("accept_cookie=") >= 0)
    {
        //Hide the cookie message
        $(".cookie-note").hide(200);
    }
    else {
        $(".cookie-note").show(200);
    }
}