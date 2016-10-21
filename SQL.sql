use ACMR

SELECT * FROM setting

select * from  Customer
select * from CustomerAttribute
select * from CustomerAttributeValue
select * from GenericAttribute

select * from Contribution
select * from ContributionPayment

update Customer set Active=1

SELECT * FROM ContributionPayment WHERE ContributionId=2
update ContributionPayment set Amount1=35, ProcessedDateOnUtc=GETUTCDATE(), StateId=1 where id>= (421)AND id<421+ (15*12)  AND ContributionId=2

UPDATE Contribution SET Active=0, UpdatedOnUtc=GETUTCDATE() WHERE ID=1

select * from loan

update ContributionPayment set a

SELECT * FROM Log
SELECT * FROM ActivityLog
SELECT * FROM ActivityLogType


 SELECT YEARS AS 'YEAR',IsAutomatic, StateId,
SUM(ENE) AS 'ENE',SUM(FEB) AS 'FEB',SUM(MAR) AS 'MAR',
SUM(ABR) AS 'ABR',SUM(MAY) AS 'MAY',SUM(JUN) AS 'JUN',
SUM(JUL) AS 'JUL',SUM(AGO) AS 'AGO',SUM(SEP) AS 'SEP',
SUM(OCT) AS 'OCT',SUM(NOV) AS 'NOV',SUM(DIC) AS 'DIC' 
FROM 
(
	SELECT CD.IsAutomatic, CD.StateId, YEAR(CD.ScheduledDateOnUtc ) AS 'YEARS', 
	'ENE' = CASE WHEN MONTH(CD.ScheduledDateOnUtc )=1 AND CD.StateId<>1 THEN SUM(CD.Amount1+CD.Amount2+CD.Amount3) ELSE 0 END ,
	'FEB' = CASE WHEN MONTH(CD.ScheduledDateOnUtc )=2 AND CD.StateId<>1 THEN SUM(CD.Amount1+CD.Amount2+CD.Amount3) ELSE 0 END ,
	'MAR' = CASE WHEN MONTH(CD.ScheduledDateOnUtc )=3 AND CD.StateId<>1 THEN SUM(CD.Amount1+CD.Amount2+CD.Amount3) ELSE 0 END ,
	'ABR' = CASE WHEN MONTH(CD.ScheduledDateOnUtc )=4 AND CD.StateId<>1 THEN SUM(CD.Amount1+CD.Amount2+CD.Amount3) ELSE 0 END ,
	'MAY' = CASE WHEN MONTH(CD.ScheduledDateOnUtc )=5 AND CD.StateId<>1 THEN SUM(CD.Amount1+CD.Amount2+CD.Amount3) ELSE 0 END ,
	'JUN' = CASE WHEN MONTH(CD.ScheduledDateOnUtc )=6 AND CD.StateId<>1 THEN SUM(CD.Amount1+CD.Amount2+CD.Amount3) ELSE 0 END ,
	'JUL' = CASE WHEN MONTH(CD.ScheduledDateOnUtc )=7 AND CD.StateId<>1 THEN SUM(CD.Amount1+CD.Amount2+CD.Amount3) ELSE 0 END ,
	'AGO' = CASE WHEN MONTH(CD.ScheduledDateOnUtc )=8 AND CD.StateId<>1 THEN SUM(CD.Amount1+CD.Amount2+CD.Amount3) ELSE 0 END ,
	'SEP' = CASE WHEN MONTH(CD.ScheduledDateOnUtc )=9 AND CD.StateId<>1 THEN SUM(CD.Amount1+CD.Amount2+CD.Amount3) ELSE 0 END ,
	'OCT' = CASE WHEN MONTH(CD.ScheduledDateOnUtc )=10 AND CD.StateId<>1 THEN SUM(CD.Amount1+CD.Amount2+CD.Amount3) ELSE 0 END ,
	'NOV' = CASE WHEN MONTH(CD.ScheduledDateOnUtc )=11 AND CD.StateId<>1 THEN SUM(CD.Amount1+CD.Amount2+CD.Amount3) ELSE 0 END ,
	'DIC' = CASE WHEN MONTH(CD.ScheduledDateOnUtc )=12 AND CD.StateId<>1 THEN SUM(CD.Amount1+CD.Amount2+CD.Amount3) ELSE 0 END , 
	SUM(CD.Amount1+CD.Amount2+CD.Amount3) AS TOTAL 
	FROM contributionPayment  CD
	INNER JOIN Contribution C ON C.Id=CD.contributionId
	WHERE C.Active=1 AND C.CustomerId=1  
	GROUP BY YEAR(CD.ScheduledDateOnUtc ), MONTH(CD.ScheduledDateOnUtc ), CD.Number,CD.IsAutomatic, CD.StateId
) AS ReportContributionPayment
GROUP BY YEARS  ,IsAutomatic, StateId,TOTAL
 --FOR XML PATH('ArrayOfReportPayment') , ELEMENTS XSINIL  
--FOR XML PATH('ArrayOfReportPayment') 

--select  *  from ContributionPayment where ContributionId=2

--update ContributionPayment set BankName ='BBVA Continental' , AccountNumber='1111111111', TransactionNumber='ffdvbrdef', Reference='gvbhgf'
--where ProcessedDateOnUtc is not null

--update ContributionPayment set StateId=4
--where BankName='BBVA Continental'