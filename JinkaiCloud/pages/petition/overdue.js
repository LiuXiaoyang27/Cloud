/***************过期案件js************************/
/* 修改人:liuxiaoyang 
 * 修改日期:20220428 */
(function ($) {
    // 请求接口地址
    var requestUrl = "/ajax/petition.ashx";
    $.page = {
        flag: 1,
        init: function () {
            $.page.grid();
            $.page.initEvent();
            $.page.search();
            $.page.resize();

        },
        initEvent: function () {
            // 点击查看详情 
            $("#gridTable").on("click", ".openDetail", function () {
                var id = $(this).data("id");
                jinkai.openWindow({
                    title: "信访详情",
                    url: "/pages/petition/detail.html?id=" + id,
                    width: "780px",
                    height: "550px"
                })

            });

        },
        grid: function () {
            $("#gridTable").grid({
                url: '/ajax/petition.ashx?action=overdue',
                datatype: "json",
                height: $(window).height() - 142,
                colModel: [
                    {
                        label: "日期", name: "createDate", width: 100, align: "center", formatter(val) {
                            if (val == "") {
                                return "";
                            }
                            return jinkai.toDate(val, "yyyy-MM-dd");
                        }
                    },
                    {
                        label: '案件名称', name: 'caseName', width: 150, classes: 'ui-ellipsis', autowidth: true,
                    },
                    {
                        label: "案件种类", name: "caseCategory", width: 120, align: "center",
                        formatter(val) {
                            return jinkai.getData().dictionaryData("caseCategory", val).text;
                        }
                    },
                    { label: "当事人姓名", name: "pName", width: 130, align: "center" },
                    { label: "接访人", name: "receiver", width: 130, align: "center" },
                    {
                        label: "办理期限", name: "rerm", width: 80, align: "center",
                        formatter: function (value) {
                            if (value == "") {
                                return "";
                            }
                            return value + "天";
                        }
                    },
                    {
                        label: "截止日期", name: "rermDate", width: 100, align: "center", formatter(val) {
                            if (val == "") {
                                return "";
                            }
                            return jinkai.toDate(val, "yyyy-MM-dd");
                        }
                    },
                    {
                        label: "过期天数", name: "warning", width: 80, align: "center", formatter: function (value) {
                            if (value == "") {
                                return "";
                            }

                            return '<span style="color:#d9534f">' + Math.abs(value) + "天" + '</span>';
                        }
                    },
                   
                ],
                cmTemplate: {
                    sortable: !1,
                    title: !1
                },
                autowidth: true,
                shrinkToFit: false
            });

            //// 窗体改变大小执行的事件
            //$(window).resize(function (event) {
            //    var $grid = $("#gridTable");
            //    $grid.jqGrid('setGridWidth', $(".main-bd").width() - 40);//去掉border的宽度    
            //});
        },

        search: function () {
            $("#search").click(function () {
                var queryJson = {
                    matchCon: $("#matchCon").val()
                }
                $("#gridTable").jqGrid("setGridParam", { postData: queryJson }).gridReload();
            });
            $("#matchCon").keydown(function (e) {
                if (e.keyCode == 13) {
                    $("#search").trigger("click");
                }
            });
        },
        resize: function () {
            $(window).resize(function () {
                $("#gridTable").setGridHeight($(window).height() - 142).setGridWidth($(".main-bd").width());
            });
        }
    }
    $(function () {
        jinkai.filterAuthorize($.page.module);
        $.page.init();
    });
})(jQuery);