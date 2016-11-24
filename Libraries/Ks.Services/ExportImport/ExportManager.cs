using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text;
using Ks.Core.Domain.Directory;
using System.IO;
using Ks.Core;
using Ks.Core.Domain;
using Ks.Core.Domain.Contract;
using Ks.Core.Domain.Customers;
using Ks.Core.Domain.Reports;
using Ks.Services.Customers;
using Ks.Services.Helpers;
using Ks.Services.KsSystems;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Ks.Services.ExportImport
{
    /// <summary>
    /// Export manager
    /// </summary>
    public partial class ExportManager : IExportManager
    {
        #region Fields

        private readonly IKsSystemService _ksSystemService;
        private readonly KsSystemInformationSettings _ksSystemInformationSettings;
        private readonly IWebHelper _webHelper;
        private readonly IDateTimeHelper _dateTimeHelper;

        #endregion

        #region Ctor

        public ExportManager(IKsSystemService ksSystemService, KsSystemInformationSettings ksSystemInformationSettings, IWebHelper webHelper, IDateTimeHelper dateTimeHelper)
        {
            _ksSystemService = ksSystemService;
            _ksSystemInformationSettings = ksSystemInformationSettings;
            _webHelper = webHelper;
            _dateTimeHelper = dateTimeHelper;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Export states to TXT
        /// </summary>
        /// <param name="states">States</param>
        /// <returns>Result in TXT (string) format</returns>
        public virtual string ExportStatesToTxt(IList<StateProvince> states)
        {
            if (states == null)
                throw new ArgumentNullException("states");

            const string SEPARATOR = ",";
            var sb = new StringBuilder();
            foreach (var state in states)
            {
                sb.Append(state.Country.TwoLetterIsoCode);
                sb.Append(SEPARATOR);
                sb.Append(state.Name);
                sb.Append(SEPARATOR);
                sb.Append(state.Abbreviation);
                sb.Append(SEPARATOR);
                sb.Append(state.Published);
                sb.Append(SEPARATOR);
                sb.Append(state.DisplayOrder);
                sb.Append(Environment.NewLine);  //new line
            }
            return sb.ToString();
        }

        public virtual void ExportReportContributionPaymentToXlsx(Stream stream, Customer customer, Contribution contribution, IList<ReportContributionPayment> reportContributionPayment)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            using (var xlPackage = new ExcelPackage(stream))
            {
                // get handle to the existing worksheet
                var worksheet = xlPackage.Workbook.Worksheets.Add("Aportaciones");
                var imagePath = _webHelper.MapPath("/Administration/Content/images/logo.png");
                //Stream binary = new MemoryStream(File.ReadAllBytes(sampleImagesPath + "logo.png"));

                //var logo = Image.FromStream(binary);
                //var picture = worksheet.Drawings.AddPicture("", logo);
                var image = new Bitmap(imagePath);
                var excelImage = worksheet.Drawings.AddPicture("ACMR", image);
                excelImage.From.Column = 0;
                excelImage.From.Row = 0;

                #region Summary
                worksheet.Cells["A6:A8"].Style.Font.Bold = true;
                worksheet.Cells["D6:D8"].Style.Font.Bold = true;

                worksheet.Cells["A6"].Value = "Aportante:";
                worksheet.Cells["B6"].Value = customer.GetFullName();
                worksheet.Cells["A7"].Value = "Dni:";
                worksheet.Cells["B7"].Value = customer.GetGenericAttribute(SystemCustomerAttributeNames.Dni);
                worksheet.Cells["A8"].Value = "N° Adm:";
                worksheet.Cells["B8"].Value = customer.GetGenericAttribute(SystemCustomerAttributeNames.AdmCode);

                worksheet.Cells["D6"].Value = "Monto:";
                worksheet.Cells["F6"].Value = contribution.AmountPayed.ToString("c", new CultureInfo("es-PE"));
                worksheet.Cells["D7"].Value = "Aportante desde:";
                worksheet.Cells["F7"].Value = _dateTimeHelper.ConvertToUserTime(contribution.CreatedOnUtc, TimeZoneInfo.Utc).ToShortDateString();
                worksheet.Cells["D8"].Value = "Ultimo Pago:";
                if (contribution.UpdatedOnUtc.HasValue)
                    worksheet.Cells["F8"].Value = _dateTimeHelper.ConvertToUserTime(contribution.UpdatedOnUtc.Value, TimeZoneInfo.Utc).ToShortDateString();
                #endregion

                #region Leyend

                worksheet.Cells["M3:M8"].Style.Font.Bold = true;
                worksheet.Cells["L3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["L3"].Style.Fill.BackgroundColor.SetColor(GetColor(1, ((int)ContributionState.Pendiente)));
                worksheet.Cells["M3"].Value = ContributionState.Pendiente.ToString();
                worksheet.Cells["L4"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["L4"].Style.Fill.BackgroundColor.SetColor(GetColor(1, ((int)ContributionState.EnProceso)));
                worksheet.Cells["M4"].Value = ContributionState.EnProceso.ToString();
                worksheet.Cells["L5"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["L5"].Style.Fill.BackgroundColor.SetColor(GetColor(1, ((int)ContributionState.PagoParcial)));
                worksheet.Cells["M5"].Value = ContributionState.PagoParcial.ToString();
                worksheet.Cells["L6"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["L6"].Style.Fill.BackgroundColor.SetColor(GetColor(1, ((int)ContributionState.Pagado)));
                worksheet.Cells["M6"].Value = ContributionState.Pagado.ToString() + " Automático";
                worksheet.Cells["L7"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["L7"].Style.Fill.BackgroundColor.SetColor(GetColor(0, ((int)ContributionState.Pagado)));
                worksheet.Cells["M7"].Value = ContributionState.Pagado.ToString() + " Manual";
                worksheet.Cells["L8"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["L8"].Style.Fill.BackgroundColor.SetColor(GetColor(0, ((int)ContributionState.SinLiquidez)));
                worksheet.Cells["M8"].Value = ContributionState.SinLiquidez.ToString();
                #endregion

                //Create Headers and format them 
                var properties = new[]
                    {
                        "Año",
                        "Enero","Febrero","Marzo","Abril","Mayo",
                        "Junio","Julio","Agosto","Setiembre","Octubre",
                        "Noviembre","Diciembre", "Total"
                    };
                for (int i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[10, i + 1].Value = properties[i];
                    worksheet.Cells[10, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[10, i + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(128, 235, 142));
                    worksheet.Cells[10, i + 1].Style.Fill.BackgroundColor.Tint = 0.599993896298105M;
                    worksheet.Cells[10, i + 1].Style.Font.Bold = true;
                }

                int row = 11;
                decimal ene, feb, mar, abr, may, jun, jul, ago, sep, oct, nov, dic, total;
                int t;
                foreach (var p in reportContributionPayment)
                {
                    int col = 1;
                    if (worksheet.Cells[row - 1, col].Value != null && int.TryParse(worksheet.Cells[row - 1, col].Value.ToString(), out t)
                        && Convert.ToInt32(worksheet.Cells[row - 1, col].Value.ToString()) == p.Year)
                        row--;

                    worksheet.Cells[row, col].Value = p.Year;
                    col++;

                    ene = Convert.ToDecimal(worksheet.Cells[row, col].Value) + (p.Ene);
                    if (worksheet.Cells[row, col].Value == null)
                    {
                        worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(GetColor(p.IsAutomatic, p.StateId));
                    }
                    else
                    {
                        if (Convert.ToDecimal(worksheet.Cells[row, col].Value) == 0)
                        {
                            worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(GetColor(p.IsAutomatic, p.StateId));
                        }
                    }
                    worksheet.Cells[row, col].Value = ene;
                    col++;

                    feb = Convert.ToDecimal(worksheet.Cells[row, col].Value) + (p.Feb);
                    if (worksheet.Cells[row, col].Value == null)
                    {
                        worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(GetColor(p.IsAutomatic, p.StateId));
                    }
                    else
                    {
                        if (Convert.ToDecimal(worksheet.Cells[row, col].Value) == 0)
                        {
                            worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(GetColor(p.IsAutomatic, p.StateId));
                        }
                    }
                    worksheet.Cells[row, col].Value = feb;
                    col++;

                    mar = Convert.ToDecimal(worksheet.Cells[row, col].Value) + (p.Mar);
                    if (worksheet.Cells[row, col].Value == null)
                    {
                        worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(GetColor(p.IsAutomatic, p.StateId));
                    }
                    else
                    {
                        if (Convert.ToDecimal(worksheet.Cells[row, col].Value) == 0)
                        {
                            worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(GetColor(p.IsAutomatic, p.StateId));
                        }
                    }
                    worksheet.Cells[row, col].Value = mar;
                    col++;

                    abr = Convert.ToDecimal(worksheet.Cells[row, col].Value) + (p.Abr);
                    if (worksheet.Cells[row, col].Value == null)
                    {
                        worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(GetColor(p.IsAutomatic, p.StateId));
                    }
                    else
                    {
                        if (Convert.ToDecimal(worksheet.Cells[row, col].Value) == 0)
                        {
                            worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(GetColor(p.IsAutomatic, p.StateId));
                        }
                    }
                    worksheet.Cells[row, col].Value = abr;
                    col++;

                    may = Convert.ToDecimal(worksheet.Cells[row, col].Value) + (p.May);
                    if (worksheet.Cells[row, col].Value == null)
                    {
                        worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(GetColor(p.IsAutomatic, p.StateId));
                    }
                    else
                    {
                        if (Convert.ToDecimal(worksheet.Cells[row, col].Value) == 0)
                        {
                            worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(GetColor(p.IsAutomatic, p.StateId));
                        }
                    }
                    worksheet.Cells[row, col].Value = may;
                    col++;

                    jun = Convert.ToDecimal(worksheet.Cells[row, col].Value) + (p.Jun);
                    if (worksheet.Cells[row, col].Value == null)
                    {
                        worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(GetColor(p.IsAutomatic, p.StateId));
                    }
                    else
                    {
                        if (Convert.ToDecimal(worksheet.Cells[row, col].Value) == 0)
                        {
                            worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(GetColor(p.IsAutomatic, p.StateId));
                        }
                    }
                    worksheet.Cells[row, col].Value = jun;
                    col++;

                    jul = Convert.ToDecimal(worksheet.Cells[row, col].Value) + (p.Jul);
                    if (worksheet.Cells[row, col].Value == null)
                    {
                        worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(GetColor(p.IsAutomatic, p.StateId));
                    }
                    else
                    {
                        if (Convert.ToDecimal(worksheet.Cells[row, col].Value) == 0)
                        {
                            worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(GetColor(p.IsAutomatic, p.StateId));
                        }
                    }
                    worksheet.Cells[row, col].Value = jul;
                    col++;

                    ago = Convert.ToDecimal(worksheet.Cells[row, col].Value) + (p.Ago);
                    if (worksheet.Cells[row, col].Value == null)
                    {
                        worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(GetColor(p.IsAutomatic, p.StateId));
                    }
                    else
                    {
                        if (Convert.ToDecimal(worksheet.Cells[row, col].Value) == 0)
                        {
                            worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(GetColor(p.IsAutomatic, p.StateId));
                        }
                    }
                    worksheet.Cells[row, col].Value = ago;
                    col++;

                    sep = Convert.ToDecimal(worksheet.Cells[row, col].Value) + (p.Sep);
                    if (worksheet.Cells[row, col].Value == null)
                    {
                        worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(GetColor(p.IsAutomatic, p.StateId));
                    }
                    else
                    {
                        if (Convert.ToDecimal(worksheet.Cells[row, col].Value) == 0)
                        {
                            worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(GetColor(p.IsAutomatic, p.StateId));
                        }
                    }
                    worksheet.Cells[row, col].Value = sep;
                    col++;

                    oct = Convert.ToDecimal(worksheet.Cells[row, col].Value) + (p.Oct);
                    if (worksheet.Cells[row, col].Value == null)
                    {
                        worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(GetColor(p.IsAutomatic, p.StateId));
                    }
                    else
                    {
                        if (Convert.ToDecimal(worksheet.Cells[row, col].Value) == 0)
                        {
                            worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(GetColor(p.IsAutomatic, p.StateId));
                        }
                    }
                    worksheet.Cells[row, col].Value = oct;
                    col++;

                    nov = Convert.ToDecimal(worksheet.Cells[row, col].Value) + (p.Nov);
                    if (worksheet.Cells[row, col].Value == null)
                    {
                        worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(GetColor(p.IsAutomatic, p.StateId));
                    }
                    else
                    {
                        if (Convert.ToDecimal(worksheet.Cells[row, col].Value) == 0)
                        {
                            worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(GetColor(p.IsAutomatic, p.StateId));
                        }
                    }
                    worksheet.Cells[row, col].Value = nov;
                    col++;

                    dic = Convert.ToDecimal(worksheet.Cells[row, col].Value) + (p.Dic);
                    if (worksheet.Cells[row, col].Value == null)
                    {
                        worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(GetColor(p.IsAutomatic, p.StateId));
                    }
                    else
                    {
                        if (Convert.ToDecimal(worksheet.Cells[row, col].Value) == 0)
                        {
                            worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(GetColor(p.IsAutomatic, p.StateId));
                        }
                    }
                    worksheet.Cells[row, col].Value = dic;
                    col++;

                    total = ene + feb + mar + abr + may + jun + jul + ago + sep + oct + nov + dic;
                    worksheet.Cells[row, col].Value = total;
                    worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(Color.White);
                    col++;

                    row++;
                }


                // we had better add some document properties to the spreadsheet 

                //set some core property values
                //var storeName = _ksSystemService;
                //var storeUrl = _ksSystemInformationSettings.StoreUrl;
                //xlPackage.Workbook.Properties.Title = string.Format("{0} products", storeName);
                //xlPackage.Workbook.Properties.Author = storeName;
                //xlPackage.Workbook.Properties.Subject = string.Format("{0} products", storeName);
                //xlPackage.Workbook.Properties.Keywords = string.Format("{0} products", storeName);
                //xlPackage.Workbook.Properties.Category = "Products";
                //xlPackage.Workbook.Properties.Comments = string.Format("{0} products", storeName);

                // set some extended property values
                //xlPackage.Workbook.Properties.Company = storeName;
                //xlPackage.Workbook.Properties.HyperlinkBase = new Uri(storeUrl);

                // save the new spreadsheet
                xlPackage.Save();
            }
        }

        public virtual void ExportReportLoanPaymentToXlsx(Stream stream, Customer customer, Loan loan, IList<ReportLoanPayment> reportLoanPayment)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            using (var xlPackage = new ExcelPackage(stream))
            {
                // get handle to the existing worksheet
                var worksheet = xlPackage.Workbook.Worksheets.Add("Apoyo Social Económico");
                var imagePath = _webHelper.MapPath("/Administration/Content/images/logo.png");
                //Stream binary = new MemoryStream(File.ReadAllBytes(sampleImagesPath + "logo.png"));

                //var logo = Image.FromStream(binary);
                //var picture = worksheet.Drawings.AddPicture("", logo);
                var image = new Bitmap(imagePath);
                var excelImage = worksheet.Drawings.AddPicture("ACMR", image);
                excelImage.From.Column = 0;
                excelImage.From.Row = 0;

                #region Summary

                worksheet.Cells["A6:A9"].Style.Font.Bold = true;
                worksheet.Cells["D6:D9"].Style.Font.Bold = true;
                worksheet.Cells["G6:G9"].Style.Font.Bold = true;

                worksheet.Cells["A6"].Value = "Aportante:";
                worksheet.Cells["B6"].Value = customer.GetFullName();
                worksheet.Cells["A7"].Value = "Dni:";
                worksheet.Cells["B7"].Value = customer.GetGenericAttribute(SystemCustomerAttributeNames.Dni);
                worksheet.Cells["A8"].Value = "N° Adm:";
                worksheet.Cells["B8"].Value = customer.GetGenericAttribute(SystemCustomerAttributeNames.AdmCode);
                worksheet.Cells["A9"].Value = "Fecha de Solicitud:";
                worksheet.Cells["B9"].Value = _dateTimeHelper.ConvertToUserTime(loan.CreatedOnUtc, DateTimeKind.Utc).ToString(CultureInfo.InvariantCulture);

                worksheet.Cells["D6"].Value = "Plazo:";
                worksheet.Cells["E6"].Value = string.Format("{0} Meses", loan.Period);
                worksheet.Cells["D7"].Value = "Cuota Mensual:";
                worksheet.Cells["E7"].Value = loan.MonthlyQuota.ToString("c", new CultureInfo("es-PE"));
                worksheet.Cells["D8"].Value = "Importe:";
                worksheet.Cells["E8"].Value = loan.LoanAmount.ToString("c", new CultureInfo("es-PE"));
                worksheet.Cells["D9"].Value = "Total a Girar:";
                worksheet.Cells["E9"].Value = loan.TotalToPay.ToString("c", new CultureInfo("es-PE"));

                worksheet.Cells["G6"].Value = "T.E.A:";
                worksheet.Cells["H6"].Value = (loan.Tea/100).ToString("p", new CultureInfo("es-PE"));
                worksheet.Cells["G7"].Value = "Seg Desgravamen:";
                worksheet.Cells["H7"].Value = (loan.Safe/100).ToString("p", new CultureInfo("es-PE"));
                worksheet.Cells["G8"].Value = "Total Intereses:";
                worksheet.Cells["H8"].Value = loan.TotalFeed.ToString("c", new CultureInfo("es-PE"));
                worksheet.Cells["G9"].Value = "Total Desgravamen:";
                worksheet.Cells["H9"].Value = loan.TotalSafe.ToString("c", new CultureInfo("es-PE"));

                #endregion

                //Create Headers and format them 
                var properties = new[]
                    {
                        "Mes","Año","Cuota","Capital","Interes","Cuota Mensual","Estado"
                    };
                for (var i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[11, i + 1].Value = properties[i];
                    worksheet.Cells[11, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[11, i + 1].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(128, 235, 142));
                    worksheet.Cells[11, i + 1].Style.Fill.BackgroundColor.Tint = 0.599993896298105M;
                    worksheet.Cells[11, i + 1].Style.Font.Bold = true;
                }

                var row = 12;
                var totalMonthlyCapital = 0M;
                var totalMonthlyFee = 0M;
                var totalMonthlyQuota = 0M;

                foreach (var p in reportLoanPayment)
                {
                    var col = 1;
                    worksheet.Cells[row, col].Value = p.MonthName;
                    col++;
                    worksheet.Cells[row, col].Value = p.Year;
                    col++;
                    worksheet.Cells[row, col].Value = p.Quota;
                    col++;
                    worksheet.Cells[row, col].Value = p.MonthlyCapital;
                    col++;
                    worksheet.Cells[row, col].Value = p.MonthlyFee;
                    col++;
                    worksheet.Cells[row, col].Value = p.MonthlyQuota;
                    col++;
                    worksheet.Cells[row, col].Value = GetStateName(p.StateId);

                    totalMonthlyCapital += p.MonthlyCapital;
                    totalMonthlyFee += p.MonthlyFee;
                    totalMonthlyQuota += p.MonthlyQuota;

                    row++;
                }

                worksheet.Cells[row, 1].Value = "Total";
                worksheet.Cells[row, 1].Style.Font.Bold = true;
                worksheet.Cells[row, 4].Value = totalMonthlyCapital;
                worksheet.Cells[row, 4].Style.Font.Bold = true;
                worksheet.Cells[row, 5].Value = totalMonthlyFee;
                worksheet.Cells[row, 5].Style.Font.Bold = true;
                worksheet.Cells[row, 6].Value = totalMonthlyQuota;
                worksheet.Cells[row, 6].Style.Font.Bold = true;

                xlPackage.Save();
            }
        }

        #endregion

        #region Utilities

        private Color GetColor(int isAutomatic, int stateId )
        {
            //ContributionState.Pendiente => Color.Gainsboro
            //ContributionState.EnProceso => Color.LightBlue
            //ContributionState.PagoParcial => Color.PaleGreen
            //ContributionState.Pagado.ToString() + " Automático"  => Color.LightPink
            //ContributionState.EnProceso.ToString()+ " Manual"  => Color.PaleGoldenrod

            //#D5D5D5(1)=Pendiente
            //#FFFFd8(2)=En_Proceso
            //#FFDAB4(3)=Pago_Parcial
            //#BDD7EE(4)=Pagado_Automatico
            //#DAFFB4(5)=Pago_Personal
            //#FFB4B4(6)=Sin_Liquidez

            if ((stateId == (int)ContributionState.Pendiente))
                return ColorTranslator.FromHtml("#D5D5D5");
            if (stateId == (int)ContributionState.SinLiquidez)
                return ColorTranslator.FromHtml("#FFB4B4");
            if ((stateId == (int)ContributionState.EnProceso))
                return ColorTranslator.FromHtml("#FFFFd8");
            if ((stateId == (int)ContributionState.PagoParcial))
                return ColorTranslator.FromHtml("#FFDAB4");
            if ((stateId == (int)ContributionState.Pagado) && isAutomatic == 1)
                return ColorTranslator.FromHtml("#BDD7EE");
            if ((stateId == (int)ContributionState.Pagado) && isAutomatic == 0)
                return ColorTranslator.FromHtml("#DAFFB4");

            return ColorTranslator.FromHtml("#D5D5D5");
        }

        private string GetStateName(int stateId)
        {
            switch (stateId)
            {
                case (int)LoanState.EnProceso: return "En Proceso";
                case (int)LoanState.Pendiente: return "Pendiente";
                case (int)LoanState.PagoParcial: return "Pago Parcial";
                case (int)LoanState.Pagado: return "Pagado";
                case (int)LoanState.SinLiquidez: return "Sin Liquidez";
            }
            return "";
        }

        #endregion
    }
}