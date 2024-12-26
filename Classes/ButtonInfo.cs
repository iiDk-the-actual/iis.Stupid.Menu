using System;

namespace iiMenu.Classes
{
    public class ButtonInfo
    {
        public string buttonText = "-";
        public string overlapText = null;

        public string toolTip = "This button doesn't have a tooltip/tutorial.";

        public Action method = null;
        public Action enableMethod = null;
        public Action disableMethod = null;

        public bool enabled = false;
        public bool isTogglable = true;
        public bool label = false;

        public string customBind = null;
    }
}
