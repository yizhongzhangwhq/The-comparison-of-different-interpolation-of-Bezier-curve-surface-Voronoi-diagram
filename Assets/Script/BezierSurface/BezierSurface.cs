using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.IO;    

public class BezierSurface : MonoBehaviour
{
        [Header("BezierSurface")]
        [SerializeField]
        public Transform Controller_rCRPoints;
        [SerializeField]
        public static List<Vector3> CRpointsPOSS;
        [SerializeField]
        public List<Vector3> Surfacepointpositions;
        Mesh meshh; 
        Vector3[] vertices;
        [SerializeField]
        int[] triangles;
        [SerializeField]
        public int Num_in_V_Direction;// Number of point in V direction
        [SerializeField]
        public int Num_in_U_Direction;// Number of point in u direction
    
        void Start()
        {
            meshh = this.GetComponent<MeshFilter>().mesh;
        }
        void Update()
        {
            MakingListOfPoints();
            CalculateSurfacePoints3();
            MakeMesh();  
        }
        public void MakingListOfPoints()
        {
            CRpointsPOSS = new List<Vector3>();
            for (int i = 0; i < Controller_rCRPoints.childCount; i++)
            {
                CRpointsPOSS.Add(Controller_rCRPoints.GetChild(i).position);
            }

        }
        private Vector3 CalCurve(float t, List<Vector3> PointPos)
        {
            int Counter = 0;
            Vector3 Resultt = new Vector3(0f, 0f, 0f);
            for (int j = 0; j < PointPos.Count; j++)
            {
                float RR = 1;

                if (Counter != 0)
                {

                    for (int i = 1; i <= Counter; i++)
                    {
                        RR *= (PointPos.Count - 1) - (Counter - i);
                        RR /= i;

                    }
                }

                Resultt = Resultt + RR * (float)Math.Pow((1 - t), (PointPos.Count - 1 - Counter)) * (float)Math.Pow(t, Counter) * PointPos[Counter];

                Counter = Counter + 1;
            }
            return Resultt;
        }
        private Vector3 CalSurfacee2(float u, float v, List<Vector3> PointPos)

        {
            List<Vector3> pointsPArt1 = new List<Vector3>();
            List<Vector3> pointsPArt2 = new List<Vector3>();
            List<Vector3> pointsPArt3 = new List<Vector3>();

            for (int i = 0; i < 5; i++)
            {
                pointsPArt1.Add(PointPos[i]);
                pointsPArt2.Add(PointPos[i + 5]);
                pointsPArt3.Add(PointPos[i + 10]);
               
            }

            List<Vector3> Allpointspos = new List<Vector3>();

            Allpointspos.Add(CalCurve(u, pointsPArt1));
            Allpointspos.Add(CalCurve(u, pointsPArt2));
            Allpointspos.Add(CalCurve(u, pointsPArt3));
        
            return CalCurve(v, Allpointspos);
        }
        public void CalculateSurfacePoints3()
        {

            Surfacepointpositions = new List<Vector3>();

            for (int i = 0; i < Num_in_U_Direction; i++)

            {


                for (int j = 0; j < Num_in_V_Direction; j++)
                {

                    float u = i / (float)(Num_in_U_Direction - 1);
                    float v = j / (float)(Num_in_V_Direction - 1);

                    Surfacepointpositions.Add(CalSurfacee2(u, v, CRpointsPOSS));
                }
            }
        }
        public void MakeMesh() 
        {
            MakeProceduralGrid3();
            UpdateMesh();
        }
        public void MakeProceduralGrid3()
        {
            vertices = new Vector3[Num_in_V_Direction * Num_in_U_Direction];
            triangles = new int[(Num_in_V_Direction - 1) * 2 * 3 * (Num_in_U_Direction - 1)];

            for (int i = 0; i < Surfacepointpositions.Count; i++)
            {

                vertices[i] = Surfacepointpositions[i];

            }

            for (int j = 0; j < Num_in_U_Direction - 1; j++)
            {
                for (int i = 0; i < Num_in_V_Direction - 1; i++)
                {

                    triangles[i * 3 + j * (Num_in_V_Direction - 1) * 2 * 3] = i + j * Num_in_V_Direction;
                    triangles[i * 3 + 1 + j * (Num_in_V_Direction - 1) * 2 * 3] = i + 1 + j * Num_in_V_Direction;
                    triangles[i * 3 + 2 + j * (Num_in_V_Direction - 1) * 2 * 3] = i + Num_in_V_Direction + j * Num_in_V_Direction;

                }

                for (int i = 0; i < (Num_in_V_Direction - 1); i++)
                {

                    triangles[((Num_in_V_Direction - 2) * 3 + 3) + i * 3 + j * (Num_in_V_Direction - 1) * 2 * 3] = i + Num_in_V_Direction + 1 + j * Num_in_V_Direction;
                    triangles[((Num_in_V_Direction - 2) * 3 + 3) + i * 3 + 1 + j * (Num_in_V_Direction - 1) * 2 * 3] = i + Num_in_V_Direction + j * Num_in_V_Direction;
                    triangles[((Num_in_V_Direction - 2) * 3 + 3) + i * 3 + 2 + j * (Num_in_V_Direction - 1) * 2 * 3] = i + 1 + j * Num_in_V_Direction;
                }
            }
        }
        public void UpdateMesh()
        {
            meshh.Clear();

            meshh.vertices = vertices;
            meshh.triangles = triangles;
            meshh.RecalculateNormals();
            
            this.GetComponent<MeshFilter>().sharedMesh = meshh;
        }
}

  