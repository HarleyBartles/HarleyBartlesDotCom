$(function () {
    $('.validate-availability').change(function () {
        var $this = $(this);
        var name = $(this).val();
        console.log(name);
        $.ajax({
            url: '/Identity/NameAvailabilityCheck',
            data: {
                name: name
            }
        }).done(function (data) {
            var out = $('[data-valmsg-for="' + $($this).attr('name') + '"]');
            if (data === false) {
                $(out).text(name + " is Unvailable")
                    .removeClass('text-success')
                    .addClass('text-danger');
            } else {
                $(out).text(name + " is Available")
                    .removeClass('text-danger')
                    .addClass('text-success');
            }
        });
    });
});