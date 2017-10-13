using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RVMUtility.Data
{
    [Serializable]
    public class CNT
    {
        public int Depth { get; set; }

        public string Name { get; set; }

        public double[] XYZ { get; set; }

        [NonSerialized]
        private CNT _Parent;
        public CNT Parent
        {
            get { return this._Parent; }
            set { this._Parent = value; }
        }

        public List<CNT> Children { get; set; }
        public List<PRIM> Prims { get; set; }

        public CNT(string Name)
        {
            this.Name = Name;
            this.Parent = null;
            this.Children = new List<CNT>();
            this.Prims = new List<PRIM>();

            XYZ = new double[3];
        }

        public void AddXYZ(string str)
        {
            str = str.Trim();
            string[] strarr = Regex.Split(str, @"\s+");

            for (int i = 0; i < 3; i++)
            {
                this.XYZ[i] = double.Parse(strarr[i]);
            }
        }

        public void AddCNT(CNT cnt)
        {
            cnt.Parent = this;
            cnt.Depth = this.Depth + 1;
            this.Children.Add(cnt);

            //Console.WriteLine(cnt.Name + ", " + cnt.Parent.Name);

        }

        public void AddPRIM(PRIM prim)
        {
            this.Prims.Add(prim);
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3}", Depth, Children.Count, Prims.Count, Name);
        }
    }
}
