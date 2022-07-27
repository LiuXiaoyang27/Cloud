(function ($) {
    $.page = {
        requestUrl: "/ajax/role.ashx",
        vue: null,
        rowData: null,
        init: function () {
            $.page.vue = jinkai.vueInit({ el: "#appForm", data: { role: {} } });
            $.page.info();
            $.page.bind();
        },
        bind: function () {
            $("#appForm").formValidate({
                onkeyup: false,
                onfocusout: function (element) { $(element).valid(); },
                rules: {
                    name: {
                        remote: {
                            url: $.page.requestUrl,
                            data: {
                                action: "checkName",
                                name: function () {
                                    return encodeURI($.trim($("[name='name']").val()));
                                },
                                id: function () {
                                    return jinkai.request().id == undefined ? '' : jinkai.request().id;
                                }
                            },
                        }
                    }
                },
                messages: {
                    name: { remote: "已存在" },
                }
            });
            $("[name='type']").comboBox();
            $.page.grid.init();
        },
        info: function () {
            var id = jinkai.request().id;
            if (id) {
                jinkai.ajax({
                    url: jinkai.toUrl($.page.requestUrl),
                    param: {
                        action: "query",
                        id: id,
                    },
                    success: function (result) {
                        $.page.rowData = result.data;
                        $.page.vue.role = result.data;
                    }
                });
            } else {
                //默认类型为系统用户
                $("[name='type']").val("2").trigger('change');
            }
        },
        grid: {
            init: function () {
                $("#gridTable").grid({
                    url: "/ajax/menu.ashx?action=list&status=1",
                    height: $(window).height() - 260,
                    cmTemplate: {
                        sortable: !1,
                        title: !1
                    },
                    colModel: [
                        { label: "主键", name: "id", hidden: true, key: true },
                        { label: "<input type='checkbox' id='all'>", name: "id", width: 50, align: "center", formatter: $.page.grid.formatter.groupFmatter },
                        { label: "调用别名", name: "module", width: 100, align: "center", hidden: true },
                        { label: "菜单名称", name: "name", width: 180, align: "left", formatter: $.page.grid.formatter.levelFmatter },
                        { label: "权限", name: "typeNumber", width: 380, align: "left", autowidth: true, formatter: $.page.grid.formatter.typeFmatter },
                    ],
                    loadComplete: function () {
                        $.page.grid.initCheckbox();
                        $.page.grid.changeType();
                    }
                });
            },
            formatter: {
                groupFmatter: function (val, opt, row) {
                    return '<input class="group" type="checkbox" id="' + val + '">';
                },
                // liuyan
                levelFmatter: function (val, opt, row) {
                    var level = parseInt(row.level);
                    var fmatter = '<i class="' + row.fontIcon + '" aria-hidden="true" style="margin-left:'+ level * 20+ 'px"></i>&nbsp;' + val                   
                    return fmatter;
                },
                typeFmatter: function (val, opt, row) {
                    var menuId = row.id;
                    var module = row.module;
                    var actions = row.typeNumber.split(',');
                    var data = ' data-module = "' + module + '" data-id="' + menuId + '" ';
                    var html_con = '<div style="display:inline-block;" class="action-list" data-id="' + menuId + '">';
                    for (var i = 0 ; i < actions.length ; i++) {
                        var action = actions[i];
                        var checked = $.page.grid.typeChecked(menuId, action);
                        if (checked) {
                            html_con += '<label style="margin-left:10px;">' +
                                '<input name="action" class="action" ' + data + ' type="checkbox" value="' + action + '" checked />&nbsp;' + $.page.grid.getActionText(action)
                                + '</label>'
                        } else {
                            html_con += '<label style="margin-left:10px;">' +
                                '<input name="action" class="action" ' + data + ' type="checkbox" value="' + action + '" />&nbsp;' + $.page.grid.getActionText(action)
                                + '</label>'
                        }
                    }
                    html_con += '<div>';
                    return html_con;
                }
            },
            getActionText: function (action) {
                var result = "";
                switch (action) {
                    case "Show":
                        result = "查看";
                        break;
                    case "Add":
                        result = "新增";
                        break;
                    case "Edit":
                        result = "修改";
                        break;
                    case "Delete":
                        result = "删除";
                        break;
                    case "Enabled":
                        result = "启用";
                        break;
                    case "Export":
                        result = "导出";
                        break;
                    case "Import":
                        result = "导入";
                        break;
                    case "Print":
                        result = "打印";
                        break;
                    case "Audit":
                        result = "审核";
                        break;
                    case "ReAudit":
                        result = "反审核";
                        break;
                    case "Upload":
                        result = "上传";
                        break;
                    case "Back":
                        result = "备份";
                        break;
                }
                return result;
            },
            typeChecked: function (menuId, action) {
                if ($.page.rowData) {
                    if ($.page.rowData.isAdmin == 1) {
                        return true;
                    }
                    if ($.page.rowData.items) {
                        for (var i = 0 ; i < $.page.rowData.items.length ; i++) {
                            var item = $.page.rowData.items[i];
                            if (item.menuId == menuId && item.action == action) {
                                return true;
                            }
                        }
                    }
                }
                return false;
            },
            initCheckbox: function () {
                $(("[type='checkbox']")).iCheck({
                    checkboxClass: 'icheckbox_minimal-blue',
                    radioClass: 'iradio_square-blue',
                    increaseArea: '20%'
                });

                //全选
                $('#all').on('ifChecked', function (event) {
                    event.stopPropagation();
                    $('.group').iCheck('check');
                });
                //反选
                $('#all').on('ifUnchecked', function (event) {
                    event.stopPropagation();
                    $('.group').iCheck('uncheck');
                });

                //全选
                $('.group').on('ifChecked', function (event) {
                    event.stopPropagation();
                    var id = $(this).attr('id');
                    $.each($('.action-list'), function () {
                        var typeId = $(this).data("id");
                        if (typeId == id) {
                            $(this).find("input").iCheck('check');
                        }
                    });
                });
                //反选
                $('.group').on('ifUnchecked', function (event) {
                    event.stopPropagation();
                    var id = $(this).attr('id');
                    $.each($('.action-list'), function () {
                        var typeId = $(this).data("id");
                        if (typeId == id) {
                            $(this).find("input").iCheck('uncheck');
                        }
                    });
                });
                if ($.page.rowData && $.page.rowData.type == 1) {
                    $('#all').iCheck('check');
                    $.page.grid.disableCheckbox();
                }      
            },
            changeType: function () {
                $("[name='type']").on("change", function (event) {
                    var type = $(this).val();
                    if (type == "1") {
                        $('#all').iCheck('check');
                        $.page.grid.disableCheckbox();
                    } else {
                        $('#all').iCheck('uncheck');
                        $.page.grid.enableCheckbox();
                    }
                })
            },
            disableCheckbox: function () {
                $(".action-list").find("input").iCheck('disable');
                $(".group").iCheck('disable');
                $("#all").iCheck('disable');
            },
            enableCheckbox: function () {
                $(".action-list").find("input").iCheck('enable');
                $(".group").iCheck('enable');
                $("#all").iCheck('enable');
            },
        },
        save: function () {
            if (!$("#appForm").formValid()) {
                return false;
            }
            var menus = JSON.stringify($.page.getMenus());
            var data = $.page.vue.role;
            if (data.id) {
                data.action = "update";
                data.menus = menus;
            } else {
                data.action = "add";
                data.menus = menus;
            }
            jinkai.ajax({
                async: true,
                type: "POST",
                url: jinkai.toUrl($.page.requestUrl),
                param: data,
                success: function (result) {
                    jinkai.msg(result.msg, "success");
                    jinkai.thisTab().$("#gridTable").gridReloadSelection();
                    jinkai.thisTab().jinkai.openSlideClose();
                },
                error: function (result) {
                    window.parent.jinkai.ajaxError(result);
                }
            });
        },
        getMenus: function () {
            var menus = [];
            $("input[name='action']:checkbox").each(function () {
                if (true == $(this).is(':checked')) {
                    var val = $(this).val();
                    var menuId = $(this).data("id");
                    var item = {
                        action: val,
                        menuId: menuId
                    };
                    menus.push(item);
                }
            });
            return menus;
        }
    }
    $(function () {
        $.page.init();
    });
})(jQuery);

