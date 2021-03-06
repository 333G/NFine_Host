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
    /// 测试任务
    /// </summary>
    ///<remarks>DisallowConcurrentExecution属性标记任务不可并行，要是上一任务没运行完即使到了运行时间也不会运行</remarks>
    [DisallowConcurrentExecution]
    class CashBackJob : IJob
    {
        private static string connStr = SysConfig.SqlConnect;
        public void Execute(IJobExecutionContext cintext)
        {
            try
            {
                int listcount = StartJob();
              //  LogHelper.WriteLog("返款任务。当前系统时间:" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss" + "操作了" + listcount + "条符合条件数据"));
            }
            catch (Exception e)
            {
                JobExecutionException e1 = new JobExecutionException(e);
                LogHelper.WriteLog("返款任务出现错误", e);
                //1.立即重新执行任务
                e1.RefireImmediately = true;
                //2.立即停止所有相关这个任务的触发器
                e1.UnscheduleAllTriggers = true;
            }
        }

        private void recharge(List<Sev_FinalSendDetail> SendDataList)
        {
            Parallel.ForEach(SendDataList, item => {
                //decimal cost = item.F_Price * item.F_LongsmsCount.ObjToDecimal();//计算总费用:单价*短信条数
                Sev_SendDateDetail SendModel = GetSendDetailModel(item.F_SendId);
                decimal Price = SendModel.F_Price;//单价
                decimal cost = Price * Math.Ceiling(Convert.ToDecimal(item.F_SmsContent.Count()) / 66);
                UpdateBalance(cost, SendModel.F_UserId,item.F_Id);//返款和更改返款状态
            });
        }

        int StartJob()
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            List<Sev_FinalSendDetail> CashBackList = GetCashBackList();
            int count = CashBackList.Count();

            recharge(CashBackList);

            stopWatch.Stop();
            Console.WriteLine("线程============" + stopWatch.ElapsedMilliseconds);
            return count;
        }
 
//****************************************************************************修改，获取数据**************************************************************************

        //获取需要返款的详单,发送失败,且没有返款
        private List<Sev_FinalSendDetail> GetCashBackList()
        {
            List<Sev_FinalSendDetail> CashBackList = new List<Sev_FinalSendDetail>();
            using (SqlSugarClient db = new SqlSugarClient(connStr))
            {
                CashBackList = db.GetList<Sev_FinalSendDetail>("select F_SendId,F_Report from Sev_FinalSendDetail  where F_IsCashBack = 'false' and F_Report<'0' ");
            }
            return CashBackList;
        }

        //更新余额(返款),更新返款状态
        private void UpdateBalance(decimal cost, int UserId,string F_Id)
        {
            OC_RechargeRecord RechargeModel = new OC_RechargeRecord();
            using (SqlSugarClient db = new SqlSugarClient(connStr))
            {
                RechargeModel.F_Id = Guid.NewGuid().ToString();
                Models.OC_UserInfo usermodel = db.Queryable<Models.OC_UserInfo>().Single(t => t.F_UserId == UserId);
                usermodel.F_Balance = usermodel.F_Balance + cost;//返款
                db.Update(usermodel);//更新
                db.Update<Sev_FinalSendDetail>(new { F_IsCashBack = true }, it => it.F_Id == F_Id);//详单状态变成已返款
                RechargeModel.F_Account = usermodel.F_Account;
                RechargeModel.F_PayTime = RechargeModel.F_CreatorTime = DateTime.Now;
                RechargeModel.F_CreatorUserId = "9f2ec079-7d0f-4fe2-90ab-8b09a8302aba";//admin
                RechargeModel.F_DeleteMark = false;
                RechargeModel.F_EnabledMark = true;
                RechargeModel.F_ManagerId = usermodel.F_ManagerId.ObjToInt();
                RechargeModel.F_OperatorId = 0;//代表系统
                RechargeModel.F_PayMode = 4;//支付方式，4，返款
                RechargeModel.F_Price= RechargeModel.F_TrueCash = RechargeModel.F_ShowCash = cost;//价格，实际金额，显示金额
                RechargeModel.F_RechargeOver =1;
                RechargeModel.F_RechargeStar = 1;
                RechargeModel.F_ShowDescription="自动返款";
                RechargeModel.F_State= "已付款";
                RechargeModel.F_TrueDescription="自动返款";
                RechargeModel.F_UserId = usermodel.F_UserId.ToString(); ;
                db.Insert(RechargeModel);
            }
        }
        //获取SendDetailModel
        private Sev_SendDateDetail GetSendDetailModel(string SendId)
        {
            Sev_SendDateDetail SendModel = new Sev_SendDateDetail();
            try
            {
                using (SqlSugarClient db = new SqlSugarClient(connStr))
                {
                    SendModel = db.Queryable<Sev_SendDateDetail>().Single(t => t.F_Id == SendId);
                    return SendModel;
                }
            }
            catch
            {
                return SendModel;
            }
        }
    }
}
