var _top = top.frames["Index"] == undefined ? window : top.frames["Index"];
//var _top = top.frames["Index"] == undefined ? parent : top.frames["Index"];
(function ($) {
    $(function () {
        if ($("[data-toggle='tooltip']").length > 0) {
            $("[data-toggle='tooltip']").tooltip();
        }
    });
    jinkai = {
        rootPath: "",
        apiUrl: "",
        ajax: function (options) {
            try {
                var defaults = {
                    type: "GET",
                    url: "",
                    param: [],
                    async: false,
                    dataType: "json",
                    headers: [],
                    contentType: "application/x-www-form-urlencoded",
                    beforeSend: null,
                    success: null,
                    complete: null,
                    error: null
                };
                var options = $.extend(defaults, options);
                $.ajax({
                    type: options.type,
                    url: options.url,
                    data: options.param,
                    async: options.async,
                    dataType: options.dataType,
                    headers: options.headers,
                    contentType: options.contentType,
                    success: function (result) {
                        if (options.success != null) {
                            if (result.status == 200) {
                                //成功回调
                                options.success(result);
                            } else if (result.status == 0) {
                                //身份验证错误
                                top.location.href = "/Login.html";
                            } else {
                                if (options.error != null) { options.error(result); } else { jinkai.ajaxError(result); }
                            }
                        }
                    },
                    beforeSend: function (XMLHttpRequest) {
                        //XMLHttpRequest.setRequestHeader("Authorize", jinkai.getAuthorize());
                        if (options.beforeSend != null) {
                            options.beforeSend(XMLHttpRequest);
                        }
                    },
                    complete: function () {
                        //请求完成的处理
                        if (options.complete != null) {
                            options.complete();
                        }
                    },
                    error: function (XMLHttpRequest) {
                        //请求出错处理
                        jinkai.ajaxError(XMLHttpRequest);
                        if (options.error != null) {
                            options.error(XMLHttpRequest);
                        }
                    }
                });
            } catch (e) {
            }
        },
        ajaxError: function (data) {
            if (data.status == 0 || data.status == 404) {
                jinkai.msg("404错误,找不到请求地址", "error");
                return false;
            }
            if (data.status == 500) {
                jinkai.msg("内部异常，请查看日志信息", "error");
                return false;
            }
            //-- by liu 
            if (data.status == -1) {
                jinkai.msg(data.msg, "error");
                return false;
            }
            //--------
            switch (data.status) {
                case 400:	//错误
                    jinkai.msg(data.msg, "error");
                    break;
                case 500:	//异常
                    console.log(data);
                    jinkai.msg(data.msg, "error");
                    break;
                case 600:	//登录过期,请重新登录
                    jinkai.cookie("error_message", data.msg, { path: "/" })
                    top.isLogOut = true;
                    top.location.href = "/Login.html";
                    localStorage.removeItem("token");
                    break;
                case 601: 	//您的帐号在其他地方已登录,被强制踢出
                    jinkai.cookie("error_message", data.msg, { path: "/" })
                    top.isLogOut = true;
                    top.location.href = "/Login.html";
                    localStorage.removeItem("token");
                    break;
                case 602: 	//Token验证失败
                    top.isLogOut = true;
                    top.location.href = "/Login.html";
                    localStorage.removeItem("token");
                    break;
                default:
                    jinkai.msg(data.msg, "error");
                    break;
            }
        },
        //数值显示格式转化
        numToCurrency: function (val, dec) {
            val = parseFloat(val);
            dec = dec || 2;	//小数位
            if (val === 0 || isNaN(val)) {
                return '';
            }
            val = val.toFixed(dec).split('.');
            var reg = /(\d{1,3})(?=(\d{3})+(?:$|\D))/g;
            return val[0].replace(reg, "$1,") + '.' + val[1];
        },

        // 转换成html格式字符串
        escape: function (str) {
            str = str.replace(/&/g, '&amp;')
            str = str.replace(/</g, '&lt;')
            str = str.replace(/>/g, '&gt;')
            str = str.replace(/"/g, '&quto;')
            str = str.replace(/'/g, '&#39;')
            str = str.replace(/`/g, '&#96;')
            str = str.replace(/\//g, '&#x2F;')
            return str
        },
        /** 
         1. 设置cookie的值，把name变量的值设为value   
        example $.cookie(’name’, ‘value’);
         2.新建一个cookie 包括有效期 路径 域名等
        example $.cookie(’name’, ‘value’, {expires: 7, path: ‘/’, domain: ‘jquery.com’, secure: true});
        3.新建cookie
        example $.cookie(’name’, ‘value’);
        4.删除一个cookie
        example $.cookie(’name’, null);
        5.取一个cookie(name)值给myvar
        var account= $.cookie('name');
        **/
        cookie: function (name, value, options) {
            if (typeof value != 'undefined') { // name and value given, set cookie
                options = options || {};
                if (value === null) {
                    value = '';
                    options.expires = -1;
                }
                var expires = '';
                if (options.expires && (typeof options.expires == 'number' || options.expires.toUTCString)) {
                    var date;
                    if (typeof options.expires == 'number') {
                        date = new Date();
                        date.setTime(date.getTime() + (options.expires * 24 * 60 * 60 * 1000));
                    } else {
                        date = options.expires;
                    }
                    expires = '; expires=' + date.toUTCString(); // use expires attribute, max-age is not supported by IE
                }
                var path = options.path ? '; path=' + options.path : '';
                var domain = options.domain ? '; domain=' + options.domain : '';
                var secure = options.secure ? '; secure' : '';
                document.cookie = [name, '=', encodeURIComponent(value), expires, path, domain, secure].join('');
            } else { // only name given, get cookie
                var cookieValue = null;
                if (document.cookie && document.cookie != '') {
                    var cookies = document.cookie.split(';');
                    for (var i = 0; i < cookies.length; i++) {
                        var cookie = jQuery.trim(cookies[i]);
                        // Does this cookie string begin with the name we want?
                        if (cookie.substring(0, name.length + 1) == (name + '=')) {
                            cookieValue = decodeURIComponent(cookie.substring(name.length + 1));
                            break;
                        }
                    }
                }
                return cookieValue;
            }
        },
        //--------------------------------------------------------
        getData: function () {
            if (_top.$.clientData == undefined) {
                return window.$.clientData;
            } else {
                return _top.$.clientData;
            }
        },
        toUrl: function (url) {
            return jinkai.apiUrl + url;
        },
        vueInit: function (options) {
            var defaults = {};
            var options = $.extend(defaults, options);
            Vue.directive('select', {
                twoWay: true,
                bind: function () {
                    var self = this;
                    $(this.el).change(function () {
                        var value = $(this).val();
                        if ($(this).attr("multiple") == "multiple") {
                            self.set(String(value));
                        } else {
                            self.set(value);
                        }
                    });
                },
                update: function (value) {
                    if (value) {
                        if ($(this.el).attr("multiple") == "multiple") {
                            $(this.el).val(value.split(",")).trigger('change');
                        } else {
                            $(this.el).val(value).trigger('change');
                        }
                    }
                }
            });
            Vue.directive('switch', {
                switchElement: null,
                twoWay: true,
                bind: function () {
                    var self = this;
                    var $element = $(this.el)[0];
                    switchElement = new Switchery($element, {
                        className: 'switchery switchery-small',
                        color: '#188ae2',
                        secondaryColor: '#dfdfdf'
                    });
                    $element.onchange = function () {
                        self.set($element.checked == true ? 1 : 0);
                    };
                },
                update: function (value) {
                    if (value) {
                        var $element = $(this.el);
                        if (($element[0].checked == true ? 1 : 0) != value) {
                            $element.trigger("click");
                        }
                    }
                }
            });
            return new Vue(options);
        },
        vueSet: function (obj, key, value) {
            Vue.set(obj, key, value);
        },
        loadStyle: function (url) {
            var link = document.createElement("link");
            link.href = url;
            link.rel = "stylesheet";
            link.type = "text/css";
            document.getElementsByTagName("head")[0].appendChild(link);
        },
        thisTab: function (name) {
            if (name != undefined) {
                return _top.frames[name];
            } else {
                return _top.frames[_top.$(".app-main-iframe:visible").attr("id")];
            }
        },
        openTab: function (options) {
            _top.app.mainIndex.tabAdd(options);
        },
        openTabClose: function (name) {
            _top.app.mainIndex.tabClose();
        },
        getGuid: function () {
            var guid = "";
            for (var i = 1; i <= 32; i++) {
                var n = Math.floor(Math.random() * 16.0).toString(16);
                guid += n;
                if ((i == 8) || (i == 12) || (i == 16) || (i == 20))
                    guid += "-";
            }
            return guid;
        },
        reload: function () {
            location.reload();
            return false;
        },
        getNumber: function () {
            var num = "";
            for (var i = 0; i < 6; i++) {
                num += Math.floor(Math.random() * 10);
            }
            var code = jinkai.toDate(new Date(), "yyyyMMddhhmmss");
            return code + num;
        },
        getDate: function (format, strInterval, number) {
            var myDate = new Date();
            var dtTmp = new Date();
            if (!!strInterval) {
                switch (strInterval) {
                    case 's':
                        myDate = new Date(Date.parse(dtTmp) + (1000 * number));// 秒
                        break;
                    case 'n':
                        myDate = new Date(Date.parse(dtTmp) + (60000 * number));// 分
                        break;
                    case 'h':
                        myDate = new Date(Date.parse(dtTmp) + (3600000 * number));// 小时
                        break;
                    case 'd':
                        myDate = new Date(Date.parse(dtTmp) + (86400000 * number));// 天
                        break;
                    case 'w':
                        myDate = new Date(Date.parse(dtTmp) + ((86400000 * 7) * number));// 星期
                        break;
                    case 'q':
                        myDate = new Date(dtTmp.getFullYear(), (dtTmp.getMonth()) + number * 3, dtTmp.getDate(), dtTmp.getHours(), dtTmp.getMinutes(), dtTmp.getSeconds());// 季度
                        break;
                    case 'm':
                        myDate = new Date(dtTmp.getFullYear(), (dtTmp.getMonth()) + number, dtTmp.getDate(), dtTmp.getHours(), dtTmp.getMinutes(), dtTmp.getSeconds());// 月
                        break;
                    case 'y':
                        myDate = new Date((dtTmp.getFullYear() + number), dtTmp.getMonth(), dtTmp.getDate(), dtTmp.getHours(), dtTmp.getMinutes(), dtTmp.getSeconds());// 年
                        break;
                    default:
                }
            }
            return jinkai.toDate(myDate, format);
        },
        toDate: function (v, format) {
            if (!v || !format) return "";
            var d = v;
            if (typeof v === 'string') {
                if (v.indexOf("/Date(") > -1)
                    d = new Date(parseInt(v.replace("/Date(", "").replace(")/", ""), 10));
                else
                    d = new Date(Date.parse(v.replace(/-/g, "/").replace("T", " ").split(".")[0]));
            } else {
                d = new Date(v)
            }
            var o = {
                "M+": d.getMonth() + 1,
                "d+": d.getDate(),
                "h+": d.getHours(),
                "H+": d.getHours(),
                "m+": d.getMinutes(),
                "s+": d.getSeconds(),
                "q+": Math.floor((d.getMonth() + 3) / 3),
                "S": d.getMilliseconds()
            };
            if (/(y+)/.test(format)) {
                format = format.replace(RegExp.$1, (d.getFullYear() + "").substr(4 - RegExp.$1.length));
            }
            for (var k in o) {
                if (new RegExp("(" + k + ")").test(format)) {
                    format = format.replace(RegExp.$1, RegExp.$1.length == 1 ? o[k] : ("00" + o[k]).substr(("" + o[k]).length));
                }
            }
            return format;
        },
        toDecimal: function (num) {
            if (num == null) {
                num = "0";
            }
            num = num.toString().replace(/\$|\,/g, '');
            if (isNaN(num))
                num = "0";
            var sign = (num == (num = Math.abs(num)));
            num = Math.floor(num * 100 + 0.50000000001);
            var cents = num % 100;
            num = Math.floor(num / 100).toString();
            if (cents < 10)
                cents = "0" + cents;
            for (var i = 0; i < Math.floor((num.length - (1 + i)) / 3) ; i++)
                num = num.substring(0, num.length - (4 * i + 3)) + '' +
                    num.substring(num.length - (4 * i + 3));
            return (((sign) ? '' : '-') + num + '.' + cents);
        },
        //将数字转换为指定小数位小数 --liuxiaoyang 20210514
        toFloat: function (num, length) {
            if (length == undefined || length < 0 || isNaN(length)) {
                length = 0;
            }
            if (num == null || num == "") {
                num = "0";
            }
            num = num.toString().replace(/\$|\,/g, '');
            if (isNaN(num))
                num = "0";
            return parseFloat(num).toFixed(length);
        },
        toFileSize: function (size) {
            if (size == null || size == "") {
                return "";
            }
            if (size < 1024.00)
                return jinkai.toDecimal(size) + " 字节";
            else if (size >= 1024.00 && size < 1048576)
                return jinkai.toDecimal(size / 1024.00) + " KB";
            else if (size >= 1048576 && size < 1073741824)
                return jinkai.toDecimal(size / 1024.00 / 1024.00) + " MB";
            else if (size >= 1073741824)
                return jinkai.toDecimal(size / 1024.00 / 1024.00 / 1024.00) + " GB";
        },
        toTreeViewJson: function (data, id) {
            var treeJson = [];
            var childNode = jinkai.jsonFind(data, function (v) { return v.parentId == id });
            if (childNode.length > 0) {
                $.each(childNode, function (i) {
                    var treeModel = {
                        id: childNode[i].id,
                        text: childNode[i].text,
                        hasChildren: jinkai.jsonFind(data, function (v) { return v.parentId == childNode[i].id }).length == 0 ? false : true,
                        ChildNodes: jinkai.toTreeViewJson(data, childNode[i].id),
                        isexpand: childNode[i].isexpand == undefined ? true : childNode[i].isexpand,
                        complete: true,
                    }
                    //编码
                    if (childNode[i].code != undefined) {
                        treeModel["code"] = childNode[i].code;
                    }
                    if (childNode[i].title != undefined) {
                        treeModel["title"] = childNode[i].title;
                    }
                    //复选框
                    if (childNode[i].showcheck != undefined) {
                        treeModel["showcheck"] = childNode[i].showcheck;
                    }
                    //复选框状态
                    if (childNode[i].checkstate != undefined) {
                        treeModel["checkstate"] = childNode[i].checkstate;
                    }
                    //图标
                    if (childNode[i].img != undefined) {
                        treeModel["img"] = childNode[i].img;
                    }
                    //扩展属性
                    if (childNode[i].extdata != undefined) {
                        treeModel["extdata"] = childNode[i].extdata;
                    }
                    //允许点击
                    if (childNode[i].click != undefined) {
                        treeModel["click"] = childNode[i].click;
                    }
                    treeJson.push(treeModel);
                });
            }
            return treeJson;
        },
        jsonFind: function (data, action) {
            if (action == null) return;
            var reval = new Array();
            $(data).each(function (i, v) {
                if (action(v)) {
                    reval.push(v);
                }
            })
            return reval;
        },
        openSlide: function (options) {
            var defaults = {
                type: "add", //add/wizard
                id: "form",
                title: "",
                url: "",
                width: null,
                callBack: null,
                style: null, //扩展写style
                loadComplete: null
            };
            var options = $.extend(defaults, options);
            options.url = jinkai.rootPath + options.url;
            var $element = $("#" + options.id);
            if ($element.length == 0) {
                var templet = '<div class="main-slide-shade" id="shade_' + options.id + '"></div>';
                templet += '<div class="main-slide" id="' + options.id + '" style="' + options.style + '">';
                templet += '    <div class="slide-hd">';
                templet += '        <div class="hd-title">' + options.title + '</div>';
                templet +=
                    '        <div class="hd-action"><button type="button" class="btn btn-primary btn-sm slide-accept"><i class="fa fa-check m-r-xs"></i>确定</button><button type="button" class="btn btn-default btn-sm slide-close"><i class="fa fa-close i-default m-r-xs"></i>关闭</button></div>';
                templet += '    </div>';
                templet += '    <div class="slide-bd"><iframe id="' + options.id + '" name="' + options.id +
                    '" width="100%" height="100%" frameborder="0" ></iframe></div>';
                templet += '</div>';
                $("body").append(templet);
                $element = $("#" + options.id);
                if (options.width != null) {
                    var _height = $(window).height() - (options.type == "wizard" ? 0 : 50);
                    $element.find(".slide-bd").height(_height);
                    $(window).resize(function () {
                        $element.find(".slide-bd").height(_height);
                    });
                    $element.css({
                        right: -options.width,
                        width: options.width
                    });
                    $element.animate({
                        right: 0,
                        speed: 500,
                    }).show(250, function () {
                        $element.find("iframe").attr("src", options.url);
                        //$element.find("iframe").load(function () {
                        //    $(this).thisWindow().$('.ajax-loader').fadeOut();
                        //    if (options.loadComplete != null) {
                        //        options.loadComplete($(this).thisWindow());
                        //    }
                        //});
                        $element.find("iframe").on("load", function () {
                            $(this).thisWindow().$('.ajax-loader').fadeOut();
                            if (options.loadComplete != null) {
                                options.loadComplete($(this).thisWindow());
                            }
                        });
                    });
                }
            }
            if (options.type == "add") {
                $element.find(".slide-accept")[0].options = options;
                $element.find(".slide-accept").unbind();
                $element.find(".slide-accept").click(function () {
                    var _options = $(this)[0].options;
                    _options.callBack(_options.id);
                });
            }
            if (options.type == "wizard") {
                $element.find(".slide-hd").remove();
                // $element.find("iframe").load(function () {
                //     $(this).thisWindow().$('#moduleSlide-title').html(options.title);
                //     $(this).thisWindow().$('#moduleSlide-close').click(function () {
                //         $element.animate({ right: -options.width, speed: 2000 }, function () {
                //             $element.prev(".main-slide-shade").remove();
                //             $element.remove();
                //         });
                //     })
                //     $(this).thisWindow().$('#moduleSlide-btn').find("a#btn_close").click(function () {
                //         $element.animate({ right: -options.width, speed: 2000 }, function () {
                //             $element.prev(".main-slide-shade").remove();
                //             $element.remove();
                //         });
                //     })
                // });
                $element.find("iframe").on("load", function () {
                    $(this).thisWindow().$('#moduleSlide-title').html(options.title);
                    $(this).thisWindow().$('#moduleSlide-close').click(function () {
                        $element.animate({ right: -options.width, speed: 2000 }, function () {
                            $element.prev(".main-slide-shade").remove();
                            $element.remove();
                        });
                    })
                    $(this).thisWindow().$('#moduleSlide-btn').find("a#btn_close").click(function () {
                        $element.animate({ right: -options.width, speed: 2000 }, function () {
                            $element.prev(".main-slide-shade").remove();
                            $element.remove();
                        });
                    })
                });
            }
            if (options.callBack == null) {
                $element.find(".slide-accept").remove();
            }
            $element.find(".slide-close").click(function () {
                $element.prev(".main-slide-shade").remove();
                $element.remove();
            });
            //全屏处理
            if (options.width == null) {
                if (options.callBack == null) {
                    $element.find(".slide-hd").remove();
                    $element.find(".slide-bd").height($(window).height() - 20);
                } else {
                    $element.find(".slide-bd").height($(window).height() - 70);
                }
                $element.css({
                    "top": "10px",
                    "border-left": "none",
                    "box-shadow": "none",
                    "margin-left": "10px",
                    "right": -($(window).width() - 20),
                    "width": $(window).width() - 20,
                    "height": $(window).height() - 20
                });
                $(window).resize(function () {
                    $element.find(".slide-bd").height($(window).height() - 20);
                    $element.css({
                        "width": $(window).width() - 20,
                        "height": $(window).height() - 20
                    });
                });
                $element.animate({
                    right: 10,
                    speed: 0
                }).show(0, function () {
                    $element.find("iframe").attr("src", options.url);
                    $element.find("iframe").on("load", function () {
                        $(this).thisWindow().$("#moduleSlide-title").html(options.title);
                        $(this).thisWindow().$("#moduleSlide-close").click(function () {
                            jinkai.thisTab().jinkai.openSlideClose();
                        });
                        $(this).thisWindow().$("#moduleSlide-btn").find("a#btn_close").click(function () {
                            jinkai.thisTab().jinkai.openSlideClose();
                        });
                        $(this).thisWindow().$(".ajax-loader").fadeOut();
                        if (options.loadComplete != null) {
                            options.loadComplete($(this).thisWindow());
                        }
                    });
                });
            }
        },
        openSlideClose: function (name) {
            if (name == undefined) {
                $(".main-slide").remove();
                $(".main-slide-shade").remove();
            } else {
                $(".main-slide#" + name).remove();
                $(".main-slide-shade#shade_" + name).remove();
            }
        },
        openContent: function (options) {
            var defaults = {
                top: true,
                skin: null,
                id: "form",
                title: "",
                width: "100px",
                height: "100px",
                content: '',
                shade: 0.3,
                fixed: true,
                move: true,
                btn: (function () {
                    if (options.callBack != undefined) {
                        return ['确定', '关闭'];
                    } else {
                        return null;
                    }
                })(),
                btnclass: ['btn btn-sm btn-primary', 'btn btn-sm btn-default'],
                callBack: null,
                loadComplete: null
            };
            var options = $.extend(defaults, options);
            var _width = _top.$(window).width() > parseInt(options.width.replace('px', '')) ? options.width : _top.$(window).width() +
                'px';
            var _height = _top.$(window).height() > parseInt(options.height.replace('px', '')) ? options.height : _top.$(
                window).height() + 'px';
            var _dialog = {
                skin: options.skin,
                id: options.id,
                type: 1,
                shade: options.shade,
                title: options.title,
                fix: options.fixed,
                offset: options.offset,
                time: options.time,
                area: [_width, _height],
                content: options.content,
                btn: options.btn,
                btnclass: options.btnclass,
                yes: function () {
                    options.callBack(options.id);
                },
                cancel: function () {
                    return true;
                }
            }
            if (options.move == false) {
                _dialog["move"] = options.move;
            }
            if (options.top == true) {
                _top.layer.open(_dialog);
            } else {
                window.layer.open(_dialog);
            }
            if (options.loadComplete != null) {
                options.loadComplete(options.id)
            }
        },
        openClose: function (name) {
            if (name == undefined) {
                name = window.name
            }
            var index = _top.layer.getFrameIndex(name);
            if (index == undefined) {
                _top.layer.closeAll('page');
            }
            _top.layer.close(index);
        },
        openWindow: function (options) {
            var defaults = {
                id: "form",
                title: '',
                width: "100px",
                height: "100px",
                url: '',
                shade: 0.3,
                fixed: false,
                btn: (function () {
                    if (options.btn != undefined && options.btn != null) {
                        return options.btn;
                    }
                    //if (options.btn == null) {
                    //    return null;
                    //}
                    if (options.callBack != undefined) {
                        return ['确定', '关闭'];
                    } else {
                        return null;
                    }
                })(),
                btnclass: ['btn btn-sm btn-primary', 'btn btn-sm btn-default'],
                callBack: null
            };
            var options = $.extend(defaults, options);
            options.url = jinkai.rootPath + options.url;
            _top.layer.open({
                id: options.id,
                type: 2,
                shade: options.shade,
                title: options.title,
                resize: false,// 禁止拉伸
                fix: true,
                area: [options.width, options.height],
                content: options.url,
                btn: options.btn,
                btnclass: options.btnclass,
                yes: function () {
                    options.callBack(options.id)
                },
                cancel: function () {
                    return true;
                }
            });
        },
        // 导入数据
        openImport: function (url, title) {
            if (title == undefined || title == null) {
                title = "导入数据";
            }
            this.openWindow({
                title: title,
                url: url,
                width: "400px",
                height: "400px",
                callback: function () {
                    this.reload()
                }
            });
        },
        confirm: function (options) {
            var defaults = {
                title: "系统提示",
                content: "",
                callBack: null
            };
            var options = $.extend(defaults, options);
            _top.layer.confirm(options.content, {
                icon: 0,
                title: options.title,
                btn: ["确定", "取消"],
                shade: 0.3,
            }, function () {
                options.callBack();
            }, function () {
                return true;
            });
        },
        alert: function (content, type) {
            var icon = 0;
            if (type == "success") {
                icon = 1;
            }
            if (type == "error") {
                icon = 2;
            }
            if (type == "warning") {
                icon = 0;
            }
            _top.layer.alert(content, {
                icon: icon,
                title: "系统提示",
                btn: ["确定"],
                shade: 0.3,
            });
        },
        request: function () {
            //var search = location.search.slice(1);
            //var arr = search.split("&");
            //var data = [];
            //for (var i = 0; i < arr.length; i++) {
            //    var ar = arr[i].split("=");
            //    data[ar[0]] = unescape(ar[1]);
            //}
            //return data;
            var param, url = location.search, theRequest = {};
            if (url.indexOf("?") != -1) {
                var str = url.substr(1);
                strs = str.split("&");
                for (var i = 0, len = strs.length; i < len; i++) {
                    param = strs[i].split("=");
                    theRequest[param[0]] = decodeURIComponent(param[1]);
                }
            }
            return theRequest;
        },
        download: function (options) {
            var defaults = {
                method: "GET",
                url: "",
                param: []
            };
            var options = $.extend(defaults, options);
            if (options.url && options.param) {
                var $form = $('<form action="' + options.url + '" method="' + (options.method || 'post') + '"></form>');
                for (var key in options.param) {
                    $form.append($('<input type="hidden" />').attr('name', key).val(options.param[key]));
                }
                //$form.append($('<input type="hidden" />').attr("name", "Authorize").val(jinkai.getAuthorize()));
                $form.append($('<input type="hidden" />').attr("name", "Authorize").val("admin"));
                $form.appendTo('body').submit().remove();
            };
        },
        msg: function (content, type) {
            if (_top.layer == undefined) {
                return;
            }
            var time = 4000;
            if (type != undefined) {
                var icon = "";
                if (type == "success") {
                    icon = 1;
                }
                if (type == "error") {
                    icon = 2;
                }
                if (type == "warning") {
                    icon = 0;
                }
                _top.layer.msg(content, {
                    icon: icon
                });
                jinkai.loading(false);
            } else {
                _top.layer.msg(content);
            }
        },
        cookie: function (name, value, options) {
            if (typeof value != "undefined") {
                options = options || {};
                if (value === null) {
                    value = '';
                    options.expires = -1;
                }
                var expires = '';
                if (options.expires && (typeof options.expires == 'number' || options.expires.toUTCString)) {
                    var date;
                    if (typeof options.expires == 'number') {
                        date = new Date();
                        date.setTime(date.getTime() + (options.expires * 24 * 60 * 60 * 1000));
                    } else {
                        date = options.expires;
                    }
                    expires = '; expires=' + date.toUTCString(); // use expires attribute, max-age is not supported by IE
                }
                var path = options.path ? '; path=' + options.path : '';
                var domain = options.domain ? '; domain=' + options.domain : '';
                var secure = options.secure ? '; secure' : '';
                document.cookie = [name, '=', encodeURIComponent(value), expires, path, domain, secure].join('');
            } else {
                var cookieValue = null;
                if (document.cookie && document.cookie != '') {
                    var cookies = document.cookie.split(';');
                    for (var i = 0; i < cookies.length; i++) {
                        var cookie = jQuery.trim(cookies[i]);
                        if (cookie.substring(0, name.length + 1) == (name + '=')) {
                            cookieValue = decodeURIComponent(cookie.substring(name.length + 1));
                            break;
                        }
                    }
                }
                return cookieValue;
            }
        },
        loading: function (bool, text) {

            if (bool == true && _top.$("body").find(".ajax-loading-text").length == 0) {
                _top.$("body").append(
                    '<div class="ajax-loading-shade"></div><div class="ajax-loading-text"><i class="fa fa-spinner fa-spin fa-3x fa-fw"></i><span>正在处理，请耐心等待…</span></div>'
                );
                var _width = _top.$("body").width();
                var _height = _top.$("body").height() == 0 ? _top.$(window).height() : _top.$("body").height();
                _top.$(".ajax-loading-text").css("left", (_width - _top.$(".ajax-loading-text").innerWidth()) / 2);
                _top.$(".ajax-loading-text").css("top", (_height - _top.$(".ajax-loading-text").innerHeight()) / 2 + 30);
            } else {
                _top.$(".ajax-loading-shade,.ajax-loading-text").remove();
            }
            if (!!text) {
                _top.$(".ajax-loading-text").find("span").html(text);
            }
        },

        // 打印
        print: function (url, title) {
            //_top.window.open("/pages/print/index.html?url=" + url);

            $("#printProxyIframe").remove();
            var document = window.document;
            var iframe, win;
            var isOk = false;
            iframe = document.createElement("iframe");
            iframe.id = "printProxyIframe";
            iframe.frameBorder = 0;
            iframe.setAttribute("style", 'position:absolute;z-index:100;left:0;top:0;width:100%;height:100%;background:#fff;visibility:hidden;');
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
            iframe.src = url;
            var timer = setInterval(function () {
                if (isOk) {
                    clearInterval(timer);
                    win.onafterprint = function () {
                        win.onafterprint = null;
                        iframe.parentNode.removeChild(iframe);
                    };
                    // 开始打印
                    if (timer) {
                        clearTimeout(timer);
                    }
                    timer = setTimeout(function () {
                        win.focus();
                        win.print();
                    }, 100);
                }
            }, 100);
            return this;
        },

        //权限验证
        verifyRight: function (module, show) {
            var system = this.getData().userInfo();
            var isAdmin = system.isAdmin;
            if (isAdmin) {
                return true;
            };

            var rights = system.rights;
            var name = module.split('_')[0];
            var btns = rights[name];
            if (btns == undefined || btns == null || btns.length < 1) {
                jinkai.rightMsg(show);
                return false;
            }
            var value = module.split('_')[1];

            if ($.inArray(value, btns) != -1) {
                return true;
            } else {
                jinkai.rightMsg(show);
                return false;
            }

            return false;
        },
        rightMsg: function (show) {
            if (show != undefined && show == false) {
                return false;
            }
            var html = [
                '<div style="padding:0px 10px">',
                '<p style="font-weight:bold">您没有该功能的使用权限！</p>',
                '<p>请联系管理员为您授权！</p>',
                '</div>'
            ].join('');
            jinkai.msg(html, "error");
        },

        // 过滤用户按钮权限
        filterAuthorize: function (moduleName) {
            // 获得用户信息
            var system = this.getData().userInfo();
            // 判断是否是超级管理员
            var isAdmin = system.isAdmin;
            if (isAdmin) {
                return;
            };

            // 获得按钮权限
            var rights = system.rights;
            var btns = rights[moduleName];

            var $element = $(".btn-authorize");
            //$element.find(".btn").attr("authorize", "no");
            $element.find("[authorize=yes]").attr("authorize", "no");
            $.each(btns, function (i) {
                // todo
                var btnName = btns[i].toLowerCase();

                $element.find("#btn_" + btnName).attr("authorize", "yes");
                // 启用、禁用共用一个权限
                if (btnName == "enabled") {
                    $element.find("#btn_disable").attr("authorize", "yes");
                    $element.find("#btn_enable").attr("authorize", "yes");
                }
                // 上移、下移共用一个权限
                if (btnName == "edit") {
                    $element.find("#btn_first").attr("authorize", "yes");
                    $element.find("#btn_next").attr("authorize", "yes");
                }
            });
            // 没有导出权限 移除弹出菜单
            if ($.inArray("Export", btns) < 0) {
                $("#btn_export").html("");
                $("#btn_export").next(".dropdown-menu").remove();

            }
            // 移除没有权限的按钮
            $element.find("[authorize=no]").parent("li").remove();
            $element.find("[authorize=no]").remove();
            $element.find("li.split").each(function () {
                if ($(this).next("li").length == 0) {
                    $(this).remove();
                }
            });
            // 添加请求日志
            requestLog(moduleName);
            // 请求日志 todo 暂不实现
            function requestLog(moduleName) {
                //jinkai.ajax({ async: true, url: jinkai.toUrl("/ajax/logs.ashx?action=RequestLog&moduleName=" +  moduleName) });
            }
        },
        //打开案件详情
        petitionDetail: function (id) {
            jinkai.openWindow({
                title: "案件详情",
                url: "/pages/petition/detail.html?id=" + id,
                width: "780px",
                height: "580px",
                resize: false
            });
        },
        newsDetail: function (id) {
            jinkai.openContent({
                title: "通知中心",
                width: "750px",
                height: "500px",
                loadComplete: function (name) {
                    jinkai.ajax({
                        async: false,
                        type: "GET",
                        url: jinkai.toUrl("/ajax/news.ashx?action=query&id=" + id),
                        param: {},
                        success: function (result) {
                            _top.$("#" + name).html('<pre style="margin: 5px;padding: 10px;"><code>' + result.data.content + '</code></pre>').height(430)
                            _top.$("#" + name).prev(".layui-layer-title").html(result.data.title + "<br/>" + '<label  class="media-meta m-r-sm">' + result.data.author + '</label><label class="media-meta">' + '发布日期：' + result.data.newsDate + '</label>');
                            _top.$("#" + name).prev(".layui-layer-title").css({ "padding-top": "15px", "height": "auto", "line-height": "inherit" });

                        },
                        error: function (result) {
                            jinkai.msg(result.msg, "error");
                        },
                    })

                }
            });
        }

    };

    $.fn.comboBoxIframe = function (options) {
        var defaults = {
            url: null,
            height: 200,
            width: $(this).innerWidth() + 2,
            change: null
        };
        var options = $.extend(defaults, options);
        var $element = $(this).before('<span style="border-color: #888 transparent transparent transparent;border-style: solid;border-width: 5px 4px 0 4px;height: 0;right: 8px; margin-left: -4px;margin-top: -2px; position: absolute;top: 50%;width: 0;"></span>');
        var $shade = $('<div id="shade_' + $element.attr("name") + '" style="display: none;width: 100%;height: 100%;opacity: 0.0;filter: alpha(opacity=00);background:#fff;position: absolute;top: 0;left: 0;z-index: 100;"></div>');
        var $box = $('<div id="box_' + $element.attr("name") + '" class="dropdown-menu" style="z-index: 101;display: none;position: absolute;left: ' + $element.offset().left + 'px;top: ' + ($element.offset().top + $element.innerHeight() + 1) + 'px;width:' + options.width + 'px;height:' + options.height + 'px;padding-top: 0px;"></div>');
        //if (options.disabled == true) {
        //    $element.attr("disabled", "disabled").css("cursor", "default");
        //    return false;
        //}
        $element.attr("readonly", "readonly").css({ "cursor": "pointer", "background-color": "#fff" });
        $element.click(function () {
            $box.html('<iframe id="' + $element.attr("name") + '" width="100%" height="100%" frameborder="0" src="' + options.url + '"></iframe>').toggle();
            $shade.toggle();
            $element.prev("span").css("border-width", "0 4px 5px 4px").css("border-color", "transparent transparent #888 transparent");
        });
        $shade.click(function () {
            $element.click();
            $element.prev("span").css("border-width", "5px 4px 0 4px").css("border-color", "#888 transparent transparent transparent");
        });
        $box.on("_change", function (e, data) {
            $element.val(data.text).attr("title", data.text);
            $element[0]["data"] = data;
            $element.parents('.control-value').find('i.error').remove();
            $element.parents('.control-value').removeClass('error');
            if (options.change != null) {
                options.change(data);
            }
            $element.click();
        });
        $("body").append($shade, $box);
    }

    $.fn.comboBox = function (options) {
        //debugger
        var $element = $(this);
        var defaults = {
            id: "id",
            text: "name",
            title: "title",
            height: 200,
            search: false,
            tags: false,
            multiple: $element.attr("multiple") == undefined ? false : true,
            data: null,
            url: null,
            param: [],
            open: null,
            change: null,
            success: null,
            placeholder: $element.find("option:eq(0)").text(),
            placeholderVal: $element.find("option:eq(0)").val()
        };
        var options = $.extend(defaults, options);
        if (options.url != null) {
            jinkai.ajax({
                type: "GET",
                url: options.url,
                param: options.param,
                success: function (result) {
                    options.data = result.data.items != undefined ? result.data.items : result.data;
                }
            });
        }
        if (options.data != null) {
            $element.empty();
            if (options.placeholder != "") {
                $element.append($("<option></option>").val(options.placeholderVal).html(options.placeholder));
            }
            if (options.success == null) {
                $.each(options.data, function (i) {
                    var $option = $("<option></option>").val(options.data[i][options.id]).text(options.data[i][options.text]);
                    if (options.data[i][options.title]) {
                        $option.attr("title", options.data[i][options.title]);
                    }
                    $element.append($option);
                });
            } else {
                options.success($element, options.data);
            }
        }
        $element.on("change", function (e) {
            var index = $(this).get(0).selectedIndex;
            var id = $.trim($(this).find("option:selected").val());
            var text = $.trim($(this).find("option:selected").text());
            var $rendered = $element.next(".select2").find("span.select2-selection__rendered");
            $rendered.html(text).attr("title", text);
            if (id != null && id != "") {
                $rendered.css("color", "");
            } else {
                $rendered.css("color", "#999");
            }
            $element.parents('.control-value,li').find('i.error').remove();
            $element.parents('.control-value,li').removeClass('error');
            //回调执行
            if (options.change != null && id != null && id != "") {
                if (options.data != null && options.data.length > 0) {
                    var selectedData = jinkai.jsonFind(options.data, function (v) {
                        return v[options.id] == id
                    });
                    if (selectedData.length == 1) {
                        if (options.change(selectedData[0]) == false) {
                            return;
                        }
                    } else {
                        if (options.change(options.data[index - 1]) == false) {
                            return;
                        }
                    }
                } else {
                    if (options.change(id, text) == false) {
                        return;
                    }
                }
                $element.select2("close");
            }
        });
        $element.on("select2:open", function (e) {
            //debugger
            var aria = $(this).next(".select2").find(".selection > span").attr("aria-owns");
            $("#" + aria).css({
                "max-height": options.height
            });
            if (options.tags == true) {
                $("#" + aria).parents(".select2-container").hide();
            }
            if (options.open != null) {
                options.open(e)
            }
        });
        $element.select2({
            minimumResultsForSearch: options.search == true ? 0 : -1,
            tags: options.tags,
            multiple: options.multiple
        });
        if ($element.find("option:eq(0)").val() == "") {
            $element.next(".select2").find(".select2-selection__rendered").css("color", "#999");
        }
        if ($element.attr("multiple") == "multiple" && options.tags == false) {
            $element.next(".select2").find(".select2-selection--multiple").css("cursor", "pointer").addClass(
                "select2-selection--single");
            $element.next(".select2").find(".select2-selection--multiple").append(
                '<span class="select2-selection__arrow" role="presentation"><b role="presentation"></b></span>');
            $element.next(".select2").find(".select2-search--inline").hide().remove();
            var $rendered = $('<span class="select2-selection__rendered" style="color: #999;"></span>').html($element.next(
                ".select2").find(".select2-selection--multiple").find("ul.select2-selection__rendered").html());
            $rendered.text(options.placeholder);
            $element.next(".select2").find(".select2-selection--multiple").find("ul.select2-selection__rendered").prop(
                'outerHTML', $rendered.prop('outerHTML'));
            $element.on("change", function (e) {
                $element.next(".select2").find(".select2-selection__choice__remove").hide();
                $element.next(".select2").find(".select2-selection__choice").css({
                    "background-color": "#fff",
                    "border": "0px",
                    "margin-right": "0px",
                    "padding-right": "0px"
                });
                $element.next(".select2").find(".select2-selection__choice").not("li:last").append(",");
            });
        }
        return $element;
    };
    $.fn.comboBoxTree = function (options) {
        var $element = $(this);
        var showcheck = false;
        var defaults = {
            id: "id",
            text: "text",
            search: false,
            data: null,
            metadata: [], //原数据，搜索才会去调用这里面数据
            url: null,
            param: [],
            height: 200,
            change: null,
            open: null,
            placeholder: $element.find("option:eq(0)").text(),
            placeholderVal: $element.find("option:eq(0)").val()
        };
        var options = $.extend(defaults, options);
        if (options.url != null) {
            jinkai.ajax({
                type: "GET",
                url: options.url,
                param: options.param,
                success: function (result) {
                    options.data = result.data.items != undefined ? result.data.items : result.data;
                }
            });
        }
        if (options.data != null) {
            loadData(options.data);

            function loadData(data) {
                $.each(data, function (i) {

                    var newJson = jQuery.extend({}, data[i]);
                    newJson.ChildNodes = [];
                    newJson.hasChildren = false;
                    newJson.parent = "";
                    options.metadata.push(newJson);
                    if (data[i].ChildNodes != undefined) {
                        loadData(data[i].ChildNodes);
                    }
                    // 如果该项不可点击，则文本框内不显示该项 TODO  --liu 
                    if (data[i].click == false) {
                        return true;
                    }

                    $element.append($("<option></option>").val(data[i][options.id]).html(data[i][options.text]));
                });
            }
        }
        $element.on("change", function (e) {
            var id = $.trim($(this).find("option:selected").val());
            var text = $.trim($(this).find("option:selected").text());
            var $rendered = $element.next(".select2").find("span.select2-selection__rendered");
            $rendered.html(text).attr("title", text);
            if (id != null && id != "") {
                $rendered.css("color", "");
            } else {
                $rendered.css("color", "#999");
            }
            $element.parents('.control-value,li').find('i.error').remove();
            $element.parents('.control-value,li').removeClass('error');

            //回调执行
            if (options.change != null && id != null && id != "") {
                var selectedData = jinkai.jsonFind(options.metadata, function (v) {
                    return v[options.id] == id
                })[0];
                options.change(selectedData);
                $element.select2("close");
            } else {
                $element.select2("close");
            }
        });
        $element.on("select2:close", function (e) {
            console.log("select2:close");
        });
        $element.on("select2:open", function (e) {
            //debugger
            if ($(".select2-results").find(".select2-results-tree").length == 0) {
                //  todo 
                $(".select2-results").append('<div class="select2-results-tree" id="select2-' + $element.attr("id") + '-results" style="max-height:' + options.height +
                    'px;height:initial;"></div>');

                //$(".select2-results").append('<div class="select2-results-tree" id="' + $(".select2-results").find("ul.select2-results__options").attr("id") + '" style="max-height:' + options.height + 'px;height:initial;"></div>');

                $(".select2-results").find("ul.select2-results__options").remove();
            }
            var $panel = $(".select2-results-tree");
            //加载数据
            if ($element.attr("multiple") == "multiple") {
                showcheck = true;
            }
            loadtreeview($panel, options.data, showcheck);
            //高亮选中
            if ($element.val() != undefined) {
                //选中
                $panel.treeview().setSelected($element.val());
                ////展开+
                //var $img = $("[data-value='" + $element.val() + "']").parent().parent().parent().parent().prev(".bbit-tree-node-el").find(".bbit-tree-ec-icon");
                //if (!$img.hasClass("bbit-tree-elbow-minus") && !$img.hasClass("bbit-tree-elbow-end-minus")) {
                //    $img.trigger("click");
                //}
            }
            //搜索事件
            if (options.search == true) {
                $(".select2-search").append(
                    '<a style="z-index: 3;color: #6b7a99;" class="form-control-feedback"><i class="fa fa-search"></i></a>');
                $(".select2-search").find("input").unbind();
                $(".select2-search").find("input").keydown(function (e) {
                    if (e.which == 13) {
                        if ($(this).val() != "") {
                            var searchData = [];
                            for (var i in options.metadata) {
                                if (options.metadata[i]["text"].indexOf($(this).val()) != -1) {
                                    searchData.push(options.metadata[i])
                                }
                            }
                            loadtreeview($panel, searchData, showcheck);
                        } else {
                            loadtreeview($panel, options.data, showcheck);
                        }
                    }
                });
            }
            //打开事件
            if (options.open != null) {
                options.open($panel);
            }
        });
        $element.select2({
            minimumResultsForSearch: options.search == true ? 0 : -1
        });
        if ($element.find("option:eq(0)").val() == "") {
            $element.next(".select2").find(".select2-selection__rendered").css("color", "#999");
        }
        if ($element.attr("multiple") == "multiple") {
            $element.next(".select2").find(".select2-selection--multiple").css("cursor", "pointer").addClass(
                "select2-selection--single");
            $element.next(".select2").find(".select2-selection--multiple").append(
                '<span class="select2-selection__arrow" role="presentation"><b role="presentation"></b></span>');
            $element.next(".select2").find(".select2-search--inline").hide().remove();
            var $rendered = $('<span class="select2-selection__rendered" style="color: #999;"></span>').html($element.next(
                ".select2").find(".select2-selection--multiple").find("ul.select2-selection__rendered").html());
            $rendered.text(options.placeholder);
            $element.next(".select2").find(".select2-selection--multiple").find("ul.select2-selection__rendered").prop(
                'outerHTML', $rendered.prop('outerHTML'));
            $element.on("change", function (e) {
                $element.next(".select2").find(".select2-selection__choice__remove").hide();
                $element.next(".select2").find(".select2-selection__choice").css({
                    "background-color": "#fff",
                    "border": "0px",
                    "margin-right": "0px",
                    "padding-right": "0px"
                });
                $element.next(".select2").find("li.select2-selection__choice").not("li:last").append(",");
            });
            //双击事件
            $element.next(".select2").delegate("li.select2-selection__choice", "dblclick", function (e) { });
        }

        function loadtreeview($panel, data, showcheck) {

            $panel.treeview({
                data: data,
                onnodeclick: function (item, status) {
                    if (item.click == false) {
                        return false;
                    }
                    if ($element.attr("multiple") == "multiple") {
                        var selectedId = item[options.id];
                        var selectedText = item[options.text];
                        var values = $element.val() == null ? [] : $element.val();
                        if ($.inArray(selectedId, values) >= 0) {
                            $element.next(".select2").find("li.select2-selection__choice[title='" + selectedText + "']").find(
                                ".select2-selection__choice__remove").trigger('click');
                        } else {
                            values.push(selectedId)
                            $element.val(values);
                        }
                        $element.trigger('change');
                    } else {
                        var selectedId = item[options.id];
                        if ($element.val() == selectedId) {
                            $element.val("");
                        } else {
                            $element.val(selectedId);
                        }
                        $element.trigger('change');
                    }
                }
            });
        }
    }
    // 判断英文字符
    $.fn.onEnglish = function () {
        var $element = $(this);
        $element.bind("contextmenu", function () {
            return false;
        });
        $element.css('ime-mode', 'disabled');
        $element.keyup(function () {
            $(this).val($(this).val().replace(/[^A-Za-z]/g, ''));
        }).bind("paste", function () {
            $(this).val($(this).val().replace(/[^A-Za-z]/g, ''));
        }).css("ime-mode", "disabled");
        return $element;
    }
    // 固定电话格式（0-9、-）liuxiaoyang 20210107
    $.fn.onPhone = function () {
        var $element = $(this);
        $element.bind("contextmenu", function () {
            return false;
        });
        $element.css('ime-mode', 'disabled');
        $element.keyup(function () {
            $(this).val($(this).val().replace(/[^0-9-]/g, ''));
        }).bind("paste", function () {
            $(this).val($(this).val().replace(/[^0-9-]/g, ''));
        }).css("ime-mode", "disabled");
        return $element;
    }

    //只允许输入整数
    $.fn.onNumber = function () {
        var $element = $(this);
        $element.bind("contextmenu", function () {
            return false;
        });
        $element.css('ime-mode', 'disabled');
        $element.keyup(function () {
            $(this).val($(this).val().replace(/[^0-9]/g, ''));
        }).bind("paste", function () {
            $(this).val($(this).val().replace(/[^0-9]/g, ''));
        }).css("ime-mode", "disabled");
        return $element;
    }

    //只允许输入浮点数
    $.fn.onDecimal = function () {
        var $element = $(this);
        $element.bind("contextmenu", function () {
            return false;
        });
        $element.css('ime-mode', 'disabled');
        $element.keyup(function () {
            $(this).val($(this).val().replace(/[^0-9.]/g, ''));
        }).bind("paste", function () {
            $(this).val($(this).val().replace(/[^0-9.]/g, ''));
        }).css("ime-mode", "disabled");
        return $element;
    }
    $.fn.thisWindow = function () {
        var $element = $(this);
        return $element[0].contentWindow || $element[0].contentDocument;
    }
    $.fn.formValidate = function (options) {
        var defaults = {
            errorPlacement: function (error, element) {
                if (element.parents('.control-value').length > 0) {
                    //element.parents('.control-value').find('i.error').remove();
                    //element.parents('.control-value').append('<i class="form-control-feedback fa fa-exclamation-circle error" data-placement="left" data-toggle="tooltip" title="' + error + '"></i>');
                    //element.parents('.control-value').find("[data-toggle='tooltip']").tooltip();
                    //element.parents('.control-value').addClass("error");  

                    // 当有多个“.control-value”时，只在一个“.control-value”中显示   --liuxiaoyang 20210113
                    element.parents('.control-value').find('i.error').remove();
                    $(element.parents('.control-value')[0]).append('<i class="form-control-feedback fa fa-exclamation-circle error" data-placement="left" data-toggle="tooltip" title="' + error + '"></i>');
                    element.parents('.control-value').find("[data-toggle='tooltip']").tooltip();
                    $(element.parents('.control-value')[0]).addClass("error");
                }
                if (element.parents('li').length > 0) {
                    element.parents('li').find('i.error').remove();
                    element.parents('li').append('<i class="form-control-feedback fa fa-exclamation-circle error" data-placement="left" data-toggle="tooltip" title="' + error + '"></i>');
                    element.parents('li').addClass("error");
                }
            },
            success: function (element) {
                if (element.parents('.control-value').length > 0) {
                    element.parents('.control-value').find('i.error').remove();
                    element.parents('.control-value').removeClass('error');
                }
                if (element.parents('li').length > 0) {
                    element.parents('li').find('i.error').remove();
                    element.parents('li').removeClass('error');
                }
            }
        };
        var options = $.extend(defaults, options);
        $(this).validate(options);
    }

    $.fn.formValid = function (options) {
        var defaults = {
            errorPlacement: function (error, element) {
                if (element.parents('.control-value').length > 0) {
                    element.parents('.control-value').find('i.error').remove();
                    element.parents('.control-value').append(
                        '<i class="form-control-feedback fa fa-exclamation-circle error" data-placement="left" data-toggle="tooltip" title="' +
                        error + '" ></i>');
                    element.parents('.control-value').find("[data-toggle='tooltip']").tooltip();
                    element.parents('.control-value').addClass("error");
                }
                if (element.parents('li').length > 0) {
                    element.parents('li').find('i.error').remove();
                    element.parents('li').append(
                        '<i class="form-control-feedback fa fa-exclamation-circle error" data-placement="left" data-toggle="tooltip" title="' +
                        error + '"></i>');
                    element.parents('li').addClass("error");
                }
            },
            success: function (element) {
                if (element.hasClass("error")) {
                    return;
                }
                if (element.parents('.control-value').length > 0) {
                    element.parents('.control-value').find('i.error').remove();
                    element.parents('.control-value').removeClass('error');
                }
                if (element.parents('li').length > 0) {
                    element.parents('li').find('i.error').remove();
                    element.parents('li').removeClass('error');
                }
            }
        };
        var options = $.extend(defaults, options);
        return $(this).valid(options);
    }

    $.fn.formValidError = function (error) {
        var element = $(this);
        if (element.parents('.control-value').length > 0) {
            element.parents('.control-value').addClass("error");
            element.parents('.control-value').find('.form-control').addClass('error');
            element.parents('.control-value').append(
                '<i class="form-control-feedback fa fa-exclamation-circle error" data-placement="left" data-toggle="tooltip" title="' +
                error + '"></i>');
            element.parents('.control-value').find("[data-toggle='tooltip']").tooltip();
            element.keydown(function () {
                element.parents('.control-value').removeClass("error");
                element.parents('.control-value').find('i.error').remove();
            });
        }
        if (element.parents('td').length > 0) {
            element.parents('td').addClass("error");
            element.parents('td').append(
                '<i class="feedback fa fa-exclamation-circle error" data-placement="left" data-toggle="tooltip" title="' + error +
                '"></i>');
            element.parents('td').find("[data-toggle='tooltip']").tooltip();
            element.keydown(function () {
                element.parents('td').removeClass("error");
                element.parents('td').find('i.error').remove();
            });
        }
        if (element.parents('tr.jqgrow').length > 0) {
            element.addClass("error");
            element.append('<i class="feedback fa fa-exclamation-circle error" data-placement="left" data-toggle="tooltip" title="' +
                error + '"></i>');
            element.find("[data-toggle='tooltip']").tooltip();
            element.on("click", function (e) {
                element.removeClass("error");
            });
            element.find('i.error').on("click", function (e) {
                element.removeClass("error");
                element.text("");
            });
        }
    }

    $.fn.grid = function (options) {
        var $element = $(this);
        var defaults = {
            authorize: false, //列头权限控制
            authorizeBindTable: $element.attr("id"), //列头权限控制绑定表格Id
            showOperate: function () { //显示工具栏
                if ($element.jqGrid("getGridParam", "multiselect") == false) {
                    $element.attr("data-selectrow", $element.jqGrid("getGridParam", "selrow"));
                }
                //显示工具栏操作按钮
                if ($element.jqGrid("getGridParam", "clickTriggerToolbar") == true && $("[data-trigger='" + $element.attr("id") +
                        "']").length > 0) {
                    var $operate = $("[data-trigger='" + $element.attr("id") + "']").css({
                        left: $element.parents(".ui-jqgrid").offset().left
                    });
                    $operate.find(".close").unbind();
                    $operate.find(".close").click(function () {
                        if (options.onSelect != null) {
                            options.onSelect($element.attr("data-selectrow"), false);
                        }
                        $operate.hide();
                        $element.resetSelection();
                        $element.attr("data-selectrow", "");
                    })
                    var length = $element.jqGrid("getGridParam", "selarrrow").length;
                    if ($element.jqGrid("getGridParam", "multiselect") == false) {
                        length = 1;
                    }
                    if (length > 0) {
                        //$operate.width($operate.parents(".main-hd").width() + 11).show().find(".first").find("span").text(length);
                        $operate.width($operate.parents(".main-hd").width() + 11).show().find("#numbers").text(length);
                        $(window).resize(function () {
                            $operate.width($operate.parents(".main-hd").width() + 11);
                        });
                    } else {
                        $operate.hide();
                    }
                }
            },
            url: "",
            mtype: "GET",
            datatype: "json",
            autowidth: true,
            multiselect: false, //复选框
            rownumbers: (options.grouping == undefined ? true : false),
            pagination: false,
            visiblePages: 8,
            rowNum: (options.pagination == undefined ? 10000 : 30),
            rowList: [30, 50, 100, 500],
            shrinkToFit: false,
            gridview: true,
            clickTriggerToolbar: true,
            loadtext: "正在加载，请稍后…",
            notext: "没有找到数据",
            loadError: function (XMLHttpRequest) {
                //jinkai.ajaxError(XMLHttpRequest);
            },
            onSelectAll: function (rowids, statue) {
                _options.showOperate();
            },
            loadBeforeSend: function (request) {
                //request.setRequestHeader("Authorize", jinkai.getAuthorize());
            },
        };
        var _options = $.extend(defaults, options);
        if (_options.authorize == true) {
            // var columnData = jinkai.jsonFind(_top.$.indexData.authorizeColumn, function (v) { return v.ModuleId == jinkai.cookie("moduleId") && v.BindTable == _options.authorizeBindTable });
            // var colModel = _options.colModel;
            // //var colModelNew = jinkai.jsonFind(colModel, function (v) { return v.key == true });
            // $.each(colModel, function (i) {
            //     var isExist = jinkai.jsonFind(columnData, function (v) { return v.EnCode == colModel[i].name }).length == 0 ? false : true;
            //     if (isExist) {
            //         colModel.hidden = true;
            //         //colModelNew.push(colModel[i]);
            //     } else {
            //         colModel.hidden = false;
            //     }
            // });
            // _options.colModel = colModel;
        }
        if (_options.width != undefined) {
            _options.autowidth = false;
        }
        if (_options.height == undefined) {
            _options.height = "100%";
        }
        if (_options.treeGrid != undefined && _options.treeGrid != false) {
            _options.treeGridModel = "adjacency";
            var colModel = _options.colModel;
            $.each(colModel, function (i) {
                if (colModel[i].name == _options.ExpandColumn) {
                    var nextName = colModel[i + 1].name;
                    _options.ExpandColumn = nextName;
                    return false;
                }
            });
        }
        if (_options.cellEdit != undefined && _options.cellEdit != false) {
            _options.beforeSelectRow = function (rowid) {
                //debugger
                $element.find("tr").find("td:eq(1) i.fa").hide();
                $element.find("tr#" + rowid).find("td:eq(1) i.fa").show();
            }
            //自己扩展取消编辑
            $(document).click(function (e) {
                if (!$(e.target).hasClass("edit-cell") && !$(e.target).parents("td").hasClass("edit-cell")) {
                    var iRow = $element.find("tr.active").index();
                    var iCol = $element.find("tr.active").find("td.edit-cell").index();
                    if (iRow > 0 && iCol > 0) {
                        $element.jqGrid("saveCell", iRow, iCol);
                        $element.find("tr.active").find("td.edit-cell").removeClass("edit-cell success");
                        $element.find("i.fa-minus").hide();
                        $element.resetSelection();
                    }
                }
                e.stopPropagation();
            });
        }
        if (_options.datatype != undefined && _options.datatype == "local") {
            _options.unwritten = false;
        }
        _options.onSelectRow = function (rowid, status) {
            var length = $element.jqGrid("getGridParam", "selarrrow").length;
            if (status || length > 0) {
                _options.showOperate();
            } else {
                $("[data-trigger='" + $element.attr("id") + "']").find(".close").trigger("click");
            }

            // TODO 20210730
            if (this.p.selarrrow.length === currIds.length) {
                $('#cb_' + $.jgrid.jqID(this.p.id), this.grid.hDiv)[this.p.useProp ? 'prop' : 'attr']("checked", true);
            }

            if (options.onSelectRow != null) {
                options.onSelectRow(rowid, status);
            }
        }
        _options.loadComplete = function () {
            if (options.loadComplete != undefined) {
                options.loadComplete()
            }
            if (!!$element.attr("data-selectrow")) {
                $element.find("tr").removeClass("success")
                $element.jqGrid("setSelection", $element.attr("data-selectrow"));
                $element.find("[aria-selected='true']").addClass("success");
            }
        }
        _options.gridComplete = function () {
            // TODO 20210730
            currIds = $element.jqGrid('getDataIDs');

            if (options.gridComplete != undefined) {
                options.gridComplete()
            }
        }
        //_options.beforeSelectRow = function (rowid, e) {
        //    console.log("beforeSelectRow", "done");
        //    $element.jqGrid('setSelection', rowid);

        //    return false;

        //}

        $element.jqGrid(_options);
        return $element;
    }

    $.fn.gridData = function () {
        var $element = $(this);
        var data = $element[0].data.items == undefined ? $element[0].data : $element[0].data.items;
        return data;
    }
    $.fn.gridRowData = function (rowid) {
        var $element = $(this);
        var data = $element[0].data.items == undefined ? $element[0].data : $element[0].data.items;
        var colModel = $element.jqGrid("getGridParam", "colModel");
        var keyField = jinkai.jsonFind(colModel, function (v) {
            return v.key == true
        })[0].name;
        if (rowid == undefined) {
            var selectedRowIds = $element.jqGrid("getGridParam", "selarrrow");
            if (selectedRowIds.length > 0) {
                var json = [];
                for (var i = 0; i < selectedRowIds.length; i++) {
                    var rowData = jinkai.jsonFind(data, function (v) {
                        return v[keyField] == selectedRowIds[i]
                    })[0];
                    json.push(rowData);
                }
                return json;
            } else {
                var selectRow = $element.attr("data-selectrow") == undefined ? $element.jqGrid('getGridParam', 'selrow') :
                    $element.attr("data-selectrow");
                if (selectRow != null) {
                    return jinkai.jsonFind(data, function (v) {
                        return v[keyField] == selectRow
                    })[0];
                } else {
                    return null;
                }
            }
        } else {
            return jinkai.jsonFind(data, function (v) {
                return v[keyField] == rowid
            })[0];
        }
    }
    $.fn.gridReload = function () {
        var $element = $(this);
        $element.resetSelection();
        $element.trigger("reloadGrid");
        $element.attr("data-selectrow", "");
        $("[data-trigger='" + $element.attr("id") + "']").hide();
    }
    $.fn.gridReloadSelection = function () {
        var $element = $(this);
        $element.resetSelection();
        $element.trigger("reloadGrid");
    }
    $.fn.gridHideCol = function (colname) {
        var $element = $(this);
        $element.setGridParam().hideCol(colname).trigger("reloadGrid");
        var $th = $element.parents('.ui-jqgrid').find("table.ui-jqgrid-htable").find("th#" + $element.attr("id") + "_" +
            colname);
        var $thautowidth = $element.parents('.ui-jqgrid').find("table.ui-jqgrid-htable").find("th[autowidth='true']");
        var $tdautowidth = $element.find("tr:first").find("td:eq(" + $thautowidth.index() + ")");
        var autowidth = $thautowidth.outerWidth() + $th.width();
        $thautowidth.width(autowidth);
        $tdautowidth.width(autowidth);
    }
    $.fn.gridMoveRow = function (rowid, method) {
        var $element = $(this);
        var rownumbers = $element.getGridParam('rownumbers');
        var $tr = $element.find("#" + rowid);
        if (method == "up") {
            var prev = $tr.prev();
            if (prev.index() > 0) {
                if (rownumbers == true) {
                    var rn = Number($tr.find("td:eq(0)").text()) - 1;
                    var targetRn = Number($tr.prev("tr").find("td:eq(0)").text()) + 1;
                    $tr.find("td:eq(0)").text(rn);
                    $tr.prev("tr").find("td:eq(0)").text(targetRn);
                }
                $tr.insertBefore(prev);
            }
        } else if (method == "down") {
            var next = $tr.next();
            if (next.index() > 0) {
                if (rownumbers == true) {
                    var rn = Number($tr.find("td:eq(0)").text()) + 1;
                    var targetRn = Number($tr.next("tr").find("td:eq(0)").text()) - 1;
                    $tr.find("td:eq(0)").text(rn);
                    $tr.next("tr").find("td:eq(0)").text(targetRn);
                }
                $tr.insertAfter(next);
            }
        }
    }
    $.fn.treeMoveRow = function (rowid, method) {
        var $element = $(this);
        var $li = $element.find("#" + $element.attr("id") + "_" + rowid.replace(/-/g, "_")).parent("li.bbit-tree-node");
        if (method == "up") {
            var prev = $li.prev();
            $li.insertBefore(prev);
        } else if (method == "down") {
            var next = $li.next();
            $li.insertAfter(next);
        }
    }
    $.fn.comboBoxContent = function (options) {
        var defaults = {
            height: 200,
            width: $(this).innerWidth() + 2,
            loadComplete: null
        };
        var options = $.extend(defaults, options);
        var $element = $(this).before(
            '<span style="border-color: #888 transparent transparent transparent;border-style: solid;border-width: 5px 4px 0 4px;height: 0;right: 8px; margin-left: -4px;margin-top: -2px; position: absolute;top: 50%;width: 0;"></span>'
        );
        var $shade = $('<div id="shade_' + $element.attr("name") +
            '" style="display: none;width: 100%;height: 100%;opacity: 0.0;filter: alpha(opacity=00);background:#fff;position: absolute;top: 0;left: 0;z-index: 100;"></div>'
        );
        var $box = $('<div id="box_' + $element.attr("name") +
            '" class="dropdown-menu" style="z-index: 101;display: none;position: absolute;left: ' + $element.offset().left +
            'px;top: ' + ($element.offset().top + $element.innerHeight() + 1) + 'px;width:' + options.width +
            'px;max-height:' + options.height + 'px;padding-top: 0px;overflow: auto;"></div>');
        //if (options.disabled == true) {
        //    $element.attr("disabled", "disabled").css("cursor", "default");
        //    return false;
        //}
        $element.attr("readonly", "readonly").css({
            "cursor": "pointer",
            "background-color": "#fff"
        });
        $element.click(function () {
            $box.toggle();
            $shade.toggle();
            $element.prev("span").css("border-width", "0 4px 5px 4px").css("border-color",
                "transparent transparent #888 transparent");
        });
        $shade.click(function () {
            $element.click();
            $element.prev("span").css("border-width", "5px 4px 0 4px").css("border-color",
                "#888 transparent transparent transparent");
        });
        $box.on("_close", function (e, data) {
            $element.parents('.control-value').find('i.error').remove();
            $element.parents('.control-value').removeClass('error');
            $element.click();
        });
        $box.on("_remove", function (e, data) {
            $element.parents('.control-value').find('i.error').remove();
            $element.parents('.control-value').removeClass('error');
            $shade.remove();
            $box.remove();
        });
        if (options.loadComplete != null) {
            options.loadComplete($box, $element)
        }
        $("#shade_" + $element.attr("name")).remove();
        $("#box_" + $element.attr("name")).remove();
        $("body").append($shade, $box);
    }

    $.fn.swapClass = function (c1, c2) {
        return this.removeClass(c1).addClass(c2);
    };
    $.fn.editor = function (options) {
        var $element = $(this);
        var defaults = {
            height: 300,
            textarea: $element,
            toolbar: ['title', 'bold', 'italic', 'underline', 'strikethrough', 'color', '|', 'ol', 'ul', 'blockquote', 'code', 'table', '|', 'link', 'image', 'hr'],
            placeholder: '这里输入内容...'
        };
        var options = $.extend(defaults, options);
        var $editor = new Simditor(options);
        $element[0]["editor"] = $editor;
        $element.parents(".simditor").find(".simditor-body").css({ "min-height": options.height });
    };
    $.fn.datePicker = function (options) {
        var $element = $(this);
        var defaults = {
            icon: true,
            el: $element.attr("id"),
            dateFmt: "yyyy-MM-dd"
        };
        var options = $.extend(defaults, options);
        $element.focus(function () {
            WdatePicker(options);
        });
        if (options.icon) {
            $element.addClass("wdatepicker");
        }
    }
    $.fn.onMaxValue = function (data) {
        var $element = $(this);
        $element.bind("contextmenu", function () {
            return false;
        });
        $element.css('ime-mode', 'disabled');
        $element.keyup(function () {
            if ($(this).val() >= data) {
                $(this).val(data);
            }
        }).bind("paste", function () {
            if ($(this).val() >= data) {
                $(this).val(data);
            }
        }).css("ime-mode", "disabled");
        return $element;
    }
})(jQuery)
