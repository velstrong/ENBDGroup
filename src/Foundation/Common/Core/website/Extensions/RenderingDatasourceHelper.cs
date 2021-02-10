using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Layouts;
using Sitecore.Rules;
using Sitecore.Rules.Actions;
using Sitecore.Rules.ConditionalRenderings;

namespace ENBDGroup.Foundation.Common.Core.Extensions
{
    public class SetItemAction<T> : RuleAction<T> where T : RuleContext
    {
        private string _dataSource;
        public string DataSource
        {
            get
            {
                return _dataSource ?? string.Empty;
            }
            set
            {
                Assert.ArgumentNotNull(value, "value");
                _dataSource = value;
            }
        }

        public override void Apply(T ruleContext) { }
    }
    public static class RenderingDatasourceHelper
    {
        /// <summary>
        /// Get the Personalised Datasource(s) from a particular Placeholder and View Rendering on an item
        /// </summary>
        /// <param name="device"></param>
        /// <param name="i">Item which has the placeholder on it</param>
        /// <param name="placeholderName">Presentation Settings Placeholder Name</param>
        /// <param name="renderingId">ID of the View Rendering used in this Placeholder</param>
        /// <returns>List of Sitecore Items used as datasources in these View Renderings</returns>
        public static Item GetPersonalizedConfiguration(this Item configuration, string fieldName)
        {
            //The Configuration item
           // var configuration = Sitecore.Context.Database.GetItem(new ID("{A43C3902-CEE6-4422-B09D-CDC4B6806031}"));
        //The field name on the Configuration item that contains the rules
       // var fieldName = "Custom Rules";
        //TemplateId of the "Motivational Message" template
       

        //Create a RuleContext to execute the rules against
        RuleContext ruleContext = new RuleContext();
        //Use Sitecore.Rules.RuleFactory to exctract the rules from the field.
        IEnumerable<Rule<RuleContext>> rules = RuleFactory.GetRules<RuleContext>(new[] { configuration }, fieldName).Rules;
            
            //Evaluate a rule and if something fails, go to the next rule.
            foreach (Rule<RuleContext> rule in rules)
            {
                bool ruleOk = rule.Evaluate(ruleContext);
                if (!ruleOk)
                    continue;
                
                //I'm only executing the first action. Which is always "set data source to Item".
                RuleAction<RuleContext> actionItem = rule.Actions.FirstOrDefault();
                if (actionItem == null)
                    continue;
                
                //Convert the RuleAction to a custom implementation that has a DataSource property.
                //The DataSource property will be automatically populated by the rules engine.
                //Note that this means that the Action defined in Sitecore must have the parameter "DataSource".
                string itemId = ((SetItemAction<RuleContext>)actionItem).DataSource;
        ID sitecoreItemId;
                if (!ID.TryParse(itemId, out sitecoreItemId))
                    continue;

                //Get the item and check if the template matches the one you expect.
                Item datasourceItem = configuration.Database.GetItem(sitecoreItemId);
                if (datasourceItem == null)
                    continue;

                return datasourceItem;
            }
            return null;
        }
public static List<Item> GetPersonalisedPlaceHolderDatasourceItems(this Item i, string placeholderName, ID renderingId)
        {
            DeviceRecords devices = i.Database.Resources.Devices;
            DeviceItem defaultDevice = devices.GetAll().Where(d => d.Name.ToLower() == "default").First();


            var test = i.Visualization.GetRenderings(defaultDevice, false);
            var result = new List<Item>();
            var renderingReferences = GetRenderingReferences(defaultDevice, i, placeholderName, renderingId).ToList();

            result = renderingReferences
                .SelectMany(x => GetPersonalizationDataSourceItem(x, i))
                .Where(y => y != null)
                .ToList();
            return result;
        }

        #region Methods to get the Datasources

        private static IEnumerable<Item> GetPersonalizationDataSourceItem(RenderingReference reference, Item i)
        {
            var list = new List<Item>();

            //if no datasource
            if (reference == null)
            {
                return list;
            }

            //else if no Personalisation is used, return the default datasource
            if (reference.Settings.Rules == null || reference.Settings.Rules.Count <= 0)
            {
                var item = GetDataSourceItem(reference);
                if (item != null)
                {
                    list.Add(item);
                }
                return list;
            }


            //note: doesn't support personalisation of the component with multiple renderings
            var ruleContext = new ConditionalRenderingsRuleContext(new List<RenderingReference> { reference }, reference)
            {
                Item = i
            };

            reference.Settings.Rules.RunFirstMatching(ruleContext);
            list.Add(GetDataSourceItem(reference.Settings.DataSource, reference.Database));

            //else Get the Personalised datasource
            //foreach (var r in reference.Settings.Rules.Rules)
            //{


            //    if (r.Evaluate(ruleContext))
            //    {
            //        list.AddRange(r.Actions
            //            .OfType<SetDataSourceAction<ConditionalRenderingsRuleContext>>()
            //            .Select(setDataSourceAction => GetDataSourceItem(setDataSourceAction.DataSource, reference.Database))
            //            .Where(dataSourceItem => dataSourceItem != null));

            //        return list; //break from loop on first evaluated
            //    }
            //}
            return list;
        }

        private static IEnumerable<RenderingReference> GetRenderingReferences(DeviceItem device, Item i, string placeholderName, ID renderingId)
        {
            //var renderingReference = new List<RenderingReference>();
            //LayoutDefinition layout = LayoutDefinition.Parse(i.Fields[FieldIDs.FinalLayoutField].Value);
            //for (int deviceIndex = layout.Devices.Count - 1; deviceIndex >= 0; deviceIndex--)
            //{
            //    var deviceItem = layout.Devices[deviceIndex] as DeviceDefinition;

            //    if (device == null) continue;

            //    // loop over renderings within the device
            //    for (int renderingIndex = deviceItem.Renderings.Count - 1; renderingIndex >= 0; renderingIndex--)
            //    {
            //        var rendering = deviceItem.Renderings[renderingIndex] as RenderingDefinition;

            //        if(rendering!=null && rendering.Placeholder== placeholderName && rendering.ItemID == renderingId.ToString())
            //        {
            //            renderingReference.Add(rendering.Conditions.Contains())
            //        }
            //    }
            //}
            return i == null
                ? new RenderingReference[0]
                : i.Visualization.GetRenderings(device, false).Where(x => x.Placeholder == placeholderName && x.RenderingID == renderingId);
        }

        private static Item GetDataSourceItem(RenderingReference reference)
        {
            return reference != null
                ? GetDataSourceItem(reference.Settings.DataSource, reference.Database)
                : null;
        }

        private static Item GetDataSourceItem(string id, Database db)
        {
            Guid itemId;
            return Guid.TryParse(id, out itemId)
                                    ? db.GetItem(new ID(itemId))
                                    : db.GetItem(id);
        }
        #endregion
    }
}