
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
    // 请求接口地址
    var requestUrl = "";
    var extensions = "";
    var action = "import";
    // 请求类型
    var type = urlParam.type;
    // 导入类型：word/excel
    var impType = urlParam.impType;
    if (impType == "" || impType == null || impType == undefined) {
        impType = "Excel";
    }
    $("#impType").text(impType);
    if (impType == "Excel") {
        extensions = "xlsx";
    }
    if (impType == "Word") {
        extensions = "docx";
        action = "impWord";
    }

    switch (type) {
        case "petition":
            requestUrl = "/ajax/petition.ashx?action=" + action;
            break;
    }
    // 点击上一步执行的方法
    $("#btn_pre").on("click", function (event) {
        window.location.href = "/pages/import/import-start.html?type=" + type + "&impType=" + impType;
    });

    //初始化Web Uploader
    var uploader = WebUploader.create({
        auto: true,                                         //自动上传
        swf: '/assets/webuploader/Uploader.swf',     // swf文件路径
        server: requestUrl,                    // 文件接收服务端。
        pick: {
            id: '#btn_upload',
            multiple: false
        },
        accept: {
            title: 'Files',
            extensions: extensions
        },
        fileNumLimit: 1,                              //最大上传数量为10
        fileSingleSizeLimit: 10 * 1024 * 1024,         //限制上传单个文件大小20M
        fileSizeLimit: 10 * 1024 * 1024,              //限制上传所有文件大小200M
        resize: false                                  // 不压缩image, 默认如果是jpeg，文件上传前会压缩一把再上传！
    });

    uploader.on('error', function (handler) {
        var text = "";
        if (handler == "Q_EXCEED_NUM_LIMIT") {
            text = "超出最大文件数";
        }
        if (handler == "Q_EXCEED_SIZE_LIMIT") {
            text = "上传文件最大为10M";
        }
        if (handler == "Q_TYPE_DENIED") {
            text = "上传文件类型不匹配";
        }
        layer.msg(text);
    });

    // 文件上传过程中创建进度条实时显示。
    uploader.on('uploadProgress', function (file, percentage) {
        layer.load();
    });

    // 当文件上传出错时触发。
    uploader.on('uploadError', function (file, reason) {
        layer.msg(reason);
    });

    // 不管成功或者失败，文件上传完成时触发。
    uploader.on('uploadComplete', function (file) {
        layer.closeAll('loading');
        uploader.reset();
    });
    // 文件上传过程中创建进度条实时显示。
    uploader.on('uploadSuccess', function (file, response) {
        var _raw = response._raw;
        console.log("response", response);
        //var status = _raw.status;
        //var msg = _raw.msg;
        var status = response.status;
        var msg = response.msg;
        var errMsg = response.errMsg;
        window.location.href = "import-msg.html?type=" + type + "&impType=" + impType + "&msg=" + msg + "&status=" + status + "&errMsg=" + errMsg;
    });

}