using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;
using System.Text.RegularExpressions;


namespace RVMUtility.Data
{
    [Serializable]
    public class PRIM
    {

        public int Type { get; set; }

        public double[,] Rotation { get; set; }
        public double[,] Bound { get; set; }
        public double[,] Length { get; set; }
        public List<Vertices> VerticesList { get; set; }


        public List<int> verCnt { get; set; }

        public PRIM(string type)
        {
            this.SetType(type);
            this.verCnt = new List<int>();

            Rotation = new double[3, 4];
            Bound = new double[2, 3];
            Length = new double[2, 5];

            VerticesList = new List<Vertices>();
        }

        public void SetRotation(string[] strarr)
        {
            //3줄 
            for (int maini = 0; maini < strarr.Length; maini++)
            {
                string str = strarr[maini].Trim();
                string[] valuesArr = Regex.Split(str, @"\s+");

                for (int subi = 0; subi < valuesArr.Length; subi++)
                {
                    this.Rotation[maini, subi] = double.Parse(valuesArr[subi]);
                }
            }
        }
        public void SetLength(string[] strarr)
        {
            //2줄
            for (int maini = 0; maini < strarr.Length; maini++)
            {
                string str = strarr[maini].Trim();
                string[] valuesArr = Regex.Split(str, @"\s+");

                for (int subi = 0; subi < valuesArr.Length; subi++)
                {
                    this.Length[maini, subi] = double.Parse(valuesArr[subi]);
                }
            }
        }
        public void SetBound(string[] strarr)
        {
            //0, 1, 2 줄
            for (int maini = 0; maini < strarr.Length; maini++)
            {
                string str = strarr[maini].Trim();
                string[] valuesArr = Regex.Split(str, @"\s+");

                for (int subi = 0; subi < valuesArr.Length; subi++)
                {
                    this.Bound[maini, subi] = double.Parse(valuesArr[subi]);
                }
            }
        }

        public void AddVertextCount(int vcnt)
        {
            this.verCnt.Add(vcnt);
        }

        public void AddVertices(Vertices v)
        {
            this.VerticesList.Add(v);
        }

        public void SetType(string typestr)
        {
            int t = -1;

            bool result = int.TryParse(typestr, out t);

            this.Type = t;
        }

        public override string ToString()
        {
            string result = string.Empty;

            if (Type == 11)
            {
                result = string.Format("{0} {1}", Type, verCnt.Count);
            }
            else
            {
                result = string.Format("{0}", Type);
            }

            return result;
        }



        public string GetFaceinfo()
        {
            int[] distinctArr = verCnt.Distinct().ToArray();
            Array.Sort(distinctArr);

            StringBuilder strb = new StringBuilder();
            for (int i = 0; i < distinctArr.Length; i++)
            {
                int cnt = verCnt.Where(x => x == distinctArr[i]).Count();

                strb.AppendFormat("{0}-{1}:", distinctArr[i], cnt);
            }
            return strb.ToString();
        }

        public int[] GetDistinct()
        {
            int[] distinctArr = verCnt.Distinct().ToArray();
            Array.Sort(distinctArr);

            return distinctArr;
        }




    }
}
