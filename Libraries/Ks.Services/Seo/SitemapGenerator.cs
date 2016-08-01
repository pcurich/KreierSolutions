using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Xml;
using Ks.Core;
using Ks.Core.Domain.Catalog;
using Ks.Core.Domain.Common;
using Ks.Core.Domain.Security;

namespace Ks.Services.Seo
{
    /// <summary>
    /// Represents a sitemap generator
    /// </summary>
    public partial class SitemapGenerator : ISitemapGenerator
    {
        #region Fields

        private readonly IKsSystemContext _ksSystemContext;
        private readonly CommonSettings _commonSettings;
        private readonly SecuritySettings _securitySettings;

        private const string DATE_FORMAT = @"yyyy-MM-dd";
        private XmlTextWriter _writer;

        #endregion

        #region Ctor

        public SitemapGenerator(IKsSystemContext ksSystemContext,
            CommonSettings commonSettings,
            SecuritySettings securitySettings)
        {
            this._ksSystemContext = ksSystemContext;
            this._commonSettings = commonSettings;
            this._securitySettings = securitySettings;
        }

        #endregion

        #region Utilities

        protected virtual string GetHttpProtocol()
        {
            return _securitySettings.ForceSslForAllPages ? "https" : "http";
        }

    /// <summary>
        /// Writes the url location to the writer.
        /// </summary>
        /// <param name="url">Url of indexed location (don't put root url information in).</param>
        /// <param name="updateFrequency">Update frequency - always, hourly, daily, weekly, yearly, never.</param>
        /// <param name="lastUpdated">Date last updated.</param>
        protected virtual void WriteUrlLocation(string url, UpdateFrequency updateFrequency, DateTime lastUpdated)
        {
            _writer.WriteStartElement("url");
            string loc = XmlHelper.XmlEncode(url);
            _writer.WriteElementString("loc", loc);
            _writer.WriteElementString("changefreq", updateFrequency.ToString().ToLowerInvariant());
            _writer.WriteElementString("lastmod", lastUpdated.ToString(DATE_FORMAT));
            _writer.WriteEndElement();
        }

        /// <summary>
        /// Method that is overridden, that handles creation of child urls.
        /// Use the method WriteUrlLocation() within this method.
        /// </summary>
        /// <param name="urlHelper">URL helper</param>
        protected virtual void GenerateUrlNodes(UrlHelper urlHelper)
        {
            //home page
            var homePageUrl = urlHelper.RouteUrl("HomePage", null, GetHttpProtocol());
            WriteUrlLocation(homePageUrl, UpdateFrequency.Weekly, DateTime.UtcNow);
            //search products
            var productSearchUrl = urlHelper.RouteUrl("ProductSearch", null, GetHttpProtocol());
            WriteUrlLocation(productSearchUrl, UpdateFrequency.Weekly, DateTime.UtcNow);
            //contact us
            var contactUsUrl = urlHelper.RouteUrl("ContactUs", null, GetHttpProtocol());
            WriteUrlLocation(contactUsUrl, UpdateFrequency.Weekly, DateTime.UtcNow);
            ////news
            //if (_newsSettings.Enabled)
            //{
            //    var url = urlHelper.RouteUrl("NewsArchive", null, GetHttpProtocol());
            //    WriteUrlLocation(url, UpdateFrequency.Weekly, DateTime.UtcNow);
            //}
            ////blog
            //if (_blogSettings.Enabled)
            //{
            //    var url = urlHelper.RouteUrl("Blog", null, GetHttpProtocol());
            //    WriteUrlLocation(url, UpdateFrequency.Weekly, DateTime.UtcNow);
            //}
            ////blog
            //if (_forumSettings.ForumsEnabled)
            //{
            //    var url = urlHelper.RouteUrl("Boards", null, GetHttpProtocol());
            //    WriteUrlLocation(url, UpdateFrequency.Weekly, DateTime.UtcNow);
            //}
            ////categories
            //if (_commonSettings.SitemapIncludeCategories)
            //{
            //    WriteCategories(urlHelper, 0);
            //}
            ////manufacturers
            //if (_commonSettings.SitemapIncludeManufacturers)
            //{
            //    WriteManufacturers(urlHelper);
            //}
            ////products
            //if (_commonSettings.SitemapIncludeProducts)
            //{
            //    WriteProducts(urlHelper);
            //}
            ////topics
            //WriteTopics(urlHelper);
        }

        //protected virtual void WriteCategories(UrlHelper urlHelper, int parentCategoryId)
        //{
        //    var categories = _categoryService.GetAllCategoriesByParentCategoryId(parentCategoryId);
        //    foreach (var category in categories)
        //    {
        //        var url = urlHelper.RouteUrl("Category", new { SeName = category.GetSeName() }, GetHttpProtocol());
        //        WriteUrlLocation(url, UpdateFrequency.Weekly, category.UpdatedOnUtc);

        //        WriteCategories(urlHelper, category.Id);
        //    }
        //}

        //protected virtual void WriteManufacturers(UrlHelper urlHelper)
        //{
        //    var manufacturers = _manufacturerService.GetAllManufacturers();
        //    foreach (var manufacturer in manufacturers)
        //    {
        //        var url = urlHelper.RouteUrl("Manufacturer", new { SeName = manufacturer.GetSeName() }, GetHttpProtocol());
        //        WriteUrlLocation(url, UpdateFrequency.Weekly, manufacturer.UpdatedOnUtc);
        //    }
        //}

        //protected virtual void WriteProducts(UrlHelper urlHelper)
        //{
        //    var products = _productService.SearchProducts(
        //        storeId: _ksSystemContext.CurrentStore.Id,
        //        visibleIndividuallyOnly: true,
        //        orderBy: ProductSortingEnum.CreatedOn);
        //    foreach (var product in products)
        //    {
        //        var url = urlHelper.RouteUrl("Product", new { SeName = product.GetSeName() }, GetHttpProtocol());
        //        WriteUrlLocation(url, UpdateFrequency.Weekly, product.UpdatedOnUtc);
        //    }
        //}

        //protected virtual void WriteTopics(UrlHelper urlHelper)
        //{
        //    var topics = _topicService.GetAllTopics(_ksSystemContext.CurrentStore.Id)
        //        .Where(t => t.IncludeInSitemap)
        //        .ToList();
        //    foreach (var topic in topics)
        //    {
        //        var url = urlHelper.RouteUrl("Topic", new { SeName = topic.GetSeName() }, GetHttpProtocol());
        //        WriteUrlLocation(url, UpdateFrequency.Weekly, DateTime.UtcNow);
        //    }
        //}

        #endregion

        #region Methods

        /// <summary>
        /// This will build an xml sitemap for better index with search engines.
        /// See http://en.wikipedia.org/wiki/Sitemaps for more information.
        /// </summary>
        /// <param name="urlHelper">URL helper</param>
        /// <returns>Sitemap.xml as string</returns>
        public virtual string Generate(UrlHelper urlHelper)
        {
            using (var stream = new MemoryStream())
            {
                Generate(urlHelper, stream);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        /// <summary>
        /// This will build an xml sitemap for better index with search engines.
        /// See http://en.wikipedia.org/wiki/Sitemaps for more information.
        /// </summary>
        /// <param name="urlHelper">URL helper</param>
        /// <param name="stream">Stream of sitemap.</param>
        public virtual void Generate(UrlHelper urlHelper, Stream stream)
        {
            _writer = new XmlTextWriter(stream, Encoding.UTF8);
            _writer.Formatting = Formatting.Indented;
            _writer.WriteStartDocument();
            _writer.WriteStartElement("urlset");
            _writer.WriteAttributeString("xmlns", "http://www.sitemaps.org/schemas/sitemap/0.9");
            _writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
            _writer.WriteAttributeString("xsi:schemaLocation", "http://www.sitemaps.org/schemas/sitemap/0.9 http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd");

            GenerateUrlNodes(urlHelper);

            _writer.WriteEndElement();
            _writer.Close();
        }

        #endregion
    }
}
