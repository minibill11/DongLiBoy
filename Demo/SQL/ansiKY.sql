USE [master]
GO
/****** Object:  Database [kongyadb]    Script Date: 2019/3/29 16:43:19 ******/
CREATE DATABASE [kongyadb]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'kongyadb', FILENAME = N'D:\Program Files\Microsoft SQL Server\MSSQL13.MSSQLSERVER\MSSQL\DATA\kongyadb.mdf' , SIZE = 24889344KB , MAXSIZE = UNLIMITED, FILEGROWTH = 10240KB )
 LOG ON 
( NAME = N'kongyadb_log', FILENAME = N'D:\Program Files\Microsoft SQL Server\MSSQL13.MSSQLSERVER\MSSQL\DATA\kongyadb_log.ldf' , SIZE = 729088KB , MAXSIZE = 1048576KB , FILEGROWTH = 65536KB )
GO
ALTER DATABASE [kongyadb] SET COMPATIBILITY_LEVEL = 100
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [kongyadb].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [kongyadb] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [kongyadb] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [kongyadb] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [kongyadb] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [kongyadb] SET ARITHABORT OFF 
GO
ALTER DATABASE [kongyadb] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [kongyadb] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [kongyadb] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [kongyadb] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [kongyadb] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [kongyadb] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [kongyadb] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [kongyadb] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [kongyadb] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [kongyadb] SET  DISABLE_BROKER 
GO
ALTER DATABASE [kongyadb] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [kongyadb] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [kongyadb] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [kongyadb] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [kongyadb] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [kongyadb] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [kongyadb] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [kongyadb] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [kongyadb] SET  MULTI_USER 
GO
ALTER DATABASE [kongyadb] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [kongyadb] SET DB_CHAINING OFF 
GO
ALTER DATABASE [kongyadb] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [kongyadb] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
ALTER DATABASE [kongyadb] SET DELAYED_DURABILITY = DISABLED 
GO
EXEC sys.sp_db_vardecimal_storage_format N'kongyadb', N'ON'
GO
ALTER DATABASE [kongyadb] SET QUERY_STORE = OFF
GO
USE [kongyadb]
GO
ALTER DATABASE SCOPED CONFIGURATION SET LEGACY_CARDINALITY_ESTIMATION = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET LEGACY_CARDINALITY_ESTIMATION = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 0;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET MAXDOP = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET PARAMETER_SNIFFING = ON;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET PARAMETER_SNIFFING = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET QUERY_OPTIMIZER_HOTFIXES = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET QUERY_OPTIMIZER_HOTFIXES = PRIMARY;
GO
USE [kongyadb]
GO
/****** Object:  User [zzb_ziyang]    Script Date: 2019/3/29 16:43:19 ******/
CREATE USER [zzb_ziyang] WITHOUT LOGIN WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [zzb_zhenbiao]    Script Date: 2019/3/29 16:43:19 ******/
CREATE USER [zzb_zhenbiao] FOR LOGIN [zzb_zhenbiao] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [zzb]    Script Date: 2019/3/29 16:43:19 ******/
CREATE USER [zzb] FOR LOGIN [zzb] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [weboperator]    Script Date: 2019/3/29 16:43:19 ******/
CREATE USER [weboperator] FOR LOGIN [weboperator] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [caijiji]    Script Date: 2019/3/29 16:43:19 ******/
CREATE USER [caijiji] FOR LOGIN [caijiji] WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_owner] ADD MEMBER [zzb_ziyang]
GO
ALTER ROLE [db_owner] ADD MEMBER [zzb_zhenbiao]
GO
ALTER ROLE [db_owner] ADD MEMBER [zzb]
GO
ALTER ROLE [db_owner] ADD MEMBER [weboperator]
GO
ALTER ROLE [db_owner] ADD MEMBER [caijiji]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [caijiji]
GO
/****** Object:  Table [dbo].[airbottle1]    Script Date: 2019/3/29 16:43:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[airbottle1](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[pressure] [float] NULL,
	[temperature] [float] NULL,
	[humidity] [float] NULL,
	[recordingTime] [datetime] NOT NULL,
	[statu] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[airbottle2]    Script Date: 2019/3/29 16:43:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[airbottle2](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[pressure] [float] NULL,
	[temperature] [float] NULL,
	[humidity] [float] NULL,
	[recordingTime] [datetime] NOT NULL,
	[statu] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[airbottle3]    Script Date: 2019/3/29 16:43:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[airbottle3](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[pressure] [float] NULL,
	[temperature] [float] NULL,
	[humidity] [float] NULL,
	[recordingTime] [datetime] NOT NULL,
	[statu] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[aircomp1]    Script Date: 2019/3/29 16:43:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[aircomp1](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[status] [int] NULL,
	[slight] [int] NULL,
	[severityint] [int] NULL,
	[pressure] [float] NULL,
	[temperature] [float] NULL,
	[current_u] [float] NULL,
	[current_v] [float] NULL,
	[current_w] [float] NULL,
	[recordingTime] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[aircomp2]    Script Date: 2019/3/29 16:43:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[aircomp2](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[status] [int] NULL,
	[slight] [int] NULL,
	[severity] [int] NULL,
	[pressure] [float] NULL,
	[temperature] [float] NULL,
	[current_u] [float] NULL,
	[current_v] [float] NULL,
	[current_w] [float] NULL,
	[recordingTime] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[aircomp3]    Script Date: 2019/3/29 16:43:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[aircomp3](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[status] [int] NULL,
	[slight] [int] NULL,
	[severity] [int] NULL,
	[pressure] [float] NULL,
	[temperature] [float] NULL,
	[current_u] [float] NULL,
	[current_v] [float] NULL,
	[current_w] [float] NULL,
	[recordingTime] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[dryer1]    Script Date: 2019/3/29 16:43:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[dryer1](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[pressure] [float] NULL,
	[temperature] [float] NULL,
	[humidity] [float] NULL,
	[recordingTime] [datetime] NOT NULL,
	[statu] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[dryer2]    Script Date: 2019/3/29 16:43:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[dryer2](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[pressure] [float] NULL,
	[temperature] [float] NULL,
	[humidity] [float] NULL,
	[recordingTime] [datetime] NOT NULL,
	[statu] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[headerpipe3inch]    Script Date: 2019/3/29 16:43:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[headerpipe3inch](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[pressure] [float] NULL,
	[temperature] [float] NULL,
	[humidity] [float] NULL,
	[recordingTime] [datetime] NOT NULL,
	[statu] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[headerpipe4inch]    Script Date: 2019/3/29 16:43:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[headerpipe4inch](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[pressure] [float] NULL,
	[temperature] [float] NULL,
	[humidity] [float] NULL,
	[recordingTime] [datetime] NOT NULL,
	[statu] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[room]    Script Date: 2019/3/29 16:43:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[room](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[temperature] [float] NULL,
	[humidity] [float] NULL,
	[recordingTime] [datetime] NOT NULL,
	[statu] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbalarm]    Script Date: 2019/3/29 16:43:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbalarm](
	[ID] [nvarchar](32) NULL,
	[DeviceKey] [nvarchar](32) NULL,
	[DeviceName] [nvarchar](max) NULL,
	[DeviceID] [int] NULL,
	[NodeID] [int] NULL,
	[Lng] [float] NULL,
	[Lat] [float] NULL,
	[AlarmType] [nvarchar](150) NULL,
	[AlarmMessage] [nvarchar](150) NULL,
	[AlarmRange] [nvarchar](150) NULL,
	[DataValue] [float] NULL,
	[RecordTime] [datetime] NULL,
	[HandingFlag] [int] NULL,
	[HandlingMethod] [nvarchar](max) NULL,
	[HandlingUser] [nvarchar](500) NULL,
	[HandlingTime] [datetime] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbhistory]    Script Date: 2019/3/29 16:43:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbhistory](
	[ID] [nvarchar](32) NULL,
	[DeviceKey] [nvarchar](32) NULL,
	[DeviceName] [nvarchar](500) NULL,
	[DeviceID] [int] NULL,
	[NodeID] [int] NULL,
	[Tem] [float] NULL,
	[Hum] [float] NULL,
	[Lng] [float] NULL,
	[Lat] [float] NULL,
	[CoordinateType] [smallint] NULL,
	[RecordTime] [datetime] NULL,
	[IsAlarmData] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbsyslog]    Script Date: 2019/3/29 16:43:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbsyslog](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [nvarchar](max) NULL,
	[Operate] [nvarchar](max) NULL,
	[Details] [nvarchar](max) NULL,
	[Result] [nvarchar](max) NULL,
	[IP] [nvarchar](max) NULL,
	[RecordTime] [datetime] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[THhistory]    Script Date: 2019/3/29 16:43:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[THhistory](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[DeviceID] [nchar](50) NULL,
	[NodeID] [nchar](50) NULL,
	[Tem] [float] NULL,
	[Hum] [float] NULL,
	[RecordTime] [datetime] NULL,
	[DeviceName] [nvarchar](500) NULL,
	[DeviceKey] [nchar](32) NULL,
 CONSTRAINT [PK_THhistory] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [DeviceID_NodeID_DeviceName-20190318-124017]    Script Date: 2019/3/29 16:43:20 ******/
CREATE NONCLUSTERED INDEX [DeviceID_NodeID_DeviceName-20190318-124017] ON [dbo].[THhistory]
(
	[DeviceID] ASC,
	[NodeID] ASC,
	[DeviceName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [r_index]    Script Date: 2019/3/29 16:43:20 ******/
CREATE NONCLUSTERED COLUMNSTORE INDEX [r_index] ON [dbo].[room]
(
	[recordingTime]
)WITH (DROP_EXISTING = OFF, COMPRESSION_DELAY = 0) ON [PRIMARY]
GO
ALTER TABLE [dbo].[airbottle1] ADD  DEFAULT (NULL) FOR [pressure]
GO
ALTER TABLE [dbo].[airbottle1] ADD  DEFAULT (NULL) FOR [temperature]
GO
ALTER TABLE [dbo].[airbottle1] ADD  DEFAULT (NULL) FOR [humidity]
GO
ALTER TABLE [dbo].[airbottle1] ADD  CONSTRAINT [DF__airbottle__recor__719CDDE7]  DEFAULT (CONVERT([char](24),getdate(),(120))) FOR [recordingTime]
GO
ALTER TABLE [dbo].[airbottle1] ADD  DEFAULT (NULL) FOR [statu]
GO
ALTER TABLE [dbo].[airbottle2] ADD  DEFAULT (NULL) FOR [pressure]
GO
ALTER TABLE [dbo].[airbottle2] ADD  DEFAULT (NULL) FOR [temperature]
GO
ALTER TABLE [dbo].[airbottle2] ADD  DEFAULT (NULL) FOR [humidity]
GO
ALTER TABLE [dbo].[airbottle2] ADD  CONSTRAINT [DF__airbottle__recor__7849DB76]  DEFAULT (CONVERT([char](24),getdate(),(120))) FOR [recordingTime]
GO
ALTER TABLE [dbo].[airbottle2] ADD  DEFAULT (NULL) FOR [statu]
GO
ALTER TABLE [dbo].[airbottle3] ADD  DEFAULT (NULL) FOR [pressure]
GO
ALTER TABLE [dbo].[airbottle3] ADD  DEFAULT (NULL) FOR [temperature]
GO
ALTER TABLE [dbo].[airbottle3] ADD  DEFAULT (NULL) FOR [humidity]
GO
ALTER TABLE [dbo].[airbottle3] ADD  CONSTRAINT [DF__airbottle__recor__7EF6D905]  DEFAULT (CONVERT([char](24),getdate(),(120))) FOR [recordingTime]
GO
ALTER TABLE [dbo].[airbottle3] ADD  DEFAULT (NULL) FOR [statu]
GO
ALTER TABLE [dbo].[aircomp1] ADD  DEFAULT (NULL) FOR [status]
GO
ALTER TABLE [dbo].[aircomp1] ADD  DEFAULT (NULL) FOR [slight]
GO
ALTER TABLE [dbo].[aircomp1] ADD  DEFAULT (NULL) FOR [severityint]
GO
ALTER TABLE [dbo].[aircomp1] ADD  DEFAULT (NULL) FOR [pressure]
GO
ALTER TABLE [dbo].[aircomp1] ADD  DEFAULT (NULL) FOR [temperature]
GO
ALTER TABLE [dbo].[aircomp1] ADD  DEFAULT (NULL) FOR [current_u]
GO
ALTER TABLE [dbo].[aircomp1] ADD  DEFAULT (NULL) FOR [current_v]
GO
ALTER TABLE [dbo].[aircomp1] ADD  DEFAULT (NULL) FOR [current_w]
GO
ALTER TABLE [dbo].[aircomp1] ADD  CONSTRAINT [DF__aircomp1__record__46B27FE2]  DEFAULT (CONVERT([char](24),getdate(),(120))) FOR [recordingTime]
GO
ALTER TABLE [dbo].[aircomp2] ADD  DEFAULT (NULL) FOR [status]
GO
ALTER TABLE [dbo].[aircomp2] ADD  DEFAULT (NULL) FOR [slight]
GO
ALTER TABLE [dbo].[aircomp2] ADD  DEFAULT (NULL) FOR [severity]
GO
ALTER TABLE [dbo].[aircomp2] ADD  DEFAULT (NULL) FOR [pressure]
GO
ALTER TABLE [dbo].[aircomp2] ADD  DEFAULT (NULL) FOR [temperature]
GO
ALTER TABLE [dbo].[aircomp2] ADD  DEFAULT (NULL) FOR [current_u]
GO
ALTER TABLE [dbo].[aircomp2] ADD  DEFAULT (NULL) FOR [current_v]
GO
ALTER TABLE [dbo].[aircomp2] ADD  DEFAULT (NULL) FOR [current_w]
GO
ALTER TABLE [dbo].[aircomp2] ADD  CONSTRAINT [DF__aircomp2__record__540C7B00]  DEFAULT (CONVERT([char](24),getdate(),(120))) FOR [recordingTime]
GO
ALTER TABLE [dbo].[aircomp3] ADD  DEFAULT (NULL) FOR [status]
GO
ALTER TABLE [dbo].[aircomp3] ADD  DEFAULT (NULL) FOR [slight]
GO
ALTER TABLE [dbo].[aircomp3] ADD  DEFAULT (NULL) FOR [severity]
GO
ALTER TABLE [dbo].[aircomp3] ADD  DEFAULT (NULL) FOR [pressure]
GO
ALTER TABLE [dbo].[aircomp3] ADD  DEFAULT (NULL) FOR [temperature]
GO
ALTER TABLE [dbo].[aircomp3] ADD  DEFAULT (NULL) FOR [current_u]
GO
ALTER TABLE [dbo].[aircomp3] ADD  DEFAULT (NULL) FOR [current_v]
GO
ALTER TABLE [dbo].[aircomp3] ADD  DEFAULT (NULL) FOR [current_w]
GO
ALTER TABLE [dbo].[aircomp3] ADD  CONSTRAINT [DF__aircomp3__record__5E8A0973]  DEFAULT (CONVERT([char](24),getdate(),(120))) FOR [recordingTime]
GO
ALTER TABLE [dbo].[dryer1] ADD  DEFAULT (NULL) FOR [pressure]
GO
ALTER TABLE [dbo].[dryer1] ADD  DEFAULT (NULL) FOR [temperature]
GO
ALTER TABLE [dbo].[dryer1] ADD  DEFAULT (NULL) FOR [humidity]
GO
ALTER TABLE [dbo].[dryer1] ADD  CONSTRAINT [DF__dryer1__recordin__05A3D694]  DEFAULT (CONVERT([char](24),getdate(),(120))) FOR [recordingTime]
GO
ALTER TABLE [dbo].[dryer1] ADD  DEFAULT (NULL) FOR [statu]
GO
ALTER TABLE [dbo].[dryer2] ADD  DEFAULT (NULL) FOR [pressure]
GO
ALTER TABLE [dbo].[dryer2] ADD  DEFAULT (NULL) FOR [temperature]
GO
ALTER TABLE [dbo].[dryer2] ADD  DEFAULT (NULL) FOR [humidity]
GO
ALTER TABLE [dbo].[dryer2] ADD  CONSTRAINT [DF__dryer2__recordin__0C50D423]  DEFAULT (CONVERT([char](24),getdate(),(120))) FOR [recordingTime]
GO
ALTER TABLE [dbo].[dryer2] ADD  DEFAULT (NULL) FOR [statu]
GO
ALTER TABLE [dbo].[headerpipe3inch] ADD  DEFAULT (NULL) FOR [pressure]
GO
ALTER TABLE [dbo].[headerpipe3inch] ADD  DEFAULT (NULL) FOR [temperature]
GO
ALTER TABLE [dbo].[headerpipe3inch] ADD  DEFAULT (NULL) FOR [humidity]
GO
ALTER TABLE [dbo].[headerpipe3inch] ADD  CONSTRAINT [DF__headerpip__recor__12FDD1B2]  DEFAULT (CONVERT([char](24),getdate(),(120))) FOR [recordingTime]
GO
ALTER TABLE [dbo].[headerpipe3inch] ADD  DEFAULT (NULL) FOR [statu]
GO
ALTER TABLE [dbo].[headerpipe4inch] ADD  DEFAULT (NULL) FOR [pressure]
GO
ALTER TABLE [dbo].[headerpipe4inch] ADD  DEFAULT (NULL) FOR [temperature]
GO
ALTER TABLE [dbo].[headerpipe4inch] ADD  DEFAULT (NULL) FOR [humidity]
GO
ALTER TABLE [dbo].[headerpipe4inch] ADD  CONSTRAINT [DF__headerpip__recor__19AACF41]  DEFAULT (CONVERT([char](24),getdate(),(120))) FOR [recordingTime]
GO
ALTER TABLE [dbo].[headerpipe4inch] ADD  DEFAULT (NULL) FOR [statu]
GO
ALTER TABLE [dbo].[room] ADD  DEFAULT (NULL) FOR [temperature]
GO
ALTER TABLE [dbo].[room] ADD  DEFAULT (NULL) FOR [humidity]
GO
ALTER TABLE [dbo].[room] ADD  CONSTRAINT [DF__room__recordingT__6AEFE058]  DEFAULT (CONVERT([char](24),getdate(),(120))) FOR [recordingTime]
GO
ALTER TABLE [dbo].[room] ADD  DEFAULT ('0') FOR [statu]
GO
ALTER TABLE [dbo].[tbhistory] ADD  DEFAULT ((0)) FOR [IsAlarmData]
GO
EXEC [kongyadb].sys.sp_addextendedproperty @name=N'temperature', @value=N'排气温度' 
GO
EXEC [kongyadb].sys.sp_addextendedproperty @name=N'status', @value=N'运行信息' 
GO
EXEC [kongyadb].sys.sp_addextendedproperty @name=N'slight', @value=N'轻度故障' 
GO
EXEC [kongyadb].sys.sp_addextendedproperty @name=N'severity', @value=N'重度故障' 
GO
EXEC [kongyadb].sys.sp_addextendedproperty @name=N'recordingTime', @value=N'记录时间' 
GO
EXEC [kongyadb].sys.sp_addextendedproperty @name=N'pressure', @value=N'排气压力' 
GO
EXEC [kongyadb].sys.sp_addextendedproperty @name=N'current_w', @value=N'W相电流' 
GO
EXEC [kongyadb].sys.sp_addextendedproperty @name=N'current_v', @value=N'V相电流' 
GO
EXEC [kongyadb].sys.sp_addextendedproperty @name=N'current_u ', @value=N'U相电流' 
GO
USE [master]
GO
ALTER DATABASE [kongyadb] SET  READ_WRITE 
GO
