$(function () {
    new WOW().init();

    var clipboard = new ClipboardJS('.btn');

    clipboard.on('success', function (e) {
        console.info('Action:', e.action);
        console.info('Text:', e.text);
        console.info('Trigger:', e.trigger);
        toastr.info("Copied")
        e.clearSelection();
    });

    clipboard.on('error', function (e) {
        console.error('Action:', e.action);
        console.error('Trigger:', e.trigger);
    });

    $('#MakeLink').on('click', function () {
        var name = $('#TwitterName').val();
        $.ajax({
            url: "/Tools/MakeSearchLink",
            data: {
                userName: name
            }
        }).done(function (data) {
            var output = $('#SearchLink');
            var copyBtn = $('#CopyLink');
            $(output).val(data);
            $('.col.invisible').removeClass('invisible');
        });
    });
});