(function ($) {
    // 请求地址
    var requestUrl = "/ajax/dictionaryType.ashx";
    var requestUrlData = "/ajax/dictionaryData.ashx";
    $.page = {
        selRowID: 0,
        module: "dictionaryList",
        init: function () {
            $.page.toolbar();
            $.page.tree();
            $.page.search();
            $.page.resize();
        },
        toolbar: function () {
            //新建
            $("#btn_add").on("click", function () {
                var right = jinkai.verifyRight($.page.module + "_Add");
                if (right) {
                    var selectedNode = $("#treeView").treeview().getSelectedData();
                    console.log(selectedNode);
                    if (selectedNode.parentId != "0") {
                        jinkai.openSlide({
                            title: "新建字典",
                            url: "Form.html?TypeId=" + selectedNode.id,
                            width: 580,
                            callBack: function (name) {
                                window.frames[name].$.page.save();
                            }
                        });
                    }
                }
            });
            //编辑
            $("#btn_edit").on("click", function () {
                var right = jinkai.verifyRight($.page.module + "_Edit");
                if (right) {
                    var selectedNode = $("#treeView").treeview().getSelectedData();
                    jinkai.openSlide({
                        title: "编辑字典",
                        url: "Form.html?id=" + $("#gridTable").gridRowData().Id + '&TypeId=' + selectedNode.id,
                        width: 580,
                        callBack: function (name) {
                            window.frames[name].$.page.save();
                        }
                    });
                }
            });
            //删除
            $("#btn_delete").on("click", function () {
                var right = jinkai.verifyRight($.page.module + "_Delete");
                if (right) {
                    var id = $("#gridTable").gridRowData().Id;
                    jinkai.confirm({
                        content: "您确定要删除这些数据吗, 是否继续？",
                        callBack: function () {
                            jinkai.ajax({
                                async: false,
                                type: "POST",
                                url: jinkai.toUrl(requestUrlData + "?action=delete&id=" + id),
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
            //上移
            $("#btn_first").on("click", function () {
                var right = jinkai.verifyRight($.page.module + "_Edit");
                if (right) {
                    var id = $("#gridTable").gridRowData().Id;
                    var $element = $("#gridTable");
                    var rownumbers = $element.getGridParam('rownumbers');
                    if (!rownumbers) {
                        jinkai.msg("当前表格不支持上移", "error");
                        return;
                    }
                    var $tr = $element.find("#" + id);
                    var prev = $tr.prev();
                    if (prev.index() <= 0) {
                        jinkai.msg("已经是第一行了", "error");
                        return;
                    }
                    jinkai.ajax({
                        async: false,
                        type: "POST",
                        url: jinkai.toUrl(requestUrlData + "?action=first&id=" + id),
                        param: {},
                        success: function (result) {
                            $("#gridTable").gridMoveRow(id, "up");
                            jinkai.thisTab().jinkai.openSlideClose();
                        },
                        error: function (result) {
                            jinkai.msg(result.msg, "error");
                            window.parent.$("#gridTable").gridReloadSelection();
                            jinkai.thisTab().jinkai.openSlideClose();
                        },
                    })
                }
            });
            //下移
            $("#btn_next").on('click', function () {
                var right = jinkai.verifyRight($.page.module + "_Edit");
                if (right) {
                    var id = $("#gridTable").gridRowData().Id;
                    var $element = $("#gridTable");
                    var rownumbers = $element.getGridParam('rownumbers');
                    if (!rownumbers) {
                        jinkai.msg("当前表格不支持下移", "error");
                        return;
                    }
                    var $tr = $element.find("#" + id);
                    var next = $tr.next();
                    if (next.index() <= 0) {
                        jinkai.msg("已经是最后一行了", "error");
                        return;
                    }
                    jinkai.ajax({
                        async: false,
                        type: "POST",
                        url: jinkai.toUrl(requestUrlData + "?action=next&id=" + id),
                        param: {},
                        success: function (result) {
                            $("#gridTable").gridMoveRow(id, "down");
                            window.parent.$("#gridTable").gridReloadSelection();
                            jinkai.thisTab().jinkai.openSlideClose();
                        },
                        error: function (result) {
                            jinkai.msg(result.msg, "error");
                        },
                    })
                }
            });
        },
        tree: function () {
            $("#treeView").treeview({
                height: $(window).height() - 56,
                url: requestUrl + "?action=treeView&type=show",
                onnodeclick: function (item) {
                    $("#txt_keyword").val("");
                    //console.log(item);
                    //if (item.parentId != "0") {
                        $.page.grid(item.id);
                   // }
                    
                }
            }).setSelected({ id: $("#treeView").find("ul>li>ul>li:first").find("span.bbit-tree-node-text").attr("data-value") });
            //扩展分类【操作按钮】
            $("#treeView").find('div.bbit-tree-node-el').each(function () {
                var $btn = $('<div data-value="' + $(this).find("span.bbit-tree-node-text").attr("data-value") + '" class="tree_btn_right"><a class="dropdown-toggle" data-toggle="dropdown"><i class="fa fa-ellipsis-h btn_more" title="更多操作"></i></a><ul class="dropdown-menu fz-sm pull-right" style="display: none;"></ul><i authorize="yes" class="fa fa-plus btn_add" title="创建分类"></i></div>');
                //$btn.find("ul.dropdown-menu").append('<li><a authorize="yes" class="btn_edit">编辑</a><a authorize="yes" class="btn_delete">删除</a><a authorize="yes" class="btn_first">上移</a><a authorize="yes" class="btn_next">下移</a></li>')
                $btn.find("ul.dropdown-menu").append('<li><a authorize="yes" class="btn_edit">编辑</a><a authorize="yes" class="btn_delete">删除</a></li>');
                if ($(this).find("i").hasClass("fa fa-folder-open")) {
                    $btn.find("i.btn_more").remove();
                } else {
                    $btn.find("i.btn_add").remove();
                }
                $(this).append($btn).css({ position: 'relative' });
                $(this).hover(function () {
                    $(this).find("div.tree_btn_right").show();
                    $(this).find(".dropdown-menu").hide();
                }, function () {
                    $(this).find("div.tree_btn_right").hide();
                    $(this).find(".dropdown-menu").hide();
                });
                $(this).find("div.tree_btn_right").on("click", function (e) {
                    e.stopPropagation();
                    $(this).children("ul.dropdown-menu").show();
                });
                //新建分类
                $btn.find(".btn_add").on("click", function (e) {
                    var right = jinkai.verifyRight($.page.module + "_Add");
                    if (right) {
                        e.stopPropagation();
                        jinkai.openWindow({
                            title: "新建类别",
                            url: "/pages/dictionary/TypeForm.html",
                            width: "450px",
                            height: "400px",
                            callBack: function (name) {
                                _top.frames[_top.frames.length - 1].$.page.save();
                            }
                        });
                    }
                });
                //编辑分类
                $btn.find(".btn_edit").on("click", function () {
                    var right = jinkai.verifyRight($.page.module + "_Edit");
                    if (right) {
                        jinkai.openWindow({
                            title: "编辑类别",
                            url: "/pages/dictionary/TypeForm.html?id=" + $btn.attr("data-value"),
                            width: "450px",
                            height: "400px",
                            callBack: function (name) {
                                _top.frames[_top.frames.length - 1].$.page.save();
                            }
                        });
                    }
                });
                //删除分类
                $btn.find(".btn_delete").on("click", function () {
                    var right = jinkai.verifyRight($.page.module + "_Delete");
                    if (right) {
                        var allowDelete = $("#gridTable").jqGrid('getRowData').length + $("#treeView").getSelectedData().ChildNodes.length == 0 ? true : false;
                        if (allowDelete == false) {
                            jinkai.msg("此记录被关联引用,不允许被删除", "error");
                            return false;
                        }
                        jinkai.confirm({
                            content: "您确定要删除当前类别吗, 是否继续？",
                            callBack: function () {
                                jinkai.ajax({
                                    async: false,
                                    type: "POST",
                                    url: jinkai.toUrl(requestUrl + "?action=delete&id=" + $btn.attr("data-value")),
                                    param: {},
                                    success: function (result) {
                                        jinkai.msg("删除成功", "success");
                                        $("#gridTable").gridReload();
                                        jinkai.thisTab().location.reload();
                                        jinkai.openClose();
                                    },
                                    error: function (result) {
                                        layer.msg(result.msg, { icon: 2 });
                                    },
                                })
                            }
                        });
                    }
                });
                //上移分类
                $btn.find(".btn_first").on("click", function () {
                    var right = jinkai.verifyRight($.page.module + "_Edit");
                    if (right) {
                        var id = $btn.attr("data-value");
                        jinkai.ajax({
                            type: "POST",
                            url: jinkai.toUrl(requestUrl + "?action=first&id=" + id),
                            success: function (result) {
                                if (result.status == 200) {
                                    $("#treeView").treeMoveRow(id, "up");
                                }
                            }
                        });
                    }
                });
                //下移分类
                $btn.find(".btn_next").on("click", function () {
                    var right = jinkai.verifyRight($.page.module + "_Edit");
                    if (right) {
                        var id = $btn.attr("data-value");
                        jinkai.ajax({
                            type: "POST",
                            url: jinkai.toUrl(requestUrl + "?action=next&id=" + id),
                            success: function (result) {
                                if (result.status == 200) {
                                    $("#treeView").treeMoveRow(id, "down");
                                }
                            }
                        });
                    }
                });
            });
        },
        grid: function (TypeId) {
            if ($(".ui-jqgrid").length > 0) {
                $("#gridTable").jqGrid("setGridParam", { url: requestUrlData + "?action=list&TypeId=" + TypeId }).gridReload();
                return false;
            }
            $("#gridTable").grid({
                authorize: true,
                url: requestUrlData + "?action=list&TypeId=" + TypeId,
                height: $(window).height() - 142,
                treeGrid: true,
                ExpandColumn: "FullName",
                colModel: [
                    { label: "主键", name: "Id", hidden: true, key: true },
                    { label: "项目名称", name: "FullName", width: 200, align: "left", sortable: false },
                    { label: "项目代码", name: "EnCode", width: 200, align: "left", sortable: false },
                    { label: "项目拼音", name: "SimpleSpelling", width: 150, align: "left", sortable: false, autowidth: true },
                    {
                        label: "项目状态", name: "EnabledMark", width: 100, align: "center", sortable: false,
                        formatter: function (value) {
                            return value == 1 ? '<span class="switchery switchery-small-xs switchery-checked"><small></small></span>' : '<span class="switchery switchery-small-xs"><small></small></span>';
                        }
                    },
                    { label: "排列码", name: "SortCode", hidden: true },
                    { label: "类别Id", name: "TypeId", hidden: true },
                ]
            });
        },
        search: function () {
            $("#btn_search").click(function () {
                $("#gridTable tr:gt(0)").hide().filter(":contains('" + $("#txt_keyword").val() + "')").show();
            });
            $("#txt_keyword").keydown(function (e) {
                if (e.keyCode == 13) {
                    $("#btn_search").trigger("click");
                }
            });
        },
        resize: function () {
            $(window).resize(function () {
                $("#treeView").setTreeHeight($(window).height() - 56.5);
                $("#gridTable").setGridHeight($(window).height() - 142).setGridWidth($(".main-bd").width());
            });
        }
    }
    $(function () {
        jinkai.filterAuthorize($.page.module);
        $.page.init();
    });
})(jQuery);