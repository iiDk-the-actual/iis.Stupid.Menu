/*
 * ii's Stupid Menu  Classes/Menu/ButtonInfo.cs
 * Copyright (C) 2025  Goldentrophy Software
 * https://github.com/iiDk-the-actual/iis.Stupid.Menu
 * 
 * Licensed under the GPL-3.0 license
 * https://www.gnu.org/licenses/gpl-3.0.html
 */

﻿using System;

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
