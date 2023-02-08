using Game.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FloatingXPNumbers
{
    static class Storage
    {
        public static float floatDuration;

        public static Queue<XPInfo> XPQueue = new Queue<XPInfo>();

        public struct XPInfo
        {
            public string warning;
            public GameObject victim;
            public float duration;
            public UIPopcornTextManager popcornTextManager;

            public XPInfo(string warning, GameObject victim, float duration, UIPopcornTextManager popcornTextManager)
            {
                this.warning = warning;
                this.victim = victim;
                this.duration = duration;
                this.popcornTextManager = popcornTextManager;
            }
        }

        public class LoadSettings
        {
            public float floatDuration;
        }
    }
}
