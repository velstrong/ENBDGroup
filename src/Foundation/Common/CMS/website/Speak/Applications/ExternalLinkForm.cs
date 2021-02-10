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
    public class ExternalLinkForm : LinkForm
    {
        /// <summary>
        /// The anchor.
        /// </summary>
        protected Edit Anchor;

        /// <summary>
        /// The class.
        /// </summary>
        protected Edit Class;

        /// <summary>
        /// The custom label.
        /// </summary>
        protected Sitecore.Web.UI.HtmlControls.Panel CustomLabel;

        /// <summary>
        /// The custom target.
        /// </summary>
        protected Edit CustomTarget;

        /// <summary>
        /// The target.
        /// </summary>
        protected Combobox Target;

        /// <summary>
        /// The text.
        /// </summary>
        protected Edit Text;

        /// <summary>
        /// The Custom Attributes.
        /// </summary>
        protected Edit CustomAttributes;

        /// <summary>
        /// The title.
        /// </summary>
        protected Edit Title;

        /// <summary>
        /// The url.
        /// </summary>
        protected Edit Url;

        /// <summary> The test button </summary>
        protected Sitecore.Web.UI.HtmlControls.Button Test;

        public ExternalLinkForm()
        {
        }

        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <returns>
        /// The path.
        /// </returns>
        /// <contract>
        ///   <ensures condition="not null" />
        /// </contract>
        private string GetPath()
        {
            string value = this.Url.Value;
            if (value.Length > 0 && value.IndexOf("://", StringComparison.InvariantCulture) < 0 && !value.StartsWith("/", StringComparison.InvariantCulture))
            {
                value = string.Concat("http://", value);
            }
            return value;
        }

        /// <summary>
        /// Called when the listbox has changed.
        /// </summary>
        protected void OnListboxChanged()
        {
            if (this.Target.Value == "Custom")
            {
                this.CustomTarget.Disabled = false;
                this.CustomLabel.Disabled = false;
                return;
            }
            this.CustomTarget.Value = string.Empty;
            this.CustomTarget.Disabled = true;
            this.CustomLabel.Disabled = true;
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
            if (base.LinkType != "external")
            {
                item = string.Empty;
            }
            string empty = string.Empty;
            string str = base.LinkAttributes["target"];
            string linkTargetValue = LinkForm.GetLinkTargetValue(str);
            if (linkTargetValue == "Custom")
            {
                empty = str;
                this.CustomTarget.Disabled = false;
                this.CustomLabel.Disabled = false;
            }
            this.Text.Value = base.LinkAttributes["text"];
            this.Url.Value = item;
            this.Target.Value = linkTargetValue;
            this.CustomTarget.Value = empty;
            this.Class.Value = base.LinkAttributes["class"];
            this.Title.Value = base.LinkAttributes["title"];
            this.CustomAttributes.Value = GetLinkValue(this.GetLink(), "customattributes");
            this.Test.ToolTip = Translate.Text("Open the specified URL in a browser.");
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
            string path = this.GetPath();
            string linkTargetAttributeFromValue = LinkForm.GetLinkTargetAttributeFromValue(this.Target.Value, this.CustomTarget.Value);
            Packet packet = new Packet("link", Array.Empty<string>());
            LinkForm.SetAttribute(packet, "text", this.Text);
            LinkForm.SetAttribute(packet, "linktype", "external");
            LinkForm.SetAttribute(packet, "url", path);
            LinkForm.SetAttribute(packet, "anchor", string.Empty);
            LinkForm.SetAttribute(packet, "title", this.Title);
            LinkForm.SetAttribute(packet, "class", this.Class);
            LinkForm.SetAttribute(packet, "customattributes", this.CustomAttributes);
            LinkForm.SetAttribute(packet, "target", linkTargetAttributeFromValue);
            Context.ClientPage.ClientResponse.SetDialogValue(packet.OuterXml);
            base.OnOK(sender, args);
        }

        /// <summary>
        /// Called when this instance has test.
        /// </summary>
        protected void OnTest()
        {
            string path = this.GetPath();
            if (path.Length > 0)
            {
                Context.ClientPage.ClientResponse.Eval(string.Concat(new string[] { "try {window.open('", path, "', '_blank') } catch(e) { alert('", Translate.Text("An error occured:"), " ' + e.description) }" }));
            }
        }
    }
}