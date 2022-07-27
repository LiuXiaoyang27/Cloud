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
    // 请求类别
    var type = urlParam.type;
    // 导入类型：word/excel
    var impType = urlParam.impType;
    if (impType == "" || impType == null || impType == undefined) {
        impType = "Excel";
    }
    $("#impType").text(impType);
    // 请求状态
    var status = urlParam.status;
    // 状态信息
    var msg = urlParam.msg;

    // 错误消息
    var errMsg = urlParam.errMsg;
    if (status == 200) {
        var _html = '<p style="color:#98CA00">' + msg + '</p> ' +
                '<p style="color:#ffba00">' + errMsg + '</p>'
        $("#msg").html(_html);
        
    } else {
        var _html = '<p style="color:#EF644C">' + msg + '</p> ' +
                '<p style="color:#ffba00">' + errMsg + '</p>'
        $("#msg").html(_html);
    }

    // 点击上一步事件
    $("#btn_pre").on("click", function (a) {
        window.location.href = "/pages/import/import-next.html?type=" + type + "&impType=" + impType;
    });

    // 点击退出事件
    $("#btn_close").on("click", function (a) {
        //关闭弹窗 20210104 todo
        var index = parent.layer.getFrameIndex(window.name); //获取窗口索引
        //jinkai.openClose();
        var a = parent.$("#gridTable");
        parent.layer.close(index);
        
    });
}
