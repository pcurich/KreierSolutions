using Ks.Batch.Util.Model;
using Ks.Batch.Caja.In;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Ks.Batch.Test;

namespace Ks.Batch.Caja.Test
{
    [TestClass]
    public class CajaTest: BaseTest
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
                    TotalContribution=34.85M,TotalPayed = 0, TotalLoan=0M,
                    InfoContribution = new InfoContribution { ContributionPaymentId= 115851100, Amount1 = 34.85M, Amount2=0M,Amount3=0M, AmountOld=0M, AmountTotal=34.85M, AmountPayed=0M, StateId=1}
                },
                //Aportacion + 1 Apoyo
                new Info
                {
                    Year = YEAR,Month=MONTH, HasAdminCode = true, AdminCode = "115851101", HasDni = true,Dni= "20108420", CustomerId = 115851101,
                    TotalContribution=34.85M,TotalPayed = 0, TotalLoan=327.78M,
                    InfoContribution = new InfoContribution {ContributionPaymentId= 115851101, Amount1 = 34.85M, Amount2=0M,Amount3=0M, AmountOld=0M, AmountTotal=34.85M, AmountPayed=0M, StateId=1},
                    InfoLoans = new List<InfoLoan>(){
                        new InfoLoan(){LoanId=115851101,LoanPaymentId = 115851101,MonthlyQuota = 327.78M,Quota = 1, Period =12}
                    }
                },
                //Aportacion + 2 Apoyo
                new Info
                {
                    Year = YEAR,Month=MONTH, HasAdminCode = true, AdminCode = "115851102", HasDni = true,Dni= "20108420", CustomerId = 115851102,
                    TotalContribution=34.85M,TotalPayed = 0, TotalLoan=384.1M,
                    InfoContribution = new InfoContribution {ContributionPaymentId= 115851102, Amount1 = 34.85M, Amount2=0M,Amount3=0M, AmountOld=0M, AmountTotal=34.85M, AmountPayed=0M, StateId=1},
                    InfoLoans = new List<InfoLoan>(){
                        new InfoLoan(){LoanId=115851102,LoanPaymentId = 115851102,  MonthlyQuota = 327.78M,Quota = 1, Period =12},
                        new InfoLoan(){LoanId=115851102,LoanPaymentId = 115851103,MonthlyQuota = 56.32M,Quota = 1, Period =12}
                    }
                }
            };
        } 

        /// <summary>
        ///---------------------------------------------------------------------------------
        ///|     Contribution  |       Loan0       |       Loan1       |       Loan2       |
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   Total   |   tCrtb   |   tLoan   |        Crtb        |        Loan        |   Return 
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|  34.85 |    NA    |                                                                                               | Sin Liquidez = 1                        | 
        /// </summary>
        [TestMethod]
        public void CP_CAJA_01()
        {
            Init();
            var report = new Report() { ParentKey = Guid.NewGuid() };

            List<Info> dataFile = InfoService.ReadFile("../../../Files/Caja/CP_CAJA_01.txt", "es-PE");
            var dao = new Merge.Dao(null);
            var result = dao.Process(report, dataFile, dataBase, "Test-Bank", "Account");

            var cp01 = (List<InfoContribution>)result["infoContributionNoCash"];
            Contribution(cp01, size: 1, contributionPaymentId: 115851100, amountPayed: 0, stateId: (int)ContributionState.SinLiquidez);
        }

        /// <summary>
        ///---------------------------------------------------------------------------------
        ///|     Contribution  |       Loan0       |       Loan1       |       Loan2       |
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   Total   |   tCrtb   |   tLoan   |        Crtb        |        Loan        |   Return 
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|  34.85 |  10.00   |        |          |        |          |        |          |    10.00  |    10.00  |    0.00   | Pago Parcial = 1   |                    | 
        /// </summary>
        [TestMethod]
        public void CP_CAJA_02()
        {
            Init();
            var report = new Report() { ParentKey = Guid.NewGuid() };

            List<Info> dataFile = InfoService.ReadFile("../../../Files/Caja/CP_CAJA_02.txt", "es-PE");
            var dao = new Merge.Dao(null);
            var result = dao.Process(report, dataFile, dataBase, "Test-Bank", "Account");
            var cp02 = (List<InfoContribution>)result["infoContributionIncomplete"];

            Contribution(cp02, size: 1, contributionPaymentId: 115851100, amountPayed: 10M, stateId: (int)ContributionState.PagoParcial);
        }

        /// <summary>
        ///---------------------------------------------------------------------------------
        ///|     Contribution  |       Loan0       |       Loan1       |       Loan2       |
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   Total   |   tCrtb   |   tLoan   |        Crtb        |        Loan        |   Return 
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|  34.85 |  34.85   |        |          |        |          |        |          |    34.85  |   34.85   |    0.00   | Pago Total = 1     |                    | 
        /// </summary>
        [TestMethod]
        public void CP_CAJA_03()
        {
            Init();
            var report = new Report() { ParentKey = Guid.NewGuid() };

            List<Info> dataFile = InfoService.ReadFile("../../../Files/Caja/CP_CAJA_03.txt", "es-PE");
            var dao = new Merge.Dao(null);
            var result = dao.Process(report, dataFile, dataBase, "Test-Bank", "Account");
            var cp03 = (List<InfoContribution>)result["infoContributionPayedComplete"];

            Contribution(cp03, size: 1, contributionPaymentId: 115851100, amountPayed: 34.85M, stateId: (int)ContributionState.Pagado);
        }

        /// <summary>
        ///---------------------------------------------------------------------------------
        ///|     Contribution  |       Loan0       |       Loan1       |       Loan2       |
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   Total   |   tCrtb   |   tLoan   |        Crtb        |        Loan        |   Return 
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|  34.85 |  50.00   |        |          |        |          |        |          |   50.00   |  34.38    |    0.00   | Pago Total = 1     |                    | Devolucion de 15.15
        /// </summary>
        [TestMethod]
        public void CP_CAJA_04()
        {
            Init();
            var report = new Report() { ParentKey = Guid.NewGuid() };

            List<Info> dataFile = InfoService.ReadFile("../../../Files/Caja/CP_CAJA_04.txt", "es-PE");
            var dao = new Merge.Dao(null);
            var result = dao.Process(report, dataFile, dataBase, "Test-Bank", "Account");

            var cp04C = (List<InfoContribution>)result["infoContributionPayedComplete"];
            var cp04R = (List<Info>)result["infoReturnPayment"];

            Contribution(cp04C, size: 1, contributionPaymentId: 115851100, amountPayed: 34.85M, stateId: (int)ContributionState.Pagado);
            ReturnPayment(cp04R, customerId: 115851100, totalPayed: 15.15M);

        }

        /// <summary>
        ///---------------------------------------------------------------------------------
        ///|     Contribution  |       Loan0       |       Loan1       |       Loan2       |
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   Total   |   tCrtb   |   tLoan   |        Crtb        |        Loan        |   Return 
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|  34.85 |  10.00   |                   | 327.78 |   0.00   |        |          |    10.00  |    10.10  |    0.00   | Pago Parcial = 1   | Sin Liquidez = 1 & Cuota Adicional =1  (327.78)
        /// </summary>
        [TestMethod]
        public void CP_CAJA_05()
        {
            Init();
            var report = new Report() { ParentKey = Guid.NewGuid() };

            List<Info> dataFile = InfoService.ReadFile("../../../Files/Caja/CP_CAJA_05.txt", "es-PE");
            var dao = new Merge.Dao(null);
            var result = dao.Process(report, dataFile, dataBase, "Test-Bank", "Account");

            var cp05C = (List<InfoContribution>)result["infoContributionIncomplete"];
            var cp05L = (List<InfoLoan>)result["infoLoanNoCash"];
            var cp05NQ = (List<InfoLoan>)result["infoLoanNextQuota"];

            Contribution(cp05C, size: 1, contributionPaymentId: 115851101, amountPayed: 10.00M, stateId: (int)ContributionState.PagoParcial);
            Loan(cp05L, loanId: 115851101, size: 1,monthlyQuota:327.78M,monthlyPayed:0M, stateId: (int)LoanState.SinLiquidez);
            NextPayment(cp05NQ, cp05L, loanId: 115851101, size:1, monthlyQuota: 327.78M);
        }

        /// <summary>
        ///---------------------------------------------------------------------------------
        ///|     Contribution  |       Loan0       |       Loan1       |       Loan2       |
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   Total   |   tCrtb   |   tLoan   |        Crtb        |        Loan        |   Return 
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|  34.85 |  10.00   |                   | 327.78 |  50.00   |                   |    60.00  |    34.85  |   25.15   | Pago total = 1     | Pago Parcial = 1 & Cuota Adicional =1 (302.63)
        /// </summary>
        [TestMethod]
        public void CP_CAJA_06()
        {
            Init();
            var report = new Report() { ParentKey = Guid.NewGuid() };

            List<Info> dataFile = InfoService.ReadFile("../../../Files/Caja/CP_CAJA_06.txt", "es-PE");
            var dao = new Merge.Dao(null);
            var result = dao.Process(report, dataFile, dataBase, "Test-Bank", "Account");

            var cp06C = (List<InfoContribution>)result["infoContributionPayedComplete"];
            var cp06L = (List<InfoLoan>)result["infoLoanIncomplete"];
            var cp06NQ = (List<InfoLoan>)result["infoLoanNextQuota"];

            Contribution(cp06C, size: 1, contributionPaymentId: 115851101, amountPayed: 34.85M, stateId: (int)ContributionState.Pagado);
            Loan(cp06L, loanId: 115851101, size: 1, monthlyQuota: 327.78M, monthlyPayed: 25.15M, stateId: (int)LoanState.PagoParcial);
            NextPayment(cp06NQ, cp06L, loanId: 115851101, size: 1, monthlyQuota: 302.63M);
        }

        /// <summary>
        ///---------------------------------------------------------------------------------
        ///|     Contribution  |       Loan0       |       Loan1       |       Loan2       |
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   Total   |   tCrtb   |   tLoan   |        Crtb        |        Loan        |   Return 
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|  34.85 |  10.00   |                   | 327.78 | 327.78   |                   |   337.78  |    34.85  |  302.93   | Pago total = 1     | Pago Parcial = 1 & Cuota Adicional =1 (24.85)
        /// </summary>
        [TestMethod]
        public void CP_CAJA_07()
        {
            Init();
            var report = new Report() { ParentKey = Guid.NewGuid() };

            List<Info> dataFile = InfoService.ReadFile("../../../Files/Caja/CP_CAJA_07.txt", "es-PE");
            var dao = new Merge.Dao(null);
            var result = dao.Process(report, dataFile, dataBase, "Test-Bank", "Account");

            var cp07C = (List<InfoContribution>)result["infoContributionPayedComplete"];
            var cp07L = (List<InfoLoan>)result["infoLoanIncomplete"];
            var cp07NQ = (List<InfoLoan>)result["infoLoanNextQuota"];

            Contribution(cp07C, size: 1, contributionPaymentId: 115851101, amountPayed: 34.85M, stateId: (int)ContributionState.Pagado);
            Loan(cp07L, loanId: 115851101, size: 1, monthlyQuota: 327.78M, monthlyPayed: 302.93M, stateId: (int)LoanState.PagoParcial);
            NextPayment(cp07NQ, cp07L, loanId: 115851101, size: 1, monthlyQuota: 24.85M);
 
        }

        /// <summary>
        ///---------------------------------------------------------------------------------
        ///|     Contribution  |       Loan0       |       Loan1       |       Loan2       |
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   Total   |   tCrtb   |   tLoan   |        Crtb        |        Loan        |   Return 
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|  34.85 |  10.00   |                   | 327.78 | 500.00   |                   |   510.00  |    34.85  |  327.78   | Pago total = 1     | Pago total = 1     | Devolucion = 1 (147.37)
        /// </summary>
        [TestMethod]
        public void CP_CAJA_08()
        {
            Init();
            var report = new Report() { ParentKey = Guid.NewGuid() };

            List<Info> dataFile = InfoService.ReadFile("../../../Files/Caja/CP_CAJA_08.txt", "es-PE");
            var dao = new Merge.Dao(null);
            var result = dao.Process(report, dataFile, dataBase, "Test-Bank", "Account");

            var cp08C = (List<InfoContribution>)result["infoContributionPayedComplete"];
            var cp08L = (List<InfoLoan>)result["infoLoanPayedComplete"];
            var cp08DV = (List<Info>)result["infoReturnPayment"];

            Contribution(cp08C, size: 1, contributionPaymentId: 115851101, amountPayed: 34.85M, stateId: (int)ContributionState.Pagado);
            Loan(cp08L, loanId: 115851101, size: 1, monthlyQuota: 327.78M, monthlyPayed: 327.78M, stateId: (int)LoanState.Pagado);
            ReturnPayment(cp08DV, customerId: 115851101, totalPayed: 147.37M);
        }

        /// <summary>
        ///---------------------------------------------------------------------------------
        ///|     Contribution  |       Loan0       |       Loan1       |       Loan2       |
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   Total   |   tCrtb   |   tLoan   |        Crtb        |        Loan                         |   Return 
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|  34.85 |  34.85   |                   | 327.78 |   0.00   |                   |   34.85   |    34.85  |    0.00   | Pago total = 1     | Sin Liquidez = 1 & Cuota Extra = 1  | 
        /// </summary>
        [TestMethod]
        public void CP_CAJA_09()
        {
            Init();
            var report = new Report() { ParentKey = Guid.NewGuid() };

            List<Info> dataFile = InfoService.ReadFile("../../../Files/Caja/CP_CAJA_09.txt", "es-PE");
            var dao = new Merge.Dao(null);
            var result = dao.Process(report, dataFile, dataBase, "Test-Bank", "Account");

            var cp09C = (List<InfoContribution>)result["infoContributionPayedComplete"];
            var cp09L = (List<InfoLoan>)result["infoLoanNoCash"];
            var cp09NQ = (List<InfoLoan>)result["infoLoanNextQuota"];

            Contribution(cp09C, size: 1, contributionPaymentId: 115851101, amountPayed: 34.85M, stateId: (int)ContributionState.Pagado);
            Loan(cp09L, loanId: 115851101, size: 1, monthlyQuota: 327.78M, monthlyPayed: 0M, stateId: (int)LoanState.SinLiquidez);
            NextPayment(cp09NQ, cp09L, loanId: 115851101, size: 1, monthlyQuota: 327.78M); 
        }

        /// <summary>
        ///---------------------------------------------------------------------------------
        ///|     Contribution  |       Loan0       |       Loan1       |       Loan2       |
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   Total   |   tCrtb   |   tLoan   |        Crtb        |        Loan                         |   Return 
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|  34.85 |  34.85   |                   | 327.78 |  50.00   |                   |   84.85   |    34.85  |   50.00   | Pago total = 1     | Pago Parcial = 1 & Cuota Extra = 1 (277.78)  | 
        /// </summary>
        [TestMethod]
        public void CP_CAJA_10()
        {
            Init();
            var report = new Report() { ParentKey = Guid.NewGuid() };

            List<Info> dataFile = InfoService.ReadFile("../../../Files/Caja/CP_CAJA_10.txt", "es-PE");
            var dao = new Merge.Dao(null);
            var result = dao.Process(report, dataFile, dataBase, "Test-Bank", "Account");

            var cp10C = (List<InfoContribution>)result["infoContributionPayedComplete"];
            var cp10L = (List<InfoLoan>)result["infoLoanIncomplete"];
            var cp10NQ = (List<InfoLoan>)result["infoLoanNextQuota"];

            Contribution(cp10C, size: 1, contributionPaymentId: 115851101, amountPayed: 34.85M, stateId: (int)ContributionState.Pagado);
            Loan(cp10L, loanId: 115851101, size: 1, monthlyQuota: 327.78M, monthlyPayed: 50.00M, stateId: (int)LoanState.PagoParcial);
            NextPayment(cp10NQ, cp10L, loanId: 115851101, size: 1, monthlyQuota: 277.78M);

        }

        /// <summary>
        ///---------------------------------------------------------------------------------
        ///|     Contribution  |       Loan0       |       Loan1       |       Loan2       |
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   Total   |   tCrtb   |   tLoan   |        Crtb        |        Loan                         |   Return 
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|  34.85 |  34.85   |                   | 327.78 |  327.78  |                   |   362.63  |   34.85   |  327.78   | Pago total = 1     | Pago Total = 1                      | 
        /// </summary>
        [TestMethod]
        public void CP_CAJA_11()
        {
            Init();
            var report = new Report() { ParentKey = Guid.NewGuid() };

            List<Info> dataFile = InfoService.ReadFile("../../../Files/Caja/CP_CAJA_11.txt", "es-PE");
            var dao = new Merge.Dao(null);
            var result = dao.Process(report, dataFile, dataBase, "Test-Bank", "Account");

            var cp11C = (List<InfoContribution>)result["infoContributionPayedComplete"];
            var cp11L = (List<InfoLoan>)result["infoLoanPayedComplete"]; 

            Contribution(cp11C, size: 1, contributionPaymentId: 115851101, amountPayed: 34.85M, stateId: (int)ContributionState.Pagado);
            Loan(cp11L, loanId: 115851101, size: 1, monthlyQuota: 327.78M, monthlyPayed: 327.78M, stateId: (int)LoanState.Pagado);
        }

        /// <summary>
        ///---------------------------------------------------------------------------------
        ///|     Contribution  |       Loan0       |       Loan1       |       Loan2       |
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   Total   |   tCrtb   |   tLoan   |        Crtb        |        Loan                         |   Return 
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|  34.85 |  34.85   |                   | 327.78 |  500.00  |                   |   534.85  |   34.85   |  327.78   | Pago total = 1     | Pago Total = 1                      | Retorno por 172.22
        /// </summary>
        [TestMethod]
        public void CP_CAJA_12()
        {
            Init();
            var report = new Report() { ParentKey = Guid.NewGuid() };

            List<Info> dataFile = InfoService.ReadFile("../../../Files/Caja/CP_CAJA_12.txt", "es-PE");
            var dao = new Merge.Dao(null);
            var result = dao.Process(report, dataFile, dataBase, "Test-Bank", "Account");

            var cp12C = (List<InfoContribution>)result["infoContributionPayedComplete"];
            var cp12L = (List<InfoLoan>)result["infoLoanPayedComplete"];
            var cp12DV = (List<Info>)result["infoReturnPayment"];

            Contribution(cp12C, size: 1, contributionPaymentId: 115851101, amountPayed: 34.85M, stateId: (int)ContributionState.Pagado);
            Loan(cp12L, loanId: 115851101, size: 1, monthlyQuota: 327.78M, monthlyPayed: 327.78M, stateId: (int)LoanState.Pagado);
            ReturnPayment(cp12DV, customerId: 115851101, totalPayed: 172.22M);

        }

        /// <summary>
        ///---------------------------------------------------------------------------------
        ///|     Contribution  |       Loan0       |       Loan1       |       Loan2       |
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   Total   |   tCrtb   |   tLoan   |        Crtb        |        Loan                         |   Return 
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|  34.85 |  50.00   |                   | 327.78 |   0.00   |                   |    50.00  |   34.85   |   15.15   | Pago total = 1     | Pago Parcial = 1 & Cuota Extra =1 (312.63)                     | 
        /// </summary>
        [TestMethod]
        public void CP_CAJA_13()
        {
            Init();
            var report = new Report() { ParentKey = Guid.NewGuid() };

            List<Info> dataFile = InfoService.ReadFile("../../../Files/Caja/CP_CAJA_13.txt", "es-PE");
            var dao = new Merge.Dao(null);
            var result = dao.Process(report, dataFile, dataBase, "Test-Bank", "Account");

            var cp13C = (List<InfoContribution>)result["infoContributionPayedComplete"];
            var cp13L = (List<InfoLoan>)result["infoLoanIncomplete"];
            var cp13NQ = (List<InfoLoan>)result["infoLoanNextQuota"];

            Contribution(cp13C, size: 1, contributionPaymentId: 115851101, amountPayed: 34.85M, stateId: (int)ContributionState.Pagado);
            Loan(cp13L, loanId: 115851101, size: 1, monthlyQuota: 327.78M, monthlyPayed: 15.15M, stateId: (int)LoanState.PagoParcial);
            NextPayment(cp13NQ, cp13L, loanId: 115851101, size: 1, monthlyQuota: 312.63M);
        }

        /// <summary>
        ///---------------------------------------------------------------------------------
        ///|     Contribution  |       Loan0       |       Loan1       |       Loan2       |
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   Total   |   tCrtb   |   tLoan   |        Crtb        |        Loan                         |   Return 
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|  34.85 |  50.00   |                   | 327.78 |  50.00   |                   |   100.00  |   34.85   |   65.15   | Pago total = 1     | Pago Parcial = 1 & Cuota Extra =1 (262.63)                     | 
        /// </summary>
        [TestMethod]
        public void CP_CAJA_14()
        {
            Init();
            var report = new Report() { ParentKey = Guid.NewGuid() };

            List<Info> dataFile = InfoService.ReadFile("../../../Files/Caja/CP_CAJA_14.txt", "es-PE");
            var dao = new Merge.Dao(null);
            var result = dao.Process(report, dataFile, dataBase, "Test-Bank", "Account");

            var cp14C = (List<InfoContribution>)result["infoContributionPayedComplete"];
            var cp14L = (List<InfoLoan>)result["infoLoanIncomplete"];
            var cp14NQ = (List<InfoLoan>)result["infoLoanNextQuota"];

            Contribution(cp14C, size: 1, contributionPaymentId: 115851101, amountPayed: 34.85M, stateId: (int)ContributionState.Pagado);
            Loan(cp14L, loanId: 115851101, size: 1, monthlyQuota: 327.78M, monthlyPayed: 65.15M, stateId: (int)LoanState.PagoParcial);
            NextPayment(cp14NQ, cp14L, loanId: 115851101, size: 1, monthlyQuota: 262.63M);
        }

        /// <summary>
        ///---------------------------------------------------------------------------------
        ///|     Contribution  |       Loan0       |       Loan1       |       Loan2       |
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   Total   |   tCrtb   |   tLoan   |        Crtb        |        Loan                         |   Return 
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|  34.85 |  50.00   |                   | 327.78 |  327.78  |                   |   377.78  |   34.85   |   327.78  | Pago total = 1     | Pago total = 1                      | Retorno = 1 (15.15)
        /// </summary>
        [TestMethod]
        public void CP_CAJA_15()
        {
            Init();
            var report = new Report() { ParentKey = Guid.NewGuid() };

            List<Info> dataFile = InfoService.ReadFile("../../../Files/Caja/CP_CAJA_15.txt", "es-PE");
            var dao = new Merge.Dao(null);
            var result = dao.Process(report, dataFile, dataBase, "Test-Bank", "Account");

            var cp15C = (List<InfoContribution>)result["infoContributionPayedComplete"];
            var cp15L = (List<InfoLoan>)result["infoLoanPayedComplete"];
            var cp15RP = (List<Info>)result["infoReturnPayment"];

            Contribution(cp15C, size: 1, contributionPaymentId: 115851101, amountPayed: 34.85M, stateId: (int)ContributionState.Pagado);
            Loan(cp15L, loanId: 115851101, size: 1, monthlyQuota: 327.78M, monthlyPayed: 327.78M, stateId: (int)LoanState.Pagado);
            ReturnPayment(cp15RP, customerId: 115851101, totalPayed: 15.15M);
        }

        /// <summary>
        ///---------------------------------------------------------------------------------
        ///|     Contribution  |       Loan0       |       Loan1       |       Loan2       |
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   Total   |   tCrtb   |   tLoan   |        Crtb        |        Loan                         |   Return 
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|  34.85 |  50.00   |                   | 327.78 |  500.00  |                   |   550.00  |   34.85   |   327.78  | Pago total = 1     | Pago total = 1                      | Retorno = 1 (187.37)
        /// </summary>
        [TestMethod]
        public void CP_CAJA_16()
        {
            Init();
            var report = new Report() { ParentKey = Guid.NewGuid() };

            List<Info> dataFile = InfoService.ReadFile("../../../Files/Caja/CP_CAJA_16.txt", "es-PE");
            var dao = new Merge.Dao(null);
            var result = dao.Process(report, dataFile, dataBase, "Test-Bank", "Account");

            var cp16C = (List<InfoContribution>)result["infoContributionPayedComplete"];
            var cp16L = (List<InfoLoan>)result["infoLoanPayedComplete"];
            var cp16RP = (List<Info>)result["infoReturnPayment"];

            Contribution(cp16C, size: 1, contributionPaymentId: 115851101, amountPayed: 34.85M, stateId: (int)ContributionState.Pagado);
            Loan(cp16L, loanId: 115851101, size: 1, monthlyQuota: 327.78M, monthlyPayed: 327.78M, stateId: (int)LoanState.Pagado);
            ReturnPayment(cp16RP, customerId: 115851101, totalPayed: 187.37M);
        }

        /// <summary>
        ///---------------------------------------------------------------------------------
        ///|     Contribution  |       Loan0       |       Loan1       |       Loan2       |
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   Total   |   tCrtb   |   tLoan   |        Crtb        |        Loan                         |   Return 
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|  34.85 |  10.00   |                   |        |          | 384.10 |   0.00   |   10.00   |   10.00   |    0.00   | Pago Parcial = 1   | Sin Liquidez = 2  & Cuota Nueva = 2 (384.10)
        /// </summary>
        [TestMethod]
        public void CP_CAJA_17()
        {
            Init();
            var report = new Report() { ParentKey = Guid.NewGuid() };

            List<Info> dataFile = InfoService.ReadFile("../../../Files/Caja/CP_CAJA_17.txt", "es-PE");
            var dao = new Merge.Dao(null);
            var result = dao.Process(report, dataFile, dataBase, "Test-Bank", "Account");

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
        ///|  34.85 |  10.00   |                   |        |          | 384.10 |   50.00  |   60.00   |   34.85   |   25.15   | Pago Total = 1     | Pago Parcial = 1  & Cuota Nueva = 2 (384.10)
        /// </summary>
        [TestMethod]
        public void CP_CAJA_18()
        {
            Init();
            var report = new Report() { ParentKey = Guid.NewGuid() };

            List<Info> dataFile = InfoService.ReadFile("../../../Files/Caja/CP_CAJA_18.txt", "es-PE");
            var dao = new Merge.Dao(null);
            var result = dao.Process(report, dataFile, dataBase, "Test-Bank", "Account");

            var cp18C = (List<InfoContribution>)result["infoContributionPayedComplete"];
            var cp18LPP = (List<InfoLoan>)result["infoLoanIncomplete"];
            var cp18LNC = (List<InfoLoan>)result["infoLoanNoCash"];
            var cp18NQ = (List<InfoLoan>)result["infoLoanNextQuota"];

            Contribution(cp18C, size: 1, contributionPaymentId: 115851102, amountPayed: 34.85M, stateId: (int)ContributionState.Pagado);

            LoanPayment(cp18LPP, 115851102, 115851102, 1, 327.78M, 25.15M, (int)LoanState.PagoParcial);
            LoanPayment(cp18LNC, 115851102, 115851103, 1, 56.32M, 0M, (int)LoanState.SinLiquidez);

            NextPayment(cp18NQ, cp18LPP, loanId: 115851102, size: 2, monthlyQuota: 302.63M + 56.32M);
        }

        /// <summary>
        ///---------------------------------------------------------------------------------
        ///|     Contribution  |       Loan0       |       Loan1       |       Loan2       |
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   Total   |   tCrtb   |   tLoan   |        Crtb        |        Loan                         |   Return 
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|  34.85 |  10.00   |                   |        |          | 384.10 |  327.78  |  337.78   |   34.85   |   302.93  | Pago Total = 1     | Pago Parcial = 1 & Pago sin liquidez = 1  & Cuota Nueva = 2 (81.17)
        /// </summary>
        [TestMethod]
        public void CP_CAJA_19()
        {
            Init();
            var report = new Report() { ParentKey = Guid.NewGuid() };

            List<Info> dataFile = InfoService.ReadFile("../../../Files/Caja/CP_CAJA_19.txt", "es-PE");
            var dao = new Merge.Dao(null);
            var result = dao.Process(report, dataFile, dataBase, "Test-Bank", "Account");

            var cp19C = (List<InfoContribution>)result["infoContributionPayedComplete"];
            var cp19LPP = (List<InfoLoan>)result["infoLoanIncomplete"];
            var cp19LNC = (List<InfoLoan>)result["infoLoanNoCash"];
            var cp19NQ = (List<InfoLoan>)result["infoLoanNextQuota"];

            Contribution(cp19C, size: 1, contributionPaymentId: 115851102, amountPayed: 34.85M, stateId: (int)ContributionState.Pagado);

            LoanPayment(cp19LPP, 115851102, 115851102, 1, 327.78M, 302.93M, (int)LoanState.PagoParcial);
            LoanPayment(cp19LNC, 115851102, 115851103, 1, 56.32M, 0M, (int)LoanState.SinLiquidez);

            NextPayment(cp19NQ, cp19LPP, loanId: 115851102, size: 2, monthlyQuota: 327.78M - 302.93M + 56.32M);
        }

        /// <summary>
        ///---------------------------------------------------------------------------------
        ///|     Contribution  |       Loan0       |       Loan1       |       Loan2       |
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   Total   |   tCrtb   |   tLoan   |        Crtb        |        Loan                         |   Return 
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|  34.85 |  10.00   |                   |        |          | 384.10 |  384.10  |  394.10   |   34.85   |   359.25  | Pago Total = 1     | Pago total = 1 & Pago parcial = 1  & Cuota Nueva = 1 (24.85)
        /// </summary>
        [TestMethod]
        public void CP_CAJA_20()
        {
            Init();
            var report = new Report() { ParentKey = Guid.NewGuid() };

            List<Info> dataFile = InfoService.ReadFile("../../../Files/Caja/CP_CAJA_20.txt", "es-PE");
            var dao = new Merge.Dao(null);
            var result = dao.Process(report, dataFile, dataBase, "Test-Bank", "Account");

            var cp20C = (List<InfoContribution>)result["infoContributionPayedComplete"];

            var cp20PC = (List<InfoLoan>)result["infoLoanPayedComplete"];
            var cp20LPP = (List<InfoLoan>)result["infoLoanIncomplete"]; 
            var cp20NQ = (List<InfoLoan>)result["infoLoanNextQuota"];

            Contribution(cp20C, size: 1, contributionPaymentId: 115851102, amountPayed: 34.85M, stateId: (int)ContributionState.Pagado);

            LoanPayment(cp20PC, 115851102, 115851102, 1, 327.78M, 327.78M, (int)LoanState.Pagado);
            LoanPayment(cp20LPP, 115851102, 115851103, 1, 56.32M, 31.47M, (int)LoanState.PagoParcial);

            NextPayment(cp20NQ, cp20LPP, loanId: 115851102, size: 1, monthlyQuota: 56.32M - (359.25M - 327.78M));
        }

        /// <summary>
        ///---------------------------------------------------------------------------------
        ///|     Contribution  |       Loan0       |       Loan1       |       Loan2       |
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   Total   |   tCrtb   |   tLoan   |        Crtb        |        Loan                         |   Return 
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|  34.85 |  10.00   |                   |        |          | 384.10 |  500.00  |  510.00   |   34.85   |   384.10  | Pago Total = 1     | Pago total = 2                      | Devolucion de 91.05
        /// </summary>
        [TestMethod]
        public void CP_CAJA_21()
        {
            Init();
            var report = new Report() { ParentKey = Guid.NewGuid() };

            List<Info> dataFile = InfoService.ReadFile("../../../Files/Caja/CP_CAJA_21.txt", "es-PE");
            var dao = new Merge.Dao(null);
            var result = dao.Process(report, dataFile, dataBase, "Test-Bank", "Account");

            var cp21C = (List<InfoContribution>)result["infoContributionPayedComplete"];

            var cp21PC = (List<InfoLoan>)result["infoLoanPayedComplete"];
            var cp21LPC = (List<InfoLoan>)result["infoLoanPayedComplete"];
            var cp21RP = (List<Info>)result["infoReturnPayment"];

            Contribution(cp21C, size: 1, contributionPaymentId: 115851102, amountPayed: 34.85M, stateId: (int)ContributionState.Pagado);

            LoanPayment(cp21PC, 115851102, 115851102, 2, 327.78M, 327.78M, (int)LoanState.Pagado);
            LoanPayment(cp21PC, 115851102, 115851103, 2, 56.32M, 56.32M, (int)LoanState.Pagado);

            ReturnPayment(cp21RP, customerId: 115851102, totalPayed: 91.05M);
        }

        /// <summary>
        ///---------------------------------------------------------------------------------
        ///|     Contribution  |       Loan0       |       Loan1       |       Loan2       |
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   Total   |   tCrtb   |   tLoan   |        Crtb        |        Loan                         |   Return 
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|  34.85 |  34.85   |                   |        |          | 384.10 |   0.00   |  34.85    |   34.85   |   0.00    | Pago Total = 1     | Sin Liquidez = 2 & Nueva Cuota = 2 (384.10)
        /// </summary>
        [TestMethod]
        public void CP_CAJA_22()
        {
            Init();
            var report = new Report() { ParentKey = Guid.NewGuid() };

            List<Info> dataFile = InfoService.ReadFile("../../../Files/Caja/CP_CAJA_22.txt", "es-PE");
            var dao = new Merge.Dao(null);
            var result = dao.Process(report, dataFile, dataBase, "Test-Bank", "Account");

            var cp22C = (List<InfoContribution>)result["infoContributionPayedComplete"];
            var cp22L = (List<InfoLoan>)result["infoLoanNoCash"];
            var cp22NP = (List<InfoLoan>)result["infoLoanNextQuota"];

            Contribution(cp22C, size: 1, contributionPaymentId: 115851102, amountPayed: 34.85M, stateId: (int)ContributionState.Pagado);

            LoanPayment(cp22L, 115851102, 115851102, 2, 327.78M, 0M, (int)LoanState.SinLiquidez);
            LoanPayment(cp22L, 115851102, 115851103, 2, 56.32M, 0M, (int)LoanState.SinLiquidez);

            NextPayment(cp22NP, cp22L, loanId: 115851102, size:2, monthlyQuota: 384.10M);
            
        }

        /// <summary>
        ///---------------------------------------------------------------------------------
        ///|     Contribution  |       Loan0       |       Loan1       |       Loan2       |
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   Total   |   tCrtb   |   tLoan   |        Crtb        |        Loan                         |   Return 
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|  34.85 |  34.85   |                   |        |          | 384.10 |  50.00   |  84.85    |   34.85   |   50.00   | Pago Total = 1     | Pago Parcial = 1 & Sin Liquidez  = 1 & Cuota nueva = 2 (334.10)
        /// </summary>
        [TestMethod]
        public void CP_CAJA_23()
        {
            Init();
            var report = new Report() { ParentKey = Guid.NewGuid() };

            List<Info> dataFile = InfoService.ReadFile("../../../Files/Caja/CP_CAJA_23.txt", "es-PE");
            var dao = new Merge.Dao(null);
            var result = dao.Process(report, dataFile, dataBase, "Test-Bank", "Account");

            var cp23C = (List<InfoContribution>)result["infoContributionPayedComplete"];
            var cp23PP = (List<InfoLoan>)result["infoLoanIncomplete"];
            var cp23NC = (List<InfoLoan>)result["infoLoanNoCash"];
            var cp23NP = (List<InfoLoan>)result["infoLoanNextQuota"];

            Contribution(cp23C, size: 1, contributionPaymentId: 115851102, amountPayed: 34.85M, stateId: (int)ContributionState.Pagado);

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
        ///|  34.85 |  34.85   |                   |        |          | 384.10 | 327.78   |  362.63   |   34.85   |  327.78   | Pago Total = 1     | Pago Total = 1 & Sin Liquidez  = 1 & Cuota nueva = 1 (56.32)
        /// </summary>
        [TestMethod]
        public void CP_CAJA_24()
        {
            Init();
            var report = new Report() { ParentKey = Guid.NewGuid() };

            List<Info> dataFile = InfoService.ReadFile("../../../Files/Caja/CP_CAJA_24.txt", "es-PE");
            var dao = new Merge.Dao(null);
            var result = dao.Process(report, dataFile, dataBase, "Test-Bank", "Account");

            var cp24C = (List<InfoContribution>)result["infoContributionPayedComplete"];
            var cp24L = (List<InfoLoan>)result["infoLoanPayedComplete"];
            var cp24NC = (List<InfoLoan>)result["infoLoanNoCash"];
            var cp24NP = (List<InfoLoan>)result["infoLoanNextQuota"];

            Contribution(cp24C, size: 1, contributionPaymentId: 115851102, amountPayed: 34.85M, stateId: (int)ContributionState.Pagado);

            LoanPayment(cp24L, 115851102, 115851102, 1, 327.78M, 327.78M,(int)LoanState.Pagado);
            LoanPayment(cp24NC, 115851102, 115851103, 1, 56.32M, 0M, (int)LoanState.SinLiquidez);

            NextPayment(cp24NP, cp24NC, loanId: 115851102, size: 1, monthlyQuota: 56.32M);

        }

        /// <summary>
        ///---------------------------------------------------------------------------------
        ///|     Contribution  |       Loan0       |       Loan1       |       Loan2       |
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   Total   |   tCrtb   |   tLoan   |        Crtb        |        Loan                         |   Return 
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|  34.85 |  34.85   |                   |        |          | 384.10 | 384.10   |   418.95  |   34.85   |  384.10   | Pago Total = 1     | Pago Total = 2 
        /// </summary>
        [TestMethod]
        public void CP_CAJA_25()
        {
            Init();
            var report = new Report() { ParentKey = Guid.NewGuid() };

            List<Info> dataFile = InfoService.ReadFile("../../../Files/Caja/CP_CAJA_25.txt", "es-PE");
            var dao = new Merge.Dao(null);
            var result = dao.Process(report, dataFile, dataBase, "Test-Bank", "Account");

            var cp25C = (List<InfoContribution>)result["infoContributionPayedComplete"];
            var cp25L = (List<InfoLoan>)result["infoLoanPayedComplete"];
 

            Contribution(cp25C, size: 1, contributionPaymentId: 115851102, amountPayed: 34.85M, stateId: (int)ContributionState.Pagado);

            LoanPayment(cp25L, 115851102, 115851102, 2, 327.78M, 327.78M, (int)LoanState.Pagado);
            LoanPayment(cp25L, 115851102, 115851103, 2, 56.32M, 56.32M, (int)LoanState.Pagado); 

        }

        /// <summary>
        ///---------------------------------------------------------------------------------
        ///|     Contribution  |       Loan0       |       Loan1       |       Loan2       |
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   Total   |   tCrtb   |   tLoan   |        Crtb        |        Loan                         |   Return 
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|  34.85 |  34.85   |                   |        |          | 384.10 | 500.00   |   418.95  |   34.85   |  384.10   | Pago Total = 1     | Pago Total = 2                      | Devolucion de 115.9
        /// </summary>
        [TestMethod]
        public void CP_CAJA_26()
        {
            Init();
            var report = new Report() { ParentKey = Guid.NewGuid() };

            List<Info> dataFile = InfoService.ReadFile("../../../Files/Caja/CP_CAJA_26.txt", "es-PE");
            var dao = new Merge.Dao(null);
            var result = dao.Process(report, dataFile, dataBase, "Test-Bank", "Account");

            var cp26C = (List<InfoContribution>)result["infoContributionPayedComplete"];
            var cp26L = (List<InfoLoan>)result["infoLoanPayedComplete"];
            var cp26DV = (List<Info>)result["infoReturnPayment"];

            Contribution(cp26C, size: 1, contributionPaymentId: 115851102, amountPayed: 34.85M, stateId: (int)ContributionState.Pagado);

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
        ///|  34.85 |  50.00   |                   |        |          | 384.10 |   0.0    |   50.00   |   34.85   |  15.15   | Pago Total = 1      | Pago Parcial = 1 & Sin liquidez = 1
        /// </summary>
        [TestMethod]
        public void CP_CAJA_27()
        {
            Init();
            var report = new Report() { ParentKey = Guid.NewGuid() };

            List<Info> dataFile = InfoService.ReadFile("../../../Files/Caja/CP_CAJA_27.txt", "es-PE");
            var dao = new Merge.Dao(null);
            var result = dao.Process(report, dataFile, dataBase, "Test-Bank", "Account");

            var cp27C = (List<InfoContribution>)result["infoContributionPayedComplete"];
            var cp27PC = (List<InfoLoan>)result["infoLoanIncomplete"];
            var cp27NC = (List<InfoLoan>)result["infoLoanNoCash"];
            var cp27NP = (List<InfoLoan>)result["infoLoanNextQuota"];


            Contribution(cp27C, size: 1, contributionPaymentId: 115851102, amountPayed: 34.85M, stateId: (int)ContributionState.Pagado);

            LoanPayment(cp27PC, 115851102, 115851102, 1, 327.78M, 15.15M, (int)LoanState.PagoParcial);
            LoanPayment(cp27NC, 115851102, 115851103, 1, 56.32M, 0M, (int)LoanState.SinLiquidez);

            NextPayment(cp27NP, cp27NC, loanId: 115851102, size: 2, monthlyQuota: 368.95M);

        }

        /// <summary>
        ///---------------------------------------------------------------------------------
        ///|     Contribution  |       Loan0       |       Loan1       |       Loan2       |
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   Total   |   tCrtb   |   tLoan   |        Crtb        |        Loan                         |   Return 
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|  34.85 |  50.00   |                   |        |          | 384.10 |   50.00  |  100.00   |   34.85   |  65.15    | Pago Total = 1      | Pago Parcial = 1 & Sin liquidez = 1 & Cuota siguiente = 2 (318.95)
        /// </summary>
        [TestMethod]
        public void CP_CAJA_28()
        {
            Init();
            var report = new Report() { ParentKey = Guid.NewGuid() };

            List<Info> dataFile = InfoService.ReadFile("../../../Files/Caja/CP_CAJA_28.txt", "es-PE");
            var dao = new Merge.Dao(null);
            var result = dao.Process(report, dataFile, dataBase, "Test-Bank", "Account");

            var cp28C = (List<InfoContribution>)result["infoContributionPayedComplete"];
            var cp28PC = (List<InfoLoan>)result["infoLoanIncomplete"];
            var cp28NC = (List<InfoLoan>)result["infoLoanNoCash"];
            var cp28NP = (List<InfoLoan>)result["infoLoanNextQuota"];


            Contribution(cp28C, size: 1, contributionPaymentId: 115851102, amountPayed: 34.85M, stateId: (int)ContributionState.Pagado);

            LoanPayment(cp28PC, 115851102, 115851102, 1, 327.78M, 65.15M, (int)LoanState.PagoParcial);
            LoanPayment(cp28NC, 115851102, 115851103, 1, 56.32M, 0M, (int)LoanState.SinLiquidez);

            NextPayment(cp28NP, cp28NC, loanId: 115851102, size: 2, monthlyQuota: 318.95M);

        }

        /// <summary>
        ///---------------------------------------------------------------------------------
        ///|     Contribution  |       Loan0       |       Loan1       |       Loan2       |
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   Total   |   tCrtb   |   tLoan   |        Crtb        |        Loan                         |   Return 
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|  34.85 |  50.00   |                   |        |          | 384.10 |  327.78  |  377.78   |   34.85   |  342.93   | Pago Total = 1      | Pago Total = 1 & Pago Parcial = 1 & Cuota siguiente = 1 (41.17)
        /// </summary>
        [TestMethod]
        public void CP_CAJA_29()
        {
            Init();
            var report = new Report() { ParentKey = Guid.NewGuid() };

            List<Info> dataFile = InfoService.ReadFile("../../../Files/Caja/CP_CAJA_29.txt", "es-PE");
            var dao = new Merge.Dao(null);
            var result = dao.Process(report, dataFile, dataBase, "Test-Bank", "Account");

            var cp29C = (List<InfoContribution>)result["infoContributionPayedComplete"];
            var cp29PC = (List<InfoLoan>)result["infoLoanPayedComplete"];
            var cp29PP = (List<InfoLoan>)result["infoLoanIncomplete"];
            var cp29NP = (List<InfoLoan>)result["infoLoanNextQuota"];


            Contribution(cp29C, size: 1, contributionPaymentId: 115851102, amountPayed: 34.85M, stateId: (int)ContributionState.Pagado);

            LoanPayment(cp29PC, 115851102, 115851102, 1, 327.78M, 327.78M, (int)LoanState.Pagado);
            LoanPayment(cp29PP, 115851102, 115851103, 1, 56.32M, 15.15M, (int)LoanState.PagoParcial);

            NextPayment(cp29NP, cp29PP, loanId: 115851102, size: 1, monthlyQuota: 41.17M);

        }

        /// <summary>
        ///---------------------------------------------------------------------------------
        ///|     Contribution  |       Loan0       |       Loan1       |       Loan2       |
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   Total   |   tCrtb   |   tLoan   |        Crtb        |        Loan                         |   Return 
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|  34.85 |  50.00   |                   |        |          | 384.10 |  384.10  |  434.10   |   34.85   |  384.10   | Pago Total = 1      | Pago Total = 2   | Devolucion = 1 (15.15)
        /// </summary>
        [TestMethod]
        public void CP_CAJA_30()
        {
            Init();
            var report = new Report() { ParentKey = Guid.NewGuid() };

            List<Info> dataFile = InfoService.ReadFile("../../../Files/Caja/CP_CAJA_30.txt", "es-PE");
            var dao = new Merge.Dao(null);
            var result = dao.Process(report, dataFile, dataBase, "Test-Bank", "Account");

            var cp30C = (List<InfoContribution>)result["infoContributionPayedComplete"];
            var cp30PC = (List<InfoLoan>)result["infoLoanPayedComplete"];
            var cp30DV = (List<Info>)result["infoReturnPayment"]; ;

            Contribution(cp30C, size: 1, contributionPaymentId: 115851102, amountPayed: 34.85M, stateId: (int)ContributionState.Pagado);

            LoanPayment(cp30PC, 115851102, 115851102, 2, 327.78M, 327.78M, (int)LoanState.Pagado);
            LoanPayment(cp30PC, 115851102, 115851103, 2, 56.32M, 56.32M, (int)LoanState.Pagado);
            ReturnPayment(cp30DV, customerId: 115851102, totalPayed: 15.15M);
        }

        /// <summary>
        ///---------------------------------------------------------------------------------
        ///|     Contribution  |       Loan0       |       Loan1       |       Loan2       |
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   DB   |   FILE   |   Total   |   tCrtb   |   tLoan   |        Crtb        |        Loan                         |   Return 
        ///----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        ///|  34.85 |  50.00   |                   |        |          | 384.10 |  500.00  |  550.00   |   34.85   |  384.10   | Pago Total = 1      | Pago Total = 2   | Devolucion = 1 (131.05)
        /// </summary>
        [TestMethod]
        public void CP_CAJA_31()
        {
            Init();
            var report = new Report() { ParentKey = Guid.NewGuid() };

            List<Info> dataFile = InfoService.ReadFile("../../../Files/Caja/CP_CAJA_31.txt", "es-PE");
            var dao = new Merge.Dao(null);
            var result = dao.Process(report, dataFile, dataBase, "Test-Bank", "Account");

            var cp31C = (List<InfoContribution>)result["infoContributionPayedComplete"];
            var cp31PC = (List<InfoLoan>)result["infoLoanPayedComplete"];
            var cp31DV = (List<Info>)result["infoReturnPayment"]; ;

            Contribution(cp31C, size: 1, contributionPaymentId: 115851102, amountPayed: 34.85M, stateId: (int)ContributionState.Pagado);

            LoanPayment(cp31PC, 115851102, 115851102, 2, 327.78M, 327.78M, (int)LoanState.Pagado);
            LoanPayment(cp31PC, 115851102, 115851103, 2, 56.32M, 56.32M, (int)LoanState.Pagado);
            ReturnPayment(cp31DV, customerId: 115851102, totalPayed: 131.05M);
        }
    }

}
