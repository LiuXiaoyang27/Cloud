
// 获得请求参数
function request() {
    var param, url = location.search, theRequest = {};
    if (url.indexOf("?") != -1) {
        var str = url.substr(1);
        strs = str.split("&");
        for (var i = 0, len = strs.length; i < len; i++) {
            param = strs[i].split("=");
            theRequest[param[0]] = decodeURIComponent(param[1]);
        }
    }
    return theRequest;
};

var urlParam = request();

if (urlParam && urlParam.type) {

    var $_download = $("#download");

    // 用于区分不同的导入操作
    var type = urlParam.type;
    // 导入类型：word/excel
    var impType = urlParam.impType;

    if (impType == "" || impType == null || impType == undefined) {
        impType = "Excel";
    }

    $("#impType").text(impType);

    switch (type) {
        case "petition":
            var href;
            // 设置下载模版地址
            if (impType == "Excel") {
                href = "/data/templates/petition.xlsx";
            }
            if (impType == "Word") {
                href = "/data/templates/petition.docx";
            }
            $_download.attr("href", href);
            break;

    }

    // 点击下一步执行的方法
    $("#next").on("click", function (a) {
        window.location.href = "import-next.html?type=" + type + "&impType=" + impType;
    });
}
