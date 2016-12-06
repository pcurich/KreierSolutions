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
 
delete Reports

select * from Reports
select * from ScheduleBatch

update ScheduleBatch  Set  NextExecutionOnUtc=GETUTCDATE() where id=20

SELECT EntityId, Attribute =[Key], Value FROM GenericAttribute 
                  WHERE KeyGroup='Customer' and  [Key] in ('Dni','AdmCode') AND 
                  EntityId IN ( SELECT EntityId FROM GenericAttribute WHERE [Key]='MilitarySituationId' AND Value=2)

				  select * from CustomerAttribute where Name like'%Situacion%'
				  select * from CustomerAttributeValue where Name like'%retiro%'
				  select * from GenericAttribute
12345678910

use acmr
select * from schedulebatch
delete schedulebatch
UPDATE ScheduleBatch SET PathBase ='C:\KS\ACMR\WinService\Ks.Batch.Copere.Out' WHERE SystemName='Ks.Batch.Copere.Out'

select * from ContributionPayment
delete from Reports
update ContributionPayment set StateId=1, ProcessedDateOnUtc=null, BankName=null, Description=null

UPDATE ScheduleBatch set PeriodMonth=9 , PeriodYear=2016 WHERE SystemName='Ks.Batch.Copere.Out'
UPDATE ScheduleBatch set PeriodMonth=12 , PeriodYear=2016 WHERE SystemName='Ks.Batch.caja.Out'

update ScheduleBatch  Set  NextExecutionOnUtc=GETUTCDATE()  WHERE SystemName='Ks.Batch.Copere.Out'

select * from Reports where Source LIKE '%Ks.Batch.Caja.%' and Period=201706

SELECT EntityId, Attribute =[Key], Value FROM GenericAttribute 
                  WHERE KeyGroup='Customer' and  [Key] in ('Dni','AdmCode','FirstName','LastName') AND 
                  EntityId IN ( SELECT EntityId FROM GenericAttribute WHERE [Key]='MilitarySituationId' AND Value=2)

				  select * from GenericAttribute


select * from GenericAttribute where 
[Key] in ('FirstName','LastName') AND
KeyGroup='Customer' AND EntityId IN (1,4)
ORDER BY EntityId
 
select * from ScheduleBatch

update ScheduleBatch set NextExecutionOnUtc =GETUTCDATE() where SystemName like '%Ks.Batch.Copere.Out%'

select * from Reports where Source in ('Ks.Batch.Caja.Out','Ks.Batch.Caja.In')

select * from Reports where [key]='bf616db1-82a4-47a5-a4bd-964e93ff5742'
select * from Reports where Source like 'Ks.Batch.Copere%'

--delete Reports
select * from Reports where ParentKey in 
(select ParentKey  from Reports where StateId=2  and len(name)<>0  group by ParentKey having count(ParentKey)=2)
 

update Reports set StateId=2 where id in (64,65,66,68)

use acmr

UPDATE ContributionPayment  SET ProcessedDateOnUtc=@ProcessedDateOnUtc, 
StateId=@StateId, BankName='SystemName', Description='Lectura automatica por sistema ACMR' WHERE Id in 
(
	SELECT CP.Id 
	FROM ContributionPayment CP
	INNER JOIN Contribution C ON C.Id=CP.ContributionId
	WHERE C.CustomerId IN (4,7,8,9) AND 
	YEAR (CP.ScheduledDateOnUtc) =2016 and MONTH(CP.ScheduledDateOnUtc)=11
)

 
select * from Reports where ParentKey ='1231eb15-e825-4981-8b94-e8baf4a5a6c6' 
update Reports set Value = ''

select * from ScheduleBatch

update ScheduleBatch set Enabled=1, NextExecutionOnUtc =GETUTCDATE(), PeriodMonth=2, PeriodYear=2017 where id>1

select * from Contribution where CustomerId=9
select * from ContributionPayment where ContributionId in (select id from Contribution where CustomerId=9)
 
 ----------------------
 --PRUEBAS DEL MERGE


select * from Contribution 
where CustomerId in (SELECT ENTITYID FROM GenericAttribute WHERE [KEY]='AdmCode' and value in ('123456911'))

SELECT CP.* FROM Contribution C 
INNER JOIN ContributionPayment CP ON CP.ContributionId=c.Id
WHERE
C.CustomerId in (SELECT ENTITYID FROM GenericAttribute WHERE [KEY]='AdmCode' and value in ('123456911'))
AND YEAR(CP.ScheduledDateOnUtc)=2016 AND MONTH(CP.ScheduledDateOnUtc)=11
ORDER BY Number


---------------------------------
--CLEAN DATA
UPDATE Contribution SET IsDelay=0, DelayCycles=0, Active=1, UpdatedOnUtc=NULL, AmountPayed=0
UPDATE ContributionPayment SET  NumberOld =0, AmountOld =0, AmountPayed=0, AmountTotal=35, ProcessedDateOnUtc=null, StateId=1, IsAutomatic=1, BankName=null,Description=''
UPDATE Loan SET IsDelay=0,  Active=1, UpdatedOnUtc=NULL, TotalPayed=0
UPDATE LoanPayment SET  MonthlyPayed =0,  ProcessedDateOnUtc=null, StateId=1, IsAutomatic=1, BankName=null,AccountNumber='',Description=''
delete Report
update ScheduleBatch set PeriodMonth=12, PeriodYear=2016,  NextExecutionOnUtc =GETUTCDATE() where  SystemName IN ('Ks.Batch.Copere.Out','Ks.Batch.Copere.In')
update ScheduleBatch set PeriodMonth=12, PeriodYear=2016,  NextExecutionOnUtc =GETUTCDATE() where  SystemName IN ('Ks.Batch.Caja.In','Ks.Batch.Caja.Out')

--fuerza el quarts

update ScheduleBatch set   NextExecutionOnUtc =GETUTCDATE() where id>0
 


 select LastExecutionOnUtc from ScheduleBatch
SELECT * FROM ContributionPayment WHERE ID=424
SELECT * FROM ContributionPayment WHERE detailsold is not null 
SELECT * FROM ContributionPayment WHERE detailsnext is not null 

select* from Report

select * from ContributionPayment where ContributionId=2
update ContributionPayment set BankName='Copere' where AccountNumber is not null
 
 select  * from Contribution
 update Contribution set AmountPayed=11000  where id=10

 update loan set TotalPayed=0

 select * from Report

 select * from ContributionPayment
 select * from LoanPayment

 SELECT  c.CustomerId, cp.AmountTotal, cp.Amount1, cp.Amount2, cp.Amount3, cp.Number  
                      FROM ContributionPayment cp  
                      INNER JOIN  Contribution c on c.Id=cp.ContributionId  
                      WHERE c.CustomerId IN (  1 ) AND  
                      cp.StateId=@StateId AND  
                      YEAR(cp.ScheduledDateOnUtc)=@Year AND  
                      MONTH(cp.ScheduledDateOnUtc)=@Month  

 SELECT c.CustomerId, cp.Id, cp.Number,cp.NumberOld, c.Id,  
		cp.Amount1, cp.Amount2, cp.Amount3,cp.AmountOld, cp.AmountTotal, 
		cp.AmountTotal,cp.AmountPayed, cp.StateId,cp.IsAutomatic,Cp.BankName,cp.AccountNumber,
		cp.TransactionNumber, cp.Reference, cp.Description
FROM ContributionPayment cp  
                      INNER JOIN  Contribution c on c.Id=cp.ContributionId  
                      WHERE c.CustomerId IN (  1 )   
  

 SELECT 
 L.CustomerId as CustomerId,
 LP.Id as LoanPaymentId,
 L.Id as LoanId,
 LP.Quota as Quota,
 LP.MonthlyQuota as MonthlyQuota,
 LP.MonthlyFee as MonthlyFee,
 LP.MonthlyCapital as MonthlyCapital,
 LP.MonthlyPayed AS MonthlyPayed,
 LP.StateId as StateId,
 LP.IsAutomatic as IsAutomatic,
 LP.BankName as BankName,
 LP.AccountNumber as AccountNumber,
 LP.TransactionNumber as TransactionNumber,
 LP.Reference as Reference,
 LP.Description as Description
 FROM 
 LoanPayment LP 
 INNER JOIN Loan L on L.Id=lp.LoanId 
 WHERE 
 L.CustomerId in (1,2,3,4,2) and 
 LP.StateId=1 and
 YEAR(lp.ScheduledDateOnUtc)=1 and 
 MONTH(lp.ScheduledDateOnUtc)=25
 ORDER BY 1

 select   * from LoanPayment 
  select   * from Loan 
 update loan set totalPayed=217.78, Active=1 where id=1
 update LoanPayment Set MonthlyPayed= 3247.22 where id=103

 select sum(MonthlyQuota)-(600+217.78) from LoanPayment where LoanId= (select id from loan where LoanNumber=10013) and stateid=1
 select 600+217.78

 update LoanPayment set stateId=2 where Id=267
 delete from LoanPayment where id>=104

  SET LANGUAGE Spanish
--ESTO NO VA A FUNCIONAR. HAY Q REVISAR EL GROUP BY 
SELECT 
YEAR(LP.ProcessedDateOnUtc) as Year,
MONTH(LP.ProcessedDateOnUtc) as MONTH,
DATENAME(mm,LP.ProcessedDateOnUtc)  as MonthName, --DATENAME(mm,LP.ProcessedDateOnUtc)  as MonthName,
SUM(lp.MonthlyPayed) as MonthlyPayed ,
LP.StateId as StateId
INTO   #tmp_reports_t
FROM  LoanPayment LP
INNER JOIN Loan L ON L.Id=LP.LoanId
WHERE L.Id=9   AND LP.MonthlyPayed>0
GROUP BY lp.TransactionNumber, YEAR(lp.ProcessedDateOnUtc),MONTH(lp.ProcessedDateOnUtc), DATENAME(mm,LP.ProcessedDateOnUtc), LP.StateId
ORDER BY 1,2

SELECT Year,MonthName,MonthlyPayed,StateId
INTO  #tmp_reports 
FROM #tmp_reports_t

select * from #tmp_reports



 select SUM(MonthlyQuota) from LoanPayment where LoanId= (select id from loan where LoanNumber=10014) and stateid=1

 update LoanPayment set stateid=2 where id=247

 select * from loan where LoanNumber=10014
 select * from LoanPayment where LoanId = (select id from Loan where LoanNumber=10014)