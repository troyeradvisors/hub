

$(document).ready(function () {
    $("a:contains('Delete')").click(function (event) {
        event.preventDefault()
        var url = $(this).attr('href');
        var confirm_box = confirm('Are you sure you want to delete?');
        if (confirm_box) {
            window.location = url;
            //uncomment below and remove above if you want the link to open in a new window
            //window.open(url,'_blank');
        }
    });

    $('input[id*=Date]').datepicker();

});