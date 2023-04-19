$(document).ready(function (e) {
    alert("Hello");
    var header = '<select class="form-control valid" id="Acc_no" name="Acc_no">';
    var body = "";
    var options = document.getElementById("Acc_no").options;
    for (var i = 0; i < options.length; i++) {
        body = body + '<option value=' + options[i].value + '>' + options[i].textContent + '</option> ';
    }
    var footer = '</select>';
    var dropDown = header + body + footer;
    var html = '<div class="row"><div class="form-group"><label class="control-label col-md-2" for="Acc_no">Account Number</label><div class="col-md-3">' + dropDown + '<span class="field-validation-valid text-danger" data-valmsg-for="Acc_no" data-valmsg-replace="true"></span></div><div class="col-md-2"><label class="control-label col-md-2" for="Budget">Budget</label></div><div class="col-md-3"><input class="form-control text-box single-line" data-val="true" data-val-number="The field Budget must be a number." id="Budget" name="Budget" type="text" value=""><span class="field-validation-valid text-danger" data-valmsg-for="Budget" data-valmsg-replace="true"></span></div><div class="col-md-2" id="remove"><a class="btn btn-danger">-</a></div></div></div>';

    $("#add").click(function (e) {
        $("#container").append(html);
    });
    //Remove
    $("#container").on('click', '#remove', function () {
        $(this).parent('div').remove();
    });
});


