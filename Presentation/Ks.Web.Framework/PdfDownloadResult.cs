using System.IO;
using System.Text;
using System.Web.Mvc;

namespace Ks.Web.Framework
{
    public class PdfDownloadResult : ActionResult
    {
        public PdfDownloadResult(MemoryStream pdf, string fileDownloadName)
        {
            Pdf = pdf;
            FileDownloadName = fileDownloadName;
        }

        public string FileDownloadName
        {
            get;
            set;
        }

        public MemoryStream Pdf
        {
            get;
            set;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.Charset = "utf-8";
            context.HttpContext.Response.ContentType = "application/pdf";
            context.HttpContext.Response.AddHeader("content-disposition", string.Format("attachment; filename={0}", FileDownloadName));
            context.HttpContext.Response.BinaryWrite(Pdf.ToArray());
            context.HttpContext.Response.End();
        }
    }

}