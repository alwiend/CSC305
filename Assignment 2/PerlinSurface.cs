/* 
UVic CSC 305, 2019 Spring
Assignment 02
Name: Alwien Dippenaar 
UVic ID: V00849850
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinSurface : MonoBehaviour 
{
    public float depth = .06f;

    public float scale = 2f;

    public float offsetX = 100f;
    public float offsetY = 100f;

    public int randX = 10;
    public int randY = 10;

    public Material MyMaterial;

    public GameObject elephant;
    public GameObject tree;
    public GameObject rock;

    //private bool hasRotatedE = true;
    //private bool hasRotatedT = true;
    //private bool hasRotatedR = true;

    public Vector3[,] gradient;
    // Use this for initialization
    void Start()
    {
        gradient = new Vector3[300, 300];

        for (int i = 0; i < 300; ++i)
        {
            for (int j = 0; j < 300; ++j)
            {
                Vector3 rand_vector = new Vector3(Random.value * 2 - 1, Random.value * 2 - 1, Random.value * 2 - 1);
                gradient[i,j] = rand_vector.normalized;
            }
        }

        offsetX = Random.Range(0f, 9999f); // To randomize the map on every play
        offsetY = Random.Range(0f, 9999f); // To randomize the map on every play

        randX = Random.Range(0, 251);
        randY = Random.Range(0, 251);

        Debug.Log("");
        Debug.Log("PRESS THE Y KEY TO EXPAND THE MAP");

        MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
        renderer.material = MyMaterial;
        GenerateParabolaSurface();
    }

    // Update is called once per frame
    void Update()
    {
        GenerateParabolaSurface();

        if (Input.GetKey(KeyCode.Y))
        {
            transform.localScale += new Vector3(200, 1, 200);
            scale = 100.0f;
        }
    }

    void GenerateParabolaSurface()
    {
        Mesh mesh = gameObject.GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        int[] indices = mesh.triangles;
        Vector2[] uvs = mesh.uv;

        mesh.Clear();

        //subdivision = how many squares per row/col
        int subdivision = 250;
        int stride = subdivision + 1;
        int num_vert = stride * stride;
        int num_tri = subdivision * subdivision * 2;

        indices = new int[num_tri * 3];
        int index_ptr = 0;
        for (int i = 0; i < subdivision; i++)
        {
            for (int j = 0; j < subdivision; j++)
            {
                int quad_corner = i * stride + j;
                indices[index_ptr] = quad_corner;
                indices[index_ptr + 1] = quad_corner + stride;
                indices[index_ptr + 2] = quad_corner + stride + 1;
                indices[index_ptr + 3] = quad_corner;
                indices[index_ptr + 4] = quad_corner + stride + 1;
                indices[index_ptr + 5] = quad_corner + 1;
                index_ptr += 6;
            }
        }
        
        Debug.Assert(index_ptr == indices.Length);

        const float xz_start = -5;
        const float xz_end = 5;
        float step = (xz_end - xz_start) / (float)(subdivision);
        vertices = new Vector3[num_vert];
        uvs = new Vector2[num_vert];
        
        float[,] height = GenerateHeights(stride, stride);

        for (int i = 0; i < stride; i++)
        {
            for (int j = 0; j < stride; j++)
            {
                // notice the bahavior here
                bool show_backface = false;
                float cur_x;
                float cur_z;
                //i don't know how this happened(showing back faces)
                if (show_backface)
                {
                    cur_x = xz_start + i * step;
                    cur_z = xz_start + j * step;
                }
                else
                {
                    cur_x = xz_start + j * step;
                    cur_z = xz_start + i * step;
                }
                float cur_y = height[i, j] * depth;
                cur_y = cur_y - 1;
                if(cur_y <= .3f)
                {
                    cur_y = .3f;
                }
                PlaceObject(i, j, cur_y, cur_x, cur_z);
                vertices[i * stride + j] = new Vector3(cur_x , cur_y, cur_z);
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = indices;
        mesh.RecalculateNormals();
    }
    // noise function and a random function

    float[,] GenerateHeights(int width, int height)
    {
        float[,] heights = new float[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                heights[x, y] = CalculateHeight(x, y, width, height);
            }
        }
        return heights;
    }

    float CalculateHeight(int x, int y, int width, int height)
    {
        float xCoord = (float)x / width * scale + offsetX;
        float yCoord = (float)y / height * scale + offsetY;

        return PerlinNoise(xCoord, yCoord, width);
    }

    float PerlinNoise(float xCoord, float yCoord, int width)
    {
        int X = Mathf.FloorToInt(xCoord) & 0xff;
        int Y = Mathf.FloorToInt(yCoord) & 0xff;
        
        xCoord -= Mathf.Floor(xCoord);
        yCoord -= Mathf.Floor(yCoord);

        float u = InterpolationFunction(xCoord);
        float v = InterpolationFunction(yCoord);

        Vector3 temp1 = new Vector3(X - xCoord, 0, Y - yCoord);
        Vector3 temp2 = new Vector3(X - (xCoord + 1), 0, Y - yCoord);
        Vector3 temp3 = new Vector3(X - xCoord, 0, Y - (yCoord + 1));
        Vector3 temp4 = new Vector3(X - (xCoord + 1), 0, Y - (yCoord + 1));

        float dot1 = Vector3.Dot(gradient[X, Y], temp1);
        float dot2 = Vector3.Dot(gradient[X + 1, Y], temp2);
        float dot3 = Vector3.Dot(gradient[X, Y + 1], temp3);
        float dot4 = Vector3.Dot(gradient[X + 1, Y + 1], temp4);

        float temp5 = Mathf.Lerp(dot1, dot2, u);
        float temp6 = Mathf.Lerp(dot3, dot4, u);

        float noise = Mathf.Lerp(temp5, temp6, v);
        
        return noise;
    }

    public static float InterpolationFunction(float t)
    {
        float t_cubic = t * t * t;
        float t_square = t * t;

        return 6 * t_cubic * t_square - 15 * t_square * t_square + 10 * t_cubic;
    }

    void PlaceObject(int i, int j, float y, float x, float z)
    {
        // Place the Elephant model on terrain
        if (randX == i && randY == j)
        {
            //normal.Normalize();
            if (y >= .3f)
            {
                elephant.transform.position = new Vector3(x, y, z);

            } 
        }
        // Place the tree model on the terrain
        if (randY == i && randY == j)
        {
            if (y >= .3f)
            {
                tree.transform.position = new Vector3(x, y, z);
            }
        }
        // Place the rocks model on the terrain
        if (randX == i && randX == j)
        {
            if (y >= .3f)
            {
                rock.transform.position = new Vector3(x, y, z);
            }
        }
    }
}
