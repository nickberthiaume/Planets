using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainFace {

    ShapeGenerator shapeGenerator;
    Mesh mesh;
    int resolution;
    Vector3 localUp;
    Vector3 axisA;
    Vector3 axisB;

    public TerrainFace(ShapeGenerator shapeGenerator, Mesh mesh, int resolution, Vector3 localUp)
    {
        this.shapeGenerator = shapeGenerator;
        this.mesh = mesh;
        this.resolution = resolution;
        this.localUp = localUp;

        //Create A and B axis based off localUp axis
        axisA = new Vector3(localUp.y, localUp.z, localUp.x);
        axisB = Vector3.Cross(localUp, axisA);
    
    }

    public void ConstructMesh()
    {
        Vector3[] vertices = new Vector3[resolution * resolution]; //Mesh will have resolution^2 number of vertices
        int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6]; //Mesh will have (resoltuion - 1)^2 *6 triangles
        int triIndex = 0;
        

        for (int y = 0; y < resolution; y++)
        {
            for(int x = 0; x < resolution; x++)
            {
                //Creates vertices according to resolution size
                int i = x + y * resolution;
                Vector2 percent = new Vector2(x, y) / (resolution - 1);
                Vector3 pointOnUnitCube = localUp + (percent.x - .5f) * 2 * axisA + (percent.y - .5f) * 2 * axisB;
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;
                vertices[i] = shapeGenerator.CalculatePointOnPlanet(pointOnUnitSphere);

                //Prevents triangles from being created outside of mesh
                if( x != (resolution - 1) && y != (resolution - 1))
                {
                    //Creates first triangle for face
                    triangles[triIndex] = i;
                    triangles[triIndex + 1] = i + resolution + 1;
                    triangles[triIndex + 2] = i + resolution;
                    //Creates second triangle for face
                    triangles[triIndex + 3] = i;
                    triangles[triIndex + 4] = i + 1;
                    triangles[triIndex + 5] = i + resolution + 1;

                    triIndex += 6; //Increments index counter by 6 because 6 vertices were already used
                }
            }
        }
        mesh.Clear(); //Clear mesh data because updating with lower resolution may cause reference errors
        //Assign vertices and triangles to mesh, and recalculates the normals
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}
