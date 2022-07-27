(function ($) {
    $.fn.treeview = function (options) {
        if (options == undefined) {
            return $(this);
        } else {
            jinkai.loadStyle("/assets/wdtree/wdtree.css");
            var dfop = {
                method: "GET",
                datatype: "json",
                url: false,
                param: null,
                cbiconpath: "/assets/wdtree/images/icons/",
                icons: ["checkbox_0.png", "checkbox_1.png", "checkbox_2.png"],
                showcheck: false,
                oncheckboxclick: false,
                onnodeclick: false,
                cascadecheck: true,
                data: null,
                clicktoggle: true,
                theme: "bbit-tree-arrows",
                isTool: false,
                nodeTools: []
            };
            $.extend(dfop, options);
            var treenodes = dfop.data;
            var me = $(this);
            var id = me.attr("id");
            if (id == null || id == "") {
                id = "bbtree" + new Date().getTime();
                me.attr("id", id);
            }
            if (dfop.height != undefined) {
                me.height(dfop.height);
            }
            if (dfop.slimscroll == true) {
                me.css({ "overflow": "auto", "overflow-y": "hidden" });
            } else {
                me.css({ "overflow": "auto" });
            }
            var html = [];
            buildtree(dfop.data, html);
            me.html('');
            me.addClass("bbit-tree").append(html.join(""));
            InitEvent(me);
            html = null;
            if (dfop.showcheck) {
                for (var i = 0; i < 3; i++) {
                    var im = new Image();
                    im.src = dfop.cbiconpath + dfop.icons[i];
                }
            }
            function buildtree(data, ht) {
                ht.push("<div class='bbit-tree-bwrap'>");
                ht.push("<div class='bbit-tree-body " + id + "'>");
                ht.push("<ul class='bbit-tree-root ", dfop.theme, "'>");
                if (data && data.length > 0) {
                    var l = data.length;
                    for (var i = 0; i < l; i++) {
                        buildnode(data[i], ht, 0, i, i == l - 1);
                    }
                }
                else {
                    asnyloadc(null, false, function (data) {
                        if (data && data.length > 0) {
                            treenodes = data;
                            dfop.data = data;
                            if (dfop.description) {
                                data.unshift({
                                    "id": "",
                                    "text": dfop.description,
                                    "value": "",
                                    "img": "-1",
                                    "parentnodes": "0",
                                    "showcheck": false,
                                    "isexpand": true,
                                    "complete": true,
                                    "hasChildren": false,
                                    "ChildNodes": []
                                });
                            }
                            var l = data.length;
                            for (var i = 0; i < l; i++) {
                                buildnode(data[i], ht, 0, i, i == l - 1);
                            }
                        }
                    });
                }
                ht.push("</ul>");
                ht.push("</div>");
                ht.push("</div>");
            }
            function buildnode(nd, ht, deep, path, isend) {
                var nid = nd.id.replace(/[^\w]/gi, "_");
                ht.push("<li class='bbit-tree-node " + (nd.cssClass == undefined ? "" : nd.cssClass) + "'>");
                var title = nd.title;
                if (title) {
                    title = nd.title;
                } else {
                    title = nd.text;
                }
                ht.push("<div id='", id, "_", nid, "' tpath='", path, "' unselectable='on' title='", title, "'");
                var cs = [];
                cs.push("bbit-tree-node-el");
                if (nd.hasChildren) {
                    cs.push(nd.isexpand ? "bbit-tree-node-expanded" : "bbit-tree-node-collapsed");
                }
                else {
                    cs.push("bbit-tree-node-leaf");
                }
                if (nd.classes) { cs.push(nd.classes); }

                ht.push(" class='", cs.join(" "), "'>");
                ht.push("<span class='bbit-tree-node-indent'>");
                if (deep == 1) {
                    ht.push("<img class='bbit-tree-icon' src='" + dfop.cbiconpath + "s.gif'/>");
                }
                else if (deep > 1) {
                    ht.push("<img class='bbit-tree-icon' src='" + dfop.cbiconpath + "s.gif'/>");
                    for (var j = 1; j < deep; j++) {
                        ht.push("<img class='bbit-tree-elbow-line' src='" + dfop.cbiconpath + "s.gif'/>");
                    }
                }
                ht.push("</span>");
                cs.length = 0;
                if (nd.hasChildren) {
                    if (nd.isexpand) {
                        cs.push(isend ? "bbit-tree-elbow-end-minus" : "bbit-tree-elbow-minus");
                    }
                    else {
                        cs.push(isend ? "bbit-tree-elbow-end-plus" : "bbit-tree-elbow-plus");
                    }
                }
                else {
                    cs.push(isend ? "bbit-tree-elbow-end" : "bbit-tree-elbow");
                }
                ht.push("<img class='bbit-tree-ec-icon ", cs.join(" "), "' src='" + dfop.cbiconpath + "s.gif'/>");
                if (dfop.showcheck && nd.showcheck) {
                    var _checkstate = nd.checkstate == null ? 0 : nd.checkstate;
                    var childNodes = nd.ChildNodes;
                    if (jinkai.jsonFind(childNodes, function (v) { return v.checkstate == 1 }).length > 0) {
                        _checkstate = 2;
                    }
                    if (_checkstate == 2 && childNodes.length == jinkai.jsonFind(childNodes, function (v) { return v.checkstate == 1 }).length) {
                        _checkstate = 1;
                    }
                    ht.push("<img  id='", id, "_", nid, "_cb' class='bbit-tree-node-cb' src='", dfop.cbiconpath, dfop.icons[_checkstate], "'/>");
                }

                if (nd.hasChildren) {
                    if (nd.img == -1) {
                        ht.push("");
                    } else
                        if (!!nd.img) {
                            ht.push("<i class=\"" + nd.img + "\"></i>");
                        } else {
                            ht.push("<i class=\"fa fa-folder-open\" style='width:15px'></i>");
                        }
                } else {
                    if (nd.img == -1) {
                        ht.push("");
                    } else
                        if (!!nd.img) {
                            ht.push("<i class=\"" + nd.img + "\"></i>");
                        } else if (nd.img == "false") {
                            ht.push("");
                        } else {
                            ht.push("<i class=\"fa fa-file-text-o\"></i>");
                        }
                }
                ht.push("<a hideFocus class='bbit-tree-node-anchor' tabIndex=1>");
                ht.push("<span data-value='" + nd.id + "' class='bbit-tree-node-text' unselectable='on'>", nd.text, "</span>");
                ht.push("</a>");

                if (dfop.isTool) {
                    ht.push("<div class='bbit-tree-node-tool'>");
                    for (var ii in dfop.nodeTools) {
                        var toolItem = dfop.nodeTools[ii];
                        ht.push("<span class='" + toolItem.img + "' title='" + toolItem.text + "'></span>");
                    }
                    ht.push("</div>");
                }
                ht.push("</div>");
                if (nd.hasChildren) {
                    if (nd.isexpand) {
                        ht.push("<ul  class='bbit-tree-node-ct'  style='z-index: 0; position: static; visibility: visible; top: auto; left: auto;'>");
                        if (nd.ChildNodes) {
                            var l = nd.ChildNodes.length;
                            for (var k = 0; k < l; k++) {
                                nd.ChildNodes[k].parent = nd;
                                buildnode(nd.ChildNodes[k], ht, deep + 1, path + "." + k, k == l - 1);
                            }
                        }
                        ht.push("</ul>");
                    } else {
                        ht.push("<ul style='display:none;'>");
                        if (nd.ChildNodes) {
                            var l = nd.ChildNodes.length;
                            for (var k = 0; k < l; k++) {
                                nd.ChildNodes[k].parent = nd;
                                buildnode(nd.ChildNodes[k], ht, deep + 1, path + "." + k, k == l - 1);
                            }
                        }
                        ht.push("</ul>");
                    }
                }
                ht.push("</li>");
                nd.render = true;
            }
            function getItem(path) {
                var ap = path.split(".");
                var t = treenodes;
                for (var i = 0; i < ap.length; i++) {
                    if (i == 0) {
                        t = t[ap[i]];
                    }
                    else {
                        t = t.ChildNodes[ap[i]];
                    }
                }
                return t;
            }
            function check(item, state, type) {
                var pstate = item.checkstate;
                if (type == 1) {
                    item.checkstate = state;
                }
                else {
                    var cs = item.ChildNodes;
                    var l = cs.length;
                    var ch = true;
                    for (var i = 0; i < l; i++) {
                        if ((state == 1 && cs[i].checkstate != 1) || state == 0 && cs[i].checkstate != 0) {
                            ch = false;
                            break;
                        }
                    }
                    if (ch) {
                        item.checkstate = state;
                    }
                    else {
                        item.checkstate = 2;
                    }
                }
                if (item.render && pstate != item.checkstate) {
                    var nid = item.id.replace(/[^\w]/gi, "_");
                    var et = $("#" + id + "_" + nid + "_cb");
                    if (et.length == 1) {
                        et.attr("src", dfop.cbiconpath + dfop.icons[item.checkstate]);
                    }
                }
            }
            function cascade(fn, item, args) {
                if (fn(item, args, 1) != false) {
                    if (item.ChildNodes != null && item.ChildNodes.length > 0) {
                        var cs = item.ChildNodes;
                        for (var i = 0, len = cs.length; i < len; i++) {
                            cascade(fn, cs[i], args);
                        }
                    }
                }
            }
            function bubble(fn, item, args) {
                var p = item.parent;
                while (p) {
                    if (fn(p, args, 0) === false) {
                        break;
                    }
                    p = p.parent;
                }
            }
            function nodeclick(e) {
                var path = $(this).attr("tpath");
                var et = e.target || e.srcElement;
                var item = getItem(path);
                if (et.tagName == "IMG") {
                    if ($(et).hasClass("bbit-tree-elbow-plus") || $(et).hasClass("bbit-tree-elbow-end-plus")) {
                        if ($(this).find('i').hasClass('fa-folder')) {
                            $(this).find('i').swapClass('fa-folder', 'fa-folder-open');
                        }
                        var ul = $(this).next();
                        if (ul.hasClass("bbit-tree-node-ct")) {
                            ul.slideDown(200);
                        }
                        else {
                            var deep = path.split(".").length;
                            if (item.complete) {
                                item.ChildNodes != null && asnybuild(item.ChildNodes, deep, path, ul, item);
                            }
                            else {
                                $(this).addClass("bbit-tree-node-loading");
                                asnyloadc(item, true, function (data) {
                                    item.complete = true;
                                    item.ChildNodes = data;
                                    asnybuild(data, deep, path, ul, item);
                                });
                            }
                        }
                        if ($(et).hasClass("bbit-tree-elbow-plus")) {
                            $(et).swapClass("bbit-tree-elbow-plus", "bbit-tree-elbow-minus");
                        }
                        else {
                            $(et).swapClass("bbit-tree-elbow-end-plus", "bbit-tree-elbow-end-minus");
                        }
                        $(this).swapClass("bbit-tree-node-collapsed", "bbit-tree-node-expanded");
                    }
                    else if ($(et).hasClass("bbit-tree-elbow-minus") || $(et).hasClass("bbit-tree-elbow-end-minus")) {
                        if ($(this).find('i').hasClass('fa-folder-open')) {
                            $(this).find('i').swapClass('fa-folder-open', 'fa-folder');
                        }
                        $(this).next().slideUp(200);
                        if ($(et).hasClass("bbit-tree-elbow-minus")) {
                            $(et).swapClass("bbit-tree-elbow-minus", "bbit-tree-elbow-plus");
                        }
                        else {
                            $(et).swapClass("bbit-tree-elbow-end-minus", "bbit-tree-elbow-end-plus");
                        }
                        $(this).swapClass("bbit-tree-node-expanded", "bbit-tree-node-collapsed");
                    }
                    else if ($(et).hasClass("bbit-tree-node-cb")) {
                        var s = item.checkstate != 1 ? 1 : 0;
                        var r = true;
                        if (dfop.oncheckboxclick) {
                            r = dfop.oncheckboxclick.call(et, item, s);
                        }
                        if (r != false) {
                            if (dfop.cascadecheck) {
                                cascade(check, item, s);
                                bubble(check, item, s);
                            }
                            else {
                                check(item, s, 1);
                            }
                        }
                    }
                }
                else {
                    if (dfop.citem) {
                        var nid = dfop.citem.id.replace(/[^\w]/gi, "_");
                        $("." + id).removeClass("bbit-tree-selected");
                    }
                    dfop.citem = item;
                    $("." + id).find('div').removeClass("bbit-tree-selected");
                    $(this).addClass("bbit-tree-selected");
                    if (dfop.onnodeclick) {
                        if (item != undefined) {
                            if (!item.expand) {
                                item.expand = function () { expandnode.call(item); };
                            }
                            dfop.onnodeclick.call(this, item);
                        }
                    }
                    if (dfop.showcheck && item.showcheck) {
                        var _checked = 0;
                        if (!$(this).find("img.bbit-tree-node-cb").hasClass("checked")) {
                            $(this).find("img.bbit-tree-node-cb").addClass("checked");
                            _checked = 1;
                        } else {
                            $(this).find("img.bbit-tree-node-cb").removeClass("checked");
                        }
                        cascade(check, item, _checked);
                        bubble(check, item, _checked);
                    }
                }
            }
            function expandnode() {
                var item = this;
                var nid = item.id.replace(/[^\w]/gi, "_");
                var img = $("#" + id + "_" + nid + " img.bbit-tree-ec-icon");
                if (img.length > 0) {
                    img.click();
                }
            }
            function asnybuild(nodes, deep, path, ul, pnode) {
                var l = nodes.length;
                //if (l > 0 && pnode.complete == false) {
                if (l > 0) {
                    var ht = [];
                    for (var i = 0; i < l; i++) {
                        nodes[i].parent = pnode;
                        buildnode(nodes[i], ht, deep, path + "." + i, i == l - 1);
                    }
                    ul.html(ht.join(""));
                    ht = null;
                    InitEvent(ul);
                }
                ul.addClass("bbit-tree-node-ct").css({ "z-index": 0, position: "static", visibility: "visible", top: "auto", left: "auto", display: "" });
                ul.prev().removeClass("bbit-tree-node-loading");
            }
            function asnyloadc(pnode, isAsync, callback) {
                if (dfop.url) {
                    if (dfop.param != null) {
                        var param = dfop.param
                    }
                    if (pnode && pnode != null) {
                        var param = builparam(pnode);
                    }
                    $.ajax({  //todo
                         type: dfop.method,
                         url: dfop.url,
                         data: param,
                         async: isAsync,
                         dataType: dfop.datatype,
                         //headers: { Authorize: jinkai.getAuthorize() },
                         success: function (result) {
                             if (result.status == 200) {
                                 callback(result.data.items != undefined ? result.data.items: result.data)
                             } else {
                                 jinkai.ajaxError(result);
                             }
                         },
                         error: function (XMLHttpRequest) {
                             jinkai.ajaxError(XMLHttpRequest);
                         }
                    });
                }
            }
            function builparam(node) {
                var p = [{ name: "id", value: encodeURIComponent(node.id) }
                    , { name: "text", value: encodeURIComponent(node.text) }
                    , { name: "value", value: encodeURIComponent(node.value) }
                    , { name: "checkstate", value: node.checkstate }];
                return p;
            }
            function bindevent() {
                $(this).hover(function () {
                    $(this).addClass("bbit-tree-node-over");
                }, function () {
                    $(this).removeClass("bbit-tree-node-over");
                }).click(nodeclick)
                    .find("img.bbit-tree-ec-icon").each(function (e) {
                        if (!$(this).hasClass("bbit-tree-elbow")) {
                            $(this).hover(function () {
                                $(this).parent().addClass("bbit-tree-ec-over");
                            }, function () {
                                $(this).parent().removeClass("bbit-tree-ec-over");
                            });
                        }
                    });
            }
            function InitEvent(parent) {
                var nodes = $("li.bbit-tree-node>div", parent);
                nodes.each(bindevent);
            }
            function refresh(itemId) {
                var nid = itemId.replace(/[^\w-]/gi, "_");
                var node = $("#" + id + "_" + nid);
                if (node.length > 0) {
                    node.addClass("bbit-tree-node-loading");
                    var isend = node.hasClass("bbit-tree-elbow-end") || node.hasClass("bbit-tree-elbow-end-plus") || node.hasClass("bbit-tree-elbow-end-minus");
                    var path = node.attr("tpath");
                    var deep = path.split(".").length;
                    var item = getItem(path);
                    if (item) {
                        asnyloadc(item, true, function (data) {
                            item.complete = true;
                            item.ChildNodes = data;
                            item.isexpand = true;
                            if (data && data.length > 0) {
                                item.hasChildren = true;
                            }
                            else {
                                item.hasChildren = false;
                            }
                            var ht = [];
                            buildnode(item, ht, deep - 1, path, isend);
                            ht.shift();
                            ht.pop();
                            var li = node.parent();
                            li.html(ht.join(""));
                            ht = null;
                            InitEvent(li);
                            bindevent.call(li.find(">div"));
                        });
                    }
                }
            }
            function getck(items, c, fn) {
                for (var i = 0, l = items.length; i < l; i++) {
                    (items[i].showcheck == true && items[i].checkstate == 1) && c.push(fn(items[i]));
                    if (items[i].ChildNodes != null && items[i].ChildNodes.length > 0) {
                        getck(items[i].ChildNodes, c, fn);
                    }
                }
            }
            function getCkAndHalfCk(items, c, fn) {
                for (var i = 0, l = items.length; i < l; i++) {
                    (items[i].showcheck == true && (items[i].checkstate == 1 || items[i].checkstate == 2)) && c.push(fn(items[i]));
                    if (items[i].ChildNodes != null && items[i].ChildNodes.length > 0) {
                        getCkAndHalfCk(items[i].ChildNodes, c, fn);
                    }
                }
            }
            me[0].t = {
                getSelectedNodes: function (gethalfchecknode) {
                    var s = [];
                    if (gethalfchecknode) {
                        getCkAndHalfCk(treenodes, s, function (item) { return item; });
                    }
                    else {
                        getck(treenodes, s, function (item) { return item; });
                    }
                    return s;
                },
                getSelectedValues: function () {
                    var s = [];
                    getck(treenodes, s, function (item) { return item.value; });
                    return s;
                },
                getCurrentItem: function () {
                    return dfop.citem;
                },
                getData: function () {
                    return dfop.data;
                },
                setSelected: function (element, item) {
                    element.find('div').removeClass("bbit-tree-selected");
                    if (item.id != undefined) {
                        element.find("#" + id + "_" + item.id.replace(/-/g, "_")).addClass("bbit-tree-selected");
                        element.find("#" + id + "_" + item.id.replace(/-/g, "_")).trigger("click");
                    } else {
                        if ($.isArray(item)) {
                            for (var i = 0; i < item.length; i++) {
                                element.find("#" + id + "_" + item[i].replace(/-/g, "_")).addClass("bbit-tree-selected");
                            }
                        } else {
                            element.find("#" + id + "_" + item.replace(/-/g, "_")).addClass("bbit-tree-selected");
                        }
                    }
                },
                setCheck: function (element, item) {
                    if (item.id != undefined) {
                        cascade(check, item, 1);
                        bubble(check, item, 1);
                    } else {
                        var tpath = element.find("#" + id + "_" + item.replace(/-/g, "_")).attr("tpath");
                        cascade(check, getItem(tpath), 1);
                        bubble(check, getItem(tpath), 1);
                    }
                },
                checkAll: function (element, checkstate) {
                    $.each(dfop.data, function (i, item) {
                        //改变状态
                        item.checkstate = checkstate;
                        //改变图标
                        var nid = item.id.replace(/[^\w]/gi, "_");
                        var et = $("#" + id + "_" + nid + "_cb");
                        if (et.length == 1) {
                            et.attr("src", dfop.cbiconpath + dfop.icons[item.checkstate]);
                        }
                        check(item, checkstate)
                    });
                    function check(item, checkstate) {
                        if (item.ChildNodes != null && item.ChildNodes.length > 0) {
                            $.each(item.ChildNodes, function (i, subItem) {
                                //改变状态
                                subItem.checkstate = checkstate;
                                //改变图标
                                var nid = subItem.id.replace(/[^\w]/gi, "_");
                                var et = $("#" + id + "_" + nid + "_cb");
                                if (et.length == 1) {
                                    et.attr("src", dfop.cbiconpath + dfop.icons[subItem.checkstate]);
                                }
                                check(subItem, checkstate);
                            });
                        }
                    }
                },
                refresh: function (itemOrItemId) {
                    var id;
                    if (typeof (itemOrItemId) == "string") {
                        id = itemOrItemId;
                    }
                    else {
                        id = itemOrItemId.id;
                    }
                    refresh(id);
                },
            };
            return me;
        }
    };
    $.fn.swapClass = function (c1, c2) {
        return this.removeClass(c1).addClass(c2);
    };
    $.fn.refresh = function (item) {
        if (this[0].t) {
            return this[0].t.refresh(item);
        }
    };
    $.fn.getCheckData = function (gethalfchecknode) {
        if (this[0].t) {
            var _gethalfchecknode = gethalfchecknode == undefined ? true : false;
            return this[0].t.getSelectedNodes(_gethalfchecknode);
        }
        return null;
    };
    $.fn.getCheckId = function (gethalfchecknode) {
        if (this[0].t) {
            var _id = [];
            var _gethalfchecknode = gethalfchecknode == undefined ? true : false;
            var _data = this[0].t.getSelectedNodes(_gethalfchecknode);
            for (var i = 0; i < _data.length; i++) {
                _id.push(_data[i].id);
            }
            return _id;
        }
        return null;
    };
    $.fn.getData = function () {
        if (this[0].t) {
            return this[0].t.getData();
        }
        return null;
    };
    $.fn.getSelectedId = function () {
        if (this[0].t) {
            if (this[0].t.getCurrentItem() != undefined) {
                return this[0].t.getCurrentItem().id;
            } else {
                return "";
            }
        }
        return null;
    };
    $.fn.getSelectedData = function () {
        if (this[0].t) {
            return this[0].t.getCurrentItem();
        }
        return null;
    };
    $.fn.getChildNodeIds = function () {
        var nodeData = this.getSelectedData();
        if (nodeData != undefined) {
            var ids = [nodeData.id];
            if (nodeData.ChildNodes.length > 0) {
                getNode(nodeData.ChildNodes);
            }
            function getNode(data) {
                $.each(data, function (i, item) {
                    ids.push(item.id);
                    if (item.ChildNodes.length > 0) {
                        getNode(item.ChildNodes);
                    }
                });
            }
            return ids;
        }
        return null;
    };
    $.fn.setSelected = function (item) {
        var $element = $(this);
        return this[0].t.setSelected($element, item);
    }
    $.fn.setCheck = function (item) {
        var $element = $(this);
        return this[0].t.setCheck($element, item);
    }
    $.fn.checkAll = function (checkstate) {
        var $element = $(this);
        return this[0].t.checkAll($element, checkstate);
    }
    $.fn.removeNode = function (item) {
        if (item == undefined) {
            return false;
        }
        var $element = $(this);
        $element.find("#" + $element.attr("id") + "_" + item.replace(/-/g, "_")).parent("li.bbit-tree-node").remove();
    }
    $.fn.setTreeHeight = function (nh) {
        var $element = $(this);
        $element.height(nh);
    }
})(jQuery);