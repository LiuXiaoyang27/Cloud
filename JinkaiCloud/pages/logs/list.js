(function ($) {
    var requestUrl = "/ajax/logs.ashx";
    $.page = {
        module: "operationLog",
        init: function () {
            $.page.toolbar();
            $.page.grid();
            $.page.search();
            $.page.resize();
        },
        toolbar: function () {
            //用户
            $("#filter-user").comboBox({
                search: true,
                url: "/ajax/admin.ashx?action=init",
                height: 200
            });
            //类型
            $("#filter-type").comboBox({
                url: requestUrl + "?action=types",
                height: 200,
            });
            //删除
            $("#btn_delete").on("click", function () {
                var right = jinkai.verifyRight($.page.module + "_Delete");
                if (right) {
                    jinkai.confirm({
                        content: "删除的日志信息将不能恢复，请确认是否删除？",
                        callBack: function () {
                            jinkai.ajax({
                                type: "POST",
                                url: jinkai.toUrl(requestUrl + "?action=delete"),
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
        },
        grid: function () {
            $("#gridTable").grid({
                authorize: true,
                url: requestUrl + "?action=list",
                styleUI: 'Bootstrap',
                datatype: "json",
                height: $(window).height() - 142,
                pagination: true,
                colModel: [
                    { label: "主键", name: "id", hidden: true, key: true },
                    {
                        label: "级别", name: "infoLevel", width: 60, align: "center", formatter: function (value) {
                            if (value) {
                                var info = "";
                                switch (value) {
                                    case "2":
                                        info = "<span class='fa fa-exclamation-circle' title='警告级别' style='color:#FFC107;font-size:17px'></span>"
                                        break;
                                    case "3":
                                        info = "<span class='fa fa-times-circle' title='错误级别' style='color:#FF0000;font-size:17px'></span>"
                                        break;
                                    default:
                                        info = "<span class='fa fa-info-circle' title='普通级别' style='color:#2196F3;font-size:17px'></span>"
                                        break;
                                }
                                return info;
                            } else {
                                return "<span class='fa fa-info-circle' title='普通级别' style='color:#2196F3;font-size:17px'></span>"
                            }
                        }
                    },
                    { label: "用户名", name: "userName", width: 150, align: "left" },
                    { label: "操作日期", name: "modifyTime", width: 150, align: "center" },
                    { label: "姓名", name: "nickName", width: 150, align: "left" },
                    { label: "操作类型", name: "typeName", width: 200, align: "left" },
                    { label: "操作内容", name: "detail", width: 450, align: "left", autowidth: true },
                ]
            });
        },
        search: function () {
            ////起始日期
            //$("#txt_startTime").datePicker();
            ////结束日期
            //$("#txt_endTime").datePicker({ minDate: "#F{$dp.$D(\'txt_startTime\')}" });

            //起始日期
            $("#txt_startTime").datePicker({ maxDate: '#F{$dp.$D(\'txt_endTime\') || \'%y-%M-%d\'}' });
            //结束日期
            $("#txt_endTime").datePicker({ minDate: "#F{$dp.$D(\'txt_startTime\')}", maxDate: '%y-%M-%d' });

            //搜索事件
            $("#btn_search").click(function () {
                var queryJson = {
                    keyword: $("#txt_keyword").val(),
                    user: $("#filter-user").val(),
                    type: $("#filter-type").val(),
                    fromDate: $("#txt_startTime").val(),
                    toDate: $("#txt_endTime").val(),
                };
                $("#gridTable").jqGrid("setGridParam", { postData: queryJson }).gridReload();
            });
            $("#txt_keyword").keydown(function (e) {
                if (e.keyCode == 13) {
                    $("#btn_search").trigger("click");
                }
            });
            //自定义查询
            $(".search-panel > a.btn-default").click(function () {
                $("#timeHorizon a.btn-default").removeClass("active");
                var value = $(this).addClass("active").attr("data-value");
                switch (value) {
                    case "1":
                        $("#txt_startTime").val(jinkai.getDate("yyyy-MM-dd", "d", -7));
                        $("#txt_endTime").val(jinkai.getDate("yyyy-MM-dd"));
                        $("#btn_search").trigger("click");
                        break;
                    case "2":
                        $("#txt_startTime").val(jinkai.getDate("yyyy-MM-dd", "m", -1));
                        $("#txt_endTime").val(jinkai.getDate("yyyy-MM-dd"));
                        $("#btn_search").trigger("click");
                        break;
                    case "3":
                        $("#txt_startTime").val(jinkai.getDate("yyyy-MM-dd", "m", -3));
                        $("#txt_endTime").val(jinkai.getDate("yyyy-MM-dd"));
                        $("#btn_search").trigger("click");
                        break;
                    case "4":
                        $("#timeHorizon .dropdown-menu").show();
                        $("#timeHorizon a.btn_search").on("click", function () {
                            $("#btn_search").trigger("click");
                            $("#timeHorizon .dropdown-menu").hide();
                        });
                        $("#timeHorizon a.btn_close").on("click", function () {
                            $("#timeHorizon .dropdown-menu").hide();
                        });
                        break;
                    default:
                        $("#txt_startTime").val("");
                        $("#txt_endTime").val("");
                        $("#btn_search").trigger("click");
                        break;
                }
            });
        },
        resize: function () {
            $(window).resize(function () {
                $("#" + $(".btn-operate").attr("data-trigger")).setGridHeight($(window).height() - 146).setGridWidth($(".main-bd").width());
            });
        }
    }
    $(function () {
        jinkai.filterAuthorize($.page.module);
        $.page.init();
    });
})(jQuery);
