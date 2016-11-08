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
	WHERE  C.Id=1  
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


update ContributionPayment set stateid=1

select * from Contribution

select * from ContributionPayment

update ContributionPayment set IsAutomatic=1 where id=4
update Contribution set Active=1 where id=1


update ContributionPayment set StateId=3 where Number=5 and ContributionId=1


use acmr

select * from reports

Copere, 
Caja Pesnion militar Policial 

=> source

al copere se le envia :  aportaciones y  prestamos por separado pero en el mismo archivo
respuetsa del copere viene uno consolidado

caja pension se le envia :  aportaciones y  prestamos sumados 
respuesta : viene uno consolidado

SELECT * FROM ContributionPayment
		4610		3000		


me colgaste

ENE FEB MAR ABR
35  35   35    35
250 250 250  250 Y 25
		ENE  
		260   35 225
 SELECT * FROM LoanPayment  -- COLUMNA MAS DE DESCRIPCION 

 select * from Setting where Value='20'


 select * from Setting where name like '%contributionsettings%'

 SELECT EntityId 
 FROM GenericAttribute 
 WHERE KeyGroup='Customer' AND [Key]='MilitarySituationId' AND Value= 1

 SELECT * 
 FROM ContributionPayment CP
 INNER JOIN Contribution C ON C.Id=CP.ContributionId
 WHERE 
 C.CustomerId IN (SELECT EntityId 
 FROM GenericAttribute 
 WHERE KeyGroup='Customer' AND [Key]='MilitarySituationId' AND Value= 1) AND
 YEAR(CP.ScheduledDateOnUtc)=2016 AND  MONTH(CP.ScheduledDateOnUtc)=6 

SELECT * FROM GenericAttribute
 



	   select * from CustomerAttributeValue where CustomerAttributeId=4

use acmr 
select * from ScheduleBatch

 insert into ScheduleBatch values 
 ('Exportacion para caja','Ks.Batch.Contribution.Caja.Out','','','','',
 0,2016,11, null,null,null,1);

 delete ScheduleBatch where id=1

 SELECT GETUTCDATE()

 select * from GenericAttribute
 update ScheduleBatch 
 set 
 name='Exportacion para Caja',
 SystemName ='Ks.Batch.Contribution.Caja.Out',
  StartExecutionOnUtc=null, 
 NextExecutionOnUtc=null,
 LastExecutionOnUtc=null,
PathRead='hola',
PathLog='',
PathMoveToDone='',
PathMoveToError='',
 FrecuencyId=15 ,
 PeriodYear=0,
 PeriodMonth=0,
 Enabled=0
 where id>1

 
 update ScheduleBatch set enabled =0
 SELECT EntityId, Attribute =[Key], Value
 FROM GenericAttribute 
 WHERE KeyGroup='Customer' and  [Key] in ('Dni','AdmCode') AND
 EntityId IN ( SELECT EntityId FROM GenericAttribute WHERE [Key]='MilitarySituationId' AND Value=2)
 

 SELECT * FROM GenericAttribute

 select c.CustomerId, cp.AmountTotal 
 from ContributionPayment cp
 INNER JOIN  Contribution c on c.Id=cp.ContributionId
 WHERE c.CustomerId in (1) AND 
 YEAR(cp.ScheduledDateOnUtc)=2016 AND
 MONTH(cp.ScheduledDateOnUtc)=12 AND 
 DAY(CP.ScheduledDateOnUtc)=20

 select * from ContributionPayment

SELECT EntityId, Attribute =[Key], Value FROM GenericAttribute 
WHERE KeyGroup='Customer' and  [Key] in ('Dni','AdmCode') AND 
EntityId IN ( SELECT EntityId FROM GenericAttribute WHERE [Key]='MilitarySituationId' AND Value=2)

update ScheduleBatch set Enabled=1 where SystemName='Ks.Batch.Contribution.Caja.Out'

delete from ScheduleBatch where SystemName='Ks.Batch.Contribution.caja.Out'

use acmr
select * from ScheduleBatch
SELECT SystemName FROM ScheduleBatch WHERE SystemName='Ks.Batch.Contribution.Caja.Out'

SELECT EntityId, Attribute =[Key], Value FROM GenericAttribute
WHERE KeyGroup='Customer' and  [Key] in ('Dni','AdmCode') AND 
EntityId IN ( SELECT EntityId FROM GenericAttribute WHERE [Key]='MilitarySituationId' AND Value=2)

SELECT cp.Id 
FROM ContributionPayment cp  
INNER JOIN  Contribution c on c.Id=cp.ContributionId 
WHERE c.CustomerId IN (1) AND 
YEAR(cp.ScheduledDateOnUtc)=2016 AND 
MONTH(cp.ScheduledDateOnUtc)=12  

UPDATE ContributionPayment SET StateId =2 WHERE ID IN (
SELECT  cp.Id
FROM ContributionPayment cp  
INNER JOIN  Contribution c on c.Id=cp.ContributionId  
WHERE c.CustomerId IN (@CustomerId) AND  
YEAR(cp.ScheduledDateOnUtc)=@Year AND  
MONTH(cp.ScheduledDateOnUtc)=@Month   
)   --ESTADO PENDIENTE

SELECT * FROM ContributionPayment

update ContributionPayment set StateId=1 where id>1

 

select * from ScheduleBatch

update ScheduleBatch  Set  NextExecutionOnUtc=GETUTCDATE() where id=17

select StartExecutionOnUtc,
NextExecutionOnUtc,
LastExecutionOnUtc from ScheduleBatch

delete from ScheduleBatch where id=19

select GETUTCDATE(), GETDATE()

select * from Reports
1) CREA EL TXT
2) ACTUALIZA LOS CRONOGRAMAS DE APORTACOINES
3) CREA REPORTES
4) ACTUALIZA EL BACHERO

delete Reports