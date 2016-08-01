using System;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Ks.Web.Framework;


namespace Ks.Admin.Extensions
{
    public static class HtmlExtensions
    {
        public static MvcHtmlString KsField<TModel>(this HtmlHelper<TModel> helper,
            System.Linq.Expressions.Expression<Func<TModel, string>> expression)
        {
            return helper.KsCommonField(expression, helper.TextBoxFor);
        }

        private static MvcHtmlString KsCommonField<TModel, TValue>(this HtmlHelper<TModel> helper,
            System.Linq.Expressions.Expression<Func<TModel, TValue>> expression,
            Func<System.Linq.Expressions.Expression<Func<TModel, TValue>>, MvcHtmlString> editor)
        {
            var sb = new StringBuilder();
            var tr = new TagBuilder("tr");

            sb.Append(tr.ToString(TagRenderMode.StartTag));

            var builder = new TagBuilder("td");
            builder.Attributes.Add("class", "adminTitle");
            sb.Append(builder.ToString(TagRenderMode.StartTag));
            sb.Append(helper.KsLabelFor(expression).ToHtmlString() + ":");
            sb.Append(builder.ToString(TagRenderMode.EndTag));

            builder = new TagBuilder("td");
            builder.Attributes.Add("class", "adminData");
            sb.Append(builder.ToString(TagRenderMode.StartTag));
            sb.Append(editor.Invoke(expression).ToHtmlString());
            sb.Append(helper.ValidationMessageFor(expression).ToHtmlString());
            sb.Append(builder.ToString(TagRenderMode.EndTag));

            sb.Append(tr.ToString(TagRenderMode.EndTag));

            return MvcHtmlString.Create(sb.ToString());
        }
    }
}