/***************部门管理相关js************************/
/* 修改人:renhonghyu
 * 修改日期:20210802 */
var id = jinkai.request().id;
var requestUrl = "/ajax/dept.ashx";
(function ($) {
    $.page = {
        vue: null,
        init: function () {
            $.page.vue = jinkai.vueInit({ el: "#appForm", data: { dept: {} } })
            $.page.bind();
            $.page.info();
        },
        bind: function () {
            $("#appForm").formValidate({
                onkeyup: false,
                onfocusout: function (element) { $(element).valid(); },
                rules: {
                    FullName: {
                        remote: {
                            url: requestUrl + "?action=checkFullName",
                            data: {
                                field: "FullName",
                                value: function () {
                                    return $("[name='FullName']").val();
                                },
                                id: function () {
                                    return jinkai.request().id == undefined ? '' : jinkai.request().id;
                                },
                                parentId: function () {
                                    return $("[name='ParentId']").val();
                                }
                            },
                        }
                    },
                    EnCode: {
                        remote: {
                            url: requestUrl + "?action=checkEnCode",
                            data: {
                                field: "EnCode",
                                value: function () {
                                    return $("[name='EnCode']").val();
                                },
                                id: function () {
                                    return jinkai.request().id == undefined ? '' : jinkai.request().id;
                                },
                                parentId: function () {
                                    return $("[name='ParentId']").val();
                                }
                            },
                        }
                    }
                },
                messages: {
                    FullName: { remote: "已存在" },
                    EnCode: { remote: "已存在" },
                }
            });
            $("[name='ParentId']").comboBoxTree({
                url: requestUrl + "?action=treeView&type=dept",
                height: 350,
                search: true,
                open: function (element) {
                    element.removeNode($.page.vue.dept.Id);
                }
            });
        },
        info: function () {
            if (id) {
                jinkai.ajax({
                    async: false,
                    type: "GET",
                    url: requestUrl + "?action=info&id=" + id,
                    param: {},
                    success: function (result) {
                        $.page.vue.dept = result.data;
                        $.page.vue.dept.ParentId = result.data.ParentId == "0" ? "-1" : result.data.ParentId;
                    },
                    error: function (result) {
                        layer.msg(result.msg, { icon: 2 });
                    },
                })
            }
        },
        save: function () {
            if (!$("#appForm").formValid()) {
                return false;
            }
            debugger;
            var data = $.page.vue.dept;
            data.ParentId = data.ParentId == -1 ? "0" : data.ParentId;
            data.EnabledMark = data.EnabledMark == undefined ? "0" : data.EnabledMark;
            var url = id == null ? requestUrl + "?action=add" : requestUrl + "?action=update";
            var title;
            if (url.substring(url.length - 6) == "update") {
                title = "修改成功"
            } else {
                title = "新增成功"
            }
            var aaa = data;
            jinkai.ajax({
                async: false,
                type: "POST",
                url: url,
                param: data,
                success: function (result) {
                    window.parent.jinkai.msg(title, "success");
                    window.parent.$("#gridTable").gridReloadSelection();
                    jinkai.thisTab().jinkai.openSlideClose();
                },
                beforeSend: function () {
                    jinkai.loading(true);
                }
            });
        }
    }
    $(function () {
        $.page.init();
    });
})(jQuery);