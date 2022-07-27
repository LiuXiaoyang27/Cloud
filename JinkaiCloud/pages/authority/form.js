(function ($) {
    var id = jinkai.request().id;
    var requestUrl = "/ajax/admin.ashx";
    $.page = {
        vue: null,
        init: function () {
            $.page.vue = jinkai.vueInit({ el: "#appForm", data: { user: {} } })
            $.page.bind();
            $.page.info();
        },
        bind: function () {
            $("#appForm").formValidate({
                onkeyup: false,
                onfocusout: function (element) { $(element).valid(); },
                rules: {
                    mobile: {
                        remote: {
                            url: requestUrl,
                            data: {
                                action: "checkMobile",
                                MobilePhone: function () {
                                    return $("[name='mobile']").val();
                                },
                                id: function () {
                                    return jinkai.request().id == undefined ? '' : jinkai.request().id;
                                }
                            },
                        }
                    },
                    username: {
                        remote: {
                            url: requestUrl,
                            data: {
                                action: "checkUserName",
                                username: function () {
                                    return $("[name='username']").val();
                                },
                                id: function () {
                                    return jinkai.request().id == undefined ? '' : jinkai.request().id;
                                }
                            },
                        }
                    },
                    pswConfirm: {
                        equalTo: "#password"
                    }
                },
                messages: {
                    mobile: { remote: "已存在" },
                    username: { remote: "已存在" },
                }
            });
            //角色
            $("[name='roleId']").comboBox({
                url: "/ajax/role.ashx?action=batch"
            });
            //部门
            $("[name='deptId']").comboBoxTree({
                search: true,
                height: 350,
                data: jinkai.getData().deptData(),
            });
            //性别
            $("[name='gender']").comboBox();
            //民族
            $("[name='nation']").comboBox({
                search: true,
                height: 300,
                data: jinkai.getData().dictionaryData("nation"),
            });
            //入职日期
            $("[name='entryDate']").datePicker({ maxDate: "%y-%M-%d" });
            //出生年月
            $("[name='birthday']").datePicker({ maxDate: "%y-%M-%d" });
            //学历
            $("[name='education']").comboBox({
                height: 300,
                data: jinkai.getData().dictionaryData("education"),
            });
            //证件
            $("[name='certificatesType']").comboBox();
            //头像上传
            $("#up-img-touch").css({ "position": "relative", "overflow": "hidden" });
            $("#up-img-touch").find("#uploadAvatar").css({ "position": "absolute", "font-size": "170px", "right": 0, "top": 0, "opacity": 0, "cursor": "pointer", "z-index": "100" });
            $("#up-img-touch").find("#uploadAvatar").change(this.uploadAvatar);
        },
        uploadAvatar: function () {
            var file = $("#uploadAvatar").get(0).files[0];
            if (file == undefined) {
                return false;
            }
            var formData = new FormData();
            formData.append("file", file);
            $.ajax({
                async: false,
                type: "POST",
                url: jinkai.toUrl("/ajax/upload.ashx?action=avatar"),
                data: formData,
                cache: false,
                contentType: false,
                processData: false,
                success: function (result) {
                    if (result.status == 200) {
                        jinkai.msg("修改头像成功", "success");
                        document.getElementById("previewAvatar").src = window.URL.createObjectURL(document.getElementById("uploadAvatar").files[0]);
                        jinkai.vueSet($.page.vue.user, "HeadIcon", result.msg);
                    } else {
                        jinkai.ajaxError(result);
                    }
                },
                beforeSend: function (XHR) {
                    jinkai.loading(true);
                },
                error: function (XMLHttpRequest) {
                    //请求出错处理
                    jinkai.ajaxError(XMLHttpRequest);

                },
                complete: function () {
                    jinkai.loading(false);
                }


            });
        },
        info: function () {
            if (id) {
                jinkai.ajax({
                    async: false,
                    type: "GET",
                    url: jinkai.toUrl(requestUrl + "?action=query&id=" + id),
                    param: {},
                    success: function (result) {
                        $("[name='username']").attr("disabled", true).css("background-color", "#f7f8fa");//用户名不可修改
                        if (result.data.avatar != "") {
                            $("#previewAvatar").attr("src", result.data.avatar);
                            jinkai.vueSet($.page.vue.user, "HeadIcon", result.data.avatar);
                        }
                        $.page.vue.user = result.data;
                    },

                    error: function (result) {
                        jinkai.msg(result.msg, "error");
                    },
                })
            }
        },
        save: function () {
            if (!$("#appForm").formValid()) {
                return false;
            }
            var data = $.page.vue.user;
            data.isAdmin = $.page.vue.user.isAdmin == undefined ? 0 : $.page.vue.user.isAdmin;
            var url = id == null ? requestUrl + "?action=add" : requestUrl + "?action=update";
            var title;
            if (url.substring(url.length - 6) == "update") {
                title = "修改成功"
            } else {
                title = "新增成功"
            }
            jinkai.ajax({
                async: true,
                type: "POST",
                url: url,
                param: data,
                success: function (result) {
                    jinkai.msg(title, "success");
                    jinkai.thisTab().$("#gridTable").gridReloadSelection();
                    jinkai.thisTab().jinkai.openSlideClose();
                },
                error: function (result) {
                    window.parent.jinkai.ajaxError(result);

                }
            });
        },
    }
    $(function () {
        $.page.init();
    });
})(jQuery);