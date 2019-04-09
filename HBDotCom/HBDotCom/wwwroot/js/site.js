$(function () {
    new WOW().init();

    var clipboard = new ClipboardJS('.btn');
    var ga = new ga();

    toastr.options = {
        "timeOut": 3000,
        "positionClass": "toast-top-center mt-5",
        "showMethod": "slideDown"
    };

    clipboard.on('success', function (e) {
        console.info('Action:', e.action);
        console.info('Text:', e.text);
        console.info('Trigger:', e.trigger);
        toastr.info("Copied");
        e.clearSelection();
        ga('send', 'event', 'link copied', 'click'); 
    });

    clipboard.on('error', function (e) {
        console.error('Action:', e.action);
        console.error('Trigger:', e.trigger);
        toastr.warning("Unable to copy automatically. Press CTRL+C to copy");
    });

    // Binds enter keypress to the click function of the next button in the DOM
    $('input[type="text"]').keypress(function (e) {
        var key = e.which;
        if (key === 13)  // the enter key code
        {
            $(this).next('.btn').click();
            return false;
        }
    });
    
});