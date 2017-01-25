﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Core.Data;
using System.Windows.Controls;

namespace GetJSON
{
    internal class DynamicSelectLayerMenu : DynamicMenu
    {
        internal delegate void ClickAction(KeyValuePair<BasicFeatureLayer, List<long>> allSelectedfeatures);
        private static Dictionary<BasicFeatureLayer, List<long>> allSelectedfeatures;

        public static void SetItems(Dictionary<BasicFeatureLayer, List<long>> asf)
        {
            allSelectedfeatures = new Dictionary<BasicFeatureLayer, List<long>>(asf);
        }

        protected override void OnPopup()
        {
            this.Add("Select layer", "", false, true, true);
            this.AddSeparator();
            if (allSelectedfeatures == null || allSelectedfeatures.Count == 0)
            {
                this.Add("No features found", "", false, false, true);
                return;
            }

            ClickAction theAction = OnMenuItemClicked;
            foreach (KeyValuePair<BasicFeatureLayer, List<long>> entry in allSelectedfeatures)
            {
                string layername = entry.Key.Name;
                int selCount = entry.Value.Count;
                this.Add($"{layername} ({selCount})", "", false, true, false, theAction, entry);
            }
        }

        private static void OnMenuItemClicked(KeyValuePair<BasicFeatureLayer, List<long>> selectedLayer)
        {
            //reselect and start converting
            QueuedTask.Run(() =>
            {
                foreach (KeyValuePair<BasicFeatureLayer, List<long>> entry in allSelectedfeatures)
                {
                    if (entry.Key.Name != selectedLayer.Key.Name)
                    {
                        //clear this selection
                        entry.Key.ClearSelection();
                    }
                }
            });

        }

    }
}