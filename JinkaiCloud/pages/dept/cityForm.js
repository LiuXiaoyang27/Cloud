/***************部门管理相关js************************/
/* 修改人:renhonghyu
 * 修改日期:20210802 */
var id = jinkai.request().id;
var requestUrl = "/ajax/dept.ashx";
(function ($) {
    $.page = {
        vue: null,
        init: function () {
            $.page.vue = jinkai.vueInit({ el: "#appForm", data: { city: {} } })
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
                url: requestUrl + "?action=treeView&Category=city",
                height: 350,
                search: true,
                open: function (element) {
                    element.removeNode($.page.vue.city.Id);
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
                        $.page.vue.city = result.data.PropertyJson == null ? "" : JSON.parse(result.data.PropertyJson);
                        $.page.vue.city.Id = result.data.Id;
                        $.page.vue.city.SortCode = result.data.SortCode;
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
            var data = {
                Id: $.page.vue.city.Id == undefined ? null : $.page.vue.city.Id,
                Category: "city",
                ParentId: $("[name='ParentId']").val() == "-1" ? "0" : $("[name='ParentId']").val(),
                EnCode: $("[name='EnCode']").val(),
                FullName: $("[name='FullName']").val(),
                EnabledMark: $.page.vue.city.EnabledMark == undefined ? 0 : $.page.vue.city.EnabledMark,
                Description: $("[name='Description']").val(),
                PropertyJson: JSON.stringify($.page.vue.city),
            };
            var url = data.Id == null ? requestUrl + "?action=add" : requestUrl + "?action=update";
            jinkai.ajax({
                async: false,
                type: "POST",
                url: url,
                param: data,
                success: function (result) {
                    jinkai.msg(result.msg, "success");
                    jinkai.thisTab().$("#gridTable").gridReloadSelection();
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