using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models
{
    class OC_GroupChannel
    {
        //子增长Id
        public int Id { get; set; }
        /// <summary>
        /// Desc:编号 
        /// Default:- 
        /// Nullable:False 
        /// </summary>
        public string F_ID { get; set; }

        /// <summary>
        /// Desc:用户ID 
        /// Default:- 
        /// Nullable:False 
        /// </summary>
        public int F_UserId { get; set; }

        /// <summary>
        /// Desc:通道类型，默认短信 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public string F_ChannelType { get; set; }

        /// <summary>
        /// Desc:通道名称 
        /// Default:- 
        /// Nullable:False 
        /// </summary>
        public string F_ChannelName { get; set; }

        /// <summary>
        /// Desc:通道价格 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public decimal? F_ChannelPrice { get; set; }

        /// <summary>
        /// Desc:移动通道ID 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public int? F_MobileChannel { get; set; }

        /// <summary>
        /// Desc:联通通道ID 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public int? F_UnicomChannel { get; set; }

        /// <summary>
        /// Desc:电信通道ID 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public int? F_TelecomChannel { get; set; }

        /// <summary>
        /// Desc:小灵通通道ID 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public int? F_ChannelXLT { get; set; }

        /// <summary>
        /// Desc:发送率 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public int? F_SendRate { get; set; }

        /// <summary>
        /// Desc:成功通讯 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public int? F_SuccessRate { get; set; }

        /// <summary>
        /// Desc:优先级 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public int? F_Priority { get; set; }

        /// <summary>
        /// Desc:管理员备注 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public string F_AdminMark { get; set; }

        /// <summary>
        /// Desc:用户备注 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public string F_UserMark { get; set; }

        /// <summary>
        /// Desc:是否是隐藏通道 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public Boolean? F_hide { get; set; }
    }
}
