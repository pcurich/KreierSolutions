use ACMR

SELECT * FROM setting

select * from  Customer
select * from CustomerAttribute
select * from CustomerAttributeValue
select * from GenericAttribute

select * from Contribution
select * from ContributionPayment

update Customer set Active=1


update ContributionPayment set Amount1=35, ProcessedDateOnUtc=GETUTCDATE(), Active=1 where id<15*12

UPDATE Contribution SET Active=0, UpdatedOnUtc=GETUTCDATE() WHERE ID=6

select * from loan

update ContributionPayment set a

SELECT * FROM Log
SELECT * FROM ActivityLog
SELECT * FROM ActivityLogType