(function ($) {
    var id;
    $.page = {
        init: function () {
            $(".control-label").css({ "min-width": "70px", "text-align": "left", "padding-right": "0px" });
            $(".layout-left-md,.layout-right-md").height($(window).height() - 20);
            $(window).resize(function () {
                $(".layout-left-md,.layout-right-md").height($(window).height() - 20);
            });
            $(".profile-nav li").click(function () {
                $(".profile-nav li").removeClass("active").removeClass("hover");
                $(this).addClass("active");
                $(".tab-pane").hide();
                $("#" + $(this).attr("data-id")).show();
                //if ($(this).attr("data-id") == "authorize-panel") {
                //    //操作权限
                //    $.page.authorizePanel.init();
                //} else

                if ($(this).attr("data-id") == "sysLog-panel") {
                    //操作日志
                    $.page.logPanel.init();
                }
            }).hover(function () {
                $(this).addClass("hover")
            }, function () {
                $(this).removeClass("hover")
            });
            this.iconPanel.init();
            this.userPanel.init();
            this.passwordPanel.init();
        },
        //更换头像
        iconPanel: {
            init: function () {
                $("#up-img-touch").hover(function () {
                    $(this).find(".avatar-over").show();
                }, function () {
                    $(this).find(".avatar-over").hide();
                }).css({ "position": "relative", "overflow": "hidden" });
                $("#up-img-touch").find("#uploadAvatar").css({ "position": "absolute", "font-size": "100px", "right": 0, "top": 0, "opacity": 0, "cursor": "pointer", "z-index": "100" });
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
                    url: jinkai.toUrl("/ajax/admin.ashx?action=avatar"),
                    data: formData,
                    //headers: { Authorize: jinkai.getAuthorize() },
                    cache: false,
                    contentType: false,
                    processData: false,
                    success: function (result) {
                        if (result.status == 200) {
                            jinkai.msg("修改头像成功", "success");
                            document.getElementById("previewAvatar").src = window.URL.createObjectURL(document.getElementById("uploadAvatar").files[0]);
                            jinkai.vueSet($.page.userPanel.vue.user, "HeadIcon", result.msg);
                        } else {
                            jinkai.ajaxError(result);
                        }
                    },
                    beforeSend: function (XHR) {
                        jinkai.loading(true);
                        //XHR.setRequestHeader("Authorize", jinkai.getAuthorize());
                    },
                    error: function (XMLHttpRequest) {
                        //请求出错处理
                        jinkai.ajaxError(XMLHttpRequest);

                    },
                    complete: function () {
                        jinkai.loading(false);
                    }


                });
            }
        },
        //个人资料
        userPanel: {
            vue: null,
            init: function () {
                this.vue = jinkai.vueInit({ el: '#userForm', data: { user: {} } });
                $("#userForm").formValidate({
                    onkeyup: false,
                    onfocusout: function (element) { $(element).valid(); },
                });
                $("input[disabled]").css("background-color", "#f7f8fa");
                //绑定
                this.bind();
                //赋值
                this.setData();
                //保存事件
                $("#btn_save_user").click(function () {
                    $.page.userPanel.saveData();
                });
            },
            bind: function () {
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
            },

            setData: function () {
                var data = jinkai.getData().userInfo();
                $("#user_name").html(data.userName + "/" + data.userAccount);
                // todo
                $.page.userPanel.vue.user = data;
                if (data.roleId == 0) {
                    $.page.userPanel.vue.user.roleName = "系统管理员";
                }
                id = data.userId
                //户名名称
                jinkai.vueSet($.page.userPanel.vue.user, "Name", data.userName);
                // 手机号码
                jinkai.vueSet($.page.userPanel.vue.user, "MobilePhone", data.mobile);
                //注册时间
                jinkai.vueSet($.page.userPanel.vue.user, "CreatorTime", jinkai.toDate(data.creatorTime, "yyyy-MM-dd HH:mm"));
                //最近登录
                jinkai.vueSet($.page.userPanel.vue.user, "PrevLogTime", jinkai.toDate(data.prevLoginTime, "yyyy-MM-dd HH:mm"));

                if (data.avatar != "") {
                    $("#previewAvatar").attr("src", data.avatar);
                }
            },
            saveData: function () {
                if (!$('#userForm').formValid()) {
                    return false;
                }
                //var data = $.page.userPanel.getData();
                var data = $.page.userPanel.vue.user;
                data.Name = data.userName;
                data.userName = data.userAccount;
                data.id = id
                jinkai.ajax({
                    async: true,
                    type: "POST",
                    url: "/ajax/admin.ashx?action=updateUserInfo",
                    param: data,
                    success: function (result) {
                        jinkai.msg("保存成功", "success");
                        _top.$.indexData.userProvider = data;
                    },
                    error: function (result) {
                        window.parent.jinkai.ajaxError(result);

                    }
                });
            }
        },
        //修改密码
        passwordPanel: {
            init: function () {
                //保存
                $("#btn_save_password").click(function () {
                    $.page.passwordPanel.saveData();
                });
            },
            getData: function () {
                var data = {
                    oldPassword: $.md5($.trim($("[name='txt_OldPassword']").val())),
                    password: $.md5($.trim($("[name='txt_NewPassword']").val())),
                    code: $.md5($.trim($("[name='txt_Code']").val())),
                    action: "updatePassword"
                }
                return data;
            },
            saveData: function () {
                if (!$('#passwordForm').formValid()) {
                    return false;
                }
                if ($("[name='txt_NewPassword']").val() == $("[name='txt_OldPassword']").val()) {
                    $("[name='txt_NewPassword']").formValidError("新密码不能与旧密码相同！");
                    return false;
                }
                if ($("[name='txt_RedoNewPassword']").val() != $("[name='txt_NewPassword']").val()) {
                    $("[name='txt_RedoNewPassword']").formValidError("您两次输入的密码不一致");
                    return false;
                }
                jinkai.ajax({
                    async: true,
                    type: "POST",
                    url: jinkai.toUrl("/ajax/admin.ashx"),
                    param: $.page.passwordPanel.getData(),
                    success: function (result) {
                        jinkai.msg(result.msg, "success");
                        setTimeout(function () {
                            //top.isLogOut = true;
                            //jinkai.openTabClose();
                            top.location.href = "/Login.html";
                            //localStorage.removeItem("token");
                        }, 500);
                    },
                    error: function (result) {
                        jinkai.loading(false);
                        jinkai.msg(result.msg, "error");
                        $("#imgcode").trigger("click");
                    },
                    beforeSend: function (request) {
                        //request.setRequestHeader("client-key", jinkai.cookie("client-key"));
                        jinkai.loading(true);
                    },
                    complate: function () {
                        jinkai.loading(false);
                    },
                });
            }
        },

        //系统日志
        logPanel: {
            init: function () {
                $("#sysLog-panel iframe").attr("src", "/pages/user/SysLog.html");
            },
        }
    }
    $(function () {
        $.page.init();
    });
})(jQuery);