(function ($) {
    $.page = {
        module: "options",
        requestUrl: "/ajax/options.ashx",
        vue: null,
        init: function () {
            $.page.vue = jinkai.vueInit({ el: "#sysInfo", data: { config: {} } })
            $.page.bind();
            $.page.info();
        },     
        bind: function () {
            $("#sysInfo").formValidate({
                onkeyup: false,
                onfocusout: function (element) { $(element).valid(); }              
            });
            $(".tab-content").height($(window).height() - 70);
            $(window).resize(function () {
                $(".tab-content").height($(window).height() - 70);
            });           
            //保存配置
            $("#btn_save").on("click", function () {
                if (!$("#sysInfo").formValid()) {
                    return false;
                }
                var params = {
                    root_key1: encodeURI($.page.vue.config.root_key1),
                    root_key2: encodeURI($.page.vue.config.root_key2),
                    root_key3: encodeURI($.page.vue.config.root_key3),
                    action: "updateOptions"
                };               
                jinkai.confirm({
                    content: "您确定要保存当前设置吗, 是否继续？",
                    callBack: function () {                       
                        jinkai.ajax({
                            async: true,
                            type: "POST",
                            url: jinkai.toUrl($.page.requestUrl),
                            param: params,                            
                            success: function (result) {
                                jinkai.msg(result.msg, "success");
                                //parent.window.location.reload();
                            }
                        });
                    }
                });
            });
        },
        info: function () {
            jinkai.ajax({
                async: true,
                type: "GET",
                url: "/ajax/options.ashx?action=getOptions",
                success: function (result) {
                    var data = result.data;
                    jinkai.vueSet($.page.vue.config, "root_key1", data.root_key1);
                    jinkai.vueSet($.page.vue.config, "root_key2", data.root_key2);
                    jinkai.vueSet($.page.vue.config, "root_key3", data.root_key3);
                },
                error: function (result) {
                    window.parent.jinkai.ajaxError(result);

                }
            });

        },
    }
    $(function () {
        jinkai.filterAuthorize($.page.module);
        $.page.init();
    });
})(jQuery);