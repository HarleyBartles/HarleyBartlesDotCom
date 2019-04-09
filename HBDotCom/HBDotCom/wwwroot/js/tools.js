$(function () {
    $('#MakeLink').on('click', function () {
        var name = $('#TwitterName').val();
        if (name.length > 0) {
            makeLink(name.trim());
        } else {
            toastr.warning("Please type a username");
        }
    });

    var makeLink = function (uName) {
        $.ajax({
            url: "/Tools/MakeSearchLink",
            data: {
                userName: uName
            }
        }).done(function (data) {
            var output = $('#SearchLink');
            $(output).val(data);
            $('.col-12.invisible').removeClass('invisible');
        });
    };
});