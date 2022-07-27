(function ($) {
    var id = jinkai.request().id;
    //请求地址
    var requestUrl = "/ajax/news.ashx";

    var editor;
    $.page = {
        vue: null,
        init: function () {
            $.page.vue = jinkai.vueInit({ el: "#appForm", data: { news: {} } })
            $.page.bind();
            $.page.info();
        },
        bind: function () {
            $("#userIds").comboBoxTree({
                data: _top.$.indexData.userTree,
                height: 350,
                open: function (element) {
                    element.removeNode($.page.vue.news.authorId);
                }
            });
            //类型
            $("[name='newsType']").comboBox({
                change: function (id) {
                    if (id == 1) {
                        var status = $.page.vue.news.status;
                        if (status == 1) {
                            $("[name='userIds']").addClass("required").parents("div.form-group").show();
                        } else {
                            $("[name='userIds']").removeClass("required").parents("div.form-group").hide();
                        }
                    } else {
                        $("[name='userIds']").removeClass("required").parents("div.form-group").hide();
                    }
                }
            });
            editor = UE.getEditor('content', {
                toolbars: [[
                    'fullscreen', 'source', '|', 'undo', 'redo', '|',
                'bold', 'italic', 'underline', 'fontborder', 'strikethrough', 'superscript', 'subscript', 'removeformat', 'formatmatch', 'autotypeset', 'blockquote', 'pasteplain', '|', 'forecolor', 'backcolor', 'insertorderedlist', 'insertunorderedlist', 'selectall', 'cleardoc', '|',
                'rowspacingtop', 'rowspacingbottom', 'lineheight', '|',
                'customstyle', 'paragraph', 'fontfamily', 'fontsize', '|',
                'directionalityltr', 'directionalityrtl', 'indent', '|',
                'justifyleft', 'justifycenter', 'justifyright', 'justifyjustify', '|', 'touppercase', 'tolowercase', '|',
                'link', 'unlink', 'anchor', '|', 'imagenone', 'imageleft', 'imageright', 'imagecenter', '|',
                'simpleupload', 'insertimage'
                ]],
                wordCount: false,
                elementPathEnabled: false
            });
            $("[name='status']").on("change", function (e) {
                var status = $.page.vue.news.status;
                var newsType = $.page.vue.news.newsType;
                if (status == 1 && newsType == 1) {
                    $("[name='userIds']").addClass("required").parents("div.form-group").show();
                } else {
                    $("[name='userIds']").removeClass("required").parents("div.form-group").hide();
                }
            })

        },
        //获得数据信息
        info: function () {
            if (id) {
                jinkai.ajax({
                    async: false,
                    type: "GET",
                    url: jinkai.toUrl(requestUrl + "?action=query&id=" + id),
                    param: {},
                    success: function (result) {
                        $.page.vue.news = result.data;
                        editor.ready(function () {
                            editor.setContent(result.data.content);
                            editor.addListener("contentchange", function () {
                                $("#introdution").html(editor.getContent());
                            })
                        });
                    },
                    error: function (result) {
                        jinkai.msg(result.msg, "error");
                    },
                })
            } else {
                jinkai.vueSet($.page.vue.news, "Id", jinkai.request().Id);
            }
        },
        //保存
        save: function () {
            if (!$("#appForm").formValid()) {
                return false;
            }
            if (!editor.hasContents()) {
                window.parent.jinkai.msg("公告内容不能为空", "error");
                return;
            }       
            var data = $.page.vue.news;   
            data["content"] = encodeURI(editor.getContent());
            var url = id == null ? requestUrl + "?action=add" : requestUrl + "?action=update";
            var title;
            if (url.substring(url.length - 6) == "update") {
                title = "修改成功"
            } else {
                title = "新增成功"
            }
            jinkai.ajax({
                async: false,
                type: "Post",
                url: url,
                param: data,
                success: function (result) {
                    window.parent.jinkai.msg(title, "success");
                    window.parent.$("#gridTable").gridReloadSelection();
                    window.parent.jinkai.openSlideClose();
                },
                error: function (result) {
                    window.parent.jinkai.ajaxError(result);
                }
                //beforeSend: function () {
                //    jinkai.loading(true);
                //}
            })
        }
    }
    $(function () {
        $.page.init();
    });
})(jQuery);