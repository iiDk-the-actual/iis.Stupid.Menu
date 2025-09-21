using System;

namespace iiMenu.Classes.Menu
{
    public class ButtonInfo
    {
        public string buttonText = "-";
        public string overlapText;

        public string toolTip = "This button doesn't have a tooltip/tutorial.";

        public Action method;
        public Action enableMethod;
        public Action disableMethod;

        public bool enabled;
        public bool isTogglable = true;

        public bool label;
        public bool incremental;

        public string customBind;
    }
}
