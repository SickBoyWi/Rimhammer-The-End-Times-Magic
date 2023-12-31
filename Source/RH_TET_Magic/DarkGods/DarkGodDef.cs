using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using UnityEngine;
using RimWorld;

namespace TheEndTimes_Magic
{
    public class DarkGodDef : ThingDef
    {
        private string symbol;
        private int sacredNumber;
        public int SacredNumber => sacredNumber;
        private string portrait = "";
        private string titles = "";
        private string domains = "";
        private string descriptionLong = "";

        [Unsaved]
        private Texture2D symbolTex;

        public string Portrait => portrait;
        public string Domains => domains;
        public string DescriptionLong => descriptionLong;
        public string Titles => titles;

        public Texture2D Symbol
        {
            get
            {
                if (this.symbolTex == null)
                {
                    this.symbolTex = ContentFinder<Texture2D>.Get(this.symbol, true);
                }
                return this.symbolTex;
            }
        }
    }
}
