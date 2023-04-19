$(function() {
    stopEvent = function (ffevent) {
        var current_window = window;
        if (current_window.event) //window.event is IE, ffevent is FF
        {
            current_window.event.stopPropagation(); //this stops event propagation
            current_window.event.preventDefault(); //this prevents default (usually is what we want)
        }
        else {
            //Firefox
            ffevent.stopPropagation();
            ffevent.preventDefault();
        };
    }

    $("#btnSubmit").click(function (event) {
        var account = $("#accountNumber").val();
        var unit = $("#unitId").val();
        var startDate = $("#st").val();
        var endDate = $("#et").val();

        if (startDate == "") {
            $("#startDateError").text("សូមជ្រើសរើសថ្ងៃចាប់ផ្ដើម");
            stopEvent(event);
        } else {
            $("#startDateError").text("");
        }
        if (endDate == "") {
            $("#endDateError").text("សូមជ្រើសរើសថ្ងៃបញ្ចប់");
            stopEvent(event);
        } else {
            $("#endDateError").text("");
        }
    });
});