(function ($) {
    $.page = {
        requestUrl: "/ajax/oldPetition.ashx",
        module: "oldPetition",
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
        toolbar: function () {
            //编辑 
            $("#gridTable").on("click", ".operating .operate-edit", function (event) {
                var right = jinkai.verifyRight($.page.module + "_Edit");
                if (right) {
                    var id = $(this).parent().data("id");
                    jinkai.openSlide({
                        title: "编辑信息",
                        url: "form.html?id=" + id,
                        width: 640,
                        callBack: function (name) {
                            window.frames[name].$.page.save();
                        }
                    })
                }
            });
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
        formatterOperate: function (val, opt, row) {

            
            var html_edit = ''
            var right = jinkai.verifyRight($.page.module + "_Edit", false);
            if (right) {
                html_edit += '<div class="operating" data-id="' + row.id + '">' +
                               '<span class="fa fa-pencil-square-o edit-color operate-edit" style="cursor:pointer" title="编辑"></span>';
            }
            html_edit += '</div>';
            return html_edit;
        },
        grid: function () {
            $("#gridTable").grid({
                url: $.page.requestUrl + "?action=list",
                height: $(window).height() - 142,
                styleUI: 'Bootstrap',
                datatype: "json",
                pagination: true,
                multiselect: true,
                colModel: [
                    { label: "标记", name: "id", hidden: true, key: true },
                    {
                        label: "操作", fixed: !0, name: "operate", width: 80, align: "center",
                        formatter: $.page.formatterOperate
                    },
                    {
                        label: "文件名称", name: "FileName", width: 200, align: "left", autowidth: true, sortable: false,
                        formatter: function (value, options, rowItem) {
                            return "<a class='cursor-pointer file-open' data-id='" + rowItem.FilePath + "'><i class='fa " + $.page.toFileExt(rowItem.FileType) + " i-default m-r-xs' />" + value + "</a>";
                        }
                    },
                    { label: "文件类型", name: "FileType", width: 130, align: "left", sortable: false, hidden: true },
                    { label: "案件名称", name: "title", width: 200, align: "left", sortable: false },
                    {
                        label: "案件日期", name: "pDate", width: 130, align: "left", sortable: false,
                        formatter: function (value) {
                            return jinkai.toDate(value, "yyyy-MM-dd HH:mm");
                        }
                    },
                    { label: "当事人", name: "pName", width: 160, align: "left", sortable: false },
                    { label: "办理人", name: "attendName", width: 160, align: "left", sortable: false },
                    {
                        label: "文件大小", name: "FileSize", width: 130, align: "left", sortable: false,
                        formatter: function (value, options, rowItem) {
                            var kb = Math.round(value / 1024, 2);
                            return kb + "KB";
                        }
                    },
                    {
                        label: "上传时间", name: "modifyTime", width: 130, align: "left", sortable: false,
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
                $("#hidWord").attr("href", filePath);
                document.getElementById("hidWord").click();
            });
        },
        upload: function () {
            //初始化Web Uploader
            var uploader = WebUploader.create({
                auto: true,                                         //自动上传
                swf: '/assets/webuploader/Uploader.swf',     // swf文件路径
                server: $.page.requestUrl + "?action=upload",                    // 文件接收服务端。
                pick: {
                    id: '#btn_upload',
                    multiple: true
                },
                accept: {
                    title: 'Files',
                    extensions: "doc,docx"
                },
                threads: 20,
                fileNumLimit: 20,                              //最大上传数量为20
                fileSingleSizeLimit: 20 * 1024 * 1024,         //限制上传单个文件大小20M
                fileSizeLimit: 400 * 1024 * 1024,              //限制上传所有文件大小400M
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
                    //$("#gridTable").gridReloadSelection();

                } else {
                    jinkai.msg(msg, "error");
                }
            });
            uploader.on('uploadFinished', function () {
                setTimeout(function () {
                    layer.closeAll('loading');
                    jinkai.reload()
                }, 3000)


            })
        },
        search: function () {
            //起始日期 
            $("#txt_startTime").datePicker({ maxDate: '#F{$dp.$D(\'txt_endTime\') || \'%y-%M-%d\'}' });
            //结束日期
            $("#txt_endTime").datePicker({ minDate: "#F{$dp.$D(\'txt_startTime\')}", maxDate: '%y-%M-%d' });

            $("#btn_search").click(function () {
                var queryJson = {
                    beginDate: $("#txt_startTime").val(),
                    endDate: $("#txt_endTime").val(),
                    matchCon: $("#txt_keyword").val()
                }
                $("#gridTable").jqGrid("setGridParam", { postData: queryJson }).gridReload();
                //$("#gridTable").jqGrid("setGridParam", { postData: { matchCon: $("#txt_keyword").val() } }).gridReload();
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