using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralMesh2 : MonoBehaviour {
    public BezierSpline spline;
    public int size = 10;
    public float width = 1;
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
            Vector3 left = rotation * Vector3.left*width;
            Vector3 right = rotation * Vector3.right*width;
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

                left = rotation * Vector3.left * width;
                right = rotation * Vector3.right * width;
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


        //duplicate for twosided meshes
        int szT = triangles.Count;
        int szV = vertices.Count;
        for(int k=0; k<szV; k++)
        {
            vertices.Add(vertices[k]);
            normals.Add(normals[k]);
        }
        var newTris = new int[szT * 2]; // double the triangles
        for (int i = 0; i < szT; i += 3)
        {
            // copy the original triangle
            newTris[i] = triangles[i];
            newTris[i + 1] = triangles[i + 1];
            newTris[i + 2] = triangles[i + 2];
            // save the new reversed triangle
            int j = i + szT;
            newTris[j] = triangles[i] + szV;
            newTris[j + 2] = triangles[i + 1] + szV;
            newTris[j + 1] = triangles[i + 2] + szV;
        }

        Vector2[] Uvs = new Vector2[vertices.Count];
        //Set UVs
        for (int i = 0; i < vertices.Count; i++)
        {

            Uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
        }

        mesh.SetVertices(vertices);
            mesh.SetNormals(normals);
            mesh.triangles = newTris;
            mesh.uv = Uvs;
            GetComponent<MeshFilter>().mesh = mesh;
            GetComponent<MeshCollider>().sharedMesh = mesh;
        }
    }


