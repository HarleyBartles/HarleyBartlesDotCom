$(function () {
    new WOW().init();

    var clipboard = new ClipboardJS('.btn');

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
    });

    clipboard.on('error', function (e) {
        console.error('Action:', e.action);
        console.error('Trigger:', e.trigger);
        toastr.warning("Unable to copy automatically. Press CTRL+C to copy");
    });

    
});