using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RVMUtility.Data;
using Aspose.ThreeD;
using Aspose.ThreeD.Entities;
using Aspose.ThreeD.Utilities;

namespace RVMUtility.Handler
{
    public class RVMToFBXConverter
    {
        public List<CNT> RootList { get; set; }
        public int[] PartCount { get; set; }
        public Scene scene { get; set; }

        public RVMToFBXConverter()
        {
            this.RootList = new List<CNT>();
            this.PartCount = new int[11];
            this.scene = new Scene();
        }

        public void Prepare(List<CNT> rootlist)
        {
            this.RootList = rootlist;
        }

        public void Convert()
        {
            foreach (CNT cnt in this.RootList)
            {
                this.browserCNT(cnt.Children);
            }
            this.scene.Save(@"C:\Users\araeng\Documents\3dsMax\import\test.fbx", FileFormat.FBX7500Binary);


            foreach (double d in this.degreecheck)
            {
                Console.WriteLine(d);
            }
        }

        private List<PRIM> browserCNT(List<CNT> cntList)
        {
            List<PRIM> primList = new List<PRIM>();
            foreach (CNT cnt in cntList)
            {
                //Console.WriteLine( new string(' ', cnt.Depth)+ cnt.Name);
                if (cnt.Prims.Count > 0)
                {
                    primList.AddRange(cnt.Prims);
                }

                if (cnt.Children.Count > 0)
                {
                    List<PRIM> subPrimList = this.browserCNT(cnt.Children);
                    if (subPrimList.Count > 0)
                        primList.AddRange(subPrimList);
                }


                if (cnt.Depth == 2)
                {
                    Mesh tmp = new Mesh(cnt.Name);

                    int pidx = 0;
                    foreach (PRIM prim in primList)
                    {
                        pidx += this.addObject(tmp, prim, pidx);
                    }

                    Node newNode = new Node(cnt.Name, tmp);

                    this.scene.RootNode.ChildNodes.Add(newNode);

                    primList.Clear();
                }
            }

            return primList;
        }

        private int addObject(Mesh mesh, PRIM prim, int startIndex)
        {
            int pointIdx = 0;
            this.PartCount[prim.Type - 1]++;
            switch (prim.Type)
            {
                case 1:
                    //pointIdx = this.makePyramid(mesh, prim, startIndex);
                    break;
                case 2:
                   // pointIdx = this.makeBox(mesh, prim, startIndex);
                    break;
                case 3:
                    pointIdx = this.makeRTorus(mesh, prim, startIndex);
                    break;
                case 4:
                    //pointIdx = this.makeTorus(mesh, prim, startIndex);
                    break;
                case 5:
                case 6:
                    //pointIdx = this.makeDish(mesh, prim, startIndex);
                    break;
                case 7:
                   // pointIdx = this.makeCylinder(mesh, prim, startIndex);
                    break;
                case 8:
                    //pointIdx = this.makeCylinder(mesh, prim, startIndex);
                    break;
                case 9:
                    //pointIdx = this.makeSphere(mesh, prim, startIndex);
                    break;
                case 10:
                    break;
                case 11:
                    //pointIdx = this.makeGroup(mesh, prim, startIndex);
                    break;

            }

            return pointIdx;
        }

        private Vector4 CalcMatrix(PRIM prim, double x, double y, double z)
        {
            Vector4 v = new Vector4();

            v.x = (prim.Rotation[0, 0] * x) + (prim.Rotation[0, 1] * y) + (prim.Rotation[0, 2] * z) + prim.Rotation[0, 3];
            v.y = (prim.Rotation[1, 0] * x) + (prim.Rotation[1, 1] * y) + (prim.Rotation[1, 2] * z) + prim.Rotation[1, 3];
            v.z = (prim.Rotation[2, 0] * x) + (prim.Rotation[2, 1] * y) + (prim.Rotation[2, 2] * z) + prim.Rotation[2, 3];

            return v;
        }

        List<double> degreecheck = new List<double>();

        // 1
        private int makePyramid(Mesh newMesh, PRIM prim, int startIndex)
        {
            double xblen = prim.Length[0, 0] / 2;
            double xtlen = prim.Length[0, 2] / 2;

            double yblen = prim.Length[0, 1] / 2;
            double ytlen = prim.Length[0, 3] / 2;

            double zlen = prim.Length[1, 2] / 2;

            //Console.WriteLine(string.Format("{0} {1} {2} {3}", xtlen, xblen, ylen, zlen));

            Vector4 v1 = this.CalcMatrix(prim, xtlen, ytlen, zlen);
            Vector4 v2 = this.CalcMatrix(prim, xtlen * -1, ytlen, zlen);
            Vector4 v3 = this.CalcMatrix(prim, xtlen * -1, ytlen * -1, zlen);
            Vector4 v4 = this.CalcMatrix(prim, xtlen, ytlen * -1, zlen);


            Vector4 v5 = this.CalcMatrix(prim, xblen, yblen, zlen * -1);
            Vector4 v6 = this.CalcMatrix(prim, xblen * -1, yblen, zlen * -1);
            Vector4 v7 = this.CalcMatrix(prim, xblen * -1, yblen * -1, zlen * -1);
            Vector4 v8 = this.CalcMatrix(prim, xblen, yblen * -1, zlen * -1);


            List<Vector4> controlPoints = new List<Vector4>();
            controlPoints.Add(v1);
            controlPoints.Add(v2);
            controlPoints.Add(v3);
            controlPoints.Add(v4);
            controlPoints.Add(v5);
            controlPoints.Add(v6);
            controlPoints.Add(v7);
            controlPoints.Add(v8);


            newMesh.ControlPoints.AddRange(controlPoints);
            PolygonBuilder builder = new PolygonBuilder(newMesh);

            builder.Begin();
            for (int i = 0; i < 4; i++)
            {
                builder.AddVertex(startIndex + i);
            }
            builder.End();

            builder.Begin();
            for (int i = 0; i < 4; i++)
            {
                builder.AddVertex(startIndex + i + 4);
            }
            builder.End();


            for (int i = 0; i < 3; i++)
            {
                builder.Begin();
                builder.AddVertex(startIndex + i);
                builder.AddVertex(startIndex + i + 4);
                builder.AddVertex(startIndex + i + 5);
                builder.AddVertex(startIndex + i + 1);
                builder.End();
            }

            builder.Begin();
            builder.AddVertex(startIndex + 3);
            builder.AddVertex(startIndex + 7);
            builder.AddVertex(startIndex + 4);
            builder.AddVertex(startIndex + 0);
            builder.End();

            return controlPoints.Count;
        }
        // 2
        private int makeBox(Mesh newMesh, PRIM prim, int startIndex)
        {
            double xlen = prim.Length[0, 0] / 2;
            double ylen = prim.Length[0, 1] / 2;
            double zlen = prim.Length[0, 2] / 2;

            //Console.WriteLine(string.Format("{0} {1} {2} {3}", xtlen, xblen, ylen, zlen));

            Vector4 v1 = this.CalcMatrix(prim, xlen, ylen, zlen);
            Vector4 v2 = this.CalcMatrix(prim, xlen * -1, ylen, zlen);
            Vector4 v3 = this.CalcMatrix(prim, xlen * -1, ylen * -1, zlen);
            Vector4 v4 = this.CalcMatrix(prim, xlen, ylen * -1, zlen);

            Vector4 v5 = this.CalcMatrix(prim, xlen, ylen, zlen * -1);
            Vector4 v6 = this.CalcMatrix(prim, xlen * -1, ylen, zlen * -1);
            Vector4 v7 = this.CalcMatrix(prim, xlen * -1, ylen * -1, zlen * -1);
            Vector4 v8 = this.CalcMatrix(prim, xlen, ylen * -1, zlen * -1);


            List<Vector4> controlPoints = new List<Vector4>();
            controlPoints.Add(v1);
            controlPoints.Add(v2);
            controlPoints.Add(v3);
            controlPoints.Add(v4);
            controlPoints.Add(v5);
            controlPoints.Add(v6);
            controlPoints.Add(v7);
            controlPoints.Add(v8);


            newMesh.ControlPoints.AddRange(controlPoints);
            PolygonBuilder builder = new PolygonBuilder(newMesh);

            builder.Begin();
            for (int i = 0; i < 4; i++)
            {
                builder.AddVertex(startIndex + i);
            }
            builder.End();

            builder.Begin();
            for (int i = 0; i < 4; i++)
            {
                builder.AddVertex(startIndex + i + 4);
            }
            builder.End();


            for (int i = 0; i < 3; i++)
            {
                builder.Begin();
                builder.AddVertex(startIndex + i);
                builder.AddVertex(startIndex + i + 4);
                builder.AddVertex(startIndex + i + 5);
                builder.AddVertex(startIndex + i + 1);
                builder.End();
            }

            builder.Begin();
            builder.AddVertex(startIndex + 3);
            builder.AddVertex(startIndex + 7);
            builder.AddVertex(startIndex + 4);
            builder.AddVertex(startIndex + 0);
            builder.End();

            return controlPoints.Count;
        }

        private int makeRTorus(Mesh newMesh, PRIM prim, int startIndex)
        {
            double bRadius = prim.Length[0, 0];
            double sRadius = prim.Length[0, 1];
            double radian = prim.Length[0, 3];
            double degree = Math.Round((180 * radian) / Math.PI);

            return 0;
             
        }

        // 5, 6 dish
        private int makeDish(Mesh newMesh, PRIM prim, int startIndex)
        {
            double radius = prim.Length[0, 0];

            double sliceCount = 8;
            double stackCount = 6;

            double phiStep = Math.PI / stackCount;
            double thetaStep = 2.0 * Math.PI / sliceCount;

            List<Vector4> controlPoints = new List<Vector4>();
            for (int i = 0; i < stackCount + 1; i++)
            {
                double phi = i * phiStep;

                for (int j = 0; j < sliceCount; j++)
                {
                    double theta = j * thetaStep;
                    double x = radius * Math.Sin(phi) * Math.Cos(theta);
                    double y = radius * Math.Cos(phi);
                    double z = radius * Math.Sin(phi) * Math.Sin(theta);

                    Vector4 v = this.CalcMatrix(prim, x, y, z);
                    controlPoints.Add(v);

                    if (i == 0 || i == stackCount)
                        break;
                }

                if (stackCount / 2 == i)
                    break;
            }


            newMesh.ControlPoints.AddRange(controlPoints);
            PolygonBuilder builder = new PolygonBuilder(newMesh);
            int sc = (int)sliceCount;

            for (int i = 0; i < sc - 1; i++)
            {
                builder.Begin();
                builder.AddVertex(startIndex + 0);
                builder.AddVertex(startIndex + i + 2);
                builder.AddVertex(startIndex + i + 1);
                builder.End();
            }
            builder.Begin();
            builder.AddVertex(startIndex + 0);
            builder.AddVertex(startIndex + 1);
            builder.AddVertex(startIndex + sc);
            builder.End();


            for (int i = 0; i < stackCount / 3; i++)
            {
                for (int j = 0; j < sc - 1; j++)
                {
                    builder.Begin();
                    builder.AddVertex(startIndex + (i * sc) + j + 1);
                    builder.AddVertex(startIndex + (i * sc) + j + 2);
                    builder.AddVertex(startIndex + (i * sc) + j + sc + 2);
                    builder.AddVertex(startIndex + (i * sc) + j + sc + 1);
                    builder.End();
                }

                builder.Begin();
                builder.AddVertex(startIndex + (i * sc) + 1);
                builder.AddVertex(startIndex + (i * sc) + sc + 1);
                builder.AddVertex(startIndex + (i * sc) + sc + sc);
                builder.AddVertex(startIndex + (i * sc) + sc);
                builder.End();
            }

            return controlPoints.Count;
        }

        // 7, 8 cylinder
        private int makeCylinder(Mesh newMesh, PRIM prim, int startIndex)
        {
            double radiusTop = 0;
            double radiusBottom = radiusTop;
            double height = 0;

            if (prim.Type == 7)
            {
                radiusBottom = prim.Length[0, 0];
                radiusTop = prim.Length[0, 1];
                height = prim.Length[0, 2];
            }
            else if (prim.Type == 8)
            {
                radiusBottom = prim.Length[0, 0];
                radiusTop = prim.Length[0, 0];
                height = prim.Length[0, 1];
            }

            int sliceCount = 8;
            int stackCount = 1;
            int ringCount = stackCount + 1;
            double stackHeight = height / stackCount;
            double radiusStep = (radiusTop - radiusBottom) / stackCount;

            List<Vector4> controlPoints = new List<Vector4>();
            for (int i = 0; i < ringCount; i++)
            {
                double z = (-0.5f * height) + (i * stackHeight);
                double r = radiusBottom + (i * radiusStep);
                double dTheta = (2.0f * Math.PI) / sliceCount;

                for (int j = 0; j < sliceCount; j++)
                {
                    double c = Math.Cos(j * dTheta);
                    double s = Math.Sin(j * dTheta);

                    Vector4 v = this.CalcMatrix(prim, r * c, r * s, z);
                    controlPoints.Add(v);
                }
            }


            newMesh.ControlPoints.AddRange(controlPoints);
            PolygonBuilder builder = new PolygonBuilder(newMesh);
            for (int i = 0; i < sliceCount - 1; i++)
            {
                builder.Begin();
                builder.AddVertex(startIndex + i);
                builder.AddVertex(startIndex + i + 1);
                builder.AddVertex(startIndex + i + sliceCount + 1);
                builder.AddVertex(startIndex + i + sliceCount);
                builder.End();
            }

            builder.Begin();
            builder.AddVertex(startIndex + 0);
            builder.AddVertex(startIndex + sliceCount);
            builder.AddVertex(startIndex + sliceCount + sliceCount - 1);
            builder.AddVertex(startIndex + sliceCount - 1);
            builder.End();


            if (prim.Type == 8)
            {
                builder.Begin();
                for (int i = 0; i < sliceCount; i++)
                {
                    builder.AddVertex(startIndex + sliceCount - 1 - i);
                }
                builder.End();

                builder.Begin();
                for (int i = 0; i < sliceCount; i++)
                {
                    builder.AddVertex(startIndex + sliceCount + i);
                }
                builder.End();
            }

            return controlPoints.Count;
        }

        // 9 sphere
        private int makeSphere(Mesh newMesh, PRIM prim, int startIndex)
        {
            double radius = prim.Length[0, 0];

            double sliceCount = 8;
            double stackCount = 6;

            double phiStep = Math.PI / stackCount;
            double thetaStep = 2.0 * Math.PI / sliceCount;

            List<Vector4> controlPoints = new List<Vector4>();
            for (int i = 0; i < stackCount + 1; i++)
            {
                double phi = i * phiStep;

                for (int j = 0; j < sliceCount; j++)
                {
                    double theta = j * thetaStep;
                    double x = radius * Math.Sin(phi) * Math.Cos(theta);
                    double y = radius * Math.Cos(phi);
                    double z = radius * Math.Sin(phi) * Math.Sin(theta);

                    Vector4 v = this.CalcMatrix(prim, x, y, z);
                    controlPoints.Add(v);

                    if (i == 0 || i == stackCount)
                        break;
                }
            }


            newMesh.ControlPoints.AddRange(controlPoints);
            PolygonBuilder builder = new PolygonBuilder(newMesh);
            int sc = (int)sliceCount;

            for (int i = 0; i < sc - 1; i++)
            {
                builder.Begin();
                builder.AddVertex(startIndex + 0);
                builder.AddVertex(startIndex + i + 2);
                builder.AddVertex(startIndex + i + 1);
                builder.End();
            }
            builder.Begin();
            builder.AddVertex(startIndex + 0);
            builder.AddVertex(startIndex + 1);
            builder.AddVertex(startIndex + sc);
            builder.End();


            for (int i = 0; i < stackCount - 2; i++)
            {
                for (int j = 0; j < sc - 1; j++)
                {
                    builder.Begin();
                    builder.AddVertex(startIndex + (i * sc) + j + 1);
                    builder.AddVertex(startIndex + (i * sc) + j + 2);
                    builder.AddVertex(startIndex + (i * sc) + j + sc + 2);
                    builder.AddVertex(startIndex + (i * sc) + j + sc + 1);
                    builder.End();
                }

                builder.Begin();
                builder.AddVertex(startIndex + (i * sc) + 1);
                builder.AddVertex(startIndex + (i * sc) + sc + 1);
                builder.AddVertex(startIndex + (i * sc) + sc + sc);
                builder.AddVertex(startIndex + (i * sc) + sc);
                builder.End();
            }

            return controlPoints.Count;
        }

        // 11 group
        private int makeGroup(Mesh newMesh, PRIM prim, int startIndex)
        {
            int cp = 0;
            foreach (Vertices vertices in prim.VerticesList)
            {
                if (vertices.VertexList.Count < 3)
                    continue;

                List<Vector4> controlPoints = new List<Vector4>();
                foreach (RVMUtility.Data.Vertex v in vertices.VertexList)
                {
                    Vector4 vector = this.CalcMatrix(prim, v.X, v.Y, v.Z);
                    controlPoints.Add(vector);
                }

                newMesh.ControlPoints.AddRange(controlPoints);
                PolygonBuilder builder = new PolygonBuilder(newMesh);
                builder.Begin();
                for (int i = 0; i < controlPoints.Count; i++)
                {
                    builder.AddVertex(startIndex + i + cp);
                }
                builder.End();
                cp += controlPoints.Count;
            }

            return cp;
        }
    }
}
