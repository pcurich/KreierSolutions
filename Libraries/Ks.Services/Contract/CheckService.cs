using System;
using System.Collections.Generic;
using System.Linq;
using Ks.Core;
using Ks.Core.Data;
using Ks.Core.Domain.Contract;

namespace Ks.Services.Contract
{
    public class CheckService : ICheckService
    {
        #region Fields

        private readonly IRepository<Check> _checkRepository;
        private readonly IRepository<Loan> _loanRepository;
        private readonly IRepository<ContributionBenefitBank> _contributionBenefitBankRepository;
        private readonly IRepository<ReturnPayment> _returnPaymentRepository;

        #endregion

        #region Constructor

        public CheckService(IRepository<Check> checkRepository, IRepository<Loan> loanRepository, IRepository<ContributionBenefitBank> contributionBenefitBankRepository, IRepository<ReturnPayment> returnPaymentRepository)
        {
            _checkRepository = checkRepository;
            _loanRepository = loanRepository;
            _contributionBenefitBankRepository = contributionBenefitBankRepository;
            _returnPaymentRepository = returnPaymentRepository;
        }

        #endregion

        #region Methods

        public virtual IPagedList<Check> SearchCheck(DateTime? @from = null, DateTime? to = null,
            string entityName = null, string bankName = null,
            string checkNumber = null, int pageIndex = 0, int pageSize = Int32.MaxValue)
        {
            var query = from c in _checkRepository.Table
                        select c;

            if (@from != null && to != null)
                query = from c in query
                        where c.CreatedOnUtc <= to && c.CreatedOnUtc >= @from
                        select c;

            if (entityName != null)
                query = from c in query
                        where c.EntityName == entityName
                        select c;

            if (bankName != null)
                query = from c in query
                        where c.BankName == bankName
                        select c;

            if (checkNumber != null)
                query = from c in query
                        where c.CheckNumber == checkNumber
                        select c;
            var result = query.ToList();


            if (entityName ==
                CommonHelper.GetKsCustomTypeConverter(typeof(ReturnPayment))
                    .ConvertToInvariantString(new ReturnPayment()))
            {
                #region internal

                var _query = from c in _returnPaymentRepository.Table
                             select c;


                if (!string.IsNullOrEmpty(bankName))
                    _query = from c in _query
                             where c.BankName == bankName
                             select c;

                if (!string.IsNullOrEmpty(checkNumber))
                    _query = from c in _query
                             where c.CheckNumber == checkNumber
                             select c;

                if (@from != null && to != null)
                    _query = from c in _query
                             where c.CreatedOnUtc <= to && c.CreatedOnUtc >= @from
                             select c;

                var _result = _query.ToList();
                result.AddRange(_result.Select(check => new Check
                {
                    AccountNumber = check.AccountNumber,
                    Amount = check.AmountToPay,
                    BankName = check.BankName,
                    EntityName = entityName,
                    EntityId = check.Id,
                    EntityTypeId = (int)EntityTypeValues.Return,
                    CheckNumber = check.CheckNumber,
                    Reason = "Se encuentra vigente",
                    CheckStateId = (int)CheckSatate.Active,
                    CreatedOnUtc = check.CreatedOnUtc,
                }));

                #endregion
            }

            if (entityName ==
                CommonHelper.GetKsCustomTypeConverter(typeof(ContributionBenefitBank))
                    .ConvertToInvariantString(new ContributionBenefitBank()))
            {
                #region internal

                var _query = from c in _contributionBenefitBankRepository.Table
                             select c;


                if (!string.IsNullOrEmpty(bankName))
                    _query = from c in _query
                             where c.BankName == bankName
                             select c;

                if (!string.IsNullOrEmpty(checkNumber))
                    _query = from c in _query
                             where c.CheckNumber == checkNumber
                             select c;

                if (@from != null && to != null)
                    _query = from c in _query
                             where c.ApprovedOnUtc <= to && c.ApprovedOnUtc >= @from
                             select c;

                var _result = _query.ToList();
                result.AddRange(_result.Select(check => new Check
                {
                    AccountNumber = check.AccountNumber,
                    Amount = check.AmountToPay,
                    BankName = check.BankName,
                    EntityName = entityName,
                    EntityId = check.Id,
                    EntityTypeId = (int)EntityTypeValues.Benefit,
                    CheckNumber = check.CheckNumber,
                    Reason = "Se encuentra vigente",
                    CheckStateId = (int)CheckSatate.Active,
                    CreatedOnUtc = check.ApprovedOnUtc ?? new DateTime(),
                }));

                #endregion
            }

            if (entityName ==
                CommonHelper.GetKsCustomTypeConverter(typeof(Loan))
                    .ConvertToInvariantString(new Loan()))
            {
                #region internal

                var _query = from c in _loanRepository.Table
                             select c;


                if (!string.IsNullOrEmpty(bankName))
                    _query = from c in _query
                             where c.BankName == bankName
                             select c;

                if (!string.IsNullOrEmpty(checkNumber))
                    _query = from c in _query
                             where c.CheckNumber == checkNumber
                             select c;

                if (@from != null && to != null)
                    _query = from c in _query
                             where c.ApprovalOnUtc <= to && c.ApprovalOnUtc >= @from
                             select c;

                var _result = _query.ToList();
                result.AddRange(_result.Select(check => new Check
                {
                    AccountNumber = check.AccountNumber,
                    Amount = check.TotalAmount,
                    BankName = check.BankName,
                    EntityName = entityName,
                    EntityId = check.Id,
                    EntityTypeId = (int)EntityTypeValues.Loan,
                    CheckNumber = check.CheckNumber,
                    Reason = "Se encuentra vigente",
                    CheckStateId = (int)CheckSatate.Active,
                    CreatedOnUtc = check.ApprovalOnUtc??new DateTime(),
                }));

                #endregion
            }

            return new PagedList<Check>(result, pageIndex, pageSize);
        }

        public virtual Check GetCheck(int checkId)
        {
            if (checkId == 0)
                return null;

            var query = _checkRepository.GetById(checkId);
            return query;
        }

        public virtual void Replace(Loan loan, Check check)
        {
            if (loan == null)
                throw new ArgumentNullException("loan");

            if (check == null)
                throw new ArgumentNullException("check");

            _loanRepository.Update(loan);
            _checkRepository.Insert(check);
        }

        public virtual void Replace(ReturnPayment returnPayment, Check check)
        {
            if (returnPayment == null)
                throw new ArgumentNullException("returnPayment");

            if (check == null)
                throw new ArgumentNullException("check");

            _returnPaymentRepository.Update(returnPayment);
            _checkRepository.Insert(check);
        }

        public virtual void Replace(ContributionBenefitBank contributionBenefitBank, Check check)
        {
            if (contributionBenefitBank == null)
                throw new ArgumentNullException("contributionBenefitBank");

            if (check == null)
                throw new ArgumentNullException("check");

            _contributionBenefitBankRepository.Update(contributionBenefitBank);
            _checkRepository.Insert(check);
        }

        public virtual IList<Check> GetChecks(DateTime? @from = null, DateTime? to = null, string entityName = null)
        {
            if (@from == null && to == null && entityName == null)
                return new List<Check>();

            var query = from c in _checkRepository.Table
                        select c;

            if (@from != null && to != null)
                query = from c in query
                        where (c.CreatedOnUtc >= @from && c.CreatedOnUtc <= to)
                        select c;


            if (entityName != null)
                query = from c in query
                        where (c.EntityName == entityName)
                        select c;

            var toList = query.ToList();
            return toList;
        }

        #endregion
    }
}