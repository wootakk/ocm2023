$(document).ready(function ($) {
    $('#Mandate_date').datetimepicker({
        format: 'DD/MM/YYYY'
    });
    $('#MEF_date').datetimepicker({
        format: 'DD/MM/YYYY'
    });
    $('#Advance_date').datetimepicker({
        format: 'DD/MM/YYYY'
    });

    allowCheckedOnlyOne();

    function allowCheckedOnlyOne() {
        $(".requiredCheckBoxes").click(function () {
            var THIS = this;
            var checkBoxes = $(".requiredCheckBoxes");
            for (var i = 0; i < checkBoxes.length; i++) {
                if (checkBoxes[i] != THIS) {
                    checkBoxes[i].checked = false;
                }
            }
        });
    }
    //jQuery.validator.methods["date"] = function (value, element) { return "Error"; }

    $("#Unit_id").chosen({});
    $('[name="Acc_no"]').chosen({});

    $("#USD").click(function () {
        
        var originalValue = this.value;
        var splitOriginalValue = originalValue.split(',');
        var integerValue = splitOriginalValue[0].split('.').join('');
        var decimalValue = splitOriginalValue[1] ? "." + splitOriginalValue[1] : " ";
        this.value = integerValue.concat(decimalValue);
        //this.value = this.value.replace(" USD", "").split('.').join('');
        
        //this.value = this.value.replace(" USD", "").split('.').join('');
    });

    $("#USD").focusout(function () {
        if (this.value) {
            
            //this.value = numeral(this.value).format('0,0').split(',').join('.') + " USD";
            var integerValue = 0;
            var decimalValue;
            var originalValue = this.value;
            var splitDecimal = originalValue.split('.');
            decimalValue = splitDecimal[1] ?","+ splitDecimal[1] : "";
            integerValue=  numeral(splitDecimal[0]).format('0,0').split(',').join('.');
            this.value = integerValue.concat(decimalValue);
            console.log(numeral(this.value));
            
            //this.value = numeral(this.value).format('0,0').split(',').join('.') + " USD";
        }
    });

});