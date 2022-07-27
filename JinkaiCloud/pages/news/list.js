(function ($) {
    // 请求接口地址
    var requestUrl = "/ajax/news.ashx";
    $.page = {
        module: "newsData",
        init: function () {
            $.page.toolbar();
            $.page.grid();
            $.page.search();
            $.page.resize();
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
                        title: "新建通知公告",
                        url: "form.html",
                        width: 640,
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
                    var id = $(this).parent().data("id");
                    jinkai.openContent({
                        title: "信息详情",
                        width: "750px",
                        height: "500px",
                        loadComplete: function (name) {
                            jinkai.ajax({
                                async: false,
                                type: "GET",
                                url: jinkai.toUrl("/ajax/news.ashx?action=query&id=" + id),
                                param: {},
                                success: function (result) {
                                    _top.$("#" + name).html('<pre style="margin: 5px;padding: 10px;"><code>' + result.data.content + '</code></pre>').height(430)
                                    _top.$("#" + name).prev(".layui-layer-title").html(result.data.title + "<br/>" + '<label  class="media-meta m-r-sm">' + result.data.author + '</label><label class="media-meta">' + '发布日期：' + result.data.newsDate + '</label>');
                                    _top.$("#" + name).prev(".layui-layer-title").css({ "padding-top": "15px", "height": "auto", "line-height": "inherit" });

                                },
                                error: function (result) {
                                    jinkai.msg(result.msg, "error");
                                },
                            })

                        }
                    });
                }
            });
            //编辑
            $("#gridTable").on("click", ".operating .operate-edit", function (event) {
                var right = jinkai.verifyRight($.page.module + "_Edit");
                if (right) {
                    var id = $(this).parent().data("id");
                    jinkai.openSlide({
                        title: "编辑通知公告",
                        url: "form.html?id=" + id,
                        width: 640,
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
                        content: "删除的数据将不能恢复，请确认是否删除？",
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
                            content: "删除的公告将不能恢复，请确认是否删除？",
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
                        jinkai.msg("请先选择要删除的数据");
                    }
                }
            });
            //发布
            $("#gridTable").on("click", ".operating .operate-send", function (event) {
                var right = jinkai.verifyRight($.page.module + "_Enabled");
                if (right) {   
                    var id = $(this).parent().data("id");
                    var param = {
                        newsId: id,
                        status: 1,
                        userIds: ""
                    }
                    var newsType = $("#gridTable").jqGrid('getRowData', id).newsType;
                    if (newsType == "通知") {
                        jinkai.openWindow({
                            title: "选择通知人员",
                            url: "news/SelectUsers.html",
                            width: "600px",
                            height: "500px",
                            callBack: function (name) {
                                _top.frames[_top.frames.length - 1].$.page.callback(function (data) {
                                    var userIds = "";
                                    if (data != null && data.length > 0) {
                                        data.forEach(function (val, index, a) {
                                            userIds += val.id + ",";
                                        })
                                        userIds = userIds.substring(0, userIds.length - 1);
                                        param.userIds = userIds;
                                        $.page.updateStatus(param);
                                    } else {
                                        jinkai.msg("请选择通知对象", "warning");
                                    }
                                    
                                });
                            }
                        });
                    } else {
                        $.page.updateStatus(param);
                    }
                    
                }
            });
            //取消发布
            $("#gridTable").on("click", ".operating .operate-cancel", function (event) {
                var right = jinkai.verifyRight($.page.module + "_Enabled");
                if (right) {
                    var id = $(this).parent().data("id");
                    var param = {
                        newsId: id,
                        status: 0,
                    }
                    $.page.updateStatus(param);
                }
            });
        },
        updateStatus:function(param){
            jinkai.ajax({
                type: "POST",
                url: jinkai.toUrl(requestUrl + "?action=release"),
                param: param,
                success: function (result) {
                    jinkai.msg(result.msg, "success");
                    $("#gridTable").gridReload();
                },
                error: function (result) {
                    jinkai.msg(result.msg, "error");
                },
            })
        },
        formatterOperate: function (val, opt, row) {
            var status = row.status;
            var html_edit = '<div class="operating" data-id="' + row.id + '">' +
                              '<span class="fa fa-search search-color operate-detail" style=" cursor:pointer" title="详情"></span>';

            var right = jinkai.verifyRight($.page.module + "_Edit", false);
            if (right) {
                if (status != 1) {
                    html_edit += '<span class="fa fa-pencil-square-o edit-color operate-edit" style="cursor:pointer" title="修改"></span>';
                }
            }
            var right = jinkai.verifyRight($.page.module + "_Enabled", false);
            if (right) {
                if (status != 1) {
                    html_edit += '<span class="fa fa-send-o copy-color operate-send" style="cursor:pointer" title="发布"></span>';
                } else {
                    html_edit += '<span class="fa fa-eye-slash delete-color operate-cancel" style="cursor:pointer" title="取消发布"></span>';
                }

            }
            html_edit += '</div>';
            return html_edit;
        },
        grid: function () {
            $("#gridTable").grid({
                authorize: true,
                url: requestUrl + "?action=list",
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
                    { label: "标题", name: "title", width: 220, align: "left", autowidth: true },
                    {
                        label: "类型", name: "newsType", width: 100, align: "center", formatter(val) {
                            if (val == "1") {
                                return "通知";
                            }

                            if (val == "2") {
                                return "公告";
                            }
                        }
                    },
                    { label: "作者", name: "author", width: 200, align: "center" },
                    {
                        label: "发布日期", name: "newsDate", width: 200, align: "center",
                        formatter(val) {
                            if (val == "") {
                                return "";
                            }
                            return jinkai.toDate(val, "yyyy-MM-dd HH:mm:ss");
                        }
                    },
                    {
                        label: "操作时间", name: "modifyTime", width: 200, align: "center",
                        formatter(val) {
                            if (val == "") {
                                return "";
                            }
                            return jinkai.toDate(val, "yyyy-MM-dd HH:mm:ss");
                        }
                    },
                    {
                        label: "状态", name: "status", width: 100, align: "center",
                        formatter: function (value) {
                            if (value == 1) {
                                return '<span class="text-success">已发送</span>';
                            } else {
                                return '<span class="text-danger">存草稿</span>';
                            }
                            //return value == 1 ? '<span class="switchery switchery-small-xs switchery-checked"><small></small></span>' : '<span class="switchery switchery-small-xs"><small></small></span>';
                        }
                    }
                ]
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
                    matchCon: $("#matchCon").val()
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