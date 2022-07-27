(function ($) {
    $.clientData = {
        timeout: 600000,// 默认时间10分钟
        data: [],       //存储数据
        // 保存用户信息
        userInfo: function () {
            var data = _top.$.indexData.userProvider;
            return data;
        },
        // 保存系统设置信息
        //userSettings: function () {
        //    var data = _top.$.indexData.userSettings;
        //    return data;
        //},
        // 获得数据字典数据
        dictionaryData: function (sort, id) {
            var _itemData = $.clientData.data["dictionaryData"];
            if (_itemData == undefined || _itemData.overdue == true) {
                jinkai.ajax({
                    url: jinkai.toUrl("/ajax/dictionaryData.ashx?action=bindDictionary"),
                    success: function (result) {
                        _itemData = { data: result.data.items, overdue: false };
                        $.clientData.data["dictionaryData"] = _itemData;
                    }
                });
                setTimeout(function () {
                    _itemData.overdue = true;
                }, $.clientData.timeout);
            }
            var data = jinkai.jsonFind(_itemData.data, function (v) { return v.key == sort })[0];
            if (id == undefined) {
                var json = [];
                $.each(data.value, function (i, item) {
                    var rowItem = {};
                    rowItem["name"] = item.text;
                    rowItem["text"] = item.text;
                    rowItem["code"] = item.code;
                    if (data.isTree == 1) {
                        rowItem["id"] = item.id;
                        rowItem["parentId"] = item.parentId;
                    } else {
                        rowItem["id"] = item.id;
                    }
                    json.push(rowItem);
                });
                return data.isTree == 0 ? json : jinkai.toTreeViewJson(json, 0);
            } else {
                var rowData = [];
                if (data.isTree == 1) {
                    rowData = jinkai.jsonFind(data.value, function (v) { return v.id == id });
                } else {
                    rowData = jinkai.jsonFind(data.value, function (v) { return v.id == id });
                }
                if (rowData.length > 0) {
                    return rowData[0];
                } else {
                    return { id: "", name: "", text :"" };
                }
            }
        },
        // 获得部门树形结构数据
        deptData: function () {
            var _itemData = $.clientData.data["deptData"];
            
            if (_itemData == undefined || _itemData.overdue == true) {
                jinkai.ajax({
                    url: jinkai.toUrl("/ajax/dept.ashx?action=treeView"),
                    success: function (result) {
                        var _data = result.data.items;
                        _itemData = { data: _data, overdue: false };
                        $.clientData.data["deptData"] = _itemData;
                    }
                });
                setTimeout(function () {
                    _itemData.overdue = true;
                }, $.clientData.timeout);
            }
            var data = _itemData.data;
            return data;
        },
    }

})(jQuery);