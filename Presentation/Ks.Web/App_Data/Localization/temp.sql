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

UPDATE ContributionPayment 
SET 
AmountPayed=#ContributionPaymentTmp5.AmountPayed,
ProcessedDateOnUtc=GETUTCDATE(), 
StateId=#ContributionPaymentTmp5.StateId,
BankName=#ContributionPaymentTmp5.BankName
FROM  #ContributionPaymentTmp5
WHERE #ContributionPaymentTmp5.ContributionPaymentId=ContributionPayment.Id 









END