/*
 * ii's Stupid Menu  Managers/CoroutineManager.cs
 * Copyright (C) 2025  Goldentrophy Software
 * https://github.com/iiDk-the-actual/iis.Stupid.Menu
 * 
 * Licensed under the GPL-3.0 license
 * https://www.gnu.org/licenses/gpl-3.0.html
 */

﻿using System.Collections;
using UnityEngine;

namespace iiMenu.Managers
{
    public class CoroutineManager : MonoBehaviour // Thanks to ShibaGT for helping with the coroutines
    {
        public static CoroutineManager instance;

        private void Awake() =>
            instance = this;

        public static Coroutine RunCoroutine(IEnumerator enumerator) =>
            instance.StartCoroutine(enumerator);

        public static void EndCoroutine(Coroutine enumerator) =>
            instance.StopCoroutine(enumerator);
    }
}
