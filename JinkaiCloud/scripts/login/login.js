var requestUrl = "ajax/login.ashx";
//检查字符串是否为空
function checkNullOrEmpty(str) {
    var result = false;
    if (str == null)
        result = true;
    str = str.replace(/(^\s*)|(\s*$)/g, "");
    if (str.length == 0)
        result = true;
    return result;
}
// 提交表单
function sumbitLogin() {
    
    var cookieEnabled = (navigator.cookieEnabled) ? true : false;
    if (!cookieEnabled) {
        alert("该浏览器Cookie设置不正确，无法正常登录");
        return false;
    }
    
    var username = $.trim($("#username").val());
    var password = $.trim($("#password").val());
    //验证码
    var txtCode = $.trim($("#txtCode").val());
   
    if (checkNullOrEmpty(username)) {
        $("#msgtip").html("请输入用户名").show();
        $("#username").focus();
        return false;
    }
    if (checkNullOrEmpty(password)) {
        $("#msgtip").html("请输入密码").show();
        $("#password").focus();
        return false;
    }
    if (checkNullOrEmpty(txtCode)) {
        $("#msgtip").html("请输入验证码").show();
        $("#txtCode").focus();
        return false;
    }
    $('.loading').show();
    $.ajax({
        type: "post",
        url: requestUrl + "?action=login",
        data: {
            username:username,
            userpwd: $.md5(password),
            txtcode: $.md5(txtCode.toLowerCase()),
            ispwd: 1
        },
        success: function (data) {
           
            if (data.status == 200) {
                $('.loading').hide();
                location.href = "index.html";
            } else {
                $("#msgtip").html(data.msg).show();
                $('.loading').hide();
                setTimeout("location.href='login.html'", 500);
            }
            return false;
        },
        timeout: 60000,
        error: function (xhr, status) {
            if (status == "timeout") {
                $("#loginerror").text("您的网络好像很糟糕，请刷新页面重试").show();
                $('.loading').hide();
                return false;
            }
            else {
                $("#loginerror").text("服务器内部错误，请重试").show();
                $('.loading').hide();
                return false;
            }
        }
    });
    return false;
}

$(function () {
    $('.loading').hide();
    document.onkeydown = function (e) {
        var ev = document.all ? window.event : e;
        if (ev.keyCode == 13) {
            $("#btnLogin").trigger("click");
        }
    };
});