CREATE FUNCTION [dbo].[nop_splitstring_to_table]
(
    @string NVARCHAR(MAX),
    @delimiter CHAR(1)
)
RETURNS @output TABLE(
    data NVARCHAR(MAX)
)
BEGIN
    DECLARE @start INT, @end INT
    SELECT @start = 1, @end = CHARINDEX(@delimiter, @string)

    WHILE @start < LEN(@string) + 1 BEGIN
        IF @end = 0 
            SET @end = LEN(@string) + 1

        INSERT INTO @output (data) 
        VALUES(SUBSTRING(@string, @start, @end - @start))
        SET @start = @end + 1
        SET @end = CHARINDEX(@delimiter, @string, @start)
    END
    RETURN
END
GO



CREATE FUNCTION [dbo].[nop_getnotnullnotempty]
(
    @p1 nvarchar(max) = null, 
    @p2 nvarchar(max) = null
)
RETURNS nvarchar(max)
AS
BEGIN
    IF @p1 IS NULL
        return @p2
    IF @p1 =''
        return @p2

    return @p1
END
GO



CREATE FUNCTION [dbo].[nop_getprimarykey_indexname]
(
    @table_name nvarchar(1000) = null
)
RETURNS nvarchar(1000)
AS
BEGIN
	DECLARE @index_name nvarchar(1000)

    SELECT @index_name = i.name
	FROM sys.tables AS tbl
	INNER JOIN sys.indexes AS i ON (i.index_id > 0 and i.is_hypothetical = 0) AND (i.object_id=tbl.object_id)
	WHERE (i.is_unique=1 and i.is_disabled=0) and (tbl.name=@table_name)

    RETURN @index_name
END
GO

CREATE PROCEDURE [FullText_IsSupported]
AS
BEGIN	
	EXEC('
	SELECT CASE SERVERPROPERTY(''IsFullTextInstalled'')
	WHEN 1 THEN 
		CASE DatabaseProperty (DB_NAME(DB_ID()), ''IsFulltextEnabled'')
		WHEN 1 THEN 1
		ELSE 0
		END
	ELSE 0
	END')
END
GO



CREATE PROCEDURE [FullText_Enable]
AS
BEGIN
	--create catalog
	EXEC('
	IF NOT EXISTS (SELECT 1 FROM sys.fulltext_catalogs WHERE [name] = ''nopCommerceFullTextCatalog'')
		CREATE FULLTEXT CATALOG [nopCommerceFullTextCatalog] AS DEFAULT')
	
	--create indexes
	DECLARE @create_index_text nvarchar(4000)
	SET @create_index_text = '
	IF NOT EXISTS (SELECT 1 FROM sys.fulltext_indexes WHERE object_id = object_id(''[Product]''))
		CREATE FULLTEXT INDEX ON [Product]([Name], [ShortDescription], [FullDescription], [Sku])
		KEY INDEX [' + dbo.[nop_getprimarykey_indexname] ('Product') +  '] ON [nopCommerceFullTextCatalog] WITH CHANGE_TRACKING AUTO'
	EXEC(@create_index_text)
	

	SET @create_index_text = '
	IF NOT EXISTS (SELECT 1 FROM sys.fulltext_indexes WHERE object_id = object_id(''[LocalizedProperty]''))
		CREATE FULLTEXT INDEX ON [LocalizedProperty]([LocaleValue])
		KEY INDEX [' + dbo.[nop_getprimarykey_indexname] ('LocalizedProperty') +  '] ON [nopCommerceFullTextCatalog] WITH CHANGE_TRACKING AUTO'
	EXEC(@create_index_text)

	SET @create_index_text = '
	IF NOT EXISTS (SELECT 1 FROM sys.fulltext_indexes WHERE object_id = object_id(''[ProductTag]''))
		CREATE FULLTEXT INDEX ON [ProductTag]([Name])
		KEY INDEX [' + dbo.[nop_getprimarykey_indexname] ('ProductTag') +  '] ON [nopCommerceFullTextCatalog] WITH CHANGE_TRACKING AUTO'
	EXEC(@create_index_text)
END
GO

CREATE PROCEDURE [FullText_Disable]
AS
BEGIN
	EXEC('
	--drop indexes
	IF EXISTS (SELECT 1 FROM sys.fulltext_indexes WHERE object_id = object_id(''[Product]''))
		DROP FULLTEXT INDEX ON [Product]
	')

	EXEC('
	IF EXISTS (SELECT 1 FROM sys.fulltext_indexes WHERE object_id = object_id(''[LocalizedProperty]''))
		DROP FULLTEXT INDEX ON [LocalizedProperty]
	')

	EXEC('
	IF EXISTS (SELECT 1 FROM sys.fulltext_indexes WHERE object_id = object_id(''[ProductTag]''))
		DROP FULLTEXT INDEX ON [ProductTag]
	')

	--drop catalog
	EXEC('
	IF EXISTS (SELECT 1 FROM sys.fulltext_catalogs WHERE [name] = ''nopCommerceFullTextCatalog'')
		DROP FULLTEXT CATALOG [nopCommerceFullTextCatalog]
	')
END
GO



CREATE PROCEDURE [LanguagePackImport]
(
	@LanguageId int,
	@XmlPackage xml
)
AS
BEGIN
	IF EXISTS(SELECT * FROM [Language] WHERE [Id] = @LanguageId)
	BEGIN
		CREATE TABLE #LocaleStringResourceTmp
			(
				[LanguageId] [int] NOT NULL,
				[ResourceName] [nvarchar](200) NOT NULL,
				[ResourceValue] [nvarchar](MAX) NOT NULL
			)

		INSERT INTO #LocaleStringResourceTmp (LanguageID, ResourceName, ResourceValue)
		SELECT	@LanguageId, nref.value('@Name', 'nvarchar(200)') ,nref.value('Value[1]', 'nvarchar(MAX)')
		FROM	@XmlPackage.nodes('//Language/LocaleResource') AS R(nref)

		DECLARE @ResourceName nvarchar(200)
		DECLARE @ResourceValue nvarchar(MAX)
		DECLARE cur_localeresource CURSOR FOR
		SELECT LanguageID, ResourceName, ResourceValue
		FROM #LocaleStringResourceTmp
		OPEN cur_localeresource
		FETCH NEXT FROM cur_localeresource INTO @LanguageId, @ResourceName, @ResourceValue
		WHILE @@FETCH_STATUS = 0
		BEGIN
			IF (EXISTS (SELECT 1 FROM [LocaleStringResource] WHERE LanguageID=@LanguageId AND ResourceName=@ResourceName))
			BEGIN
				UPDATE [LocaleStringResource]
				SET [ResourceValue]=@ResourceValue
				WHERE LanguageID=@LanguageId AND ResourceName=@ResourceName
			END
			ELSE 
			BEGIN
				INSERT INTO [LocaleStringResource]
				(
					[LanguageId],
					[ResourceName],
					[ResourceValue]
				)
				VALUES
				(
					@LanguageId,
					@ResourceName,
					@ResourceValue
				)
			END
			
			
			FETCH NEXT FROM cur_localeresource INTO @LanguageId, @ResourceName, @ResourceValue
			END
		CLOSE cur_localeresource
		DEALLOCATE cur_localeresource

		DROP TABLE #LocaleStringResourceTmp
	END
END
GO


--new stored procedure
CREATE PROCEDURE [dbo].[DeleteGuests]
(
	@CreatedFromUtc datetime,
	@CreatedToUtc datetime,
	@TotalRecordsDeleted int = null OUTPUT
)
AS
BEGIN
	CREATE TABLE #tmp_guests (CustomerId int)
		
	INSERT #tmp_guests (CustomerId)
	SELECT [Id] FROM [Customer] c with (NOLOCK)
	WHERE
	--created from
	((@CreatedFromUtc is null) OR (c.[CreatedOnUtc] > @CreatedFromUtc))
	AND
	--created to
	((@CreatedToUtc is null) OR (c.[CreatedOnUtc] < @CreatedToUtc))
	AND

	--guests only
	(EXISTS(SELECT 1 FROM [Customer_CustomerRole_Mapping] ccrm with (NOLOCK) inner join [Customer] with (NOLOCK) on ccrm.[Customer_Id]=c.[Id] inner join [CustomerRole] cr with (NOLOCK) on cr.[Id]=ccrm.[CustomerRole_Id] WHERE cr.[SystemName] = N'Guests'))
	AND

	--no system accounts
	(c.IsSystemAccount = 0)
	
	--delete guests
	DELETE [Customer]
	WHERE [Id] IN (SELECT [CustomerID] FROM #tmp_guests)
	
	--delete attributes
	DELETE [GenericAttribute]
	WHERE ([EntityID] IN (SELECT [CustomerID] FROM #tmp_guests))
	AND
	([KeyGroup] = N'Customer')
	
	--total records
	SELECT @TotalRecordsDeleted = COUNT(1) FROM #tmp_guests
	
	DROP TABLE #tmp_guests
END
GO

CREATE PROCEDURE SearchAllTables
(
@SearchStr nvarchar(100)
)
AS
BEGIN

-- Copyright © 2002 Narayana Vyas Kondreddi. All rights reserved.
-- Purpose: To search all columns of all tables for a given search string
-- Written by: Narayana Vyas Kondreddi
-- Site: http://vyaskn.tripod.com
-- Tested on: SQL Server 7.0 and SQL Server 2000
-- Date modified: 28th July 2002 22:50 GMT


CREATE TABLE #Results (ColumnName nvarchar(370), ColumnValue nvarchar(3630))

SET NOCOUNT ON

DECLARE @TableName nvarchar(256), @ColumnName nvarchar(128), @SearchStr2 nvarchar(110)
SET  @TableName = ''
SET @SearchStr2 = QUOTENAME('%' + @SearchStr + '%','''')

WHILE @TableName IS NOT NULL
BEGIN
SET @ColumnName = ''
SET @TableName = 
(
SELECT MIN(QUOTENAME(TABLE_SCHEMA) + '.' + QUOTENAME(TABLE_NAME))
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE'
AND QUOTENAME(TABLE_SCHEMA) + '.' + QUOTENAME(TABLE_NAME) > @TableName
AND OBJECTPROPERTY(
OBJECT_ID(
QUOTENAME(TABLE_SCHEMA) + '.' + QUOTENAME(TABLE_NAME)
), 'IsMSShipped'
       ) = 0
)

WHILE (@TableName IS NOT NULL) AND (@ColumnName IS NOT NULL)
BEGIN
SET @ColumnName =
(
SELECT MIN(QUOTENAME(COLUMN_NAME))
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = PARSENAME(@TableName, 2)
AND TABLE_NAME = PARSENAME(@TableName, 1)
AND DATA_TYPE IN ('char', 'varchar', 'nchar', 'nvarchar')
AND QUOTENAME(COLUMN_NAME) > @ColumnName
)

IF @ColumnName IS NOT NULL
BEGIN
INSERT INTO #Results
EXEC
(
'SELECT ''' + @TableName + '.' + @ColumnName + ''', LEFT(' + @ColumnName + ', 3630) 
FROM ' + @TableName + ' (NOLOCK) ' +
' WHERE ' + @ColumnName + ' LIKE ' + @SearchStr2
)
END
END 
END

SELECT ColumnName, ColumnValue FROM #Results
END
GO

CREATE PROCEDURE [dbo].[SummaryReportContributionPayment]
(
	@ContributionId int,
	@NameReport nvarchar(255),
	@ReportState int,
	@Source nvarchar(250),
	@TotalRecords int = null OUTPUT
)
AS
BEGIN

SELECT convert(int,ROW_NUMBER()  OVER(ORDER BY YEARS DESC))  AS Id, YEARS AS 'Year', StateId, IsAutomatic, 
SUM(ENE) AS 'Ene',SUM(FEB) AS 'Feb',SUM(MAR) AS 'Mar',
SUM(ABR) AS 'Abr',SUM(MAY) AS 'May',SUM(JUN) AS 'Jun',
SUM(JUL) AS 'Jul',SUM(AGO) AS 'Ago',SUM(SEP) AS 'Sep',
SUM(OCT) AS 'Oct',SUM(NOV) AS 'Nov',SUM(DIC) AS 'Dic' 
INTO  #tmp_reports
FROM 
(
	SELECT CD.IsAutomatic, CD.StateId, YEAR(CD.ScheduledDateOnUtc ) AS 'YEARS', 
	'ENE' = CASE WHEN MONTH(CD.ScheduledDateOnUtc )=1 AND CD.StateId<>1 THEN SUM(CD.AmountPayed) ELSE 0 END ,
	'FEB' = CASE WHEN MONTH(CD.ScheduledDateOnUtc )=2 AND CD.StateId<>1 THEN SUM(CD.AmountPayed) ELSE 0 END ,
	'MAR' = CASE WHEN MONTH(CD.ScheduledDateOnUtc )=3 AND CD.StateId<>1 THEN SUM(CD.AmountPayed) ELSE 0 END ,
	'ABR' = CASE WHEN MONTH(CD.ScheduledDateOnUtc )=4 AND CD.StateId<>1 THEN SUM(CD.AmountPayed) ELSE 0 END ,
	'MAY' = CASE WHEN MONTH(CD.ScheduledDateOnUtc )=5 AND CD.StateId<>1 THEN SUM(CD.AmountPayed) ELSE 0 END ,
	'JUN' = CASE WHEN MONTH(CD.ScheduledDateOnUtc )=6 AND CD.StateId<>1 THEN SUM(CD.AmountPayed) ELSE 0 END ,
	'JUL' = CASE WHEN MONTH(CD.ScheduledDateOnUtc )=7 AND CD.StateId<>1 THEN SUM(CD.AmountPayed) ELSE 0 END ,
	'AGO' = CASE WHEN MONTH(CD.ScheduledDateOnUtc )=8 AND CD.StateId<>1 THEN SUM(CD.AmountPayed) ELSE 0 END ,
	'SEP' = CASE WHEN MONTH(CD.ScheduledDateOnUtc )=9 AND CD.StateId<>1 THEN SUM(CD.AmountPayed) ELSE 0 END ,
	'OCT' = CASE WHEN MONTH(CD.ScheduledDateOnUtc )=10 AND CD.StateId<>1 THEN SUM(CD.AmountPayed) ELSE 0 END ,
	'NOV' = CASE WHEN MONTH(CD.ScheduledDateOnUtc )=11 AND CD.StateId<>1 THEN SUM(CD.AmountPayed) ELSE 0 END ,
	'DIC' = CASE WHEN MONTH(CD.ScheduledDateOnUtc )=12 AND CD.StateId<>1 THEN SUM(CD.AmountPayed) ELSE 0 END  
	FROM contributionPayment  CD
	INNER JOIN Contribution C ON C.Id=CD.contributionId
	WHERE  C.Id=@ContributionId and CD.ContributionId=@ContributionId 
	GROUP BY YEAR(CD.ScheduledDateOnUtc ), MONTH(CD.ScheduledDateOnUtc ), CD.StateId, CD.IsAutomatic
) AS ReportContributionPayment
GROUP BY YEARS  ,IsAutomatic, StateId 

DECLARE @newId uniqueidentifier =NEWID();
DECLARE @value XML=(SELECT * FROM  #tmp_reports order by 1 desc FOR XML PATH ('ReportContributionPayment'), root ('ArrayOfReportContributionPayment'))

DELETE FROM Report WHERE source=@Source
INSERT INTO Report VALUES (@newId,@NameReport,@value,'',@ReportState,'',@Source,@newId,GETUTCDATE())
SELECT @TotalRecords = COUNT(1) FROM #tmp_reports
SELECT * FROM Report WHERE [key]=@newId

END
GO

CREATE PROCEDURE [dbo].[SummaryReportLoanPayment]
(
	@LoanId int,
	@NameReport nvarchar(255),
	@ReportState int,
	@Source nvarchar(250),
	@TotalRecords int = null OUTPUT
)
AS
BEGIN

SET LANGUAGE Spanish

SELECT 
DATENAME(mm,LP.ScheduledDateOnUtc)  as MonthName,
YEAR(LP.ScheduledDateOnUtc) as Year,
lp.Quota as Quota,
lp.MonthlyCapital as MonthlyCapital,
lp.MonthlyFee as MonthlyFee,
lp.MonthlyQuota as MonthlyQuota ,
lp.MonthlyPayed as MonthlyPayed,
lp.StateId as StateId,
lp.description as Description
INTO #tmp_reports
FROM  LoanPayment LP
INNER JOIN Loan L ON L.Id=LP.LoanId
WHERE L.Id=@LoanId

DECLARE @newId uniqueidentifier =NEWID();
DECLARE @value XML=(SELECT * FROM  #tmp_reports order by Quota  FOR XML PATH ('ReportLoanPayment'), root ('ArrayOfReportLoanPayment'))

DELETE FROM Report WHERE source=@Source
INSERT INTO Report VALUES (@newId,@NameReport,@value,'',@ReportState,'',@Source,@newId,GETUTCDATE())
SELECT @TotalRecords = COUNT(1) FROM #tmp_reports
SELECT * FROM Report WHERE [key]=@newId
 
END
GO

CREATE PROCEDURE [dbo].[SummaryReportLoanPaymentKardex]
(
	@LoanId int,
	@NameReport nvarchar(255),
	@ReportState int,
	@Source nvarchar(250),
	@TotalRecords int = null OUTPUT
)
AS
BEGIN

SET LANGUAGE Spanish
SELECT 
YEAR(LP.ProcessedDateOnUtc) as Year,
MONTH(LP.ProcessedDateOnUtc) as MONTH,
DATENAME(mm,LP.ProcessedDateOnUtc)  as MonthName,
SUM(lp.MonthlyPayed) as MonthlyPayed ,
LP.StateId as StateId,
LP.IsAutomatic AS IsAutomatic
INTO #tmp_reports_t
FROM  LoanPayment LP
INNER JOIN Loan L ON L.Id=LP.LoanId
WHERE L.Id=@LoanId   AND LP.MonthlyPayed>0
GROUP BY lp.TransactionNumber, YEAR(lp.ProcessedDateOnUtc),MONTH(lp.ProcessedDateOnUtc), DATENAME(mm,LP.ProcessedDateOnUtc), LP.StateId, LP.IsAutomatic
ORDER BY 1,2

SELECT Year,MonthName,MonthlyPayed,StateId, IsAutomatic
INTO  #tmp_reports 
FROM #tmp_reports_t

DECLARE @newId uniqueidentifier =NEWID();
DECLARE @value XML=(SELECT * FROM  #tmp_reports   FOR XML PATH ('ReportLoanPaymentKardex'), root ('ArrayOfReportLoanPaymentKardex'))

DELETE FROM Report WHERE source=@Source
INSERT INTO Report VALUES (@newId,@NameReport,@value,'',@ReportState,'',@Source,@newId,GETUTCDATE())
SELECT @TotalRecords = COUNT(1) FROM #tmp_reports
SELECT * FROM Report WHERE [key]=@newId
 
END
GO

CREATE PROCEDURE [dbo].[SummaryReportContributionBenefit]
(
	@ContributionBenefitId int,
	@NameReport nvarchar(255),
	@ReportState int,
	@Source nvarchar(250),
	@TotalRecords int = null OUTPUT
)
AS
BEGIN

DECLARE @combinedString VARCHAR(MAX)
SELECT @combinedString = COALESCE(@combinedString + '|', '') +CompleteName + '('+RelationShip+' '+CAST(Ratio*100 as nvarchar(15)) +' %)-'+ CAST(AmountToPay AS NVARCHAR(10))
FROM ContributionBenefitBank
WHERE ContributionBenefitId = @ContributionBenefitId 

SELECT 
B.Name as BenefitName,
CASE WHEN B.BenefitTypeId=1 then 'AUXILIO ECONOMICO' else 'BENEFICIO ECONOMICO' end AS BenefitType,
CF.NumberOfLiquidation AS NumberOfLiquidation,
CF.AmountBaseOfBenefit AS AmountBaseOfBenefit,
CF.YearInActivity AS YearInActivity,
CF.TabValue AS TabValue,
CF.Discount AS Discount,
CF.SubTotalToPay AS SubTotalToPay,
CF.TotalContributionCaja AS TotalContributionCaja,
CF.TotalContributionCopere AS TotalContributionCopere,
CF.TotalContributionPersonalPayment AS TotalContributionPersonalPayment,
CF.TotalLoan AS TotalLoan,
CF.TotalContributionPersonalPayment  AS TotalLoanToPay,
CF.ReserveFund AS ReserveFund,
CF.TotalReationShip AS TotalReationShip,
CF.TotalToPay AS TotalToPay,
CF.CreatedOnUtc AS CreatedOnUtc,
@combinedString AS Checks
INTO #tmp_reports
FROM ContributionBenefit  CF
INNER JOIN Benefit B ON B.Id=CF.BenefitId
WHERE CF.Id=@ContributionBenefitId


DECLARE @newId uniqueidentifier =NEWID();
DECLARE @value XML=(SELECT * FROM  #tmp_reports order by 1 desc FOR XML PATH ('ReportContributionBenefit'), root ('ArrayOfReportContributionBenefit'))

DELETE FROM Report WHERE source=@Source
INSERT INTO Report VALUES (@newId,@NameReport,@value,'',@ReportState,'',@Source,@newId,GETUTCDATE())
SELECT @TotalRecords = COUNT(1) FROM #tmp_reports
SELECT * FROM Report WHERE [key]=@newId

END
GO

CREATE PROCEDURE FixQuotaCopere 
AS
BEGIN 
	
	Declare @MaxToPay decimal(6,2)= (select value from Setting where Name  =  'contributionsettings.maximumcharge')
	DECLARE @ContributionPaymentId INT
	DECLARE @NextId INT 
	DECLARE @OffSet decimal(12,2) 
	DECLARE @LastDescription NVARCHAR(MAX)
	DECLARE @LastNumber INT
	DECLARE @ValueIndex INT =  0;
	

	DECLARE MY_CURSOR CURSOR 
	  LOCAL STATIC READ_ONLY FORWARD_ONLY
	FOR 
	SELECT id FROM ContributionPayment WHERE AmountTotal>@MaxToPay

	OPEN MY_CURSOR
	FETCH NEXT FROM MY_CURSOR INTO @ContributionPaymentId
	WHILE @@FETCH_STATUS = 0
	BEGIN 
		SET @ValueIndex=  0;

		WHILE @ValueIndex <= 20
		BEGIN
			SET @OffSet = (select AmountTotal-@MaxToPay from ContributionPayment WHERE Id=(@ContributionPaymentId+@ValueIndex))
			SET @LastDescription =(select ISNULL([DESCRIPTION],'')  from ContributionPayment WHERE Id=(@ContributionPaymentId+@ValueIndex))
			SET @LastNumber =(select Number from ContributionPayment WHERE Id=(@ContributionPaymentId+@ValueIndex))
			
			IF(@OffSet>0)
				BEGIN
					UPDATE ContributionPayment 
					SET AmountTotal=@MaxToPay, [DESCRIPTION]=ISNULL([DESCRIPTION],'')  + '|' + 'El valor de la couta a alcanzado el valor máximo, el saldo se va a prorratear en la siguiente couta pendiente de envio (S/ ' + CAST(@OffSet AS NVARCHAR(50)) + ')'  
					WHERE  Id=@ContributionPaymentId+@ValueIndex  
					
					UPDATE ContributionPayment 
					SET AmountTotal=AmountTotal+@OffSet,
						AmountOld=@OffSet, 
						[DESCRIPTION]=ISNULL([DESCRIPTION],'')  + '|' + 'El valor de la couta a aumentado debido a un prorrateo de una couta anterior N° '+CAST(@LastNumber AS NVARCHAR(50)), 
						NumberOld =@LastNumber						
					WHERE  Id=(@ContributionPaymentId+1+@ValueIndex) 
				END
			SET @ValueIndex = @ValueIndex + 1;
		END	
		FETCH NEXT FROM MY_CURSOR INTO @ContributionPaymentId
	END
	CLOSE MY_CURSOR
	DEALLOCATE MY_CURSOR

	
	UPDATE Contribution
	set Contribution.AmountPayed = T.AmountPayedTotal
	FROM 
	(
	SELECT  ContributionId, sum(AmountPayed) AS AmountPayedTotal
	FROM ContributionPayment 
	Group BY ContributionId
	)  T 
	WHERE T.ContributionId=Contribution.Id
	
	UPDATE Contribution 	
	set Contribution.PayedCycles =T.PayedCycles
	FROM 
	(
		SELECT ContributionPayment.ContributionId, COUNT(*) AS PayedCycles
		FROM ContributionPayment 
		WHERE ContributionPayment.StateId=4 
		GROUP BY ContributionPayment.ContributionId
	) T
	WHERE  T.ContributionId=Contribution.Id


	UPDATE Contribution 	
	set Contribution.PartialCycles =T.PartialCycles
	FROM 
	(
		SELECT ContributionPayment.ContributionId, COUNT(*) AS PartialCycles
		FROM ContributionPayment 
		WHERE ContributionPayment.StateId=3
		GROUP BY ContributionPayment.ContributionId
	) T
	WHERE  T.ContributionId=Contribution.Id
 
END 
GO

CREATE PROCEDURE FixQuotaCaja 
AS
BEGIN
 
	
	Declare @MaxToPay decimal(6,2)= (select value from Setting where Name  =  'contributionsettings.maximumcharge')
	DECLARE @ContributionPaymentId INT
	DECLARE @NextId INT 
	DECLARE @OffSet decimal(12,2) 
	DECLARE @LastDescription NVARCHAR(MAX)
	DECLARE @LastNumber INT
	DECLARE @ValueIndex INT =  0;
	

	DECLARE MY_CURSOR CURSOR 
	  LOCAL STATIC READ_ONLY FORWARD_ONLY
	FOR 
	SELECT id FROM ContributionPayment WHERE AmountTotal>@MaxToPay

	OPEN MY_CURSOR
	FETCH NEXT FROM MY_CURSOR INTO @ContributionPaymentId
	WHILE @@FETCH_STATUS = 0
	BEGIN 
		SET @ValueIndex=  0;

		WHILE @ValueIndex <= 20
		BEGIN
			SET @OffSet = (select AmountTotal-@MaxToPay from ContributionPayment WHERE Id=(@ContributionPaymentId+@ValueIndex))
			SET @LastDescription =(select ISNULL([DESCRIPTION],'')  from ContributionPayment WHERE Id=(@ContributionPaymentId+@ValueIndex))
			SET @LastNumber =(select Number from ContributionPayment WHERE Id=(@ContributionPaymentId+@ValueIndex))
			
			IF(@OffSet>0)
				BEGIN
					UPDATE ContributionPayment 
					SET AmountTotal=@MaxToPay, [DESCRIPTION]=ISNULL([DESCRIPTION],'')  + '|' + 'El valor de la couta a alcanzado el valor máximo, el saldo se va a prorratear en la siguiente couta pendiente de envio (S/ ' + CAST(@OffSet AS NVARCHAR(50)) + ')'  
					WHERE  Id=@ContributionPaymentId+@ValueIndex  
					
					UPDATE ContributionPayment 
					SET AmountTotal=AmountTotal+@OffSet,
						AmountOld=@OffSet, 
						[DESCRIPTION]=ISNULL([DESCRIPTION],'')  + '|' + 'El valor de la couta a aumentado debido a un prorrateo de una couta anterior N° '+CAST(@LastNumber AS NVARCHAR(50)), 
						NumberOld =@LastNumber						
					WHERE  Id=(@ContributionPaymentId+1+@ValueIndex) 
				END
			SET @ValueIndex = @ValueIndex + 1;
		END	
		FETCH NEXT FROM MY_CURSOR INTO @ContributionPaymentId
	END
	CLOSE MY_CURSOR
	DEALLOCATE MY_CURSOR

	
	UPDATE Contribution
	set Contribution.AmountPayed = T.AmountPayedTotal
	FROM 
	(
	SELECT  ContributionId, sum(AmountPayed) AS AmountPayedTotal
	FROM ContributionPayment 
	Group BY ContributionId
	)  T 
	WHERE T.ContributionId=Contribution.Id
	
	UPDATE Contribution 	
	set Contribution.PayedCycles =T.PayedCycles
	FROM 
	(
		SELECT ContributionPayment.ContributionId, COUNT(*) AS PayedCycles
		FROM ContributionPayment 
		WHERE ContributionPayment.StateId=4 
		GROUP BY ContributionPayment.ContributionId
	) T
	WHERE  T.ContributionId=Contribution.Id


	UPDATE Contribution 	
	set Contribution.PartialCycles =T.PartialCycles
	FROM 
	(
		SELECT ContributionPayment.ContributionId, COUNT(*) AS PartialCycles
		FROM ContributionPayment 
		WHERE ContributionPayment.StateId=3
		GROUP BY ContributionPayment.ContributionId
	) T
	WHERE  T.ContributionId=Contribution.Id
 
END 
GO

CREATE PROCEDURE [UpdateContributionPaymentCopere]
(
	@XmlPackage xml, 
	@Year int, 
	@Month int
)
AS
BEGIN

CREATE TABLE #ContributionPaymentCopereTmp
( 
	[ContributionPaymentId] [int] NOT NULL,
	[Number] [int] NOT NULL,
	[NumberOld] [int] NOT NULL,
	[ContributionId] [int] NOT NULL,
	[Amount1] decimal(6,2) NOT NULL,
	[Amount2] decimal(6,2) NOT NULL,
	[Amount3] decimal(6,2) NOT NULL,
	[AmountOld] decimal(6,2) NOT NULL,
	[AmountTotal] decimal(6,2) NOT NULL,
	[AmountPayed] decimal(6,2) NOT NULL,
	[StateId] int NOT NULL,
	[BankName] [nvarchar](500) NOT NULL,
	[AccountNumber] [nvarchar](500) NOT NULL,
	[TransactionNumber] [nvarchar](500) NOT NULL,
	[Reference] [nvarchar](500) NOT NULL,
	[Description] [nvarchar](500) NOT NULL
)

INSERT INTO #ContributionPaymentCopereTmp			
SELECT	
nref.value('ContributionPaymentId[1]', 'int'), --ContributionPaymentId
nref.value('Number[1]', 'int'), --Number
nref.value('NumberOld[1]', 'int'), --NumberOld
nref.value('ContributionId[1]', 'int'), --ContributionId
nref.value('Amount1[1]', 'decimal(6,2)'),--Amount1
nref.value('Amount2[1]', 'decimal(6,2)'),--Amount2
nref.value('Amount3[1]', 'decimal(6,2)'),--Amount3
nref.value('AmountOld[1]', 'decimal(6,2)'),--AmountOld
nref.value('AmountTotal[1]', 'decimal(6,2)'),--AmountTotal
nref.value('AmountPayed[1]', 'decimal(6,2)'),--AmountPayed
nref.value('StateId[1]', 'int'), --StateId
nref.value('BankName[1]', 'nvarchar(500)'), --BankName
nref.value('AccountNumber[1]', 'nvarchar(500)'), --AccountNumber
nref.value('TransactionNumber[1]', 'nvarchar(500)'), --TransactionNumber
nref.value('Reference[1]', 'nvarchar(500)'), --Reference
nref.value('Description[1]', 'nvarchar(1000)') --[Description]
FROm @XmlPackage.nodes('//ArrayOfInfoContribution/InfoContribution') AS R(nref)

------------------------------------------------------------------------
--1) Split int 3 types {PagoParcial = 3,Pagado = 4,SinLiquidez=5}
------------------------------------------------------------------------

select  * into #ContributionPaymentCopereTmp3 from #ContributionPaymentCopereTmp where stateId=3
select  * into #ContributionPaymentCopereTmp4 from #ContributionPaymentCopereTmp where stateId=4
select  * into #ContributionPaymentCopereTmp5 from #ContributionPaymentCopereTmp where stateId=5

------------------------------------------------------------------------
--2) Update Data 4 {PagoParcial = 3, Pagado = 4, SinLiquidez=5}
------------------------------------------------------------------------

UPDATE ContributionPayment 
SET 
AmountPayed=#ContributionPaymentCopereTmp4.AmountPayed,
ProcessedDateOnUtc=GETUTCDATE(), 
StateId=4,
BankName=#ContributionPaymentCopereTmp4.BankName,
AccountNumber  = cast(convert(NVARCHAR, getutcdate(), 112) +REPLACE(convert(NVARCHAR, getutcdate(), 114) ,':','') as nvarchar(50))
FROM  #ContributionPaymentCopereTmp4
WHERE #ContributionPaymentCopereTmp4.ContributionPaymentId=ContributionPayment.Id 

------------------------------------------------------------------------
--3) Update Data 5 {PagoParcial = 3, Pagado = 4, SinLiquidez=5}
------------------------------------------------------------------------

Declare @MaximumCharge decimal(6,2)= (select Value from Setting where Name  = 'contributionsettings.maximumcharge')

UPDATE  ContributionPayment 
set 
ContributionPayment.StateId=5,
ContributionPayment.AmountPayed=#ContributionPaymentCopereTmp5.AmountPayed,
ContributionPayment.ProcessedDateOnUtc=GETUTCDATE(),
ContributionPayment.BankName=#ContributionPaymentCopereTmp5.BankName,
ContributionPayment.Description='No se ha enviado informacion de pagos en la interfaz del ' + #ContributionPaymentCopereTmp5.BankName
FROM #ContributionPaymentCopereTmp5 
WHERE #ContributionPaymentCopereTmp5.ContributionPaymentId=ContributionPayment.Id

 

UPDATE  ContributionPayment 
SET 
ContributionPayment.AmountTotal=ContributionPayment.AmountTotal+TMP.NoPayed,
contributionPayment.AmountOld=@MaximumCharge,
ContributionPayment.NumberOld=TMP.Number,
ContributionPayment.[Description] ='Valor de la couta aumentado por el sistema ACMR debido a la falta de liquitdez de la cuota N° ' + CAST(TMP.Number as nvarchar(3)),
ContributionPayment.AccountNumber  = cast(convert(NVARCHAR, getutcdate(), 112) +REPLACE(convert(NVARCHAR, getutcdate(), 114) ,':','') as nvarchar(50))
FROM 
(
	SELECT CP.ContributionId AS ContributionId, NextPayment.NumberNextQuota AS NumberNextQuota ,
	CPT.Number AS Number, CPT.AmountTotal AS NoPayed
	--The nex quota and the Actual 
	FROM 
	ContributionPayment CP
	INNER JOIN (
			SELECT ContributionId, MIN(Number) as NumberNextQuota
			FROM ContributionPayment 
			WHERE StateId = 1 --Get the next Quota in stateId=1 (Pendiente) 
			GROUP BY ContributionId
	) NextPayment ON NextPayment.ContributionId=CP.ContributionId
	INNER JOIN #ContributionPaymentCopereTmp5 CPT ON CPT.ContributionId=CP.ContributionId  --This is Unique
	GROUP BY CP.ContributionId , NextPayment.NumberNextQuota  ,	CPT.Number ,CPT.AmountTotal
) as TMP
WHERE --only the next quota to pay
ContributionPayment.ContributionId=TMP.ContributionId AND ContributionPayment.Number=TMP.NumberNextQuota

UPDATE Contribution set IsDelay=1 , DelayCycles =DelayCycles+1
FROM  #ContributionPaymentCopereTmp5 
WHERE #ContributionPaymentCopereTmp5.ContributionId = Contribution.Id

------------------------------------------------------------------------
--3) Update Data 3 {PagoParcial = 3, Pagado = 4, SinLiquidez=5}
------------------------------------------------------------------------

UPDATE  ContributionPayment 
set 
ContributionPayment.StateId=3,
ContributionPayment.AmountPayed=#ContributionPaymentCopereTmp3.AmountPayed,
ContributionPayment.ProcessedDateOnUtc=GETUTCDATE(),
ContributionPayment.BankName=#ContributionPaymentCopereTmp3.BankName,
ContributionPayment.Description='Pago parcial realizado del copere, el saldo se va a cargar en la siguiente couta con estado pendiente'
FROM #ContributionPaymentCopereTmp3 
WHERE #ContributionPaymentCopereTmp3 .ContributionPaymentId=ContributionPayment.Id

UPDATE  ContributionPayment 
SET 
ContributionPayment.AmountTotal=ContributionPayment.AmountTotal+TMP.OffSet,
contributionPayment.AmountOld=TMP.OffSet,
ContributionPayment.NumberOld=TMP.Number, 
ContributionPayment.AccountNumber  = cast(convert(NVARCHAR, getutcdate(), 112) +REPLACE(convert(NVARCHAR, getutcdate(), 114) ,':','') as nvarchar(50)),
ContributionPayment.[Description] ='Valor de la couta aumentado por el sistema ACMR debido a la falta de liquitdez de la cuota N° ' + CAST(TMP.Number as nvarchar(3))
FROM 
(
	SELECT CP.ContributionId AS ContributionId, NextPayment.NumberNextQuota AS NumberNextQuota ,
	CPT.Number AS Number, CPT.AmountTotal-CPT.AmountPayed AS OffSet
	--The nex quota and the Actual 
	FROM 
	ContributionPayment CP
	INNER JOIN (
			SELECT ContributionId,MIN(Number) as NumberNextQuota
			FROM ContributionPayment 
			WHERE StateId = 1 --Get the next Quota in stateId=1 (Pendiente) 
			GROUP BY ContributionId
	) NextPayment ON NextPayment.ContributionId=CP.ContributionId
	INNER JOIN #ContributionPaymentCopereTmp3 CPT ON CPT.ContributionId=CP.ContributionId  --This is Unique
	GROUP BY CP.ContributionId , NextPayment.NumberNextQuota ,CPT.Number ,CPT.AmountTotal-CPT.AmountPayed
) as TMP
WHERE --only the next quota to pay
ContributionPayment.ContributionId=TMP.ContributionId AND ContributionPayment.Number=TMP.NumberNextQuota

EXEC FixQuotaCopere

END
GO

CREATE PROCEDURE [UpdateContributionPaymentCaja]
(
	@XmlPackage xml, 
	@Year int, 
	@Month int
)
AS
BEGIN

CREATE TABLE #ContributionPaymentCajaTmp
( 
	[ContributionPaymentId] [int] NOT NULL,
	[Number] [int] NOT NULL,
	[NumberOld] [int] NOT NULL,
	[ContributionId] [int] NOT NULL,
	[Amount1] decimal(6,2) NOT NULL,
	[Amount2] decimal(6,2) NOT NULL,
	[Amount3] decimal(6,2) NOT NULL,
	[AmountOld] decimal(6,2) NOT NULL,
	[AmountTotal] decimal(6,2) NOT NULL,
	[AmountPayed] decimal(6,2) NOT NULL,
	[StateId] int NOT NULL,
	[BankName] [nvarchar](500) NOT NULL,
	[AccountNumber] [nvarchar](500) NOT NULL,
	[TransactionNumber] [nvarchar](500) NOT NULL,
	[Reference] [nvarchar](500) NOT NULL,
	[Description] [nvarchar](500) NOT NULL
)

INSERT INTO #ContributionPaymentCajaTmp			
SELECT	
nref.value('ContributionPaymentId[1]', 'int'), --ContributionPaymentId
nref.value('Number[1]', 'int'), --Number
nref.value('NumberOld[1]', 'int'), --NumberOld
nref.value('ContributionId[1]', 'int'), --ContributionId
nref.value('Amount1[1]', 'decimal(6,2)'),--Amount1
nref.value('Amount2[1]', 'decimal(6,2)'),--Amount2
nref.value('Amount3[1]', 'decimal(6,2)'),--Amount3
nref.value('AmountOld[1]', 'decimal(6,2)'),--AmountOld
nref.value('AmountTotal[1]', 'decimal(6,2)'),--AmountTotal
nref.value('AmountPayed[1]', 'decimal(6,2)'),--AmountPayed
nref.value('StateId[1]', 'int'), --StateId
nref.value('BankName[1]', 'nvarchar(500)'), --BankName
nref.value('AccountNumber[1]', 'nvarchar(500)'), --AccountNumber
nref.value('TransactionNumber[1]', 'nvarchar(500)'), --TransactionNumber
nref.value('Reference[1]', 'nvarchar(500)'), --Reference
nref.value('Description[1]', 'nvarchar(1000)') --[Description]
FROm @XmlPackage.nodes('//ArrayOfInfoContribution/InfoContribution') AS R(nref)

------------------------------------------------------------------------
--1) Split int 3 types {PagoParcial = 3,Pagado = 4,SinLiquidez=5}
------------------------------------------------------------------------

select  * into #ContributionPaymentCajaTmp3 from #ContributionPaymentCajaTmp where stateId=3
select  * into #ContributionPaymentCajaTmp4 from #ContributionPaymentCajaTmp where stateId=4
select  * into #ContributionPaymentCajaTmp5 from #ContributionPaymentCajaTmp where stateId=5

------------------------------------------------------------------------
--2) Update Data 4 {PagoParcial = 3, Pagado = 4, SinLiquidez=5}
------------------------------------------------------------------------

UPDATE ContributionPayment 
SET 
AmountPayed=#ContributionPaymentCajaTmp4.AmountPayed,
ProcessedDateOnUtc=GETUTCDATE(), 
StateId=4,
BankName=#ContributionPaymentCajaTmp4.BankName,
AccountNumber  = cast(convert(NVARCHAR, getutcdate(), 112) +REPLACE(convert(NVARCHAR, getutcdate(), 114) ,':','') as nvarchar(50))
FROM  #ContributionPaymentCajaTmp4
WHERE #ContributionPaymentCajaTmp4.ContributionPaymentId=ContributionPayment.Id 

------------------------------------------------------------------------
--3) Update Data 5 {PagoParcial = 3, Pagado = 4, SinLiquidez=5}
------------------------------------------------------------------------

Declare @MaximumCharge decimal(6,2)= (select Value from Setting where Name  = 'contributionsettings.maximumcharge')

UPDATE  ContributionPayment 
set 
ContributionPayment.StateId=5,
ContributionPayment.AmountPayed=#ContributionPaymentCajaTmp5.AmountPayed,
ContributionPayment.ProcessedDateOnUtc=GETUTCDATE(),
ContributionPayment.BankName=#ContributionPaymentCajaTmp5.BankName,
ContributionPayment.Description='No se ha enviado informacion de pagos en la interfaz del ' + #ContributionPaymentCajaTmp5.BankName
FROM #ContributionPaymentCajaTmp5 
WHERE #ContributionPaymentCajaTmp5.ContributionPaymentId=ContributionPayment.Id

 

UPDATE  ContributionPayment 
SET 
ContributionPayment.AmountTotal=ContributionPayment.AmountTotal+TMP.NoPayed,
contributionPayment.AmountOld=@MaximumCharge,
ContributionPayment.NumberOld=TMP.Number,
ContributionPayment.[Description] ='Valor de la couta aumentado por el sistema ACMR debido a la falta de liquitdez de la cuota N° ' + CAST(TMP.Number as nvarchar(3)),
ContributionPayment.AccountNumber  = cast(convert(NVARCHAR, getutcdate(), 112) +REPLACE(convert(NVARCHAR, getutcdate(), 114) ,':','') as nvarchar(50))
FROM 
(
	SELECT CP.ContributionId AS ContributionId, NextPayment.NumberNextQuota AS NumberNextQuota ,
	CPT.Number AS Number, CPT.AmountTotal AS NoPayed
	--The nex quota and the Actual 
	FROM 
	ContributionPayment CP
	INNER JOIN (
			SELECT ContributionId, MIN(Number) as NumberNextQuota
			FROM ContributionPayment 
			WHERE StateId = 1 --Get the next Quota in stateId=1 (Pendiente) 
			GROUP BY ContributionId
	) NextPayment ON NextPayment.ContributionId=CP.ContributionId
	INNER JOIN #ContributionPaymentCajaTmp5 CPT ON CPT.ContributionId=CP.ContributionId  --This is Unique
	GROUP BY CP.ContributionId , NextPayment.NumberNextQuota  ,	CPT.Number ,CPT.AmountTotal
) as TMP
WHERE --only the next quota to pay
ContributionPayment.ContributionId=TMP.ContributionId AND ContributionPayment.Number=TMP.NumberNextQuota

UPDATE Contribution set IsDelay=1 , DelayCycles =DelayCycles+1
FROM  #ContributionPaymentCajaTmp5 
WHERE #ContributionPaymentCajaTmp5.ContributionId = Contribution.Id

------------------------------------------------------------------------
--3) Update Data 3 {PagoParcial = 3, Pagado = 4, SinLiquidez=5}
------------------------------------------------------------------------

UPDATE  ContributionPayment 
set 
ContributionPayment.StateId=3,
ContributionPayment.AmountPayed=#ContributionPaymentCajaTmp3.AmountPayed,
ContributionPayment.ProcessedDateOnUtc=GETUTCDATE(),
ContributionPayment.BankName=#ContributionPaymentCajaTmp3.BankName,
ContributionPayment.Description='Pago parcial realizado del Caja, el saldo se va a cargar en la siguiente couta con estado pendiente'
FROM #ContributionPaymentCajaTmp3 
WHERE #ContributionPaymentCajaTmp3 .ContributionPaymentId=ContributionPayment.Id

UPDATE  ContributionPayment 
SET 
ContributionPayment.AmountTotal=ContributionPayment.AmountTotal+TMP.OffSet,
contributionPayment.AmountOld=TMP.OffSet,
ContributionPayment.NumberOld=TMP.Number, 
ContributionPayment.AccountNumber  = cast(convert(NVARCHAR, getutcdate(), 112) +REPLACE(convert(NVARCHAR, getutcdate(), 114) ,':','') as nvarchar(50)),
ContributionPayment.[Description] ='Valor de la couta aumentado por el sistema ACMR debido a la falta de liquitdez de la cuota N° ' + CAST(TMP.Number as nvarchar(3))
FROM 
(
	SELECT CP.ContributionId AS ContributionId, NextPayment.NumberNextQuota AS NumberNextQuota ,
	CPT.Number AS Number, CPT.AmountTotal-CPT.AmountPayed AS OffSet
	--The nex quota and the Actual 
	FROM 
	ContributionPayment CP
	INNER JOIN (
			SELECT ContributionId,MIN(Number) as NumberNextQuota
			FROM ContributionPayment 
			WHERE StateId = 1 --Get the next Quota in stateId=1 (Pendiente) 
			GROUP BY ContributionId
	) NextPayment ON NextPayment.ContributionId=CP.ContributionId
	INNER JOIN #ContributionPaymentCajaTmp3 CPT ON CPT.ContributionId=CP.ContributionId  --This is Unique
	GROUP BY CP.ContributionId , NextPayment.NumberNextQuota ,CPT.Number ,CPT.AmountTotal-CPT.AmountPayed
) as TMP
WHERE --only the next quota to pay
ContributionPayment.ContributionId=TMP.ContributionId AND ContributionPayment.Number=TMP.NumberNextQuota

EXEC FixQuotaCaja

END
GO

CREATE PROCEDURE [UpdateLoanPaymentCopere]
(
	@XmlPackage xml, 
	@Year int, 
	@Month int
)
AS
BEGIN

CREATE TABLE #LoanPaymentCopereTmp
( 
	[LoanPaymentId] [int] NOT NULL,
	[LoanId] [int] NOT NULL,	
	[Quota] [int] NOT NULL,
	[MonthlyQuota] decimal(6,2) NOT NULL,
	[MonthlyFee] decimal(6,2) NOT NULL,
	[MonthlyCapital] decimal(6,2) NOT NULL,
	[MonthlyPayed] decimal(6,2) NOT NULL,
	[StateId] int NOT NULL,
	[BankName] [nvarchar](500) NOT NULL,
	[AccountNumber] [nvarchar](500) NOT NULL,
	[TransactionNumber] [nvarchar](500) NOT NULL,
	[Reference] [nvarchar](500) NOT NULL,
	[Description] [nvarchar](500) NOT NULL
)
insert into #LoanPaymentCopereTmp					
SELECT	
nref.value('LoanPaymentId[1]', 'int'), --LoanPaymentId
nref.value('LoanId[1]', 'int'), --LoanId
nref.value('Quota[1]', 'int'), --Quota
nref.value('MonthlyQuota[1]', 'decimal(6,2)'),--MonthlyQuota
nref.value('MonthlyFee[1]', 'decimal(6,2)'),--MonthlyFee
nref.value('MonthlyCapital[1]', 'decimal(6,2)'),--MonthlyCapital
nref.value('MonthlyPayed[1]', 'decimal(6,2)'),--MonthlyPayed
nref.value('StateId[1]', 'int'), --StateId
nref.value('BankName[1]', 'nvarchar(500)'), --BankName
nref.value('AccountNumber[1]', 'nvarchar(500)'), --AccountNumber
nref.value('TransactionNumber[1]', 'nvarchar(500)'), --TransactionNumber
nref.value('Reference[1]', 'nvarchar(500)'), --Reference
nref.value('Description[1]', 'nvarchar(1000)') --[Description]
FROm @XmlPackage.nodes('//ArrayOfInfoLoan/InfoLoan') AS R(nref)

------------------------------------------------------------------------
--1) Split int 3 types {PagoParcial = 3,Pagado = 4,SinLiquidez=5}
------------------------------------------------------------------------

select  * into #LoanPaymentTmpCopere3 from #LoanPaymentCopereTmp where stateId=3
select  * into #LoanPaymentTmpCopere4 from #LoanPaymentCopereTmp where stateId=4
select  * into #LoanPaymentTmpCopere5 from #LoanPaymentCopereTmp where stateId=5

------------------------------------------------------------------------
--2) Update Data 4 {PagoParcial = 3, Pagado = 4, SinLiquidez=5}
------------------------------------------------------------------------

UPDATE LoanPayment 
SET 
MonthlyPayed=#LoanPaymentTmpCopere4.MonthlyPayed,
ProcessedDateOnUtc=GETUTCDATE(), 
StateId=4,
BankName=#LoanPaymentTmpCopere4.BankName,
AccountNumber = cast(convert(NVARCHAR, getutcdate(), 112) +REPLACE(convert(NVARCHAR, getutcdate(), 114) ,':','') as nvarchar(50)),
Description='Cobro realizado correctamente por la interfaz de cobros de ACMR'
FROM  #LoanPaymentTmpCopere4
WHERE #LoanPaymentTmpCopere4.LoanPaymentId=LoanPayment.Id 

------------------------------------------------------------------------
--3) Update Data 5 {PagoParcial = 3, Pagado = 4, SinLiquidez=5}
------------------------------------------------------------------------

--Update LoanPayment
UPDATE  LoanPayment 
SET 
LoanPayment.StateId=5,
LoanPayment.AccountNumber = cast(convert(NVARCHAR, getutcdate(), 112) +REPLACE(convert(NVARCHAR, getutcdate(), 114) ,':','') as nvarchar(50)),
LoanPayment.ProcessedDateOnUtc=GETUTCDATE(), 
BankName=#LoanPaymentTmpCopere5.BankName
FROM #LoanPaymentTmpCopere5 
WHERE #LoanPaymentTmpCopere5.LoanPaymentId=LoanPayment.Id

--Insert New LoanPayment
SELECT LP.LoanId,(MAX(LP.Quota)+1 ) as Quota, MAX(LP.ScheduledDateOnUtc) AS ScheduledDateOnUtc  into         #tempMax
FROM  LoanPayment LP
INNER JOIN  #LoanPaymentTmpCopere5 T ON T.LoanId=LP.LoanId
GROUP BY LP.LoanId

INSERT INTO LoanPayment  
select T.LoanId,T.Quota,L.MonthlyQuota,L.MonthlyFee,
L.MonthlyCapital,0, 1, 1,'','', '','',
'Cuota Agregada por falta de liquidez en el pago del periodo ' +cast(2016 as nvarchar(4)) + cast(15 as nvarchar(2)),
DATEADD (month , 1 ,T.ScheduledDateOnUtc), null 
FROM  LoanPayment L
INNER JOIN  #tempMax T ON T.LoanId=L.LoanId
INNER JOIN #LoanPaymentTmpCopere5 LT ON LT.LoanPaymentId=L.Id


------------------------------------------------------------------------
--3) Update Data 3 {PagoParcial = 3, Pagado = 4, SinLiquidez=5}
------------------------------------------------------------------------

--Update LoanPayment
UPDATE  LoanPayment 
SET 
LoanPayment.StateId=3,
LoanPayment.MonthlyPayed = #LoanPaymentTmpCopere3.MonthlyPayed,
LoanPayment.BankName = #LoanPaymentTmpCopere3.BankName,
LoanPayment.ProcessedDateOnUtc=GETUTCDATE(),
LoanPayment.AccountNumber = cast(convert(NVARCHAR, getutcdate(), 112) +REPLACE(convert(NVARCHAR, getutcdate(), 114) ,':','') as nvarchar(50))
FROM #LoanPaymentTmpCopere3 
WHERE #LoanPaymentTmpCopere3.LoanPaymentId=LoanPayment.Id

--Insert New LoanPayment
SELECT LP.LoanId,(MAX(LP.Quota)+1 ) as Quota, 
MAX(LP.ScheduledDateOnUtc) AS ScheduledDateOnUtc  into     #tempMax2
FROM  LoanPayment LP
INNER JOIN  #LoanPaymentTmpCopere3 T ON T.LoanId=LP.LoanId
GROUP BY LP.LoanId

INSERT INTO LoanPayment  
select   
T.LoanId,T.Quota,L.MonthlyQuota-LT.MonthlyPayed,
L.MonthlyFee*(((L.MonthlyQuota-LT.MonthlyPayed)/L.MonthlyQuota)),
L.MonthlyCapital*(((L.MonthlyQuota-LT.MonthlyPayed)/L.MonthlyQuota)),
0, 1, 1,'','', '','',
'Cuota Agregada por pago parcial en el pago del periodo ' +cast(2016 as nvarchar(4)) + cast(15 as nvarchar(2)),
DATEADD (month , 1 ,T.ScheduledDateOnUtc)  , null 
FROM  LoanPayment L
INNER JOIN  #tempMax2 T ON T.LoanId=L.LoanId
INNER JOIN #LoanPaymentTmpCopere3 LT ON LT.LoanPaymentId=L.Id

UPDATE Loan
SET Loan.TotalPayed = T.AmountTotalPayed
FROM 
(
	SELECT  LoanId, sum(MonthlyPayed) AS AmountTotalPayed
	FROM LoanPayment 
	Group BY LoanId
)  T 
WHERE T.LoanId=Loan.Id

END
GO

CREATE PROCEDURE [UpdateLoanPaymentCaja]
(
	@XmlPackage xml, 
	@Year int, 
	@Month int
)
AS
BEGIN

CREATE TABLE #LoanPaymentTmpCaja
( 
	[LoanPaymentId] [int] NOT NULL,
	[LoanId] [int] NOT NULL,	
	[Quota] [int] NOT NULL,
	[MonthlyQuota] decimal(6,2) NOT NULL,
	[MonthlyFee] decimal(6,2) NOT NULL,
	[MonthlyCapital] decimal(6,2) NOT NULL,
	[MonthlyPayed] decimal(6,2) NOT NULL,
	[StateId] int NOT NULL,
	[BankName] [nvarchar](500) NOT NULL,
	[AccountNumber] [nvarchar](500) NOT NULL,
	[TransactionNumber] [nvarchar](500) NOT NULL,
	[Reference] [nvarchar](500) NOT NULL,
	[Description] [nvarchar](500) NOT NULL
)
insert into #LoanPaymentTmpCaja					
SELECT	
nref.value('LoanPaymentId[1]', 'int'), --LoanPaymentId
nref.value('LoanId[1]', 'int'), --LoanId
nref.value('Quota[1]', 'int'), --Quota
nref.value('MonthlyQuota[1]', 'decimal(6,2)'),--MonthlyQuota
nref.value('MonthlyFee[1]', 'decimal(6,2)'),--MonthlyFee
nref.value('MonthlyCapital[1]', 'decimal(6,2)'),--MonthlyCapital
nref.value('MonthlyPayed[1]', 'decimal(6,2)'),--MonthlyPayed
nref.value('StateId[1]', 'int'), --StateId
nref.value('BankName[1]', 'nvarchar(500)'), --BankName
nref.value('AccountNumber[1]', 'nvarchar(500)'), --AccountNumber
nref.value('TransactionNumber[1]', 'nvarchar(500)'), --TransactionNumber
nref.value('Reference[1]', 'nvarchar(500)'), --Reference
nref.value('Description[1]', 'nvarchar(1000)') --[Description]
FROm @XmlPackage.nodes('//ArrayOfInfoLoan/InfoLoan') AS R(nref)

------------------------------------------------------------------------
--1) Split int 3 types {PagoParcial = 3,Pagado = 4,SinLiquidez=5}
------------------------------------------------------------------------

select  * into #LoanPaymentTmpCaja3 from #LoanPaymentTmpCaja where stateId=3
select  * into #LoanPaymentTmpCaja4 from #LoanPaymentTmpCaja where stateId=4
select  * into #LoanPaymentTmpCaja5 from #LoanPaymentTmpCaja where stateId=5

------------------------------------------------------------------------
--2) Update Data 4 {PagoParcial = 3, Pagado = 4, SinLiquidez=5}
------------------------------------------------------------------------

UPDATE LoanPayment 
SET 
MonthlyPayed=#LoanPaymentTmpCaja4.MonthlyPayed,
ProcessedDateOnUtc=GETUTCDATE(), 
StateId=4,
BankName=#LoanPaymentTmpCaja4.BankName,
AccountNumber = cast(convert(NVARCHAR, getutcdate(), 112) +REPLACE(convert(NVARCHAR, getutcdate(), 114) ,':','') as nvarchar(50)),
Description='Cobro realizado correctamente por la interfaz de cobros de ACMR'
FROM  #LoanPaymentTmpCaja4
WHERE #LoanPaymentTmpCaja4.LoanPaymentId=LoanPayment.Id 

------------------------------------------------------------------------
--3) Update Data 5 {PagoParcial = 3, Pagado = 4, SinLiquidez=5}
------------------------------------------------------------------------

--Update LoanPayment
UPDATE  LoanPayment 
SET 
LoanPayment.StateId=5,
LoanPayment.AccountNumber = cast(convert(NVARCHAR, getutcdate(), 112) +REPLACE(convert(NVARCHAR, getutcdate(), 114) ,':','') as nvarchar(50)),
LoanPayment.ProcessedDateOnUtc=GETUTCDATE(), 
BankName=#LoanPaymentTmpCaja5.BankName
FROM #LoanPaymentTmpCaja5 
WHERE #LoanPaymentTmpCaja5.LoanPaymentId=LoanPayment.Id

--Insert New LoanPayment
SELECT LP.LoanId,(MAX(LP.Quota)+1 ) as Quota, MAX(LP.ScheduledDateOnUtc) AS ScheduledDateOnUtc  into         #tempMax
FROM  LoanPayment LP
INNER JOIN  #LoanPaymentTmpCaja5 T ON T.LoanId=LP.LoanId
GROUP BY LP.LoanId

INSERT INTO LoanPayment  
select T.LoanId,T.Quota,L.MonthlyQuota,L.MonthlyFee,
L.MonthlyCapital,0, 1, 1,'','', '','',
'Cuota Agregada por falta de liquidez en el pago del periodo ' +cast(2016 as nvarchar(4)) + cast(15 as nvarchar(2)),
DATEADD (month , 1 ,T.ScheduledDateOnUtc), null 
FROM  LoanPayment L
INNER JOIN  #tempMax T ON T.LoanId=L.LoanId
INNER JOIN #LoanPaymentTmpCaja5 LT ON LT.LoanPaymentId=L.Id


------------------------------------------------------------------------
--3) Update Data 3 {PagoParcial = 3, Pagado = 4, SinLiquidez=5}
------------------------------------------------------------------------

--Update LoanPayment
UPDATE  LoanPayment 
SET 
LoanPayment.StateId=3,
LoanPayment.MonthlyPayed = #LoanPaymentTmpCaja3.MonthlyPayed,
LoanPayment.ProcessedDateOnUtc=GETUTCDATE(), 
LoanPayment.BankName = #LoanPaymentTmpCaja3.BankName,
LoanPayment.AccountNumber = cast(convert(NVARCHAR, getutcdate(), 112) +REPLACE(convert(NVARCHAR, getutcdate(), 114) ,':','') as nvarchar(50))
FROM #LoanPaymentTmpCaja3 
WHERE #LoanPaymentTmpCaja3.LoanPaymentId=LoanPayment.Id

--Insert New LoanPayment
SELECT LP.LoanId,(MAX(LP.Quota)+1 ) as Quota, MAX(LP.ScheduledDateOnUtc) AS ScheduledDateOnUtc  into     #tempMax2
FROM  LoanPayment LP
INNER JOIN  #LoanPaymentTmpCaja3 T ON T.LoanId=LP.LoanId
GROUP BY LP.LoanId

INSERT INTO LoanPayment  
select   T.LoanId,T.Quota,L.MonthlyQuota-LT.MonthlyPayed,
L.MonthlyFee*(((L.MonthlyQuota-LT.MonthlyPayed)/L.MonthlyQuota)),
L.MonthlyCapital*(((L.MonthlyQuota-LT.MonthlyPayed)/L.MonthlyQuota)),
0, 1, 1,'','', '','',
'Cuota Agregada por pago parcial en el pago del periodo ' +cast(2016 as nvarchar(4)) + cast(15 as nvarchar(2)),
DATEADD (month , 1 ,T.ScheduledDateOnUtc)  , null 
FROM  LoanPayment L
INNER JOIN  #tempMax2 T ON T.LoanId=L.LoanId
INNER JOIN #LoanPaymentTmpCaja3 LT ON LT.LoanPaymentId=L.Id

UPDATE Loan
SET Loan.TotalPayed = T.AmountTotalPayed
FROM 
(
	SELECT  LoanId, sum(MonthlyPayed) AS AmountTotalPayed
	FROM LoanPayment 
	Group BY LoanId
)  T 
WHERE T.LoanId=Loan.Id

END
GO

CREATE PROCEDURE [dbo].[ReportGlobal]
(
	@Year int,
	@Month int,
	@Type int, 
	@Copere nvarchar(1000),
	@Caja nvarchar(1000),
	@NameReport nvarchar(255),
	@ReportState int,
	@Source nvarchar(250),
	@TotalRecords int = null OUTPUT
)
AS
BEGIN

If(@Type=1)
BEGIN
	SELECT * INTO Type1
	FROM ( 
		SELECT 
		Source='Apoyo Ecónomico',
		AdmCode = '                                                                                                       ',
		FirstName =  '                                                                                                       ',
		LastName =  '                                                                                                       ',
		CustomerId = L.CustomerId,
		Number=LP.Quota,
		Payed=LP.MonthlyPayed,
		LP.IsAutomatic,
		Lp.BankName,
		lP.StateId
		FROM LoanPayment LP
		INNER JOIN Loan L ON L.Id=LP.LoanId
		WHERE YEAR(LP.ProcessedDateOnUtc)=@Year AND MONTH(LP.ProcessedDateOnUtc)=@Month 
		
		UNION

		SELECT 
		Source='Aportaciones',
		AdmCode = '                                                                                                       ',
		FirstName =  '                                                                                                       ',
		LastName =  '                                                                                                       ',
		CustomerId = L.CustomerId,
		Number=LP.Number,
		Payed=LP.AmountPayed,
		LP.IsAutomatic,
		Lp.BankName,
		lP.StateId
		FROM ContributionPayment LP
		INNER JOIN Contribution L ON L.Id=LP.ContributionId
		WHERE YEAR(LP.ProcessedDateOnUtc)=@Year AND MONTH(LP.ProcessedDateOnUtc)=@Month
		) AS T

	Update Type1 set AdmCode=Value
	FROM GenericAttribute WHERE KeyGroup='Customer' AND EntityId=Type1.CustomerId AND [Key]='AdmCode'

	Update Type1 set FirstName=Value
	FROM GenericAttribute WHERE KeyGroup='Customer' AND EntityId=Type1.CustomerId AND [Key]='FirstName'

	Update Type1 set LastName=Value
	FROM GenericAttribute WHERE KeyGroup='Customer' AND EntityId=Type1.CustomerId AND [Key]='LastName'
	
	IF(@Copere='Copere' and @Caja='Caja')
	BEGIN
		DELETE FROM Type1 WHERE  BankName NOT IN (@Copere,@Caja) 
	END

	IF(@Copere='' AND @Caja<>'')
	BEGIN
		DELETE FROM Type1 WHERE  BankName NOT IN (@Caja) 
	END

	IF(@Copere<>'' AND @Caja='')
	BEGIN
		DELETE FROM Type1 WHERE  BankName NOT IN (@Copere) 
	END

	END


If(@Type=2)
BEGIN
	SELECT 
	Source='Aportaciones',
	AdmCode = '                                                                                                       ',
	FirstName =  '                                                                                                       ',
	LastName =  '                                                                                                       ',
	CustomerId = L.CustomerId,
	Number=LP.Number,
	Payed=LP.AmountPayed,
	LP.IsAutomatic,
	Lp.BankName,
	lP.StateId 
	INTO Type2
	FROM ContributionPayment LP
	INNER JOIN Contribution L ON L.Id=LP.ContributionId
	WHERE YEAR(LP.ProcessedDateOnUtc)=@Year AND MONTH(LP.ProcessedDateOnUtc)=@Month  

	Update Type2 set AdmCode=Value
	FROM GenericAttribute WHERE KeyGroup='Customer' AND EntityId=Type2.CustomerId AND [Key]='AdmCode'

	Update Type2 set FirstName=Value
	FROM GenericAttribute WHERE KeyGroup='Customer' AND EntityId=Type2.CustomerId AND [Key]='FirstName'

	Update Type2 set LastName=Value
	FROM GenericAttribute WHERE KeyGroup='Customer' AND EntityId=Type2.CustomerId AND [Key]='LastName'
	
	IF(@Copere='Copere' and @Caja='Caja')
	BEGIN
		DELETE FROM Type2 WHERE  BankName NOT IN (@Copere,@Caja) 
	END

	IF(@Copere='' AND @Caja<>'')
	BEGIN
		DELETE FROM Type2 WHERE  BankName NOT IN (@Caja) 
	END

	IF(@Copere<>'' AND @Caja='')
	BEGIN
		DELETE FROM Type2 WHERE  BankName NOT IN (@Copere) 
	END

END

If(@Type=3)
BEGIN
	SELECT 
	Source='Apoyo Ecónomico',
	AdmCode = '                                                                                                       ',
	FirstName =  '                                                                                                       ',
	LastName =  '                                                                                                       ',
	CustomerId = L.CustomerId,
	Number=LP.Quota,
	Payed=LP.MonthlyPayed,
	LP.IsAutomatic,
	Lp.BankName,
	lP.StateId
	INTO Type3
	FROM LoanPayment LP
	INNER JOIN Loan L ON L.Id=LP.LoanId
	WHERE YEAR(LP.ProcessedDateOnUtc)=@Year AND MONTH(LP.ProcessedDateOnUtc)=@Month  

	Update Type3 set AdmCode=Value
	FROM GenericAttribute WHERE KeyGroup='Customer' AND EntityId=Type3.CustomerId AND [Key]='AdmCode'

	Update Type3 set FirstName=Value
	FROM GenericAttribute WHERE KeyGroup='Customer' AND EntityId=Type3.CustomerId AND [Key]='FirstName'

	Update Type3 set LastName=Value
	FROM GenericAttribute WHERE KeyGroup='Customer' AND EntityId=Type3.CustomerId AND [Key]='LastName'
	
	IF(@Copere='Copere' and @Caja='Caja')--TODOS
	BEGIN
		DELETE FROM Type3 WHERE  BankName NOT IN (@Copere,@Caja) 
	END

	IF(@Copere='' AND @Caja<>'')--CAJA
	BEGIN
		DELETE FROM Type3 WHERE  BankName NOT IN (@Caja) 
	END

	IF(@Copere<>'' AND @Caja='')--COPERE
	BEGIN
		DELETE FROM Type3 WHERE  BankName NOT IN (@Copere) 
	END
 
END

DECLARE @newId uniqueidentifier =NEWID();
DECLARE @value XML;

IF OBJECT_ID (N'Type1', N'U') IS NOT NULL 
BEGIN

SET @value=(SELECT * FROM  Type1 order by 1 desc FOR XML PATH ('ReportGlobal'), root ('ArrayOfReportGlobal'))


DELETE FROM Report WHERE source=@Source
INSERT INTO Report VALUES (@newId,@NameReport,@value,'',@ReportState,'',@Source,@newId,GETUTCDATE())
SELECT @TotalRecords = COUNT(1) FROM Type1
DROP TABLE Type1
SELECT * FROM Report WHERE [key]=@newId
 
END

IF OBJECT_ID (N'Type2', N'U') IS NOT NULL 
BEGIN

SET @value =(SELECT * FROM  Type2 order by 1 desc FOR XML PATH  ('ReportGlobal'), root ('ArrayOfReportGlobal'))

DELETE FROM Report WHERE source=@Source
INSERT INTO Report VALUES (@newId,@NameReport,@value,'',@ReportState,'',@Source,@newId,GETUTCDATE())
SELECT @TotalRecords = COUNT(1) FROM Type2
DROP TABLE Type2
SELECT * FROM Report WHERE [key]=@newId

END

IF OBJECT_ID (N'Type3', N'U') IS NOT NULL 
BEGIN

SET @value =(SELECT * FROM  Type3 order by 1 desc FOR XML PATH  ('ReportGlobal'), root ('ArrayOfReportGlobal'))

DELETE FROM Report WHERE source=@Source
INSERT INTO Report VALUES (@newId,@NameReport,@value,'',@ReportState,'',@Source,@newId,GETUTCDATE())
SELECT @TotalRecords = COUNT(1) FROM Type3
DROP TABLE Type3
SELECT * FROM Report WHERE [key]=@newId

END
 
END
GO

CREATE PROCEDURE [dbo].[ReportLoanDetails]
(
	@FromYear int,
	@FromMonth int,
	@FromDay int,
	@ToYear int,
	@ToMonth int,
	@ToDay int,
	@Type int, -- 2 TODOS, 1 COPERE  , CAJA
	@State int,-- 2 TODOS, 1 VIGENTES, 0 CANCELADO
	@NameReport nvarchar(255),
	@ReportState int,
	@Source nvarchar(250),
	@TotalRecords int = null OUTPUT
)
AS
BEGIN

SELECT 
CAST(cast(ApprovalOnUtc as DATE)  AS NVARCHAR(15)) AS ApprovalDate,
CAST(LoanNumber AS NVARCHAR(15)) as LoanNumber,
CASE WHEN Active=1 THEN 'Vigente' ELSE 'Cancelado' End  AS LoanState,
ISNULL(CheckNumber,'') AS CheckNumber,
Source='                                              ',
CustomerId,
FirstName='                                                               ', 
LastName='                                                                ',
AdmCode='                          ',
CustomerState='                                                          ',
LoanAmount, 
TotalSafe,
TotalToPay,
TotalFeed,
TotalAmount,
TotalPayed,
CAST(ROUND(TotalPayed/(1+PERIOD*Tea/1200),2) AS DECIMAL) AS PayedCapital,
CAST(ROUND(totalPayed-(TotalPayed/(1+PERIOD*Tea/1200)),2) AS DECIMAL) AS PayedTax,
TotalAmount- Totalpayed as Debit ,
Period,
MonthlyQuota,
CAST(ROUND(MonthlyQuota/(1+PERIOD*Tea/1200),2) AS decimal)  AS QoutaCapital,
CAST(MonthlyQuota-ROUND(MonthlyQuota/(1+PERIOD*Tea/1200),2) AS decimal)  AS QoutaTax,
CAST(cast(UpdatedOnUtc  as DATE)  AS NVARCHAR(15)) AS LastDate
INTO  #Temp_Loan
FROM Loan 
WHERE Year(ApprovalOnUtc)*10000+Month(ApprovalOnUtc)*100+day(ApprovalOnUtc)>=(@FromYear*10000+@FromMonth*100+@FromDay) AND  
	  Year(ApprovalOnUtc)*10000+Month(ApprovalOnUtc)*100+day(ApprovalOnUtc)<=(@ToYear*10000+@ToMonth*100+@ToDay) 


Update #Temp_Loan set CustomerState=case when Customer.Active=1 then 'Activo' else 'Inactivo' end 
FROM Customer where Id=#Temp_Loan.CustomerId 

Update #Temp_Loan set AdmCode=Value
FROM GenericAttribute WHERE KeyGroup='Customer' AND EntityId=#Temp_Loan.CustomerId AND [Key]='AdmCode'

Update #Temp_Loan set FirstName=Value
FROM GenericAttribute WHERE KeyGroup='Customer' AND EntityId=#Temp_Loan.CustomerId AND [Key]='FirstName'

Update #Temp_Loan set LastName=Value
FROM GenericAttribute WHERE KeyGroup='Customer' AND EntityId=#Temp_Loan.CustomerId AND [Key]='LastName'

UPDATE #Temp_Loan SET SOURCE='CPMP'  WHERE CustomerID IN (SELECT EntityId from GenericAttribute G WHERE G.KeyGroup='Customer' AND  G.[KEY]='MilitarySituationId' AND G.VALUE=2) --CAJA

UPDATE #Temp_Loan SET SOURCE='COPERE'  WHERE CustomerID IN (SELECT EntityId from GenericAttribute G WHERE G.KeyGroup='Customer' AND  G.[KEY]='MilitarySituationId' AND G.VALUE=1) --COPERE
	  
--	@Type int, -- 2 TODOS, 1 COPERE  , CAJA
if(@Type=1)-- SOLO COPERE 
BEGIN
	DELETE FROM #Temp_Loan WHERE CustomerID IN (SELECT EntityId from GenericAttribute G WHERE G.KeyGroup='Customer' AND  G.[KEY]='MilitarySituationId' AND G.VALUE=2) --BORRO CAJA
END

if(@Type=2)-- SOLO CAJA 
BEGIN
	DELETE FROM #Temp_Loan WHERE CustomerID IN (SELECT EntityId from GenericAttribute G WHERE G.KeyGroup='Customer' AND  G.[KEY]='MilitarySituationId' AND G.VALUE=1) --BORRO COPERE
END

--	@State int,-- 2 TODOS, 1 VIGENTES, 0 CANCELADO
-- 1 'Vigente' 2 'Cancelado'

if(@State=1)-- SOLO VIGENTES 
BEGIN
	DELETE FROM #Temp_Loan WHERE LoanState ='Cancelado'
END

if(@State=0)-- SOLO CANCELADOS 
BEGIN
	DELETE FROM #Temp_Loan WHERE LoanState ='Vigente'
END

DECLARE @newId uniqueidentifier =NEWID();
DECLARE @value XML=(SELECT * FROM  #Temp_Loan order by 1 desc FOR XML PATH ('ReportLoanDetail'), root ('ArrayOfReportLoanDetail'))

DELETE FROM Report WHERE source=@Source
INSERT INTO Report VALUES (@newId,@NameReport,@value,'',@ReportState,'',@Source,@newId,GETUTCDATE())
SELECT @TotalRecords = COUNT(1) FROM #Temp_Loan
SELECT * FROM Report WHERE [key]=@newId

END
GO

CREATE PROCEDURE [dbo].[SummaryReportContribution]
(
	@FromYear int,
	@ToYear int,
	@TypeSource int, --1 COPERE 2 CAJA 0 TODOS
	@NameReport nvarchar(255),
	@ReportState int,
	@Source nvarchar(250),
	@TotalRecords int = null OUTPUT
)

AS
BEGIN

SELECT 
C.CUSTOMERID,
TYPESOURCE='                                                  ',
ENE=CASE WHEN MONTH(CONVERT(datetime,SWITCHOFFSET(CONVERT(datetimeoffset,ProcessedDateOnUtc),DATENAME(TzOffset, SYSDATETIMEOFFSET()))))=1 then SUM(CP.AmountPayed) ELSE 0 END ,
FEB=CASE WHEN MONTH(CONVERT(datetime,SWITCHOFFSET(CONVERT(datetimeoffset,ProcessedDateOnUtc),DATENAME(TzOffset, SYSDATETIMEOFFSET()))))=2 then SUM(CP.AmountPayed) ELSE 0 END ,
MAR=CASE WHEN MONTH(CONVERT(datetime,SWITCHOFFSET(CONVERT(datetimeoffset,ProcessedDateOnUtc),DATENAME(TzOffset, SYSDATETIMEOFFSET()))))=3 then SUM(CP.AmountPayed) ELSE 0 END ,
ABR=CASE WHEN MONTH(CONVERT(datetime,SWITCHOFFSET(CONVERT(datetimeoffset,ProcessedDateOnUtc),DATENAME(TzOffset, SYSDATETIMEOFFSET()))))=4 then SUM(CP.AmountPayed) ELSE 0 END ,
MAY=CASE WHEN MONTH(CONVERT(datetime,SWITCHOFFSET(CONVERT(datetimeoffset,ProcessedDateOnUtc),DATENAME(TzOffset, SYSDATETIMEOFFSET()))))=5 then SUM(CP.AmountPayed) ELSE 0 END ,
JUN=CASE WHEN MONTH(CONVERT(datetime,SWITCHOFFSET(CONVERT(datetimeoffset,ProcessedDateOnUtc),DATENAME(TzOffset, SYSDATETIMEOFFSET()))))=6 then SUM(CP.AmountPayed) ELSE 0 END ,
JUL=CASE WHEN MONTH(CONVERT(datetime,SWITCHOFFSET(CONVERT(datetimeoffset,ProcessedDateOnUtc),DATENAME(TzOffset, SYSDATETIMEOFFSET()))))=7 then SUM(CP.AmountPayed) ELSE 0 END ,
AGO=CASE WHEN MONTH(CONVERT(datetime,SWITCHOFFSET(CONVERT(datetimeoffset,ProcessedDateOnUtc),DATENAME(TzOffset, SYSDATETIMEOFFSET()))))=8 then SUM(CP.AmountPayed) ELSE 0 END ,
SEP=CASE WHEN MONTH(CONVERT(datetime,SWITCHOFFSET(CONVERT(datetimeoffset,ProcessedDateOnUtc),DATENAME(TzOffset, SYSDATETIMEOFFSET()))))=9 then SUM(CP.AmountPayed) ELSE 0 END ,
OCT=CASE WHEN MONTH(CONVERT(datetime,SWITCHOFFSET(CONVERT(datetimeoffset,ProcessedDateOnUtc),DATENAME(TzOffset, SYSDATETIMEOFFSET()))))=10 then SUM(CP.AmountPayed) ELSE 0 END ,
NOV=CASE WHEN MONTH(CONVERT(datetime,SWITCHOFFSET(CONVERT(datetimeoffset,ProcessedDateOnUtc),DATENAME(TzOffset, SYSDATETIMEOFFSET()))))=11 then SUM(CP.AmountPayed) ELSE 0 END ,
DIC=CASE WHEN MONTH(CONVERT(datetime,SWITCHOFFSET(CONVERT(datetimeoffset,ProcessedDateOnUtc),DATENAME(TzOffset, SYSDATETIMEOFFSET()))))=12 then SUM(CP.AmountPayed) ELSE 0 END 
INTO  #TEMP_1 --DROP TABLE  #TEMP_1
FROM 
CONTRIBUTIONPAYMENT CP
INNER JOIN CONTRIBUTION C ON C.ID=CP.CONTRIBUTIONID
WHERE YEAR(CONVERT(datetime,SWITCHOFFSET(CONVERT(datetimeoffset,ProcessedDateOnUtc),DATENAME(TzOffset, SYSDATETIMEOFFSET()))))<=@ToYear  AND
      YEAR(CONVERT(datetime,SWITCHOFFSET(CONVERT(datetimeoffset,ProcessedDateOnUtc),DATENAME(TzOffset, SYSDATETIMEOFFSET()))))>=@FromYear 
GROUP BY C.CUSTOMERID,ProcessedDateOnUtc
order by 1


IF(@TypeSource=1)
BEGIN
	DELETE FROM #TEMP_1 WHERE CustomerID IN (SELECT EntityId from GenericAttribute G WHERE G.KeyGroup='Customer' AND  G.[KEY]='MilitarySituationId' AND G.VALUE=2) --borro caja
END

IF(@TypeSource=2)
BEGIN
	DELETE FROM #TEMP_1 WHERE CustomerID IN (SELECT EntityId from GenericAttribute G WHERE G.KeyGroup='Customer' AND  G.[KEY]='MilitarySituationId' AND G.VALUE=1) --borro copere 
END


UPDATE #TEMP_1 SET TYPESOURCE='COPERE' WHERE CustomerID IN (SELECT EntityId from GenericAttribute G WHERE G.KeyGroup='Customer' AND  G.[KEY]='MilitarySituationId' AND G.VALUE=1) 
UPDATE #TEMP_1 SET TYPESOURCE='CPMP' WHERE CustomerID IN (SELECT EntityId from GenericAttribute G WHERE G.KeyGroup='Customer' AND  G.[KEY]='MilitarySituationId' AND G.VALUE=2) 
 

SELECT 
CustomerId,
CustomerName='                                              ',
CustomerLastName='                                              ',
CustomerAdmCode='                                ',
TypeSource,
SUM(ENE) AS Ene,SUM(FEB) AS Feb,SUM(MAR) AS Mar,SUM(ABR) AS Abr, SUM(MAY) AS May,SUM(JUN) AS Jun,SUM(JUL) AS Jul ,
SUM(AGO) AS Ago,SUM(SEP) AS Sep ,SUM(OCT) AS Oct ,SUM(NOV) AS Nov,SUM(DIC) AS Dic
INTO #TEMP_2
FROM #TEMP_1
GROUP BY CustomerId,TypeSource

Update #TEMP_2 set CustomerName=Value
FROM GenericAttribute WHERE KeyGroup='Customer' AND EntityId=#TEMP_2.CustomerId AND [Key]='FirstName'
Update #TEMP_2 set CustomerLastName=Value
FROM GenericAttribute WHERE KeyGroup='Customer' AND EntityId=#TEMP_2.CustomerId AND [Key]='LastName'
Update #TEMP_2 set CustomerAdmCode=Value
FROM GenericAttribute WHERE KeyGroup='Customer' AND EntityId=#TEMP_2.CustomerId AND [Key]='AdmCode'

DECLARE @newId uniqueidentifier =NEWID();
DECLARE @value XML=(SELECT * FROM  #TEMP_2 order by 1 desc FOR XML PATH ('ReportSummaryContribution'), root ('ArrayOfReportSummaryContribution'))

DELETE FROM Report WHERE source=@Source
INSERT INTO Report VALUES (@newId,@NameReport,@value,'',@ReportState,'',@Source,@newId,GETUTCDATE())
SELECT @TotalRecords = COUNT(1) FROM #TEMP_2
SELECT * FROM Report WHERE [key]=@newId

END
GO

CREATE  PROCEDURE [dbo].[ReportMilitarSituation]
(
	@MilitarSituation int,
	@LoanState int,
	@ContributionState int,
	@NameReport nvarchar(255),
	@ReportState int,
	@Source nvarchar(250),
	@TotalRecords int = null OUTPUT
)

AS
BEGIN

SELECT 
AdmCode='                                      ',
FirstName='                                    ',
LastName='                                    ',
CustomerId=Id,
MilitarSituationId=@MilitarSituation,
MilitarSituation='                                   '
INTO TEMP_MIL_SIT
FROM CUSTOMER 
WHERE ID IN ( 
SELECT DISTINCT EntityId FROM GenericAttribute where keygroup='Customer' and [Key]='MilitarySituationId' and Value=@MilitarSituation
)


Update TEMP_MIL_SIT set AdmCode=Value
FROM GenericAttribute WHERE KeyGroup='Customer' AND EntityId=TEMP_MIL_SIT.CustomerId AND [Key]='AdmCode'

Update TEMP_MIL_SIT set FirstName=Value
FROM GenericAttribute WHERE KeyGroup='Customer' AND EntityId=TEMP_MIL_SIT.CustomerId AND [Key]='FirstName'

Update TEMP_MIL_SIT set LastName=Value
FROM GenericAttribute WHERE KeyGroup='Customer' AND EntityId=TEMP_MIL_SIT.CustomerId AND [Key]='LastName'

DECLARE @SQL NVARCHAR(MAX)
 
IF(@ContributionState =-1)
BEGIN
	SET @SQL ='

		SELECT T_C.*, 
		ContributionAuthorizeDiscont =Contribution.AuthorizeDiscount, 
		ContributionAmountMeta=Contribution.AmountMeta,
		ContributionAmountPayed=Contribution.AmountPayed, 
		ContributionState=Contribution.Active
		INTO TEMP_MIL_SIT2
		FROM Contribution 
		INNER JOIN TEMP_MIL_SIT T_C on Contribution.CustomerId=T_C.CustomerId 	

	'
END
ELSE
BEGIN

	SET @SQL ='

	SELECT T_C.*, 
	ContributionAuthorizeDiscont =Contribution.AuthorizeDiscount, 
	ContributionAmountMeta=Contribution.AmountMeta,
	ContributionAmountPayed=Contribution.AmountPayed, 
	ContributionState=Contribution.Active
	INTO TEMP_MIL_SIT2
	FROM Contribution 
	INNER JOIN TEMP_MIL_SIT T_C on Contribution.CustomerId=T_C.CustomerId 	AND Contribution.Active='+CAST(@ContributionState AS nvarchar(2))

END

IF(@LoanState=-1)
BEGIN
	SET @SQL =@SQL +'

		SELECT T_L.* ,
		LoanNumber =Loan.LoanNumber, 
		LoanAmount=Loan.LoanAmount,
		LoanTotalAmount=Loan.TotalAmount,
		LoanTotalPayed=Loan.TotalPayed,
		LoanPeriod=Loan.Period,
		LoanState=Loan.Active
		INTO TEMP_MIL_SIT3
		FROM Loan 
		INNER JOIN TEMP_MIL_SIT2 T_L ON Loan.CustomerId=T_L.CustomerId 

	'
END
ELSE
BEGIN

	SET @SQL =@SQL+ '

	SELECT T_L.* ,
	LoanNumber =Loan.LoanNumber, 
	LoanAmount=Loan.LoanAmount,
	LoanTotalAmount=Loan.TotalAmount,
	LoanTotalPayed=Loan.TotalPayed,
	LoanPeriod=Loan.Period,
	LoanState=Loan.Active
	INTO TEMP_MIL_SIT3
	FROM Loan 
	INNER JOIN TEMP_MIL_SIT2 T_L ON Loan.CustomerId=T_L.CustomerId  	AND Loan.Active='+CAST(@LoanState AS nvarchar(2))

END

 
PRINT @SQL
EXECUTE sp_executesql @SQL


DECLARE @newId uniqueidentifier =NEWID();
DECLARE @value XML=(SELECT * FROM  TEMP_MIL_SIT3 order by 1 desc FOR XML PATH ('ReportMilitarSituation'), root ('ArrayOfReportMilitarSituation'))

DELETE FROM Report WHERE source=@Source
INSERT INTO Report VALUES (@newId,@NameReport,@value,'',@ReportState,'',@Source,@newId,GETUTCDATE())
SELECT @TotalRecords = COUNT(1) FROM TEMP_MIL_SIT3
SELECT * FROM Report WHERE [key]=@newId

  IF OBJECT_ID('dbo.TEMP_MIL_SIT', 'U') IS NOT NULL 
  DROP TABLE dbo.TEMP_MIL_SIT; 
  
  IF OBJECT_ID('dbo.TEMP_MIL_SIT2', 'U') IS NOT NULL 
  DROP TABLE dbo.TEMP_MIL_SIT2; 
  
  IF OBJECT_ID('dbo.TEMP_MIL_SIT3', 'U') IS NOT NULL 
  DROP TABLE dbo.TEMP_MIL_SIT3; 
 

END
GO

CREATE PROCEDURE [dbo].[ReportSummaryBankPayment]
(
	@FromYear int,
	@FromMonth int,
	@FromDay int,
	@ToYear int,
	@ToMonth int,
	@ToDay int,
	@Type int, -- 0 TODOS, 1 COPERE  , 2 CAJA
	@Data int, -- 0 TODOS,1 CONTRIBUTION, 2 LOAN
	@NameReport nvarchar(255),
	@ReportState int,
	@Source nvarchar(250),
	@TotalRecords int = null OUTPUT
)
AS
BEGIN

SELECT 
l.CustomerId as CustomerId,
FirstName='                                                               ', 
LastName='                                                                ',
AdmCode='                          ',
Dni='                                                          ',
MilitarySituation='                                                          ',
lp.TransactionNumber,
lp.ProcessedDateOnUtc,
lp.Reference,
lp.BankName,
sum(lp.MonthlyPayed) as AmountPayed
INTO  #Temp_Loan
FROM Loan l 
inner join LoanPayment lp on lp.loanId =l.id
WHERE  lp.StateId=7 and 
Year(lp.ProcessedDateOnUtc)*10000+Month(lp.ProcessedDateOnUtc)*100+day(lp.ProcessedDateOnUtc)>=(2016*10000+01*100+01) AND  
	  Year(lp.ProcessedDateOnUtc)*10000+Month(lp.ProcessedDateOnUtc)*100+day(lp.ProcessedDateOnUtc)<=(2017*10000+03*100+01) 

GROUP BY l.CustomerId,lp.TransactionNumber,lp.ProcessedDateOnUtc,lp.Reference,lp.BankName

Update #Temp_Loan set Dni=Value
FROM GenericAttribute WHERE KeyGroup='Customer' AND EntityId=#Temp_Loan.CustomerId AND [Key]='Dni'

Update #Temp_Loan set AdmCode=Value
FROM GenericAttribute WHERE KeyGroup='Customer' AND EntityId=#Temp_Loan.CustomerId AND [Key]='AdmCode'

Update #Temp_Loan set FirstName=Value
FROM GenericAttribute WHERE KeyGroup='Customer' AND EntityId=#Temp_Loan.CustomerId AND [Key]='FirstName'

Update #Temp_Loan set LastName=Value
FROM GenericAttribute WHERE KeyGroup='Customer' AND EntityId=#Temp_Loan.CustomerId AND [Key]='LastName'

UPDATE #Temp_Loan SET MilitarySituation='CPMP'  WHERE CustomerID IN (SELECT EntityId from GenericAttribute G WHERE G.KeyGroup='Customer' AND  G.[KEY]='MilitarySituationId' AND G.VALUE=2) --CAJA

UPDATE #Temp_Loan SET MilitarySituation='COPERE'  WHERE CustomerID IN (SELECT EntityId from GenericAttribute G WHERE G.KeyGroup='Customer' AND  G.[KEY]='MilitarySituationId' AND G.VALUE=1) --COPERE

select * from #Temp_Loan
DECLARE @newId uniqueidentifier =NEWID();
DECLARE @value XML=(SELECT * FROM  #Temp_Loan order by 1 desc FOR XML PATH ('ReportBankPayment'), root ('ArrayOfReportBankPayment'))

DELETE FROM Report WHERE source=@Source
INSERT INTO Report VALUES (@newId,@NameReport,@value,'',@ReportState,'',@Source,@newId,GETUTCDATE())
SELECT @TotalRecords = COUNT(1) FROM #Temp_Loan
SELECT * FROM Report WHERE [key]=@newId

END
GO