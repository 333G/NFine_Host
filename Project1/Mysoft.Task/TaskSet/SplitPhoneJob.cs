using Models;
using Mysoft.Utility;
using Quartz;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ToolGood.Words;

namespace Mysoft.Task.TaskSet
{
    /// <summary>
    /// 测试任务
    /// </summary>
    ///<remarks>DisallowConcurrentExecution属性标记任务不可并行，要是上一任务没运行完即使到了运行时间也不会运行</remarks>
    [DisallowConcurrentExecution]
    public class SplitPhoneJob : IJob
    {
        private static string connStr = SysConfig.SqlConnect;
        private static string sensitive = GetSensitive();
        private static Dictionary<string, string> PhoneInfoDic = new Dictionary<string, string>();
        List<Models.OC_BlackList> blackWhitelist = GetBlackWhiteList();
        private static string GetSensitive()//获取系统敏感词字符串
        {
            using (SqlSugarClient db = new SqlSugarClient(connStr))
            {
                List<SMS_SensitiveWords> SensitiveList = db.SqlQuery<SMS_SensitiveWords>("select F_SensitiveWords from SMS_SensitiveWords where F_IsChannelKeyWord='0'");

                foreach(var item in SensitiveList)
                {
                    sensitive = sensitive + item.F_SensitiveWords + "|";
                }
            }
            return sensitive;
        }
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                int listcount = StartJob();
                LogHelper.WriteLog("号码拆分，短信拆大段任务。当前系统时间:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss" + "操作了" + listcount + "条符合条件数据"));
            }
            catch (Exception ex)
            {
                JobExecutionException e2 = new JobExecutionException(ex);
                LogHelper.WriteLog("任务异常", ex);
                //1.立即重新执行任务 
                e2.RefireImmediately = true;
                //2 立即停止所有相关这个任务的触发器
                //e2.UnscheduleAllTriggers=true; 
            }
        }

        //线程方式，返回详单的实体，即拆号码（F_MobileList）
        private void SubmitSevSendDateDetailParallel(List<SMC_SendSms> SendList)
        {
           // var parallelOption = new ParallelOptions() { MaxDegreeOfParallelism = 500 };
            Parallel.ForEach(SendList, (item ,ParallelLoopState)=>
            {
                LogHelper.WriteLog("某一次拆号码的开始:" +item.F_Id+"=======时间7" +DateTime.Now.ToString());
                ChangeOperateStateStar(item);//修改发送状态为处理中
                List<Sev_SendDateDetail> list_catch_sdd = new List<Sev_SendDateDetail>();//每个用户的缓存发送列表,用于单个用户发送任务的计费。
                OC_GroupChannel GroupModel = GetGroupChannel(item.F_GroupChannelId);//获取短信单价以及基准移动通道Id,还有发送率
                decimal ChannelPrice = GroupModel.F_ChannelPrice.ObjToDecimal();
                int BaseMobileChannelId = GroupModel.F_MobileChannel.ObjToInt();
                OC_BaseChannel BaseChannelModel = GetBaseChannel(BaseMobileChannelId);
                if (BaseMobileChannelId == 0)
                {
                    item.F_SendState = -1;//发送失败
                    ChangeSendState(item);//修改发送状态失败
                    ParallelLoopState.Break();
                    return;
                }
                else
                {
                    decimal unitprice= ChenkBalance(item.F_SmsContent, item.F_MobileList, ChannelPrice, item.F_CreatorUserId, BaseChannelModel, item.F_Id);
                    if (unitprice>0)//余额判断，如果余额够，扣费,并且更新发送表的总金额
                    {
                        if (item.F_MobileList != null)
                        {
                            Models.OC_UserInfo UserInfoModel = new Models.OC_UserInfo();
                            UserInfoModel = GetUserInfo(item.F_CreatorUserId);//获取用户信息实体
                            Sev_SendDateDetail ssdd = new Sev_SendDateDetail();//生成实体对象
                            ssdd.SMC_F_Id = item.F_Id;
                            ssdd.F_RootId = item.F_RootId.ObjToInt();
                            ssdd.F_UserId = UserInfoModel.F_UserId.ObjToInt();
                            ssdd.F_CreatorUserId = item.F_CreatorUserId;
                            ssdd.F_SendTime = item.F_SendTime;//发送时间继承（其实是前台的提交时间，或者定时发送的时间）
                            ssdd.F_Reissue = 0;//补发次数初始为0
                            ssdd.F_Price = unitprice;//每次发送短信的单次总价
                            ssdd.F_IsCashBack = false;//默认没有返款
                            ssdd.F_SignLocation = BaseChannelModel.F_autograph;//取签名位置，1前；2后；3无
                            ssdd.F_UserSignature = GetUsersignature(item.F_CreatorUserId);//取用户签名
                            string Subscrib = BaseChannelModel.F_unsubscribe;//根据基准移动通道Id获取是否开启退订，如果返回值不是null，就是开启退订。

                            string[] mobilelist = item.F_MobileList.Split(',');
                            int MobileCount = mobilelist.Count();
                            if (MobileCount > 500)//发送号码数量>500才进入扣量判断
                            {
                                //对队列进行发送率，成功率与扣量计算处理
                                decimal SendRate = (decimal)GroupModel.F_SendRate.ObjToInt() / 100;//发送率
                                int BuckleBaseNum = 0;//计数次数
                                decimal BuckleNum;//扣除间隔条数
                                if (SendRate == 1)//满发送率，不扣量
                                { 
                                    for (int n = 0; n < MobileCount; n++)//每个号码添加一个详单到list
                                    {
                                        ssdd.F_BlackWhite = 2;//默认没有黑白名单（0白1黑）
                                        ssdd.F_DealState = 0;//默认可以拆分状态
                                        ssdd.F_Level = item.F_Priority;//优先级继承
                                        ssdd.F_Buckle = 0;//初始没有扣量
                                       //====================================//这4个状态是会变的，所以每次要重新重置
                                        ssdd.F_PhoneCode = mobilelist[n].Replace("\t", "");
                                        string[] PhoneCodeAndlocatiostring = (GetOperatoeAndLocation(ssdd.F_PhoneCode)).Split(';');//根据号码查运营商和号码归属地
                                        ssdd.F_Operator = PhoneCodeAndlocatiostring[0].ObjToInt();//运营商
                                        ssdd.F_Province = PhoneCodeAndlocatiostring[1];//省份
                                        int ChannelId = GetChannelId(ssdd.F_Operator.ObjToInt(), item.F_GroupChannelId, ssdd.F_Province);/// 根据运营商，更新成BaseChannelId
                                        ssdd.F_ChannelId = ChannelId;
                                        if (ChannelId == 0)
                                            ssdd.F_DealState = 2;//拆分失败
                                        ssdd.F_ProtocolType = GetProtocol(ssdd.F_ChannelId.ObjToInt());//获取发送协议类型

                                        int LinmitNum = UserInfoModel.F_MessageNum.ObjToInt();
                                        int verificationNum_1 = UserInfoModel.F_OneCode.ObjToInt();
                                        int verificationNum_24 = UserInfoModel.F_TwentyFourCode.ObjToInt();
                                        if (item.F_IsVerificationCode == true)//是验证码
                                        {
                                            if (verificationNum_1 != 0 && verificationNum_1 >= OneHourverificationCount(ssdd.F_PhoneCode))
                                                ssdd.F_DealState = 5;//验证码号码超过一小时发送次数限制，不发送。
                                            else if (verificationNum_24 != 0 && verificationNum_24 >= DailyverificationCount(ssdd.F_PhoneCode))
                                                ssdd.F_DealState = 5;//验证码号码超过24发送次数限制，不发送。
                                        }
                                        else//非验证码
                                        {
                                            if (LinmitNum != 0 && verificationNum_24 > DailyverificationCount(ssdd.F_PhoneCode))
                                                ssdd.F_DealState = 5;//短信超过24小时发送限制
                                        }

                                        //ssdd.F_ChannelSignature//暂时无此适配
                                        ssdd.F_SmsContent = item.F_SmsContent;//添加短信
                                                                          
                                        Sev_SendDateDetail BlackWhiteModel = BlackWhiteChecked(ssdd);
                                        ssdd.F_DealState = BlackWhiteModel.F_DealState;
                                        ssdd.F_BlackWhite = BlackWhiteModel.F_BlackWhite;
                                        ssdd.F_Level = BlackWhiteModel.F_Level;
                                        SubmitSendDateList(ssdd, Subscrib, BaseMobileChannelId);//往数据图提交处理好的senddate
                                    }
                                    LogHelper.WriteLog("某一次拆号码的结束:" + item.F_Id + "=======时间7" + DateTime.Now.ToString());
                                }
                                else
                                {
                                    BuckleNum = Math.Ceiling(SendRate / (1 - SendRate));//获取扣除间隔条数，向上取整

                                    for (int n = 0; n < MobileCount; n++)//每个号码添加一个详单到list
                                    {
                                        ssdd.F_BlackWhite = 2;//默认没有黑白名单（0白1黑）
                                        ssdd.F_DealState = 0;//默认可以拆分状态
                                        ssdd.F_Level = item.F_Priority;//优先级继承
                                        ssdd.F_Buckle = 0;//初始没有扣量
                                                          //这4个状态是会变的，所以每次要重新重置
                                        ssdd.F_PhoneCode = mobilelist[n].Replace("\t", "");
                                        Sev_SendDateDetail BlackWhiteModel = BlackWhiteChecked(ssdd);
                                        ssdd.F_BlackWhite = BlackWhiteModel.F_BlackWhite;//黑白名单
                                        ssdd.F_Level = BlackWhiteModel.F_Level;
                                        BuckleBaseNum++;
                                        if (BuckleBaseNum > BuckleNum && ssdd.F_BlackWhite != 1 && ssdd.F_BlackWhite != 0)
                                        {
                                            ssdd.F_Buckle = 1;//被扣量
                                            ssdd.F_DealState = 3;//被选为扣量，扣量状态（前台显示已发送，但是实际上不给发。可以写补发程序进行补发）  
                                            BuckleBaseNum = 0;//重置计数次数                
                                        }
                                        else if (BuckleBaseNum > BuckleNum && (ssdd.F_BlackWhite == 1 || ssdd.F_BlackWhite == 0))//若扣量的是黑白名单，跳过
                                        {
                                            BuckleBaseNum--;
                                        }
                                        string[] PhoneCodeAndlocatiostring = (GetOperatoeAndLocation(ssdd.F_PhoneCode)).Split(';');//根据号码查运营商和号码归属地
                                        ssdd.F_Operator = PhoneCodeAndlocatiostring[0].ObjToInt();//运营商
                                        ssdd.F_Province = PhoneCodeAndlocatiostring[1];//省份
                                        int ChannelId = GetChannelId(ssdd.F_Operator.ObjToInt(), item.F_GroupChannelId, ssdd.F_Province);/// 根据运营商，更新成BaseChannelId
                                        ssdd.F_ChannelId = ChannelId;
                                        if (ChannelId == 0)
                                            ssdd.F_DealState = 2;//拆分失败
                                        ssdd.F_ProtocolType = GetProtocol(ssdd.F_ChannelId.ObjToInt());//获取发送协议类型

                                        int LinmitNum = UserInfoModel.F_MessageNum.ObjToInt();
                                        int verificationNum_1 = UserInfoModel.F_OneCode.ObjToInt();
                                        int verificationNum_24 = UserInfoModel.F_TwentyFourCode.ObjToInt();
                                        if (item.F_IsVerificationCode == true)//是验证码
                                        {
                                            if (verificationNum_1 != 0 && verificationNum_1 >= OneHourverificationCount(ssdd.F_PhoneCode))
                                                ssdd.F_DealState = 5;//验证码号码超过一小时发送次数限制，不发送。
                                            else if (verificationNum_24 != 0 && verificationNum_24 >= DailyverificationCount(ssdd.F_PhoneCode))
                                                ssdd.F_DealState = 5;//验证码号码超过24发送次数限制，不发送。
                                        }
                                        else//非验证码
                                        {
                                            if (LinmitNum != 0 && verificationNum_24 > DailyverificationCount(ssdd.F_PhoneCode))
                                                ssdd.F_DealState = 5;//短信超过24小时发送限制
                                        }

                                        //ssdd.F_ChannelSignature//暂时无此适配
                                        ssdd.F_SmsContent = item.F_SmsContent;//添加短信

                                        ssdd.F_DealState = BlackWhiteModel.F_DealState;
                                        SubmitSendDateList(ssdd, Subscrib, BaseMobileChannelId);//往数据图提交处理好的senddate
                                    }
                                    LogHelper.WriteLog("某一次拆号码的结束:" + item.F_Id + "=======时间7" + DateTime.Now.ToString());
                                }
                            }
                            else//不进入扣量程序
                            {
                                for (int n = 0; n < MobileCount; n++)//每个号码添加一个详单到list
                                {
                                    ssdd.F_BlackWhite = 2;//默认没有黑白名单（0白1黑）
                                    ssdd.F_DealState = 0;//默认可以拆分状态
                                    ssdd.F_Level = item.F_Priority;//优先级继承
                                    ssdd.F_Buckle = 0;//初始没有扣量
                                    //这4个状态是会变的，所以每次要重新重置
                                    ssdd.F_PhoneCode = mobilelist[n].Replace("\t", "");

                                    string[] PhoneCodeAndlocatiostring = (GetOperatoeAndLocation(ssdd.F_PhoneCode)).Split(';');//根据号码查运营商和号码归属地

                                    ssdd.F_Operator = PhoneCodeAndlocatiostring[0].ObjToInt();//运营商
                                    ssdd.F_Province = PhoneCodeAndlocatiostring[1];//省份
                                    int ChannelId = GetChannelId(ssdd.F_Operator.ObjToInt(), item.F_GroupChannelId, ssdd.F_Province);/// 根据运营商，更新成BaseChannelId
                                    ssdd.F_ChannelId = ChannelId;
                                    if (ChannelId == 0)
                                        ssdd.F_DealState = 2;//拆分失败
                                    ssdd.F_ProtocolType = GetProtocol(ssdd.F_ChannelId.ObjToInt());//获取发送协议类型

                                    int LinmitNum = UserInfoModel.F_MessageNum.ObjToInt();
                                    int verificationNum_1 = UserInfoModel.F_OneCode.ObjToInt();
                                    int verificationNum_24 = UserInfoModel.F_TwentyFourCode.ObjToInt();
                                    if (item.F_IsVerificationCode == true)//是验证码
                                    {
                                        if (verificationNum_1 != 0 && verificationNum_1 >= OneHourverificationCount(ssdd.F_PhoneCode))
                                            ssdd.F_DealState = 5;//验证码号码超过一小时发送次数限制，不发送。
                                        else if (verificationNum_24 != 0 && verificationNum_24 >= DailyverificationCount(ssdd.F_PhoneCode))
                                            ssdd.F_DealState = 5;//验证码号码超过24发送次数限制，不发送。
                                    }
                                    else//非验证码
                                    {
                                        if (LinmitNum != 0 && verificationNum_24 > DailyverificationCount(ssdd.F_PhoneCode))
                                            ssdd.F_DealState = 5;//短信超过24小时发送限制
                                    }

                                    //ssdd.F_ChannelSignature//暂时无此适配
                                    ssdd.F_SmsContent = item.F_SmsContent;//添加短信

                                    Sev_SendDateDetail BlackWhiteModel = BlackWhiteChecked(ssdd);
                                    ssdd.F_DealState = BlackWhiteModel.F_DealState;
                                    ssdd.F_BlackWhite = BlackWhiteModel.F_BlackWhite;
                                    ssdd.F_Level = BlackWhiteModel.F_Level;
                                    SubmitSendDateList(ssdd, Subscrib, BaseMobileChannelId);//往数据图提交处理好的senddate
                                }
                                LogHelper.WriteLog("某一次拆号码的结束:" + item.F_Id + "=======时间7" + DateTime.Now.ToString());
                            }
                        }
                    }
                    else
                    {
                        ChangesendStateDone(item.F_Id);//发送余额不足，整条打回发送表
                        item.F_SendState = -1;//发送失败
                    }
                }
                ChangeSendState(item);//修改状态为已处理，发送时间取这个时候的时间
            });
        }

        //拆大段
        private void SubmitSendDateList(Sev_SendDateDetail SendModel,string Subscrib,int BaseMobileChannelId)//根据基础通道Id获取是否开启退订，如果返回值不是null，就是开启退订。
        {
            LogHelper.WriteLog("某一次拆大段的开始=="+SendModel.F_Id+ "=========时间5：" + DateTime.Now.ToString());
            List<Sev_SendDateDetail> DealedSendList = new List<Sev_SendDateDetail>();
            string SmsContent = SendModel.F_SmsContent;
            
            OC_BaseChannel BaseChannelModel = new OC_BaseChannel();
            using (SqlSugarClient db = new SqlSugarClient(connStr))
            {
                BaseChannelModel = db.Queryable<OC_BaseChannel>().SingleOrDefault(t => t.Id == BaseMobileChannelId);//取通道
            }
            int ChannelMaxLength = BaseChannelModel.F_LongSmsNumber.ObjToInt();//单长短信最大字符数，根据此来拆大段
            if (ChannelMaxLength < 66)
                ChannelMaxLength = 70;
            if (Subscrib != null ||Subscrib!="")//通道开启退订
            {
                StringSearch keyword = new StringSearch();
                string[] s = { Subscrib };//判断是否有退订字符
                keyword.SetKeywords(s);
                if (!keyword.ContainsAny(SmsContent))//内容中没有退订字符
                {
                    SmsContent += Subscrib;//加上通道规定的退订内容
                }
                SendModel.F_LongsmsCount = GetLongSmsCount(SendModel.F_ChannelId.ObjToInt(), SendModel.F_SmsContent);//短信计数,添加的标记字符也算在内
                if (SendModel.F_LongsmsCount <= 0)
                {
                    if (SendModel.F_LongsmsCount == 0)
                        SendModel.F_DealState = 9;//不支持长短信发送，拆分失败
                    else
                        SendModel.F_DealState = 2;//失败
                }
                if (SendModel.F_SignLocation == 1 || SendModel.F_SignLocation == 2)//开启签名
                {
                    int SplitSmsCount = ChannelMaxLength - SendModel.F_UserSignature.Length - 4 - 2;//每条短信的的正文内容数量,4是：“1/n(”,2是签名的【】
                    if (SendModel.F_UserSignature.Length + SmsContent.Length + 2 > ChannelMaxLength)//判断是否拆大段
                    {
                        decimal Count = Math.Ceiling(Convert.ToDecimal(SendModel.F_SmsContent.Length) / SplitSmsCount);//向上取整数
                        for (int i = 0; i < Count; i++)
                        {
                            string newsmscontent = null;
                            Sev_SendDateDetail newSendMode = new Sev_SendDateDetail();//生成实体对象
                            //继承内容
                            newSendMode.F_AccessNumber = SendModel.F_AccessNumber;
                            newSendMode.F_Buckle = SendModel.F_Buckle;
                            newSendMode.F_ChannelId = SendModel.F_ChannelId;
                            newSendMode.F_ChannelSignature = SendModel.F_ChannelSignature;
                            newSendMode.F_CreatorTime = SendModel.F_CreatorTime;
                            newSendMode.F_CreatorUserId = SendModel.F_CreatorUserId;
                            newSendMode.F_DealState = SendModel.F_DealState;
                            newSendMode.F_IsCashBack = SendModel.F_IsCashBack;
                            newSendMode.F_IsVerificationCode = SendModel.F_IsVerificationCode;
                            newSendMode.F_Level = SendModel.F_Level;
                            newSendMode.F_Operator = SendModel.F_Operator;
                            newSendMode.F_PhoneCode = SendModel.F_PhoneCode;
                            newSendMode.F_Price = SendModel.F_Price;
                            newSendMode.F_ProtocolType = SendModel.F_ProtocolType;
                            newSendMode.F_Province = SendModel.F_Province;
                            newSendMode.F_Reissue = SendModel.F_Reissue;
                            newSendMode.F_RootId = SendModel.F_RootId;
                            newSendMode.F_SendTime = SendModel.F_SendTime;
                            newSendMode.F_SignLocation = SendModel.F_SignLocation;
                            newSendMode.F_Synchro = SendModel.F_Synchro;
                            newSendMode.F_UserId = SendModel.F_UserId;
                            newSendMode.F_UserSignature = SendModel.F_UserSignature;
                            newSendMode.F_BlackWhite = SendModel.F_BlackWhite;
                            newSendMode.SMC_F_Id = SendModel.SMC_F_Id;
                            //================================================================
                            newSendMode.F_Id = Guid.NewGuid().ToString();
                            if (SendModel.F_SignLocation == 1)//签名在前
                            {
                                if (((i + 1) * SplitSmsCount) < SmsContent.Length)//避免超出长度
                                {
                                    newsmscontent = "【" + SendModel.F_UserSignature + "】" + (i + 1) + "/" + Count + ")" + SmsContent.Substring(i * SplitSmsCount, SplitSmsCount);
                                }
                                else
                                {
                                    newsmscontent = "【" + SendModel.F_UserSignature + "】" + (i + 1) + "/" + Count + ")" + SmsContent.Substring(i * SplitSmsCount, SmsContent.Length - i * SplitSmsCount);
                                }
                            }
                            else//签名在后
                            {
                                if (((i + 1) * SplitSmsCount) < SmsContent.Length)//避免超出长度
                                {
                                    newsmscontent = (i + 1) + "/" + Count + ")"+ SmsContent.Substring(i * SplitSmsCount,  SplitSmsCount) + "【" + SendModel.F_UserSignature + "】";
                                }
                                else
                                {
                                    newsmscontent = (i + 1) + "/" + Count + ")"+ SmsContent.Substring(i * SplitSmsCount, SmsContent.Length - i * SplitSmsCount) + "【" + SendModel.F_UserSignature + "】";
                                }
                            }
                            newSendMode.F_SmsContent = newsmscontent;//新拆分的大段信息内容
                            newSendMode.F_LongsmsCount = Count.ObjToInt();
                            DealedSendList.Add(newSendMode);
                        }
                    }
                    else//不需要拆大段
                    {
                        string newsmscontent = null;
                        if (SendModel.F_SignLocation == 1)//签名在前
                            newsmscontent = "【" + SendModel.F_UserSignature + "】" + SmsContent;
                        else//签名在后
                            newsmscontent = SmsContent + "【" + SendModel.F_UserSignature + "】";

                        Sev_SendDateDetail newSendMode = new Sev_SendDateDetail();
                        //继承内容
                        newSendMode.F_AccessNumber = SendModel.F_AccessNumber;
                        newSendMode.F_Buckle = SendModel.F_Buckle;
                        newSendMode.F_ChannelId = SendModel.F_ChannelId;
                        newSendMode.F_ChannelSignature = SendModel.F_ChannelSignature;
                        newSendMode.F_CreatorTime = SendModel.F_CreatorTime;
                        newSendMode.F_CreatorUserId = SendModel.F_CreatorUserId;
                        newSendMode.F_DealState = SendModel.F_DealState;
                        newSendMode.F_IsCashBack = SendModel.F_IsCashBack;
                        newSendMode.F_IsVerificationCode = SendModel.F_IsVerificationCode;
                        newSendMode.F_Level = SendModel.F_Level;
                        newSendMode.F_Operator = SendModel.F_Operator;
                        newSendMode.F_PhoneCode = SendModel.F_PhoneCode;
                        newSendMode.F_Price = SendModel.F_Price;
                        newSendMode.F_ProtocolType = SendModel.F_ProtocolType;
                        newSendMode.F_Province = SendModel.F_Province;
                        newSendMode.F_Reissue = SendModel.F_Reissue;
                        newSendMode.F_RootId = SendModel.F_RootId;
                        newSendMode.F_SendTime = SendModel.F_SendTime;
                        newSendMode.F_SignLocation = SendModel.F_SignLocation;
                        newSendMode.F_Synchro = SendModel.F_Synchro;
                        newSendMode.F_UserId = SendModel.F_UserId;
                        newSendMode.F_UserSignature = SendModel.F_UserSignature;
                        newSendMode.F_BlackWhite = SendModel.F_BlackWhite;
                        newSendMode.SMC_F_Id = SendModel.SMC_F_Id;
                        newSendMode.F_LongsmsCount = 1;
                        //================================================================
                        newSendMode.F_Id = Guid.NewGuid().ToString();
                        newSendMode.F_SmsContent = SmsContent;
                        DealedSendList.Add(newSendMode);
                    }
                }
                else//不开启签名
                {
                    int SplitSmsCount = ChannelMaxLength - 4;//每条短信的的正文内容数量,4是：“1/n(”
                    if (SmsContent.Length > ChannelMaxLength)//拆大段
                    {
                        decimal Count = Math.Ceiling(Convert.ToDecimal(SendModel.F_SmsContent.Length) / SplitSmsCount);//向上取整数
                        for (int i = 0; i < Count; i++)
                        {
                            Sev_SendDateDetail newSendMode = new Sev_SendDateDetail();
                            //继承内容
                            newSendMode.F_AccessNumber = SendModel.F_AccessNumber;
                            newSendMode.F_Buckle = SendModel.F_Buckle;
                            newSendMode.F_ChannelId = SendModel.F_ChannelId;
                            newSendMode.F_ChannelSignature = SendModel.F_ChannelSignature;
                            newSendMode.F_CreatorTime = SendModel.F_CreatorTime;
                            newSendMode.F_CreatorUserId = SendModel.F_CreatorUserId;
                            newSendMode.F_DealState = SendModel.F_DealState;
                            newSendMode.F_IsCashBack = SendModel.F_IsCashBack;
                            newSendMode.F_IsVerificationCode = SendModel.F_IsVerificationCode;
                            newSendMode.F_Level = SendModel.F_Level;
                            newSendMode.F_Operator = SendModel.F_Operator;
                            newSendMode.F_PhoneCode = SendModel.F_PhoneCode;
                            newSendMode.F_Price = SendModel.F_Price;
                            newSendMode.F_ProtocolType = SendModel.F_ProtocolType;
                            newSendMode.F_Province = SendModel.F_Province;
                            newSendMode.F_Reissue = SendModel.F_Reissue;
                            newSendMode.F_RootId = SendModel.F_RootId;
                            newSendMode.F_SendTime = SendModel.F_SendTime;
                            newSendMode.F_SignLocation = SendModel.F_SignLocation;
                            newSendMode.F_Synchro = SendModel.F_Synchro;
                            newSendMode.F_UserId = SendModel.F_UserId;
                            newSendMode.F_UserSignature = SendModel.F_UserSignature;
                            newSendMode.F_BlackWhite = SendModel.F_BlackWhite;
                            newSendMode.SMC_F_Id = SendModel.SMC_F_Id;
                            //================================================================
                            newSendMode.F_Id = Guid.NewGuid().ToString();
                            string newsmscontent = null;
                            if ((i + 1) * SplitSmsCount < SmsContent.Length)
                                newsmscontent = (i + 1) + "/" + Count + ")" + SmsContent.Substring(i * SplitSmsCount, SplitSmsCount);
                            else
                                newsmscontent = (i + 1) + "/" + Count + ")" + SmsContent.Substring(i * SplitSmsCount, SmsContent.Length - i * SplitSmsCount);

                            newSendMode.F_SmsContent = newsmscontent;
                            newSendMode.F_LongsmsCount = Count.ObjToInt();
                            DealedSendList.Add(newSendMode);
                        }
                    }
                    else//不拆大段
                    {
                        Sev_SendDateDetail newSendMode = new Sev_SendDateDetail();
                        //继承内容
                        newSendMode.F_AccessNumber = SendModel.F_AccessNumber;
                        newSendMode.F_Buckle = SendModel.F_Buckle;
                        newSendMode.F_ChannelId = SendModel.F_ChannelId;
                        newSendMode.F_ChannelSignature = SendModel.F_ChannelSignature;
                        newSendMode.F_CreatorTime = SendModel.F_CreatorTime;
                        newSendMode.F_CreatorUserId = SendModel.F_CreatorUserId;
                        newSendMode.F_DealState = SendModel.F_DealState;
                        newSendMode.F_IsCashBack = SendModel.F_IsCashBack;
                        newSendMode.F_IsVerificationCode = SendModel.F_IsVerificationCode;
                        newSendMode.F_Level = SendModel.F_Level;
                        newSendMode.F_Operator = SendModel.F_Operator;
                        newSendMode.F_PhoneCode = SendModel.F_PhoneCode;
                        newSendMode.F_Price = SendModel.F_Price;
                        newSendMode.F_ProtocolType = SendModel.F_ProtocolType;
                        newSendMode.F_Province = SendModel.F_Province;
                        newSendMode.F_Reissue = SendModel.F_Reissue;
                        newSendMode.F_RootId = SendModel.F_RootId;
                        newSendMode.F_SendTime = SendModel.F_SendTime;
                        newSendMode.F_SignLocation = SendModel.F_SignLocation;
                        newSendMode.F_Synchro = SendModel.F_Synchro;
                        newSendMode.F_UserId = SendModel.F_UserId;
                        newSendMode.F_UserSignature = SendModel.F_UserSignature;
                        newSendMode.F_BlackWhite = SendModel.F_BlackWhite;
                        newSendMode.SMC_F_Id = SendModel.SMC_F_Id;
                        //================================================================
                        newSendMode.F_LongsmsCount = 1;
                        newSendMode.F_Id = Guid.NewGuid().ToString();
                        newSendMode.F_SmsContent = SmsContent;
                        DealedSendList.Add(newSendMode);
                    }
                }
            }
            
            else//通道不开启退订
            {
                SendModel.F_LongsmsCount = GetLongSmsCount(SendModel.F_ChannelId.ObjToInt(), SendModel.F_SmsContent);//短信计数,添加的标记字符也算在内
                if (SendModel.F_LongsmsCount <= 0)
                {
                    if (SendModel.F_LongsmsCount == 0)
                        SendModel.F_DealState = 9;//不支持长短信发送，拆分失败
                    else
                        SendModel.F_DealState = 2;//失败
                }
                if (SendModel.F_SignLocation == 1 || SendModel.F_SignLocation == 2)//开启签名
                {
                    int SplitSmsCount = ChannelMaxLength - SendModel.F_UserSignature.Length - 4 - 2;//每条短信的的正文内容数量,4是：“1/n(”,2是签名的【】
                    if (SendModel.F_UserSignature.Length + SmsContent.Length + 2 > ChannelMaxLength)//判断是否拆大段
                    {
                        decimal Count = Math.Ceiling(Convert.ToDecimal(SendModel.F_SmsContent.Length) / SplitSmsCount);//向上取整数
                        for (int i = 0; i < Count; i++)
                        {
                            string newsmscontent = null;
                            Sev_SendDateDetail newSendMode = new Sev_SendDateDetail();//生成实体对象

                            //继承内容
                            newSendMode.F_AccessNumber = SendModel.F_AccessNumber;
                            newSendMode.F_Buckle = SendModel.F_Buckle;
                            newSendMode.F_ChannelId = SendModel.F_ChannelId;
                            newSendMode.F_ChannelSignature = SendModel.F_ChannelSignature;
                            newSendMode.F_CreatorTime = SendModel.F_CreatorTime;
                            newSendMode.F_CreatorUserId = SendModel.F_CreatorUserId;
                            newSendMode.F_DealState = SendModel.F_DealState;
                            newSendMode.F_IsCashBack = SendModel.F_IsCashBack;
                            newSendMode.F_IsVerificationCode = SendModel.F_IsVerificationCode;
                            newSendMode.F_Level = SendModel.F_Level;
                            newSendMode.F_Operator = SendModel.F_Operator;
                            newSendMode.F_PhoneCode = SendModel.F_PhoneCode;
                            newSendMode.F_Price = SendModel.F_Price;
                            newSendMode.F_ProtocolType = SendModel.F_ProtocolType;
                            newSendMode.F_Province = SendModel.F_Province;
                            newSendMode.F_Reissue = SendModel.F_Reissue;
                            newSendMode.F_RootId = SendModel.F_RootId;
                            newSendMode.F_SendTime = SendModel.F_SendTime;
                            newSendMode.F_SignLocation = SendModel.F_SignLocation;
                            newSendMode.F_Synchro = SendModel.F_Synchro;
                            newSendMode.F_UserId = SendModel.F_UserId;
                            newSendMode.F_UserSignature = SendModel.F_UserSignature;
                            newSendMode.F_BlackWhite = SendModel.F_BlackWhite;
                            newSendMode.SMC_F_Id = SendModel.SMC_F_Id;
                            //================================================================
                            newSendMode.F_Id = Guid.NewGuid().ToString();
                            if (SendModel.F_SignLocation == 1)//签名在前
                            {
                                if (((i + 1) * SplitSmsCount) < SmsContent.Length)//避免超出长度
                                {
                                    newsmscontent = "【" + SendModel.F_UserSignature + "】" + (i + 1) + "/" + Count + ")" + SmsContent.Substring(i * SplitSmsCount,  SplitSmsCount);
                                }
                                else
                                {
                                    newsmscontent = "【" + SendModel.F_UserSignature + "】" + (i + 1) + "/" + Count + ")" + SmsContent.Substring(i * SplitSmsCount, SmsContent.Length - i * SplitSmsCount);
                                }
                            }
                            else//签名在后
                            {
                                if (((i + 1) * SplitSmsCount) < SmsContent.Length)//避免超出长度
                                {
                                    newsmscontent = (i + 1) + "/" + Count + ")" + SmsContent.Substring(i * SplitSmsCount, SplitSmsCount) + "【" + SendModel.F_UserSignature + "】";
                                }
                                else
                                {
                                    newsmscontent = (i + 1) + "/" + Count + ")" + SmsContent.Substring(i * SplitSmsCount, SmsContent.Length - i * SplitSmsCount) + "【" + SendModel.F_UserSignature + "】";
                                }
                            }
                            newSendMode.F_SmsContent = newsmscontent;//新拆分的大段信息内容
                            newSendMode.F_LongsmsCount = Count.ObjToInt();
                            DealedSendList.Add(newSendMode);
                        }
                    }
                    else//不需要拆大段
                    {
                        string newsmscontent = null;
                        if (SendModel.F_SignLocation == 1)//签名在前
                            newsmscontent = "【" + SendModel.F_UserSignature + "】" + SmsContent;
                        else//签名在后
                            newsmscontent = SmsContent + "【" + SendModel.F_UserSignature + "】";

                        Sev_SendDateDetail newSendMode = new Sev_SendDateDetail();
                        //继承内容
                        newSendMode.F_AccessNumber = SendModel.F_AccessNumber;
                        newSendMode.F_Buckle = SendModel.F_Buckle;
                        newSendMode.F_ChannelId = SendModel.F_ChannelId;
                        newSendMode.F_ChannelSignature = SendModel.F_ChannelSignature;
                        newSendMode.F_CreatorTime = SendModel.F_CreatorTime;
                        newSendMode.F_CreatorUserId = SendModel.F_CreatorUserId;
                        newSendMode.F_DealState = SendModel.F_DealState;
                        newSendMode.F_IsCashBack = SendModel.F_IsCashBack;
                        newSendMode.F_IsVerificationCode = SendModel.F_IsVerificationCode;
                        newSendMode.F_Level = SendModel.F_Level;
                        newSendMode.F_Operator = SendModel.F_Operator;
                        newSendMode.F_PhoneCode = SendModel.F_PhoneCode;
                        newSendMode.F_Price = SendModel.F_Price;
                        newSendMode.F_ProtocolType = SendModel.F_ProtocolType;
                        newSendMode.F_Province = SendModel.F_Province;
                        newSendMode.F_Reissue = SendModel.F_Reissue;
                        newSendMode.F_RootId = SendModel.F_RootId;
                        newSendMode.F_SendTime = SendModel.F_SendTime;
                        newSendMode.F_SignLocation = SendModel.F_SignLocation;
                        newSendMode.F_Synchro = SendModel.F_Synchro;
                        newSendMode.F_UserId = SendModel.F_UserId;
                        newSendMode.F_UserSignature = SendModel.F_UserSignature;
                        newSendMode.F_BlackWhite = SendModel.F_BlackWhite;
                        newSendMode.SMC_F_Id = SendModel.SMC_F_Id;
                        //================================================================
                        newSendMode.F_Id = Guid.NewGuid().ToString();
                        newSendMode.F_LongsmsCount = 1;
                        newSendMode.F_SmsContent = SmsContent;
                        DealedSendList.Add(newSendMode);
                    }
                }
                else//不开启签名
                {
                    int SplitSmsCount = ChannelMaxLength - 4;//每条短信的的正文内容数量,4是：“1/n(”
                    if (SmsContent.Length > ChannelMaxLength)//拆大段
                    {
                        decimal Count = Math.Ceiling(Convert.ToDecimal(SendModel.F_SmsContent.Length) / SplitSmsCount);//向上取整数
                        for (int i = 0; i < Count; i++)
                        {
                            Sev_SendDateDetail newSendMode = new Sev_SendDateDetail();
                            //继承内容
                            newSendMode.F_AccessNumber = SendModel.F_AccessNumber;
                            newSendMode.F_Buckle = SendModel.F_Buckle;
                            newSendMode.F_ChannelId = SendModel.F_ChannelId;
                            newSendMode.F_ChannelSignature = SendModel.F_ChannelSignature;
                            newSendMode.F_CreatorTime = SendModel.F_CreatorTime;
                            newSendMode.F_CreatorUserId = SendModel.F_CreatorUserId;
                            newSendMode.F_DealState = SendModel.F_DealState;
                            newSendMode.F_IsCashBack = SendModel.F_IsCashBack;
                            newSendMode.F_IsVerificationCode = SendModel.F_IsVerificationCode;
                            newSendMode.F_Level = SendModel.F_Level;
                            newSendMode.F_Operator = SendModel.F_Operator;
                            newSendMode.F_PhoneCode = SendModel.F_PhoneCode;
                            newSendMode.F_Price = SendModel.F_Price;
                            newSendMode.F_ProtocolType = SendModel.F_ProtocolType;
                            newSendMode.F_Province = SendModel.F_Province;
                            newSendMode.F_Reissue = SendModel.F_Reissue;
                            newSendMode.F_RootId = SendModel.F_RootId;
                            newSendMode.F_SendTime = SendModel.F_SendTime;
                            newSendMode.F_SignLocation = SendModel.F_SignLocation;
                            newSendMode.F_Synchro = SendModel.F_Synchro;
                            newSendMode.F_UserId = SendModel.F_UserId;
                            newSendMode.F_UserSignature = SendModel.F_UserSignature;
                            newSendMode.F_BlackWhite = SendModel.F_BlackWhite;
                            newSendMode.SMC_F_Id = SendModel.SMC_F_Id;
                            //================================================================
                            newSendMode.F_Id = Guid.NewGuid().ToString();
                            string newsmscontent = null;
                            if ((i + 1) * SplitSmsCount < SmsContent.Length)
                                newsmscontent = (i + 1) + "/" + Count + ")" + SmsContent.Substring(i * SplitSmsCount, SplitSmsCount);
                            else
                                newsmscontent = (i + 1) + "/" + Count + ")" + SmsContent.Substring(i * SplitSmsCount, SmsContent.Length - i * SplitSmsCount);

                            newSendMode.F_SmsContent = newsmscontent;
                            newSendMode.F_LongsmsCount = Count.ObjToInt();
                            DealedSendList.Add(newSendMode);
                        }
                    }
                    else//不拆大段
                    {
                        Sev_SendDateDetail newSendMode = new Sev_SendDateDetail();
                        //继承内容
                        newSendMode.F_AccessNumber = SendModel.F_AccessNumber;
                        newSendMode.F_Buckle = SendModel.F_Buckle;
                        newSendMode.F_ChannelId = SendModel.F_ChannelId;
                        newSendMode.F_ChannelSignature = SendModel.F_ChannelSignature;
                        newSendMode.F_CreatorTime = SendModel.F_CreatorTime;
                        newSendMode.F_CreatorUserId = SendModel.F_CreatorUserId;
                        newSendMode.F_DealState = SendModel.F_DealState;
                        newSendMode.F_IsCashBack = SendModel.F_IsCashBack;
                        newSendMode.F_IsVerificationCode = SendModel.F_IsVerificationCode;
                        newSendMode.F_Level = SendModel.F_Level;
                        newSendMode.F_Operator = SendModel.F_Operator;
                        newSendMode.F_PhoneCode = SendModel.F_PhoneCode;
                        newSendMode.F_Price = SendModel.F_Price;
                        newSendMode.F_ProtocolType = SendModel.F_ProtocolType;
                        newSendMode.F_Province = SendModel.F_Province;
                        newSendMode.F_Reissue = SendModel.F_Reissue;
                        newSendMode.F_RootId = SendModel.F_RootId;
                        newSendMode.F_SendTime = SendModel.F_SendTime;
                        newSendMode.F_SignLocation = SendModel.F_SignLocation;
                        newSendMode.F_Synchro = SendModel.F_Synchro;
                        newSendMode.F_UserId = SendModel.F_UserId;
                        newSendMode.F_UserSignature = SendModel.F_UserSignature;
                        newSendMode.F_BlackWhite = SendModel.F_BlackWhite;
                        newSendMode.SMC_F_Id = SendModel.SMC_F_Id;
                        //================================================================
                        newSendMode.F_LongsmsCount = 1;
                        newSendMode.F_Id = Guid.NewGuid().ToString();
                        newSendMode.F_SmsContent = SmsContent;
                        DealedSendList.Add(newSendMode);
                    }
                }
            }
            try
            {
                using (SqlSugarClient db = new SqlSugarClient(connStr))//开启数据库连接
                {
                    //db.SqlBulkCopy(DealedSendList); //批量插入 海量数据插入Sev_SendDateDetail，插入之后就清除list//SEV_SendDataDetail数据插入（有黑名单验证）
                    db.InsertRange(DealedSendList);//批量插入数据（备用方法）,不会删除list
                    LogHelper.WriteLog("某一次拆大段的结束==" + SendModel.F_Id + "=========时间6：" + DateTime.Now.ToString());
                }
            }
            catch(Exception ex)
            { LogHelper.WriteLog("某一次拆大段异常：" + ex); }
        }


        //线程方式进行黑名单判断，返回SendData的List<>.，为发送做最后的准备。
        private Sev_SendDateDetail BlackWhiteChecked(Sev_SendDateDetail SendModel)
        {
            Sev_SendDateDetail NewModel = new Sev_SendDateDetail();
            NewModel.F_Level = SendModel.F_Level;
            NewModel.F_BlackWhite = SendModel.F_BlackWhite;
            NewModel.F_DealState = SendModel.F_DealState;
            foreach (var num in blackWhitelist)
            {
                if (SendModel.F_PhoneCode == num.F_Mobile)//如果联系电话在黑名单中，修改状态
                {
                    if (num.F_Sign.ObjToBool())
                    {
                        NewModel.F_BlackWhite = 1;//修改为黑名单
                        NewModel.F_DealState = 7;//报告状态为，黑名单
                        break;
                    }
                    else
                    {
                        NewModel.F_BlackWhite = 0;//系统白名单
                        NewModel.F_Level = 180;//修改优先级
                    }
                }
            }
            return NewModel;
        }

        //修改处理状态，处理中
        private void ChangeOperateStateStar(SMC_SendSms SendModel)
        {
            SendModel.F_OperateState = 1; //修改处理状态为处理ing   
            using (SqlSugarClient db = new SqlSugarClient(connStr))
            {
                db.Update(SendModel); //批量更新 发送列表内容的处理状态为处理中
            }
        }

        //修改发送状态,已发送
        private void ChangeSendState(SMC_SendSms SendModel)
        {
            SendModel.F_OperateState = 9; //修改处理状态为已处理
            if (SendModel.F_SendState != -1)
            {
                SendModel.F_SendState = 9;//修改发送状态为已发送（测试用）
                SendModel.F_SendTime = DateTime.Now;
            }
            else
            {
                //发送失败
            }
            using (SqlSugarClient db = new SqlSugarClient(connStr))
            {
                db.Update(SendModel); //批量更新 发送列表内容的处理状态为处理中
            }
        }

        //短信内容敏感词处理,线程方法||顺带判断是否为验证码短信
        private List<SMC_SendSms> DirtyWordRemove(List<SMC_SendSms> SendList)
        {
            Parallel.ForEach(SendList, item =>
            {
                StringSearch keyword = new StringSearch();
                keyword.SetKeywords(sensitive.Split('|'));//全局变量
                item.F_SmsContent = keyword.Replace(item.F_SmsContent, '*');//系统关键字去除
                string[] s = { "验证码" };//判断是否有关键字“验证码”
                keyword.SetKeywords(s);
                if (keyword.ContainsAny(item.F_SmsContent))
                {
                    item.F_IsVerificationCode = true;//修改，短信验证码标记为true
                }
                string ChannelSensitive = GetChannelSensitive(item.F_GroupChannelId);//获取通道关键字
                if (ChannelSensitive != null)
                {
                    keyword.SetKeywords(ChannelSensitive.Split('|'));
                    item.F_SmsContent = keyword.Replace(item.F_SmsContent, '*');//通道关键字去除
                }
                //内容中如果存在【】需替换为（）
                string[] change_1 = { "【" };
                keyword.SetKeywords(change_1);
                item.F_SmsContent = keyword.Replace(item.F_SmsContent, '(');
                string[] change_2 = { "】" };
                keyword.SetKeywords(change_2);
                item.F_SmsContent = keyword.Replace(item.F_SmsContent, ')');
            });
            return SendList;
        }

        //发送短信主程序
        int StartJob()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            LogHelper.WriteLog("流程开始============时间1：" + DateTime.Now.ToString());
            List<SMC_SendSms> ReadyList = GetSMCSendSmsList();//获取列表
            LogHelper.WriteLog("获取列表结束，开始敏感词============时间2：" + DateTime.Now.ToString() + "====个数" + ReadyList.Count());
            ReadyList = DirtyWordRemove(ReadyList);//敏感词替换，判断是否为验证码,处理list
            LogHelper.WriteLog("短信内容的敏感词，替换词处理结束，开始拆号============时间3：" + DateTime.Now.ToString() + "=====个数" + ReadyList.Count());
            int count = ReadyList.Count();//计数

            SubmitSevSendDateDetailParallel(ReadyList);//线程方式获取发送数据（黑名单检测通过）
            LogHelper.WriteLog("拆号结束===========时间4：" + DateTime.Now.ToString());


            stopWatch.Stop();
            Console.WriteLine("线程============" + stopWatch.ElapsedMilliseconds);
            return count;
        }

//****************************************************************************修改，获取数据**************************************************************************************
       
        //取得发送表数据，已审核，待发送，处理中，F_DealState=9，F_SendState=0，F_OperateState=0，//已审核，未处理，未发送//根据优先级排序(大的在前）
        private List<SMC_SendSms> GetSMCSendSmsList()
        {
            List<SMC_SendSms> list_smc_sendsms = new List<SMC_SendSms>();
            using (SqlSugarClient db = new SqlSugarClient(connStr))//开启数据库连接
            {
                list_smc_sendsms = db.SqlQuery<SMC_SendSms>("select F_Id,F_MobileList,F_CreatorUserId,F_CreatorTime,F_MobileCount,F_SmsContent,F_RootId,F_GroupChannelId,F_DealState,F_SendState,F_Priority,F_SendSign,F_SendTime,F_OperateState from SMC_SendSms where F_DealState=9 and F_SendState=0 and F_OperateState=0 and F_IsTimer='false' ORDER BY F_Priority DESC");
            }
            return list_smc_sendsms;
        }

        //取得黑白名单列表
        private static List<Models.OC_BlackList> GetBlackWhiteList()
        {
            List<Models.OC_BlackList> list_blacklist = new List<Models.OC_BlackList>();
            using (SqlSugarClient db = new SqlSugarClient(connStr))//开启数据库连接
            {
                string sql = "select Mobile from TXL_BlackList";
                list_blacklist = db.SqlQuery<Models.OC_BlackList>(sql);
            }
            return list_blacklist;
        }

        //获取通道敏感词
        private string GetChannelSensitive(string GroupChannelId)
        {
            List<SMS_SensitiveWords> Sensitivelist = new List<SMS_SensitiveWords>();
            try
            {
                using (SqlSugarClient db = new SqlSugarClient(connStr))
                {
                    OC_GroupChannel GroupModel = db.Queryable<OC_GroupChannel>().SingleOrDefault(t => t.F_ID == GroupChannelId);
                    int MobileChannelId = GroupModel.F_MobileChannel.ObjToInt();
                    int UnicomChannelId = GroupModel.F_UnicomChannel.ObjToInt();
                    int TelecomChannelId = GroupModel.F_TelecomChannel.ObjToInt();
                    string sql = "select F_SensitiveWords from SMS_SensitiveWords where F_IsChannelKeyWord='1'and (F_ChannelId=@MobileChannelId or F_ChannelId=@UnicomChannelId or F_ChannelId=@TelecomChannelId)";
                    Sensitivelist = db.SqlQuery<SMS_SensitiveWords>(sql, new { MobileChannelId = MobileChannelId, UnicomChannelId = UnicomChannelId, TelecomChannelId = TelecomChannelId });
                    string SensitiveStr = null;
                    if (Sensitivelist.Count() > 0)
                    {
                        foreach (var item in Sensitivelist)
                        {
                            SensitiveStr += item.F_SensitiveWords + "|";
                        }
                    }
                    return SensitiveStr;
                }
            }
            catch
            {
                return null;
            }
        }

        //根据用户F_Id获取Id和24小时验证码发送量，1小时验证码发送量，每个号码接收短信次数限制量
        private Models.OC_UserInfo GetUserInfo(string F_Id)
        {
            using (SqlSugarClient db = new SqlSugarClient(connStr))
            {
                string sql = "select F_UserId from OC_UserInfo where F_Id=@F_Id";
                Models.OC_UserInfo data=db.SqlQuery<Models.OC_UserInfo>(sql,new { F_Id=F_Id}).Single();
                return data;
            }
        }

        //根据号码获取当日的验证码发送限制，根据号码获取当天的发送量
        private int DailyverificationCount(string PhoneCode)
        {
            int count = 0;
            using (SqlSugarClient db = new SqlSugarClient(connStr))
            {
                //获取某个号码当天发送次数
                count = db.Queryable<Sev_SendDateDetail>().Where(t => t.F_PhoneCode == PhoneCode && DateTime.Today < t.F_SendTime && t.F_SendTime < DateTime.Now).Count();
            }
            return count;
        }

        //根据号码获取一小时的验证码发送限制
        private int OneHourverificationCount(string PhoneCode)
        {
            int count = 0;
            using (SqlSugarClient db = new SqlSugarClient(connStr))
            {
                //获取某个号码当天发送次数
                count = db.Queryable<Sev_SendDateDetail>().Where(t => t.F_PhoneCode == PhoneCode && DateTime.Now.AddHours(-1) < t.F_SendTime && t.F_SendTime < DateTime.Now.AddHours(+1)).Count();
            }
            return count;
        }
        //根据号码获取运营商和归属地
        private string GetOperatoeAndLocation(string PhoneCode)
        {
            string codeandlocation = null;
            try
            {
                PhoneCode = PhoneCode.Substring(0, 7);//取号码的前7位进行判断
                if (PhoneInfoDic.ContainsKey(PhoneCode))//直接从字典里取数据
                {
                    codeandlocation = PhoneInfoDic[PhoneCode];
                    return codeandlocation;
                }
                else
                {
                    Sys_PhoneNumAreaInfo PhoneInfo = new Sys_PhoneNumAreaInfo();
                    using (SqlSugarClient db = new SqlSugarClient(connStr))
                    {
                        //PhoneInfo = db.Queryable<Sys_PhoneNumAreaInfo>().SingleOrDefault(t => t.F_NumSegment == PhoneCode);
                        string sql = "select F_Operator,F_AreaCode from Sys_PhoneNumAreaInfo where F_Id = @PhoneCode";//主键有聚焦索引，会更快
                                                                                                                      // Dictionary<string, string> list6 = db.SqlQuery<KeyValuePair<string, string>>("select F_Id,F_AreaCode from Student").ToDictionary(it => it.Key, it => it.Value);
                        PhoneInfo = db.SqlQuery<Sys_PhoneNumAreaInfo>(sql, new { PhoneCode = PhoneCode }).Single();
                    }
                    if (PhoneInfo.F_Operator == "中国移动" || PhoneInfo.F_Operator == "移动")
                        codeandlocation = "1" + ";";
                    else if (PhoneInfo.F_Operator == "中国联通" || PhoneInfo.F_Operator == "联通")
                        codeandlocation = "2" + ";";
                    else
                        codeandlocation = "3" + ";";

                    codeandlocation += PhoneInfo.F_AreaCode;
                    PhoneInfoDic.Add(PhoneCode,codeandlocation);//添加到键值对到电话信息字典
                    return codeandlocation;
                }
            }
            catch { return "0" + ";" + "未知"; }
          
        }

        //根据运营商获取组合通道内的基础通道
        private int GetChannelId(int Operator, string GroupChannelId,string Province)
        {
            int ChannelId=0;
            int SwitchChannelId = 0;
            try
            {
                int ProvinceId = Province.ObjToInt();
                using (SqlSugarClient db = new SqlSugarClient(connStr))
                {
                    if (Operator == 1)//移动
                        ChannelId = db.Queryable<OC_GroupChannel>().SingleOrDefault(t => t.F_ID == GroupChannelId).F_MobileChannel.ObjToInt();
                    else if (Operator == 2)//联通
                        ChannelId = db.Queryable<OC_GroupChannel>().SingleOrDefault(t => t.F_ID == GroupChannelId).F_UnicomChannel.ObjToInt();
                    else
                        ChannelId = db.Queryable<OC_GroupChannel>().SingleOrDefault(t => t.F_ID == GroupChannelId).F_TelecomChannel.ObjToInt();
                    try
                    {
                        SwitchChannelId = db.Queryable<OC_ChannelProvince>().SingleOrDefault(t => t.F_ChannelId == ChannelId && t.F_ProvinceId == ProvinceId).F_SwitchChannelId;
                        if (SwitchChannelId != 0)//根据不同省份来变化Id
                            ChannelId = SwitchChannelId;
                    }
                    catch
                    {
                        return ChannelId; //未识别省份的，用原来的通道
                    }
                    return ChannelId;
                }
            }
            catch
            { return 0; }
        }

        //根据通道Id获取对应的发送协议
        private int GetProtocol(int ChannelId)
        {
            int ProtocolType ;
            try
            {
                using (SqlSugarClient db = new SqlSugarClient(connStr))
                {
                    ProtocolType = db.Queryable<OC_ChannelConfig>().Single(t => t.F_ChannelId == ChannelId).F_ProtocolType;
                }
                return ProtocolType;
            }
            catch
            { return 1; }
        }

        //根据通道Id获取长短信计数
        private int GetLongSmsCount(int ChannelId, string smscontent)
        {
            int CharacterCount = 0;
            OC_BaseChannel BaseChannelModel = new OC_BaseChannel();
            try
            {
                using (SqlSugarClient db = new SqlSugarClient(connStr))
                {
                    BaseChannelModel = db.Queryable<OC_BaseChannel>().SingleOrDefault(t => t.Id == ChannelId);
                }
                CharacterCount = BaseChannelModel.F_CharacterCount.ObjToInt();
                int ContentCount = 1;
                if (CharacterCount * ContentCount < smscontent.Length)
                {
                    ContentCount++;
                }
                if (BaseChannelModel.F_LongSmsSign == false)
                {
                    if (ContentCount > 1)
                        return 0;//不支持长短信却发了长短信
                }
                return ContentCount;
            }
            catch
            {
                return -1;
            }
        }

        //根据通道Id取出组合通道model
        private OC_GroupChannel GetGroupChannel(string GroupChannelId)
        {
            OC_GroupChannel model = new OC_GroupChannel();
            using (SqlSugarClient db = new SqlSugarClient(connStr))
            {
                 model = db.Queryable<OC_GroupChannel>().SingleOrDefault(t => t.F_ID == GroupChannelId);
            }
            return model;
        }

        //判断余额
        private decimal ChenkBalance(string SmsContent,string MobileList,decimal ChannelPrice,string UserF_Id,OC_BaseChannel BaseChannelModel, string SendFId)
        {
            decimal Balance = 0;
            decimal cost = 0;
            decimal SMS_number = 0;
            decimal unitprice = 0;//每次发送的短信价格
            int SmsCount = 0;//计费短信条数
            string Subscrib = BaseChannelModel.F_unsubscribe;//根据基准移动通道Id获取是否开启退订，如果返回值不是null，就是开启退订。
            string UserSignature = GetUsersignature(UserF_Id);
            string[] mobilelist = MobileList.Split(',');
            int MobileCount = mobilelist.Count();//号码数
            int ChannelMaxLength = BaseChannelModel.F_LongSmsNumber.ObjToInt();//单长短信最大字符数，根据此来拆大段
            int F_SignLocation = BaseChannelModel.F_autograph;//签名位置，1前；2后；3无
            if (Subscrib != null ||Subscrib !="")//通道开启退订
            {
                StringSearch keyword = new StringSearch();
                string[] s = { Subscrib };//判断是否有退订字符
                keyword.SetKeywords(s);
                if (!keyword.ContainsAny(SmsContent))//内容中没有退订字符
                {
                    SmsContent += Subscrib;//加上通道规定的退订内容
                }

                if (F_SignLocation == 1 || F_SignLocation == 2)//开启签名
                {
                    if ((SmsContent.Length + UserSignature.Length + 2) > ChannelMaxLength)//需要拆大段
                    {
                        int SplitSmsCount = ChannelMaxLength - UserSignature.Length - 4 - 2;//每条大段的的正文内容数量,4是：“1/n(”,2是签名加上的【】
                        SmsCount = Math.Ceiling(Convert.ToDecimal(SmsContent.Length) / SplitSmsCount).ObjToInt();//预估拆成的大段de数目
                        decimal LetterCount = SmsContent.Length + (UserSignature.Length + 2 + 4) * SmsCount;//总字数：【内容字数+预估大段数*（签名+2+4）】,4是：“1/n(”,2是签名加上的【】
                        SMS_number = Math.Ceiling(LetterCount / 66);//单条计费信息条数：总字数/66=实际发送条数(向上取整)
                        unitprice = SMS_number * ChannelPrice;
                        cost = unitprice * MobileCount;//该批信息总费用：实际发送条数*号码个数*组合通道单价
                    }
                    else//不拆大段
                    {
                        decimal LetterCount = SmsContent.Length + UserSignature.Length + 2;//总字数。内容字数+签名+2 ,2是签名加上的【】
                        SMS_number = Math.Ceiling(LetterCount / 66);//单条计费信息条数：总字数/66=实际发送条数(向上取整)
                        unitprice = SMS_number * ChannelPrice;
                        cost = unitprice * MobileCount;
                    }

                }
                else//没有签名
                {
                    if (SmsContent.Length>ChannelMaxLength)//拆大段
                    {
                        int SplitSmsCount = ChannelMaxLength - 4 ;//每条大段的的正文内容数量,4是：“1/n(”
                        SmsCount = Math.Ceiling(Convert.ToDecimal(SmsContent.Length) / SplitSmsCount).ObjToInt();//预估拆成的大段de数目
                        decimal LetterCount = SmsContent.Length + 4 * SmsCount;//总字数：【内容字数+预估大段数*（4）】,4是：“1/n(”
                        SMS_number = Math.Ceiling(LetterCount / 66);//单条信息计费条数：总字数/66=实际发送条数(向上取整)
                        unitprice = SMS_number * ChannelPrice;
                        cost = unitprice * MobileCount;
                    }
                    else//不拆大段
                    {
                        SMS_number = Math.Ceiling(Convert.ToDecimal(SmsContent.Length) / 66);//单条计费信息条数：总字数/66=实际发送条数(向上取整)
                        unitprice = SMS_number * ChannelPrice;
                        cost = unitprice * MobileCount;
                    }
                }
            }
            else//不退订
            {
                if (F_SignLocation == 1 || F_SignLocation == 2)//开启签名
                {
                    if ((SmsContent.Length + UserSignature.Length + 2) > ChannelMaxLength)//需要拆大段
                    {
                        int SplitSmsCount = ChannelMaxLength - UserSignature.Length - 4 - 2;//每条大段的的正文内容数量,4是：“1/n(”,2是签名加上的【】
                        SmsCount = Math.Ceiling(Convert.ToDecimal(SmsContent.Length) / SplitSmsCount).ObjToInt();//预估拆成的大段de数目
                        decimal LetterCount = SmsContent.Length + (UserSignature.Length + 2 + 4) * SmsCount;//总字数：【内容字数+预估大段数*（签名+2+4）】,4是：“1/n(”,2是签名加上的【】
                        SMS_number = Math.Ceiling(LetterCount / 66);//单条计费信息条数：总字数/66=实际发送条数(向上取整)
                        unitprice = SMS_number * ChannelPrice;
                        cost = unitprice * MobileCount;//该批信息总费用：实际发送条数*号码个数*组合通道单价
                    }
                    else//不拆大段
                    {
                        decimal LetterCount = SmsContent.Length + UserSignature.Length + 2;//总字数。内容字数+签名+2 ,2是签名加上的【】
                        SMS_number = Math.Ceiling(LetterCount / 66);//单条计费信息条数：总字数/66=实际发送条数(向上取整)
                        unitprice = SMS_number * ChannelPrice;
                        cost = unitprice * MobileCount;
                    }

                }
                else//没有签名
                {
                    if (SmsContent.Length > ChannelMaxLength)//拆大段
                    {
                        int SplitSmsCount = ChannelMaxLength - 4;//每条大段的的正文内容数量,4是：“1/n(”
                        SmsCount = Math.Ceiling(Convert.ToDecimal(SmsContent.Length) / SplitSmsCount).ObjToInt();//预估拆成的大段de数目
                        decimal LetterCount = SmsContent.Length + 4 * SmsCount;//总字数：【内容字数+预估大段数*（4）】,4是：“1/n(”
                        SMS_number = Math.Ceiling(LetterCount / 66);//单条信息计费条数：总字数/66=实际发送条数(向上取整)
                        unitprice = SMS_number * ChannelPrice;
                        cost = unitprice * MobileCount;
                    }
                    else//不拆大段
                    {
                        SMS_number = Math.Ceiling(Convert.ToDecimal(SmsContent.Length) / 66);//单条计费信息条数：总字数/66=实际发送条数(向上取整)
                        unitprice = SMS_number * ChannelPrice;
                        cost = unitprice * MobileCount;
                    }
                }
            }

            using (SqlSugarClient db = new SqlSugarClient(connStr))
            {
                Models.OC_UserInfo usermodel = db.Queryable<Models.OC_UserInfo>().Single(t => t.F_Id == UserF_Id);
                if ((usermodel.F_Balance) * 100 > cost)//余额充足,用户余额是元，cost是分
                {
                    Balance = usermodel.F_Balance - cost / 100;//分转换成元
                    db.Update<Models.OC_UserInfo>(new { F_Balance = Balance }, it => it.F_Id == UserF_Id);//更新用户余额（扣费）
                    db.Update<SMC_SendSms>(new { F_Price = cost }, it => it.F_Id == SendFId);//更新发送表的金额数
                    return unitprice;
                }
                else//余额不足,不扣费，直接打回总条发送任务
                    return -1;
            }
        }

        //获取用户签名
        private string GetUsersignature(string F_Id)
        {
            string UserSignature;
            using (SqlSugarClient db = new SqlSugarClient(connStr))
            {
                string sql = "select F_Signature from Sys_User where F_Id=@F_Id";
                UserSignature = db.SqlQuery<Sys_User>(sql, new { F_Id = F_Id }).Single().F_Signature;
            }
                return UserSignature;
        }

        //获取基础通道信息
        private OC_BaseChannel GetBaseChannel(int BaseMobileChannelId)
        {
            OC_BaseChannel model = new OC_BaseChannel();
            try
            {
                using (SqlSugarClient db = new SqlSugarClient(connStr))
                {
                    model = db.Queryable<OC_BaseChannel>().SingleOrDefault(t => t.Id == BaseMobileChannelId);//签名位置，1前；2后；3无
                    return model;
                }
            }
            catch { return model; }
        }

        //修改处理状态为余额不足
        private void ChangesendStateDone(string F_Id)
        {
            using (SqlSugarClient db = new SqlSugarClient(connStr))
            {
                db.Update<SMC_SendSms>(new { F_OperateState = -1 }, it => it.F_Id == F_Id);
            }
        }

        //class newSenModel//封装一个实体类,防止干扰
        //{

        //    Sev_SendDateDetail senddatedetail = new Sev_SendDateDetail();
        //    public Sev_SendDateDetail newsenddatedetaiModel
        //    {
        //        get
        //        {
        //            return senddatedetail;
        //        }
        //    }
        //}

        //class RadomNum//泛型随机数列表用的随机数类，加了索引Id
        //{
        //    public int Id { get; set; }
        //}
    }
}
