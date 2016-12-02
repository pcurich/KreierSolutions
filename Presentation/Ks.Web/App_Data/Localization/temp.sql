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
	[IsAutomatic] int NOT NULL,
	[BankName] [nvarchar](500) NOT NULL,
	[AccountNumber] [nvarchar](500) NOT NULL,
	[TransactionNumber] [nvarchar](500) NOT NULL,
	[Reference] [nvarchar](500) NOT NULL,
	[Description] [nvarchar](500) NOT NULL
)
			
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
nref.value('IsAutomatic[1]', 'int'), --IsAutomatic	
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
StateId=#ContributionPaymentTmp4.StateId,
BankName=#ContributionPaymentTmp4.BankName
FROM  #ContributionPaymentTmp4
WHERE #ContributionPaymentTmp4.ContributionPaymentId=ContributionPayment.Id 

------------------------------------------------------------------------
--2) Update Data 5 {PagoParcial = 3, Pagado = 4, SinLiquidez=5}
------------------------------------------------------------------------

Declare @ValueOfQuota1 int= (select Value from Setting where Name  = 'contributionsettings.amount1')
Declare @ValueOfQuota2 int= (select Value from Setting where Name  = 'contributionsettings.amount2')
Declare @ValueOfQuota3 int= (select Value from Setting where Name  = 'contributionsettings.amount3')

UPDATE  ContributionPayment 
SET 
ContributionPayment.AmountTotal=ContributionPayment.AmountTotal+TMP.NoPayed,
contributionPayment.AmountOld=@ValueOfQuota1+@ValueOfQuota2+@ValueOfQuota3,
ContributionPayment.NumberOld=TMP.Number,
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

UPDATE Contribution set IsDelay=1  DelayCycles =DelayCycles+1
FROM  #ContributionPaymentTmp5 
WHERE #ContributionPaymentTmp5.ContributionId = Contribution.Id

------------------------------------------------------------------------
--2) Update Data 3 {PagoParcial = 3, Pagado = 4, SinLiquidez=5}
------------------------------------------------------------------------

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

EXEC FixQuota

END