using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models
{
    class OC_BaseChannel
    {
        /// <summary>
        /// Desc:编号 
        /// Default:- 
        /// Nullable:False 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Desc:guid 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public string F_Id { get; set; }

        /// <summary>
        /// Desc:通道名称 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public string F_ChannelName { get; set; }

        /// <summary>
        /// Desc:运营商（1=移动；2=联通；3=电信；4=其他；默认是1） 
        /// Default:((1)) 
        /// Nullable:False 
        /// </summary>
        public int F_Operator { get; set; }

        /// <summary>
        /// Desc:通道状态（1=启用；0=暂停；） 
        /// Default:((1)) 
        /// Nullable:True 
        /// </summary>
        public int? F_ChannelState { get; set; }

        /// <summary>
        /// Desc:是否分享 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public Boolean? F_IsShared { get; set; }

        /// <summary>
        /// Desc:分享标记 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public string F_SharedSign { get; set; }

        /// <summary>
        /// Desc:启用时间 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public TimeSpan F_StartTime { get; set; }

        /// <summary>
        /// Desc:终止时间 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public TimeSpan F_EndTime { get; set; }

        /// <summary>
        /// Desc:通道单价(分) 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public decimal? F_Price { get; set; }

        /// <summary>
        /// Desc:通道余额(元) 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public decimal? F_ChaBalance { get; set; }

        /// <summary>
        /// Desc:剩余条数 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public int? F_SurplusNum { get; set; }

        /// <summary>
        /// Desc:已用条数 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public int? F_SendedNum { get; set; }

        /// <summary>
        /// Desc:签名（1=前置签名；2=后置签名；3=无需签名；） 
        /// Default:((1)) 
        /// Nullable:False 
        /// </summary>
        public int F_autograph { get; set; }

        /// <summary>
        /// Desc:退订（0=不开启退订；1=开启退订；） 
        /// Default:((1)) 
        /// Nullable:False 
        /// </summary>
        public string F_unsubscribe { get; set; }

        /// <summary>
        /// Desc:字符数 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public int? F_CharacterCount { get; set; }

        /// <summary>
        /// Desc:监控状态（0=不监控；1=开启监控） 
        /// Default:((0)) 
        /// Nullable:True 
        /// </summary>
        public Boolean? F_MonitorState { get; set; }

        /// <summary>
        /// Desc:监控时长（秒为单位；默认是0，0=不监控；） 
        /// Default:((0)) 
        /// Nullable:True 
        /// </summary>
        public int? F_MonitorTime { get; set; }

        /// <summary>
        /// Desc:监控手机号（多个手机号用,隔开） 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public string F_MonitorMobile { get; set; }

        /// <summary>
        /// Desc:计费字数（默认是67个字） 
        /// Default:((67)) 
        /// Nullable:True 
        /// </summary>
        public int? F_ChargeNumber { get; set; }

        /// <summary>
        /// Desc:长短信标志（0=不支持；1=支持；默认是1） 
        /// Default:((1)) 
        /// Nullable:True 
        /// </summary>
        public Boolean? F_LongSmsSign { get; set; }

        /// <summary>
        /// Desc:长字符数 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public int? F_LongSmsNumber { get; set; }

        /// <summary>
        /// Desc:最后一次发送时间 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public DateTime? F_LastSendTime { get; set; }

        /// <summary>
        /// Desc:备注 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public string F_Description { get; set; }

        /// <summary>
        /// Desc:是否删除（0=不删除；1=删除） 
        /// Default:((0)) 
        /// Nullable:True 
        /// </summary>
        public Boolean? F_DeleteMark { get; set; }

        /// <summary>
        /// Desc:是否可用（1=可用；0=禁用） 
        /// Default:((1)) 
        /// Nullable:True 
        /// </summary>
        public Boolean? F_EnabledMark { get; set; }

        /// <summary>
        /// Desc:创建时间 
        /// Default:(getdate()) 
        /// Nullable:True 
        /// </summary>
        public DateTime? F_CreatorTime { get; set; }

        /// <summary>
        /// Desc:创建人（账号） 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public string F_CreatorUserId { get; set; }

        /// <summary>
        /// Desc:最后编辑时间 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public DateTime? F_LastModifyTime { get; set; }

        /// <summary>
        /// Desc:最后编辑人（账号） 
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
        /// Desc:删除人（账号） 
        /// Default:- 
        /// Nullable:True 
        /// </summary>
        public string F_DeleteUserId { get; set; }

        //签名内容
        public string F_autographContent { get; set; }
    }
}
