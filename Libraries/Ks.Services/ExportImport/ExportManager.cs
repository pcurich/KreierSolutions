using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Text;
using Ks.Core.Domain.Directory;
using System.IO;
using System.IO.Pipes;
using Ks.Core;
using Ks.Core.Domain;
using Ks.Core.Domain.Contract;
using Ks.Core.Domain.Customers;
using Ks.Core.Domain.Report;
using Ks.Services.Customers;
using Ks.Services.Helpers;
using Ks.Services.KsSystems;
using Newtonsoft.Json.Serialization;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
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
                worksheet.Cells["F6"].Value = contribution.AmountTotal.ToString("c", new CultureInfo("es-PE"));
                worksheet.Cells["D7"].Value = "Aportante desde:";
                worksheet.Cells["F7"].Value = _dateTimeHelper.ConvertToUserTime(contribution.CreatedOnUtc, TimeZoneInfo.Local).ToShortDateString();
                worksheet.Cells["D8"].Value = "Ultimo Pago:";
                if (contribution.UpdatedOnUtc.HasValue)
                    worksheet.Cells["F8"].Value = _dateTimeHelper.ConvertToUserTime(contribution.UpdatedOnUtc.Value, TimeZoneInfo.Local).ToShortDateString();
                #endregion

                #region Leyend

                worksheet.Cells["M3:M7"].Style.Font.Bold = true;
                worksheet.Cells["L3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["L3"].Style.Fill.BackgroundColor.SetColor(Color.Gainsboro);
                worksheet.Cells["M3"].Value = ContributionState.Pendiente.ToString();
                worksheet.Cells["L4"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["L4"].Style.Fill.BackgroundColor.SetColor(Color.LightBlue);
                worksheet.Cells["M4"].Value = ContributionState.EnProceso.ToString();
                worksheet.Cells["L5"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["L5"].Style.Fill.BackgroundColor.SetColor(Color.PaleGreen);
                worksheet.Cells["M5"].Value = ContributionState.PagoParcial.ToString();
                worksheet.Cells["L6"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["L6"].Style.Fill.BackgroundColor.SetColor(Color.LightPink);
                worksheet.Cells["M6"].Value = ContributionState.Pagado.ToString() + " Automático";
                worksheet.Cells["L7"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells["L7"].Style.Fill.BackgroundColor.SetColor(Color.PaleGoldenrod);
                worksheet.Cells["M7"].Value = ContributionState.Pagado.ToString() + " Manual";

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
                        && Convert.ToInt32(worksheet.Cells[row-1, col].Value.ToString()) == p.Year)
                        row--;

                    worksheet.Cells[row, col].Value = p.Year;
                    col++;

                    ene = Convert.ToDecimal(worksheet.Cells[row, col].Value) + (p.Ene);
                    if (worksheet.Cells[row, col].Value == null)
                    {
                        worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(getColor(p.IsAutomatic, p.StateId, ene));
                    }
                    else
                    {
                        if (Convert.ToDecimal(worksheet.Cells[row, col].Value) == 0)
                        {
                            worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(getColor(p.IsAutomatic, p.StateId, ene));
                        }
                    }
                    worksheet.Cells[row, col].Value = ene;
                    col++;

                    feb = Convert.ToDecimal(worksheet.Cells[row, col].Value) + (p.Feb);
                    if (worksheet.Cells[row, col].Value == null)
                    {
                        worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(getColor(p.IsAutomatic, p.StateId, feb));
                    }
                    else
                    {
                        if (Convert.ToDecimal(worksheet.Cells[row, col].Value) == 0)
                        {
                            worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(getColor(p.IsAutomatic, p.StateId, feb));
                        }
                    }
                    worksheet.Cells[row, col].Value = feb;
                    col++;

                    mar = Convert.ToDecimal(worksheet.Cells[row, col].Value) + (p.Mar);
                    if (worksheet.Cells[row, col].Value == null)
                    {
                        worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(getColor(p.IsAutomatic, p.StateId, mar));
                    }
                    else
                    {
                        if (Convert.ToDecimal(worksheet.Cells[row, col].Value) == 0)
                        {
                            worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(getColor(p.IsAutomatic, p.StateId, mar));
                        }
                    }
                    worksheet.Cells[row, col].Value = mar;
                    col++;

                    abr = Convert.ToDecimal(worksheet.Cells[row, col].Value) + (  p.Abr  );
                    if (worksheet.Cells[row, col].Value == null)
                    {
                        worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(getColor(p.IsAutomatic, p.StateId, abr));
                    }
                    else
                    {
                        if (Convert.ToDecimal(worksheet.Cells[row, col].Value) == 0)
                        {
                            worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(getColor(p.IsAutomatic, p.StateId, abr));
                        }
                    }
                    worksheet.Cells[row, col].Value = abr;
                    col++;

                    may = Convert.ToDecimal(worksheet.Cells[row, col].Value) + ( p.May );
                    if (worksheet.Cells[row, col].Value == null)
                    {
                        worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(getColor(p.IsAutomatic, p.StateId, may));
                    }
                    else
                    {
                        if (Convert.ToDecimal(worksheet.Cells[row, col].Value) == 0)
                        {
                            worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(getColor(p.IsAutomatic, p.StateId, may));
                        }
                    }
                    worksheet.Cells[row, col].Value = may;
                    col++;

                    jun = Convert.ToDecimal(worksheet.Cells[row, col].Value) + (  p.Jun );
                    if (worksheet.Cells[row, col].Value == null)
                    {
                        worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(getColor(p.IsAutomatic, p.StateId, jun));
                    }
                    else
                    {
                        if (Convert.ToDecimal(worksheet.Cells[row, col].Value) == 0)
                        {
                            worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(getColor(p.IsAutomatic, p.StateId, jun));
                        }
                    }
                    worksheet.Cells[row, col].Value = jun;
                    col++;

                    jul = Convert.ToDecimal(worksheet.Cells[row, col].Value) + ( p.Jul  );
                    if (worksheet.Cells[row, col].Value == null)
                    {
                        worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(getColor(p.IsAutomatic, p.StateId, jul));
                    }
                    else
                    {
                        if (Convert.ToDecimal(worksheet.Cells[row, col].Value) == 0)
                        {
                            worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(getColor(p.IsAutomatic, p.StateId, jul));
                        }
                    }
                    worksheet.Cells[row, col].Value = jul;
                    col++;

                    ago = Convert.ToDecimal(worksheet.Cells[row, col].Value) + (  p.Ago  );
                    if (worksheet.Cells[row, col].Value == null)
                    {
                        worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(getColor(p.IsAutomatic, p.StateId, ago));
                    }
                    else
                    {
                        if (Convert.ToDecimal(worksheet.Cells[row, col].Value) == 0)
                        {
                            worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(getColor(p.IsAutomatic, p.StateId, ago));
                        }
                    }
                    worksheet.Cells[row, col].Value = ago;
                    col++;

                    sep = Convert.ToDecimal(worksheet.Cells[row, col].Value) + ( p.Sep  );
                    if (worksheet.Cells[row, col].Value == null)
                    {
                        worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(getColor(p.IsAutomatic, p.StateId, sep));
                    }
                    else
                    {
                        if (Convert.ToDecimal(worksheet.Cells[row, col].Value) == 0)
                        {
                            worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(getColor(p.IsAutomatic, p.StateId, sep));
                        }
                    }
                    worksheet.Cells[row, col].Value = sep;
                    col++;

                    oct = Convert.ToDecimal(worksheet.Cells[row, col].Value) +( p.Oct );
                    if (worksheet.Cells[row, col].Value == null)
                    {
                        worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(getColor(p.IsAutomatic, p.StateId, oct));
                    }
                    else
                    {
                        if (Convert.ToDecimal(worksheet.Cells[row, col].Value) == 0)
                        {
                            worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(getColor(p.IsAutomatic, p.StateId, oct));
                        }
                    }
                    worksheet.Cells[row, col].Value = oct;
                    col++;

                    nov = Convert.ToDecimal(worksheet.Cells[row, col].Value) + (  p.Nov  );
                    if (worksheet.Cells[row, col].Value == null)
                    {
                        worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(getColor(p.IsAutomatic, p.StateId, nov));    
                    }
                    else
                    {
                        if (Convert.ToDecimal(worksheet.Cells[row, col].Value) == 0)
                        {
                            worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(getColor(p.IsAutomatic, p.StateId, nov));
                        }
                    }
                    worksheet.Cells[row, col].Value = nov;
                    col++;

                    dic = Convert.ToDecimal(worksheet.Cells[row, col].Value) + (  p.Dic);
                    if (worksheet.Cells[row, col].Value == null)
                    {
                        worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(getColor(p.IsAutomatic, p.StateId,dic));
                    }
                    else
                    {
                        if (Convert.ToDecimal(worksheet.Cells[row, col].Value) == 0)
                        {
                            worksheet.Cells[row, col].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[row, col].Style.Fill.BackgroundColor.SetColor(getColor(p.IsAutomatic, p.StateId, dic));
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



        #endregion

        #region Utilities
        private Color getColor(bool isAutomatic, int stateId,decimal amount)
        {
            //ContributionState.Pendiente => Color.Gainsboro
            //ContributionState.EnProceso => Color.LightBlue
            //ContributionState.PagoParcial => Color.PaleGreen
            //ContributionState.Pagado.ToString() + " Automático"  => Color.LightPink
            //ContributionState.EnProceso.ToString()+ " Manual"  => Color.PaleGoldenrod
            if ((stateId == (int)ContributionState.Pendiente))
                return Color.Gainsboro;
            if ((stateId == (int)ContributionState.EnProceso) && amount > 0)
                return Color.LightBlue;
            if ((stateId == (int)ContributionState.PagoParcial) && amount > 0)
                return Color.PaleGreen;
            if ((stateId == (int)ContributionState.Pagado) && isAutomatic && amount > 0)
                return Color.LightPink;
            if ((stateId == (int)ContributionState.Pagado) && !isAutomatic && amount > 0)
                return Color.PaleGoldenrod;
            return Color.White;
        }

        #endregion
    }
}