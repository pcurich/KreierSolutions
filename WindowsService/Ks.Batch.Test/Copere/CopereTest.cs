using Ks.Batch.Util.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Ks.Batch.Test;
using Ks.Batch.Copere.In;

namespace Ks.Batch.Copere.Test
{
    [TestClass]
    public class CopereTest : BaseTest
    {
        public const int YEAR = 2019;
        public const int MONTH = 9;

        public List<Info> dataBase;

        public void Init()
        {
            dataBase = new List<Info>()
            {
                //Solo aportaciones
                new Info
                {
                    Year = YEAR,Month=MONTH, HasAdminCode = true, AdminCode = "115851100", HasDni = true,Dni= "08760679", CustomerId = 115851100,
                    TotalContribution=35.40M,TotalPayed = 0, TotalLoan=0M,
                    InfoContribution = new InfoContribution { ContributionPaymentId= 115851100, Amount1 = 34.85M, Amount2=0.55M,Amount3=0M, AmountOld=0M, AmountTotal=35.40M, AmountPayed=0M, StateId=1}
                },
                //Aportacion + 1 Apoyo
                new Info
                {
                    Year = YEAR,Month=MONTH, HasAdminCode = true, AdminCode = "115851101", HasDni = true,Dni= "20108420", CustomerId = 115851101,
                    TotalContribution=35.40M,TotalPayed = 0, TotalLoan=327.78M,
                    InfoContribution = new InfoContribution {ContributionPaymentId= 115851101, Amount1 = 34.85M, Amount2=0.55M,Amount3=0M, AmountOld=0M, AmountTotal=35.40M, AmountPayed=0M, StateId=1},
                    InfoLoans = new List<InfoLoan>(){
                        new InfoLoan(){LoanId=115851101,LoanPaymentId = 115851101,MonthlyQuota = 327.78M,Quota = 1, Period =12}
                    }
                },
                //Aportacion + 2 Apoyo
                new Info
                {
                    Year = YEAR,Month=MONTH, HasAdminCode = true, AdminCode = "115851102", HasDni = true,Dni= "20108420", CustomerId = 115851102,
                    TotalContribution=35.40M,TotalPayed = 0, TotalLoan=384.1M,
                    InfoContribution = new InfoContribution {ContributionPaymentId= 115851102, Amount1 = 34.85M, Amount2=0.55M,Amount3=0M, AmountOld=0M, AmountTotal=35.40M, AmountPayed=0M, StateId=1},
                    InfoLoans = new List<InfoLoan>(){
                        new InfoLoan(){LoanId=115851102,LoanPaymentId = 115851102,  MonthlyQuota = 327.78M,Quota = 1, Period =12},
                        new InfoLoan(){LoanId=115851102,LoanPaymentId = 115851103,MonthlyQuota = 56.32M,Quota = 1, Period =12}
                    }
                }
            };
        }

        public IDictionary<string, object> Process(string i)
        {
            List<Info> dataFileASE = InfoService.ReadFile("../../../Files/Copere/ASE_CP_COPERE_" + int.Parse(i) + ".txt", "es-PE", false, true);
            List<Info> dataFileAPR = InfoService.ReadFile("../../../Files/Copere/APR_CP_COPERE_" + int.Parse(i) + ".txt", "es-PE", true, false);
            dataFileASE.AddRange(dataFileAPR);
            var dataFile = dataFileASE;

            var report = new Report() { ParentKey = Guid.NewGuid() };
            var dao = new Merge.Dao(null);

            dataFile = dao.JoinData(dataFile);
            var result = dao.Process(report, dataFile, dataBase, "Test-Bank", "Account");

            return result;
        }


        /// <summary>
        ///---------------------------------------------------------------------------------
        ///|     Contribution  |       Loan0       |       Loan1       |       Loan2       |
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   Total   |   tCrtb   |   tLoan   |        Crtb        |        Loan        |   Return 
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|  35.40 |    NA    |                                                                                               | Sin Liquidez = 1                        | 
        /// </summary>
        [TestMethod]
        public void CP_COPERE_01()
        {
            Init();
            var result = Process("01");

            var cp01 = (List<InfoContribution>)result["infoContributionNoCash"];
            Contribution(cp01, size: 1, contributionPaymentId: 115851100, amountPayed: 0, stateId: (int)ContributionState.SinLiquidez);
        }

        /// <summary>
        ///---------------------------------------------------------------------------------
        ///|     Contribution  |       Loan0       |       Loan1       |       Loan2       |
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   Total   |   tCrtb   |   tLoan   |        Crtb        |        Loan        |   Return 
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|  35.40 |  10.00   |        |          |        |          |        |          |    10.00  |    10.00  |    0.00   | Pago Parcial = 1   |                    | 
        /// </summary>
        [TestMethod]
        public void CP_COPERE_02()
        {
            Init();
            var result = Process("02");

            var cp02 = (List<InfoContribution>)result["infoContributionIncomplete"];

            Contribution(cp02, size: 1, contributionPaymentId: 115851100, amountPayed: 10M, stateId: (int)ContributionState.PagoParcial);
        }

        /// <summary>
        ///---------------------------------------------------------------------------------
        ///|     Contribution  |       Loan0       |       Loan1       |       Loan2       |
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   Total   |   tCrtb   |   tLoan   |        Crtb        |        Loan        |   Return 
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|  35.40 |  35.40   |        |          |        |          |        |          |    35.40  |   35.40   |    0.00   | Pago Total = 1     |                    | 
        /// </summary>
        [TestMethod]
        public void CP_COPERE_03()
        {
            Init();
            var result = Process("03");

            var cp03 = (List<InfoContribution>)result["infoContributionPayedComplete"];

            Contribution(cp03, size: 1, contributionPaymentId: 115851100, amountPayed: 35.40M, stateId: (int)ContributionState.Pagado);
        }

        /// <summary>
        ///---------------------------------------------------------------------------------
        ///|     Contribution  |       Loan0       |       Loan1       |       Loan2       |
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   Total   |   tCrtb   |   tLoan   |        Crtb        |        Loan        |   Return 
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|  35.40 |  50.00   |        |          |        |          |        |          |   50.00   |  35.40    |    0.00   | Pago Total = 1     |                    | Devolucion de 14.60
        /// </summary>
        [TestMethod]
        public void CP_COPERE_04()
        {
            Init();
            var result = Process("04");

            var cp04C = (List<InfoContribution>)result["infoContributionPayedComplete"];
            var cp04R = (List<Info>)result["infoReturnPayment"];

            Contribution(cp04C, size: 1, contributionPaymentId: 115851100, amountPayed: 35.40M, stateId: (int)ContributionState.Pagado);
            ReturnPayment(cp04R, customerId: 115851100, totalPayed: 14.60M);

        }

        /// <summary>
        ///---------------------------------------------------------------------------------
        ///|     Contribution  |       Loan0       |       Loan1       |       Loan2       |
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   Total   |   tCrtb   |   tLoan   |        Crtb        |        Loan        |   Return 
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|  35.40 |  10.00   |                   | 327.78 |   0.00   |        |          |    10.00  |    10.00  |    0.00   | Pago Parcial = 1   | Sin Liquidez = 1 & Cuota Adicional =1  (327.78)
        /// </summary>
        [TestMethod]
        public void CP_COPERE_05()
        {
            Init();
            var result = Process("05");

            var cp05C = (List<InfoContribution>)result["infoContributionIncomplete"];
            var cp05L = (List<InfoLoan>)result["infoLoanNoCash"];
            var cp05NQ = (List<InfoLoan>)result["infoLoanNextQuota"];

            Contribution(cp05C, size: 1, contributionPaymentId: 115851101, amountPayed: 10.00M, stateId: (int)ContributionState.PagoParcial);
            Loan(cp05L, loanId: 115851101, size: 1, monthlyQuota: 327.78M, monthlyPayed: 0M, stateId: (int)LoanState.SinLiquidez);
            NextPayment(cp05NQ, cp05L, loanId: 115851101, size: 1, monthlyQuota: 327.78M);
        }

        /// <summary>
        ///---------------------------------------------------------------------------------
        ///|     Contribution  |       Loan0       |       Loan1       |       Loan2       |
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   Total   |   tCrtb   |   tLoan   |        Crtb        |        Loan        |   Return 
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|  35.40 |  10.00   |                   | 327.78 |  50.00   |                   |    60.00  |   10.00   |   50.00   | Pago Parcial = 1     | Pago Parcial = 1 & Cuota Adicional =1 (277.78)
        /// </summary>
        [TestMethod]
        public void CP_COPERE_06()
        {
            Init();
            var result = Process("06");

            var cp06C = (List<InfoContribution>)result["infoContributionIncomplete"];
            var cp06L = (List<InfoLoan>)result["infoLoanIncomplete"];
            var cp06NQ = (List<InfoLoan>)result["infoLoanNextQuota"];

            Contribution(cp06C, size: 1, contributionPaymentId: 115851101, amountPayed: 10.00M, stateId: (int)ContributionState.PagoParcial);
            Loan(cp06L, loanId: 115851101, size: 1, monthlyQuota: 327.78M, monthlyPayed: 50.00M, stateId: (int)LoanState.PagoParcial);
            NextPayment(cp06NQ, cp06L, loanId: 115851101, size: 1, monthlyQuota: 277.78M);
        }

        /// <summary>
        ///---------------------------------------------------------------------------------
        ///|     Contribution  |       Loan0       |       Loan1       |       Loan2       |
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   Total   |   tCrtb   |   tLoan   |        Crtb        |        Loan        |   Return 
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|  35.40 |  10.00   |                   | 327.78 | 327.78   |                   |   337.78  |   10.00   |  327.78   | Pago parcial = 1   | Pago total = 1 
        /// </summary>
        [TestMethod]
        public void CP_COPERE_07()
        {
            Init();
            var result = Process("07");

            var cp07C = (List<InfoContribution>)result["infoContributionIncomplete"];
            var cp07L = (List<InfoLoan>)result["infoLoanPayedComplete"];

            Contribution(cp07C, size: 1, contributionPaymentId: 115851101, amountPayed: 10.00M, stateId: (int)ContributionState.PagoParcial);
            Loan(cp07L, loanId: 115851101, size: 1, monthlyQuota: 327.78M, monthlyPayed: 327.78M, stateId: (int)LoanState.Pagado);
            
        }

        /// <summary>
        ///---------------------------------------------------------------------------------
        ///|     Contribution  |       Loan0       |       Loan1       |       Loan2       |
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   Total   |   tCrtb   |   tLoan   |        Crtb        |        Loan        |   Return 
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|  35.40 |  10.00   |                   | 327.78 | 500.00   |                   |   510.00  |   10.00   |  327.78   | Pago Parcial = 1   | Pago total = 1     | Devolucion = 1 (172.22)
        /// </summary>
        [TestMethod]
        public void CP_COPERE_08()
        {
            Init();
            var result = Process("08");

            var cp08C = (List<InfoContribution>)result["infoContributionIncomplete"];
            var cp08L = (List<InfoLoan>)result["infoLoanPayedComplete"];
            var cp08DV = (List<Info>)result["infoReturnPayment"];

            Contribution(cp08C, size: 1, contributionPaymentId: 115851101, amountPayed: 10.00M, stateId: (int)ContributionState.PagoParcial);
            Loan(cp08L, loanId: 115851101, size: 1, monthlyQuota: 327.78M, monthlyPayed: 327.78M, stateId: (int)LoanState.Pagado);
            ReturnPayment(cp08DV, customerId: 115851101, totalPayed: 172.22M);
        }

        /// <summary>
        ///---------------------------------------------------------------------------------
        ///|     Contribution  |       Loan0       |       Loan1       |       Loan2       |
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   Total   |   tCrtb   |   tLoan   |        Crtb        |        Loan                         |   Return 
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|  35.40 |  35.40   |                   | 327.78 |   0.00   |                   |   35.40   |    35.40  |    0.00   | Pago total = 1     | Sin Liquidez = 1 & Cuota Extra = 1  | 
        /// </summary>
        [TestMethod]
        public void CP_COPERE_09()
        {
            Init();
            var result = Process("09");

            var cp09C = (List<InfoContribution>)result["infoContributionPayedComplete"];
            var cp09L = (List<InfoLoan>)result["infoLoanNoCash"];
            var cp09NQ = (List<InfoLoan>)result["infoLoanNextQuota"];

            Contribution(cp09C, size: 1, contributionPaymentId: 115851101, amountPayed: 35.40M, stateId: (int)ContributionState.Pagado);
            Loan(cp09L, loanId: 115851101, size: 1, monthlyQuota: 327.78M, monthlyPayed: 0M, stateId: (int)LoanState.SinLiquidez);
            NextPayment(cp09NQ, cp09L, loanId: 115851101, size: 1, monthlyQuota: 327.78M);
        }

        /// <summary>
        ///---------------------------------------------------------------------------------
        ///|     Contribution  |       Loan0       |       Loan1       |       Loan2       |
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   Total   |   tCrtb   |   tLoan   |        Crtb        |        Loan                         |   Return 
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|  35.40 |  35.40   |                   | 327.78 |  50.00   |                   |   84.85   |    35.40  |   50.00   | Pago total = 1     | Pago Parcial = 1 & Cuota Extra = 1 (277.78)  | 
        /// </summary>
        [TestMethod]
        public void CP_COPERE_10()
        {
            Init();
            var result = Process("10");

            var cp10C = (List<InfoContribution>)result["infoContributionPayedComplete"];
            var cp10L = (List<InfoLoan>)result["infoLoanIncomplete"];
            var cp10NQ = (List<InfoLoan>)result["infoLoanNextQuota"];

            Contribution(cp10C, size: 1, contributionPaymentId: 115851101, amountPayed: 35.40M, stateId: (int)ContributionState.Pagado);
            Loan(cp10L, loanId: 115851101, size: 1, monthlyQuota: 327.78M, monthlyPayed: 50.00M, stateId: (int)LoanState.PagoParcial);
            NextPayment(cp10NQ, cp10L, loanId: 115851101, size: 1, monthlyQuota: 277.78M);

        }

        /// <summary>
        ///---------------------------------------------------------------------------------
        ///|     Contribution  |       Loan0       |       Loan1       |       Loan2       |
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   Total   |   tCrtb   |   tLoan   |        Crtb        |        Loan                         |   Return 
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|  35.40 |  35.40   |                   | 327.78 |  327.78  |                   |   362.63  |   35.40   |  327.78   | Pago total = 1     | Pago Total = 1                      | 
        /// </summary>
        [TestMethod]
        public void CP_COPERE_11()
        {
            Init();
            var result = Process("11");

            var cp11C = (List<InfoContribution>)result["infoContributionPayedComplete"];
            var cp11L = (List<InfoLoan>)result["infoLoanPayedComplete"];

            Contribution(cp11C, size: 1, contributionPaymentId: 115851101, amountPayed: 35.40M, stateId: (int)ContributionState.Pagado);
            Loan(cp11L, loanId: 115851101, size: 1, monthlyQuota: 327.78M, monthlyPayed: 327.78M, stateId: (int)LoanState.Pagado);
        }

        /// <summary>
        ///---------------------------------------------------------------------------------
        ///|     Contribution  |       Loan0       |       Loan1       |       Loan2       |
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   Total   |   tCrtb   |   tLoan   |        Crtb        |        Loan                         |   Return 
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|  35.40 |  35.40   |                   | 327.78 |  500.00  |                   |   535.40  |   35.40   |  327.78   | Pago total = 1     | Pago Total = 1                      | Retorno por 172.22
        /// </summary>
        [TestMethod]
        public void CP_COPERE_12()
        {
            Init();
            var result = Process("12");

            var cp12C = (List<InfoContribution>)result["infoContributionPayedComplete"];
            var cp12L = (List<InfoLoan>)result["infoLoanPayedComplete"];
            var cp12DV = (List<Info>)result["infoReturnPayment"];

            Contribution(cp12C, size: 1, contributionPaymentId: 115851101, amountPayed: 35.40M, stateId: (int)ContributionState.Pagado);
            Loan(cp12L, loanId: 115851101, size: 1, monthlyQuota: 327.78M, monthlyPayed: 327.78M, stateId: (int)LoanState.Pagado);
            ReturnPayment(cp12DV, customerId: 115851101, totalPayed: 172.22M);

        }

        /// <summary>
        ///---------------------------------------------------------------------------------
        ///|     Contribution  |       Loan0       |       Loan1       |       Loan2       |
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   Total   |   tCrtb   |   tLoan   |        Crtb        |        Loan                         |   Return 
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|  35.40 |  50.00   |                   | 327.78 |   0.00   |                   |    50.00  |   35.40   |   15.15   | Pago total = 1     | Sin Liquidez = 1 & Cuota Extra =1 (327.78)                     | 
        /// </summary>
        [TestMethod]
        public void CP_COPERE_13()
        {
            Init();
            var result = Process("13");

            var cp13C = (List<InfoContribution>)result["infoContributionPayedComplete"];
            var cp13L = (List<InfoLoan>)result["infoLoanNoCash"];
            var cp13NQ = (List<InfoLoan>)result["infoLoanNextQuota"];

            Contribution(cp13C, size: 1, contributionPaymentId: 115851101, amountPayed: 35.40M, stateId: (int)ContributionState.Pagado);
            Loan(cp13L, loanId: 115851101, size: 1, monthlyQuota: 327.78M, monthlyPayed: 0M, stateId: (int)LoanState.SinLiquidez);
            NextPayment(cp13NQ, cp13L, loanId: 115851101, size: 1, monthlyQuota: 327.78M);
        }

        /// <summary>
        ///---------------------------------------------------------------------------------
        ///|     Contribution  |       Loan0       |       Loan1       |       Loan2       |
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   Total   |   tCrtb   |   tLoan   |        Crtb        |        Loan                         |   Return 
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|  35.40 |  50.00   |                   | 327.78 |  50.00   |                   |   100.00  |   35.40   |   50.00   | Pago total = 1     | Pago Parcial = 1 & Cuota Extra =1 (277.78)                     | 
        /// </summary>
        [TestMethod]
        public void CP_COPERE_14()
        {
            Init();
            var result = Process("14");

            var cp14C = (List<InfoContribution>)result["infoContributionPayedComplete"];
            var cp14L = (List<InfoLoan>)result["infoLoanIncomplete"];
            var cp14NQ = (List<InfoLoan>)result["infoLoanNextQuota"];

            Contribution(cp14C, size: 1, contributionPaymentId: 115851101, amountPayed: 35.40M, stateId: (int)ContributionState.Pagado);
            Loan(cp14L, loanId: 115851101, size: 1, monthlyQuota: 327.78M, monthlyPayed: 50.00M, stateId: (int)LoanState.PagoParcial);
            NextPayment(cp14NQ, cp14L, loanId: 115851101, size: 1, monthlyQuota: 277.78M);
        }

        /// <summary>
        ///---------------------------------------------------------------------------------
        ///|     Contribution  |       Loan0       |       Loan1       |       Loan2       |
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   Total   |   tCrtb   |   tLoan   |        Crtb        |        Loan                         |   Return 
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|  35.40 |  50.00   |                   | 327.78 |  327.78  |                   |   377.78  |   35.40   |   327.78  | Pago total = 1     | Pago total = 1                      | Retorno = 1 (14.60)
        /// </summary>
        [TestMethod]
        public void CP_COPERE_15()
        {
            Init();
            var result = Process("15");

            var cp15C = (List<InfoContribution>)result["infoContributionPayedComplete"];
            var cp15L = (List<InfoLoan>)result["infoLoanPayedComplete"];
            var cp15RP = (List<Info>)result["infoReturnPayment"];

            Contribution(cp15C, size: 1, contributionPaymentId: 115851101, amountPayed: 35.40M, stateId: (int)ContributionState.Pagado);
            Loan(cp15L, loanId: 115851101, size: 1, monthlyQuota: 327.78M, monthlyPayed: 327.78M, stateId: (int)LoanState.Pagado);
            ReturnPayment(cp15RP, customerId: 115851101, totalPayed: 14.60M);
        }

        /// <summary>
        ///---------------------------------------------------------------------------------
        ///|     Contribution  |       Loan0       |       Loan1       |       Loan2       |
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   Total   |   tCrtb   |   tLoan   |        Crtb        |        Loan                         |   Return 
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|  35.40 |  50.00   |                   | 327.78 |  500.00  |                   |   550.00  |   35.40   |   327.78  | Pago total = 1     | Pago total = 1                      | Retorno = 1 (186.82)
        /// </summary>
        [TestMethod]
        public void CP_COPERE_16()
        {
            Init();
            var result = Process("16");

            var cp16C = (List<InfoContribution>)result["infoContributionPayedComplete"];
            var cp16L = (List<InfoLoan>)result["infoLoanPayedComplete"];
            var cp16RP = (List<Info>)result["infoReturnPayment"];

            Contribution(cp16C, size: 1, contributionPaymentId: 115851101, amountPayed: 35.40M, stateId: (int)ContributionState.Pagado);
            Loan(cp16L, loanId: 115851101, size: 1, monthlyQuota: 327.78M, monthlyPayed: 327.78M, stateId: (int)LoanState.Pagado);
            ReturnPayment(cp16RP, customerId: 115851101, totalPayed: 186.82M);
        }

        /// <summary>
        ///---------------------------------------------------------------------------------
        ///|     Contribution  |       Loan0       |       Loan1       |       Loan2       |
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   Total   |   tCrtb   |   tLoan   |        Crtb        |        Loan                         |   Return 
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|  35.40 |  10.00   |                   |        |          | 384.10 |   0.00   |   10.00   |   10.00   |    0.00   | Pago Parcial = 1   | Sin Liquidez = 2  & Cuota Nueva = 2 (384.10)
        /// </summary>
        [TestMethod]
        public void CP_COPERE_17()
        {
            Init();
            var result = Process("17");

            var cp17C = (List<InfoContribution>)result["infoContributionIncomplete"];
            var cp17L = (List<InfoLoan>)result["infoLoanNoCash"];
            var cp17NQ = (List<InfoLoan>)result["infoLoanNextQuota"];

            Contribution(cp17C, size: 1, contributionPaymentId: 115851102, amountPayed: 10.00M, stateId: (int)ContributionState.PagoParcial);

            LoanPayment(cp17L, 115851102, 115851102, 2, 327.78M, 0M, (int)LoanState.SinLiquidez);
            LoanPayment(cp17L, 115851102, 115851103, 2, 56.32M, 0M, (int)LoanState.SinLiquidez);

            NextPayment(cp17NQ, cp17L, loanId: 115851102, size: 2, monthlyQuota: 384.10M);
        }

        /// <summary>
        ///---------------------------------------------------------------------------------
        ///|     Contribution  |       Loan0       |       Loan1       |       Loan2       |
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   Total   |   tCrtb   |   tLoan   |        Crtb        |        Loan                         |   Return 
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|  35.40 |  10.00   |                   |        |          | 384.10 |   50.00  |   60.00   |   35.40   |   50.00   | Pago parcial = 1   | Pago Parcial = 1  & Cuota Nueva = 2 (334.1)
        /// </summary>
        [TestMethod]
        public void CP_COPERE_18()
        {
            Init();
            var result = Process("18");

            var cp18C = (List<InfoContribution>)result["infoContributionIncomplete"];
            var cp18LPP = (List<InfoLoan>)result["infoLoanIncomplete"];
            var cp18LNC = (List<InfoLoan>)result["infoLoanNoCash"];
            var cp18NQ = (List<InfoLoan>)result["infoLoanNextQuota"];

            Contribution(cp18C, size: 1, contributionPaymentId: 115851102, amountPayed: 10.00M, stateId: (int)ContributionState.PagoParcial);

            LoanPayment(cp18LPP, 115851102, 115851102, 1, 327.78M, 50.00M, (int)LoanState.PagoParcial);
            LoanPayment(cp18LNC, 115851102, 115851103, 1, 56.32M, 0M, (int)LoanState.SinLiquidez);

            NextPayment(cp18NQ, cp18LPP, loanId: 115851102, size: 2, monthlyQuota: 334.10M);
        }

        /// <summary>
        ///---------------------------------------------------------------------------------
        ///|     Contribution  |       Loan0       |       Loan1       |       Loan2       |
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   Total   |   tCrtb   |   tLoan   |        Crtb        |        Loan                         |   Return 
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|  35.40 |  10.00   |                   |        |          | 384.10 |  327.78  |  337.78   |   10.00   |   327.78  | Pago Parcial = 1   | Pago Total = 1 & Pago sin liquidez = 1  & Cuota Nueva = 1 (56.32)
        /// </summary>
        [TestMethod]
        public void CP_COPERE_19()
        {
            Init();
            var result = Process("19");

            var cp19C = (List<InfoContribution>)result["infoContributionIncomplete"];
            var cp19LPC = (List<InfoLoan>)result["infoLoanPayedComplete"];
            var cp19LNC = (List<InfoLoan>)result["infoLoanNoCash"];
            var cp19NQ = (List<InfoLoan>)result["infoLoanNextQuota"];

            Contribution(cp19C, size: 1, contributionPaymentId: 115851102, amountPayed: 10.00M, stateId: (int)ContributionState.PagoParcial);

            LoanPayment(cp19LPC, 115851102, 115851102, 1, 327.78M, 327.78M, (int)LoanState.Pagado);
            LoanPayment(cp19LNC, 115851102, 115851103, 1, 56.32M, 0M, (int)LoanState.SinLiquidez);

            NextPayment(cp19NQ, cp19LPC, loanId: 115851102, size: 1, monthlyQuota: 56.32M);
        }

        /// <summary>
        ///---------------------------------------------------------------------------------
        ///|     Contribution  |       Loan0       |       Loan1       |       Loan2       |
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   Total   |   tCrtb   |   tLoan   |        Crtb        |        Loan                         |   Return 
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|  35.40 |  10.00   |                   |        |          | 384.10 |  384.10  |  394.10   |   10.00   |   384.10  | Pago Parcial = 1   | Pago total = 2
        /// </summary>
        [TestMethod]
        public void CP_COPERE_20()
        {
            Init();
            var result = Process("20");

            var cp20C = (List<InfoContribution>)result["infoContributionIncomplete"];
            var cp20PC = (List<InfoLoan>)result["infoLoanPayedComplete"];

            Contribution(cp20C, size: 1, contributionPaymentId: 115851102, amountPayed: 10.00M, stateId: (int)ContributionState.PagoParcial);

            LoanPayment(cp20PC, 115851102, 115851102, 2, 327.78M, 327.78M, (int)LoanState.Pagado);
            LoanPayment(cp20PC, 115851102, 115851103, 2, 56.32M, 56.32M, (int)LoanState.Pagado);

        }

        /// <summary>
        ///---------------------------------------------------------------------------------
        ///|     Contribution  |       Loan0       |       Loan1       |       Loan2       |
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   Total   |   tCrtb   |   tLoan   |        Crtb        |        Loan                         |   Return 
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|  35.40 |  10.00   |                   |        |          | 384.10 |  500.00  |  510.00   |   10.00   |   384.10   | Pago Parcial = 1  | Pago total = 2                      | Devolucion de 115.90
        /// </summary>
        [TestMethod]
        public void CP_COPERE_21()
        {
            Init();
            var result = Process("21");

            var cp21C = (List<InfoContribution>)result["infoContributionIncomplete"];

            var cp21PC = (List<InfoLoan>)result["infoLoanPayedComplete"];
            var cp21LPC = (List<InfoLoan>)result["infoLoanPayedComplete"];
            var cp21RP = (List<Info>)result["infoReturnPayment"];

            Contribution(cp21C, size: 1, contributionPaymentId: 115851102, amountPayed: 10.0M, stateId: (int)ContributionState.PagoParcial);

            LoanPayment(cp21PC, 115851102, 115851102, 2, 327.78M, 327.78M, (int)LoanState.Pagado);
            LoanPayment(cp21PC, 115851102, 115851103, 2, 56.32M, 56.32M, (int)LoanState.Pagado);

            ReturnPayment(cp21RP, customerId: 115851102, totalPayed: 115.90M);
        }

        /// <summary>
        ///---------------------------------------------------------------------------------
        ///|     Contribution  |       Loan0       |       Loan1       |       Loan2       |
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   Total   |   tCrtb   |   tLoan   |        Crtb        |        Loan                         |   Return 
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|  35.40 |  35.40   |                   |        |          | 384.10 |   0.00   |  35.40    |   35.40   |   0.00    | Pago Total = 1     | Sin Liquidez = 2 & Nueva Cuota = 2 (384.10)
        /// </summary>
        [TestMethod]
        public void CP_COPERE_22()
        {
            Init();
            var result = Process("22");

            var cp22C = (List<InfoContribution>)result["infoContributionPayedComplete"];
            var cp22L = (List<InfoLoan>)result["infoLoanNoCash"];
            var cp22NP = (List<InfoLoan>)result["infoLoanNextQuota"];

            Contribution(cp22C, size: 1, contributionPaymentId: 115851102, amountPayed: 35.40M, stateId: (int)ContributionState.Pagado);

            LoanPayment(cp22L, 115851102, 115851102, 2, 327.78M, 0M, (int)LoanState.SinLiquidez);
            LoanPayment(cp22L, 115851102, 115851103, 2, 56.32M, 0M, (int)LoanState.SinLiquidez);

            NextPayment(cp22NP, cp22L, loanId: 115851102, size: 2, monthlyQuota: 384.10M);

        }

        /// <summary>
        ///---------------------------------------------------------------------------------
        ///|     Contribution  |       Loan0       |       Loan1       |       Loan2       |
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   Total   |   tCrtb   |   tLoan   |        Crtb        |        Loan                         |   Return 
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|  35.40 |  35.40   |                   |        |          | 384.10 |  50.00   |  84.85    |   35.40   |   50.00   | Pago Total = 1     | Pago Parcial = 1 & Sin Liquidez  = 1 & Cuota nueva = 2 (334.10)
        /// </summary>
        [TestMethod]
        public void CP_COPERE_23()
        {
            Init();
            var result = Process("23");

            var cp23C = (List<InfoContribution>)result["infoContributionPayedComplete"];
            var cp23PP = (List<InfoLoan>)result["infoLoanIncomplete"];
            var cp23NC = (List<InfoLoan>)result["infoLoanNoCash"];
            var cp23NP = (List<InfoLoan>)result["infoLoanNextQuota"];

            Contribution(cp23C, size: 1, contributionPaymentId: 115851102, amountPayed: 35.40M, stateId: (int)ContributionState.Pagado);

            LoanPayment(cp23PP, 115851102, 115851102, 1, 327.78M, 50M, (int)LoanState.PagoParcial);
            LoanPayment(cp23NC, 115851102, 115851103, 1, 56.32M, 0M, (int)LoanState.SinLiquidez);

            NextPayment(cp23NP, cp23NC, loanId: 115851102, size: 2, monthlyQuota: 384.10M - 50M);

        }

        /// <summary>
        ///---------------------------------------------------------------------------------
        ///|     Contribution  |       Loan0       |       Loan1       |       Loan2       |
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   Total   |   tCrtb   |   tLoan   |        Crtb        |        Loan                         |   Return 
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|  35.40 |  35.40   |                   |        |          | 384.10 | 327.78   |  362.63   |   35.40   |  327.78   | Pago Total = 1     | Pago Total = 1 & Sin Liquidez  = 1 & Cuota nueva = 1 (56.32)
        /// </summary>
        [TestMethod]
        public void CP_COPERE_24()
        {
            Init();
            var result = Process("24");

            var cp24C = (List<InfoContribution>)result["infoContributionPayedComplete"];
            var cp24L = (List<InfoLoan>)result["infoLoanPayedComplete"];
            var cp24NC = (List<InfoLoan>)result["infoLoanNoCash"];
            var cp24NP = (List<InfoLoan>)result["infoLoanNextQuota"];

            Contribution(cp24C, size: 1, contributionPaymentId: 115851102, amountPayed: 35.40M, stateId: (int)ContributionState.Pagado);

            LoanPayment(cp24L, 115851102, 115851102, 1, 327.78M, 327.78M, (int)LoanState.Pagado);
            LoanPayment(cp24NC, 115851102, 115851103, 1, 56.32M, 0M, (int)LoanState.SinLiquidez);

            NextPayment(cp24NP, cp24NC, loanId: 115851102, size: 1, monthlyQuota: 56.32M);

        }

        /// <summary>
        ///---------------------------------------------------------------------------------
        ///|     Contribution  |       Loan0       |       Loan1       |       Loan2       |
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   Total   |   tCrtb   |   tLoan   |        Crtb        |        Loan                         |   Return 
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|  35.40 |  35.40   |                   |        |          | 384.10 | 384.10   |   418.95  |   35.40   |  384.10   | Pago Total = 1     | Pago Total = 2 
        /// </summary>
        [TestMethod]
        public void CP_COPERE_25()
        {
            Init();
            var result = Process("25");

            var cp25C = (List<InfoContribution>)result["infoContributionPayedComplete"];
            var cp25L = (List<InfoLoan>)result["infoLoanPayedComplete"];


            Contribution(cp25C, size: 1, contributionPaymentId: 115851102, amountPayed: 35.40M, stateId: (int)ContributionState.Pagado);

            LoanPayment(cp25L, 115851102, 115851102, 2, 327.78M, 327.78M, (int)LoanState.Pagado);
            LoanPayment(cp25L, 115851102, 115851103, 2, 56.32M, 56.32M, (int)LoanState.Pagado);

        }

        /// <summary>
        ///---------------------------------------------------------------------------------
        ///|     Contribution  |       Loan0       |       Loan1       |       Loan2       |
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   Total   |   tCrtb   |   tLoan   |        Crtb        |        Loan                         |   Return 
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|  35.40 |  35.40   |                   |        |          | 384.10 | 500.00   |   418.95  |   35.40   |  384.10   | Pago Total = 1     | Pago Total = 2                      | Devolucion de 115.9
        /// </summary>
        [TestMethod]
        public void CP_COPERE_26()
        {
            Init();
            var result = Process("26");

            var cp26C = (List<InfoContribution>)result["infoContributionPayedComplete"];
            var cp26L = (List<InfoLoan>)result["infoLoanPayedComplete"];
            var cp26DV = (List<Info>)result["infoReturnPayment"];

            Contribution(cp26C, size: 1, contributionPaymentId: 115851102, amountPayed: 35.40M, stateId: (int)ContributionState.Pagado);

            LoanPayment(cp26L, 115851102, 115851102, 2, 327.78M, 327.78M, (int)LoanState.Pagado);
            LoanPayment(cp26L, 115851102, 115851103, 2, 56.32M, 56.32M, (int)LoanState.Pagado);

            ReturnPayment(cp26DV, customerId: 115851102, totalPayed: 115.9M);

        }

        /// <summary>
        ///---------------------------------------------------------------------------------
        ///|     Contribution  |       Loan0       |       Loan1       |       Loan2       |
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   Total   |   tCrtb   |   tLoan   |        Crtb        |        Loan                         |   Return 
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|  35.40 |  50.00   |                   |        |          | 384.10 |   0.0    |   50.00   |   35.40   |    0.00   | Pago Total = 1     | Sin liquidez = 2                    | Devolucion de 14.60    
        /// </summary>
        [TestMethod]
        public void CP_COPERE_27()
        {
            Init();
            var result = Process("27");

            var cp27C = (List<InfoContribution>)result["infoContributionPayedComplete"];
            var cp27NC = (List<InfoLoan>)result["infoLoanNoCash"];
            var cp27DV = (List<Info>)result["infoReturnPayment"];


            Contribution(cp27C, size: 1, contributionPaymentId: 115851102, amountPayed: 35.40M, stateId: (int)ContributionState.Pagado);

            LoanPayment(cp27NC, 115851102, 115851102, 2, 327.78M, 0M, (int)LoanState.SinLiquidez);
            LoanPayment(cp27NC, 115851102, 115851103, 2, 56.32M, 0M, (int)LoanState.SinLiquidez);

            ReturnPayment(cp27DV, customerId: 115851102, totalPayed: 14.60M);

        }

        /// <summary>
        ///---------------------------------------------------------------------------------
        ///|     Contribution  |       Loan0       |       Loan1       |       Loan2       |
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   Total   |   tCrtb   |   tLoan   |        Crtb        |        Loan                         |   Return 
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|  35.40 |  50.00   |                   |        |          | 384.10 |   50.00  |  100.00   |   35.40   |  50.00    | Pago Total = 1     | Pago Parcial = 1 & Sin liquidez = 1 & Cuota siguiente = 2 (334.10)
        /// </summary>
        [TestMethod]
        public void CP_COPERE_28()
        {
            Init();
            var result = Process("28");

            var cp28C = (List<InfoContribution>)result["infoContributionPayedComplete"];
            var cp28PC = (List<InfoLoan>)result["infoLoanIncomplete"];
            var cp28NC = (List<InfoLoan>)result["infoLoanNoCash"];
            var cp28NP = (List<InfoLoan>)result["infoLoanNextQuota"];


            Contribution(cp28C, size: 1, contributionPaymentId: 115851102, amountPayed: 35.40M, stateId: (int)ContributionState.Pagado);

            LoanPayment(cp28PC, 115851102, 115851102, 1, 327.78M, 50.00M, (int)LoanState.PagoParcial);
            LoanPayment(cp28NC, 115851102, 115851103, 1, 56.32M, 0M, (int)LoanState.SinLiquidez);

            NextPayment(cp28NP, cp28NC, loanId: 115851102, size: 2, monthlyQuota: 334.10M);

        }

        /// <summary>
        ///---------------------------------------------------------------------------------
        ///|     Contribution  |       Loan0       |       Loan1       |       Loan2       |
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   Total   |   tCrtb   |   tLoan   |        Crtb        |        Loan                         |   Return 
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|  35.40 |  50.00   |                   |        |          | 384.10 |  327.78  |  377.78   |   35.40   |  327.78   | Pago Total = 1      | Pago Total = 1 & Sin Liquidez = 1 & Cuota siguiente = 1 (56.32)
        /// </summary>
        [TestMethod]
        public void CP_COPERE_29()
        {
            Init();
            var result = Process("29");

            var cp29C = (List<InfoContribution>)result["infoContributionPayedComplete"];
            var cp29PC = (List<InfoLoan>)result["infoLoanPayedComplete"];
            var cp29NC = (List<InfoLoan>)result["infoLoanNoCash"];
            var cp29NP = (List<InfoLoan>)result["infoLoanNextQuota"];


            Contribution(cp29C, size: 1, contributionPaymentId: 115851102, amountPayed: 35.40M, stateId: (int)ContributionState.Pagado);

            LoanPayment(cp29PC, 115851102, 115851102, 1, 327.78M, 327.78M, (int)LoanState.Pagado);
            LoanPayment(cp29NC, 115851102, 115851103, 1, 56.32M, 0.0M, (int)LoanState.SinLiquidez);

            NextPayment(cp29NP, cp29NC, loanId: 115851102, size: 1, monthlyQuota: 56.32M);

        }

        /// <summary>
        ///---------------------------------------------------------------------------------
        ///|     Contribution  |       Loan0       |       Loan1       |       Loan2       |
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   Total   |   tCrtb   |   tLoan   |        Crtb        |        Loan                         |   Return 
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|  35.40 |  50.00   |                   |        |          | 384.10 |  384.10  |  434.10   |   35.40   |  384.10   | Pago Total = 1      | Pago Total = 2   | Devolucion = 1 (14.60)
        /// </summary>
        [TestMethod]
        public void CP_COPERE_30()
        {
            Init();
            var result = Process("30");

            var cp30C = (List<InfoContribution>)result["infoContributionPayedComplete"];
            var cp30PC = (List<InfoLoan>)result["infoLoanPayedComplete"];
            var cp30DV = (List<Info>)result["infoReturnPayment"]; ;

            Contribution(cp30C, size: 1, contributionPaymentId: 115851102, amountPayed: 35.40M, stateId: (int)ContributionState.Pagado);

            LoanPayment(cp30PC, 115851102, 115851102, 2, 327.78M, 327.78M, (int)LoanState.Pagado);
            LoanPayment(cp30PC, 115851102, 115851103, 2, 56.32M, 56.32M, (int)LoanState.Pagado);
            ReturnPayment(cp30DV, customerId: 115851102, totalPayed: 14.60M);
        }

        /// <summary>
        ///---------------------------------------------------------------------------------
        ///|     Contribution  |       Loan0       |       Loan1       |       Loan2       |
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   Total   |   tCrtb   |   tLoan   |        Crtb        |        Loan                         |   Return 
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|  35.40 |  50.00   |                   |        |          | 384.10 |  500.00  |  550.00   |   35.40   |  384.10   | Pago Total = 1      | Pago Total = 2   | Devolucion = 1 (130.50)
        /// </summary>
        [TestMethod]
        public void CP_COPERE_31()
        {
            Init();
            var result = Process("31");

            var cp31C = (List<InfoContribution>)result["infoContributionPayedComplete"];
            var cp31PC = (List<InfoLoan>)result["infoLoanPayedComplete"];
            var cp31DV = (List<Info>)result["infoReturnPayment"]; ;

            Contribution(cp31C, size: 1, contributionPaymentId: 115851102, amountPayed: 35.40M, stateId: (int)ContributionState.Pagado);

            LoanPayment(cp31PC, 115851102, 115851102, 2, 327.78M, 327.78M, (int)LoanState.Pagado);
            LoanPayment(cp31PC, 115851102, 115851103, 2, 56.32M, 56.32M, (int)LoanState.Pagado);
            ReturnPayment(cp31DV, customerId: 115851102, totalPayed: 130.50M);
        }
    }
}
