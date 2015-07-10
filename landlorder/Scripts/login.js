jQuery(document).ready(function ($) {
    tab = $('.tabs h3 a');

    tab.on('click', function (event) {
        event.preventDefault();
        tab.removeClass('active');
        $(this).addClass('active');

        tab_content = $(this).attr('href');
        $('div[id$="tab-content"]').removeClass('active');
        $(tab_content).addClass('active');
    });


    $('#login').click(function () {
        $('#login-tab').addClass('class=active');
        $('#login-tab-content').addClass('class=active');
    });
    $('#signup').click(function () {
        $('#login-tab').addClass('class=active');
        $('#login-tab-content').addClass('class=active');
    });
});



