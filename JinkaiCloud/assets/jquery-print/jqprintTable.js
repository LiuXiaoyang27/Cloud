
$.fn.printArea = function (opt) {
    var document = window.document;
    opt = $.extend({
        preview: false,     // 是否预览
        table: false,       // 是否打印table
        usePageStyle: true, // 是否使用页面中的样式
    }, opt);
    var content,
        iframe,
        win,
        links = document.getElementsByTagName("link"),
        html = '<!doctype html><html><head><meta charset="utf-8"><title></title>';
    // 自动添加样式
    for (var i = 0, len = links.length; i < len; i++) {
        if (links[i].rel === 'stylesheet') {
            if (opt.usePageStyle || links[i].href.indexOf('.css') !== -1) {
                html += links[i].outerHTML;
            }
        }
    }

    content = opt.table ? '' : this[0].outerHTML;
    html += '</head><body>' + content + '</body></html>';

    // 构造iframe
    var _self = $(this).clone(), timer, firstCall, win, $html = $(html);
    var isOk = false;
    iframe = document.createElement("iframe");
    iframe.id = "printProxyIframe";
    iframe.frameBorder = 0;
    iframe.setAttribute("style", 'position:absolute;z-index:100;left:0;top:0;width:100%;height:100%;background:#fff;' + (opt.preview ? '' : 'visibility:hidden;'));
    document.body.appendChild(iframe);
    iframe.onload = function () {
        if (iframe.contentWindow == undefined) {
            win = iframe;
        }
        else {
            win = iframe.contentWindow;
        }
        win.canAccess = true;
        isOk = true;
    }
    iframe.src = "javascript:void((function(){document.open();document.domain='" + document.domain + "';document.close()})())";
    var timer = setInterval(function () {
        if (isOk) {
            clearInterval(timer);
            win.onafterprint = function () {
                win.onafterprint = null;
                iframe.parentNode.removeChild(iframe);
            };
            if (opt.table) {
                if (opt.title != undefined) {
                    $(win.document.body).append("<div class='ui-print-title'>" + opt.title + "</div>");
                }
                var $printArea = _self.find('.printArea');
                $printArea.addClass("ui-print");
                $.each($printArea, function (i, item) {
                    var $_area = $(item)
                    //子标题
                    if ($_area.find('.ui-report-subtitle').length > 0) {
                        $(win.document.body).append($_area.find('.ui-report-subtitle'));
                    }
                    //表格内容
                    if ($_area.find('.ui-jqgrid').length > 0) {
                        var $tb = $_area.find("table.ui-jqgrid-htable").eq(0).clone().removeAttr("style").attr("class", "ui-table-print");
                        var $data = $_area.find("table.ui-jqgrid-btable").eq(0).find("tbody").clone();
                        var $summary = $_area.find("table.ui-jqgrid-ftable").find("tbody").clone();
                        $tb.find("th").css("width", "auto");
                        $summary.find("td").css("width", "auto");
                        $data.children().eq(0).remove();
                        $tb.append($data).append($summary);
                        $(win.document.body).append($html).append($tb);
                    }
                    //其他属性.form-group
                    if ($_area.find('.form-group').length > 0) {
                        //属性：printWidth、printHide
                        var $div = $_area.eq(0).clone();
                        $div.find(".form-group").each(function () {
                            var printWidth = $(this).attr("printWidth");
                            if (printWidth != undefined) {
                                $(this).css("width", printWidth);
                            }
                            if ($(this).attr("printHide") == "true") {
                                $(this).remove();
                            }
                        });
                        $(win.document.body).append($div);
                    }
                });
            }
            // 开始打印
            if (timer) {
                clearTimeout(timer);
            }
            timer = setTimeout(function () {
                win.focus();
                win.print();
            }, 100);

            if (!opt.preview) {
                // 自销毁
                setTimeout(function () {
                    iframe.parentNode && iframe.parentNode.removeChild(iframe);
                }, 1000);
            }
        }
    }, 100);
    return this;
};

$.fn.printTable = function (opt) {
    opt = opt || {};
    opt.table = true;
    opt.usePageStyle = false;
    opt.title = opt.title;
    return this.printArea(opt);
};
