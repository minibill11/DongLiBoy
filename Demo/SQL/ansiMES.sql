USE [master]
GO
/****** Object:  Database [JianHeMES]    Script Date: 2019/3/29 16:42:22 ******/
CREATE DATABASE [JianHeMES]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'JianHeMES_Data0', FILENAME = N'D:\Program Files\Microsoft SQL Server\MSSQL13.MSSQLSERVER\MSSQL\DATA\JianHeMES_Data.mdf' , SIZE = 46784KB , MAXSIZE = UNLIMITED, FILEGROWTH = 10240KB )
 LOG ON 
( NAME = N'JianHeMES_Log0', FILENAME = N'D:\Program Files\Microsoft SQL Server\MSSQL13.MSSQLSERVER\MSSQL\DATA\JianHeMES_Log.ldf' , SIZE = 1027072KB , MAXSIZE = 2048GB , FILEGROWTH = 10240KB )
GO
ALTER DATABASE [JianHeMES] SET COMPATIBILITY_LEVEL = 130
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [JianHeMES].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [JianHeMES] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [JianHeMES] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [JianHeMES] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [JianHeMES] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [JianHeMES] SET ARITHABORT OFF 
GO
ALTER DATABASE [JianHeMES] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [JianHeMES] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [JianHeMES] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [JianHeMES] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [JianHeMES] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [JianHeMES] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [JianHeMES] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [JianHeMES] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [JianHeMES] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [JianHeMES] SET  DISABLE_BROKER 
GO
ALTER DATABASE [JianHeMES] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [JianHeMES] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [JianHeMES] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [JianHeMES] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [JianHeMES] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [JianHeMES] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [JianHeMES] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [JianHeMES] SET RECOVERY FULL 
GO
ALTER DATABASE [JianHeMES] SET  MULTI_USER 
GO
ALTER DATABASE [JianHeMES] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [JianHeMES] SET DB_CHAINING OFF 
GO
ALTER DATABASE [JianHeMES] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [JianHeMES] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [JianHeMES] SET DELAYED_DURABILITY = DISABLED 
GO
EXEC sys.sp_db_vardecimal_storage_format N'JianHeMES', N'ON'
GO
ALTER DATABASE [JianHeMES] SET QUERY_STORE = OFF
GO
USE [JianHeMES]
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
USE [JianHeMES]
GO
/****** Object:  User [zzb_zhenbiao]    Script Date: 2019/3/29 16:42:22 ******/
CREATE USER [zzb_zhenbiao] FOR LOGIN [zzb_zhenbiao] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [zzb]    Script Date: 2019/3/29 16:42:22 ******/
CREATE USER [zzb] FOR LOGIN [zzb] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [weboperator]    Script Date: 2019/3/29 16:42:22 ******/
CREATE USER [weboperator] FOR LOGIN [weboperator] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  User [inkjetroom]    Script Date: 2019/3/29 16:42:22 ******/
CREATE USER [inkjetroom] WITHOUT LOGIN WITH DEFAULT_SCHEMA=[dbo]
GO
ALTER ROLE [db_owner] ADD MEMBER [zzb_zhenbiao]
GO
ALTER ROLE [db_owner] ADD MEMBER [zzb]
GO
ALTER ROLE [db_owner] ADD MEMBER [weboperator]
GO
ALTER ROLE [db_owner] ADD MEMBER [inkjetroom]
GO
ALTER ROLE [db_datawriter] ADD MEMBER [inkjetroom]
GO
/****** Object:  Table [dbo].[__MigrationHistory]    Script Date: 2019/3/29 16:42:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__MigrationHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ContextKey] [nvarchar](300) NOT NULL,
	[Model] [varbinary](max) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK_dbo.__MigrationHistory2] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC,
	[ContextKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AdapterCard_Power_Collection]    Script Date: 2019/3/29 16:42:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AdapterCard_Power_Collection](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrderNum] [nvarchar](max) NULL,
	[BoxBarCode] [nvarchar](max) NULL,
	[BarCodesNum] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.AdapterCard_Power_Collection] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Appearance_OQCCheckAbnormal]    Script Date: 2019/3/29 16:42:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Appearance_OQCCheckAbnormal](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [int] NOT NULL,
	[DetialedDescription] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.Appearance_OQCCheckAbnormal] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Appearances]    Script Date: 2019/3/29 16:42:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Appearances](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrderNum] [nvarchar](max) NULL,
	[ToOrderNum] [nvarchar](max) NULL,
	[BarCodesNum] [nvarchar](max) NULL,
	[ModuleGroupNum] [nvarchar](max) NULL,
	[OQCCheckBT] [datetime] NULL,
	[OQCPrincipal] [nvarchar](max) NULL,
	[OQCCheckFT] [datetime] NULL,
	[OQCCheckDate] [int] NOT NULL,
	[OQCCheckTime] [time](7) NULL,
	[OQCCheckTimeSpan] [nvarchar](max) NULL,
	[Appearance_OQCCheckAbnormal] [nvarchar](max) NULL,
	[RepairCondition] [nvarchar](max) NULL,
	[OQCCheckFinish] [bit] NOT NULL,
	[Remark] [nvarchar](max) NULL,
	[CustomerBarCodesNum] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.Appearances] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetRoles]    Script Date: 2019/3/29 16:42:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetRoles](
	[Id] [nvarchar](128) NOT NULL,
	[Name] [nvarchar](256) NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserClaims]    Script Date: 2019/3/29 16:42:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserClaims](
	[Id] [int] NOT NULL,
	[UserId] [nvarchar](128) NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserLogins]    Script Date: 2019/3/29 16:42:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserLogins](
	[LoginProvider] [nvarchar](128) NOT NULL,
	[ProviderKey] [nvarchar](128) NOT NULL,
	[UserId] [nvarchar](128) NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUserRoles]    Script Date: 2019/3/29 16:42:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUserRoles](
	[UserId] [nvarchar](128) NOT NULL,
	[RoleId] [nvarchar](128) NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AspNetUsers]    Script Date: 2019/3/29 16:42:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AspNetUsers](
	[Id] [nvarchar](128) NOT NULL,
	[Email] [nvarchar](256) NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](max) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[LockoutEndDateUtc] [datetime] NULL,
	[LockoutEnabled] [bit] NOT NULL,
	[AccessFailedCount] [int] NOT NULL,
	[UserName] [nvarchar](256) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AssembleLineIds]    Script Date: 2019/3/29 16:42:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AssembleLineIds](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[LineID] [nvarchar](max) NULL,
	[AssembleLineName] [nvarchar](max) NULL,
	[AssembleLineDiscription] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.AssembleLineIds] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Assembles]    Script Date: 2019/3/29 16:42:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Assembles](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrderNum] [nvarchar](max) NULL,
	[BarCode_Prefix] [nvarchar](max) NULL,
	[BoxBarCode] [nvarchar](max) NULL,
	[AssembleBT] [datetime] NULL,
	[AssemblePrincipal] [nvarchar](max) NULL,
	[AssembleFT] [datetime] NULL,
	[ModelList] [nvarchar](max) NULL,
	[AssembleTime] [time](7) NULL,
	[AssembleFinish] [bit] NULL,
	[WaterproofTestBT] [datetime] NULL,
	[WaterproofTestPrincipal] [nvarchar](max) NULL,
	[WaterproofTestFT] [datetime] NULL,
	[WaterproofAbnormal] [int] NULL,
	[WaterproofMaintaince] [nvarchar](max) NULL,
	[WaterproofTestTimeSpan] [time](7) NULL,
	[WaterproofTestFinish] [bit] NULL,
	[AssembleAdapterCardBT] [datetime] NULL,
	[AssembleAdapterCardPrincipal] [nvarchar](max) NULL,
	[AssembleAdapterCardFT] [datetime] NULL,
	[AssembleAdapterTime] [time](7) NULL,
	[AssembleAdapterFinish] [bit] NULL,
	[ViewCheckBT] [datetime] NULL,
	[AssembleViewCheckPrincipal] [nvarchar](max) NULL,
	[ViewCheckFT] [datetime] NULL,
	[ViewCheckTime] [time](7) NULL,
	[ViewCheckAbnormal] [int] NULL,
	[ViewCheckFinish] [bit] NULL,
	[ElectricityCheckBT] [datetime] NULL,
	[AssembleElectricityCheckPrincipal] [nvarchar](max) NULL,
	[ElectricityCheckFT] [datetime] NULL,
	[ElectricityCheckTime] [time](7) NULL,
	[ElectricityCheckAbnormal] [int] NULL,
	[ElectricityCheckFinish] [bit] NULL,
	[AssembleLineId] [int] NULL,
	[Burn_in_Id] [int] NULL,
	[Appearance_Id] [int] NULL,
	[Packaging_Id] [int] NULL,
	[AdapterCard_Power_List] [nvarchar](max) NULL,
	[PQCCheckBT] [datetime] NULL,
	[AssemblePQCPrincipal] [nvarchar](max) NULL,
	[PQCCheckFT] [datetime] NULL,
	[PQCCheckTime] [time](7) NULL,
	[PQCCheckAbnormal] [nvarchar](max) NULL,
	[PQCRepairCondition] [nvarchar](max) NULL,
	[PQCCheckFinish] [bit] NOT NULL,
	[PQCCheckDate] [int] NOT NULL,
	[Remark] [nvarchar](max) NULL,
	[RepetitionPQCCheck] [bit] NOT NULL,
	[RepetitionPQCCheckCause] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[BarCodes]    Script Date: 2019/3/29 16:42:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[BarCodes](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[OrderNum] [nvarchar](max) NOT NULL,
	[ToOrderNum] [nvarchar](max) NULL,
	[BarCode_Prefix] [nvarchar](max) NOT NULL,
	[BarCodesNum] [nvarchar](max) NOT NULL,
	[ModuleGroupNum] [nvarchar](max) NULL,
	[BarCodeType] [nvarchar](max) NOT NULL,
	[CreateDate] [datetime] NULL,
	[Creator] [nvarchar](max) NULL,
	[Burn_in_Id] [int] NULL,
	[Appearance_Id] [int] NULL,
	[Packaging_Id] [int] NULL,
	[IsRepertory] [bit] NOT NULL,
	[Remark] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Burn_in]    Script Date: 2019/3/29 16:42:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Burn_in](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrderNum] [nvarchar](max) NULL,
	[BarCodesNum] [nvarchar](max) NULL,
	[OQCCheckBT] [datetime] NULL,
	[OQCPrincipal] [nvarchar](max) NULL,
	[OQCCheckFT] [datetime] NULL,
	[OQCCheckDate] [int] NULL,
	[OQCCheckTime] [time](7) NULL,
	[OQCCheckTimeSpan] [nvarchar](max) NULL,
	[Burn_in_OQCCheckAbnormal] [nvarchar](max) NULL,
	[RepairCondition] [nvarchar](max) NULL,
	[OQCCheckFinish] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.Burn_in] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Burn_in_OQCCheckAbnormal]    Script Date: 2019/3/29 16:42:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Burn_in_OQCCheckAbnormal](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [int] NOT NULL,
	[DetialedDescription] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.Burn_in_OQCCheckAbnormal] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CalibrationRecords]    Script Date: 2019/3/29 16:42:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CalibrationRecords](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[OrderNum] [nvarchar](max) NOT NULL,
	[BarCodesNum] [nvarchar](max) NULL,
	[ModuleGroupNum] [nvarchar](max) NULL,
	[BeginCalibration] [datetime] NULL,
	[FinishCalibration] [datetime] NULL,
	[Normal] [bit] NOT NULL,
	[AbnormalDescription] [nvarchar](max) NULL,
	[CalibrationDate] [int] NOT NULL,
	[CalibrationTime] [time](7) NULL,
	[CalibrationTimeSpan] [nvarchar](max) NULL,
	[Operator] [nvarchar](max) NULL,
	[RepetitionCalibration] [bit] NOT NULL,
	[RepetitionCalibrationCause] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FinalQCs]    Script Date: 2019/3/29 16:42:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FinalQCs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrderNum] [nvarchar](max) NULL,
	[BarCodesNum] [nvarchar](max) NULL,
	[FQCCheckBT] [datetime] NULL,
	[FQCPrincipal] [nvarchar](max) NULL,
	[FQCCheckFT] [datetime] NULL,
	[FQCCheckDate] [int] NOT NULL,
	[FQCCheckTime] [time](7) NULL,
	[FQCCheckTimeSpan] [nvarchar](max) NULL,
	[FinalQC_FQCCheckAbnormal] [nvarchar](max) NULL,
	[FQCCheckFinish] [bit] NOT NULL,
	[Remark] [nvarchar](max) NULL,
	[RepetitionFQCCheck] [bit] NOT NULL,
	[RepetitionFQCCheckCause] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.FinalQCs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[IQCReports]    Script Date: 2019/3/29 16:42:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[IQCReports](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Material_SN] [nvarchar](max) NOT NULL,
	[RoHS_REACH] [bit] NOT NULL,
	[OrderNumber] [nvarchar](max) NULL,
	[EquipmentNum] [nvarchar](max) NOT NULL,
	[Provider] [nvarchar](max) NOT NULL,
	[MaterialName] [nvarchar](max) NOT NULL,
	[ModelNumber] [nvarchar](max) NOT NULL,
	[Specification] [nvarchar](max) NOT NULL,
	[MaterialQuantity] [nvarchar](max) NOT NULL,
	[IncomingDate] [datetime] NOT NULL,
	[ApplyPurchaseOrderNum] [nvarchar](max) NOT NULL,
	[BatchNum] [nvarchar](max) NOT NULL,
	[InspectionDate] [datetime] NOT NULL,
	[SamplingPlan] [nvarchar](max) NOT NULL,
	[MaterialVersion] [nvarchar](max) NULL,
	[C1] [nvarchar](max) NULL,
	[C2] [nvarchar](max) NULL,
	[C3] [nvarchar](max) NULL,
	[C4] [nvarchar](max) NULL,
	[C5] [nvarchar](max) NULL,
	[C6] [nvarchar](max) NULL,
	[C7] [nvarchar](max) NULL,
	[C8] [nvarchar](max) NULL,
	[C9] [nvarchar](max) NULL,
	[D1] [nvarchar](max) NULL,
	[D2] [nvarchar](max) NULL,
	[D3] [nvarchar](max) NULL,
	[D4] [nvarchar](max) NULL,
	[D5] [nvarchar](max) NULL,
	[D6] [nvarchar](max) NULL,
	[D7] [nvarchar](max) NULL,
	[D8] [nvarchar](max) NULL,
	[D9] [nvarchar](max) NULL,
	[E1] [nvarchar](max) NULL,
	[E2] [nvarchar](max) NULL,
	[E3] [nvarchar](max) NULL,
	[E4] [nvarchar](max) NULL,
	[E5] [nvarchar](max) NULL,
	[E6] [nvarchar](max) NULL,
	[E7] [nvarchar](max) NULL,
	[E8] [nvarchar](max) NULL,
	[E9] [nvarchar](max) NULL,
	[F1] [nvarchar](max) NULL,
	[F2] [nvarchar](max) NULL,
	[F3] [nvarchar](max) NULL,
	[F4] [nvarchar](max) NULL,
	[F5] [nvarchar](max) NULL,
	[F6] [nvarchar](max) NULL,
	[F7] [nvarchar](max) NULL,
	[F8] [nvarchar](max) NULL,
	[F9] [nvarchar](max) NULL,
	[S0] [nvarchar](max) NULL,
	[S1] [nvarchar](max) NULL,
	[S11] [nvarchar](max) NULL,
	[S12] [nvarchar](max) NULL,
	[S13] [nvarchar](max) NULL,
	[S14] [nvarchar](max) NULL,
	[S15] [nvarchar](max) NULL,
	[S2] [nvarchar](max) NULL,
	[S21] [nvarchar](max) NULL,
	[S22] [nvarchar](max) NULL,
	[S23] [nvarchar](max) NULL,
	[S24] [nvarchar](max) NULL,
	[S25] [nvarchar](max) NULL,
	[S3] [nvarchar](max) NULL,
	[S31] [nvarchar](max) NULL,
	[S32] [nvarchar](max) NULL,
	[S33] [nvarchar](max) NULL,
	[S34] [nvarchar](max) NULL,
	[S35] [nvarchar](max) NULL,
	[SR] [nvarchar](max) NULL,
	[SRJson] [nvarchar](max) NULL,
	[R0] [nvarchar](max) NULL,
	[R1] [nvarchar](max) NULL,
	[R11] [nvarchar](max) NULL,
	[R12] [nvarchar](max) NULL,
	[R13] [nvarchar](max) NULL,
	[R2] [nvarchar](max) NULL,
	[R21] [nvarchar](max) NULL,
	[R22] [nvarchar](max) NULL,
	[R23] [nvarchar](max) NULL,
	[R3] [nvarchar](max) NULL,
	[R31] [nvarchar](max) NULL,
	[R32] [nvarchar](max) NULL,
	[R33] [nvarchar](max) NULL,
	[P0] [nvarchar](max) NULL,
	[P1] [nvarchar](max) NULL,
	[P11] [nvarchar](max) NULL,
	[P12] [nvarchar](max) NULL,
	[P13] [nvarchar](max) NULL,
	[P2] [nvarchar](max) NULL,
	[P21] [nvarchar](max) NULL,
	[P22] [nvarchar](max) NULL,
	[P23] [nvarchar](max) NULL,
	[P3] [nvarchar](max) NULL,
	[P31] [nvarchar](max) NULL,
	[P32] [nvarchar](max) NULL,
	[P33] [nvarchar](max) NULL,
	[AM] [nvarchar](max) NULL,
	[AM0] [nvarchar](max) NULL,
	[AM1] [nvarchar](max) NULL,
	[AM11] [nvarchar](max) NULL,
	[AM12] [nvarchar](max) NULL,
	[AM13] [nvarchar](max) NULL,
	[AM2] [nvarchar](max) NULL,
	[AM21] [nvarchar](max) NULL,
	[AM22] [nvarchar](max) NULL,
	[AM23] [nvarchar](max) NULL,
	[AM3] [nvarchar](max) NULL,
	[AM31] [nvarchar](max) NULL,
	[AM32] [nvarchar](max) NULL,
	[AM33] [nvarchar](max) NULL,
	[AM4] [nvarchar](max) NULL,
	[AM41] [nvarchar](max) NULL,
	[AM42] [nvarchar](max) NULL,
	[AM43] [nvarchar](max) NULL,
	[NG1] [nvarchar](max) NULL,
	[NG2] [nvarchar](max) NULL,
	[NG3] [nvarchar](max) NULL,
	[NGD] [bit] NOT NULL,
	[NGHandle] [nvarchar](max) NULL,
	[ReportRemark] [nvarchar](max) NULL,
	[Inspector] [nvarchar](max) NULL,
	[Creator] [nvarchar](max) NULL,
	[CreatedDate] [datetime] NULL,
	[Assessor] [nvarchar](max) NULL,
	[AssessedDate] [datetime] NULL,
	[AssessorRemark] [nvarchar](max) NULL,
	[AssessorPass] [bit] NULL,
	[Approve] [nvarchar](max) NULL,
	[ApprovedDate] [datetime] NULL,
	[ApproveRemark] [nvarchar](max) NULL,
	[ApprovePass] [bit] NULL,
	[SG] [nvarchar](max) NULL,
	[SGJson] [nvarchar](max) NULL,
	[SB] [nvarchar](max) NULL,
	[SBJson] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.IQCReports] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ModelCollections]    Script Date: 2019/3/29 16:42:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ModelCollections](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[StationId] [nvarchar](max) NULL,
	[OrderNum] [nvarchar](max) NULL,
	[BoxBarCode] [nvarchar](max) NULL,
	[BarCodesNum] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.ModelCollections] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderInformations]    Script Date: 2019/3/29 16:42:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderInformations](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[OrderNum] [nvarchar](max) NULL,
	[ModuleGroupQuantity] [int] NOT NULL,
	[PlaceAnOrderDate] [datetime] NULL,
	[DateOfDelivery] [datetime] NULL,
	[CreateDate] [datetime] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderMgm_Delete]    Script Date: 2019/3/29 16:42:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderMgm_Delete](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrderNum] [nvarchar](max) NULL,
	[DeleteDate] [datetime] NULL,
	[Deleter] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.OrderMgm_Delete] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderMgms]    Script Date: 2019/3/29 16:42:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderMgms](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[OrderNum] [nvarchar](max) NOT NULL,
	[BarCode_Prefix] [nvarchar](max) NOT NULL,
	[CustomerName] [nvarchar](max) NOT NULL,
	[ContractDate] [datetime] NOT NULL,
	[DeliveryDate] [datetime] NOT NULL,
	[PlanInputTime] [datetime] NOT NULL,
	[PlanCompleteTime] [datetime] NOT NULL,
	[Area] [nvarchar](max) NOT NULL,
	[Boxes] [int] NOT NULL,
	[Models] [int] NOT NULL,
	[ModelsMore] [int] NOT NULL,
	[Powers] [int] NOT NULL,
	[PowersMore] [int] NOT NULL,
	[AdapterCard] [int] NOT NULL,
	[AdapterCardMore] [int] NOT NULL,
	[BarCodeCreated] [int] NULL,
	[BarCodeCreateDate] [datetime] NULL,
	[BarCodeCreator] [nvarchar](max) NULL,
	[CompletedRate] [real] NULL,
	[OrderCreateDate] [datetime] NULL,
	[Burn_in_Id] [int] NULL,
	[ModelType] [nvarchar](max) NULL,
	[BoxType] [nvarchar](max) NULL,
	[PowerType] [nvarchar](max) NULL,
	[AdapterCardType] [nvarchar](max) NULL,
	[Appearance_Id] [int] NULL,
	[Packaging_Id] [int] NULL,
	[PlatformType] [nvarchar](max) NULL,
	[IsRepertory] [bit] NOT NULL,
	[Remark] [nvarchar](max) NULL,
	[IsAOD] [bit] NOT NULL,
	[AOD_PC_Description] [nvarchar](max) NULL,
	[AOD_TNG_Description] [nvarchar](max) NULL,
	[AOD_ASSEMB_Description] [nvarchar](max) NULL,
	[AOD_QA_Description] [nvarchar](max) NULL,
	[AODConverter] [nvarchar](max) NULL,
	[AODConvertDate] [datetime] NULL,
	[AOD_Description] [nvarchar](max) NULL,
	[ModulePieceBarCodeCreated] [int] NULL,
	[ModulePieceBarCodeCreateDate] [datetime] NULL,
	[ModulePieceBarCodeCreator] [nvarchar](max) NULL,
	[PowerBarCodeCreated] [int] NULL,
	[PowerBarCodeCreateDate] [datetime] NULL,
	[PowerBarCodeCreator] [nvarchar](max) NULL,
	[AdapterCardBarCodeCreated] [int] NULL,
	[AdapterCardBarCodeCreateDate] [datetime] NULL,
	[AdapterCardBarCodeCreator] [nvarchar](max) NULL,
	[ProcessingRequire] [nvarchar](max) NULL,
	[StandardRequire] [nvarchar](max) NULL,
	[Capacity] [int] NOT NULL,
	[HandSampleScedule] [nvarchar](max) NULL,
	[CapacityQ] [decimal](18, 2) NOT NULL,
	[IsAbnormalOrder] [bit] NOT NULL,
	[AbnormalOrderConverter] [nvarchar](max) NULL,
	[AbnormalOrderConvertDate] [datetime] NULL,
	[AbnormalOrder_Description] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Packaging_OQCCheckAbnormal]    Script Date: 2019/3/29 16:42:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Packaging_OQCCheckAbnormal](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [int] NOT NULL,
	[DetialedDescription] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.Packaging_OQCCheckAbnormal] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Packagings]    Script Date: 2019/3/29 16:42:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Packagings](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrderNum] [nvarchar](max) NULL,
	[BarCodesNum] [nvarchar](max) NULL,
	[OQCCheckBT] [datetime] NULL,
	[OQCPrincipal] [nvarchar](max) NULL,
	[OQCCheckFT] [datetime] NULL,
	[OQCCheckTime] [time](7) NULL,
	[OQCCheckTimeSpan] [nvarchar](max) NULL,
	[Packaging_OQCCheckAbnormal] [int] NOT NULL,
	[RepairCondition] [nvarchar](max) NULL,
	[OQCCheckFinish] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.Packagings] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Personnel_daily]    Script Date: 2019/3/29 16:42:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Personnel_daily](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Department] [nvarchar](max) NULL,
	[Principal] [nvarchar](max) NULL,
	[Aurhorized_personnel] [int] NOT NULL,
	[Need_personnel] [int] NOT NULL,
	[Employees_personnel] [int] NOT NULL,
	[Today_on_board_employees] [int] NOT NULL,
	[Today_on_board_workers] [int] NOT NULL,
	[Interview] [int] NOT NULL,
	[Todoy_dimission_employees] [int] NOT NULL,
	[Todoy_dimission_workers] [int] NOT NULL,
	[Resigned_that_month] [int] NOT NULL,
	[Date] [datetime] NULL,
	[Workers_personnel] [int] NOT NULL,
	[Reporter] [nvarchar](max) NULL,
	[Resigned_workers_that_month] [int] NOT NULL,
	[Todoy_dimission_employees_over7days] [int] NOT NULL,
	[Todoy_dimission_employees_nvever_over7days] [int] NOT NULL,
 CONSTRAINT [PK_dbo.Personnel_daily] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Personnel_of_Contrast]    Script Date: 2019/3/29 16:42:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Personnel_of_Contrast](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Department] [nvarchar](max) NULL,
	[Number] [int] NOT NULL,
	[Zhang_WorkingHour] [decimal](18, 2) NOT NULL,
	[Zhang_Pay] [decimal](18, 2) NOT NULL,
	[Total_Staff] [decimal](18, 2) NOT NULL,
	[Overtime_Total] [decimal](18, 2) NOT NULL,
	[Pay_Total] [decimal](18, 2) NOT NULL,
	[Daily_Pay] [decimal](18, 2) NOT NULL,
	[Date] [datetime] NULL,
	[CreateDate] [datetime] NULL,
	[Creator] [nvarchar](max) NULL,
	[Week_number] [int] NOT NULL,
	[Monday] [datetime] NULL,
	[Sunday] [datetime] NULL,
	[Year] [int] NOT NULL,
	[Month] [int] NOT NULL,
	[Week] [int] NOT NULL,
 CONSTRAINT [PK_dbo.Personnel_of_Contrast] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Personnel_Recruitment]    Script Date: 2019/3/29 16:42:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Personnel_Recruitment](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Department_weekly] [nvarchar](max) NULL,
	[Demand_jobs] [nvarchar](max) NULL,
	[Compile] [int] NOT NULL,
	[Demand_number] [int] NOT NULL,
	[Employed] [int] NOT NULL,
	[Unfinished_nember] [int] NOT NULL,
	[Application_date] [datetime] NULL,
	[Work_date] [datetime] NULL,
	[Invite_Plan_Cycle] [int] NOT NULL,
	[Invite_Actaul_Cycle] [int] NOT NULL,
	[Date] [datetime] NULL,
	[CreateDate] [datetime] NULL,
	[Creator] [nvarchar](max) NULL,
	[Week_number] [int] NOT NULL,
	[Monday] [datetime] NULL,
	[Sunday] [datetime] NULL,
	[Year] [int] NOT NULL,
	[Month] [int] NOT NULL,
	[Week] [int] NOT NULL,
 CONSTRAINT [PK_dbo.Personnel_Recruitment] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Personnel_Turnoverrate]    Script Date: 2019/3/29 16:42:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Personnel_Turnoverrate](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Department] [nvarchar](max) NULL,
	[Month_Beginning] [int] NOT NULL,
	[Month_End] [int] NOT NULL,
	[Average_Number] [int] NOT NULL,
	[Departure] [int] NOT NULL,
	[Turnover_Rate] [decimal](18, 2) NOT NULL,
	[Average_Value] [decimal](18, 2) NOT NULL,
	[Date] [datetime] NULL,
	[CreateDate] [datetime] NULL,
	[Creator] [nvarchar](max) NULL,
	[Year] [int] NOT NULL,
	[Month] [int] NOT NULL,
 CONSTRAINT [PK_dbo.Personnel_Turnoverrate] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PQCCheckabnormals]    Script Date: 2019/3/29 16:42:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PQCCheckabnormals](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [int] NOT NULL,
	[DetialedDescription] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SMT_OrderInfo]    Script Date: 2019/3/29 16:42:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SMT_OrderInfo](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrderNum] [nvarchar](50) NULL,
	[Quantity] [int] NOT NULL,
	[PlatformType] [nvarchar](max) NULL,
	[Customer] [nvarchar](max) NULL,
	[DeliveryDate] [datetime] NULL,
	[Status] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.SMT_OrderInfo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SMT_ProductionBoardTable]    Script Date: 2019/3/29 16:42:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SMT_ProductionBoardTable](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrderNum] [nvarchar](50) NULL,
	[JobContent] [nvarchar](50) NULL,
	[LineNum] [int] NOT NULL,
 CONSTRAINT [PK_dbo.SMT_ProductionBoardTable] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SMT_ProductionData]    Script Date: 2019/3/29 16:42:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SMT_ProductionData](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[LineNum] [int] NOT NULL,
	[OrderNum] [nvarchar](max) NULL,
	[NormalCount] [int] NOT NULL,
	[AbnormalCount] [int] NOT NULL,
	[ProductionDate] [datetime] NULL,
	[BeginTime] [datetime] NULL,
	[EndTime] [datetime] NULL,
	[BarcodeNum] [nvarchar](max) NULL,
	[Result] [bit] NOT NULL,
	[Operator] [nvarchar](max) NULL,
	[JobContent] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.SMT_ProductionData] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SMT_ProductionDataSMT_OrderInfo]    Script Date: 2019/3/29 16:42:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SMT_ProductionDataSMT_OrderInfo](
	[SMT_ProductionData_Id] [int] NOT NULL,
	[SMT_OrderInfo_Id] [int] NOT NULL,
 CONSTRAINT [PK_dbo.SMT_ProductionDataSMT_OrderInfo] PRIMARY KEY CLUSTERED 
(
	[SMT_ProductionData_Id] ASC,
	[SMT_OrderInfo_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SMT_ProductionDataSMT_ProductionLineInfo]    Script Date: 2019/3/29 16:42:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SMT_ProductionDataSMT_ProductionLineInfo](
	[SMT_ProductionData_Id] [int] NOT NULL,
	[SMT_ProductionLineInfo_Id] [int] NOT NULL,
 CONSTRAINT [PK_dbo.SMT_ProductionDataSMT_ProductionLineInfo] PRIMARY KEY CLUSTERED 
(
	[SMT_ProductionData_Id] ASC,
	[SMT_ProductionLineInfo_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SMT_ProductionLineInfo]    Script Date: 2019/3/29 16:42:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SMT_ProductionLineInfo](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[LineNum] [int] NOT NULL,
	[ProducingOrderNum] [nvarchar](max) NULL,
	[CreateDate] [datetime] NULL,
	[Team] [nvarchar](max) NULL,
	[GroupLeader] [nvarchar](max) NULL,
	[Status] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.SMT_ProductionLineInfo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SMT_ProductionLineInfoSMT_OrderInfo]    Script Date: 2019/3/29 16:42:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SMT_ProductionLineInfoSMT_OrderInfo](
	[SMT_ProductionLineInfo_Id] [int] NOT NULL,
	[SMT_OrderInfo_Id] [int] NOT NULL,
 CONSTRAINT [PK_dbo.SMT_ProductionLineInfoSMT_OrderInfo] PRIMARY KEY CLUSTERED 
(
	[SMT_ProductionLineInfo_Id] ASC,
	[SMT_OrderInfo_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SMT_ProductionPlan]    Script Date: 2019/3/29 16:42:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SMT_ProductionPlan](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[LineNum] [int] NOT NULL,
	[OrderNum] [nvarchar](max) NULL,
	[CreateDate] [datetime] NULL,
	[Quantity] [int] NOT NULL,
	[Capacity] [decimal](18, 2) NOT NULL,
	[JobContent] [nvarchar](max) NULL,
	[Remark] [nvarchar](max) NULL,
	[PlanProductionDate] [datetime] NULL,
	[Creator] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.SMT_ProductionPlan] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SMT_ProductionPlanSMT_OrderInfo]    Script Date: 2019/3/29 16:42:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SMT_ProductionPlanSMT_OrderInfo](
	[SMT_ProductionPlan_Id] [int] NOT NULL,
	[SMT_OrderInfo_Id] [int] NOT NULL,
 CONSTRAINT [PK_dbo.SMT_ProductionPlanSMT_OrderInfo] PRIMARY KEY CLUSTERED 
(
	[SMT_ProductionPlan_Id] ASC,
	[SMT_OrderInfo_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SMT_ProductionPlanSMT_ProductionData]    Script Date: 2019/3/29 16:42:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SMT_ProductionPlanSMT_ProductionData](
	[SMT_ProductionPlan_Id] [int] NOT NULL,
	[SMT_ProductionData_Id] [int] NOT NULL,
 CONSTRAINT [PK_dbo.SMT_ProductionPlanSMT_ProductionData] PRIMARY KEY CLUSTERED 
(
	[SMT_ProductionPlan_Id] ASC,
	[SMT_ProductionData_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SMT_ProductionPlanSMT_ProductionLineInfo]    Script Date: 2019/3/29 16:42:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SMT_ProductionPlanSMT_ProductionLineInfo](
	[SMT_ProductionPlan_Id] [int] NOT NULL,
	[SMT_ProductionLineInfo_Id] [int] NOT NULL,
 CONSTRAINT [PK_dbo.SMT_ProductionPlanSMT_ProductionLineInfo] PRIMARY KEY CLUSTERED 
(
	[SMT_ProductionPlan_Id] ASC,
	[SMT_ProductionLineInfo_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[sysdiagrams]    Script Date: 2019/3/29 16:42:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[sysdiagrams](
	[name] [nvarchar](128) NOT NULL,
	[principal_id] [int] NOT NULL,
	[diagram_id] [int] NOT NULL,
	[version] [int] NULL,
	[definition] [varbinary](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Tests]    Script Date: 2019/3/29 16:42:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tests](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[OrderNum] [nvarchar](50) NULL,
	[JobContent] [nvarchar](50) NULL,
	[PlatformType] [nvarchar](50) NULL,
	[LineNum] [int] NOT NULL,
 CONSTRAINT [PK_dbo.Tests] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TestTable]    Script Date: 2019/3/29 16:42:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TestTable](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BarCodesNum] [nvarchar](50) NULL,
	[Name] [nvarchar](50) NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Useroles]    Script Date: 2019/3/29 16:42:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Useroles](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [nvarchar](50) NULL,
	[UserID] [int] NOT NULL,
	[Department] [nvarchar](50) NULL,
	[RolesName] [nvarchar](50) NULL,
	[Roles] [nvarchar](50) NULL,
 CONSTRAINT [PK_dbo.Useroles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserRolelistTables]    Script Date: 2019/3/29 16:42:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserRolelistTables](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Department] [nvarchar](50) NULL,
	[RolesCode] [int] NOT NULL,
	[RolesName] [nvarchar](50) NULL,
	[Discription] [nvarchar](50) NULL,
 CONSTRAINT [PK_dbo.UserRolelistTables] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 2019/3/29 16:42:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [nvarchar](max) NULL,
	[Password] [nvarchar](max) NULL,
	[CreateDate] [datetime] NOT NULL,
	[Creator] [nvarchar](max) NULL,
	[UserAuthorize] [int] NOT NULL,
	[Deleter] [nvarchar](max) NULL,
	[DeleteDate] [datetime] NULL,
	[Description] [nvarchar](max) NULL,
	[UserNum] [int] NOT NULL,
	[Role] [nvarchar](max) NULL,
	[Department] [nvarchar](max) NULL,
	[Burn_in_Id] [int] NULL,
	[Appearance_Id] [int] NULL,
	[Packaging_Id] [int] NULL,
	[LineNum] [int] NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ViewCheckabnormals]    Script Date: 2019/3/29 16:42:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ViewCheckabnormals](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [int] NOT NULL,
	[DetialedDescription] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Waterproofabnormals]    Script Date: 2019/3/29 16:42:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Waterproofabnormals](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [int] NOT NULL,
	[DetialedDescription] [nvarchar](max) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ZS400]    Script Date: 2019/3/29 16:42:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ZS400](
	[编号] [nchar](50) NULL,
	[轨道] [nchar](50) NULL,
	[BARCODE] [nchar](50) NULL,
	[开始时间] [nchar](50) NULL,
	[结束时间] [nchar](50) NULL,
	[完成状态] [nchar](50) NULL,
	[喷墨程序号] [nchar](50) NULL,
	[脉冲时间] [nchar](50) NULL,
	[循环总数] [nchar](50) NULL,
	[关闭电压] [nchar](50) NULL,
	[撞针行程] [nchar](50) NULL,
	[打开时间] [nchar](50) NULL,
	[关闭时间] [nchar](50) NULL,
	[喷嘴型号] [nchar](50) NULL,
	[备注] [nchar](100) NULL
) ON [PRIMARY]
GO
/****** Object:  Index [IX_Appearance_Id]    Script Date: 2019/3/29 16:42:26 ******/
CREATE NONCLUSTERED INDEX [IX_Appearance_Id] ON [dbo].[Assembles]
(
	[Appearance_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Burn_in_Id]    Script Date: 2019/3/29 16:42:26 ******/
CREATE NONCLUSTERED INDEX [IX_Burn_in_Id] ON [dbo].[Assembles]
(
	[Burn_in_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Packaging_Id]    Script Date: 2019/3/29 16:42:26 ******/
CREATE NONCLUSTERED INDEX [IX_Packaging_Id] ON [dbo].[Assembles]
(
	[Packaging_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Appearance_Id]    Script Date: 2019/3/29 16:42:26 ******/
CREATE NONCLUSTERED INDEX [IX_Appearance_Id] ON [dbo].[BarCodes]
(
	[Appearance_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Burn_in_Id]    Script Date: 2019/3/29 16:42:26 ******/
CREATE NONCLUSTERED INDEX [IX_Burn_in_Id] ON [dbo].[BarCodes]
(
	[Burn_in_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Packaging_Id]    Script Date: 2019/3/29 16:42:26 ******/
CREATE NONCLUSTERED INDEX [IX_Packaging_Id] ON [dbo].[BarCodes]
(
	[Packaging_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Appearance_Id]    Script Date: 2019/3/29 16:42:26 ******/
CREATE NONCLUSTERED INDEX [IX_Appearance_Id] ON [dbo].[OrderMgms]
(
	[Appearance_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Burn_in_Id]    Script Date: 2019/3/29 16:42:26 ******/
CREATE NONCLUSTERED INDEX [IX_Burn_in_Id] ON [dbo].[OrderMgms]
(
	[Burn_in_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Packaging_Id]    Script Date: 2019/3/29 16:42:26 ******/
CREATE NONCLUSTERED INDEX [IX_Packaging_Id] ON [dbo].[OrderMgms]
(
	[Packaging_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_SMT_OrderInfo_Id]    Script Date: 2019/3/29 16:42:26 ******/
CREATE NONCLUSTERED INDEX [IX_SMT_OrderInfo_Id] ON [dbo].[SMT_ProductionDataSMT_OrderInfo]
(
	[SMT_OrderInfo_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_SMT_ProductionData_Id]    Script Date: 2019/3/29 16:42:26 ******/
CREATE NONCLUSTERED INDEX [IX_SMT_ProductionData_Id] ON [dbo].[SMT_ProductionDataSMT_OrderInfo]
(
	[SMT_ProductionData_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_SMT_ProductionData_Id]    Script Date: 2019/3/29 16:42:26 ******/
CREATE NONCLUSTERED INDEX [IX_SMT_ProductionData_Id] ON [dbo].[SMT_ProductionDataSMT_ProductionLineInfo]
(
	[SMT_ProductionData_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_SMT_ProductionLineInfo_Id]    Script Date: 2019/3/29 16:42:26 ******/
CREATE NONCLUSTERED INDEX [IX_SMT_ProductionLineInfo_Id] ON [dbo].[SMT_ProductionDataSMT_ProductionLineInfo]
(
	[SMT_ProductionLineInfo_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_SMT_OrderInfo_Id]    Script Date: 2019/3/29 16:42:26 ******/
CREATE NONCLUSTERED INDEX [IX_SMT_OrderInfo_Id] ON [dbo].[SMT_ProductionLineInfoSMT_OrderInfo]
(
	[SMT_OrderInfo_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_SMT_ProductionLineInfo_Id]    Script Date: 2019/3/29 16:42:26 ******/
CREATE NONCLUSTERED INDEX [IX_SMT_ProductionLineInfo_Id] ON [dbo].[SMT_ProductionLineInfoSMT_OrderInfo]
(
	[SMT_ProductionLineInfo_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_SMT_OrderInfo_Id]    Script Date: 2019/3/29 16:42:26 ******/
CREATE NONCLUSTERED INDEX [IX_SMT_OrderInfo_Id] ON [dbo].[SMT_ProductionPlanSMT_OrderInfo]
(
	[SMT_OrderInfo_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_SMT_ProductionPlan_Id]    Script Date: 2019/3/29 16:42:26 ******/
CREATE NONCLUSTERED INDEX [IX_SMT_ProductionPlan_Id] ON [dbo].[SMT_ProductionPlanSMT_OrderInfo]
(
	[SMT_ProductionPlan_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_SMT_ProductionData_Id]    Script Date: 2019/3/29 16:42:26 ******/
CREATE NONCLUSTERED INDEX [IX_SMT_ProductionData_Id] ON [dbo].[SMT_ProductionPlanSMT_ProductionData]
(
	[SMT_ProductionData_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_SMT_ProductionPlan_Id]    Script Date: 2019/3/29 16:42:26 ******/
CREATE NONCLUSTERED INDEX [IX_SMT_ProductionPlan_Id] ON [dbo].[SMT_ProductionPlanSMT_ProductionData]
(
	[SMT_ProductionPlan_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_SMT_ProductionLineInfo_Id]    Script Date: 2019/3/29 16:42:26 ******/
CREATE NONCLUSTERED INDEX [IX_SMT_ProductionLineInfo_Id] ON [dbo].[SMT_ProductionPlanSMT_ProductionLineInfo]
(
	[SMT_ProductionLineInfo_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_SMT_ProductionPlan_Id]    Script Date: 2019/3/29 16:42:26 ******/
CREATE NONCLUSTERED INDEX [IX_SMT_ProductionPlan_Id] ON [dbo].[SMT_ProductionPlanSMT_ProductionLineInfo]
(
	[SMT_ProductionPlan_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Appearance_Id]    Script Date: 2019/3/29 16:42:26 ******/
CREATE NONCLUSTERED INDEX [IX_Appearance_Id] ON [dbo].[Users]
(
	[Appearance_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Burn_in_Id]    Script Date: 2019/3/29 16:42:26 ******/
CREATE NONCLUSTERED INDEX [IX_Burn_in_Id] ON [dbo].[Users]
(
	[Burn_in_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Packaging_Id]    Script Date: 2019/3/29 16:42:26 ******/
CREATE NONCLUSTERED INDEX [IX_Packaging_Id] ON [dbo].[Users]
(
	[Packaging_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Appearances] ADD  CONSTRAINT [DF__Appearanc__OQCCh__0504B816]  DEFAULT ((0)) FOR [OQCCheckDate]
GO
ALTER TABLE [dbo].[Assembles] ADD  CONSTRAINT [DF__Assembles__Assem__778AC167]  DEFAULT ((0)) FOR [AssembleLineId]
GO
ALTER TABLE [dbo].[Assembles] ADD  CONSTRAINT [DF__Assembles__PQCCh__3C34F16F]  DEFAULT ((0)) FOR [PQCCheckFinish]
GO
ALTER TABLE [dbo].[Assembles] ADD  CONSTRAINT [DF__Assembles__PQCCh__6D2D2E85]  DEFAULT ((0)) FOR [PQCCheckDate]
GO
ALTER TABLE [dbo].[Assembles] ADD  DEFAULT ((0)) FOR [RepetitionPQCCheck]
GO
ALTER TABLE [dbo].[BarCodes] ADD  CONSTRAINT [DF__BarCodes__IsRepe__6F4A8121]  DEFAULT ((0)) FOR [IsRepertory]
GO
ALTER TABLE [dbo].[Burn_in] ADD  CONSTRAINT [DF__Burn_in__OQCChec__6E2152BE]  DEFAULT ((0)) FOR [OQCCheckDate]
GO
ALTER TABLE [dbo].[Burn_in] ADD  CONSTRAINT [DF__Burn_in__OQCChec__7E02B4CC]  DEFAULT ((0)) FOR [OQCCheckFinish]
GO
ALTER TABLE [dbo].[CalibrationRecords] ADD  CONSTRAINT [DF__Calibrati__Calib__3D491139]  DEFAULT ((0)) FOR [CalibrationDate]
GO
ALTER TABLE [dbo].[CalibrationRecords] ADD  CONSTRAINT [DF__Calibrati__Repet__0BE6BFCF]  DEFAULT ((0)) FOR [RepetitionCalibration]
GO
ALTER TABLE [dbo].[FinalQCs] ADD  DEFAULT ((0)) FOR [RepetitionFQCCheck]
GO
ALTER TABLE [dbo].[OrderMgms] ADD  CONSTRAINT [DF__OrderMgms__PlanI__00DF2177]  DEFAULT ('1900-01-01T00:00:00.000') FOR [PlanInputTime]
GO
ALTER TABLE [dbo].[OrderMgms] ADD  CONSTRAINT [DF__OrderMgms__PlanC__01D345B0]  DEFAULT ('1900-01-01T00:00:00.000') FOR [PlanCompleteTime]
GO
ALTER TABLE [dbo].[OrderMgms] ADD  CONSTRAINT [DF__OrderMgms__IsRep__7D98A078]  DEFAULT ((0)) FOR [IsRepertory]
GO
ALTER TABLE [dbo].[OrderMgms] ADD  CONSTRAINT [DF__OrderMgms__IsAOD__041093DD]  DEFAULT ((0)) FOR [IsAOD]
GO
ALTER TABLE [dbo].[OrderMgms] ADD  DEFAULT ('') FOR [ProcessingRequire]
GO
ALTER TABLE [dbo].[OrderMgms] ADD  DEFAULT ('') FOR [StandardRequire]
GO
ALTER TABLE [dbo].[OrderMgms] ADD  DEFAULT ((0)) FOR [Capacity]
GO
ALTER TABLE [dbo].[OrderMgms] ADD  DEFAULT ((0)) FOR [CapacityQ]
GO
ALTER TABLE [dbo].[OrderMgms] ADD  DEFAULT ((0)) FOR [IsAbnormalOrder]
GO
ALTER TABLE [dbo].[Personnel_daily] ADD  CONSTRAINT [DF__Personnel__Need___1CDC41A7]  DEFAULT ((0)) FOR [Need_personnel]
GO
ALTER TABLE [dbo].[Personnel_daily] ADD  DEFAULT ((0)) FOR [Workers_personnel]
GO
ALTER TABLE [dbo].[Personnel_daily] ADD  DEFAULT ((0)) FOR [Resigned_workers_that_month]
GO
ALTER TABLE [dbo].[Personnel_daily] ADD  DEFAULT ((0)) FOR [Todoy_dimission_employees_over7days]
GO
ALTER TABLE [dbo].[Personnel_daily] ADD  DEFAULT ((0)) FOR [Todoy_dimission_employees_nvever_over7days]
GO
ALTER TABLE [dbo].[Personnel_of_Contrast] ADD  DEFAULT ((0)) FOR [Year]
GO
ALTER TABLE [dbo].[Personnel_of_Contrast] ADD  DEFAULT ((0)) FOR [Month]
GO
ALTER TABLE [dbo].[Personnel_of_Contrast] ADD  DEFAULT ((0)) FOR [Week]
GO
ALTER TABLE [dbo].[Personnel_Recruitment] ADD  DEFAULT ((0)) FOR [Year]
GO
ALTER TABLE [dbo].[Personnel_Recruitment] ADD  DEFAULT ((0)) FOR [Month]
GO
ALTER TABLE [dbo].[Personnel_Recruitment] ADD  DEFAULT ((0)) FOR [Week]
GO
ALTER TABLE [dbo].[Personnel_Turnoverrate] ADD  DEFAULT ((0)) FOR [Year]
GO
ALTER TABLE [dbo].[Personnel_Turnoverrate] ADD  DEFAULT ((0)) FOR [Month]
GO
ALTER TABLE [dbo].[SMT_ProductionData] ADD  DEFAULT ((0)) FOR [Result]
GO
ALTER TABLE [dbo].[SMT_ProductionPlan] ADD  DEFAULT ((0)) FOR [Quantity]
GO
ALTER TABLE [dbo].[SMT_ProductionPlan] ADD  DEFAULT ((0)) FOR [Capacity]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT ((0)) FOR [LineNum]
GO
ALTER TABLE [dbo].[BarCodes]  WITH CHECK ADD  CONSTRAINT [FK_dbo.BarCodes_dbo.Appearances_Appearance_Id] FOREIGN KEY([Appearance_Id])
REFERENCES [dbo].[Appearances] ([Id])
GO
ALTER TABLE [dbo].[BarCodes] CHECK CONSTRAINT [FK_dbo.BarCodes_dbo.Appearances_Appearance_Id]
GO
ALTER TABLE [dbo].[BarCodes]  WITH CHECK ADD  CONSTRAINT [FK_dbo.BarCodes_dbo.Burn_in_Burn_in_Id] FOREIGN KEY([Burn_in_Id])
REFERENCES [dbo].[Burn_in] ([Id])
GO
ALTER TABLE [dbo].[BarCodes] CHECK CONSTRAINT [FK_dbo.BarCodes_dbo.Burn_in_Burn_in_Id]
GO
ALTER TABLE [dbo].[BarCodes]  WITH CHECK ADD  CONSTRAINT [FK_dbo.BarCodes_dbo.Packagings_Packaging_Id] FOREIGN KEY([Packaging_Id])
REFERENCES [dbo].[Packagings] ([Id])
GO
ALTER TABLE [dbo].[BarCodes] CHECK CONSTRAINT [FK_dbo.BarCodes_dbo.Packagings_Packaging_Id]
GO
ALTER TABLE [dbo].[OrderMgms]  WITH CHECK ADD  CONSTRAINT [FK_dbo.OrderMgms_dbo.Appearances_Appearance_Id] FOREIGN KEY([Appearance_Id])
REFERENCES [dbo].[Appearances] ([Id])
GO
ALTER TABLE [dbo].[OrderMgms] CHECK CONSTRAINT [FK_dbo.OrderMgms_dbo.Appearances_Appearance_Id]
GO
ALTER TABLE [dbo].[OrderMgms]  WITH CHECK ADD  CONSTRAINT [FK_dbo.OrderMgms_dbo.Burn_in_Burn_in_Id] FOREIGN KEY([Burn_in_Id])
REFERENCES [dbo].[Burn_in] ([Id])
GO
ALTER TABLE [dbo].[OrderMgms] CHECK CONSTRAINT [FK_dbo.OrderMgms_dbo.Burn_in_Burn_in_Id]
GO
ALTER TABLE [dbo].[OrderMgms]  WITH CHECK ADD  CONSTRAINT [FK_dbo.OrderMgms_dbo.Packagings_Packaging_Id] FOREIGN KEY([Packaging_Id])
REFERENCES [dbo].[Packagings] ([Id])
GO
ALTER TABLE [dbo].[OrderMgms] CHECK CONSTRAINT [FK_dbo.OrderMgms_dbo.Packagings_Packaging_Id]
GO
ALTER TABLE [dbo].[SMT_ProductionDataSMT_OrderInfo]  WITH CHECK ADD  CONSTRAINT [FK_dbo.SMT_ProductionDataSMT_OrderInfo_dbo.SMT_OrderInfo_SMT_OrderInfo_Id] FOREIGN KEY([SMT_OrderInfo_Id])
REFERENCES [dbo].[SMT_OrderInfo] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[SMT_ProductionDataSMT_OrderInfo] CHECK CONSTRAINT [FK_dbo.SMT_ProductionDataSMT_OrderInfo_dbo.SMT_OrderInfo_SMT_OrderInfo_Id]
GO
ALTER TABLE [dbo].[SMT_ProductionDataSMT_OrderInfo]  WITH CHECK ADD  CONSTRAINT [FK_dbo.SMT_ProductionDataSMT_OrderInfo_dbo.SMT_ProductionData_SMT_ProductionData_Id] FOREIGN KEY([SMT_ProductionData_Id])
REFERENCES [dbo].[SMT_ProductionData] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[SMT_ProductionDataSMT_OrderInfo] CHECK CONSTRAINT [FK_dbo.SMT_ProductionDataSMT_OrderInfo_dbo.SMT_ProductionData_SMT_ProductionData_Id]
GO
ALTER TABLE [dbo].[SMT_ProductionDataSMT_ProductionLineInfo]  WITH CHECK ADD  CONSTRAINT [FK_dbo.SMT_ProductionDataSMT_ProcutionLineInfo_dbo.SMT_ProcutionLineInfo_SMT_ProcutionLineInfo_Id] FOREIGN KEY([SMT_ProductionLineInfo_Id])
REFERENCES [dbo].[SMT_ProductionLineInfo] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[SMT_ProductionDataSMT_ProductionLineInfo] CHECK CONSTRAINT [FK_dbo.SMT_ProductionDataSMT_ProcutionLineInfo_dbo.SMT_ProcutionLineInfo_SMT_ProcutionLineInfo_Id]
GO
ALTER TABLE [dbo].[SMT_ProductionDataSMT_ProductionLineInfo]  WITH CHECK ADD  CONSTRAINT [FK_dbo.SMT_ProductionDataSMT_ProcutionLineInfo_dbo.SMT_ProductionData_SMT_ProductionData_Id] FOREIGN KEY([SMT_ProductionData_Id])
REFERENCES [dbo].[SMT_ProductionData] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[SMT_ProductionDataSMT_ProductionLineInfo] CHECK CONSTRAINT [FK_dbo.SMT_ProductionDataSMT_ProcutionLineInfo_dbo.SMT_ProductionData_SMT_ProductionData_Id]
GO
ALTER TABLE [dbo].[SMT_ProductionLineInfoSMT_OrderInfo]  WITH CHECK ADD  CONSTRAINT [FK_dbo.SMT_ProcutionLineInfoSMT_OrderInfo_dbo.SMT_OrderInfo_SMT_OrderInfo_Id] FOREIGN KEY([SMT_OrderInfo_Id])
REFERENCES [dbo].[SMT_OrderInfo] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[SMT_ProductionLineInfoSMT_OrderInfo] CHECK CONSTRAINT [FK_dbo.SMT_ProcutionLineInfoSMT_OrderInfo_dbo.SMT_OrderInfo_SMT_OrderInfo_Id]
GO
ALTER TABLE [dbo].[SMT_ProductionLineInfoSMT_OrderInfo]  WITH CHECK ADD  CONSTRAINT [FK_dbo.SMT_ProcutionLineInfoSMT_OrderInfo_dbo.SMT_ProcutionLineInfo_SMT_ProcutionLineInfo_Id] FOREIGN KEY([SMT_ProductionLineInfo_Id])
REFERENCES [dbo].[SMT_ProductionLineInfo] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[SMT_ProductionLineInfoSMT_OrderInfo] CHECK CONSTRAINT [FK_dbo.SMT_ProcutionLineInfoSMT_OrderInfo_dbo.SMT_ProcutionLineInfo_SMT_ProcutionLineInfo_Id]
GO
ALTER TABLE [dbo].[SMT_ProductionPlanSMT_OrderInfo]  WITH CHECK ADD  CONSTRAINT [FK_dbo.SMT_ProductionPlanSMT_OrderInfo_dbo.SMT_OrderInfo_SMT_OrderInfo_Id] FOREIGN KEY([SMT_OrderInfo_Id])
REFERENCES [dbo].[SMT_OrderInfo] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[SMT_ProductionPlanSMT_OrderInfo] CHECK CONSTRAINT [FK_dbo.SMT_ProductionPlanSMT_OrderInfo_dbo.SMT_OrderInfo_SMT_OrderInfo_Id]
GO
ALTER TABLE [dbo].[SMT_ProductionPlanSMT_OrderInfo]  WITH CHECK ADD  CONSTRAINT [FK_dbo.SMT_ProductionPlanSMT_OrderInfo_dbo.SMT_ProductionPlan_SMT_ProductionPlan_Id] FOREIGN KEY([SMT_ProductionPlan_Id])
REFERENCES [dbo].[SMT_ProductionPlan] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[SMT_ProductionPlanSMT_OrderInfo] CHECK CONSTRAINT [FK_dbo.SMT_ProductionPlanSMT_OrderInfo_dbo.SMT_ProductionPlan_SMT_ProductionPlan_Id]
GO
ALTER TABLE [dbo].[SMT_ProductionPlanSMT_ProductionData]  WITH CHECK ADD  CONSTRAINT [FK_dbo.SMT_ProductionPlanSMT_ProductionData_dbo.SMT_ProductionData_SMT_ProductionData_Id] FOREIGN KEY([SMT_ProductionData_Id])
REFERENCES [dbo].[SMT_ProductionData] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[SMT_ProductionPlanSMT_ProductionData] CHECK CONSTRAINT [FK_dbo.SMT_ProductionPlanSMT_ProductionData_dbo.SMT_ProductionData_SMT_ProductionData_Id]
GO
ALTER TABLE [dbo].[SMT_ProductionPlanSMT_ProductionData]  WITH CHECK ADD  CONSTRAINT [FK_dbo.SMT_ProductionPlanSMT_ProductionData_dbo.SMT_ProductionPlan_SMT_ProductionPlan_Id] FOREIGN KEY([SMT_ProductionPlan_Id])
REFERENCES [dbo].[SMT_ProductionPlan] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[SMT_ProductionPlanSMT_ProductionData] CHECK CONSTRAINT [FK_dbo.SMT_ProductionPlanSMT_ProductionData_dbo.SMT_ProductionPlan_SMT_ProductionPlan_Id]
GO
ALTER TABLE [dbo].[SMT_ProductionPlanSMT_ProductionLineInfo]  WITH CHECK ADD  CONSTRAINT [FK_dbo.SMT_ProductionPlanSMT_ProcutionLineInfo_dbo.SMT_ProcutionLineInfo_SMT_ProcutionLineInfo_Id] FOREIGN KEY([SMT_ProductionLineInfo_Id])
REFERENCES [dbo].[SMT_ProductionLineInfo] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[SMT_ProductionPlanSMT_ProductionLineInfo] CHECK CONSTRAINT [FK_dbo.SMT_ProductionPlanSMT_ProcutionLineInfo_dbo.SMT_ProcutionLineInfo_SMT_ProcutionLineInfo_Id]
GO
ALTER TABLE [dbo].[SMT_ProductionPlanSMT_ProductionLineInfo]  WITH CHECK ADD  CONSTRAINT [FK_dbo.SMT_ProductionPlanSMT_ProcutionLineInfo_dbo.SMT_ProductionPlan_SMT_ProductionPlan_Id] FOREIGN KEY([SMT_ProductionPlan_Id])
REFERENCES [dbo].[SMT_ProductionPlan] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[SMT_ProductionPlanSMT_ProductionLineInfo] CHECK CONSTRAINT [FK_dbo.SMT_ProductionPlanSMT_ProcutionLineInfo_dbo.SMT_ProductionPlan_SMT_ProductionPlan_Id]
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Users_dbo.Appearances_Appearance_Id] FOREIGN KEY([Appearance_Id])
REFERENCES [dbo].[Appearances] ([Id])
GO
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK_dbo.Users_dbo.Appearances_Appearance_Id]
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Users_dbo.Burn_in_Burn_in_Id] FOREIGN KEY([Burn_in_Id])
REFERENCES [dbo].[Burn_in] ([Id])
GO
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK_dbo.Users_dbo.Burn_in_Burn_in_Id]
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Users_dbo.Packagings_Packaging_Id] FOREIGN KEY([Packaging_Id])
REFERENCES [dbo].[Packagings] ([Id])
GO
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK_dbo.Users_dbo.Packagings_Packaging_Id]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'订单' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'OrderMgms'
GO
USE [master]
GO
ALTER DATABASE [JianHeMES] SET  READ_WRITE 
GO
