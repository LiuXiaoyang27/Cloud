// -----------------------------------------------------------------------
// Eros Fratini - eros@recoding.it
// jqprint 0.3
//
// - 19/06/2009 - some new implementations, added Opera support
// - 11/05/2009 - first sketch
//
// Printing plug-in for jQuery, evolution of jPrintArea: http://plugins.jquery.com/project/jPrintArea
// requires jQuery 1.3.x
//
// Licensed under the MIT license: http://www.opensource.org/licenses/mit-license.php
//------------------------------------------------------------------------

(function ($) {
    var opt;

    $.fn.jqprint = function (options) {
        opt = $.extend({}, $.fn.jqprint.defaults, options);
        var $element = (this instanceof jQuery) ? this : $(this);
        if (opt.operaSupport && $.browser.opera) {
            var tab = window.open("", "jqPrint-preview");
            tab.document.open();
            var doc = tab.document;
        }
        else {
            var $iframe = $("<iframe  />");
            if (!opt.debug) { $iframe.css({ position: "absolute", width: "0px", height: "0px", left: "-600px", top: "-600px" }); }
            $iframe.appendTo("body");
            var doc = $iframe.thisWindow().document;
        }
        if (opt.importCSS) {
            if ($("link[media=print]").length > 0) {
                $("link[media=print]").each(function () {
                    doc.write("<link type='text/css' rel='stylesheet' href='" + $(this).attr("href") + "' media='print' />");
                });
            }
            if (opt.cssFile.length > 0) {
                $.each(opt.cssFile, function (i, item) {
                    doc.write("<link type='text/css' rel='stylesheet' href='" + item + "' media='print' />");
                });
            }
            //else {
            //    $("link").each(function () {
            //        doc.write("<link type='text/css' rel='stylesheet' href='" + $(this).attr("href") + "' />");
            //    });
            //}
        }
        if (opt.printContainer) { doc.write($element.outer()); }
        else { $element.each(function () { doc.write($(this).html()); }); }
        doc.close();
        (opt.operaSupport && $.browser.opera ? tab : $iframe.thisWindow()).focus();
        setTimeout(function () { (opt.operaSupport && $.browser.opera ? tab : $iframe.thisWindow()).print(); if (tab) { tab.close(); } }, 1000);
    }

    $.fn.jqprint.defaults = {
        debug: false,
        importCSS: true,
        cssFile: [],
        printContainer: true,
        operaSupport: false
    };
    jQuery.fn.outer = function () {
        return $($('<div></div>').html(this.clone())).html();
    }
})(jQuery);