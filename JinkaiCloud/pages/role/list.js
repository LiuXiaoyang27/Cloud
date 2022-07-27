(function ($) {
    $.page = {
        requestUrl: "/ajax/role.ashx",
        module: "role",
        init: function () {
            $.page.toorbar();
            $.page.grid();
            $.page.search();
            $.page.resize();
        },
        toorbar: function () {
            //新建
            $("#btn_add").on("click", function () {
                var right = jinkai.verifyRight($.page.module + "_Add");
                if (right) {
                    jinkai.openSlide({
                        title: "新增角色",
                        url: "form.html",
                        width: 820,
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
                        title: "编辑角色",
                        url: "form.html?id=" + id,
                        width: 820,
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
                    var rowData = $("#gridTable").gridRowData();
                    var admin = rowData.isAdmin;
                    if (admin == "1") {
                        jinkai.msg("系统角色不允许删除！", "warning");
                    } else {
                        var id = rowData.id;
                        $.page.del(id);
                    }
                }
            });
            // 点击状态
            $("#gridTable").on("click", ".set-status", function () {
                var right = jinkai.verifyRight($.page.module + "_Enabled");
                if (right) {
                    var admin = $(this).data("admin");
                    if (admin == "1") {
                        jinkai.msg("系统角色不允许禁用！", "warning");
                    } else {
                        var id = $(this).data("id");
                        var status = !$(this).data("status");
                        $.page.setStatus(id, status);
                    }
                }
            });
        },
        grid: function () {
            $("#gridTable").grid({
                url: $.page.requestUrl + "?action=list",
                height: $(window).height() - 142,
                pagination: true,
                cmTemplate: {
                    sortable: !1,
                    title: !1
                },
                colModel: [
                    { lable: "主键", name: "id", hidden: true, key: true },
					{ label: "角色名称", name: "name", width: 200 },
                    { label: "角色类型", name: "type", width: 150, align: "center", formatter: $.page.formatter.typeFmatter },
                    { label: "类型说明", name: "typeName", width: 150, align: "center", formatter: $.page.formatter.typeNameFmatter },
                    { label: "系统角色", name: "remark", width: 150, align: "center", formatter: $.page.formatter.adminFmatter },
                    { label: "状态", name: "status", width: 120, align: "center", formatter: $.page.formatter.statusFmatter }
                ]
            });
        },
        formatter: {
            typeFmatter: function (val, opt, row) {
                if (row.type == 1) {
                    return '<span style="color:#007EE6">超级用户</span>';
                } else {
                    return "<span>系统用户</span>";
                }
            },
            typeNameFmatter: function (val, opt, row) {
                if (row.type == 1) {
                    return '<span style="color:#007EE6">所有权限</span>';
                } else {
                    return "<span>分配权限</span>";
                }
            },
            adminFmatter: function (val, opt, row) {
                if (row.isAdmin == 1) {
                    return '<span style="color:#007EE6">是</span>';
                } else {
                    return "<span>否</span>";
                }
            },
            statusFmatter: function (val, opt, row) {
                var status = val;
                var name = status == 0 ? "已禁用" : "已启用";
                var cls = status == 0 ? "btn-danger" : "btn-success";
                if (row.isAdmin == "1") {
                    return '<a disabled style="pointer-events:none;" class="set-status btn ' + cls + ' btn-ss m-r-xs" data-status="' + status + '" data-id="' + opt.rowId + '" data-admin="' + row.isAdmin + '">' + name + "</a>";
                } else {
                    return '<a class="set-status btn ' + cls + ' btn-ss m-r-xs" data-status="' + status + '" data-id="' + opt.rowId + '" data-admin="' + row.isAdmin + '">' + name + "</a>";
                }
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
        search: function () {
            $("#btn_search").click(function () {
                $("#gridTable").jqGrid("setGridParam", {
                    postData: {
                        skey: $("#txt_keyword").val()
                    }
                }).gridReload();
            });
            $("#txt_keyword").keydown(function (e) {
                if (e.keyCode == 13) {
                    $("#btn_search").trigger("click");
                }
            });
        },
        resize: function () {
            $(window).resize(function () {
                $("#gridTable").setGridHeight($(window).height() - 142).setGridWidth($(".main-bd").width());
            });
        }
    };
    $(function () {
        jinkai.filterAuthorize($.page.module);
        $.page.init();
    });

})(jQuery);
