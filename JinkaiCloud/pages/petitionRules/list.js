(function ($) {
    $.page = {
        requestUrl: "/ajax/petitionRules.ashx",
        module: "petitionRules",
        init: function () {
            // 移除cbox点击事件，防止多次点击，无法选中问题。
            $("#gridTable").on("click", ".cbox", function (event) {
                return false;
            });
            $.page.toolbar();
            $.page.grid();
            $.page.upload();
            $.page.search();
            $.page.resize();
        },
        toolbar:function(){
            //删除 
            $("#btn_delete").on("click", function () {
                var right = jinkai.verifyRight($.page.module + "_Delete");
                if (right) {
                    var id = $("#gridTable").jqGrid("getGridParam", "selarrrow");
                    if (id && 0 != id.length) {
                        jinkai.confirm({
                            content: "删除的数据将不能恢复，请确认是否删除？",
                            callBack: function () {
                                jinkai.ajax({
                                    type: "POST",
                                    url: jinkai.toUrl($.page.requestUrl + "?action=delete&id=" + id.join()),
                                    param: {},
                                    success: function (result) {
                                        jinkai.msg("删除成功", "success");
                                        $("#gridTable").gridReload();
                                    },
                                    error: function (result) {
                                        jinkai.msg(result.msg, "error");
                                    },
                                })
                            }
                        });
                    } else {
                        jinkai.msg("请先选择要删除的数据");
                    }
                }
            });
        },
        grid: function () {
            $("#gridTable").grid({
                url: $.page.requestUrl+"?action=list",
                height: $(window).height() - 142,
                styleUI: 'Bootstrap',
                datatype: "json",
                pagination: true,
                multiselect: true,
                colModel: [
                    { label: "标记", name: "id", hidden: true, key: true },
                    {
                        label: "文件名称", name: "FileName", width: 200, align: "left", autowidth: true, sortable: false,
                        formatter: function (value, options, rowItem) {
                            return "<a class='cursor-pointer file-open' data-id='" + rowItem.FilePath + "'><i class='fa " + $.page.toFileExt(rowItem.FileType) + " i-default m-r-xs' />" + value + "</a>";
                        }
                    },
                    { label: "文件类型", name: "FileType", width: 130, align: "left", sortable: false },
                    {
                        label: "文件大小", name: "FileSize", width: 130, align: "left", sortable: false,
                        formatter: function (value, options, rowItem) {
                            var kb = Math.round(value / 1024, 2);
                            return kb + "KB";
                        }
                    },
                    {
                        label: "修改时间", name: "modifyTime", width: 130, align: "left", sortable: false,
                        formatter: function (value) {
                            return jinkai.toDate(value, "yyyy-MM-dd HH:mm");
                        }
                    }
                ]
            });
            //点击预览
            $("#gridTable").delegate("a.file-open", "click", function (e) {
                var filePath = $(this).attr("data-id");
                var fileName = $(this).text();
                jinkai.openSlide({
                    title: "文件预览 - " + fileName,
                    url: "Preview.html?filePath=" + filePath
                })
            });
        },
        upload: function () {
            //初始化Web Uploader
            var uploader = WebUploader.create({
                auto: true,                                         //自动上传
                swf: '/assets/webuploader/Uploader.swf',     // swf文件路径
                server: $.page.requestUrl+"?action=upload",                    // 文件接收服务端。
                pick: {
                    id: '#btn_upload',
                    multiple: false
                },
                accept: {
                    title: 'Files',
                    extensions: "pdf,doc,docx,xlsx,xls,ppt,pptx"
                },
                fileNumLimit: 1,                              //最大上传数量为10
                fileSingleSizeLimit: 50 * 1024 * 1024,         //限制上传单个文件大小20M
                fileSizeLimit: 50 * 1024 * 1024,              //限制上传所有文件大小200M
                resize: false,                              // 不压缩image, 默认如果是jpeg，文件上传前会压缩一把再上传！
                duplicate: false
            });
            uploader.on('error', function (handler, b) {
                var text = "";
                if (handler == "Q_EXCEED_NUM_LIMIT") {
                    text = "超出最大文件数";
                }
                if (handler == "Q_EXCEED_SIZE_LIMIT") {
                    text = "上传文件最大为50M";
                }
                if (handler == "Q_TYPE_DENIED") {
                    text = "上传文件类型不匹配";
                }
                jinkai.msg(text);
            });

            // 文件上传过程中创建进度条实时显示。
            uploader.on('uploadProgress', function (file, percentage) {
                layer.load();
            });

            // 当文件上传出错时触发。
            uploader.on('uploadError', function (file, reason) {
                debugger;
                layer.msg(reason);
            });

            // 不管成功或者失败，文件上传完成时触发。
            uploader.on('uploadComplete', function (file) {
                layer.closeAll('loading');
                uploader.reset();
            });
            // 文件上传过程中创建进度条实时显示。
            uploader.on('uploadSuccess', function (file, response) {
                var status = response.status;
                var msg = response.msg;
                if (status != -1) {
                    $("#gridTable").gridReloadSelection();
                } else {
                    jinkai.msg(msg,"error");
                }    
            });
        },
        search: function () {
            $("#btn_search").click(function () {
                $("#gridTable").jqGrid("setGridParam", { postData: { matchCon: $("#txt_keyword").val() } }).gridReload();
            });
            $("#txt_keyword").keydown(function (e) {
                if (e.keyCode == 13) {
                    $("#btn_search").trigger("click");
                }
            });
        },
        toFileExt: function (ext) {
            if (ext == "doc" || ext == "docx") {
                return "fa-file-word-o";
            }
            if (ext == "xls" || ext == "xlsx") {
                return "fa-file-excel-o";
            }
            if (ext == "ppt" || ext == "pptx") {
                return "fa-file-powerpoint-o";
            }
            if (ext == "pdf") {
                return "fa-file-pdf-o "
            }
            return "fa-file-o";
        },
        resize: function () {
            $(window).resize(function () {
                $("#gridTable").setGridHeight($(window).height() - 142).setGridWidth($(".main-bd").width());
            });
        }
    }
    $(function () {
        $.page.init();
    });
})(jQuery);