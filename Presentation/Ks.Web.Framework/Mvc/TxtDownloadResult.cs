using System.Text;
using System.Web.Mvc;

namespace Ks.Web.Framework.Mvc
{
    public class TxtDownloadResult: ActionResult
    {
        public TxtDownloadResult(string txt, string fileDownloadName)
        {
            Txt = txt;
            FileDownloadName = fileDownloadName;
        }

        public string FileDownloadName{get;set;}

        public string Txt { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            var sb = new StringBuilder();
            sb.Append(Txt);

            context.HttpContext.Response.Charset = "utf-8";
            context.HttpContext.Response.ContentType = "text/plain";
            context.HttpContext.Response.AddHeader("content-disposition", string.Format("attachment; filename={0}", FileDownloadName));
            context.HttpContext.Response.BinaryWrite(Encoding.UTF8.GetBytes(sb.ToString()));
            context.HttpContext.Response.End();
        }
    }
}