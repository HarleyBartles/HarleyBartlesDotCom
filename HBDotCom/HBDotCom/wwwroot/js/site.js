$(function () {

    var headerSize = $('nav.navbar.navbar-default.navbar-fixed-top').outerHeight(true);
    var headerSpacers = $('.header.sizer');

    headerSpacers.each(function(){
        this.outerHeight = headerSize;
    });

    new WOW().init();

    //$('#owl-carousel').owlCarousel();
    var $owl = $('#owl-carousel'); 

    if ($owl.length > 0) {

        // Hide the overlays ready for wow to reveal them.
        $(".owl-overlay:not(:first) .wow").hide();

        $owl.owlCarousel(
            {
                items: 1,
                navigation: true, // Show next and prev buttons
                autoplay: true,
                loop: true,
                slideSpeed: 300,
                paginationSpeed: 1000,
                singleItem: true
            });

        //all other is the same
        $owl.on('translated.owl.carousel',
            function (event) {
                $(".active .wow").addClass("animated").show();
                $(".owl-item:not(.active) .wow").hide();//.removeClass("fadeInUp");
            });
    }

    
   
});