using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models
{
    class Sys_User
    {

        /// <summary>
        /// Desc:主键，自增长 
        /// Default:- 
        /// Nullable:False 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Desc:用户GUID 
        /// Default:- 
        /// Nullable:False 
        /// </summary>
        public string F_Id { get; set; }

        /// <summary>
        /// Desc:父级ID 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public string F_ParentId { get; set; }

        /// <summary>
        /// Desc:节点深度 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public int? F_Depth { get; set; }

        /// <summary>
        /// Desc:账户 
        /// Default:- 
        /// Nullable:False 
        /// </summary>
        public string F_Account { get; set; }

        /// <summary>
        /// Desc:姓名 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public string F_RealName { get; set; }

        /// <summary>
        /// Desc:呢称 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public string F_NickName { get; set; }

        /// <summary>
        /// Desc:头像 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public string F_HeadIcon { get; set; }

        /// <summary>
        /// Desc:性别 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public Boolean? F_Gender { get; set; }

        /// <summary>
        /// Desc:生日 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public DateTime? F_Birthday { get; set; }

        /// <summary>
        /// Desc:手机 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public string F_MobilePhone { get; set; }

        /// <summary>
        /// Desc:邮箱 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public string F_Email { get; set; }

        /// <summary>
        /// Desc:微信 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public string F_WeChat { get; set; }

        /// <summary>
        /// Desc:主管主键 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public string F_ManagerId { get; set; }

        /// <summary>
        /// Desc:安全级别 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public int? F_SecurityLevel { get; set; }

        /// <summary>
        /// Desc:个性签名 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public string F_Signature { get; set; }

        /// <summary>
        /// Desc:组织主键 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public string F_OrganizeId { get; set; }

        /// <summary>
        /// Desc:部门主键 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public string F_DepartmentId { get; set; }

        /// <summary>
        /// Desc:角色主键 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public string F_RoleId { get; set; }

        /// <summary>
        /// Desc:岗位主键 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public string F_DutyId { get; set; }

        /// <summary>
        /// Desc:是否管理员 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public Boolean? F_IsAdministrator { get; set; }

        /// <summary>
        /// Desc:排序码 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public int? F_SortCode { get; set; }

        /// <summary>
        /// Desc:删除标志 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public Boolean? F_DeleteMark { get; set; }

        /// <summary>
        /// Desc:有效标志 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public Boolean? F_EnabledMark { get; set; }

        /// <summary>
        /// Desc:描述 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public string F_Description { get; set; }

        /// <summary>
        /// Desc:创建时间 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public DateTime? F_CreatorTime { get; set; }

        /// <summary>
        /// Desc:创建用户 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public string F_CreatorUserId { get; set; }

        /// <summary>
        /// Desc:最后修改时间 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public DateTime? F_LastModifyTime { get; set; }

        /// <summary>
        /// Desc:最后修改用户 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public string F_LastModifyUserId { get; set; }

        /// <summary>
        /// Desc:删除时间 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public DateTime? F_DeleteTime { get; set; }

        /// <summary>
        /// Desc:删除用户 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public string F_DeleteUserId { get; set; }
    }
}
