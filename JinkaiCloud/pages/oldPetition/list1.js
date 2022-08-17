(function ($) {
    var module = "oldPetition";
    $.page = {
        //oper: jinkai.request().oper,
        //rowId: jinkai.request().rowId,
        requestUrl: "/ajax/oldPetition.ashx",
        locationId: "",
        storageInfo: "",
        vue: null,
        init: function () {
            $.page.bind();
        },
        bind: function () {
            $(".tab-content").height($(window).height() - 70);
            $(window).resize(function () {
                $(".tab-content").height($(window).height() - 70);
            });
            $.thisPage.uploaderPdf = $("[name='pdfJson']").upfilePdf();

        }
       
    }
    $(function () {
        jinkai.filterAuthorize(module);
        $.page.init();
    });
})(jQuery);