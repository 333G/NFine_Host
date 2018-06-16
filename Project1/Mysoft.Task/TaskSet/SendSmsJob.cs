﻿using Models;
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

namespace Mysoft.Task.TaskSet
{
    /// <summary>
    /// 发送前的准备任务
    /// </summary>
    ///<remarks>DisallowConcurrentExecution属性标记任务不可并行，要是上一任务没运行完即使到了运行时间也不会运行</remarks>
    [DisallowConcurrentExecution]
    public class SendSmsJob : IJob
    {
        private static string connStr = SysConfig.SqlConnect;
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                //获取任务执行参数,任务启动时会读取配置文件TaskConfig.xml节点TaskParam的值传递过来、、好像是空值
                //object objParam = context.JobDetail.JobDataMap.Get("TaskParam");


                int listcount = StartJob();

                LogHelper.WriteLog("短信审核任务。当前系统时间:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss" + "操作了" + listcount + "条符合条件数据"));
            }
            catch (Exception ex)
            {
                JobExecutionException e2 = new JobExecutionException(ex);
                LogHelper.WriteLog("发送任务异常 ", ex);
                //1.立即重新执行任务 
                e2.RefireImmediately = true;
                //2 立即停止所有相关这个任务的触发器
                //e2.UnscheduleAllTriggers=true; 
            }
        }

        //进行标记啊啥的,不需要拆66字了
        private void GetFinalList(List<Sev_SendDateDetail> HttpSendList)
        {
            try
            {
                for (int x = 0; x < HttpSendList.Count(); x++)
                {
                    Sev_FinalSendDetail senditem = new Sev_FinalSendDetail();
                    senditem.F_Id = Guid.NewGuid().ToString();
                    senditem.F_SendId = HttpSendList[x].F_Id;
                    senditem.F_Level = HttpSendList[x].F_Level;
                    senditem.F_DealState = 0;//默认未发送
                    senditem.F_Report = "0";//默认未收到回复报告
                    senditem.F_CreateTime = DateTime.Now;
                    senditem.F_SendTime = HttpSendList[x].F_SendTime;
                    senditem.F_Reissue = 0;//初始化补发次数为0；
                    senditem.F_Response = 0;//默认未收到应答
                    senditem.F_SmsContent = HttpSendList[x].F_SmsContent;//内容直接复制拆大段表的内容，不需要拆分
                    senditem.F_CreatorUserId = HttpSendList[x].F_CreatorUserId;
                    senditem.F_IsCashBack = false;
                    //decimal SmsCount = Math.Ceiling(Convert.ToDecimal(item.F_SmsContent.Length) / 66);//向上取整获取发送条数
                    //for (int i = 0; i < SmsCount; i++)
                    //{
                    //    senditem.F_Id = Guid.NewGuid().ToString();
                    //    senditem.F_SmsOrderNo = i + 1;
                    //    if ((i + 1) * 66 < item.F_SmsContent.Length)
                    //        senditem.F_SmsContent = item.F_SmsContent.Substring(i * 66, 66);//进行66个字的拆分
                    //    else
                    //        senditem.F_SmsContent = item.F_SmsContent.Substring(i * 66, item.F_SmsContent.Length-66);//避免超过长度

                    //    senditem.F_SmsCount = senditem.F_SmsContent.Length;
                    //    FinalList.Add(senditem);//加入队列
                    //}
                    using (SqlSugarClient db = new SqlSugarClient(connStr))//开启数据库连接
                    {
                        db.Insert(senditem);//单个插入，客户要求
                        db.Update<Sev_SendDateDetail>(new { F_DealState = 1 }, it => it.F_Id == HttpSendList[x].F_Id);//更新状态为已拆分状态
                    }
                }
            }
            catch(Exception ex)
            { LogHelper.WriteLog("发送任务出现异常：" + ex); }
        }

        //批量修改扣量数据的发送状态，有失败和成功
        private void ChangeBuckleSendState(List<Sev_SendDateDetail> BuckleSendList)
        {
            List<Sev_FinalSendDetail> DealedList = new List<Sev_FinalSendDetail>();
            Dictionary<string, List<Sev_FinalSendDetail>> dic = new Dictionary<string, List<Sev_FinalSendDetail>>();//新建字典类，判断发送批次<key:F_SendId;value:EntityList>
            try
            {
                for (int i = 0; i < BuckleSendList.Count; i++)
                {
                    if (!dic.ContainsKey(BuckleSendList[i].SMC_F_Id))//添加键值对
                    {
                        dic.Add(BuckleSendList[i].SMC_F_Id, new List<Sev_FinalSendDetail>());//添加新的键
                        Sev_FinalSendDetail senditem = new Sev_FinalSendDetail();
                        senditem.F_Id = Guid.NewGuid().ToString();
                        senditem.F_SendId = BuckleSendList[i].F_Id;
                        senditem.F_Level = BuckleSendList[i].F_Level;
                        senditem.F_DealState = 0;//默认未发送
                        senditem.F_Report = "0";//默认未收到回复报告
                        senditem.F_CreateTime = DateTime.Now;
                        senditem.F_Reissue = 0;//初始化补发次数为0；
                        senditem.F_Response = 0;//默认未收到应答
                        senditem.F_SmsContent = BuckleSendList[i].F_SmsContent;//内容直接复制拆大段表的内容，不需要拆分
                        senditem.F_IsCashBack = false;
                        senditem.F_SendTime = BuckleSendList[i].F_SendTime;
                        senditem.F_CreatorUserId = BuckleSendList[i].F_CreatorUserId;
                        dic[BuckleSendList[i].F_Id].Add(senditem);//添加Value值
                    }
                    else
                    {//添加新的元素
                        Sev_FinalSendDetail senditem = new Sev_FinalSendDetail();
                        senditem.F_Id = Guid.NewGuid().ToString();
                        senditem.F_SendId = BuckleSendList[i].F_Id;
                        senditem.F_Level = BuckleSendList[i].F_Level;
                        senditem.F_DealState = 0;//默认未发送
                        senditem.F_Report = "0";//默认未收到回复报告
                        senditem.F_CreateTime = DateTime.Now;
                        senditem.F_Reissue = 0;//初始化补发次数为0；
                        senditem.F_Response = 0;//默认未收到应答
                        senditem.F_SmsContent = BuckleSendList[i].F_SmsContent;//内容直接复制拆大段表的内容，不需要拆分
                        senditem.F_SendTime = BuckleSendList[i].F_SendTime;
                        senditem.F_CreatorUserId = BuckleSendList[i].F_CreatorUserId;
                        senditem.F_IsCashBack = false;
                        dic[BuckleSendList[i].SMC_F_Id].Add(senditem);
                    }
                    BuckleSendList[i].F_DealState = 1;//更新状态为已拆分状态
                }
                Parallel.ForEach(dic, item =>
                 {
                     decimal SuccessRate = (decimal)GetSendRate(item.Key) / 100; ;//成功率
                     decimal SendFalseNum = Math.Floor(item.Value.Count() * (1 - SuccessRate));//计算发送的失败条数，向下取整数
                     Random radom = new Random();
                     List<int> RadomList = new List<int>();//随机数列
                     for (int x = 0; x < SendFalseNum; x++)//获取不重复的随机数
                     {
                         int RadomNum = radom.Next(0, item.Value.Count() - 1);
                         if (!RadomList.Contains(RadomNum)) //集合list不包含num，就把num添加进list。这样保证随机数不重复
                         {
                             RadomList.Add(RadomNum);
                         }
                         else
                             x--;
                     }
                     for (int i = 0; i < item.Value.Count(); i++)
                     {
                         if (RadomList.Contains(i))//在随机数列中，判断为发送失败
                         {
                             item.Value[i].F_Report = "2";//失败报告
                             item.Value[i].F_Response = 2;//已收到应答
                             item.Value[i].F_DealState = 2;//已处理已发送，已经收到应答，失败
                         }
                         else
                         {
                             item.Value[i].F_Report = "3";//成功报告，区别于正常的成功。前台1，3都是成功，但是1是正常的3是扣量的
                             item.Value[i].F_Response = 2;//已收到应答
                             item.Value[i].F_DealState = 2;//已处理已发送，已经收到应答，成功
                         }
                         using (SqlSugarClient db = new SqlSugarClient(connStr))//开启数据库连接
                         {
                             db.Insert(item.Value);//单个插入，客户要求
                         }
                     }
                 });
                using (SqlSugarClient db = new SqlSugarClient(connStr))//开启数据库连接
                {
                    db.SqlBulkReplace(BuckleSendList);//单个插入，客户要求
                }
            }
            catch (Exception ex)
            { LogHelper.WriteLog("发送任务扣量方法出现异常：" + ex); }
            
        }

        int StartJob()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            List<Sev_SendDateDetail> HttpSendList = GetHttpSendList();
            List<Sev_SendDateDetail> BuckleList = GetBuckleList();
            int count = HttpSendList.Count() + BuckleList.Count();

            GetFinalList(HttpSendList);//赋值短信内容更新列表的拆分状态，并插入数据库
            ChangeBuckleSendState(BuckleList);//扣量列表做扣量处理（状态成功或者失败），并插入数据库      

            stopWatch.Stop();
            Console.WriteLine("线程============" + stopWatch.ElapsedMilliseconds);
            return count;
        }



        //*************************************************************************数据库操作方法***************************************************************************************

        //获取Http发送方式的发送数据，Sev_SendDateDetail的处理状态（F_DealState)为0，可拆分.发送协议(F_ProtocolType)为1,http发送方式，根据优先级（F_Level）排序
        private List<Sev_SendDateDetail> GetHttpSendList()
        {
            List<Sev_SendDateDetail> httpsendlist = new List<Sev_SendDateDetail>();
            using (SqlSugarClient db = new SqlSugarClient(connStr))
            {
                string sql = "select  F_Id,SMC_F_Id,F_SmsContent,F_Level,F_SendTime from Sev_SendDateDetail"//要批量更新状态，只能全部取过来，不然会更新出bug
                       + " where F_DealState=0  and F_ProtocolType=1 ORDER BY F_Level DESC";
                httpsendlist = db.SqlQuery<Sev_SendDateDetail>(sql);//F_CreatorUserId是用户的F_Id
            }
            return httpsendlist;
        }

        //获取Http发送方式的扣量数据，Sev_SendDateDetail的处理状态（F_DealState)为3，可拆分.发送协议(F_ProtocolType)为1,http发送方式
        private List<Sev_SendDateDetail> GetBuckleList()
        {
            List<Sev_SendDateDetail> buckleList = new List<Sev_SendDateDetail>();
            using (SqlSugarClient db = new SqlSugarClient(connStr))
            {
                string sql = "select * from Sev_SendDateDetail"//数据全取，为了之后好批量更新
                       + " where F_DealState=3 and F_ProtocolType=1 and (( F_SendTime+'00:30:00')< GETDATE())";//推迟30分钟模拟
                buckleList = db.SqlQuery<Sev_SendDateDetail>(sql);//F_CreatorUserId是用户的F_Id
            }
            return buckleList;
        }

        //根据F_SendId获取发送任务的组合通道F_ID再取得GroupChannel实体对象（发送率，成功率等）
        private decimal GetSendRate(string SendId)
        {
            OC_GroupChannel groupchannel = new OC_GroupChannel();
            decimal SuccessRate = 100;
            using (SqlSugarClient db = new SqlSugarClient(connStr))
            {
                string SMC_F_Id = db.Queryable<Sev_SendDateDetail>().SingleOrDefault(t => t.F_Id == SendId).SMC_F_Id;
                string GroupChannelFid = db.Queryable<SMC_SendSms>().SingleOrDefault(t => t.F_Id == SMC_F_Id).F_GroupChannelId;
                SuccessRate = Convert.ToDecimal(db.Queryable<OC_GroupChannel>().SingleOrDefault(t => t.F_ID == GroupChannelFid).F_SuccessRate);
            }
            return SuccessRate;
        }

    }
}