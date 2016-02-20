$(function () {
    $(".newCaptcha").click(function (e) {
        var cmd = $(this);
        var url = cmd.prev(".captchaContainer").find("img").first().attr("src");
        url = url.replace(/(.*?)\?r=\d+\.\d+$/, "$1")
        if (typeof console != "undefined") {
			console.log(url);
		}
        var img = new Image();
        $(img).load(function () {
            cmd.prev(".captchaContainer").empty().append(img);
        });
        img.src = url + "?r=" + Math.random();
        e.preventDefault();
    });
});