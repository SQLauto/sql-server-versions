/******************************************************************************
    event handlers
******************************************************************************/
$(".input-box").blur(function () {
    var elementValue = $(this).val();

    $.isNumeric(elementValue) && elementValue.indexOf(".") < 0 ?
        $(this).css("color", "black") :
        $(this).css("color", "red");
});

$(".input-box").focus(function () {
    $(this).css("color", "black");
    $(this).select();
});
$(".output-data").focus(function () {
    $(this).css("color", "black");
    $(this).select();
});

$("#releaseDate").blur(function () {
    isValidDateFormat($("#releaseDate").val()) ?
        $("#releaseDate").css("color", "black") : $("#releaseDate").css("color", "red");
});

$("#search").click(function () {
    // reset the environment
    //
    clearMessage();
    $(".ref-link").remove();
    $(".ref-link-anchor").remove();

    if (!isVersionDataValid()) {
        $(this).css("color", "red");
    }
    else {
        $(this).css("color", "black");
        requestVersionData();
    }
});

$("#add").click(function () {
    clearMessage();

    if (isAllInputDataValid()) {
        addNewVersionData();
    }
    else {
        setMessage("bad data!", true);
    }
});

$("#addRefLink").click(function (e) {
    e.preventDefault();

    /*
    if ($(".ref-link").length == 0)
        $("<br /><input class=\"ref-link\" />").insertAfter($("#addRefLink"));
    else {
        // if the user hasn't entered a ref link in 
        // the last ref link input box then don't let 
        // them keep adding more
        //
        if ($(".ref-link").last().val() == "")
            return;

        $("<input class=\"ref-link\" />").insertAfter($(".ref-link").last());
    }
    */

    var appendElement = "<input class=\"ref-link\" />";
    if ($(".ref-link").length == 0)
        appendElement = "<br />" + appendElement;
    else
        if ($(".ref-link").last().val() == "")
            return;

    $("#refLinksCol").append(appendElement);

    $(".ref-link").last().focus();
});

$(document).ready(function () {
    $(".input-box").first().focus();
});
/******************************************************************************
    end event handlers
******************************************************************************/

function isVersionDataValid() {
    var dataIsValid = true;

    // loop through all of the intput boxes and make sure that 
    // they contain only numerica data
    //
    // it is assumed that length is fine because of HTML maxlength
    //
    $(".input-box").each(function (idx, element) {
        dataIsValid =
            $.isNumeric($(this).val()) &&
            $(this).val().indexOf(".") < 0;

        return dataIsValid;
    });

    return dataIsValid;
}

function isAllInputDataValid() {
    return (
        isVersionDataValid() &&
        isValidDateFormat($("#releaseDate").val())
        );
}

function requestVersionData() {
    var url = $(location).attr("href") + "api/version/";

    // at this point we already know the data is inputed and valid 
    // so we just need to start appending
    //
    url +=
        $("#major").val() + "/" +
        $("#minor").val() + "/" +
        $("#build").val() + "/" +
        $("#revision").val();

    $.getJSON(url, function (data) {
        if (data) {
            //$("#search").css("color", "blue");
            displayData(data);
        }
        else {
            formatToRequestData();
        }
    })
        .fail(function (data) {
            $("#search").css("color", "red");
        });
}

function displayData(data) {
    var newRefLinkElement;

    $("#allVersionData").css("visibility", "visible");
    $("#referenceLinksData").css("visibility", "visible");
    $("#addRefLink").css("visibility", "collapse");

    $("#friendlyNameLong").attr("readonly", "readonly");
    $("#friendlyNameShort").attr("readonly", "readonly");
    $("#releaseDate").attr("readonly", "readonly");

    $("#add").css("visibility", "hidden");

    $("#friendlyNameLong").css("background-color", "transparent");
    $("#friendlyNameShort").css("background-color", "transparent");
    $("#releaseDate").css("background-color", "transparent");

    $("#friendlyNameLong").val(data.FriendlyNameLong);
    $("#friendlyNameShort").val(data.FriendlyNameShort);
    $("#releaseDate").val(getFormattedDate(data.ReleaseDate));

    for (var i = 0; i < data.ReferenceLinks.length; i++) {
        // add an anchor tag for each link
        //
        newRefLinkElement = "<a class=\"ref-link-anchor\" href=\"" + data.ReferenceLinks[i] + "\">" + data.ReferenceLinks[i] + "</a>";
        if (i > 0)
            newRefLinkElement = "<br />" + newRefLinkElement;

        $(newRefLinkElement).insertBefore($("#addRefLink"));
    }
}

function formatToRequestData() {
    $("#allVersionData").css("visibility", "visible");
    $("#referenceLinksData").css("visibility", "visible");
    $("#addRefLink").css("visibility", "visible");

    $("#friendlyNameLong").val("");
    $("#friendlyNameShort").val("");
    $("#releaseDate").val("");

    $("#add").css("visibility", "visible");

    $("#friendlyNameLong").removeAttr("readonly");
    $("#friendlyNameShort").removeAttr("readonly");
    $("#releaseDate").removeAttr("readonly");

    $("#friendlyNameLong").css("background-color", "white");
    $("#friendlyNameShort").css("background-color", "white");
    $("#releaseDate").css("background-color", "white");

    $("#friendlyNameLong").focus();
}

function addNewVersionData() {
    var newVersionInfo = {
        Major: $("#major").val(),
        Minor: $("#minor").val(),
        Build: $("#build").val(),
        Revision: $("#revision").val(),
        FriendlyNameLong: $("#friendlyNameLong").val(),
        FriendlyNameShort: $("#friendlyNameShort").val(),
        ReleaseDate: $("#releaseDate").val(),
        ReferenceLinks: getAllInputReferenceLinks()
    };
    var url = $(location).attr("href") + "api/version/";
    var newData = JSON.stringify(newVersionInfo);

    $.ajax({
        type: "POST",
        url: url,
        contentType: "application/json",
        data: newData
    })
        .success(function () {
            //$("#add").css("color", "green");
            setMessage("got it! thanks!", false);
            setTimeout(function () {
                $("#search").click();
            }, 2000);
            //$("#search").delay(10000).click();
            //$("#search").click();
        })
        .fail(function () {
            setMessage("uh oh... something went wrong", true);
        });
}

function getFormattedDate(dateString) {
    var dateObj = new Date(dateString);
    var regEx = new RegExp();

    return (
        (dateObj.getMonth() + 1) + "/" +
        dateObj.getDate() + "/" +
        dateObj.getFullYear()
    );
}

function isValidDateFormat(dateString) {
    var datePattern = /^(0?[1-9]|1[012])[- /.](0?[1-9]|[12][0-9]|3[01])[- /.](19|20)\d\d$/;
    var matches = dateString.match(datePattern);

    if (matches)
        return true;
    else
        return false;
}

function setMessage(message, isError) {
    isError ? $("#errorText").css("color", "red") : $("#errorText").css("color", "blue");

    $("#errorText").text(message);

    $("#errorData").css("visibility", "visible");
}

function clearMessage() {
    $("#errorText").text("");
    $("#errorData").css("visibility", "hidden");
}

function getAllInputReferenceLinks() {
    var allRefLinks = [];

    $(".ref-link").each(function () {
        if ($(this).val() != "")
            allRefLinks[allRefLinks.length] = $(this).val().trim();
    });

    if (allRefLinks.length == 0)
        return null;
    else
        return allRefLinks;
}