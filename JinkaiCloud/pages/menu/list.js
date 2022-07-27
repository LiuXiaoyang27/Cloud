(function ($) {
    $.page = {
        requestUrl: "/ajax/menu.ashx",
        module: "menu",
        init: function () {
            $.page.toorbar();
            $.page.grid();
            $.page.resize();
        },
        toorbar: function () {
            //新建
            $("#btn_add").on("click", function () {
                var right = jinkai.verifyRight($.page.module + "_Add");
                if (right) {
                    jinkai.openSlide({
                        title: "新建频道",
                        url: "form.html",
                        width: 580,
                        callBack: function (name) {
                            window.frames[name].$.page.save();
                        }
                    });
                }
            });
            //编辑
            $("#btn_edit").on("click", function () {
                var right = jinkai.verifyRight($.page.module + "_Edit");
                if (right) {
                    var id = $("#gridTable").gridRowData().id;
                    jinkai.openSlide({
                        title: "编辑频道",
                        url: "form.html?id=" + id,
                        width: 580,
                        callBack: function (name) {
                            window.frames[name].$.page.save();
                        }
                    });
                }
            });
            //删除
            $("#btn_delete").on("click", function () {
                var right = jinkai.verifyRight($.page.module + "_Delete");
                if (right) {
                    var id = $("#gridTable").gridRowData().id;
                    $.page.del(id)
                }
            });
            // 点击状态
            $("#gridTable").on("click", ".set-status", function () {
                debugger;
                var right = jinkai.verifyRight($.page.module + "_Enabled");
                if (right) {
                    var id = $(this).data("id");
                    var status = !$(this).data("status");
                    $.page.setStatus(id, status);
                }
            });
        },
        grid: function () {
            $("#gridTable").grid({
                authorize: true,
                url: $.page.requestUrl + "?action=list",
                height: $(window).height() - 106,
                treeGrid: true,
                ExpandColumn: "name",//ExpandColClick: true, //点击行时也会展开/收缩表格
                cmTemplate: {
                    sortable: !1,
                    title: !1
                },
                colModel: [
                    { lable: "主键", name: "id", hidden: true, key: true },
                    { label: "导航名称", name: "name", width: 200, formatter: $.page.formatter.levelFormatter },
					{ label: "调用别名", name: "module", width: 150 },
                    { label: "导航地址", name: "linkUrl", width: 400 },
                    { label: "导航类型", name: "navType", width: 80, align: "center", formatter: $.page.formatter.typeFormatter },
                    { label: "序号", name: "ordnum", width: 100, align: "center" },
                    { label: "备注", name: "remark", width: 300, autowidth: true },
                    { label: "状态", name: "status", width: 120, align: "center", formatter: $.page.formatter.statusFormatter }
                ]
            });
        },
        formatter: {
            levelFormatter: function (val, opt, row) {

                return "<i class=\"p-r-xs i-default " + row.fontIcon + "\" style=\"width: 18px;\"></i>" + val;
            },
            typeFormatter: function (val, opt, row) {
                return val == 1 ? "<i class=\"fa fa-folder i-default \"></i>" :
								"<i class=\"fa fa-internet-explorer i-default \"></i>";
            },
            statusFormatter: function (val, opt, row) {
                var status = val;
                var name = status == 0 ? "已禁用" : "已启用";
                var cls = status == 0 ? "btn-danger" : "btn-success";
                var id = row.id;
                return '<a class="set-status btn ' + cls + ' btn-ss m-r-xs" data-status="' + status + '" data-id="' + opt.rowId + '">' + name + "</a>";
            }
        },
        //删除
        del: function (id) {
            jinkai.confirm({
                content: "删除的数据将不能恢复，请确认是否删除？",
                callBack: function () {
                    jinkai.ajax({
                        async: true,
                        type: "POST",
                        url: jinkai.toUrl($.page.requestUrl),
                        param: {
                            id: id,
                            action: "delete"
                        },
                        success: function (result) {
                            jinkai.msg("删除成功！", "success");
                            $("#gridTable").gridReload();
                        }, beforeSend: function () {
                            jinkai.loading(true);
                        }, complate: function () {
                            jinkai.loading(false);
                        }, error: function (result) {
                            jinkai.ajaxError(result);
                        }
                    });

                }
            });
        },
        //更改状态
        setStatus: function (id, status) {
            id && jinkai.ajax({
                async: true,
                type: "POST",
                url: jinkai.toUrl($.page.requestUrl),
                param: {
                    id: id,
                    disable: Number(status),
                    action: "disable"
                },
                success: function (result) {
                    jinkai.msg("状态已修改！", "success");
                    $("#gridTable").jqGrid("setCell", id, "status", Number(status));
                }, error: function (result) {
                    jinkai.ajaxError(result);
                }
            });
        },
        resize: function () {
            $(window).resize(function () {
                $("#gridTable").setGridHeight($(window).height() - 106).setGridWidth($(".main-bd").width());
            });
        }
    };
    $(function () {
        jinkai.filterAuthorize($.page.module);
        $.page.init();
    });

})(jQuery);