using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RVMUtility.Data
{
    public class Vertices
    {
        public List<Vertex> VertexList { get; set; }
        public Vertices()
        {
            this.VertexList = new List<Vertex>();
        }

        public void AddVertex(string v, string r)
        {

            string[] vArr = Regex.Split(v.Trim(), @"\s+");
            string[] rArr = Regex.Split(r.Trim(), @"\s+");

            if (vArr.Length != 3)
                return;

            double[] vfArr = new double[3];

            double tmp =0;
            for (int i = 0; i < vArr.Length; i++)
            {
                bool result = double.TryParse(vArr[i], out tmp);

                if (result == false)
                    return;

                vfArr[i] = tmp;
            }

            Vertex newVertex = new Vertex()
            {
                X = vfArr[0],
                Y = vfArr[1],
                Z = vfArr[2],
            };

            this.VertexList.Add(newVertex);
        }
    }
}
