(function ($) {
    $.page = {
        requestUrl: "/ajax/menu.ashx",
        vue: null,
        init: function () {
            $.page.vue = jinkai.vueInit({
                el: "#appForm", data: {
                    menu: {
                        ordnum: 99,
                        typeNumber: "",
                        LinkTarget: "_self"
                    }
                }
            })
            $.page.bind();
            $.page.event();
            $.page.info();
        },
        bind: function () {
            $("#appForm").formValidate({
                onkeyup: false,
                onfocusout: function (element) { $(element).valid(); },
                rules: {
                    module: {
                        remote: {
                            url: $.page.requestUrl,
                            data: {
                                action: "checkModule",
                                module: function () {
                                    return $.trim($("[name='module']").val());
                                },
                                id: function () {
                                    return jinkai.request().id == undefined ? '' : jinkai.request().id;
                                }
                            },
                        }
                    }
                },
                messages: {
                    module: { remote: "已存在" },
                }
            });

            $("[name='parentId']").comboBoxTree({
                search: true,
                url: $.page.requestUrl + "?action=treeView",
                height: 350,
                open: function (element) {
                    element.removeNode($.page.vue.menu.id + "");
                }
            });
            $("[name='navType']").comboBox({
                change: function (id) {
                    if (id == 1) {
                        $("[name='linkUrl']").removeClass("required").parents("div.form-group").hide();
                    } else if (id == 2) {
                        $("[name='linkUrl']").removeClass("required").parents("div.form-group").show();
                    }
                }
            });
            $("[name='LinkTarget']").next("ul.dropdown-menu").find("li").on("click", function () {
                jinkai.vueSet($.page.vue.menu, "LinkTarget", $(this).find("a").html());
            });
        },
        event: function () {
            //选择图标
            $("#btnadd").on("click", function () {
                jinkai.openWindow({
                    title: "选择图标",
                    url: "/pages/menu/icon.html",
                    width: "500px",
                    height: "450px",
                });
            });
        },
        info: function () {
            var params = jinkai.request();
            var id = params.id;
            if (id) {
                jinkai.ajax({
                    url: jinkai.toUrl($.page.requestUrl),
                    param: {
                        action: "query",
                        id: jinkai.request().id
                    },
                    success: function (result) {
                        var data = result.data;
                        $.page.vue.menu = data;
                        $("#showIcon i").addClass("fa " + data.fontIcon);
                        $.page.vue.menu.parentId = data.parentId == "0" ? "-1" : data.parentId;
                        var types = data.typeNumber.split(",");
                        for (var i = 0 ; i < types.length; i++) {
                            $.each($('input:checkbox'), function () {
                                if ($(this).val() == types[i]) {
                                    this.checked = true;
                                }
                            });
                        }
                        $("[name='module']").attr("disabled", "disabled");
                    }
                });
            }
        },
        save: function () {
            if (!$("#appForm").formValid()) {
                return false;
            }
            var types = $.page.getTypeNums();
            var data = $.page.vue.menu;
            data.parentId = data.parentId == "-1" ? "0" : data.parentId;
            data.typeNumber = types;
            console.log(data)
            if (jinkai.request().id) {
                data.action = "update";
            } else {
                data.action = "add";

            }
            jinkai.ajax({
                async: true,
                type: "POST",
                url: jinkai.toUrl($.page.requestUrl),
                param: data,
                success: function (result) {
                    jinkai.msg(result.msg, "success");
                    jinkai.thisTab().location.reload();
                    jinkai.openClose();
                },
                error: function (result) {
                    window.parent.jinkai.ajaxError(result);
                }
            });
        },
        getTypeNums: function () {
            var types = "";
            $.each($('input:checkbox'), function () {
                if (this.checked) {
                    if (types == "") {
                        types = $(this).val();
                    } else {
                        types += "," + $(this).val();
                    }
                }
            });
            return types;
        }
    }
    $(function () {
        $.page.init();
    });
})(jQuery);