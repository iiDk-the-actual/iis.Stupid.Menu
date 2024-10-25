using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace iiMenu.Classes
{
    public class CoroutineManager : MonoBehaviour // Thanks to ShibaGT for helping with the coroutines
    {
        public static CoroutineManager instance = null;

        private void Awake()
        {
            instance = this;
        }

        public static Coroutine RunCoroutine(IEnumerator enumerator)
        {
            return instance.StartCoroutine(enumerator);
        }

        public static void EndCoroutine(Coroutine enumerator)
        {
            instance.StopCoroutine(enumerator);
        }
    }
}
