$(document).ready(function () {
    $("#input-friendlynamelong").keyup(function () {
        $("#repeat-friendlynamelong").text($(this).val());
    });

    $("#input-friendlynameshort").keyup(function () {
        $("#repeat-friendlynameshort").text($(this).val());
    });
});
