// INCLUDE JQUERY UI v1.12.1 JS
$(function () {
    var $number = $("#number");
    var $number2 = $("#number2");
    $number.html('0');
    $("#slider").slider({
        range: false,
        step: 1,
        min: 0,
        max: 99,
        value: 0,
        animate: "slow",
        orientation: "horizontal",
        slide: function (event, ui) {
            var prev = parseInt($number.text());
            var next = Math.round(ui.value);

            if (prev < next) {
                $number2.html(prev).addClass('prevOut');
                setTimeout(function () {
                    $number2.html('').removeClass('prevOut');
                }, 300);
                $number.html(next).addClass('nextIn');
                setTimeout(function () {
                    $number.removeClass('nextIn');
                }, 300);
            } else {
                $number2.html(prev).addClass('prevIn');
                setTimeout(function () {
                    $number2.html('').removeClass('prevIn');
                }, 300);
                $number.html(next).addClass('nextOut');
                setTimeout(function () {
                    $number.removeClass('nextOut');
                }, 300);
            }
            var divWidth = $('.bottomSlide').width();
            $('.bottomSlide > .tick').css('left', divWidth * (next / 99));

            // Highlights on the Ruler Texts
            if (next <= 6) {
                $('ul.ruler > li:nth-child(1)').addClass('active');
            } else if (next >= 27 && next <= 39) {
                $('ul.ruler > li:nth-child(2)').addClass('active');
            } else if (next >= 60 && next <= 72) {
                $('ul.ruler > li:nth-child(3)').addClass('active');
            } else if (next >= 94) {
                $('ul.ruler > li:nth-child(4)').addClass('active');
            } else {
                $('ul.ruler > li').removeClass('active');
            }
        }
    });
});