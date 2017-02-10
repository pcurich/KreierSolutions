using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using HtmlAgilityPack;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.html;
using iTextSharp.tool.xml.parser;
using iTextSharp.tool.xml.pipeline.css;
using iTextSharp.tool.xml.pipeline.end;
using iTextSharp.tool.xml.pipeline.html;
using Image = iTextSharp.tool.xml.html.Image;

namespace Ks.Admin.Extensions
{
    public static class PdfExtensions
    {
        public static byte[] GetPdf(string pHtml)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(pHtml);
            //Convert HTML to well-formed XHTML
            htmlDoc.OptionFixNestedTags = true;
            htmlDoc.OptionOutputAsXml = true;
            htmlDoc.OptionCheckSyntax = true;
            var bodyNode = htmlDoc.DocumentNode;
            pHtml = bodyNode.WriteTo();

            // create a stream that we can write to, in this case a MemoryStream  
            var stream = new MemoryStream();

            // create an iTextSharp Document which is an abstraction of a PDF but **NOT** a PDF  
            var document = new Document(PageSize.A4, 25, 25, 25, 0);

            // create a writer that's bound to our PDF abstraction and our stream  
            var writer = PdfWriter.GetInstance(document, stream);
            writer.CloseStream = false;
            // open the document for writing  
            document.Open();
            document.NewPage();

            // read html data to StringReader  
            var html = new StringReader(pHtml);

            try
            {
                XMLWorkerHelper.GetInstance().ParseXHtml(writer, document, html);
            }
            catch (Exception exception)
            {
                var e = exception.Message;
                throw;
            }


            // close document  
            document.Close();

            // get bytes from stream  

            return stream.ToArray();
        }

        public static byte[] ConvertHtmlToPdf(this string htmlText)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(htmlText);
            //Convert HTML to well-formed XHTML
            htmlDoc.OptionFixNestedTags = true;
            htmlDoc.OptionOutputAsXml = true;
            htmlDoc.OptionCheckSyntax = true;
            var bodyNode = htmlDoc.DocumentNode;
            htmlText = bodyNode.WriteTo();

            //Create a byte array that will hold final PDF
            Byte[] bytes;
            var ms = new MemoryStream();
            var doc = new Document(PageSize.A4, 10, 10, 10, 0);
            var writer = PdfWriter.GetInstance(doc, ms);
            writer.CloseStream = false;
            var srHtml = new StringReader(htmlText);
            //Open the document for writing
            doc.Open();
            //Add support for embeded images
            var tagProcessors = (DefaultTagProcessorFactory) Tags.GetHtmlTagProcessorFactory();
            tagProcessors.RemoveProcessor(HTML.Tag.IMG);
            tagProcessors.AddProcessor(HTML.Tag.IMG, new CustomImageTagProcessor());

            //var arialuniTff = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "ARIALUNI.TTF");
            //FontFactory.Register(arialuniTff);

            var htmlContext = new HtmlPipelineContext(null);
            htmlContext.SetTagFactory(tagProcessors);
            var cssResolver = XMLWorkerHelper.GetInstance().GetDefaultCssResolver(true);
            IPipeline pipeline = new CssResolverPipeline(cssResolver,
                new HtmlPipeline(htmlContext, new PdfWriterPipeline(doc, writer)));

            var worker = new XMLWorker(pipeline, true);
            var xmlParser = new XMLParser(true, worker, Encoding.Unicode);
            xmlParser.Parse(srHtml);
            srHtml.Close();
            doc.Close();
            writer.Close();
            doc.Close();
            bytes = ms.ToArray();
            ms.Close();
            //Now we just need to do something with those bytes.
            //Here I'm writing them to disk but if you were in ASP.Net you might Response.BinaryWrite() them.
            // var testFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "test.pdf");
            //System.IO.File.WriteAllBytes(testFile, bytes);

            return bytes;
        }

        public class CustomImageTagProcessor : Image
        {
            public override IList<IElement> End(IWorkerContext ctx, Tag tag, IList<IElement> currentContent)
            {
                var attributes = tag.Attributes;
                string src;
                if (!attributes.TryGetValue(HTML.Attribute.SRC, out src))
                    return new List<IElement>(1);

                if (string.IsNullOrEmpty(src))
                    return new List<IElement>(1);

                if (src.StartsWith("data:image/", StringComparison.InvariantCultureIgnoreCase))
                {
                    // data:[<MIME-type>][;charset=<encoding>][;base64],<data>
                    var base64Data = src.Substring(src.IndexOf(",") + 1);
                    var imagedata = Convert.FromBase64String(base64Data);
                    var image = iTextSharp.text.Image.GetInstance(imagedata);

                    var list = new List<IElement>();
                    var htmlPipelineContext = GetHtmlPipelineContext(ctx);
                    list.Add(
                        GetCssAppliers()
                            .Apply(
                                new Chunk(
                                    (iTextSharp.text.Image) GetCssAppliers().Apply(image, tag, htmlPipelineContext), 0,
                                    0, true), tag, htmlPipelineContext));
                    return list;
                }
                return base.End(ctx, tag, currentContent);
            }
        }
    }
}