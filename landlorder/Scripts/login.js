jQuery(document).ready(function ($) {   

    $('.tabs h3 a').on('click', function (event) {
        tab = $('.tabs h3 a');
        event.preventDefault();
        tab.removeClass('active');
        $(this).addClass('active');

        tab_content = $(this).attr('href');
        $('div[id$="tab-content"]').removeClass('active');
        $(tab_content).addClass('active');
    });


   
});



