$(function () {
    $("#userName").change(function () {
        var username = $(this).val();
        $.ajax({
            url: '/Identity/UserNameCheck',
            data: {
                username: username
            }
        }).done(function (data) {
            if (data === false) {
                alert(username + " is Unvailable");
            } else {
                alert(username + " is Available");
            }
        });
    });
});