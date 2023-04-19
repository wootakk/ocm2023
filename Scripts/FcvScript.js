
$(document).ready(function ($) {
    /* Inint Date */
    $('#FCVDate').datetimepicker({
        format: 'DD/MM/YYYY'
    });
    $('#MEF_date').datetimepicker({
        format: 'DD/MM/YYYY'
    });
    $('#Letter_date').datetimepicker({
        format: 'DD/MM/YYYY'
    });
    $('#Acc_no').chosen({});
    $('#Department_of_commitment').chosen({});

    AddCurrencyFormat(".number");
    
    $("#FCV_amount").focusout(function () {
        $("#AmountInLetter").val(toWords(this.value));
    });

    allowCheckedOnlyOne();

    function allowCheckedOnlyOne() {
        $(".requiredCheckBoxes").click(function() {
            var THIS = this;
            var checkBoxes = $(".requiredCheckBoxes");
            for (var i = 0; i < checkBoxes.length; i++) {
                if (checkBoxes[i] != THIS) {
                    checkBoxes[i].checked = false;
                }
            }
        });
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

    var th = ['', 'ពាន់', 'លាន', 'ពពាន់លាន', 'ពាន់ពាន់លាន'];
    var dg = ['សូន្យ', 'មួយ', 'ពីរ', 'បី', 'បួន', 'ប្រាំ', 'ប្រាំមួយ', 'ប្រាំពីរ', 'ប្រាំបី', 'ប្រាំបួន'];
    var tn = ['ដប់', 'ដប់មួយ', 'ដប់ពីរ', 'ដប់បី', 'ដប់ប្រាំ', 'ដប់ប្រាំមួយ', 'ដប់ប្រាំពីរ', 'ដប់ប្រាំបី', 'ដប់ប្រាំបី', 'ដប់ប្រាំបួន'];
    var tw = ['ម្ភៃ', 'សាមសិប', 'សែសិប', 'ហាសិប', 'ហុកសិប', 'ចិតសិប', 'ប៉ែតសិប', 'កៅសិប'];

    function toWords(s) {
        s = s.toString();
        s = s.replace(/[\, ]/g, '');
        if (s != parseFloat(s)) return 'សុំទោសនោះមិនមែនជាលេខ';
        var x = s.indexOf('.');
        if (x == -1) x = s.length;
        if (x > 15) return 'ធំ​ណាស់ / Too big';
        var n = s.split('');
        //   var str = '\uF06E​​    ';
        var str = '';
        var sk = 0;
        for (var i = 0; i < x; i++) {
            if ((x - i) % 3 == 2) {
                if (n[i] == '1') {
                    str += tn[Number(n[i + 1])] + '​';
                    i++;
                    sk = 1;
                } else if (n[i] != 0) {
                    str += tw[n[i] - 2] + '​';
                    sk = 1;
                }
            } else if (n[i] != 0) {
                str += dg[n[i]] + ' ';
                if ((x - i) % 3 == 0) str += 'រយ';
                sk = 1;
            }
            if ((x - i) % 3 == 1) {
                if (sk) str += th[(x - i - 1) / 3] + '​';
                sk = 0;
            }
        }
        if (x != s.length) {
            var y = s.length;
            str += 'ចុច ';
            for (var i = x + 1; i < y; i++) str += dg[n[i]] + '​';
        }
        if (str == '')
            return str.replace(/\s+/g, '​');
        else
            return str.replace(/\s+/g, '​') + "រៀលគត់";
    }
    
});