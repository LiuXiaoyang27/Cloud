/***************信访案件form.html js************************/
/* 修改人:liuxiaoyang
 * 修改日期:20220428 */
(function ($) {
    var id = jinkai.request().id;
    //请求地址  
    var requestUrl = "/ajax/petition.ashx";

    var editor;
    $.page = {
        vue: null,
        init: function () {
            $.page.vue = jinkai.vueInit({ el: "#appForm", data: { petition: {} } })
            $.page.info();
            $.page.files();
            $.page.bind();
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
                        $.page.setData(result.data);
                        //$.page.vue.petition = result.data;
                    },
                    error: function (result) {
                        jinkai.msg(result.msg, "error");
                    },
                })
            }
        },
        bind: function () {
            $(".queueList").on("click", "img", function () {
                var data = $(this).attr("data");
                window.open(data);
            });
        },
        setData: function (data) {
            $.page.vue.petition = data;
            $.page.vue.petition.createDate = jinkai.toDate(data.createDate, "yyyy-MM-dd");
            $.page.vue.petition.caseType = jinkai.getData().dictionaryData("caseType", $.page.vue.petition.caseType).text;
            $.page.vue.petition.caseCategory = jinkai.getData().dictionaryData("caseCategory", $.page.vue.petition.caseCategory).text;
            $.page.vue.petition.caseSource = jinkai.getData().dictionaryData("caseSource", $.page.vue.petition.caseSource).text;
            $.page.vue.petition.channels = jinkai.getData().dictionaryData("channels", $.page.vue.petition.channels).text;
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
                        var files = result.data.listDocs;
                        $.page.uploaderSet(files);
                    },
                    error: function (result) {
                        jinkai.msg(result.msg, "error");
                    },
                })
            }
        },
        uploaderSet: function (files) {
            var $wrap = $('#uploader'),

           // 图片容器
           $queue = $('<ul class="filelist"></ul>')
               .appendTo($wrap.find('.queueList'));

            for (var i = 0; i < files.length; i++) {
                var data = files[i];
                var href = data.url;
                var src;
                if (data.type == "gif" || data.type == "jpg" || data.type == "jpeg" || data.type == "png" || data.type == "bmp") {
                    src = href;
                } else {
                    var src = "/images/pdf.png";
                    if (data.type == "docx" || data.type == "doc") {
                        src = "/images/icon_word.png";
                    }
                    if (data.type == "xlsx" || data.type == "xls") {
                        src = "/images/icon_excel.png";
                    }
                    if (data.type == "mp4" || data.type == "avi" || data.type == "rmvb" || data.type == "ts") {
                        src = "/images/video.png";
                    }
                }

                var $li = $('<li id="' + files[i].id + '"  data-pid="' + files[i].id + '">' +
                            '<p class="title">' + files[i].name + '</p>' +
                            '<p class="imgWrap"><img src="' + src + '" data="' + href + '"/></p>' +
                            '</li>');
                $li.appendTo($queue);
            }
        }
    }
    $(function () {
        $.page.init();
    });
})(jQuery);