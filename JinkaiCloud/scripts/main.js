(function ($) {
    $.page = {
        userinfo: jinkai.getData().userInfo(),
        init: function () {
            jinkai.ajax({
                url: jinkai.toUrl("/ajax/utils.ashx?action=index"),
                success: function (result) {
                    var data = result.data;
                    $.page.initTotal(data.total);
                    $.page.initChart(data.act);
                    $.page.initNews(data.news);
                    $.page.initGrid();
                    $.page.initEvent();
                }
            });
        },
        initTotal: function (data) {
            $("#petition_count").html(data.petition_count);
            $("#finish_count").html(data.finish_count);
            $("#user_count").html(data.user_count);
            $("#act_count").html(data.notfinish_count);
            $("#lbl_overdueCount").html(data.overdue_count);
        },
        initEvent: function () {
            // 点击查看详情 
            $("#gridTable").on("click", ".openDetail", function () {

                var id = $(this).data("id");
                jinkai.petitionDetail(id);

            });

        },
        initGrid: function () {
            $("#gridTable").grid({
                url: '/ajax/petition.ashx?action=warnlist',
                datatype: "json",
                height: 200,
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
                        formatter: function (val, opt, row) {
                            return '<span class="petition_link openDetail" data-id="' + opt.rowId + '" style="color:#005CA1">' + val + '</span>'
                        }
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
                        label: "剩余天数", name: "warning", width: 80, align: "center", formatter: function (value) {
                            if (value == "") {
                                return "";
                            }
                            return value + "天";
                        }
                    }
                ],
                cmTemplate: {
                    sortable: !1,
                    title: !1
                },
                autowidth: true,
                shrinkToFit: false,
                gridComplete: function () {
                    var ids = $("#gridTable").getDataIDs();
                    for (var i = 0; i < ids.length; i++) {
                        var rowData = $("#gridTable").getRowData(ids[i]);
                        var warn = rowData.warning;
                        warn = warn.substring(0, warn.length - 1);
                        if (warn <= 1) {
                            $("#gridTable").find('#' + ids[i]).find("td").addClass("warn-red");
                        } else if (1 < warn <= 3) {
                            $("#gridTable").find('#' + ids[i]).find("td").addClass("warn-yellow");
                        } else {
                            $("#gridTable").find('#' + ids[i]).find("td").addClass("warn-blue");
                        }
                    }
                }
            });

            // 窗体改变大小执行的事件
            $(window).resize(function (event) {
                var $grid = $("#gridTable");
                $grid.jqGrid('setGridWidth', $(".main-bd").width() - 40);//去掉border的宽度    
            });
        },
        initNews: function (items) {
            var html = "";
            for (var i = 0 ; i < items.length ; i++) {
                var data = items[i];
                var typeName = data.newsType == "1" ? "通知" : "公告";
                var datetime = jinkai.toDate(data.newsDate, "MM-dd");
                var li = '<li style="color:#005CA1"><a style="color:#005CA1" data-id="' + data.id + '" href="javascript:void(0)">【' + typeName + '】' + data.title + '</a><span class="time">' + datetime + '</span></li>';
                html += li;
            }

            $(".noticelist").html(html);
            $(".noticelist a").click(function () {
                var dataId = $(this).attr("data-id");
                jinkai.newsDetail(dataId);
            });
        },
        initChart: function (data) {

            var dataAxis = [];

            var num = [];
            for (var i = 0 ; i < data.length ; i++) {
                dataAxis[i] = data[i].name;
                num[i] = data[i].value;
            }
            var storage = echarts.init(document.getElementById("storage"));
            var options = {
                tooltip: {
                    trigger: 'axis',
                },
                legend: {
                    data: ["案件数量"]
                },
                grid: {
                    top: 60,
                    left: 40,
                    right: 40,
                    bottom: 20,
                    containLabel: true
                },
                calculable: true,

                xAxis: [
                    {
                        type: 'category',
                        axisTick: { show: false },
                        data: dataAxis,
                        axisLabel: {
                            interval: 0
                        }
                    }
                ],
                yAxis: [
                    {
                        type: 'value',
                        name: "件",
                    }
                ],
                series: [
                    {
                        name: "案件数量",
                        type: "line",
                        //stack: unit,
                        label: {
                            normal: {
                                position: 'top',
                                show: true
                            }
                        },
                        data: num
                    }
                ]
            };
            storage.setOption(options, true);

        }
    }
    $(function () {
        $.page.init();
    });
})(jQuery);