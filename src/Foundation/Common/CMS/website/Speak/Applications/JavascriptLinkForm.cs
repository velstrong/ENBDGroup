using Sitecore;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Shell.Applications.Dialogs;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Pages;
using Sitecore.Web.UI.Sheer;
using Sitecore.Xml;
using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using System.Xml;

namespace LivApp.Foundation.CMS.Speak.Applications
{
    public class JavascriptLinkForm : LinkForm
    {
        /// <summary>
        /// The class.
        /// </summary>
        protected Edit Class;

        /// <summary>
        /// The text.
        /// </summary>
        protected Edit Text;

        /// <summary>
        /// The title.
        /// </summary>
        protected Edit Title;

        /// <summary>
        /// The Custom Attributes.
        /// </summary>
        protected Edit CustomAttributes;

        /// <summary>
        /// The url.
        /// </summary>
        protected Memo Url;

        public JavascriptLinkForm()
        {
        }

        /// <summary>
        /// Raises the load event.
        /// </summary>
        /// <param name="e">
        /// The <see cref="T:System.EventArgs" /> instance containing the event data.
        /// </param>
        /// <remarks>
        /// This method notifies the server control that it should perform actions common to each HTTP
        /// request for the page it is associated with, such as setting up a database query. At this
        /// stage in the page lifecycle, server controls in the hierarchy are created and initialized,
        /// view state is restored, and form controls reflect client-side data. Use the IsPostBack
        /// property to determine whether the page is being loaded in response to a client postback,
        /// or if it is being loaded and accessed for the first time.
        /// </remarks>
        protected override void OnLoad(EventArgs e)
        {
            Assert.ArgumentNotNull(e, "e");
            base.OnLoad(e);
            if (Context.ClientPage.IsEvent)
            {
                return;
            }
            string item = base.LinkAttributes["url"];
            if (base.LinkType != "javascript")
            {
                item = string.Empty;
            }
            this.Text.Value = base.LinkAttributes["text"];
            this.Url.Value = item;
            this.Class.Value = base.LinkAttributes["class"];
            this.Title.Value = base.LinkAttributes["title"];
            this.CustomAttributes.Value = GetLinkValue(this.GetLink(), "customattributes");
        }
        protected virtual string GetLinkValue(string link, string key)
        {
            Assert.ArgumentNotNull(link, "link");
            XmlDocument xmlDocument = XmlUtil.LoadXml(link);
            if (xmlDocument == null)
            {
                return string.Empty;
            }
            XmlNode xmlNodes = xmlDocument.SelectSingleNode("/link");
            if (xmlNodes == null)
            {
                return string.Empty;
            }
            return XmlUtil.GetAttribute(key, xmlNodes);
        }
        /// <summary>
        /// Handles a click on the OK button.
        /// </summary>
        /// <param name="sender">
        /// </param>
        /// <param name="args">
        /// </param>
        /// <remarks>
        /// When the user clicks OK, the dialog is closed by calling
        /// the <see cref="M:Sitecore.Web.UI.Sheer.ClientResponse.CloseWindow">CloseWindow</see> method.
        /// </remarks>
        protected override void OnOK(object sender, EventArgs args)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(args, "args");
            string value = this.Url.Value;
            if (value.Length > 0 && value.IndexOf("javascript:", StringComparison.InvariantCulture) < 0)
            {
                value = string.Concat("javascript:", value);
            }
            Packet packet = new Packet("link", Array.Empty<string>());
            LinkForm.SetAttribute(packet, "text", this.Text);
            LinkForm.SetAttribute(packet, "linktype", "javascript");
            LinkForm.SetAttribute(packet, "url", value);
            LinkForm.SetAttribute(packet, "anchor", string.Empty);
            LinkForm.SetAttribute(packet, "title", this.Title);
            LinkForm.SetAttribute(packet, "class", this.Class);
            LinkForm.SetAttribute(packet, "customattributes", this.CustomAttributes);
            Context.ClientPage.ClientResponse.SetDialogValue(packet.OuterXml);
            base.OnOK(sender, args);
        }
    }
}