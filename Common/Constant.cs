using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public class Constant
    {
        /// <summary>
        /// 不存在信息消息
        /// </summary>
        public static string MSG_NOT_HAVE = "不存在该{0}，请刷新后重试";

        /// <summary>
        /// 统一管理操作枚举
        /// </summary>
        public enum ActionEnum
        {
            /// <summary>
            /// 显示
            /// </summary>
            Show,
            /// <summary>
            /// 查看
            /// </summary>
            View,
            /// <summary>
            /// 添加
            /// </summary>
            Add,
            /// <summary>
            /// 修改
            /// </summary>
            Edit,
            /// <summary>
            /// 删除
            /// </summary>
            Delete,
            /// <summary>
            /// 审核
            /// </summary>
            Audit,
            /// <summary>
            /// 反审核
            /// </summary>
            ReAudit,
            /// <summary>
            /// 回复
            /// </summary>
            Reply,
            /// <summary>
            /// 确认
            /// </summary>
            Confirm,
            /// <summary>
            /// 取消
            /// </summary>
            Cancel,
            /// <summary>
            /// 作废
            /// </summary>
            Invalid,
            /// <summary>
            /// 生成
            /// </summary>
            Build,
            /// <summary>
            /// 安装
            /// </summary>
            Instal,
            /// <summary>
            /// 卸载
            /// </summary>
            UnLoad,
            /// <summary>
            /// 登录
            /// </summary>
            Login,
            /// <summary>
            /// 备份
            /// </summary>
            Back,
            /// <summary>
            /// 还原
            /// </summary>
            Restore,
            /// <summary>
            /// 替换
            /// </summary>
            Replace,
            /// <summary>
            /// 复制
            /// </summary>
            Copy,
            /// <summary>
            /// 下载
            /// </summary>
            Download,
            /// <summary>
            /// 导出
            /// </summary>
            Export,
            /// <summary>
            /// 导入
            /// </summary>
            Import,
            /// <summary>
            /// 打印
            /// </summary>
            Print,
            /// <summary>
            /// 启用/禁用
            /// </summary>
            Enabled,
            /// <summary>
            /// 上传
            /// </summary>
            Upload
        }

        public enum InfoEnum {
            Noraml = 1,
            Warn = 2,
            Error = 3,
            Notice = 4
        }
        /// <summary>
        /// 类别枚举
        /// </summary>
        public enum CategoryEnum
        {
            /// <summary>
            /// 设备类别
            /// </summary>
            trade,
            /// <summary>
            /// 使用单位类别
            /// </summary>
            customertype,
            /// <summary>
            /// 供应商类别
            /// </summary>
            supplytype,
            /// <summary>
            /// 支出类别
            /// </summary>
            paccttype,
            /// <summary>
            /// 收入类别
            /// </summary>
            raccttype,
            /// <summary>
            /// 结算方式
            /// </summary>
            PayMethod
        }
    }
}
