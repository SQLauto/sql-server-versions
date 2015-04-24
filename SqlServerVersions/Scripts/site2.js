﻿function isDateValid(dateInput) {
    var dateRegex = new RegExp(/\b\d{1,2}[\/-]\d{1,2}[\/-]\d{4}\b/);
    return dateRegex.test(dateInput);
}

function isAllInputThereAndValid() {
    if (
        $("#input-friendlynamelong").val().trim() &&
        $("#input-friendlynameshort").val().trim() &&
        isDateValid($("#input-releasedate").val()) &&
        $("#input-referencelink").val().trim()) {
        return true;
    }
    else {
        return false;
    }
}

function enableSubmitIfNecessary() {
    if (isAllInputThereAndValid()) {
        $("#add-new-build").removeAttr("disabled");
    }
    else {
        $("#add-new-build").attr("disabled", "disabled");
    }
}

$(document).ready(function () {
    $("#input-friendlynamelong").keyup(function () {
        $("#repeat-friendlynamelong").text($(this).val());
        enableSubmitIfNecessary();
    });

    $("#input-friendlynameshort").keyup(function () {
        $("#repeat-friendlynameshort").text($(this).val());
        enableSubmitIfNecessary();
    });

    $("#input-releasedate").keyup(function () {
        enableSubmitIfNecessary();
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
                $("#repeat-releasedate").text("Released on: " + releaseDateInput);
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
            $("#hidden-supported").val("False");
            $(this).removeClass("backfill-supported").addClass("backfill-unsupported");

            $("#repeat-issupported").text("Unsupported");
            $("#repeat-issupported").removeClass("backfill-supported").addClass("backfill-unsupported");
        }
        else {
            $(this).text("supported");
            $("#hidden-supported").val("True");
            $(this).removeClass("backfill-unsupported").addClass("backfill-supported");

            $("#repeat-issupported").text("Supported");
            $("#repeat-issupported").removeClass("backfill-unsupported").addClass("backfill-supported");
        }
    });

    $("#input-referencelink").keyup(function () {
        var inputRefLink = $(this).val();
        $("#repeat-referencelink").attr("href", inputRefLink);
        $("#repeat-referencelink").text(inputRefLink);
        enableSubmitIfNecessary();
    });
});