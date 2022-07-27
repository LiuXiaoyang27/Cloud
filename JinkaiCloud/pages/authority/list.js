(function ($) {
    $.page = {
        userInfo: jinkai.getData().userInfo(),
        requestUrl: "/ajax/admin.ashx",
        module: "authority",
        init: function () {
            $.page.toorbar();
            $.page.grid();
            $.page.search();
            $.page.resize();
        },
        toorbar: function () {
            $("#treeView").treeview({
                height: $(window).height() - 56,
                data: jinkai.getData().deptData(),
                onnodeclick: function (item) {
                    $("#txt_keyword").val("");
                    $("#btn_search").trigger("click");
                }
            })
            //新建
            $("#btn_add").on("click", function () {
                var right = jinkai.verifyRight($.page.module + "_Add");
                if (right) {
                    jinkai.openSlide({
                        title: "新增管理员",
                        url: "form.html",
                        width: 780,
                        callBack: function (name) {
                            window.frames[name].$.page.save();
                        }
                    });
                }
            });

            //重置密码
            $("#btn_password").on("click", function () {
                var right = jinkai.verifyRight($.page.module + "_Edit");
                if (right) {
                    var rowData = $("#gridTable").gridRowData();
                    var id = rowData.id;
                    $.page.resetPwd(rowData.id);
                }

            });
            //编辑
            $("#btn_edit").on("click", function () {
                var right = jinkai.verifyRight($.page.module + "_Edit");
                if (right) {
                    var rowData = $("#gridTable").gridRowData();
                    if (rowData.roleId == 0) {
                        jinkai.msg("系统用户不可编辑！", "warning");
                    } else {
                        jinkai.openSlide({
                            title: "编辑管理员",
                            url: "form.html?id=" + rowData.id,
                            width: 780,
                            callBack: function (name) {
                                window.frames[name].$.page.save();
                            }
                        });
                    }
                }
            });
            //删除
            $("#btn_delete").on("click", function () {
                var right = jinkai.verifyRight($.page.module + "_Delete");
                if (right) {
                    var rowData = $("#gridTable").gridRowData();
                    if (rowData.roleId == 0) {
                        jinkai.msg("系统用户不允许删除！", "warning");
                        return;
                    }
                    if ($.page.userInfo.userId == rowData.id) {
                        jinkai.msg("当前登陆用户不允许删除！", "warning");
                        return;
                    }
                    var id = rowData.id;
                    $.page.del(id + "");
                }
            });
            // 点击状态
            $("#gridTable").on("click", ".set-status", function () {
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
                mtype: "POST",
                url: $.page.requestUrl + "?action=list",
                height: $(window).height() - 178,
                pagination: true,
                cmTemplate: {
                    sortable: !1,
                    title: !1
                },
                colModel: [
                    { lable: "主键", name: "id", hidden: true, key: true },
					{ label: "用户名", name: "username", width: 120, title: !1 },
					{ label: "姓名", name: "name", width: 120, formatter: $.page.formatter.nameFormatter },
                    { label: "手机号码", name: "mobile", width: 120, align: "center" },
                    { label: "部门", name: "deptName", width: 200, align: "left" },
                    { label: "性别", name: "gender", width: 60, align: "center", formatter: $.page.formatter.sexFormatter },
				    { label: "用户角色", name: "roleName", width: 200, align: "left", formatter: $.page.formatter.roleFormatter },
                    { label: "备注", name: "description", width: 180, autowidth: true },
                    { label: "状态", name: "status", width: 100, align: "center", formatter: $.page.formatter.statusFmatter }
                ]
            });
        },
        formatter: {
            sexFormatter: function (val) {
                var name = "";
                switch (val) {
                    case "1":
                        name = "男";
                        break;
                    case "2":
                        name = "女";
                        break;
                    case "3":
                        name = "保密";
                        break;
                }
                return name;
            },
            nameFormatter: function (val) {
                return '<span>' + jinkai.escape(val) + '</span>';
            },
            roleFormatter: function (val, opt, row) {
                if (row.roleId == 0) {
                    return '<span style="color:#007EE6">系统角色</span>';
                } else {
                    return val;
                }
            },
            statusFmatter: function (val, opt, row) {
                if (row.roleId == undefined) {
                    row = $("#gridTable").gridRowData(opt.rowId);
                }
                var status = val;
                var name = status == 0 ? "已禁用" : "已启用";
                var cls = status == 0 ? "btn-danger" : "btn-success";
                var id = row.id;
                if (row.roleId == 0 || $.page.userInfo.userId == row.id) {
                    return '<a disabled style="pointer-events:none;" class="set-status btn ' + cls + ' btn-ss m-r-xs" data-status="' + status + '" data-id="' + id + '" >' + name + "</a>"
                }
                else {
                    return '<a class="set-status btn ' + cls + ' btn-ss m-r-xs" data-status="' + status + '" data-id="' + id + '" >' + name + "</a>"
                }
            }
        },
        search: function () {
            $("#btn_search").click(function () {
                // 获得部门的所有下级部门Ids
                var deptIds = $("#treeView").getChildNodeIds();
                if (deptIds == undefined) {
                    deptIds = "";
                } else {
                    deptIds = deptIds.join();
                }
                $("#gridTable").jqGrid("setGridParam", {
                    postData: {
                        skey: $("#txt_keyword").val(),
                        deptId: deptIds
                    }
                }).gridReload();
            });
            $("#txt_keyword").keydown(function (e) {
                if (e.keyCode == 13) {
                    $("#btn_search").trigger("click");
                }
            });
        },
        //删除
        del: function (id) {
            jinkai.confirm({
                content: "删除的项将不能恢复，请确认是否删除？",
                callBack: function () {
                    jinkai.ajax({
                        async: true,
                        type: "POST",
                        url: jinkai.toUrl($.page.requestUrl),
                        param: {
                            id: id,
                            action: "delete",
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
        // 重置密码
        resetPwd: function (userId) {
            jinkai.confirm({
                content: "<p>您确定要将该用户的密码重置为初始密码嘛?</p><p>初始密码:123456</p>",
                callBack: function () {
                    jinkai.ajax({
                        async: true,
                        type: "POST",
                        url: jinkai.toUrl($.page.requestUrl),
                        param: {
                            userId: userId,
                            action: "resetPassword",
                        },
                        success: function (result) {
                            jinkai.msg("重置密码成功！", "success");
                            //$("#gridTable").gridReload();
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
                    status: Number(status),
                    action: "disable",
                },
                success: function (result) {
                    jinkai.msg("状态已修改！", "success");
                    $("#gridTable").jqGrid("setCell", id, "status", status);
                }, error: function (result) {
                    jinkai.ajaxError(result);
                }
            });
        },
        resize: function () {
            $(window).resize(function () {
                $("#gridTable").setGridHeight($(window).height() - 178).setGridWidth($(".main-bd").width());
            });
        }
    };
    $(function () {
        jinkai.filterAuthorize($.page.module);
        $.page.init();
    });

})(jQuery);