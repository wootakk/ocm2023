$(document).ready(function ($) {
    var header = '<select class="form-control valid" id="Acc_no" name="Acc_no">';
    var body = "";
    var options = document.getElementById("Acc_no");
    if (options != null) {
        options = options.options;
        for (var i = 0; i < options.length; i++) {
            body = body + '<option value=' + options[i].value + '>' + options[i].textContent + '</option> ';
        }
    }
    
    var footer = '</select>';
    var dropDown = header + body + footer;


    var html = '<div class="row"><div class="col-md-4">' + dropDown + '<span class="field-validation-valid text-danger" data-valmsg-for="Acc_no" data-valmsg-replace="true"></span></div><div class="col-md-4"><input class="form-control number text-box single-line" data-val="true" data-val-number="The field Budget must be a number." id="Budget" name="Budget" type="text" value=""><span class="field-validation-valid text-danger" data-valmsg-for="Budget" data-valmsg-replace="true"></span></div><div class="col-md-4" id="remove"><a class="btn-grid btn btn-danger">-</a></div></div></div>';
    var Html_Transfer = '<div class="row">    <div class="col-md-10"><div class="col-md-3">                ' + dropDown + '            </div>            <div class="col-md-3">                <input class="form-control Transfer_Increase text-box single-line" data-val="true" data-val-number="Must be a number." id="transfer_increase" name="transfer_increase" type="text" value="">            </div>            <div class="col-md-3">                <input class="form-control text-box single-line Transfer_Decrease" data-val="true" data-val-number="Must be a number." id="transfer_decrease" name="transfer_decrease" type="text" value="">            </div>            <div class="col-md-3">                <input class="form-control text-box single-line Transfer_Add" data-val="true" data-val-number="Must be a number." id="transfer_add" name="transfer_add" type="text" value="">            </div>        </div>   <div class="col-md-2" id="remove"> <a class="btn-grid btn btn-danger">-</a></div></div>';

    var HTML_Mandate = '<div class="row"><div class="form-group"><div class="col-md-3"><label class="control-label" for="Acc_no">Account Number</label>' + dropDown + '<span class="field-validation-valid text-danger" data-valmsg-for="Acc_no" data-valmsg-replace="true"></span></div><div class="col-md-3"><label class="control-label" for="Acc_no">FCV Number</label><input type="text" name="FCV_no" id="FCV_no" value="" class="form-control FCV_no" /><span class="field-validation-valid text-danger" data-valmsg-for="FCV_no" data-valmsg-replace="true"></span></div><div class="col-md-3"><label class="control-label" for="Mandate_amount">Mandate Amount</label><input type="text" name="Mandate_amount" value="" class="form-control Mandate_amount"><span class="field-validation-valid text-danger" data-valmsg-for="Mandate_amount" data-valmsg-replace="true"></span></div><div class="col-md-3" id="remove"><label class="control-label"><br></label><br><a class="btn-grid btn btn-danger">-</a></div></div></div>';

    $("#add").click(function (e) {
        $("#container").append(html);
        $('[name="Acc_no"]').chosen({

        });
    });
    //Transfer
    $("#add_transfer").click(function (e) {
        $("#container").append(Html_Transfer);
        $('[name="Acc_no"]').chosen({

        });
    });
    //Mandate
    $('#add_mandate').click(function (e) {
        $("#container").append(HTML_Mandate);
    });
    //Remove
    $("#container").on('click', '#remove', function () {
        $(this).parent('div').remove();
    });
});


