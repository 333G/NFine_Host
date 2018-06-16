using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mysoft.Task.Models
{
    class OC_BlackList
    {

        /// <summary>
        /// Desc:主键，自增长 
        /// Default:- 
        /// Nullable:False 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Desc:手机号 
        /// Default:- 
        /// Nullable:False 
        /// </summary>
        public string F_Mobile { get; set; }

        /// <summary>
        /// Desc:类型（0=白名单；1=黑名单） 
        /// Default:((1)) 
        /// Nullable:True 
        /// </summary>
        public Boolean? F_Sign { get; set; }

        /// <summary>
        /// Desc:级别 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public int? F_Level { get; set; }

        /// <summary>
        /// Desc:状态（0=禁用；1=启用；） 
        /// Default:((1)) 
        /// Nullable:False 
        /// </summary>
        public Boolean F_EnabledMark { get; set; }

        /// <summary>
        /// Desc:备注 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public string F_Description { get; set; }

        /// <summary>
        /// Desc:用户ID(如果管理员添加的，这里为0，用户添加的，这里是用户ID) 
        /// Default:((0)) 
        /// Nullable:True 
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// Desc:来源（1=管理员添加） （2=自动任务添加）
        /// Default:((1)) 
        /// Nullable:True 
        /// </summary>
        public int? ComeFrom { get; set; }

        /// <summary>
        /// Desc:创建 
        /// Default:(getdate()) 
        /// Nullable:True 
        /// </summary>
        public DateTime? F_CreatorTime { get; set; }

        /// <summary>
        /// Desc:创建人 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public int? F_CreatorUserId { get; set; }

        /// <summary>
        /// Desc:- 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public string Creator { get; set; }

        /// <summary>
        /// Desc:最后编辑时间 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public DateTime? F_LastModifyTime { get; set; }

        /// <summary>
        /// Desc:最后编辑用户ID 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public int? F_LastModifyUserId { get; set; }

        /// <summary>
        /// Desc:最后编辑人账号 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public string F_LastUserAccount { get; set; }

        /// <summary>
        /// Desc:删除时间 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public DateTime? F_DeleteTime { get; set; }

        /// <summary>
        /// Desc:删除用户ID 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public int? F_DeleteUserId { get; set; }

        /// <summary>
        /// Desc:删除状态（0=未删除；1=已经删除） 
        /// Default:((0)) 
        /// Nullable:False 
        /// </summary>
        public Boolean? F_DeleteMark { get; set; }
    }
}
