
using System;
using System.Collections.Generic;

namespace Model
{
    /// <summary>
    /// 管理员表
    /// </summary>
    public class Admin
    {
        /// <summary>
        /// 自增主键ID
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string username { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string userpwd { get; set; }
        /// <summary>
        /// 姓名/昵称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        public string mobile { get; set; }
        /// <summary>
        /// 用户头像（暂不实现该功能，保留）
        /// </summary>
        public string avatar { get; set; }
        /// <summary>
        /// 目前状态
        /// </summary>
        public int status { get; set; }
        /// <summary>
        ///  用户组别
        /// </summary>
        public string roleId { get; set; }
        /// <summary>
        /// 用户角色类型
        /// </summary>
        public int roleType { get; set; }
        /// <summary>
        /// 用户角色名称
        /// </summary>
        public string roleName { get; set; }
        /// <summary>
        /// 用户是否被删除字段
        /// </summary>
        public int isDelete { get; set; }
        /// <summary>
        /// 用户权限
        /// </summary>
        public string rightIds { get; set; }

        /// <summary>
        /// 保存权限
        /// </summary>
        public List<string> rights { get; set; }
        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime modifyTime { get; set; }

        /// <summary>
        /// TOKEN
        /// </summary>
        public string token { get; set; }

        /// <summary>
        /// 授权是否管理所有仓库:0 管理单个或多个仓库 1 管理所有仓库
        /// </summary>
        public int isAdmin { get; set; }

        // todo

        public string creatorTime { get; set; }


        public string loginTime { get; set; }

        public string loginIPAddress { get; set; }

        public string prevLoginTime { get; set; }

        public string prevLoginIPAddress { get; set; }

        /// <summary>
        /// 部门Id
        /// </summary>
        public string deptId { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>
        public string deptName { get; set; }
        /// <summary>
        /// 备注信息
        /// </summary>
        public string description { get; set; }
        /// <summary>
        /// 性别(1男 2女 3保密）
        /// </summary>
        public int gender { get; set; }
        /// <summary>
        /// 民族
        /// </summary>
        public int nation { get; set; }
        /// <summary>
        /// 籍贯
        /// </summary>
        public string nativeplace { get; set; }
        /// <summary>
        /// 入职日期
        /// </summary>
        public string entryDate { get; set; }
        /// <summary>
        /// 电子邮箱
        /// </summary>
        public string email { get; set; }
        /// <summary>
        /// 证件类型
        /// </summary>
        public string certificatesType { get; set; }
        /// <summary>
        /// 证件号码
        /// </summary>
        public string certificatesNumber { get; set; }
        /// <summary>
        /// 文化程度
        /// </summary>
        public int education { get; set; }
        /// <summary>
        /// 出生年月
        /// </summary>
        public string birthday { get; set; }
        /// <summary>
        /// 办公电话
        /// </summary>
        public string telephone { get; set; }
        /// <summary>
        /// 办公座机
        /// </summary>
        public string landLine { get; set; }
        /// <summary>
        /// 通讯地址
        /// </summary>
        public string postalAddress { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        public string sortCode { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CREATORTIME { get; set; }
        /// <summary>
        /// 创建人员ID
        /// </summary>
        public string creatorUserId { get; set; }
        /// <summary>
        /// 修改人员ID
        /// </summary>
        public string modifyUserId { get; set; }
        /// <summary>
        /// 删除时间
        /// </summary>
        public DateTime deleteTime { get; set; }
        /// <summary>
        /// 删除人ID
        /// </summary>
        public string deleteUserId { get; set; }
        /// <summary>
        /// 登录地址
        /// </summary>
        public string LoginIpAddress { get; set; }
    }
}
