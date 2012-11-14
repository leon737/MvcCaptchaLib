/*
ASP.NET MVC Web Application Captcha Library Copyright (C) 2009-2012 Leonid Medyantsev, Leonid Gordo

This library is free software; you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation; 
either version 3.0 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this library; 
if not, write to the Free Software Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA 
*/

using System;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Mvc.Html;

namespace CaptchaLib
{
    public static class HtmlHelperExtensions
    {

        public static IHtmlString Captcha(this HtmlHelper htmlHelper, string name, string actionName, string refreshLabel)
        {
            return Captcha(htmlHelper, name, actionName, null /* controllerName */, refreshLabel, null /* routeValue */);
        }

        public static IHtmlString Captcha(this HtmlHelper htmlHelper, string name,
            string actionName, string refreshLabel, object routeValues)
        {
            return Captcha(htmlHelper, name, actionName, null /* controllerName */, refreshLabel, routeValues);
        }

        public static IHtmlString Captcha(this HtmlHelper htmlHelper, string name,
            string actionName, string controllerName, string refreshLabel)
        {
            return Captcha(htmlHelper, name, null /* routeName */, actionName, controllerName, null /* routeValues */, refreshLabel, null);
        }

        public static IHtmlString Captcha(this HtmlHelper htmlHelper, string name,
            string actionName, string controllerName, string refreshLabel, object routeValues)
        {
            return Captcha(htmlHelper, name, null /* routeName */, actionName, controllerName, routeValues, refreshLabel, null);
        }
        
        public static IHtmlString Captcha(this HtmlHelper htmlHelper, string name,
            string routeName, string actionName, string controllerName,
            object routeValues, string refreshLabel, object htmlAttributes)
        {
            return CaptchaHelper(htmlHelper, name, routeName, actionName, controllerName, routeValues, refreshLabel, null);
        }

        public static IHtmlString CaptchaFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression,
            string actionName, string refreshLabel)
        {
            return CaptchaFor(htmlHelper, expression, actionName, null /* controllerName */, refreshLabel);
        }

        public static IHtmlString CaptchaFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression,
            string actionName, string controllerName, string refreshLabel)
        {
            return CaptchaFor(htmlHelper, expression, actionName, controllerName, null /* routeValues */, refreshLabel, null /* htmlAttributes */);
        }

        public static IHtmlString CaptchaFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression,
            string actionName, string controllerName, object routeValues, string refreshLabel, object htmlAttributes)
        {
            return CaptchaHelper(htmlHelper, ExpressionHelper.GetExpressionText(expression), null /* routeName */, actionName, controllerName, routeValues, refreshLabel, htmlAttributes);
        }

        private static IHtmlString CaptchaHelper(this HtmlHelper htmlHelper, string name,
           string routeName, string actionName, string controllerName,
           object routeValues, string refreshLabel, object htmlAttributes)
        {
            var container = new TagBuilder("div");
            container.MergeAttribute("class", "captchaContainer");

            var image = new TagBuilder("img");
            image.MergeAttribute("src", UrlHelper.GenerateUrl(routeName, actionName, controllerName, (RouteValueDictionary)routeValues, 
                htmlHelper.RouteCollection, htmlHelper.ViewContext.RequestContext, true));
            image.MergeAttribute("border", "0");

            container.InnerHtml = image.ToString(TagRenderMode.SelfClosing);

            var refresh = new TagBuilder("a");
            refresh.MergeAttribute("href", "#");
            refresh.MergeAttribute("class", "newCaptcha");
            refresh.SetInnerText(refreshLabel);

            var br = new TagBuilder("br");

            var textBox = htmlHelper.TextBox(name, "", htmlAttributes);

            return new HtmlString(
                container.ToString(TagRenderMode.Normal) + Environment.NewLine +
                refresh.ToString(TagRenderMode.Normal) + Environment.NewLine +
                br.ToString(TagRenderMode.SelfClosing) + Environment.NewLine +
                textBox + Environment.NewLine);
        }
    }
}
