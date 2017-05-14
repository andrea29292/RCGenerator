using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralMesh2 : MonoBehaviour {
    public BezierSpline spline;
    public int size = 10;
    // Use this for initialization
    void Start () {
        
           CreateMesh();

           
    }

    public void  CreateMesh()
    {
            Mesh mesh = new Mesh();
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            List<Vector3> normals = new List<Vector3>();

            Vector3 Start = spline.GetPoint(0f);
            Quaternion rotation = Quaternion.LookRotation(spline.GetVelocity(0));
            Vector3 left = rotation * Vector3.left;
            Vector3 right = rotation * Vector3.right;
            Vector3 up = rotation * Vector3.up;
            vertices.Add(Start + right);
            vertices.Add(Start + left);
            normals.Add(up);
            normals.Add(up);
            int triIndex = 0;

            // higher number means smoother but also more verts/tris
            for (int i = 0; i <= size; i++)
            {
                float t = (float)i / (float)size;
                Vector3 End = spline.GetPoint(t);
            rotation = Quaternion.LookRotation(spline.GetVelocity(t));

            left = rotation * Vector3.left;
                right = rotation * Vector3.right;
                up = rotation * Vector3.up;

                vertices.Add(End + right);
                vertices.Add(End + left);
                normals.Add(up);
                normals.Add(up);

                triangles.Add(triIndex);
                triangles.Add(triIndex + 1);
                triangles.Add(triIndex + 2);

                triangles.Add(triIndex + 2);
                triangles.Add(triIndex + 1);
                triangles.Add(triIndex + 3);

                triIndex += 2;

                Start = End;
            }

            mesh.SetVertices(vertices);
            mesh.SetNormals(normals);
            mesh.SetTriangles(triangles, 0);
            GetComponent<MeshFilter>().mesh = mesh;
        }
    }


