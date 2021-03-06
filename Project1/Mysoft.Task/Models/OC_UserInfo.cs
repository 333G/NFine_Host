﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mysoft.Task.Models
{
    class OC_UserInfo
    {
        /// <summary>
        /// Desc:编号 
        /// Default:- 
        /// Nullable:False 
        /// </summary>
        public string F_Id { get; set; }

        /// <summary>
        /// Desc:账号GUID 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public string F_UserFid { get; set; }

        /// <summary>
        /// Desc:账号ID 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public int? F_UserId { get; set; }

        /// <summary>
        /// Desc:祖宗账号ID 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public int? F_RootId { get; set; }

        /// <summary>
        /// Desc:业务员账号ID 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public int? F_ManagerId { get; set; }

        /// <summary>
        /// Desc:账号 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public string F_Account { get; set; }

        /// <summary>
        /// Desc:发送数量 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public int? F_SendedNum { get; set; }

        /// <summary>
        /// Desc:余额 
        /// Default:- 
        /// Nullable:False 
        /// </summary>
        public Decimal F_Balance { get; set; }

        /// <summary>
        /// Desc:审核 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public int? F_Reviewed { get; set; }

        /// <summary>
        /// Desc:状态 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public string F_State { get; set; }

        /// <summary>
        /// Desc:删除标记 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public Boolean? F_DeleteMark { get; set; }

        /// <summary>
        /// Desc:可用标记 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public Boolean? F_EnabledMark { get; set; }

        /// <summary>
        /// Desc:备注 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public string F_Description { get; set; }

        /// <summary>
        /// Desc:记录时间  
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public DateTime? F_CreatorTime { get; set; }

        /// <summary>
        /// Desc:记录人ID 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public string F_CreatorUserId { get; set; }

        /// <summary>
        /// Desc:最近修改时间 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public DateTime? F_LastModifyTime { get; set; }

        /// <summary>
        /// Desc:最近修改人 
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
        /// Desc:删除人 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public string F_DeleteUserId { get; set; }
        public Boolean? F_BalanceReminder { get; set; }
        /// <summary>
        /// Desc:日最多接收短信 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public int? F_MessageNum { get; set; }
        /// <summary>
        /// Desc:1小时验证码限制 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public int? F_OneCode { get; set; }
        /// <summary>
        /// Desc:24小时验证码限制 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public int? F_TwentyFourCode { get; set; }
    }
}
