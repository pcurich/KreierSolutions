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

INSERT INTO #ContributionPaymentTmp			
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

select  * into #ContributionPaymentTmp3 from #ContributionPaymentTmp where stateId=3
select  * into #ContributionPaymentTmp4 from #ContributionPaymentTmp where stateId=4
select  * into #ContributionPaymentTmp5 from #ContributionPaymentTmp where stateId=5

------------------------------------------------------------------------
--2) Update Data 4 {PagoParcial = 3, Pagado = 4, SinLiquidez=5}
------------------------------------------------------------------------

UPDATE ContributionPayment 
SET 
AmountPayed=#ContributionPaymentTmp4.AmountPayed,
ProcessedDateOnUtc=GETUTCDATE(), 
StateId=4,
BankName=#ContributionPaymentTmp4.BankName,
AccountNumber  = cast(convert(NVARCHAR, getutcdate(), 112) +REPLACE(convert(NVARCHAR, getutcdate(), 114) ,':','') as nvarchar(50))
FROM  #ContributionPaymentTmp4
WHERE #ContributionPaymentTmp4.ContributionPaymentId=ContributionPayment.Id 

------------------------------------------------------------------------
--3) Update Data 5 {PagoParcial = 3, Pagado = 4, SinLiquidez=5}
------------------------------------------------------------------------

Declare @ValueOfQuota1 int= (select Value from Setting where Name  = 'contributionsettings.amount1')
Declare @ValueOfQuota2 int= (select Value from Setting where Name  = 'contributionsettings.amount2')
Declare @ValueOfQuota3 int= (select Value from Setting where Name  = 'contributionsettings.amount3')

UPDATE  ContributionPayment 
set 
ContributionPayment.StateId=5,
ContributionPayment.AmountPayed=#ContributionPaymentTmp5.AmountPayed,
ContributionPayment.ProcessedDateOnUtc=GETUTCDATE(),
ContributionPayment.BankName=#ContributionPaymentTmp5.BankName,
ContributionPayment.Description='No se ha enviado informacion de pagos en la interfaz del ' + #ContributionPaymentTmp5.BankName
FROM #ContributionPaymentTmp5 
WHERE #ContributionPaymentTmp5.ContributionPaymentId=ContributionPayment.Id

 

UPDATE  ContributionPayment 
SET 
ContributionPayment.AmountTotal=ContributionPayment.AmountTotal+TMP.NoPayed,
contributionPayment.AmountOld=@ValueOfQuota1+@ValueOfQuota2+@ValueOfQuota3,
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
	INNER JOIN #ContributionPaymentTmp5 CPT ON CPT.ContributionId=CP.ContributionId  --This is Unique
	GROUP BY CP.ContributionId , NextPayment.NumberNextQuota  ,	CPT.Number ,CPT.AmountTotal
) as TMP
WHERE --only the next quota to pay
ContributionPayment.ContributionId=TMP.ContributionId AND ContributionPayment.Number=TMP.NumberNextQuota

UPDATE Contribution set IsDelay=1 , DelayCycles =DelayCycles+1
FROM  #ContributionPaymentTmp5 
WHERE #ContributionPaymentTmp5.ContributionId = Contribution.Id

------------------------------------------------------------------------
--3) Update Data 3 {PagoParcial = 3, Pagado = 4, SinLiquidez=5}
------------------------------------------------------------------------

UPDATE  ContributionPayment 
set 
ContributionPayment.StateId=3,
ContributionPayment.AmountPayed=#ContributionPaymentTmp3.AmountPayed,
ContributionPayment.ProcessedDateOnUtc=GETUTCDATE(),
ContributionPayment.BankName=#ContributionPaymentTmp3.BankName,
ContributionPayment.Description='Pago parcial realizado del copere, el saldo se va a cargar en la siguiente couta con estado pendiente'
FROM #ContributionPaymentTmp3 
WHERE #ContributionPaymentTmp3 .ContributionPaymentId=ContributionPayment.Id

UPDATE  ContributionPayment 
SET 
ContributionPayment.AmountTotal=ContributionPayment.AmountTotal+TMP.OffSet,
contributionPayment.AmountOld=TMP.OffSet,
ContributionPayment.NumberOld=TMP.Number, 
ContributionPayment.StateId=3,
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
	INNER JOIN #ContributionPaymentTmp3 CPT ON CPT.ContributionId=CP.ContributionId  --This is Unique
	GROUP BY CP.ContributionId , NextPayment.NumberNextQuota ,CPT.Number ,CPT.AmountTotal-CPT.AmountPayed
) as TMP
WHERE --only the next quota to pay
ContributionPayment.ContributionId=TMP.ContributionId AND ContributionPayment.Number=TMP.NumberNextQuota

EXEC FixQuota

END
GO

CREATE PROCEDURE [UpdateLoanPayment]
(
	@XmlPackage xml, 
	@Year int, 
	@Month int
)
AS
BEGIN

CREATE TABLE #LoanPaymentTmp
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
insert into #LoanPaymentTmp					
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

select  * into #LoanPaymentTmp3 from #LoanPaymentTmp where stateId=3
select  * into #LoanPaymentTmp4 from #LoanPaymentTmp where stateId=4
select  * into #LoanPaymentTmp5 from #LoanPaymentTmp where stateId=5

------------------------------------------------------------------------
--2) Update Data 4 {PagoParcial = 3, Pagado = 4, SinLiquidez=5}
------------------------------------------------------------------------

UPDATE LoanPayment 
SET 
MonthlyPayed=#LoanPaymentTmp4.MonthlyPayed,
ProcessedDateOnUtc=GETUTCDATE(), 
StateId=4,
BankName=#LoanPaymentTmp4.BankName,
AccountNumber = cast(convert(NVARCHAR, getutcdate(), 112) +REPLACE(convert(NVARCHAR, getutcdate(), 114) ,':','') as nvarchar(50)),
Description='Cobro realizado correctamente por la interfaz de cobros de ACMR'
FROM  #LoanPaymentTmp4
WHERE #LoanPaymentTmp4.LoanPaymentId=LoanPayment.Id 

------------------------------------------------------------------------
--3) Update Data 5 {PagoParcial = 3, Pagado = 4, SinLiquidez=5}
------------------------------------------------------------------------

--Update LoanPayment
UPDATE  LoanPayment 
SET 
LoanPayment.StateId=5,
LoanPayment.AccountNumber = cast(convert(NVARCHAR, getutcdate(), 112) +REPLACE(convert(NVARCHAR, getutcdate(), 114) ,':','') as nvarchar(50)),
ProcessedDateOnUtc=GETUTCDATE(), 
BankName=#LoanPaymentTmp5.BankName
FROM #LoanPaymentTmp5 
WHERE #LoanPaymentTmp5.LoanPaymentId=LoanPayment.Id

--Insert New LoanPayment
SELECT LP.LoanId,(MAX(LP.Quota)+1 ) as Quota, MAX(LP.ScheduledDateOnUtc) AS ScheduledDateOnUtc  into         #tempMax
FROM  LoanPayment LP
INNER JOIN  #LoanPaymentTmp5 T ON T.LoanId=LP.LoanId
GROUP BY LP.LoanId

INSERT INTO LoanPayment  
select T.LoanId,T.Quota,L.MonthlyQuota,L.MonthlyFee,
L.MonthlyCapital,0, 1, L.IsAutomatic,'','', '','',
'Cuota Agregada por falta de liquidez en el pago del periodo ' +cast(2016 as nvarchar(4)) + cast(15 as nvarchar(2)),
DATEADD (month , 1 ,T.ScheduledDateOnUtc), null 
FROM  LoanPayment L
INNER JOIN  #tempMax T ON T.LoanId=L.LoanId
INNER JOIN #LoanPaymentTmp5 LT ON LT.LoanPaymentId=L.Id


------------------------------------------------------------------------
--3) Update Data 3 {PagoParcial = 3, Pagado = 4, SinLiquidez=5}
------------------------------------------------------------------------

--Update LoanPayment
UPDATE  LoanPayment 
SET 
LoanPayment.StateId=3,
LoanPayment.MonthlyPayed = #LoanPaymentTmp3.MonthlyPayed,
LoanPayment.BankName = #LoanPaymentTmp3.BankName,
LoanPayment.AccountNumber = cast(convert(NVARCHAR, getutcdate(), 112) +REPLACE(convert(NVARCHAR, getutcdate(), 114) ,':','') as nvarchar(50))
FROM #LoanPaymentTmp3 
WHERE #LoanPaymentTmp3.LoanPaymentId=LoanPayment.Id

--Insert New LoanPayment
SELECT LP.LoanId,(MAX(LP.Quota)+1 ) as Quota, MAX(LP.ScheduledDateOnUtc) AS ScheduledDateOnUtc  into     #tempMax2
FROM  LoanPayment LP
INNER JOIN  #LoanPaymentTmp3 T ON T.LoanId=LP.LoanId
GROUP BY LP.LoanId

INSERT INTO LoanPayment  
select   T.LoanId,T.Quota,L.MonthlyQuota-LT.MonthlyPayed,
L.MonthlyFee*(1-((L.MonthlyQuota-LT.MonthlyPayed)/L.MonthlyQuota)),
L.MonthlyCapital*(1-((L.MonthlyQuota-LT.MonthlyPayed)/L.MonthlyQuota)),
0, 1, L.IsAutomatic,'','', '','',
'Cuota Agregada por pago parcial en el pago del periodo ' +cast(2016 as nvarchar(4)) + cast(15 as nvarchar(2)),
DATEADD (month , 1 ,T.ScheduledDateOnUtc)  , null 
FROM  LoanPayment L
INNER JOIN  #tempMax2 T ON T.LoanId=L.LoanId
INNER JOIN #LoanPaymentTmp3 LT ON LT.LoanPaymentId=L.Id

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