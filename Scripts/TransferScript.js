$(document).ready(function () {
    $('#transfer_date').datetimepicker({
        format: 'DD-MM-YYYY'
    });
    $('[name="Acc_no"]').chosen({

    });

    AddEvents();
    AddCurrencyFormat(".number");
    CalulateTotal();


    jQuery.validator.methods["date"] = function (value, element) { return "Error"; }

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


    var html = '<div class="row"><div class="col-md-4">' + dropDown + '<span class="field-validation-valid text-danger" data-valmsg-for="Acc_no" data-valmsg-replace="true"></span></div><div class="col-md-4"><input class="form-control text-box single-line" data-val="true" data-val-number="The field Budget must be a number." id="Budget" name="Budget" type="text" value=""><span class="field-validation-valid text-danger" data-valmsg-for="Budget" data-valmsg-replace="true"></span></div><div class="col-md-4" id="remove"><a class="btn-grid btn btn-danger">-</a></div></div></div>';
    var Html_Transfer = '<div class="row">    <div class="col-md-10"><div class="col-md-3">                ' + dropDown + '            </div>            <div class="col-md-3">                <input class="form-control number Transfer_Increase text-box single-line" data-val="true" data-val-number="Must be a number." id="transfer_increase" name="transfer_increase" type="text" value="">            </div>            <div class="col-md-3">                <input class="form-control number text-box single-line Transfer_Decrease" data-val="true" data-val-number="Must be a number." id="transfer_decrease" name="transfer_decrease" type="text" value="">            </div>            <div class="col-md-3">                <input class="form-control number text-box single-line Transfer_Add" data-val="true" data-val-number="Must be a number." id="transfer_add" name="transfer_add" type="text" value="">            </div>        </div>   <div class="col-md-2" id="remove"> <a class="btn-grid btn btn-danger">-</a></div></div>';
    //Transfer
    $("#add_transfer").click(function (e) {
        $("#container").append(Html_Transfer);
        $('[name="Acc_no"]').chosen({

        });
        CalulateTotal();
        AddEvents();
        AddCurrencyFormat(".number");
    });
    //Remove
    $("#container").on('click', '#remove', function () {
        $(this).parent('div').remove();
        CalulateTotal();
    });

    
    function AddEvents() {
        $(".Transfer_Increase").keyup(function () {
            CalulateTotal();
        });
        $(".Transfer_Decrease").keyup(function () {
            CalulateTotal();
        });
        $(".Transfer_Add").keyup(function () {
            CalulateTotal();
        });
    }

    function CalulateTotal() {
        var TransferIncrease = $(".Transfer_Increase");
        var TransferDecrease = $(".Transfer_Decrease");
        var TransferAdd = $(".Transfer_Add");
        var TITotal = 0, TDTotal=0, TATotal=0;
        for (var i = 0; i < TransferIncrease.length; i++) {
            if (TransferIncrease[i].value)
                TITotal = TITotal + parseFloat(TransferIncrease[i].value.replace(" ៛", "").split('.').join(''));
            if (TransferDecrease[i].value)
                TDTotal = TDTotal + parseFloat(TransferDecrease[i].value.replace(" ៛", "").split('.').join(''));
            if (TransferAdd[i].value)
                TATotal = TATotal + parseFloat(TransferAdd[i].value.replace(" ៛", "").split('.').join(''));
        }
        $("#TotalIncrease").val(numeral(TITotal).format('0,0').split(',').join('.') + " ៛");
        $("#TotalDecrease").val(numeral(TDTotal).format('0,0').split(',').join('.') + " ៛");
        $("#TotalAdd").val(numeral(TATotal).format('0,0').split(',').join('.') + " ៛");
    }

    function AddCurrencyFormat(className) {
        $(className).focusout(function () {
            if (this.value.indexOf('.') === -1)
                this.value = numeral(this.value).format('0,0').split(',').join('.') + " ៛";
        });
        $(className).click(function () {
            this.value = this.value.replace(" ៛", "").split('.').join('');
        });
    }

    $("#btnSubmit").click(function () {
        var data = $(".number");
        data.each(function (index, obj) {
            obj.value = obj.value.replace(" ៛", "").split('.').join('');
        });
    });

});
