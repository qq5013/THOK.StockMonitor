SELECT     ROW_NUMBER() OVER (ORDER BY STOCKOUTID) ROW_INDEX, A.LINECODE, A.CIGARETTECODE, A.CIGARETTENAME, A.CHANNELCODE AS SORTCHANNELCODE, 
C.CHANNELCODE, B.CHANNELTYPE, STATE, C.CHANNELNAME
FROM         dbo.AS_STOCK_OUT A LEFT JOIN
                      dbo.AS_SC_CHANNELUSED B ON A.LINECODE = B.LINECODE AND A.CHANNELCODE = B.CHANNELCODE JOIN
                      dbo.v_stockchannel C ON A.CIGARETTECODE = C.CIGARETTECODE
WHERE     C.CHANNELTYPE = 1 AND C.CHANNELNAME='002'

--安阳补货监控系统数据库StockDB修改记录
--添加的表：AS_STATEMANAGER_LED，AS_STATEMANAGER_ORDER，AS_STATEMANAGER_SCANNER
--AS_STATEMANAGER_LED
USE [StockDB]
GO
/****** 对象:  Table [dbo].[AS_STATEMANAGER_LED]    脚本日期: 10/06/2011 13:48:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AS_STATEMANAGER_LED](
	[STATECODE] [varchar](50) COLLATE Chinese_PRC_CI_AS NOT NULL,
	[ROW_INDEX] [varchar](50) COLLATE Chinese_PRC_CI_AS NOT NULL,
	[VIEWNAME] [varchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[PLCSERVICESNAME] [varchar](50) COLLATE Chinese_PRC_CI_AS NOT NULL,
	[RELEASEITEMNAME] [varchar](50) COLLATE Chinese_PRC_CI_AS NOT NULL,
	[LEDCODE] [varchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[REMARK] [varchar](50) COLLATE Chinese_PRC_CI_AS NULL,
 CONSTRAINT [PK_AS_STATEMANAGER_LED] PRIMARY KEY CLUSTERED 
(
	[STATECODE] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF


--AS_STATEMANAGER_ORDER
USE [StockDB]
GO
/****** 对象:  Table [dbo].[AS_STATEMANAGER_ORDER]    脚本日期: 10/06/2011 13:48:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AS_STATEMANAGER_ORDER](
	[STATECODE] [varchar](50) COLLATE Chinese_PRC_CI_AS NOT NULL,
	[ROW_INDEX] [varchar](50) COLLATE Chinese_PRC_CI_AS NOT NULL,
	[VIEWNAME] [varchar](50) COLLATE Chinese_PRC_CI_AS NOT NULL,
	[PLCSERVICESNAME] [varchar](50) COLLATE Chinese_PRC_CI_AS NOT NULL,
	[ORDERITEMNAME] [varchar](50) COLLATE Chinese_PRC_CI_AS NOT NULL,
	[CHECKITEMNAME] [varchar](50) COLLATE Chinese_PRC_CI_AS NOT NULL,
	[ORDERQUANTITY] [varchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[REMARK] [varchar](50) COLLATE Chinese_PRC_CI_AS NULL,
 CONSTRAINT [PK_AS_STATEMANAGER_ORDER] PRIMARY KEY CLUSTERED 
(
	[STATECODE] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF


--AS_STATEMANAGER_SCANNER
USE [StockDB]
GO
/****** 对象:  Table [dbo].[AS_STATEMANAGER_SCANNER]    脚本日期: 10/06/2011 13:49:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AS_STATEMANAGER_SCANNER](
	[STATECODE] [varchar](50) COLLATE Chinese_PRC_CI_AS NOT NULL,
	[ROW_INDEX] [varchar](50) COLLATE Chinese_PRC_CI_AS NOT NULL,
	[VIEWNAME] [varchar](50) COLLATE Chinese_PRC_CI_AS NOT NULL,
	[PLCSERVICESNAME] [varchar](50) COLLATE Chinese_PRC_CI_AS NOT NULL,
	[RELEASEITEMNAME] [varchar](50) COLLATE Chinese_PRC_CI_AS NOT NULL,
	[LEDCODE] [varchar](50) COLLATE Chinese_PRC_CI_AS NULL,
	[REMARK] [varchar](50) COLLATE Chinese_PRC_CI_AS NULL,
 CONSTRAINT [PK_AS_STATEMANEGER_SCANNER] PRIMARY KEY CLUSTERED 
(
	[STATECODE] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF

--修改的表：AS_STOCK_OUT，AS_STOCK_OUT_BATCH
--AS_STOCK_OUT
USE [StockDB]
GO
/****** 对象:  Table [dbo].[AS_STOCK_OUT]    脚本日期: 10/06/2011 13:51:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AS_STOCK_OUT](
	[STOCKOUTID] [int] NOT NULL,
	[BATCHNO] [int] NOT NULL,
	[LINECODE] [char](2) COLLATE Chinese_PRC_CI_AS NOT NULL,
	[SORTNO] [int] NOT NULL,
	[SERIALNO] [int] NOT NULL,
	[CIGARETTECODE] [varchar](50) COLLATE Chinese_PRC_CI_AS NOT NULL,
	[CIGARETTENAME] [varchar](50) COLLATE Chinese_PRC_CI_AS NOT NULL,
	[BARCODE] [varchar](50) COLLATE Chinese_PRC_CI_AS NOT NULL,
	[CHANNELCODE] [varchar](50) COLLATE Chinese_PRC_CI_AS NOT NULL,
	[LED_STATE] [char](1) COLLATE Chinese_PRC_CI_AS NOT NULL CONSTRAINT [DF__AS_STOCK___ELABE__09DE7BCC]  DEFAULT ((0)),
	[SCAN_STATE_01] [char](1) COLLATE Chinese_PRC_CI_AS NOT NULL CONSTRAINT [DF_AS_STOCK_OUT_SCAN_STATE_01]  DEFAULT ((0)),
	[SCAN_STATE_02] [char](1) COLLATE Chinese_PRC_CI_AS NOT NULL CONSTRAINT [DF_AS_STOCK_OUT_SCAN_STATE_02]  DEFAULT ((0)),
	[SCAN_STATE_03] [char](1) COLLATE Chinese_PRC_CI_AS NOT NULL CONSTRAINT [DF_AS_STOCK_OUT_ISSCAN]  DEFAULT ((0)),
	[SCAN_STATE_04] [char](1) COLLATE Chinese_PRC_CI_AS NOT NULL CONSTRAINT [DF_AS_STOCK_OUT_SCAN_STATE_04]  DEFAULT ((0)),
	[SCAN_STATE_05] [char](1) COLLATE Chinese_PRC_CI_AS NOT NULL CONSTRAINT [DF_AS_STOCK_OUT_SCAN_STATE_05]  DEFAULT ((0)),
	[SUPPLYCAR_STATE] [char](1) COLLATE Chinese_PRC_CI_AS NOT NULL CONSTRAINT [DF_AS_STOCK_OUT_SUPPLYCARSTATE]  DEFAULT ((0)),
	[STATE] [char](1) COLLATE Chinese_PRC_CI_AS NOT NULL CONSTRAINT [DF__AS_STOCK___STATE__0AD2A005]  DEFAULT ((0)),
	[ORDERDATE] [datetime] NULL,
 CONSTRAINT [PK__AS_STOCK_OUT__08EA5793] PRIMARY KEY CLUSTERED 
(
	[STOCKOUTID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF

--AS_STOCK_OUT_BATCH
USE [StockDB]
GO
/****** 对象:  Table [dbo].[AS_STOCK_OUT_BATCH]    脚本日期: 10/06/2011 13:52:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[AS_STOCK_OUT_BATCH](
	[BATCHNO] [int] NOT NULL,
	[LINECODE] [char](2) COLLATE Chinese_PRC_CI_AS NOT NULL,
	[SORTNO] [int] NOT NULL,
	[CHANNELGROUP] [int] NOT NULL CONSTRAINT [DF_AS_STOCK_OUT_BATCH_CHANNELGROUP]  DEFAULT ((0)),
	[QUANTITY] [int] NOT NULL,
	[OUTQUANTITY] [int] NOT NULL,
	[MODE] [char](1) COLLATE Chinese_PRC_CI_AS NOT NULL CONSTRAINT [DF__as_stock_b__MODE__07020F21]  DEFAULT ('1'),
	[CHANNELTYPE] [char](1) COLLATE Chinese_PRC_CI_AS NULL,
 CONSTRAINT [PK__as_stock_batch__060DEAE8] PRIMARY KEY CLUSTERED 
(
	[BATCHNO] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF


--添加的视图：V_STATE_LED02，V_STATE_ORDER01，V_STATE_ORDER02，V_STATE_SCANNER02，
--V_STATE_LED02
USE [StockDB]
GO
/****** 对象:  View [dbo].[V_STATE_LED02]    脚本日期: 10/06/2011 13:53:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[V_STATE_LED02]
AS
SELECT     ROW_NUMBER() OVER (ORDER BY STOCKOUTID) ROW_INDEX, A.LINECODE, A.CIGARETTECODE, A.CIGARETTENAME, A.CHANNELCODE AS SORTCHANNELCODE, 
C.CHANNELCODE, B.CHANNELTYPE, STATE, C.CHANNELNAME
FROM         dbo.AS_STOCK_OUT A LEFT JOIN
                      dbo.AS_SC_CHANNELUSED B ON A.LINECODE = B.LINECODE AND A.CHANNELCODE = B.CHANNELCODE JOIN
                      dbo.v_stockchannel C ON A.CIGARETTECODE = C.CIGARETTECODE
WHERE     C.CHANNELTYPE = 3

GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[42] 4[21] 2[15] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' ,@level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'VIEW', @level1name=N'V_STATE_LED02'

GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 ,@level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'VIEW', @level1name=N'V_STATE_LED02'

--V_STATE_ORDER01
USE [StockDB]
GO
/****** 对象:  View [dbo].[V_STATE_ORDER01]    脚本日期: 10/06/2011 13:54:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[V_STATE_ORDER01]
AS
SELECT     ROW_NUMBER() OVER (ORDER BY STOCKOUTID) ROW_INDEX, A.LINECODE, A.CIGARETTECODE, A.CIGARETTENAME, A.CHANNELCODE AS SORTCHANNELCODE, 
C.CHANNELCODE, B.CHANNELTYPE, STATE, C.CHANNELNAME
FROM         dbo.AS_STOCK_OUT A LEFT JOIN
                      dbo.AS_SC_CHANNELUSED B ON A.LINECODE = B.LINECODE AND A.CHANNELCODE = B.CHANNELCODE JOIN
                      dbo.v_stockchannel C ON A.CIGARETTECODE = C.CIGARETTECODE
WHERE     C.CHANNELTYPE = 2

GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[22] 4[11] 2[34] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 2205
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' ,@level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'VIEW', @level1name=N'V_STATE_ORDER01'

GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 ,@level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'VIEW', @level1name=N'V_STATE_ORDER01'

--V_STATE_ORDER02
USE [StockDB]
GO
/****** 对象:  View [dbo].[V_STATE_ORDER02]    脚本日期: 10/06/2011 13:56:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[V_STATE_ORDER02]
AS
SELECT     ROW_NUMBER() OVER (ORDER BY STOCKOUTID) ROW_INDEX, A.LINECODE, A.CIGARETTECODE, A.CIGARETTENAME, A.CHANNELCODE AS SORTCHANNELCODE, 
C.CHANNELCODE, B.CHANNELTYPE, STATE, C.CHANNELNAME
FROM         dbo.AS_STOCK_OUT A LEFT JOIN
                      dbo.AS_SC_CHANNELUSED B ON A.LINECODE = B.LINECODE AND A.CHANNELCODE = B.CHANNELCODE JOIN
                      dbo.v_stockchannel C ON A.CIGARETTECODE = C.CIGARETTECODE

GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[41] 4[20] 2[11] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' ,@level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'VIEW', @level1name=N'V_STATE_ORDER02'

GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 ,@level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'VIEW', @level1name=N'V_STATE_ORDER02'



--V_STATE_SCANNER02
USE [StockDB]
GO
/****** 对象:  View [dbo].[V_STATE_SCANNER02]    脚本日期: 10/06/2011 13:57:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[V_STATE_SCANNER02]
AS
SELECT     ROW_NUMBER() OVER (ORDER BY STOCKOUTID) ROW_INDEX, A.LINECODE, A.CIGARETTECODE, A.BARCODE, A.CIGARETTENAME, 
A.CHANNELCODE AS SORTCHANNELCODE, C.CHANNELCODE, B.SUPPLYADDRESS, B.CHANNELTYPE, STATE, C.CHANNELNAME
FROM         dbo.AS_STOCK_OUT A LEFT JOIN
                      dbo.AS_SC_CHANNELUSED B ON A.LINECODE = B.LINECODE AND A.CHANNELCODE = B.CHANNELCODE JOIN
                      dbo.v_stockchannel C ON A.CIGARETTECODE = C.CIGARETTECODE

GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' ,@level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'VIEW', @level1name=N'V_STATE_SCANNER02'

GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 ,@level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'VIEW', @level1name=N'V_STATE_SCANNER02'


