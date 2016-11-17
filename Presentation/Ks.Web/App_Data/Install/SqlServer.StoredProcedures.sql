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

CREATE PROCEDURE FixQuota 
AS
BEGIN

	Declare @MaxToPay int= (select value from Setting where Name  =  'contributionsettings.maximumcharge')
	DECLARE @ContributionPaymentId INT
	DECLARE @NextId INT 
	DECLARE @OffSet decimal 
	DECLARE @LastDescription NVARCHAR(MAX)
	DECLARE @LastNumber INT
	DECLARE @ValueIndex INT =  0;
	

	DECLARE MY_CURSOR CURSOR 
	  LOCAL STATIC READ_ONLY FORWARD_ONLY
	FOR 
	SELECT id FROM ContributionPayment WHERE AmountTotal>070

	OPEN MY_CURSOR
	FETCH NEXT FROM MY_CURSOR INTO @ContributionPaymentId
	WHILE @@FETCH_STATUS = 0
	BEGIN 
		SET @ValueIndex=  0;
		WHILE @ValueIndex <= 10
		BEGIN
			SET @OffSet = (select AmountTotal-@MaxToPay from ContributionPayment WHERE Id=(@ContributionPaymentId+@ValueIndex))
			SET @LastDescription =(select ISNULL([DESCRIPTION],'')  from ContributionPayment WHERE Id=(@ContributionPaymentId+@ValueIndex))
			SET @LastNumber =(select Number from ContributionPayment WHERE Id=(@ContributionPaymentId+@ValueIndex))
			
			IF(@OffSet>0)
				BEGIN
					UPDATE ContributionPayment 
					SET AmountTotal=@MaxToPay						
					WHERE  Id=@ContributionPaymentId+@ValueIndex
					
					UPDATE ContributionPayment 
					SET AmountTotal=AmountTotal+@OffSet,
						AmountOld=@OffSet, 
						[DESCRIPTION]=ISNULL([DESCRIPTION],'')  + '|' + @LastDescription , 
						NumberOld =@LastNumber,
						DetailsNext=ISNULL(DetailsNext,'') + '|'+cast(@LastNumber as nvarchar(4))+'-'+ cast(@OffSet as nvarchar(10))						
					WHERE  Id=(@ContributionPaymentId+1+@ValueIndex)
				END
			SET @ValueIndex = @ValueIndex + 1;
		END	
		FETCH NEXT FROM MY_CURSOR INTO @ContributionPaymentId
	END
	CLOSE MY_CURSOR
	DEALLOCATE MY_CURSOR

END
 
GO

CREATE PROCEDURE [UpdateContributionPayment]
(
	@XmlPackage xml, 
	@Year int, 
	@Month int
)
AS
BEGIN
 
CREATE TABLE #ContributionPaymentTmp
			( 
				[Id] [int] NOT NULL,[ContributionId] [int] NOT NULL,[CustomerId] [int] NOT NULL,
				[Amount1] [decimal] NOT NULL,[Amount2] [decimal] NOT NULL,[Amount3] [decimal] NOT NULL,
				[AmountTotal] [decimal] NOT NULL,[AmountPayed] [decimal] NOT NULL,[StateId] [int] NOT NULL,
				[BankName] [nvarchar](500) NOT NULL,[Description] [nvarchar](1000) NOT NULL,
				[Number] [int] NOT NULL,[IsDelay] [int] NOT NULL,[CycleOfDelay] [int] NOT NULL
			)

 
INSERT INTO #ContributionPaymentTmp (Id,ContributionId,CustomerId, Amount1,Amount2,Amount3,AmountTotal,AmountPayed,StateId,BankName,[Description],Number,CycleOfDelay,IsDelay)
SELECT	0, --Id
		0, --ContributionId
		nref.value('CustomerId[1]', 'int'), --CustomerId
		nref.value('Amount1[1]', 'decimal'),--Amount1
		nref.value('Amount2[1]', 'decimal'),--Amount2
		nref.value('Amount3[1]', 'decimal'),--Amount3
		nref.value('AmountTotal[1]', 'decimal'),--AmountTotal
		nref.value('AmountPayed[1]', 'decimal'),--AmountPayed
		nref.value('StateId[1]', 'int'), --StateId
		nref.value('BankName[1]', 'nvarchar(500)'), --BankName
		nref.value('Description[1]', 'nvarchar(1000)'), --[Description]
		nref.value('Number[1]', 'nvarchar(1000)'), --[Number]
		0,0 --Delay thinks
		FROm @XmlPackage.nodes('//ArrayOfInfo/Info') AS R(nref)

------------------------------------------------------------------------
--1) Complete whith the ContributionId 
------------------------------------------------------------------------
UPDATE #ContributionPaymentTmp SET  ContributionId= Contribution.Id
FROM Contribution 
wHERE Contribution.CustomerId =#ContributionPaymentTmp.CustomerId AND Contribution.Active=1

------------------------------------------------------------------------ 
--2) Complete with the id of ContributionPayment
------------------------------------------------------------------------
UPDATE #ContributionPaymentTmp SET  Id= ContributionPayment.Id
FROM ContributionPayment 
wHERE ContributionPayment.ContributionId =#ContributionPaymentTmp.ContributionId AND 
	  YEAR(ContributionPayment.ScheduledDateOnUtc)=@Year and MONTH(ContributionPayment.ScheduledDateOnUtc)=@Month AND
	  ContributionPayment.StateId=2 --InProcess

------------------------------------------------------------------------
--                       UPDATE   #ContributionPaymentTmp
------------------------------------------------------------------------	  
------------------------------------------------------------------------
--3) Update the ContributionsPayments
------------------------------------------------------------------------
UPDATE ContributionPayment 
SET 
AmountPayed=#ContributionPaymentTmp.AmountPayed,
ProcessedDateOnUtc=GETUTCDATE(), 
StateId=#ContributionPaymentTmp.StateId 
FROM  #ContributionPaymentTmp
WHERE #ContributionPaymentTmp.Id=ContributionPayment.Id 

------------------------------------------------------------------------
--4) Update delay cycles and state
------------------------------------------------------------------------
Declare @TimeToDelay int= (select value from Setting where Name ='contributionsettings.cycleofdelay')
UPDATE #ContributionPaymentTmp 
SET CycleOfDelay=temp.CountState, 
IsDelay=temp.IsDelay
FROM 
(
	SELECT 
	ContributionId, 
	COUNT(StateId) as CountState,
	IsDelay = case when COUNT(StateId)>0 then 1 else 0 end,
	[Description]= case when COUNT(StateId)=@TimeToDelay then 'El aportante ha sido dado de baja debido a que ha pasado ' + CAST(@TimeToDelay as nvarchar(1)) + ' meses en estado de SIN LIQUIDEZ'  ELSE '' END
	FROM 
	ContributionPayment 
	WHERE ContributionPayment.StateId =5 -- sin liquidez
	GROUP BY ContributionId
) as temp 
WHERE 
temp.ContributionId=#ContributionPaymentTmp.ContributionId

------------------------------------------------------------------------
--5) Update the contribution (per customer)
------------------------------------------------------------------------
Update Contribution 
SET Contribution.AmountTotal=Contribution.AmountTotal+#ContributionPaymentTmp.AmountPayed,
	Contribution.CycleOfDelay=#ContributionPaymentTmp.CycleOfDelay,
	Contribution.Active = case when #ContributionPaymentTmp.CycleOfDelay=@TimeToDelay then 0 else 1 end,
	Contribution.IsDelay=#ContributionPaymentTmp.IsDelay,
	Contribution.UpdatedOnUtc=GETUTCDATE(),
	Contribution.[Description]= #ContributionPaymentTmp.[Description]
FROM #ContributionPaymentTmp 
WHERE #ContributionPaymentTmp.ContributionId=Contribution.Id
 

 ------------------------------------------------------------------------
 --			  	      SPLIT  5= SIN LIQUIDEZ  3=PAGO PARCIAL  
 ------------------------------------------------------------------------
 ------------------------------------------------------------------------
--6) Alter the next Quota to pay
------------------------------------------------------------------------
SELECT * INTO #ContributionPaymentTmp5 FROM  #ContributionPaymentTmp WHERE  StateId=5

 ------------------------------------------------------------------------
--7) Find the next quota an update the fields NumberOld and AmountOld
-- with data sin liquidez
------------------------------------------------------------------------
Declare @ValueOfQuota1 int= (select Value from Setting where Name  = 'contributionsettings.amount1')
Declare @ValueOfQuota2 int= (select Value from Setting where Name  = 'contributionsettings.amount2')
Declare @ValueOfQuota3 int= (select Value from Setting where Name  = 'contributionsettings.amount3')

UPDATE  ContributionPayment 
SET 
ContributionPayment.AmountTotal=ContributionPayment.AmountTotal+TMP.NoPayed,
contributionPayment.AmountOld=@ValueOfQuota1+@ValueOfQuota2+@ValueOfQuota3,
ContributionPayment.NumberOld=TMP.Number,
ContributionPayment.DetailsOld=ISNULL(ContributionPayment.DetailsOld,'') +'|'+cast(TMP.Number as nvarchar(4))+'-'+ cast((@ValueOfQuota1+@ValueOfQuota2+@ValueOfQuota3) as nvarchar(10)), 
ContributionPayment.[Description] ='Valor de la couta aumentado por el sistema ACMR debido a la falta de liquitdez de la cuota N° ' + CAST(TMP.Number as nvarchar(3))
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
	INNER JOIN #ContributionPaymentTmp5 CPT ON CPT.ContributionId=CP.ContributionId  --This is Unique
	GROUP BY CP.ContributionId , NextPayment.NumberNextQuota  ,	CPT.Number ,CPT.AmountTotal
) as TMP
WHERE --only the next quota to pay
ContributionPayment.ContributionId=TMP.ContributionId AND ContributionPayment.Number=TMP.NumberNextQuota


	------------------------------------------------------------------------
--8) Create table Parcial 3
------------------------------------------------------------------------

SELECT * INTO #ContributionPaymentTmp3 FROM  #ContributionPaymentTmp WHERE  StateId=3

UPDATE  ContributionPayment 
SET 
ContributionPayment.AmountTotal=ContributionPayment.AmountTotal+TMP.OffSet,
contributionPayment.AmountOld=TMP.OffSet,
ContributionPayment.NumberOld=TMP.Number, 
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
	INNER JOIN #ContributionPaymentTmp3 CPT ON CPT.ContributionId=CP.ContributionId  --This is Unique
	GROUP BY CP.ContributionId , NextPayment.NumberNextQuota ,CPT.Number ,CPT.AmountTotal-CPT.AmountPayed
) as TMP
WHERE --only the next quota to pay
ContributionPayment.ContributionId=TMP.ContributionId AND ContributionPayment.Number=TMP.NumberNextQuota

------------------------------------------------------------------------
--9) fIX The next quota
------------------------------------------------------------------------
EXEC FixQuota

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

CREATE PROC SearchAllTables
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
	@TotalRecordsDeleted int = null OUTPUT
)
AS
BEGIN

SELECT convert(int,ROW_NUMBER()  OVER(ORDER BY YEARS DESC))  AS Id, YEARS AS 'YEAR',IsAutomatic, StateId,
SUM(ENE) AS 'ENE',SUM(FEB) AS 'FEB',SUM(MAR) AS 'MAR',
SUM(ABR) AS 'ABR',SUM(MAY) AS 'MAY',SUM(JUN) AS 'JUN',
SUM(JUL) AS 'JUL',SUM(AGO) AS 'AGO',SUM(SEP) AS 'SEP',
SUM(OCT) AS 'OCT',SUM(NOV) AS 'NOV',SUM(DIC) AS 'DIC' 
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
	GROUP BY YEAR(CD.ScheduledDateOnUtc ), MONTH(CD.ScheduledDateOnUtc ),  CD.IsAutomatic, CD.StateId
) AS ReportContributionPayment
GROUP BY YEARS  ,IsAutomatic, StateId 

SELECT @TotalRecordsDeleted = COUNT(1) FROM #tmp_reports
SELECT * FROM  #tmp_reports order by 1 desc
END
GO