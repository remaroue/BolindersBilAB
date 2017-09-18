var api_key = "40f2eef1b2884b309ff075ca16508d76";
$.ajax({
    url: "https://newsapi.org/v1/articles?source=the-next-web&sortBy=latest&apiKey=" + api_key,
    success: function (result) {
        console.log(result);

        $.each(result.articles, function (key, value) {
            console.log(value);

            const url = "https://i.ytimg.com/vi/d_T5P-zIIAs/maxresdefault.jpg";

            var article = [
                ' <div class="card col-md-4" style="border:none;">',
                '<img class="card-img-top" src="' + value.urlToImage + '" alt="Card image cap">',
                '<div class="card-body">',
                '<h4>'+ value.title +'</h4>',
                '<p class="card-text">Some quick example text to build on the card title and make up the bulk of the cards content.</p > ',
                '</div>',
                '</div>'].join("");

            $(".news-feed .row").append(article);
        });
    }
});