CREATE PROCEDURE [dbo].[SummaryMergeDetails]
(
	@Year int,
	@Month int,
	@Type int, -- 1 copere 2 caja
	@StateId int,
	@NameReport nvarchar(255),
	@ReportState int,
	@Source nvarchar(250),
	@TotalRecords int = null OUTPUT
)
AS
BEGIN

	SELECT DISTINCT EntityId into #t_Entity  FROM GenericAttribute where keygroup='Customer' and [Key]='MilitarySituationId' and [Value]= @Type

	Select * into #tmp_reports 
from (
	select 
		'ASE' AS Type,
		CASE WHEN stateid= 1 THEN Cast(stateid as nchar(2)) + ' - Pendiente' ELSE 
		CASE WHEN stateid= 2 THEN Cast(stateid as nchar(2)) + ' - EnProceso' ELSE 
		CASE WHEN stateid= 3 THEN Cast(stateid as nchar(2)) + ' - PagoParcial' ELSE 
		CASE WHEN stateid= 4 THEN Cast(stateid as nchar(2)) + ' - Pagado' ELSE 
		CASE WHEN stateid= 5 THEN Cast(stateid as nchar(2)) + ' - SinLiquidez' ELSE 
		CASE WHEN stateid= 6 THEN Cast(stateid as nchar(2)) + ' - Anulado' ELSE 
		CASE WHEN stateid= 7 THEN Cast(stateid as nchar(2)) + ' - PagoPersonal' ELSE 
		CASE WHEN stateid= 8 THEN Cast(stateid as nchar(2)) + ' - Devolucion' ELSE 
		CASE WHEN stateid= 9 THEN Cast(stateid as nchar(2)) + ' - Cancelado' ELSE 
		'' END END END END END END END END END  AS State,
		CAST(LP.Quota  as numeric) AS Number,
		cast(lp.MonthlyPayed as decimal(10,4)) as Payed  
	from LOAN L
	INNER JOIN LOANPAYMENT LP ON LP.LOANId = L.Id
	WHERE MONTH(LP.ScheduledDateOnUtc) = @Month and year(LP.ScheduledDateOnUtc) = @Year and l.CustomerId in (select EntityId from #t_Entity) and l.Active = 1
 
	UNION ALL

	select 
		'APO' AS Type,
		CASE WHEN stateid= 1 THEN Cast(stateid as nchar(2)) + ' - Pendiente' ELSE 
		CASE WHEN stateid= 2 THEN Cast(stateid as nchar(2)) + ' - EnProceso' ELSE 
		CASE WHEN stateid= 3 THEN Cast(stateid as nchar(2)) + ' - PagoParcial' ELSE 
		CASE WHEN stateid= 4 THEN Cast(stateid as nchar(2)) + ' - Pagado' ELSE 
		CASE WHEN stateid= 5 THEN Cast(stateid as nchar(2)) + ' - SinLiquidez' ELSE 
		CASE WHEN stateid= 6 THEN Cast(stateid as nchar(2)) + ' - Devolucion' ELSE 
		CASE WHEN stateid= 7 THEN Cast(stateid as nchar(2)) + ' - PagoPersonal' ELSE 
		'' END END END END END END END  AS State,
		cast(CP.Number as numeric) as Number,		
		cast(cp.AmountPayed as decimal(10,4)) as Payed
	from Contribution C
	INNER JOIN ContributionPayment CP ON CP.ContributionId = C.Id
	WHERE MONTH(CP.ScheduledDateOnUtc) = @Month and year(CP.ScheduledDateOnUtc) = @Year and c.CustomerId in (select EntityId from #t_Entity) and c.Active = 1
) as xx 

DECLARE @newId uniqueidentifier =NEWID();
DECLARE @value XML=(SELECT * FROM  #tmp_reports order by 1 desc FOR XML PATH ('ReportSummaryMerge'), root ('ArrayOfReportSummaryMerge'))
SELECT @TotalRecords = COUNT(1) FROM #tmp_reports

SELECT 0 as Id, @newId as [Key] ,@NameReport AS Name ,@value as Value,'' as  PathBase ,@ReportState as StateId,'' as Period ,@Source as Source,@newId as ParentKey,GETUTCDATE() as DateUtc

END

--DECLARE @TotalRecords int
--exec [dbo].[SummaryMergeDetails] 2023, 3,1,2,'ABC0',1,'SOURCE', @TotalRecords OUTPUT
 
