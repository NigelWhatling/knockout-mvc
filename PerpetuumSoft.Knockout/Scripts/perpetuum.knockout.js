executeOnServer = function (model, url, token, modelOut, executeSettings, successOverride) {

    var $this = $(this);

    var settings = {
        url: url,
        type: (executeSettings && executeSettings.requestType) ? settings.requestType : 'POST',
        data: (model) ? ko.mapping.toJSON(model) : null,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            if (data.redirect) {
                location.href = resolveUrl(data.url);
            }
            else if (successOverride) {
                successOverride(data);
            }
            else if (modelOut) {
                ko.mapping.fromJS(data, modelOut);
            }
            else if (model) {
                ko.mapping.fromJS(data, model);
            }
            if (executeSettings && executeSettings.success) {
                executeSettings.success(jqXHR, settings);
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
            alert(msg + "\r\n" + jqXHR.responseText);
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

    if (token) {
        settings.headers = { "X-Request-Verification-Token": token };
    }

    $.ajax(settings);
};

resolveUrl = function (url) {
    if (url.indexOf("~/") == 0) {
        url = baseUrl + url.substring(2);
    }
    return url;
};
