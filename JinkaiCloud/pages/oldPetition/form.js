(function ($) {
    var id = jinkai.request().id;
    //请求地址
    var requestUrl = "/ajax/oldPetition.ashx";

    var editor;
    $.page = {
        vue: null,
        init: function () {
            $.page.vue = jinkai.vueInit({ el: "#appForm", data: { oldPetition: {} } })
            $.page.bind();
            $.page.info();
        },
        bind: function () {
            //案件日期
            $("#pDate").datePicker({ dateFmt: "yyyy-MM-dd HH:mm" });
          
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
                        $.page.vue.oldPetition = result.data;
                    },
                    error: function (result) {
                        jinkai.msg(result.msg, "error");
                    },
                })
            } else {
                jinkai.vueSet($.page.vue.oldPetition, "Id", jinkai.request().Id);
            }
        },
        //保存
        save: function () {
            if (!$("#appForm").formValid()) {
                return false;
            }

            var data = $.page.vue.oldPetition;
            var url = requestUrl + "?action=update";
            jinkai.ajax({
                async: false,
                type: "Post",
                url: url,
                param: data,
                success: function (result) {
                    window.parent.jinkai.msg("保存成功", "success");
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