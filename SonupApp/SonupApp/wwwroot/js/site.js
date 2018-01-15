// Write your JavaScript code.

//const end = {};
//end.then = function () { };

const ajax = {};

ajax.post = function (url, args) {
    const fn = $.post(url, args).then(function (rsp) {
        if (rsp.IsSuccess) {
            return rsp.Data;
        } else {
            throw "【系统信息】" + rsp.Message;
        }
    });
     
    return fn;
}