(function ($) {
    var module = 'oldPetition';
    $.thisPage = {
        right: true,//上传权限
        filePdfList: {}, // 保存上传word信息 
        uploaderPdf: null,
        pdfList: [],//回显的PDF信息
        init: function () {
            this.initEvent();
            this.uploaderGet();
        },
        uploaderGet: function () {

            jinkai.ajax({
                url: jinkai.toUrl("/ajax/oldPetition.ashx"),
                param: {
                    action: "getUpload"
                },
                success: function (result) {
                    $.thisPage.pdfList = result.data.listDocs;
                    $.thisPage.addPdfDiv(result.data.listDocs);
                }
            });

        },
        initEvent: function () {

            $(".panel-body").on("click", "img", function () {
                var data = $(this).attr("data");
                window.open(data)
            })
        },
        addPdfDiv: function (items) {
            $.each(items, function (index, item) {
                $.thisPage.filePdfList[item.id] = item;
            })
        },
        addPdfDivOne: function (data, $list) {
            $.thisPage.filePdfList[data.id] = data;
            var $li = $list.find("li");
            $li.attr('data-pid', data.id);
            var $img = $li.find("p.imgWrap").find("img");
            $img.attr('alt', data.name);
            $img.attr('data', data.url);
        },
        getPdfFiles: function () {
            return this.filePdfList;
        },
        uploaderSet: function ($queue, fiels, type) {
            for (var i = 0; i < fiels.length; i++) {
                (function () {
                    var href = fiels[i].url;
                    var src;
                    if (type == 'word') {
                        src = "/images/icon_word.png";
                    }

                    var $li = $('<li id="' + fiels[i].id + '"  data-pid="' + fiels[i].id + '">' +
                                '<p class="title">' + fiels[i].name + '</p>' +
                                '<p class="imgWrap"><img src="' + src + '" data="' + href + '"/></p>' +
                                '</li>');
                    if ($.thisPage.right) {
                        var $btns = $('<div class="file-panel">\
                                        <span class="cancel">删除</span>\
                                   </div>').appendTo($li);
                        $li.on('mouseenter', function () {
                            $btns.stop().animate({ height: 30 });
                        });

                        $li.on('mouseleave', function () {
                            $btns.stop().animate({ height: 0 });
                        });
                        $btns.on('click', 'span', function () {
                            var id = $li.data("pid");
                            $li.off().find('.file-panel').off().end().remove();
                            if (type == 'word') {
                                $.thisPage.filePdfList[id].status = 1;
                            }

                        });
                    }

                    $li.append('<span class="Success"></span>');

                    $li.appendTo($queue);


                })(i)
            }
        }
    };

    //上传PDF
    $.fn.upfilePdf = function () {
        var $wrap = $('#uploaderPdf'),

           // pdf容器
           $queue = $('<ul class="filelist"></ul>')
               .appendTo($wrap.find('.queueList')),

           // 状态栏，包括进度和控制按钮
           $statusBar = $wrap.find('.statusBar'),

           // 文件总体选择信息。
           $info = $statusBar.find('.info'),

           // 上传按钮
           //$upload = $wrap.find('.uploadBtn'),

           // 没选择文件之前的内容。
           $placeHolder = $wrap.find('.placeholder'),
            // 添加的文件数量
            fileCount = 0,

            // 添加的文件总大小
            fileSize = 0,

            ratio = window.devicePixelRatio || 1,

            //缩略图大小
            thumbnailWidth = 100 * ratio,
            thumbnailHeight = 100 * ratio;

        //判断权限
        $.thisPage.right = jinkai.verifyRight(module + "_Upload");//上传权限
        var pick = $.thisPage.right ? { id: '#filePickerPdf', label: '点击选择word文件' } : {};
        var pick1 = $.thisPage.right ? { id: '#filePickerPdf2', label: '继续添加' } : {};

        var uploader = WebUploader.create({
            auto: true, // 选完文件后，是否自动上传 
            swf: '/assets/webuploader/Uploader.swf', // swf文件路径 
            server: '/ajax/oldPetition.ashx?action=upload', // 文件接收服务端 
            pick: pick, // 选择文件的按钮。可选 
            // 只允许选择word文件。 
            accept: {
                title: 'Applications',
                extensions: 'doc,docx',
                mimeTypes: 'application/msword,application/vnd.openxmlformats-officedocument.wordprocessingml.document'
            },
            dnd: '.queueList',
            // 禁掉全局的拖拽功能。这样不会出现文件拖进页面的时候，把文件打开。
            disableGlobalDnd: true,
            threads: 20,
            fileNumLimit: 20,                              //最大上传数量为20
            fileSingleSizeLimit: 20 * 1024 * 1024,         //限制上传单个文件大小20M
            fileSizeLimit: 400 * 1024 * 1024,              //限制上传所有文件大小400M
        });
        // 添加“添加文件”的按钮，
        uploader.addButton(pick1);

        //加载现有word
        if ($.thisPage.pdfList.length > 0) {
            $placeHolder.addClass('element-invisible');
            $statusBar.show();
            $.thisPage.uploaderSet($queue, $.thisPage.pdfList, "word");
        }

        uploader.on("fileQueued", function (file) {
            fileCount++;
            fileSize += file.size;

            if (fileCount === 1) {
                $placeHolder.addClass('element-invisible');
                $statusBar.show();
                uploader.refresh();
            }
            var $li = $('<li id="' + file.id + '"  data-pid="' + file.id + '">' +
                   '<p class="title">' + file.name + '</p>' +
                   '<p class="imgWrap"></p>' +
                   '</li>'),
                   $btns = $('<div class="file-panel">' +
                    '<span class="cancel">删除</span>' +
                    '</div>').appendTo($li),
                $wrap = $li.find('p.imgWrap'),
                $info = $('<p class="error"></p>');


            $wrap.text('预览中');
            uploader.makeThumb(file, function (error, src) {

                if (error) {
                    if (file.ext == "docx" || file.ext == "doc") {
                        var img = $('<img src="/images/icon_word.png">');
                    } else {
                        $wrap.text('不能预览');
                        return;
                    }
                }

                $wrap.empty().append(img);

            }, thumbnailWidth, thumbnailHeight);

            $li.on('mouseenter', function () {
                $btns.stop().animate({ height: 30 });
            });

            $li.on('mouseleave', function () {
                $btns.stop().animate({ height: 0 });
            });
            $btns.on('click', 'span', function () {
                //var index = $(this).index();
                var id = $li.data("pid");
                $li.off().find('.file-panel').off().end().remove();
                $.thisPage.filePdfList[id].status = 1;
            });
            $li.appendTo($queue);

        });

        uploader.on("uploadSuccess", function (file, result) {
            var $li = $('#' + file.id);
            if (200 != result.status) {
                jinkai.msg("上传word失败！", "error")
            } else {
                $li.append('<span class="Success"></span>');
                $.thisPage.addPdfDivOne(result.data, $queue);
            }
        });
        uploader.on("uploadError", function (file, a, b, c) {
            var $li = $('#' + file.id),
                $info = $('<p class="error"></p>');
            $info.text("上传出错！").appendTo($li);
        });
        uploader.on("error", function (type, file) {
            if (type == "Q_EXCEED_NUM_LIMIT") {
                text = "超出最大文件数";
            } else if (type == "Q_TYPE_DENIED") {
                jinkai.msg("文件格式不正确", "warning");
            } else if (type == "F_EXCEED_SIZE") {
                jinkai.msg("文件大小不能超过20M", "warning");
            } else {
                jinkai.msg("添加文件失败", "warning");
            }
        });
        return uploader;
    };

    $(function () {
        $.thisPage.init();
    });
})(jQuery)