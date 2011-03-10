using System;
using System.Collections.Generic;
using System.Linq;

namespace AshMind.LightWiki.Web.ViewModels {
    public class MainViewModel {
        public MainViewModel(WikiPageViewModel current, WikiPageHierarchyModel hierarchy) {
            this.Current = current;
            this.Hierachy = hierarchy;
        }

        public WikiPageViewModel Current   { get; private set; }
        public WikiPageHierarchyModel Hierachy { get; private set; }
    }
}