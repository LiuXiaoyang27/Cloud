+ function ($, window) {
    var app = {
        name: 'JINKAI',
        version: '1.0.0'
    };
    app.defaults = {
        sidebar: {
            folded: false,
            theme: "light",
            themes: ["light", "dark"]
        },
        navbar: {
            theme: "blue",
            themes: ["default", "white", "red", "green", "blue", "purple"]
        }
    };
    app.$body = $("body");
    // 左侧导航栏
    app.$sidebar = $("#app-aside");
    // 头部导航栏
    app.$navbar = $("#app-navbar");
    // frame主题
    app.$main = $("#app-main");
    app.settings = app.defaults;
    var appSettings = app.name + "Settings";
    app.storage = $.localStorage;
    if (app.storage.isEmpty(appSettings)) {
        app.storage.set(appSettings, app.settings);
    } else {
        app.settings = app.storage.get(appSettings);
    }
    app.saveSettings = function () {
        app.storage.set(appSettings, app.settings);
    };
    // initialize navbar
    app.$body.addClass("theme-" + app.settings.navbar.theme);
    // initialize sidebar
    app.$sidebar.addClass("in");
    app.settings.sidebar.folded &&
		app.$sidebar.addClass("folded") &&
		app.$body.addClass("sb-folded") &&
		$("#aside-fold").removeClass("is-active");
    // initialize main
    app.$main.addClass("in");
    app.init = function () { };
    window.app = app;
}(jQuery, window); +
function ($, window) {
    var $body = app.$body;
    var $navbar = app.$navbar;
    var navbar = {};
    navbar.init = function () {
        this.listenForEvents();
    };
    navbar.listenForEvents = function () {
        $(document)
			.on("click", "#navbar-search-open", openSearch)
			.on("click", "#search-close, .search-backdrop", closeSearch);
        $("#navbar-search").find("input").bind("keypress", function (event) {
            if (event.keyCode == "13") {
                var value = $(this).val();
                if (value != "") {
                    var menuData = $.indexData.authorizeMenu;
                    var menuRow = jinkai.jsonFind(menuData, function (v) {
                        return v.FullName == value && v.Type == 2
                    });
                    if (menuRow.length > 0) {
                        var options = {
                            id: menuRow[0].Id,
                            url: menuRow[0].UrlAddress,
                            img: menuRow[0].Icon,
                            text: menuRow[0].FullName,
                            module: menuRow[0].module // TODO
                        };
                        _top.app.mainIndex.tabAdd(options);
                        $("#navbar-search").find("input").val("");
                        $(".search-backdrop").trigger("click");
                    }
                }
            }
        });
    };
    navbar.getAppliedTheme = function () {
        var appliedTheme = "",
			themes = app.settings.navbar.themes,
			theme;
        for (theme in themes) {
            if ($navbar.hasClass(themes[theme])) {
                appliedTheme = themes[theme];
                break;
            }
        }
        return appliedTheme;
    };
    navbar.getCurrentTheme = function () {
        return app.settings.navbar.theme;
    };
    navbar.setTheme = function (theme) {
        if (theme) app.settings.navbar.theme = theme;
    };
    navbar.applyTheme = function () {
        var appliedTheme = this.getAppliedTheme();
        var currentTheme = this.getCurrentTheme();
        $navbar.removeClass(appliedTheme).addClass(currentTheme);
        $body.removeClass("theme-" + appliedTheme).addClass("theme-" + currentTheme);
    };

    function openSearch(e) {
        e.preventDefault();
        e.stopPropagation();
        $navbar.append('<div class="search-backdrop"></div>');
        $("#navbar-search").find("input").focus();
        $("#navbar-search").addClass("open");
        $(".search-backdrop").addClass("open");
    }

    function closeSearch(e) {
        e.preventDefault();
        e.stopPropagation();
        $("#navbar-search").removeClass("open");
        $(".search-backdrop").removeClass("open").remove();
    }
    window.app.navbar = navbar;
}(jQuery, window); +
function ($, window) {
    var $body = app.$body;
    var $sidebar = app.$sidebar;
    var $sidebarFold = $("#aside-fold");
    var $sidebarToggle = $("#aside-toggle");
    var sidebar = {};
    sidebar.init = function () {
        this.listenForEvents();
        this.userSet()
    };
    sidebar.listenForEvents = function () {
        var self = this;
        self.initScroll();
        self.toggleScroll();
        $body.on("mouseenter mouseleave", ".folded:not(.open) .has-submenu", function (e) {
            e.preventDefault();
            $body.find(".has-submenu").removeClass("open");
            $(this).addClass("open");
            $(this).find(".submenu").toggle().end().siblings().find(".submenu").hide();
            if (e.type == "mouseleave") {
                $body.find(".has-submenu").removeClass("open");
            }
        });
        $body.on("click", ".submenu-toggle", function (e) {
            e.preventDefault();
            // todo 
            if (!$(this).parents("#app-aside").hasClass("folded")) {
            //if (!$(this).parent().hasClass("open") && !$(this).parents("#app-aside").hasClass("folded")) {
                $(this).parent().toggleClass("open").find(".submenu").slideToggle(300, function () {
                    $(this).css("height", "inherit");
                }).end().siblings().removeClass("open").find(".submenu").slideUp(300)
            }
        });
        $body.on("click", ".childMenu-toggle", function (e) {
            e.preventDefault();
            $(this).parent().toggleClass("open").find(".childMenu").slideToggle(300).end().siblings().removeClass("open").find(
				".childMenu").slideUp(300);
        });
        $("#app-brand").on("click", function (e) {
            $(".sb-folded").removeClass("sb-folded");
            $(".app-aside").removeClass("folded")
        });
        $sidebarFold.on("click", function (e) {
            $body.find(".has-submenu").removeClass("open");
            $body.find(".has-submenu").find("ul.submenu").hide();
            e.preventDefault();
            self.fold();
            self.toggleScroll();
            if ($(".app-aside").hasClass("folded")) {
                $sidebarFold.attr("title", "展开导航");
                $(".aside-menu > li").each(function (i, item) {
                    if ($(item).hasClass('has-submenu')) {
                        var menuHeading = $(item).find('> a > .menu-text').text();
                        $(item).find('.submenu').first().prepend('<li class="menu-heading"><a>' + menuHeading + '</a></li>');
                    }
                });
            } else {
                $sidebarFold.attr("title", "收起导航");
                $("ul#aside-menu").find('.menu-heading').remove();
                $("ul#aside-menu").find("li.has-submenu:first").find("a.submenu-toggle:not(.menu-item)").trigger("click");
            }
        });
        $sidebarToggle.on("click", self.open);
        $body.on("click", ".aside-backdrop", self.close);
        $(window).on("load", function (e) {
            var ww = $(window).width();
            if (ww < 992 && app.$sidebar.hasClass("folded")) {
                app.$sidebar.removeClass("folded");
                app.$body.removeClass("sb-folded");
                sidebar.toggleScroll();
            } else if (ww >= 992 && app.settings.sidebar.folded) {
                app.$sidebar.addClass("folded");
                app.$body.addClass("sb-folded");
                sidebar.toggleScroll();
            }
        });
        // todo 
        $(window).resize(function () {
            var ww = $(window).width();
            if (ww < 992) {

            } else if (ww >= 992) {
                
            }
        });

    };
    sidebar.getAppliedTheme = function () {
        var appliedTheme = "",
			themes = app.settings.sidebar.themes,
			theme;
        for (theme in themes) {
            if ($sidebar.hasClass(themes[theme])) {
                appliedTheme = themes[theme];
                break;
            }
        }
        return appliedTheme;
    };
    sidebar.getCurrentTheme = function () {
        return app.settings.sidebar.theme;
    };
    sidebar.setTheme = function (theme) {
        if (theme) app.settings.sidebar.theme = theme;
    };
    sidebar.applyTheme = function () {
        $sidebar.removeClass(this.getAppliedTheme())
			.addClass(this.getCurrentTheme());
    };
    sidebar.fold = function () {
        $sidebarFold.toggleClass("is-active");
        $sidebar.toggleClass("folded");
        $body.toggleClass("sb-folded");
    };
    sidebar.open = function (e) {
        e.preventDefault();
        $sidebar.after('<div class="aside-backdrop"></div>');
        $sidebar.addClass("open");
        $sidebarToggle.addClass("is-active");
        $body.addClass("sb-open");
    };
    sidebar.close = function (e) {
        e.preventDefault();
        $sidebar.removeClass("open");
        $sidebarToggle.removeClass("is-active");
        $body.removeClass("sb-open").find(".aside-backdrop").remove();
    };
    sidebar.initScroll = function () {
        $("#aside-scroll-inner").slimScroll({
            height: $(window).height() - 130,
            position: "right",
            size: "5px",
            color: "#98a6ad",
            wheelStep: 5
        });
    };
    sidebar.toggleScroll = function () {
        var $scrollContainer = $("#aside-scroll-inner");
        if ($body.hasClass("sb-folded")) {
            $scrollContainer.css("overflow", "inherit").parent().css("overflow", "inherit");
            $scrollContainer.siblings(".slimScrollBar").css("visibility", "hidden");
        } else {
            $scrollContainer.css("overflow", "hidden").parent().css("overflow", "hidden");
            $scrollContainer.siblings(".slimScrollBar").css("visibility", "visible");
        }
    };
    sidebar.userSet = function () {
        var $aside_user = $(".aside-user")
        $aside_user.find("#skin-customizer").click(function () {
            $("#app-customizer").addClass("open")
        })
    };
    window.app.sidebar = sidebar;
}(jQuery, window); +
function ($, window) {
    var $body = app.$body;
    var $navbar = app.$navbar;
    var $navbar_menu = app.$sidebar.find("ul#aside-menu");
    var $navbar_tab = app.$navbar.find(".app-navbar-tabs");
    var $main = app.$main;
    var mainIndex = {};
    mainIndex.init = function () {
        $body.find("#app-main").height($(window).height() - $navbar.height());
        var fa_width = 485;
        $navbar_tab.width($(window).width() - fa_width);
        $(window).resize(function (e) {
            $body.find("#app-main").height($(window).height() - $navbar.height());
            $("#app-aside").find(".slimScrollDiv").height($(window).height() - 130);
            $("#app-aside").find(".aside-scroll-inner").height($(window).height() - 130);
            $navbar_tab.width($(window).width() - fa_width);
        });
        //if (localStorage.getItem("lockScreen") == 1) {
        //    mainIndex.lockScreen();
        //}
        this.loadLeftNav();
        this.loadToNav();
        this.loadUser();
    }
    mainIndex.loadLeftNav = function () {
        //debugger
        var menuData = $.indexData.authorizeMenu;
        if (menuData.length == 0) {
            jinkai.msg("您的权限不足，请联系管理员", "error");
            return false;
        }
        var menuTemplet = '';
        // todo 新增data-module
        $.each(menuData, function (i) {
            var menuRow = menuData[i];
            if (menuRow.ParentId == "0") {
                menuTemplet += '<li class="has-submenu">';
                menuTemplet += '	<a ' + (menuRow.UrlAddress == "#" ? "disabled" : "") +
					' class="menu-link submenu-toggle cursor-pointer ' + (menuRow.Type == 1 ? "" : "menu-item") + '" data-href="' +
					//menuRow.UrlAddress + '" data-target=' + menuRow.LinkTarget + ' data-id=' + menuRow.Id + '>';
                    menuRow.UrlAddress + '" data-target=' + menuRow.LinkTarget + ' data-id=' + menuRow.Id + ' data-module="' + menuRow.module + '">'; // todo
                menuTemplet += '		<span class="menu-icon"><i class="ico ' + menuRow.Icon + '"></i></span>';
                menuTemplet += '		<span class="menu-text foldable">' + menuRow.FullName + '</span>';
                menuTemplet += '		<span class="menu-caret"><i class="fa fa-angle-right"></i></span>';
                menuTemplet += '	</a>';
                var childNodes = jinkai.jsonFind(menuData, function (v) {
                    return v.ParentId == menuRow.Id
                });
                if (childNodes.length > 0) {
                    menuTemplet += '<ul class="submenu">';
                    $.each(childNodes, function (j) {
                        var menuRowChild = childNodes[j];
                        menuTemplet += '<li>';
                        var subChildNodes = jinkai.jsonFind(menuData, function (v) {
                            return v.ParentId == menuRowChild.Id
                        });
                        if (subChildNodes.length > 0) {
                            menuTemplet += '<a class="childMenu-toggle cursor-pointer"><i class="ico ' + menuRowChild.Icon + '"></i>' +
								menuRowChild.FullName + ''
                            menuTemplet += '<span class="menu-caret"><i class="fa fa-angle-right"></i></span>';
                            menuTemplet += '</a>';
                            menuTemplet += '<ul class="childMenu">';
                            $.each(subChildNodes, function (k) {
                                var menuRowSubChild = subChildNodes[k];
                                menuTemplet += '<li><a ' + (menuRowSubChild.UrlAddress == "#" ? "disabled" : "") + ' data-href="' +
									menuRowSubChild.UrlAddress + '" data-target=' + menuRowSubChild.LinkTarget +
									' class="menu-item" data-id=' + menuRowSubChild.Id + ' data-module="' + menuRowSubChild.module + '"><i style="margin-left:20px;" class="ico ' + menuRowSubChild.Icon +
									'"></i>' + menuRowSubChild.FullName + '</a></li>';
                            });
                            menuTemplet += '</ul>';
                        } else {
                            menuTemplet += '<a ' + (menuRowChild.UrlAddress == "#" ? "disabled" : "") + ' data-href="' + menuRowChild
								.UrlAddress + '" data-target=' + menuRowChild.LinkTarget + ' data-module="' + menuRowChild.module + '" class="menu-item" data-id=' +
								menuRowChild.Id + '><i class="ico ' + menuRowChild.Icon + '"></i>' + menuRowChild.FullName + '</a>'
                        }
                        menuTemplet += '</li>';
                    });
                    menuTemplet += '</ul>';
                }
                menuTemplet += '</li>';
            }
        });
        $navbar_menu.html(menuTemplet);
        $navbar_menu.find("a.menu-item").find("span.menu-caret").remove();
        $navbar_menu.find("a.menu-item:not([data-href='#'])").on("click", mainIndex.tabAdd);
        $navbar_menu.find("li.has-submenu:first").find("a.submenu-toggle:not(.menu-item)").trigger("click");
        $navbar_tab.find("ul").find("a.menu-tab").click(function () {
            var dataId = $(this).attr("data-id");
            mainIndex.tabActive(dataId);
        });
    };
    mainIndex.loadToNav = function () {
        //$("#top-nav").find("#navbar-news-open").click(function () {
        //    jinkai.openContent({
        //        title: "公告详情",
        //        width: "750px",
        //        height: "500px",
        //        loadComplete: function (name) {
        //            jinkai.ajax({
        //                async: false,
        //                type: "GET",
        //                url: jinkai.toUrl("/ajax/news.ashx?action=recently"),
        //                param: {},
        //                success: function (result) {
        //                    _top.$("#" + name).html('<pre style="margin: 5px;padding: 10px;"><code>' + result.data.content + '</code></pre>').height(430)
        //                    _top.$("#" + name).prev(".layui-layer-title").html(result.data.title + "<br/>" + '<label  class="media-meta m-r-sm">' + result.data.author + '</label><label class="media-meta">' + '发布日期：' + result.data.newsDate + '</label>');
        //                    _top.$("#" + name).prev(".layui-layer-title").css({ "padding-top": "15px", "height": "auto", "line-height": "inherit" });

        //                },
        //                error: function (result) {
        //                    jinkai.msg(result.msg, "error");
        //                },
        //            })

        //        }
        //    });
        //});
        $("#top-nav").find("#navbar-news-open").click(function () {
            jinkai.openWindow({
                title: "过期案件信息",
                url: "/pages/petition/overdue.html",
                width: "960px",
                height: "550px"
            })
        });
        // 点击头部帮助文档
        $("#top-nav").find("#navbar-help-open").click(function () {
            $("#hidBlankHref").attr("href", "/data/help/帮助文档.pdf");
            document.getElementById("hidBlankHref").click();
            return false;
        });
        // 点击头部下载中心
        $("#top-nav").find("#navbar-download-open").click(function () {
            layer.open({
                type: 1,
                shade: false,
                title: false, //不显示标题
                area: ['450px', '280px'], //宽高
                content: $('#app-download')             
            });
            //layer.msg("ddddddddddddd");
            return false;
        });
        // 点击头部清除缓存
        $("#top-nav").find("#navbar-clear-open").click(function () {
            jinkai.ajax({
                type: "POST",
                dataType: "json",
                url: "/ajax/utils.ashx?action=clear",
                success: function (result) {

                    if (result && result.status == 200) {
                        jinkai.msg("清除缓存成功", "success");
                        setTimeout(function () {
                            jinkai.reload();
                            //window.location.href = '/Index.html';
                        }, 500);

                    } else {                        
                        jinkai.reload();
                    }
                },
                error: function () {
                    jinkai.reload();
                    //window.location.href = '/Index.html';
                }
            })
            return false;
        });

        this.tabContextmenu();
    };
    mainIndex.loadUser = function () {
        var $sidebar = $("#app-aside");
        //个人资料
        $sidebar.find("#btn_userInfo").click(function () {
            jinkai.openTab({
                id: "userSetting",
                text: "个人信息",
                url: "/pages/user/index.html",
                img: "fa fa-user",
            });
        });
        //皮肤设置
        $sidebar.find("#btn_skin").click(function () {
            jinkai.openContent({
                top: false,
                title: "皮肤设置",
                width: "630px",
                height: "450px",
                loadComplete: function (name) {
                    var templetHtml = "";
                    templetHtml += '<div class="theme-set-content">';
                    templetHtml += '    <ul class="theme-set-list">';
                    templetHtml += '        <li data-value="default">';
                    templetHtml +=
						'            <div class="theme-box"><div class="theme-img" style="background-color:#363d4d;">经典灰</div><div class="theme-selected"><i class="iconfont icon-check"></i></div></div>';
                    templetHtml += '        </li>';
                    templetHtml += '        <li data-value="orange">';
                    templetHtml +=
						'            <div class="theme-box"><div class="theme-img" style="background-color:#E67D21;">青春黄</div><div class="theme-selected"><i class="iconfont icon-check"></i></div></div>';
                    templetHtml += '        </li>';
                    templetHtml += '        <li data-value="red">';
                    templetHtml +=
						'            <div class="theme-box"><div class="theme-img" style="background-color:#e76e66;">活力红</div><div class="theme-selected"><i class="iconfont icon-check"></i></div></div>';
                    templetHtml += '        </li>';
                    templetHtml += '        <li data-value="green">';
                    templetHtml +=
						'            <div class="theme-box"><div class="theme-img" style="background-color:#1f9885;">荷叶绿</div><div class="theme-selected"><i class="iconfont icon-check"></i></div></div>';
                    templetHtml += '        </li>';
                    templetHtml += '        <li data-value="blue">';
                    templetHtml +=
						'            <div class="theme-box"><div class="theme-img" style="background-color:#3e8ecd;">宝石蓝</div><div class="theme-selected"><i class="iconfont icon-check"></i></div></div>';
                    templetHtml += '        </li>';
                    templetHtml += '        <li data-value="purple">';
                    templetHtml +=
						'            <div class="theme-box"><div class="theme-img" style="background-color:#9264d1;">紫罗兰</div><div class="theme-selected"><i class="iconfont icon-check"></i></div></div>';
                    templetHtml += '        </li>';
                    templetHtml += '    </ul>';
                    templetHtml += '</div';
                    var $templetHtml = $(templetHtml);
                    $templetHtml.find("li").click(function () {
                        $templetHtml.find("li").removeClass("active");
                        $(this).addClass("active");
                        var theme = $(this).attr("data-value");
                        $body.removeClass("theme-" + app.settings.navbar.theme).addClass("theme-" + theme);
                        app.settings.navbar.theme = theme;
                    });
                    _top.$("#" + name).html($templetHtml);
                    _top.$("#" + name).find("li[data-value='" + app.settings.navbar.theme + "']").addClass("active");
                },
                callBack: function (name) {
                    app.settings.navbar.theme = _top.$("#" + name).find("li.active").attr("data-value");
                    app.saveSettings();
                    jinkai.openClose();
                }
            });
        });
        //关于平台
        $sidebar.find("#btn_about").click(function () {
            jinkai.openContent({
                top: false,
                title: "关于平台",
                width: "400px",
                height: "250px",
                loadComplete: function (name) {
                    var $template = $(
						'<div style="width:350px;margin: 30px;"><div class="col-sm-6" style="width: 37%;float: left;">' +
                        '<img src="/logo.jpg" style="width: 95px; height: 95px;text-align: center;border-radius: 10px;border:none;margin-bottom:0px;background-color: #007ed3;color: #fff;"></i></div>' +
                        '<div class="col-sm-6" style="width: 63%;float: left;"><p style="font-weight:bold;"><a href="#" target="_blank">网上信访</a></p><p>版本：V1.0.0</p><p>作者：金开科技</p><p>大连金开科技有限公司</p></div><div class="col-sm-12" style="float: left;margin-top: 20px;">我们致力于打造更加优秀的企业团队为客户服务！</div></div>'
					);
                    _top.$("#" + name).html($template);
                }
            });
        });
        //锁住屏幕
        $sidebar.find("#btn_lockScreen").click(function () {
            mainIndex.lockScreen();
        });
        //退出系统
        $sidebar.find("#btn_logout").click(function () {
            jinkai.confirm({
                content: "您确定要退出应用程序吗？",
                callBack: function () {
                    jinkai.ajax({
                        async: true,
                        url: jinkai.toUrl("/ajax/admin.ashx?action=loginOut"),
                        complete: function () {
                            top.isLogOut = true;
                            top.location.href = "/Login.html";
                            localStorage.removeItem("token");
                        }
                    });
                }
            });
        });
    }
   
    mainIndex.tabAdd = function (options) {
        var defaults = {
            id: $(this).attr("data-id"),
            url: $(this).attr("data-href"),
            img: $(this).find("i").attr("class"),
            text: $(this).text(),
            module: $(this).attr("data-module")
        };
        var options = $.extend(defaults, options);
        options.url = options.url;
        var openTarget = $(this).attr("data-target");
        if (openTarget == "_blank") {
            var module = options.module;
            console.log(module);
            // 视频监控需要使用IE浏览器打开
            if (module == "timeVideo" || module == "historyVideo") {
                window.location.href = "openIE:" + options.url; // todo
            } else {
                window.open(options.url, "_blank");
            }

           
        } else {
            if ($navbar_tab.find("ul li").find("#menu-tab" + options.id + "").length == 0) {
                var $tab = $('<li title="双击可关闭" class="active"><a class="menu-tab cursor-pointer" id="menu-tab' + options.id +
					'" data-id="' + options.id + '" data-url="' + options.url + '"><i class="' + options.img + '"></i><span>' +
					options.text + '</span></a></li>');
                $navbar_tab.find("ul li").removeClass("active");
                $navbar_tab.find("ul").append($tab);
                $tab.find("a.menu-tab").click(function () {
                    mainIndex.tabActive($(this).attr("data-id"));
                });
                $tab.find("a.menu-tab").dblclick(function () {
                    mainIndex.tabClose($(this).attr("data-id"))
                })
                var $iframe = $('<iframe class="app-main-iframe" id="iframe' + options.id + '" name="iframe' + options.id +
					'" data-id="' + options.id + '" width="100%" height="100%" src="' + options.url + '" frameborder="0" ></iframe>'
				);
                // $iframe.load(function() {
                // 	$(this).thisWindow().$(".ajax-loader").fadeOut();
                // });

                $iframe.on("load", function () {
                    $(this).thisWindow().$('.ajax-loader').fadeOut();
                });
                $main.find("div.app-main-iframe,iframe.app-main-iframe").hide();
                $main.append($iframe);
                mainIndex.tabScroll();
            } else {
                $navbar_tab.find("ul li").removeClass("active");
                $navbar_tab.find("ul li").find("#menu-tab" + options.id + "").parents("li").addClass("active");
                $main.find("#iframe" + options.id + "").show().siblings(".app-main-iframe").hide();
            }
            mainIndex.setModuleId(options.url);
        }
    };
    mainIndex.tabActive = function (dataId) {
        $navbar_tab.find('ul li').removeClass('active');
        $navbar_tab.find('ul li').find("#menu-tab" + dataId + "").parents('li').addClass('active');
        $main.find("#iframe" + dataId + "").show().siblings('.app-main-iframe').hide();
        mainIndex.setModuleId($main.find("#iframe" + dataId + "").attr("src"));
        mainIndex.tabScroll();
    };
    mainIndex.tabClose = function (dataId) {
        var index = $navbar_tab.find('ul li').find("#menu-tab" + (dataId == undefined ? $navbar_tab.find('ul li.active').find(
			"a").attr("data-id") : dataId) + "").parents('li').index();
        if (index == 0) {
            return false;
        }
        $main.find("#iframe" + dataId + "").remove();
        $navbar_tab.find('ul li').find("#menu-tab" + dataId + "").parents('li').remove();
        if (index == $navbar_tab.find('ul li').length) {
            var dataId = $navbar_tab.find('ul li:eq(' + (index - 1) + ')').find('a.menu-tab').attr("data-id");
            mainIndex.tabActive(dataId);
        } else {
            var dataId = $navbar_tab.find('ul li:eq(' + (index) + ')').find('a.menu-tab').attr("data-id");
            mainIndex.tabActive(dataId);
        }
    };
    mainIndex.tabScroll = function () {
        var ul_width = $navbar_tab.find("ul").outerWidth();
        var tabs_width = $navbar_tab.outerWidth();
        if (ul_width > tabs_width) {
            $navbar_tab.find("ul").animate({
                marginLeft: 0 - (ul_width - tabs_width) + 'px'
            }, "fast");
        } else {
            $navbar_tab.find("ul").animate({
                marginLeft: '0px'
            }, "fast");
        }
        if ($navbar_tab.find("ul").find("li").length == 1) {
            $navbar_tab.find("ul").animate({
                marginLeft: '0px'
            }, "fast");
        }
    };
    mainIndex.tabContextmenu = function () {
        $navbar_tab.contextmenu();
        $("#app-navbar-contextmenu").find("li a").click(function () {
            var type = $(this).attr("data-type");
            switch (type) {
                case "reloadCurrent":
                    jinkai.thisTab().jinkai.reload();
                    break;
                case "closeCurrent":
                    var dataId = $navbar_tab.find('ul li.active').find("a").attr("data-id");
                    mainIndex.tabClose(dataId);
                    break;
                case "closeAll":
                    $navbar_tab.find("ul li").not(":first").each(function () {
                        var dataId = $(this).find("a").attr("data-id");
                        mainIndex.tabClose(dataId);
                    });
                    break;
                case "closeOther":
                    $navbar_tab.find("ul li").not(":first").not(".active").each(function () {
                        var dataId = $(this).find("a").attr("data-id");
                        mainIndex.tabClose(dataId);
                    });
                    break;
                case "fullScreen":
                    var de = document.documentElement;
                    if (de.requestFullscreen) {
                        de.requestFullscreen();
                    } else if (de.mozRequestFullScreen) {
                        de.mozRequestFullScreen();
                    } else if (de.webkitRequestFullScreen) {
                        de.webkitRequestFullScreen();
                    }
                    break;
                case "tabPreview":
                    mainIndex.tabPreview();
                    break;
                default:

            }
        })
    };
    mainIndex.tabPreview = function () {
        var $templet = $(
			'<div id="app-preview"><ul class="app-navbar-tabs-preview-panel" style="display:block"></ul><div class="app-navbar-tabs-preview-close"><a class="close-btn">关闭全部窗口</a></div><div class="app-navbar-tabs-preview-shade"></div></div>'
		);
        $templet.find(".app-navbar-tabs-preview-panel").css({
            width: $(window).width() - 30,
            height: $(window).height() - 120
        });
        //debugger
        // 头部导航栏

        //console.log(app.$navbar.find(".app-navbar-tabs").find("li").length);
        app.$navbar.find(".app-navbar-tabs").find("li").each(function (i) {
            if (i > 0) {
                var dataId = $(this).find("a").attr("data-id");
                var dataUrl = $(this).find("a").attr("data-url");
                var dataName = $(this).find("a span").html();
                var active = $(this).attr("class");
                var $li = $('<li data-id="' + dataId +
					'" class="animatedFast zoomIn"><div class="current-box"><span class="current">当前页面</span></div><div class="title">' +
					dataName +
					'</div><span class="glyphicon glyphicon-remove"></span><iframe class="app-preview-iframe" frameborder="no" border="0" marginwidth="0" marginheight="0" src="' +
					dataUrl + '"></iframe></li>');
                //$li.find("iframe").load(function () {
                //    $(this).thisWindow().$('.ajax-loader').fadeOut();
                //});
                $li.find(".app-preview-iframe").on("load",function () {
                    $(this).thisWindow().$('.ajax-loader').fadeOut();
                });

                if (active == "active") {
                    $li.find("span.current").show();
                }
                $li.find("div.current-box").click(function () {
                    mainIndex.tabActive(dataId);
                });
                $li.find("span.glyphicon-remove").click(function () {
                    mainIndex.tabClose(dataId);
                });
                $templet.find("ul.app-navbar-tabs-preview-panel").append($li);
            }
        });
        $templet.find('.close-btn').click(function () {
            app.$navbar.find(".app-navbar-tabs").find("ul li").not(":first").each(function () {
                var dataId = $(this).find("a").attr("data-id");
                mainIndex.tabClose(dataId);
            });
            $(this).parents('#app-preview').remove();
        });
        $templet.find('.app-navbar-tabs-preview-panel').click(function () {
            $(this).parents('#app-preview').remove();
        });
        // todo
        //$templet.find('.app-navbar-tabs-preview-panel').show();
       
        //console.log($templet);
        $("body").append($templet);
    }
    mainIndex.lockScreen = function () {
        var userName = $("#aside-user").find("small").html();
        var userAvatar = $("#aside-user").find(".img-responsive").attr("src");
        $(".lock-screen").remove();
        var $templet = $('<div class="lock-screen"><div class="lock-screen-img" ><img src="' + userAvatar +
			'" /></div ><div style="margin: 0 auto;width: 210px;"><h4>' + userName +
			'</h4><div class="input-group"> <input type="password" class="form-control p-r-lg input-sm" placeholder="输入密码解锁"><span class="input-group-btn"><button class="btn btn-primary btn-sm" type="button"><i class="ace-icon iconfont icon-lock-open"></i></button></span></div></div></div>'
		)
        $templet.find("button").click(function () {
            unlock();
        });
        $templet.find("input").keydown(function (e) {
            if (e.keyCode == 13) {
                unlock();
            }
        });

        function unlock() {
            var notyf = new Notyf({
                delay: 4000
            });
            var $password = $templet.find("input");
            if ($password.val() == "") {
                notyf.error("请输入解锁密码");
                $password.focus();
                return false;
            }
            // jinkai.ajax({
            //     async: true,
            //     url: jinkai.toUrl("/api/Login/TryPassword"),
            //     param: { account: $.trim($.indexData.userProvider.userAccount), password: $.md5($.trim($password.val())) },
            //     success: function (result) {
            //         $("body >*").removeClass("lock-screen-hidden");
            //         $("#index-loader").fadeOut();
            //         $templet.remove();
            //         localStorage.removeItem("lockScreen");
            //     },
            //     error: function (result) {
            //         notyf.error("您输入的密码不正确");
            //         $password.val("");
            //         $password.focus();
            //     },
            //     beforeSend: function () {
            //         jinkai.loading(true);
            //     },
            //     complete: function () {
            //         jinkai.loading(false);
            //     },
            // });
        }
        $templet.find("input").focus();
        $("body").append($templet);
        $("body >*").addClass("lock-screen-hidden");
        localStorage.setItem("lockScreen", 1);
    }
    mainIndex.setModuleId = function (url) {
        //功能id  todo
        //var item = $.indexData.authorizeUrl[url];
        //if (item) {
        //    jinkai.cookie("moduleId", item.id);
        //}
    }
    window.app.mainIndex = mainIndex;
}(jQuery, window); +
// 初始化数据
function ($, window) {
    jinkai.cookie("moduleId", null);
    
    $.indexData = {
        init: function () {
            jinkai.ajax({
                async: false, // 同步
                url: jinkai.toUrl("/ajax/admin.ashx?action=index"),//TODO
                success: function (result) {
                    $.indexData = result.data;
                    //$.indexData = result;
                    window.app.mainIndex.init();
                    var userProvider = result.data.userProvider;
                    if (userProvider != null) {
                        var text = userProvider.userName + "/" + userProvider.userAccount;
                        $("#userName").html(text).attr("title", text).attr("data-value", userProvider.userId);

                        if (userProvider.avatar != "") {
                            $("#userHead").attr("src", jinkai.toUrl(userProvider.avatar));
                        }
                       
                        //if (userProvider.prevLogin == 1) {
                        //    //右下角提示上次登录信息
                        //    jinkai.openContent({
                        //        top: false,
                        //        id: "prevLogin",
                        //        title: "上次登录信息",
                        //        width: "260px",
                        //        height: "160px",
                        //        offset: "rb",
                        //        shade: 0,
                        //        time: 5000,
                        //        loadComplete: function (name) {
                        //            var templetHtml = "<div style='padding: 1rem;padding-left: 20px;padding-bottom: 0px;'>";
                        //            templetHtml += '<p>时间：' + jinkai.toDate(userProvider.prevLoginTime, "yyyy-MM-dd HH:mm") + '</p>';
                        //            templetHtml += '<p>地点：' + '辽宁省大连市' + '</p>';
                        //            templetHtml += '<p>&nbsp;&nbsp;&nbsp;&nbsp;IP：' + userProvider.prevLoginIPAddress + '</p></div>';
                        //            _top.$("#" + name).html(templetHtml);
                        //        }
                        //    });
                        //}
                    }
                    //if (localStorage.getItem("lockScreen") == 1) {
                    //    window.app.mainIndex.lockScreen();
                    //}
                    setTimeout(function () {
                        $("#index-loader").fadeOut();
                    }, 200);
                }
            });
        }
    }
    window.app.indexData = $.indexData;
}(jQuery, window); +
function ($, window) {
    // 无任何操作
    window.app.init();
    // 主要监听点击头部查询菜单按钮
    window.app.navbar.init();
    window.app.sidebar.init();
    window.app.indexData.init();
}(jQuery, window);
