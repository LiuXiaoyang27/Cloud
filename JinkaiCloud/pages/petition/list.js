/***************信访案件list.html js************************/
/* 修改人:liuxiaoyang 
 * 修改日期:20220428 */
(function ($) {
    // 请求接口地址
    var requestUrl = "/ajax/petition.ashx";
    $.page = {
        flag: 1,
        module: "petitionQuery",
        init: function () {
            $.page.bind();
            $.page.toolbar();
            $.page.grid();
            $.page.search();
            $.page.resize();
        },
        bind: function () {
            $("#caseSource").comboBox({
                data: jinkai.getData().dictionaryData("caseSource")
            });
            $("#caseType").comboBox({
                data: jinkai.getData().dictionaryData("caseType")
            });
            $("#caseCategory").comboBox({
                data: jinkai.getData().dictionaryData("caseCategory")
            });
            $("#channels").comboBox({
                data: jinkai.getData().dictionaryData("channels")
            });

            //点击页面 隐藏导入下拉框
            document.onclick = function (e) {
                var e = e || window.event;
                var elem = e.target || e.srcElement;
                while (elem) {
                    if (elem && elem.id == "btn_import") {
                        return;
                    }
                    elem = elem.parentNode;
                }
                $(".imp").hide();
                $.page.flag = 1;
            }
        },
        toolbar: function () {
            // 移除cbox点击事件，防止多次点击，无法选中问题。
            $("#gridTable").on("click", ".cbox", function (event) {
                return false;
            });
            //新建
            $("#btn_add").on("click", function () {
                var right = jinkai.verifyRight($.page.module + "_Add");
                if (right) {
                    jinkai.openSlide({
                        title: "新建信访信息",
                        url: "form.html",
                        width: 680,
                        callBack: function (name) {
                            window.frames[name].$.page.save();
                        }
                    })
                }
            });
            //详情
            $("#gridTable").on("click", ".operating .operate-detail", function (event) {
                var right = jinkai.verifyRight($.page.module + "_Show");
                if (right) {
                    //详细 
                    jinkai.openWindow({
                        title: "案件详情",
                        url: "petition/detail.html?id=" + $(this).parent().data("id"),
                        width: "780px",
                        height: "580px"
                    })
                }
            });
            //编辑 
            $("#gridTable").on("click", ".operating .operate-edit", function (event) {
                var right = jinkai.verifyRight($.page.module + "_Edit");
                if (right) {
                    var id = $(this).parent().data("id");
                    jinkai.openSlide({
                        title: "修改信访信息",
                        url: "form.html?id=" + id,
                        width: 680,
                        callBack: function (name) {
                            window.frames[name].$.page.save();
                        }
                    })
                }
            });
            //列表删除 
            $("#gridTable").on("click", ".operating .operate-delete", function (event) {
                var right = jinkai.verifyRight($.page.module + "_Delete");
                if (right) {
                    var id = $(this).parent().data("id");
                    jinkai.confirm({
                        content: "删除的信息将不能恢复，请确认是否删除？",
                        callBack: function () {
                            jinkai.ajax({
                                async: false,
                                type: "POST",
                                url: jinkai.toUrl(requestUrl + "?action=delete&id=" + id),
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
                }
            });
            //删除 
            $("#btn_delete").on("click", function () {
                var right = jinkai.verifyRight($.page.module + "_Delete");
                if (right) {
                    var id = $("#gridTable").jqGrid("getGridParam", "selarrrow");
                    if (id && 0 != id.length) {
                        jinkai.confirm({
                            content: "删除的信息将不能恢复，请确认是否删除？",
                            callBack: function () {
                                jinkai.ajax({
                                    type: "POST",
                                    url: jinkai.toUrl(requestUrl + "?action=batchDelete&id=" + id.join()),
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
                        jinkai.msg("请先选择要删除的信访信息");
                    }
                }
            });
            // 打印
            $("#btn_print").click(function (event) {
                var right = jinkai.verifyRight($.page.module + "_Print");
                if (right) {
                    var selarrrow = $("#gridTable").jqGrid("getGridParam", "selarrrow");

                    var queryJson = $.page.getQueryJson();
                    queryJson.action = "pdf";
                    queryJson.id = selarrrow.join();
                    queryJson.module = $.page.module;

                    jinkai.ajax({
                        async: false,
                        type: "GET",
                        url: jinkai.toUrl(requestUrl),
                        param: queryJson,
                        success: function (result) {
                            var href = result.msg;
                            $("#hidPdf").attr("href", href);
                            document.getElementById("hidPdf").click();
                        },
                        error: function (result) {
                            jinkai.msg(result.msg, "error");
                        },
                    })
                }
            });
            //导出PDF
            $("#btn_export_pdf").click(function (event) {
                var right = jinkai.verifyRight($.page.module + "_Export");
                if (right) {
                    var id = $("#gridTable").jqGrid("getGridParam", "selarrrow");
                    jinkai.ajax({
                        async: false,
                        type: "GET",
                        url: jinkai.toUrl(requestUrl + "?action=pdf&id=" + id.join()),
                        param: {},
                        success: function (result) {
                            var href = result.msg;
                            $("#hidPdf").attr("href", href);
                            document.getElementById("hidPdf").click();
                        },
                        error: function (result) {
                            jinkai.msg(result.msg, "error");
                        },
                    })
                }
            });
            //导出Excel
            $("#btn_export_excel").click(function (event) {
                var right = jinkai.verifyRight($.page.module + "_Export");
                if (right) {
                    var selarrrow = $("#gridTable").jqGrid("getGridParam", "selarrrow");

                    var queryJson = $.page.getQueryJson();
                    queryJson.action = "export";
                    queryJson.id = selarrrow.join();
                    queryJson.module = $.page.module;

                    jinkai.ajax({
                        async: false, // 同步 
                        type: "GET",
                        url: jinkai.toUrl(requestUrl),
                        param: queryJson,
                        success: function (result) {
                            var href = result.msg;
                            jinkai.msg("导出excel成功", "success");
                            $("#hidExcel").attr("href", href);

                            document.getElementById("hidExcel").click();
                        },
                        error: function (result) {
                            jinkai.msg(result.msg, "error");
                        },
                    })
                }
            });
            // 点击导入执行的方法
            $("#btn_import").click(function () {
                var right = jinkai.verifyRight($.page.module + "_Import");
                if (right) {
                    if ($.page.flag) {
                        $(".imp").show();
                        var impUrl = "/pages/import/import-start.html?type=petition";
                        $("#btn_import_excel").click(function (event) {
                            jinkai.openImport(impUrl + "&impType=Excel");
                        })
                        $("#btn_import_word").click(function (event) {
                            jinkai.openImport(impUrl + "&impType=Word");
                        })
                        $.page.flag = 0;
                    } else {
                        $(".imp").hide();
                        $.page.flag = 1;
                    }
                }
            });
            // 点击状态 
            $("#gridTable").on("click", ".set-status", function () {
                var right = jinkai.verifyRight($.page.module + "_Enabled");
                if (right) {
                    var id = $(this).data("id");
                    var status = $(this).data("status");
                    if (status == 0) {
                        status = !status;
                        $.page.setStatus(id, status);
                    }

                }
            });
        },
        //更改状态
        setStatus: function (id, status) {
            id && jinkai.ajax({
                async: true,
                type: "POST",
                url: jinkai.toUrl(requestUrl),
                param: {
                    id: id,
                    status: Number(status),
                    action: "release",
                },
                success: function (result) {
                    jinkai.msg("状态已修改！", "success");
                    $("#gridTable").jqGrid("setCell", id, "status", status);
                }, error: function (result) {
                    jinkai.ajaxError(result);
                }
            });
        },
        // 获得请求参数
        getQueryJson: function () {
            var queryJson = {
                matchCon: $("#matchCon").val(),
                beginDate: $("#txt_startTime").val(),
                endDate: $("#txt_endTime").val(),
                caseSource: $("#caseSource").val(),
                caseType: $("#caseType").val(),
                caseCategory: $("#caseCategory").val(),
                channels: $("#channels").val(),
                __timespan: new Date().getTime()
            };
            return queryJson;
        },
        formatterOperate: function (val, opt, row) {

            var html_edit = '<div class="operating" data-id="' + row.id + '">' +
                              '<span class="fa fa-search search-color operate-detail" style=" cursor:pointer" title="详情"></span>';

            var right = jinkai.verifyRight($.page.module + "_Edit", false);
            if (right) {
                html_edit += '<span class="fa fa-pencil-square-o edit-color operate-edit" style="cursor:pointer" title="修改"></span>';
            }
            var right = jinkai.verifyRight($.page.module + "_Delete", false);
            if (right) {
                html_edit += '<span class="fa fa-trash-o delete-color operate-delete" style="cursor:pointer" title="删除"></span>';
            }
            html_edit += '</div>';
            return html_edit;
        },
        grid: function () { 
            $("#gridTable").grid({
                authorize: true,
                url: "/ajax/petition.ashx?action=list",
                styleUI: 'Bootstrap',
                datatype: "json",
                height: $(window).height() - 142,
                pagination: true,
                multiselect: true,
                colModel: [
                    {
                        label: "操作", fixed: !0, name: "operate", width: 130, align: "center",
                        formatter: $.page.formatterOperate
                    },
                    {
                        label: "日期", name: "createDate", width: 100, align: "center", formatter(val) {
                            if (val == "") {
                                return "";
                            }
                            return jinkai.toDate(val, "yyyy-MM-dd");
                        }
                    },
                    { label: "当事人姓名", name: "pName", width: 130, align: "center" },
                    { label: "身份证号", name: "pIdCard", width: 180, align: "left" },
                    { label: "家庭住址", name: "pAddress", width: 100, align: "left", autowidth: true },
                    {
                        label: "案件类型", name: "caseType", width: 120, align: "center",
                        formatter(val) {
                            return jinkai.getData().dictionaryData("caseType", val).text;
                        }
                    },
                    { label: "案件名称", name: "caseName", width: 150, align: "center" },
                    {
                        label: "案件种类", name: "caseCategory", width: 120, align: "center",
                        formatter(val) {
                            return jinkai.getData().dictionaryData("caseCategory", val).text;
                        }
                    },
                    {
                        label: "案件来源", name: "caseSource", width: 120, align: "center",
                        formatter(val) {
                            return jinkai.getData().dictionaryData("caseSource", val).text;
                        }
                    },
                    {
                        label: "来访渠道", name: "channels", width: 120, align: "center",
                        formatter(val) {
                            return jinkai.getData().dictionaryData("channels", val).text;
                        }
                    },
                    { label: "接访人", name: "receiver", width: 130, align: "center" },
                    {
                        label: "办理期限", name: "rerm", width: 80, align: "center",
                        formatter: function (value) {
                            if (value == "") {
                                return "";
                            }
                            return value + "天";
                        }
                    },
                    { label: "扩展字段1", name: "ext1", width: 100, align: "left" },
                    { label: "扩展字段2", name: "ext2", width: 100, align: "left" },
                    { label: "扩展字段3", name: "ext3", width: 100, align: "left" },
                    { label: "扩展字段4", name: "ext4", width: 100, align: "left" },
                    { label: "扩展字段5", name: "ext5", width: 100, align: "left" },
                    {
                        label: "状态", name: "status", width: 100, align: "center",
                        formatter: function (val, opt, row) {
                            var status = val;
                            var name = status == 0 ? "未办理" : "已办理";
                            var cls = status == 0 ? "btn-danger" : "btn-success";
                            var id = row.id;
                            return '<a class="set-status btn ' + cls + ' btn-ss m-r-xs" data-status="' + status + '" data-id="' + opt.rowId + '">' + name + "</a>";
                        }
                    }
                ],
                loadComplete: function () {
                    var dataList = $("#gridTable").gridData();
                    var data;
                    var boolext1 = 0, boolext2 = 0, boolext3 = 0, boolext4 = 0, boolext5 = 0;
                    for (var i in dataList) {
                        data = dataList[i];
                        //判断指定的列是否有数据 
                        if (data.ext1 == null || data.ext1 == "") {
                            if (!boolext1) {
                                boolext1 = 0;
                            }                           
                        } else {
                            boolext1 = 1;
                        }
                        if (data.ext2 == null || data.ext2 == "") {
                            if (!boolext2) {
                                boolext2 = 0;
                            }
                        } else {
                            boolext2 = 1;
                        }
                        if (data.ext3 == null || data.ext3 == "") {
                            if (!boolext3) {
                                boolext3 = 0;
                            }
                        } else {
                            boolext3 = 1;
                        }
                        if (data.ext4 == null || data.ext4 == "") {
                            if (!boolext4) {
                                boolext4 = 0;
                            }
                        } else {
                            boolext4 = 1;
                        }
                        if (data.ext5 == null || data.ext5 == "") {
                            if (!boolext5) {
                                boolext5 = 0;
                            }
                        } else {
                            boolext5 = 1;
                        }
                    }
                    if (boolext1) {
                        $("#gridTable").setGridParam().showCol("ext1");
                    } else {
                        $("#gridTable").setGridParam().hideCol("ext1");
                    }
                    if (boolext2) {
                        $("#gridTable").setGridParam().showCol("ext2");
                    } else {
                        $("#gridTable").setGridParam().hideCol("ext2");
                    }
                    if (boolext3) {
                        $("#gridTable").setGridParam().showCol("ext3");
                    } else {
                        $("#gridTable").setGridParam().hideCol("ext3");
                    }
                    if (boolext4) {
                        $("#gridTable").setGridParam().showCol("ext4");
                    } else {
                        $("#gridTable").setGridParam().hideCol("ext4");
                    }
                    if (boolext5) {
                        $("#gridTable").setGridParam().showCol("ext5");
                    } else {
                        $("#gridTable").setGridParam().hideCol("ext5");
                    }
                    $("#gridTable").trigger("reloadGrid");
                }
            });
        },

        search: function () {
            //起始日期 
            $("#txt_startTime").datePicker({ maxDate: '#F{$dp.$D(\'txt_endTime\') || \'%y-%M-%d\'}' });
            //结束日期
            $("#txt_endTime").datePicker({ minDate: "#F{$dp.$D(\'txt_startTime\')}", maxDate: '%y-%M-%d' });


            $("#search").click(function () {
                var queryJson = {
                    beginDate: $("#txt_startTime").val(),
                    endDate: $("#txt_endTime").val(),
                    matchCon: $("#matchCon").val(),
                    caseSource: $("#caseSource").val(),
                    caseType: $("#caseType").val(),
                    caseCategory: $("#caseCategory").val(),
                    channels: $("#channels").val()
                }
                $("#gridTable").jqGrid("setGridParam", { postData: queryJson }).gridReload();
            });
            //自定义查询
            $(".search-panel > a.btn-default").click(function () {
                $("#timeHorizon a.btn-default").removeClass("active");
                var value = $(this).addClass("active").attr("data-value");
                switch (value) {
                    case "1":
                        $("#txt_startTime").val(jinkai.getDate("yyyy-MM-dd", "d", -7));
                        $("#txt_endTime").val(jinkai.getDate("yyyy-MM-dd"));
                        $("#search").trigger("click");
                        break;
                    case "2":
                        $("#txt_startTime").val(jinkai.getDate("yyyy-MM-dd", "m", -1));
                        $("#txt_endTime").val(jinkai.getDate("yyyy-MM-dd"));
                        $("#search").trigger("click");
                        break;
                    case "3":
                        $("#txt_startTime").val(jinkai.getDate("yyyy-MM-dd", "m", -3));
                        $("#txt_endTime").val(jinkai.getDate("yyyy-MM-dd"));
                        $("#search").trigger("click");
                        break;
                    case "4":
                        $("#timeHorizon .dropdown-menu").show();
                        $("#timeHorizon a.search").on("click", function () {
                            $("#search").trigger("click");
                            $("#timeHorizon .dropdown-menu").hide();
                        });
                        $("#timeHorizon a.btn_close").on("click", function () {
                            $("#timeHorizon .dropdown-menu").hide();
                        });
                        break;
                    default:
                        $("#filter-customer input").val("");
                        $("#filter-goods input").val("");
                        $("#filter-storage input").val("");
                        $("#txt_startTime").val("");
                        $("#txt_endTime").val("");
                        $("#search").trigger("click");
                        break;
                }
            });
            $("#matchCon").keydown(function (e) {
                if (e.keyCode == 13) {
                    $("#search").trigger("click");
                }
            });
        },
        resize: function () {
            $(window).resize(function () {
                $("#gridTable").setGridHeight($(window).height() - 142).setGridWidth($(".main-bd").width());
            });
        }
    }
    $(function () {
        jinkai.filterAuthorize($.page.module);
        $.page.init();
    });
})(jQuery);