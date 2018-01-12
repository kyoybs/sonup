// Write your JavaScript code.

//const end = {};
//end.then = function () { };

const ajax = {};

ajax.post = function (url, args) {
    const fn = $.post(url, args).then(function (rsp) {
        if (rsp.IsSuccess) {
            return rsp.Data;
        } else {
            alert("Error:" + rsp.Message);
            return Promis.reject("");
        }
    }).catch(function (err) { alert(error); });

    return fn;
}