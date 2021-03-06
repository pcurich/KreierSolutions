USE [ACMR]
GO
/****** Object:  StoredProcedure [dbo].[TestContributionPayment]    Script Date: 15/01/2017 12:58:59 a.m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[TestContributionPayment]
(
	@CustomerId INT,
	@StateId INT, --4 ES PAGADO, 7PagoPersonal
	@BankName NVARCHAR(255), --COPERE 
	@AccountNumber NVARCHAR(255),--COPERE
	@TransactionNumber NVARCHAR(255),
	@Reference NVARCHAR(255)
)
AS
BEGIN

DECLARE @IsAutomatic INT =1

IF(@StateId=7)
BEGIN
	SET @IsAutomatic=0
END


IF(@BankName='Copere' )
BEGIN
SET @AccountNumber='Copere'
END

IF(@BankName='Caja' )
BEGIN
SET @AccountNumber='Caja'
END


Update ContributionPayment set AmountPayed=AmountTotal ,StateId=@StateId, ProcessedDateOnUtc=ScheduledDateOnUtc,IsAutomatic=@IsAutomatic,
BankName=@BankName , AccountNumber=@AccountNumber, TransactionNumber=@TransactionNumber,Reference=@Reference
where Id in (
SELECT   top 1 id  FROM ContributionPayment
WHERE ContributionId  IN (SELECT Id FROM Contribution WHERE CustomerId=@CustomerId) and StateId=1
order by number asc)

UPDATE Contribution SET AmountPayed= (select SUM(AmountPayed) from ContributionPayment WHERE ContributionId IN (SELECT Id FROM Contribution WHERE CustomerId=@CustomerId) )
WHERE CustomerId=@CustomerId
 

UPDATE Contribution SET UpdatedOnUtc= (select Max(ProcessedDateOnUtc) from ContributionPayment WHERE ProcessedDateOnUtc is not null and  ContributionId IN (SELECT Id FROM Contribution WHERE CustomerId=@CustomerId) )
WHERE CustomerId=@CustomerId 


END