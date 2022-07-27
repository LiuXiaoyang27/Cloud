(function ($) {
    var $element = window.frameElement.offsetParent;
    var id = jinkai.request().id;
    var index = parent.layer.getFrameIndex(window.name); //获取窗口索引
    var requestUrl = "/ajax/dictionaryType.ashx";
    $.page = {
        vue: null,
        init: function () {
            $.page.vue = jinkai.vueInit({
                el: "#appForm", data: {
                    category: {
                        IsTree: 0
                    }
                }
            })
            $.page.bind();
            $.page.info();
        },
        bind: function () {
            $("#appForm").formValidate({
                onkeyup: false,
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
                url: requestUrl + "?action=treeView&type=edit",
                height: 230,
                open: function (element) {
                    element.removeNode($.page.vue.category.Id);
                }
            });
        },
        info: function () {
            if (id) {
                jinkai.ajax({
                    type: "GET",
                    url: requestUrl + "?action=info&id=" + id,
                    param: {},
                    success: function (result) {
                        $.page.vue.category = result.data;
                        $.page.vue.category.ParentId = result.data.ParentId == "0" ? "apply" : result.data.ParentId
                    },
                    error: function (result) {
                        layer.msg(result.msg);
                    },
                })
            }
        },
        save: function () {
            if (!$("#appForm").formValid()) {
                return false;
            }
            var data = $.page.vue.category;
            data["ParentId"] = data.ParentId == "-1" ? "0" : data.ParentId;
            var url = id == null ? requestUrl + "?action=add" : requestUrl + "?action=update";
            var title;
            if (url.substring(url.length - 6) == "update") {
                title = "修改成功"
            } else {
                title = "新增成功"
            }
            jinkai.ajax({
                type: "POST",
                url: url,
                param: data,
                success: function (result) {
                    window.parent.jinkai.msg(title, "success");
                    window.parent.$("#gridTable").gridReloadSelection();
                    jinkai.thisTab().location.reload();
                    jinkai.openClose();
                },
                beforeSend: function () {
                    jinkai.loading(true);
                }
            })
        }
    }
    $(function () {
        $.page.init();
    });
})(jQuery);