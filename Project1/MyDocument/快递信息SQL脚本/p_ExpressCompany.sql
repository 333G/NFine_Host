---------------------------数据字典生成工具(V2.5)--------------------------------
GO
IF NOT EXISTS(SELECT 1 FROM sysobjects WHERE id=OBJECT_ID('[p_ExpressCompany]'))
BEGIN
/*==============================================================*/
/* Table: p_ExpressCompany                                              */
/*==============================================================*/
CREATE TABLE [dbo].[p_ExpressCompany](
	[CompanyGUID] uniqueidentifier  NOT NULL  DEFAULT newsequentialid() ,
	[CompanyName] nvarchar(100)   ,
	[CompanyCode] nvarchar(100)   ,
	[CreatedOn] datetime   DEFAULT getdate() ,
	PRIMARY KEY(CompanyGUID)
)
	

declare @CurrentUser sysname
select @CurrentUser = user_name()
execute sp_addextendedproperty 'MS_Description', '快递公司信息','user', @CurrentUser, 'table', 'p_ExpressCompany'
execute sp_addextendedproperty 'MS_Description',  '快递公司GUID' ,'user', @CurrentUser, 'table', 'p_ExpressCompany', 'column', 'CompanyGUID'
execute sp_addextendedproperty 'MS_Description',  '快递公司名称' ,'user', @CurrentUser, 'table', 'p_ExpressCompany', 'column', 'CompanyName'
execute sp_addextendedproperty 'MS_Description',  '快递公司简称' ,'user', @CurrentUser, 'table', 'p_ExpressCompany', 'column', 'CompanyCode'
execute sp_addextendedproperty 'MS_Description',  '创建日期' ,'user', @CurrentUser, 'table', 'p_ExpressCompany', 'column', 'CreatedOn'

END
GO

/****初始化快递公司数据*****/
GO
IF NOT EXISTS(SELECT 1 FROM dbo.p_ExpressCompany)
BEGIN
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'6956f12f-a254-e511-8d70-00155d0c740d', N'圆通快递', N'yuantong')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'6a56f12f-a254-e511-8d70-00155d0c740d', N'申通快递', N'shentong')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'6b56f12f-a254-e511-8d70-00155d0c740d', N'顺丰快递', N'shunfeng')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'6c56f12f-a254-e511-8d70-00155d0c740d', N'韵达快递', N'yunda')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'6d56f12f-a254-e511-8d70-00155d0c740d', N'德邦物流', N'debangwuliu')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'6e56f12f-a254-e511-8d70-00155d0c740d', N'中通快递', N'zhongtong')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'6f56f12f-a254-e511-8d70-00155d0c740d', N'百世汇通', N'huitongkuaidi')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'7056f12f-a254-e511-8d70-00155d0c740d', N'邮政包裹', N'youzhengguonei')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'7156f12f-a254-e511-8d70-00155d0c740d', N'EMS', N'ems')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'7256f12f-a254-e511-8d70-00155d0c740d', N'安能物流', N'annengwuliu')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'7356f12f-a254-e511-8d70-00155d0c740d', N'安迅物流', N'anxl')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'7456f12f-a254-e511-8d70-00155d0c740d', N'包裹/平邮/挂号信', N'youzhengguonei')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'7556f12f-a254-e511-8d70-00155d0c740d', N'巴伦支快递', N'balunzhi')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'7656f12f-a254-e511-8d70-00155d0c740d', N'北青小红帽', N'xiaohongmao')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'7756f12f-a254-e511-8d70-00155d0c740d', N'百世汇通', N'huitongkuaidi')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'7856f12f-a254-e511-8d70-00155d0c740d', N'百福东方物流', N'baifudongfang')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'7956f12f-a254-e511-8d70-00155d0c740d', N'邦送物流', N'bangsongwuliu')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'7a56f12f-a254-e511-8d70-00155d0c740d', N'宝凯物流', N'lbbk')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'7b56f12f-a254-e511-8d70-00155d0c740d', N'百千诚物流', N'bqcwl')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'7c56f12f-a254-e511-8d70-00155d0c740d', N'博源恒通', N'byht')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'7d56f12f-a254-e511-8d70-00155d0c740d', N'COE（东方快递）', N'coe')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'7e56f12f-a254-e511-8d70-00155d0c740d', N'城市100', N'city100')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'7f56f12f-a254-e511-8d70-00155d0c740d', N'传喜物流', N'chuanxiwuliu')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'8056f12f-a254-e511-8d70-00155d0c740d', N'城际速递', N'chengjisudi')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'8156f12f-a254-e511-8d70-00155d0c740d', N'成都立即送', N'lijisong')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'8256f12f-a254-e511-8d70-00155d0c740d', N'出口易', N'chukou1')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'8356f12f-a254-e511-8d70-00155d0c740d', N'DHL快递（中国件）', N'dhl')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'8456f12f-a254-e511-8d70-00155d0c740d', N'DHL（国际件）', N'dhlen')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'8556f12f-a254-e511-8d70-00155d0c740d', N'DHL（德国件）', N'dhlde')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'8656f12f-a254-e511-8d70-00155d0c740d', N'德邦', N'debangwuliu')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'8756f12f-a254-e511-8d70-00155d0c740d', N'大田物流', N'datianwuliu')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'8856f12f-a254-e511-8d70-00155d0c740d', N'东方快递', N'coe')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'8956f12f-a254-e511-8d70-00155d0c740d', N'递四方', N'disifang')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'8a56f12f-a254-e511-8d70-00155d0c740d', N'大洋物流', N'dayangwuliu')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'8b56f12f-a254-e511-8d70-00155d0c740d', N'店通快递', N'diantongkuaidi')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'8c56f12f-a254-e511-8d70-00155d0c740d', N'德创物流', N'dechuangwuliu')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'8d56f12f-a254-e511-8d70-00155d0c740d', N'东红物流', N'donghong')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'8e56f12f-a254-e511-8d70-00155d0c740d', N'D速物流', N'dsukuaidi')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'8f56f12f-a254-e511-8d70-00155d0c740d', N'东瀚物流', N'donghanwl')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'9056f12f-a254-e511-8d70-00155d0c740d', N'达方物流', N'dfpost')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'9156f12f-a254-e511-8d70-00155d0c740d', N'EMS快递查询', N'ems')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'9256f12f-a254-e511-8d70-00155d0c740d', N'EMS国际快递查询', N'emsguoji')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'9356f12f-a254-e511-8d70-00155d0c740d', N'FedEx快递查询', N'fedex')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'9456f12f-a254-e511-8d70-00155d0c740d', N'FedEx国际件', N'fedex')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'9556f12f-a254-e511-8d70-00155d0c740d', N'FedEx（美国）', N'fedexus')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'9656f12f-a254-e511-8d70-00155d0c740d', N'凡客如风达', N'rufengda')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'9756f12f-a254-e511-8d70-00155d0c740d', N'飞康达物流', N'feikangda')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'9856f12f-a254-e511-8d70-00155d0c740d', N'飞豹快递', N'feibaokuaidi')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'9956f12f-a254-e511-8d70-00155d0c740d', N'飞狐快递', N'feihukuaidi')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'9a56f12f-a254-e511-8d70-00155d0c740d', N'凡宇速递', N'fanyukuaidi')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'9b56f12f-a254-e511-8d70-00155d0c740d', N'颿达国际', N'fandaguoji')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'9c56f12f-a254-e511-8d70-00155d0c740d', N'飞远配送', N'feiyuanvipshop')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'9d56f12f-a254-e511-8d70-00155d0c740d', N'国通快递', N'guotongkuaidi')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'9e56f12f-a254-e511-8d70-00155d0c740d', N'国际邮件查询', N'youzhengguoji')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'9f56f12f-a254-e511-8d70-00155d0c740d', N'港中能达物流', N'ganzhongnengda')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'a056f12f-a254-e511-8d70-00155d0c740d', N'挂号信/国内邮件', N'youzhengguonei')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'a156f12f-a254-e511-8d70-00155d0c740d', N'共速达', N'gongsuda')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'a256f12f-a254-e511-8d70-00155d0c740d', N'广通速递', N'gtongsudi')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'a356f12f-a254-e511-8d70-00155d0c740d', N'港快速递', N'gdkd')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'a456f12f-a254-e511-8d70-00155d0c740d', N'GATI快递', N'gaticn')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'a556f12f-a254-e511-8d70-00155d0c740d', N'高铁速递', N'hre')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'a656f12f-a254-e511-8d70-00155d0c740d', N'冠达快递', N'gda')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'a756f12f-a254-e511-8d70-00155d0c740d', N'华宇物流', N'tiandihuayu')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'a856f12f-a254-e511-8d70-00155d0c740d', N'恒路物流', N'hengluwuliu')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'a956f12f-a254-e511-8d70-00155d0c740d', N'好来运快递', N'hlyex')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'aa56f12f-a254-e511-8d70-00155d0c740d', N'华夏龙物流', N'huaxialongwuliu')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'ab56f12f-a254-e511-8d70-00155d0c740d', N'海航天天', N'tiantian')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'ac56f12f-a254-e511-8d70-00155d0c740d', N'河北建华', N'hebeijianhua')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'ad56f12f-a254-e511-8d70-00155d0c740d', N'海盟速递', N'haimengsudi')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'ae56f12f-a254-e511-8d70-00155d0c740d', N'华企快运', N'huaqikuaiyun')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'af56f12f-a254-e511-8d70-00155d0c740d', N'昊盛物流', N'haoshengwuliu')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'b056f12f-a254-e511-8d70-00155d0c740d', N'户通物流', N'hutongwuliu')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'b156f12f-a254-e511-8d70-00155d0c740d', N'华航快递', N'hzpl')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'b256f12f-a254-e511-8d70-00155d0c740d', N'黄马甲快递', N'huangmajia')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'b356f12f-a254-e511-8d70-00155d0c740d', N'合众速递（UCS）', N'ucs')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'b456f12f-a254-e511-8d70-00155d0c740d', N'皇家物流', N'pfcexpress')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'b556f12f-a254-e511-8d70-00155d0c740d', N'伙伴物流', N'huoban')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'b656f12f-a254-e511-8d70-00155d0c740d', N'佳吉物流', N'jiajiwuliu')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'b756f12f-a254-e511-8d70-00155d0c740d', N'佳怡物流', N'jiayiwuliu')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'b856f12f-a254-e511-8d70-00155d0c740d', N'加运美快递', N'jiayunmeiwuliu')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'b956f12f-a254-e511-8d70-00155d0c740d', N'急先达物流', N'jixianda')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'ba56f12f-a254-e511-8d70-00155d0c740d', N'京广速递快件', N'jinguangsudikuaijian')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'bb56f12f-a254-e511-8d70-00155d0c740d', N'晋越快递', N'jinyuekuaidi')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'bc56f12f-a254-e511-8d70-00155d0c740d', N'京东快递', N'jd')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'bd56f12f-a254-e511-8d70-00155d0c740d', N'捷特快递', N'jietekuaidi')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'be56f12f-a254-e511-8d70-00155d0c740d', N'久易快递', N'jiuyicn')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'bf56f12f-a254-e511-8d70-00155d0c740d', N'快捷快递', N'kuaijiesudi')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'c056f12f-a254-e511-8d70-00155d0c740d', N'康力物流', N'kangliwuliu')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'c156f12f-a254-e511-8d70-00155d0c740d', N'跨越物流', N'kuayue')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'c256f12f-a254-e511-8d70-00155d0c740d', N'快优达速递', N'kuaiyouda')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'c356f12f-a254-e511-8d70-00155d0c740d', N'快淘快递', N'kuaitao')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'c456f12f-a254-e511-8d70-00155d0c740d', N'联邦快递（国内）', N'lianbangkuaidi')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'c556f12f-a254-e511-8d70-00155d0c740d', N'联昊通物流', N'lianhaowuliu')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'c656f12f-a254-e511-8d70-00155d0c740d', N'龙邦速递', N'longbanwuliu')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'c756f12f-a254-e511-8d70-00155d0c740d', N'乐捷递', N'lejiedi')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'c856f12f-a254-e511-8d70-00155d0c740d', N'立即送', N'lijisong')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'c956f12f-a254-e511-8d70-00155d0c740d', N'蓝弧快递', N'lanhukuaidi')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'ca56f12f-a254-e511-8d70-00155d0c740d', N'民航快递', N'minghangkuaidi')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'cb56f12f-a254-e511-8d70-00155d0c740d', N'美国快递', N'meiguokuaidi')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'cc56f12f-a254-e511-8d70-00155d0c740d', N'门对门', N'menduimen')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'cd56f12f-a254-e511-8d70-00155d0c740d', N'明亮物流', N'mingliangwuliu')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'ce56f12f-a254-e511-8d70-00155d0c740d', N'民邦速递', N'minbangsudi')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'cf56f12f-a254-e511-8d70-00155d0c740d', N'闽盛快递', N'minshengkuaidi')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'd056f12f-a254-e511-8d70-00155d0c740d', N'能达速递', N'ganzhongnengda')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'd156f12f-a254-e511-8d70-00155d0c740d', N'偌亚奥国际', N'nuoyaao')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'd256f12f-a254-e511-8d70-00155d0c740d', N'南京晟邦物流', N'nanjingshengbang')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'd356f12f-a254-e511-8d70-00155d0c740d', N'平安达腾飞', N'pingandatengfei')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'd456f12f-a254-e511-8d70-00155d0c740d', N'陪行物流', N'peixingwuliu')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'd556f12f-a254-e511-8d70-00155d0c740d', N'全峰快递', N'quanfengkuaidi')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'd656f12f-a254-e511-8d70-00155d0c740d', N'全一快递', N'quanyikuaidi')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'd756f12f-a254-e511-8d70-00155d0c740d', N'全日通快递', N'quanritongkuaidi')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'd856f12f-a254-e511-8d70-00155d0c740d', N'全晨快递', N'quanchenkuaidi')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'd956f12f-a254-e511-8d70-00155d0c740d', N'7天连锁物流', N'sevendays')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'da56f12f-a254-e511-8d70-00155d0c740d', N'如风达快递', N'rufengda')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'db56f12f-a254-e511-8d70-00155d0c740d', N'日昱物流', N'riyuwuliu')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'dc56f12f-a254-e511-8d70-00155d0c740d', N'申通快递', N'shentong')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'dd56f12f-a254-e511-8d70-00155d0c740d', N'顺丰速运', N'shunfeng')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'de56f12f-a254-e511-8d70-00155d0c740d', N'速尔快递', N'suer')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'df56f12f-a254-e511-8d70-00155d0c740d', N'山东海红', N'haihongwangsong')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'e056f12f-a254-e511-8d70-00155d0c740d', N'盛辉物流', N'shenghuiwuliu')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'e156f12f-a254-e511-8d70-00155d0c740d', N'世运快递', N'shiyunkuaidi')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'e256f12f-a254-e511-8d70-00155d0c740d', N'盛丰物流', N'shengfengwuliu')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'e356f12f-a254-e511-8d70-00155d0c740d', N'上大物流', N'shangda')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'e456f12f-a254-e511-8d70-00155d0c740d', N'三态速递', N'santaisudi')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'e556f12f-a254-e511-8d70-00155d0c740d', N'赛澳递', N'saiaodi')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'e656f12f-a254-e511-8d70-00155d0c740d', N'申通E物流', N'shentong')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'e756f12f-a254-e511-8d70-00155d0c740d', N'圣安物流', N'shenganwuliu')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'e856f12f-a254-e511-8d70-00155d0c740d', N'山西红马甲', N'sxhongmajia')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'e956f12f-a254-e511-8d70-00155d0c740d', N'穗佳物流', N'suijiawuliu')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'ea56f12f-a254-e511-8d70-00155d0c740d', N'沈阳佳惠尔', N'syjiahuier')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'eb56f12f-a254-e511-8d70-00155d0c740d', N'上海林道货运', N'shlindao')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'ec56f12f-a254-e511-8d70-00155d0c740d', N'TNT快递', N'tnt')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'ed56f12f-a254-e511-8d70-00155d0c740d', N'天天快递', N'tiantian')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'ee56f12f-a254-e511-8d70-00155d0c740d', N'天地华宇', N'tiandihuayu')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'ef56f12f-a254-e511-8d70-00155d0c740d', N'通和天下', N'tonghetianxia')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'f056f12f-a254-e511-8d70-00155d0c740d', N'天纵物流', N'tianzong')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'f156f12f-a254-e511-8d70-00155d0c740d', N'UPS快递查询', N'ups')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'f256f12f-a254-e511-8d70-00155d0c740d', N'UPS国际快递', N'ups')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'f356f12f-a254-e511-8d70-00155d0c740d', N'UC优速快递', N'youshuwuliu')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'f456f12f-a254-e511-8d70-00155d0c740d', N'USPS美国邮政', N'usps')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'f556f12f-a254-e511-8d70-00155d0c740d', N'万象物流', N'wanxiangwuliu')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'f656f12f-a254-e511-8d70-00155d0c740d', N'微特派', N'weitepai')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'f756f12f-a254-e511-8d70-00155d0c740d', N'万家物流', N'wanjiawuliu')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'f856f12f-a254-e511-8d70-00155d0c740d', N'希优特快递', N'xiyoutekuaidi')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'f956f12f-a254-e511-8d70-00155d0c740d', N'新邦物流', N'xinbangwuliu')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'fa56f12f-a254-e511-8d70-00155d0c740d', N'信丰物流', N'xinfengwuliu')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'fb56f12f-a254-e511-8d70-00155d0c740d', N'新蛋物流', N'neweggozzo')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'fc56f12f-a254-e511-8d70-00155d0c740d', N'祥龙运通物流', N'xianglongyuntong')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'fd56f12f-a254-e511-8d70-00155d0c740d', N'西安城联速递', N'xianchengliansudi')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'fe56f12f-a254-e511-8d70-00155d0c740d', N'西安喜来快递', N'xilaikd')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'ff56f12f-a254-e511-8d70-00155d0c740d', N'圆通速递', N'yuantong')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'0057f12f-a254-e511-8d70-00155d0c740d', N'韵达快运', N'yunda')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'0157f12f-a254-e511-8d70-00155d0c740d', N'运通快递', N'yuntongkuaidi')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'0257f12f-a254-e511-8d70-00155d0c740d', N'邮政国内', N'youzhengguonei')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'0357f12f-a254-e511-8d70-00155d0c740d', N'邮政国际', N'youzhengguoji')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'0457f12f-a254-e511-8d70-00155d0c740d', N'远成物流', N'yuanchengwuliu')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'0557f12f-a254-e511-8d70-00155d0c740d', N'亚风速递', N'yafengsudi')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'0657f12f-a254-e511-8d70-00155d0c740d', N'优速快递', N'youshuwuliu')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'0757f12f-a254-e511-8d70-00155d0c740d', N'亿顺航', N'yishunhang')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'0857f12f-a254-e511-8d70-00155d0c740d', N'越丰物流', N'yuefengwuliu')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'0957f12f-a254-e511-8d70-00155d0c740d', N'源安达快递', N'yuananda')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'0a57f12f-a254-e511-8d70-00155d0c740d', N'原飞航物流', N'yuanfeihangwuliu')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'0b57f12f-a254-e511-8d70-00155d0c740d', N'邮政EMS速递', N'ems')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'0c57f12f-a254-e511-8d70-00155d0c740d', N'银捷速递', N'yinjiesudi')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'0d57f12f-a254-e511-8d70-00155d0c740d', N'一统飞鸿', N'yitongfeihong')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'0e57f12f-a254-e511-8d70-00155d0c740d', N'宇鑫物流', N'yuxinwuliu')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'0f57f12f-a254-e511-8d70-00155d0c740d', N'易通达', N'yitongda')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'1057f12f-a254-e511-8d70-00155d0c740d', N'邮必佳', N'youbijia')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'1157f12f-a254-e511-8d70-00155d0c740d', N'一柒物流', N'yiqiguojiwuliu')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'1257f12f-a254-e511-8d70-00155d0c740d', N'音素快运', N'yinsu')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'1357f12f-a254-e511-8d70-00155d0c740d', N'亿领速运', N'yilingsuyun')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'1457f12f-a254-e511-8d70-00155d0c740d', N'煜嘉物流', N'yujiawuliu')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'1557f12f-a254-e511-8d70-00155d0c740d', N'英脉物流', N'gml')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'1657f12f-a254-e511-8d70-00155d0c740d', N'云豹国际货运', N'leopard')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'1757f12f-a254-e511-8d70-00155d0c740d', N'中通快递', N'zhongtong')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'1857f12f-a254-e511-8d70-00155d0c740d', N'宅急送', N'zhaijisong')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'1957f12f-a254-e511-8d70-00155d0c740d', N'中铁快运', N'zhongtiewuliu')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'1a57f12f-a254-e511-8d70-00155d0c740d', N'中铁物流', N'ztky')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'1b57f12f-a254-e511-8d70-00155d0c740d', N'中邮物流', N'zhongyouwuliu')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'1c57f12f-a254-e511-8d70-00155d0c740d', N'中国东方(COE)', N'coe')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'1d57f12f-a254-e511-8d70-00155d0c740d', N'芝麻开门', N'zhimakaimen')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'1e57f12f-a254-e511-8d70-00155d0c740d', N'中国邮政快递', N'youzhengguonei')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'1f57f12f-a254-e511-8d70-00155d0c740d', N'郑州建华', N'zhengzhoujianhua')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'2057f12f-a254-e511-8d70-00155d0c740d', N'中速快件', N'zhongsukuaidi')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'2157f12f-a254-e511-8d70-00155d0c740d', N'中天万运', N'zhongtianwanyun')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'2257f12f-a254-e511-8d70-00155d0c740d', N'中睿速递', N'zhongruisudi')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'2357f12f-a254-e511-8d70-00155d0c740d', N'中外运速递', N'zhongwaiyun')
INSERT [dbo].[p_ExpressCompany] ([CompanyGUID], [CompanyName], [CompanyCode]) VALUES (N'2457f12f-a254-e511-8d70-00155d0c740d', N'增益速递', N'zengyisudi')

END
GO
