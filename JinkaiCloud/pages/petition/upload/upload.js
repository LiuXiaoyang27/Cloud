(function ($) {
    // 当domReady的时候开始初始化
    $(function () {

        $.page.files();

        var $wrap = $('#uploader'),

            // 文件容器 
            $queue = $('<ul class="filelist"></ul>')
                .appendTo($wrap.find('.queueList')),

            // 状态栏，包括进度和控制按钮
            $statusBar = $wrap.find('.statusBar'),

            // 文件总体选择信息。
            $info = $statusBar.find('.info'),

            // 上传按钮
            $upload = $wrap.find('.uploadBtn'),

            // 没选择文件之前的内容。
            $placeHolder = $wrap.find('.placeholder'),

            $progress = $statusBar.find('.progress').hide(),

            // 添加的文件数量
            fileCount = 0,

            // 添加的文件总大小
            fileSize = 0,

            listFileCount = 0,
            // 优化retina, 在retina下这个值是2
            ratio = window.devicePixelRatio || 1,

            // 缩略图大小
            thumbnailWidth = 110 * ratio,
            thumbnailHeight = 110 * ratio,

            // 可能有pedding, ready, uploading, confirm, done.
            state = 'pedding',

            // 所有文件的进度信息，key为file id
            percentages = {},
            // 判断浏览器是否支持文件的base64
            isSupportBase64 = (function () {
                var data = new Image();
                var support = true;
                data.onload = data.onerror = function () {
                    if (this.width != 1 || this.height != 1) {
                        support = false;
                    }
                }
                data.src = "data:image/gif;base64,R0lGODlhAQABAIAAAAAAAP///ywAAAAAAQABAAACAUwAOw==";
                return support;
            })(),

            // 检测是否已经安装flash，检测flash的版本
            flashVersion = (function () {
                var version;

                try {
                    version = navigator.plugins['Shockwave Flash'];
                    version = version.description;
                } catch (ex) {
                    try {
                        version = new ActiveXObject('ShockwaveFlash.ShockwaveFlash')
                                .GetVariable('$version');
                    } catch (ex2) {
                        version = '0.0';
                    }
                }
                version = version.match(/\d+/g);
                return parseFloat(version[0] + '.' + version[1], 10);
            })(),

            supportTransition = (function () {
                var s = document.createElement('p').style,
                    r = 'transition' in s ||
                            'WebkitTransition' in s ||
                            'MozTransition' in s ||
                            'msTransition' in s ||
                            'OTransition' in s;
                s = null;
                return r;
            })(),

            // WebUploader实例
            uploader;

        if (!WebUploader.Uploader.support('flash') && WebUploader.browser.ie) {

            // flash 安装了但是版本过低。
            if (flashVersion) {
                (function (container) {
                    window['expressinstallcallback'] = function (state) {
                        switch (state) {
                            case 'Download.Cancelled':
                                layer.msg('您取消了更新！')
                                break;

                            case 'Download.Failed':
                                layer.msg('安装失败')
                                break;

                            default:
                                layer.msg('安装已成功，请刷新！');
                                break;
                        }
                        delete window['expressinstallcallback'];
                    };

                    var swf = './expressInstall.swf';
                    // insert flash object
                    var html = '<object type="application/' +
                            'x-shockwave-flash" data="' + swf + '" ';

                    if (WebUploader.browser.ie) {
                        html += 'classid="clsid:d27cdb6e-ae6d-11cf-96b8-444553540000" ';
                    }

                    html += 'width="100%" height="100%" style="outline:0">' +
                        '<param name="movie" value="' + swf + '" />' +
                        '<param name="wmode" value="transparent" />' +
                        '<param name="allowscriptaccess" value="always" />' +
                    '</object>';

                    container.html(html);

                })($wrap);

                // 压根就没有安转。
            } else {
                $wrap.html('<a href="http://www.adobe.com/go/getflashplayer" target="_blank" border="0"><img alt="get flash player" src="http://www.adobe.com/macromedia/style_guide/images/160x41_Get_Flash_Player.jpg" /></a>');
            }

            return;
        } else if (!WebUploader.Uploader.support()) {
            layer.msg('Web Uploader 不支持您的浏览器！');
            return;
        }

        // 实例化
        uploader = WebUploader.create({
            pick: {
                id: '#filePicker',
                label: '点击选择文件'
            },
            formData: {
                uid: 123
            },
            dnd: '.queueList',
            paste: '#uploader',
            swf: '/assets/webuploader/Uploader.swf',     // swf文件路径
            chunked: false,
            chunkSize: 512 * 1024,
            // 上传接口地址 TODO
            server: '/ajax/upload.ashx?action=uploadPetition',
            // runtimeOrder: 'flash',

            // 接收类型 TODO
            accept: {
                title: 'Images',
                extensions: 'bmp,pdf,jpg,text,png,gif,mp4,avi,rmvb,ts,doc,xls,docx,xlsx',
                //mimeTypes: 'image/*,audio/*,application/* '
            },
            duplicate :false,
            // 禁掉全局的拖拽功能。这样不会出现文件拖进页面的时候，把文件打开。
            disableGlobalDnd: true,
            fileNumLimit: 20, // 20个
            fileSizeLimit: 400 * 1024 * 1024,    // 400M
            fileSingleSizeLimit: 20 * 1024 * 1024    // 20M
        });

        // 拖拽时不接受 js, txt 文件。
        //uploader.on('dndAccept', function (items) {
        //    debugger;
        //    var denied = false,
        //        len = items.length,
        //        i = 0,
        //        // 修改js类型
        //        //unAllowed = 'text/plain;application/javascript ';
        //        // 拖拽时不接受 js, txt 文件。 TODO
        //        unAllowed = 'text/plain';
        //    for (; i < len; i++) {
        //        // 如果在列表里面
        //        if (~unAllowed.indexOf(items[i].type)) {
        //            denied = true;
        //            break;
        //        }
        //    }

        //    return !denied;
        //});

        uploader.on('dialogOpen', function () {
            //console.log('here');
        });

        // uploader.on('filesQueued', function() {
        //     uploader.sort(function( a, b ) {
        //         if ( a.name < b.name )
        //           return -1;
        //         if ( a.name > b.name )
        //           return 1;
        //         return 0;
        //     });
        // });

        // 添加“添加文件”的按钮，
        uploader.addButton({
            id: '#filePicker2',
            label: '继续添加'
        });

        uploader.on('ready', function () {
            window.uploader = uploader;
        });


        //加载现有图片
        if ($.page.listDocs.length > 0) {
            $placeHolder.addClass('element-invisible');
            $statusBar.show();

            $info.hide();
            initUpload();
        }

        function initUpload() {

            var files = $.page.listDocs;
            //$info.html("已上传" + files.length + "个文件");
            listFileCount = files.length;
            for (var i = 0; i < files.length; i++) {
                (function () {
                    var href = files[i].url;
                    var src = href;
                    var type = files[i].type;
                    switch(type){
                        case "pdf":
                            src = "/images/pdf.png";
                            break;
                        case "docx":
                        case "doc":
                            src = "/images/icon_word.png";
                            break;
                        case "xlsx":
                        case "xls":
                            src = "/images/icon_excel.png";
                            break;
                        case "bmp":
                        case "mp4":
                        case "avi":
                        case "rmvb":
                        case "ts":
                            src = "/images/video.png";
                            break;
                    }
                    var $li = $('<li id="' + files[i].id + '"  data-pid="' + files[i].id + '">' +
                                '<p class="imgWrap"><img src="' + src + '" data="' + href + '"/></p>' +
                                '<p class="title">' + files[i].name + '</p>' +
                                '</li>'),
                        $btns = $('<div class="file-panel">\
                                    <span class="cancel">删除</span>\
                                </div>').appendTo($li);
                    $li.append('<span class="success"></span>');

                    $li.appendTo($queue);

                    $li.on('mouseenter', function () {
                        $btns.stop().animate({ height: 30 });
                    });

                    $li.on('mouseleave', function () {
                        $btns.stop().animate({ height: 0 });
                    });
                    $btns.on('click', '.cancel', function () {
                        var id = $li.data("pid");
                        $li.off().find('.file-panel').off().end().remove();

                        $.page.fileList[id].status = 1;
                        listFileCount--;

                        if (listFileCount + fileCount == 0) {
                            $placeHolder.removeClass('element-invisible');
                            $queue.hide();
                            $statusBar.addClass('element-invisible');
                        }
                    });
                })(i);
            }
        }

        // 当有文件添加进来时执行，负责view的创建
        function addFile(file) {
            var type = file.ext;
            var iconUrl;
            switch (type) {
                case "pdf":
                    iconUrl = "/images/pdf.png";
                    break;
                case "docx":
                case "doc":
                    iconUrl = "/images/icon_word.png";
                    break;
                case "xlsx":
                case "xls":
                    iconUrl = "/images/icon_excel.png";
                    break;
                case "bmp":
                case "mp4":
                case "avi":
                case "rmvb":
                case "ts":
                    iconUrl = "/images/video.png";
                    break;
            }
            var $li = $('<li id="' + file.id + '">' +
                    '<p class="title">' + file.name + '</p>' +
                    '<p class="imgWrap"></p>' +
                    '<p class="progress"><span></span></p>' +
                    '</li>'),

                $btns = $('<div class="file-panel">' +
                    '<span class="cancel">删除</span>' +
                    '<span class="rotateRight">向右旋转</span>' +
                    '<span class="rotateLeft">向左旋转</span></div>').appendTo($li),
                $prgress = $li.find('p.progress span'),
                $wrap = $li.find('p.imgWrap'),
                $info = $('<p class="error"></p>'),

                showError = function (code) {
                    switch (code) {
                        case 'exceed_size':
                            text = '文件大小超出';
                            break;
                        case 'interrupt':
                            text = '上传暂停';
                            break;
                        default:
                            text = '上传失败，请重试';
                            break;
                    }

                    $info.text(text).appendTo($li);
                };

            if (file.getStatus() === 'invalid') {
                showError(file.statusText);
            } else {
                // @todo lazyload
                $wrap.text('预览中');
                uploader.makeThumb(file, function (error, src) {
                    var img;

                    if (error) {
                        //$wrap.text('不能预览');
                        img = $('<img src="' + iconUrl + '">');
                        $wrap.empty().append(img);
                        return;
                    }

                    if (isSupportBase64) {
                        img = $('<img src="' + src + '">');
                        $wrap.empty().append(img);
                    }
                }, thumbnailWidth, thumbnailHeight);

                percentages[file.id] = [file.size, 0];
                file.rotation = 0;
            }

            file.on('statuschange', function (cur, prev) {

                if (prev === 'progress') {
                    $prgress.hide().width(0);
                } else if (prev === 'queued') {
                    $li.off('mouseenter mouseleave');
                    $btns.remove();
                }

                // 成功
                if (cur === 'error' || cur === 'invalid') {
                    console.log(file.statusText);
                    showError(file.statusText);
                    percentages[file.id][1] = 1;
                } else if (cur === 'interrupt') {
                    showError('interrupt');
                } else if (cur === 'queued') {
                    $info.remove();
                    $prgress.css('display', 'block');
                    percentages[file.id][1] = 0;
                } else if (cur === 'progress') {
                    $info.remove();
                    $prgress.css('display', 'block');
                } else if (cur === 'complete') {
                    $prgress.hide().width(0);
                    //$li.append('<span class="success"></span>');
                }

                $li.removeClass('state-' + prev).addClass('state-' + cur);
            });

            $li.on('mouseenter', function () {
                $btns.stop().animate({ height: 30 });
            });

            $li.on('mouseleave', function () {
                $btns.stop().animate({ height: 0 });
            });

            $btns.on('click', 'span', function () {
                var index = $(this).index(),
                    deg;

                switch (index) {
                    case 0:
                        uploader.removeFile(file);
                        return;

                    case 1:
                        file.rotation += 90;
                        break;

                    case 2:
                        file.rotation -= 90;
                        break;
                }

                if (supportTransition) {
                    deg = 'rotate(' + file.rotation + 'deg)';
                    $wrap.css({
                        '-webkit-transform': deg,
                        '-mos-transform': deg,
                        '-o-transform': deg,
                        'transform': deg
                    });
                } else {
                    $wrap.css('filter', 'progid:DXImageTransform.Microsoft.BasicImage(rotation=' + (~~((file.rotation / 90) % 4 + 4) % 4) + ')');
                }


            });

            $li.appendTo($queue);
        }

        // 负责view的销毁
        function removeFile(file) {
            var $li = $('#' + file.id);

            delete percentages[file.id];
            updateTotalProgress();
            $li.off().find('.file-panel').off().end().remove();
        }

        function updateTotalProgress() {
            var loaded = 0,
                total = 0,
                spans = $progress.children(),
                percent;

            $.each(percentages, function (k, v) {
                total += v[0];
                loaded += v[0] * v[1];
            });

            percent = total ? loaded / total : 0;


            spans.eq(0).text(Math.round(percent * 100) + '%');
            spans.eq(1).css('width', Math.round(percent * 100) + '%');
            updateStatus();
        }

        // 更新文件状态
        function updateStatus() {
            $info.show();
            var text = '', stats;

            if (state === 'ready') {
                text = '选中<span style="color:blue;">' + fileCount + '</span>个文件，共<span style="color:red;">' +
                        WebUploader.formatSize(fileSize) + '</span>。';
            } else if (state === 'confirm') {
                stats = uploader.getStats();
                if (stats.uploadFailNum) {
                    text = '已成功上传<span style="color:green;">' + stats.successNum + '</span>个文件，<span style="color:red;">' +
                        stats.uploadFailNum + '</span>个文件上传失败，<a class="retry" href="#">重新上传</a>失败文件或<a class="ignore" href="#">忽略</a>'
                }

            } else {
                stats = uploader.getStats();
                //text = '共' + fileCount + '个（' +
                //        WebUploader.formatSize(fileSize) +
                //        '），已上传' + stats.successNum + '个';

                //if (stats.uploadFailNum) {
                //    text += '，失败' + stats.uploadFailNum + '个';
                //}
                if (errCount == 0) {
                    text = '共<span style="color:blue;">' + fileCount + '</span>个（<span style="color:red;">' +
                        WebUploader.formatSize(fileSize) +
                        '</span>），已上传<span style="color:green;">' + stats.successNum + '</span>个';

                    if (stats.uploadFailNum) {
                        text += '，失败<span style="color:red;">' + stats.uploadFailNum + '</span>个';
                    }
                } else {
                    text = '共<span style="color:blue;">' + fileCount + '</span>个（<span style="color:red;">' +
                       WebUploader.formatSize(fileSize) +
                       '</span>），已上传<span style="color:green;">' + (stats.successNum - errCount) + '</span>个';

                    if (stats.uploadFailNum) {
                        text += '，失败<span style="color:red;">' + stats.uploadFailNum + errCount + '</span>个';
                    } else {
                        text += '，失败<span style="color:red;">' + errCount + '</span>个';
                    }
                }

            }

            $info.html(text);
        }

        function setState(val) {
            var file, stats;

            if (val === state) {
                return;
            }

            $upload.removeClass('state-' + state);
            $upload.addClass('state-' + val);
            state = val;

            switch (state) {
                case 'pedding':
                    $placeHolder.removeClass('element-invisible');
                    $queue.hide();
                    $statusBar.addClass('element-invisible');
                    uploader.refresh();
                    break;

                case 'ready':
                    $placeHolder.addClass('element-invisible');
                    $('#filePicker2').removeClass('element-invisible');
                    $queue.show();
                    $statusBar.removeClass('element-invisible');
                    uploader.refresh();
                    break;

                case 'uploading':
                    $('#filePicker2').addClass('element-invisible');
                    $progress.show();
                    $upload.text('暂停上传');
                    break;

                case 'paused':
                    $progress.show();
                    $upload.text('继续上传');
                    break;

                case 'confirm':
                    $progress.hide();
                    $('#filePicker2').removeClass('element-invisible');
                    $upload.text('开始上传');

                    stats = uploader.getStats();
                    if (stats.successNum && !stats.uploadFailNum) {
                        setState('finish');
                        return;
                    }
                    break;
                case 'finish':
                    stats = uploader.getStats();
                    // todo
                    if (stats.successNum) {
                        //layer.msg("上传成功");
                    } else {
                        // 没有成功的文件，重设
                        state = 'done';
                        location.reload();
                    }
                    break;
            }

            updateStatus();
        }

        uploader.onUploadProgress = function (file, percentage) {
            var $li = $('#' + file.id),
                $percent = $li.find('.progress span');

            $percent.css('width', percentage * 100 + '%');
            percentages[file.id][1] = percentage;
            updateTotalProgress();
        };

        uploader.onFileQueued = function (file) {
            fileCount++;
            fileSize += file.size;

            if (fileCount === 1) {
                $placeHolder.addClass('element-invisible');
                $statusBar.show();
            }

            addFile(file);
            setState('ready');
            updateTotalProgress();
        };

        uploader.onFileDequeued = function (file) {
            fileCount--;
            fileSize -= file.size;

            if (!(fileCount + listFileCount)) {
                setState('pedding');
            }

            removeFile(file);
            updateTotalProgress();

        };

        uploader.on('all', function (type) {
            var stats;
            switch (type) {
                case 'uploadFinished':
                    setState('confirm');
                    break;

                case 'startUpload':
                    setState('uploading');
                    break;

                case 'stopUpload':
                    setState('paused');
                    break;

            }
        });

        var errCount = 0;
        // 文件上传过程中创建进度条实时显示。
        uploader.on('uploadSuccess', function (file, response) {
            var _raw = response._raw;
            //console.log("_raw", _raw);
            //console.log("response", response);

            var status = response.status;
            var msg = response.msg;

            var $li = $("#" + file.id);
            if (status != 200) {
                var errMsg = "上传失败";
                if (msg != "") {
                    errMsg = msg;
                }

                var $info = $('<p class="error"></p>');
                $info.text(errMsg).appendTo($li);
                //$li.find('.success').hide();
                errCount++;
            } else {

                var data = response.data;
                //layer.msg("上传成功");
                console.log("上传成功, data:" + data);
                $.page.fileList[data.id] = data;
                $li.append('<span class="success"></span>');
            }
        });

        uploader.onError = function (code) {
            console.log('Eroor: ' + code);

            var text = "";
            if (code == "Q_EXCEED_NUM_LIMIT") {
                text = "超出最大文件数";
            }
            if (code == "Q_EXCEED_SIZE_LIMIT") {
                text = "上传文件最大为200K";
            }
            if (code == "Q_TYPE_DENIED") {
                text = "上传文件类型不匹配";
            }
            if (code == "F_DUPLICATE") {
                text = "上传文件重复，已自动排除";
            }
            if (code == "F_EXCEED_SIZE") {
                text = "上传文件最大为200K";
            }
            layer.msg(text);
        };

        $upload.on('click', function () {
            if ($(this).hasClass('disabled')) {
                return false;
            }

            if (state === 'ready') {
                uploader.upload();
            } else if (state === 'paused') {
                uploader.upload();
            } else if (state === 'uploading') {
                uploader.stop();
            }
        });

        $info.on('click', '.retry', function () {
            uploader.retry();
        });

        $info.on('click', '.ignore', function () {
            // alert( 'todo' );
        });

        $upload.addClass('state-' + state);
        updateTotalProgress();
    });

})(jQuery);
