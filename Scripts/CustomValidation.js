
//In order this function to work you must add (onsubmit = "validateAllInputBoxes(event);") into your form
stopEvent = function (ffevent) {
        var current_window = window;
        if (current_window.event) //window.event is IE, ffevent is FF
        {
            //IE
            //current_window.event.cancelBubble = true; //this stops event propagation
            //current_window.event.returnValue = false; //this prevents default (usually is what we want)
            current_window.event.stopPropagation(); //this stops event propagation
            current_window.event.preventDefault(); //this prevents default (usually is what we want)
        }
        else {
            //Firefox
            ffevent.stopPropagation();
            ffevent.preventDefault();
        };
    }

function validateAllInputBoxes(ffevent) {
    var validationState = false;
    var checkboxes= $(".requiredCheckBoxes");
    if (checkboxes.length > 0) {
        if ($('.requiredCheckBoxes:checkbox:checked').length == 0) {
            $("#CheckBoxValidation").text("សូមធ្វើការជ្រើសរើសប្រអប់");
            stopEvent(ffevent);
            validationState = true;
        } 
    }
        
        var inputs = $(".required-input");
        for (var i = 0; i < inputs.length; ++i)
            if (inputs[i].type === 'text') {
                if (inputs[i].value === '' && inputs[i].nextElementSibling != null) {
                    inputs[i].nextElementSibling.textContent = "សូមបំពេញទិន្នន័យ";
                    stopEvent(ffevent);
                    validationState = true;
                }
            }
    
        var selects = $(".required-select");
        for (var i = 0; i < selects.length; i++)
            if (selects[i].value === '' && selects[i].nextElementSibling != null) {
                selects[i].nextElementSibling.nextElementSibling.textContent = "សូមជ្រើសរើសទិន្នន័យ";
                stopEvent(ffevent);
                validationState = true;
            }


        if (validationState) {
            $(".number").each(function (index, object) {
                this.value = numeral(this.value).format('0,0').split(',').join('.') + " ៛";
            });
            validationState = false;
        }

        $(".required-input").keyup(function () {
            (this).nextElementSibling.innerHTML = '';
        });
        $(".required-select").change(function () {
            (this).nextElementSibling.nextElementSibling.innerHTML = '';
        });
    }

