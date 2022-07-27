(function ($) {
    var $element = window.frameElement.offsetParent;
    var id = jinkai.request().id;
    var TypeId = jinkai.request().TypeId;
    var requestUrl = "/ajax/dictionaryData.ashx";
    $.page = {
        vue: null,
        init: function () {
            $.page.vue = jinkai.vueInit({ el: "#appForm", data: { dictionaryData: {} } });
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
                                TypeId: function () {
                                    return $.page.vue.dictionaryData.TypeId;
                                },
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
                                TypeId: function () {
                                    return $.page.vue.dictionaryData.TypeId;
                                },
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
                url: requestUrl + "?action=treeView&TypeId=" + jinkai.request().TypeId,
                height: 350,
                search: true,
                open: function (element) {
                    element.removeNode($.page.vue.dictionaryData.Id);
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
                        $.page.vue.dictionaryData = result.data;
                        $.page.vue.dictionaryData.ParentId = result.data.ParentId == "0" ? jinkai.request().TypeId : result.data.ParentId;
                    },
                    error: function (result) {
                        layer.msg(result.msg, { icon: 2 });
                    },
                })
            } else {
                jinkai.vueSet($.page.vue.dictionaryData, "TypeId", jinkai.request().TypeId);
                jinkai.vueSet($.page.vue.dictionaryData, "ParentId", jinkai.request().TypeId);
            }
        },
        save: function () {
            if (!$("#appForm").formValid()) {
                return false;
            }
            var data = $.page.vue.dictionaryData;
            data.TypeId = TypeId;
            data.EnabledMark = data.EnabledMark == undefined ? "0" : data.EnabledMark;
            data["ParentId"] = data.ParentId == data.TypeId ? "0" : data.ParentId;
            var url = id == null ? requestUrl + "?action=add" : requestUrl + "?action=update";
            var title;
            if (url.substring(url.length - 6) == "update") {
                title = "修改成功"
            } else {
                title = "新增成功"
            }
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
            })
        }
    }
    $(function () {
        // jinkai.filterAuthorize();
        $.page.init();
    });
})(jQuery);