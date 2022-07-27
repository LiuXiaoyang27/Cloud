(function ($) {
    $.page = {
        init: function () {
            $(".content").height($(window).height() - 50);
            $.page.bind();
        },
        bind: function () {
            jinkai.loading(true);
            var filePath = jinkai.request().filePath;
            //$(".content").find("iframe").attr("src", jnpf.toUrl("/api/Extend/DocumentPreview/PreviewFile/" + jnpf.request().fileId));
            $(".content").find("iframe").attr("src", filePath);
            //$(".content").find("iframe").load(function () {
            //    jinkai.loading(false);
            //});
            jinkai.loading(false);
        }
    }
    $(function () {
        $.page.init();
        //$("#moduleSlide-title").html(jinkai.request().fileId)
        $("#btn_close").click(function () {     
            jinkai.thisTab().jinkai.openSlideClose();
        });
       
    });
})(jQuery);