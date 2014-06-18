ko.mvc = {};

ko.mvc.executeSettings = {
    requestType: "POST",
    processData: undefined,
    success: undefined,
    error: undefined,
    beforeSend: undefined,
    complete: undefined
};

ko.mvc.init = function (viewModel, requestValidationToken) {

    $.ajaxPrefilter(function (options) {
        options.headers = $.extend({}, { "X-Request-Verification-Token": requestValidationToken }, options.headers);
    });

    viewModel.submitForm = function (form, model, url, modelOut, executeSettings, successOverride) {
        if (!$(form).valid || $(form).valid()) {
            viewModel.executeOnServer(model, url, modelOut, executeSettings, successOverride, form);
        }
    }

    viewModel.executeOnServer = function (model, url, modelOut, executeSettings, successOverride, form) {

        executeSettings = $.extend({}, ko.mvc.executeSettings, executeSettings);

        var settings = {
            url: url,
            type: (executeSettings && executeSettings.requestType) ? executeSettings.requestType : "POST",
            data: (model) ? ko.mapping.toJSON(model) : null,
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                var modelData = (executeSettings && executeSettings.processData) ? executeSettings.processData(data) : data;

                if (data.redirect) {
                    location.href = resolveUrl(data.url);
                }
                else if (successOverride) {
                    successOverride(data);
                }
                else if (modelOut) {
                    ko.mapping.fromJS(modelData, {}, modelOut);
                }
                else if (model) {
                    ko.mapping.fromJS(modelData, {}, model);
                }

                if (executeSettings && executeSettings.success) {
                    executeSettings.success(data, settings, form);
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                var msg;
                switch (textStatus) {
                    case "timeout":
                        msg = "There was a timeout posting data to the server.";
                        break;
                    case "abort":
                        msg = "Posting data to the server was aborted.";
                        break;
                    default:
                        msg = "There was an error posting data to the server.";
                }
                if (errorThrown) {
                    msg = msg + "\r\n" + errorThrown;
                }
                alert(msg);
                //alert(msg + "\r\n" + jqXHR.responseText);
                if (executeSettings && executeSettings.error) {
                    executeSettings.error(jqXHR, settings);
                }
            },
            beforeSend: function (jqXHR, settings) {
                if (executeSettings && executeSettings.beforeSend) {
                    executeSettings.beforeSend(jqXHR, settings);
                }
            },
            complete: function (jqXHR, textStatus) {
                if (executeSettings && executeSettings.complete) {
                    executeSettings.complete(jqXHR, textStatus);
                }
            }
        };

        $.ajax(settings);
    };
};

ko.mvc.resolveUrl = function (url) {
    if (url.indexOf("~/") == 0) {
        url = baseUrl + url.substring(2);
    }
    return url;
};

ko.mvc.executeSettings.processData = function (data) {
    if (data.__Data) {
        return data.__Data;
    } else {
        return data;
    }
}

ko.mvc.executeSettings.success = function (data, settings, form) {
    if (data.__Message) {
        $.bootstrapGrowl(data.__Message);
    }

    if (data.__Redirect) {
        location.href = ko.mvc.resolveUrl(data.__Redirect);
    }

    var $form = $(form);
    $form.resetSummary();
    var validator = $form.data('validator');
    if (validator) {
        validator.checkForm();
        if (data.__ModelErrors) {
            for (var i = 0; i < data.__ModelErrors.length; i++) {
                if (data.__ModelErrors[i].Key.length > 0) {
                    validator.errorList.push({
                        message: data.__ModelErrors[i].Message,
                        element: $form.find("[name='" + data.__ModelErrors[i].Key + "']")
                    });
                }
                else {
                    $form.find("[data-valmsg-summary=true]")
                        .removeClass("validation-summary-valid")
                        .addClass("validation-summary-errors")
                        .find("ul")
                        .append("<li>" + data.__ModelErrors[i].Message + "</li>");
                }
            }

            validator.defaultShowErrors();
        }
    }
}

// @torbjorn-nomell http://stackoverflow.com/a/15136297/517016
//
jQuery.fn.resetSummary = function () {
    var form = this.is('form') ? this : this.closest('form');
    form.find("[data-valmsg-summary=true]")
        .removeClass("validation-summary-errors")
        .addClass("validation-summary-valid")
        .find("ul")
        .empty();
    return this;
};

//$( function () {
//    $('form').each(function () {
//        var validator = $(this).data('validator');
//        if (validator) {
//            validator.settings.showErrors = function (map, errors) {
//                this.defaultShowErrors();
//                if ($('div[data-valmsg-summary=true] li:visible').length) {
//                    this.checkForm();
//                    if (this.errorList.length)
//                        $(this.currentForm).triggerHandler("invalid-form", [this]);
//                    else
//                        $(this.currentForm).resetSummary();
//                }
//            };
//        }
//    });
//});
// --@torbjorn-nomell

