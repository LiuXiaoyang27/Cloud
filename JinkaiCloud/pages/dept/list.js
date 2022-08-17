/***************部门管理相关js************************/
/* 修改人:renhonghyu
 * 修改日期:20210802 */

(function ($) {
    $.page = {
        requestUrl: "/ajax/dept.ashx",
        module: "deptList",
        init: function () {
            $.page.toorbar();
            $.page.grid();
            $.page.search();
            $.page.resize();
        },
        toorbar: function () {
            //新建
            $("#btn_add").on("click", function () {
                var right = jinkai.verifyRight($.page.module + "_Add");
                if (right) {
                    jinkai.openSlide({
                        title: "新增部门",
                        url: "form.html",
                        width: 580,
                        callBack: function (name) {
                            window.frames[name].$.page.save();
                        }
                    });
                }
            });
            //编辑
            $("#btn_edit").on("click", function () {
                var right = jinkai.verifyRight($.page.module + "_Edit");
                if (right) {
                    var id = $("#gridTable").gridRowData().Id;
                    jinkai.openSlide({
                        title: "编辑部门",
                        url: "form.html?id=" + id,
                        width: 580,
                        callBack: function (name) {
                            window.frames[name].$.page.save();
                        }
                    })
                }
            });
            //删除
            $("#btn_delete").on("click", function () {
                var right = jinkai.verifyRight($.page.module + "_Delete");
                if (right) {
                    var id = $("#gridTable").gridRowData().Id;
                    jinkai.confirm({
                        content: "您确定要删除部门吗, 是否继续？",
                        callBack: function () {
                            jinkai.ajax({
                                async: false,
                                type: "POST",
                                url: jinkai.toUrl($.page.requestUrl + "?action=delete&id=" + id),
                                param: {},
                                success: function (result) {
                                    jinkai.msg("删除成功", "success");
                                    $("#gridTable").gridReload();
                                },
                                error: function (result) {
                                    jinkai.msg(result.msg, "error");
                                },
                            })
                        }
                    });
                }
            });
        },
        grid: function () {
            $("#gridTable").grid({
                url: $.page.requestUrl + "?action=list",
                styleUI: 'Bootstrap',
                datatype: "json",
                authorize: true,
                height: $(window).height() - 106,
                treeGrid: true,
                ExpandColumn: "FullName",
                ExpandColClick: true, // 点击展开
                colModel: [
                    { lable: "主键", name: "Id", hidden: true, key: true },
                    { label: "名称", name: "FullName", width: 200, align: "left", sortable: false, autowidth: true },
                    { label: "编码", name: "EnCode", width: 150, align: "center", sortable: false },
                    { label: "拼音", name: "SimpleSpelling", width: 150, align: "center", sortable: false },
                    { label: "手机", name: "mobile", width: 150, align: "center", sortable: false },
                    { label: "固话", name: "tel", width: 180, align: "center", sortable: false },
                    { label: "描述", name: "Description", width: 350, align: "center", sortable: false },
                    {
                        label: "状态", name: "EnabledMark", width: 80, align: "center",
                        formatter: function (value) {
                            return value == 1 ? '<span class="switchery switchery-small-xs switchery-checked"><small></small></span>' : '<span class="switchery switchery-small-xs"><small></small></span>';
                        }
                    }
                ]
            });
        },
        search: function () {
            $("#search").click(function () {
                //$("#gridTable").jqGrid("setGridParam", {
                //    postData: {
                //        skey: $("#txt_keyword").val()
                //    }
                //}).gridReload();
                $("#gridTable tr:gt(0)").hide().filter(":contains('" + $("#txt_keyword").val() + "')").show();
            });
            $("#txt_keyword").keydown(function (e) {
                if (e.keyCode == 13) {
                    $("#search").trigger("click");
                }
            });
        },
        resize: function () {
            $(window).resize(function () {
                $("#gridTable").setGridHeight($(window).height() - 106).setGridWidth($(".main-bd").width());
            });
        }
    };
    $(function () {
        jinkai.filterAuthorize($.page.module);
        $.page.init();
    });

})(jQuery);