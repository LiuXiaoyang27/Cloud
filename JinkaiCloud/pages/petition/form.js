/***************信访案件form.html js************************/
/* 修改人:liuxiaoyang
 * 修改日期:20220428 */
(function ($) {
    var id = jinkai.request().id;
    //请求地址
    var requestUrl = "/ajax/petition.ashx";

    $.page = {
        vue: null,
        fileList: {}, // 保存上传文件信息 
        listDocs: [],
        //pdfTab: "",//上传文件页面
        init: function () {
            $.page.vue = jinkai.vueInit({ el: "#appForm", data: { petition: {} } })
            $.page.bind();
            $.page.info();
        },
        bind: function () {
            //日期
            $("#createDate").datePicker();
            $("[name='caseType']").comboBox({
                data: jinkai.getData().dictionaryData("caseType"),
                height: 300
            });
            $("[name='caseCategory']").comboBox({
                data: jinkai.getData().dictionaryData("caseCategory"),
                height: 300
            });
            $("[name='caseSource']").comboBox({
                data: jinkai.getData().dictionaryData("caseSource"),
                height: 300
            });
            $("[name='channels']").comboBox({
                data: jinkai.getData().dictionaryData("channels"),
                height: 300
            });

            //切换到上传tab页时，再调用上传插件（ps：页面加载时调用插件无效）
            //$(".nav-tabs a").on("shown.bs.tab", function (e) {
            //    var activeTab = $(e.target).text();
            //    // var previousTab = $(e.relatedTarget).text();前一个tab
            //    if (activeTab == "文件资料") {
            //        if ($.page.pdfTab == "") {
            //            $.page.pdfTab = activeTab;
            //            $.thisPage.uploaderPdf = $("[name='fileJson']").uplodeFile();
            //        }
            //    }

            //});
        },
        //获取文件
        files: function () {

            if (id) {
                jinkai.ajax({
                    async: false, // 同步
                    type: "GET",
                    url: jinkai.toUrl(requestUrl + "?action=getUpload&id=" + id),
                    param: {},
                    success: function (result) {
                        $.page.listDocs = result.data.listDocs;
                        var files = result.data.listDocs;
                        $.each(files, function (index, data) {
                            $.page.fileList[data.id] = data;
                        });
                    },
                    error: function (result) {
                        jinkai.msg(result.msg, "error");
                    },
                })
            }

        },
        //获得数据信息
        info: function () {
            if (id) {
                jinkai.ajax({
                    async: false,
                    type: "GET",
                    url: jinkai.toUrl(requestUrl + "?action=query&id=" + id),
                    param: {},
                    success: function (result) {
                        $.page.vue.petition = result.data;
                    },
                    error: function (result) {
                        jinkai.msg(result.msg, "error");
                    },
                })
            } else {
                var date = new Date();
                jinkai.vueSet($.page.vue.petition, "createDate", date.toLocaleDateString().replace(/\//g,"-"));
            }
        },
        // 获得上传的文件地址信息
        //getFiles: function () {
        //    var filePdf = $.thisPage.getFiles();
        //    var fileList = [];
        //    for (var e in filePdf) {
        //        fileList.push(filePdf[e]);
        //    }
        //    return JSON.stringify(fileList);
        //},
        //保存
        save: function () {
            if (!$("#appForm").formValid()) {
                return false;
            }

            var data = $.page.vue.petition;
            var files = [];
            for (var e in $.page.fileList) {
                files.push($.page.fileList[e]);
            }
            data.filePdfs = JSON.stringify(files);
            //data.filePdfs = $.page.getFiles;

            var url = id == null ? requestUrl + "?action=add" : requestUrl + "?action=update";
            var title;
            if (url.substring(url.length - 6) == "update") {
                title = "修改成功"
            } else {
                title = "新增成功"
            }
            jinkai.ajax({
                async: false,
                type: "Post",
                url: url,
                param: data,
                success: function (result) {
                    window.parent.jinkai.msg(title, "success");
                    window.parent.$("#gridTable").gridReloadSelection();
                    window.parent.jinkai.openSlideClose();
                },
                error: function (result) {
                    window.parent.jinkai.ajaxError(result);
                }
                //beforeSend: function () {
                //    jinkai.loading(true);
                //}
            })
        }
    }
    $(function () {
        $.page.init();
    });
})(jQuery);