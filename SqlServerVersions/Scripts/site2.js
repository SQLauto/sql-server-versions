function isDateValid(dateInput) {
    var dateRegex = new RegExp(/\b\d{1,2}[\/-]\d{1,2}[\/-]\d{4}\b/);
    return dateRegex.test(dateInput);
}

$(document).ready(function () {
    $("#input-friendlynamelong").keyup(function () {
        $("#repeat-friendlynamelong").text($(this).val());
    });

    $("#input-friendlynameshort").keyup(function () {
        $("#repeat-friendlynameshort").text($(this).val());
    });

    $("#input-releasedate").change(function () {
        var releaseDateInput = $(this).val();

        if (!releaseDateInput.trim()) {
            $("#repeat-releasedate").text("");
            $(".date-error").css("visibility", "hidden");
        }
        else {
            if (isDateValid(releaseDateInput)) {
                $(".date-error").css("visibility", "hidden");
                $("#repeat-releasedate").text(releaseDateInput);
            }
            else {
                $("#repeat-releasedate").text("");
                $(".date-error").css("visibility", "visible");
            }
        }
    });

    $("#input-supported").click(function () {
        if ($(this).hasClass("backfill-supported")) {
            $(this).text("unsupported");
            $(this).removeClass("backfill-supported").addClass("backfill-unsupported");

            $("#repeat-issupported").text("Unsupported");
            $("#repeat-issupported").removeClass("backfill-supported").addClass("backfill-unsupported");
        }
        else {
            $(this).text("supported");
            $(this).removeClass("backfill-unsupported").addClass("backfill-supported");

            $("#repeat-issupported").text("Supported");
            $("#repeat-issupported").removeClass("backfill-unsupported").addClass("backfill-supported");
        }
    });

    $("#input-referencelink").keyup(function () {
        var inputRefLink = $(this).val();
        $("#repeat-referencelink").attr("href", inputRefLink);
        $("#repeat-referencelink").text(inputRefLink);
    });
});
