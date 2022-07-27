using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoftBody : MonoBehaviour
{
    public GameObject pointPrefab;
    public GameObject springPrefab;

    public int m_width;
    public int m_height;
    public float m_springLength;
    public float m_springConst;
    public float m_dampingFactor;
    public bool m_displayStructure;

    [HideInInspector]
    public int width;
    [HideInInspector]
    public int height;
    [HideInInspector]
    public float springLength;
    [HideInInspector]
    public float springConst;
    [HideInInspector]
    public float dampingFactor;
    [HideInInspector]
    public bool displayStructure;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    [HideInInspector]
    public Point[] points;
    [HideInInspector]
    public Spring[] springs;

    private void Awake() 
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
    }

    private Point getPoint(int x, int y)
    {
        return points[x * (height + 1) + y];
    }

    /*
    POINT ARRAY: column first

    3 7 11
    2 6 10
    1 5 9
    0 4 8
    */
    public void buildObject()
    {
        for (int i = transform.childCount; i > 0; i--)
            DestroyImmediate(transform.GetChild(0).gameObject);

        width = m_width;
        height = m_height;
        springLength = m_springLength;
        springConst = m_springConst;
        dampingFactor = m_dampingFactor;
        displayStructure = m_displayStructure;

        points = new Point[(width + 1) * (height + 1)];
        springs = new Spring[width * (height + 1) + (width + 1) * height + width * height * 2];

        for (int i = 0; i < width + 1; i++)
            for (int j = 0; j < height + 1; j++)
            {
                GameObject point = Instantiate(pointPrefab, transform.position + new Vector3(i - width / 2f, j - height / 2f, 0f) * springLength, Quaternion.identity, transform);

                if (displayStructure)
                    point.GetComponent<SpriteRenderer>().enabled = true;

                point.GetComponentsInChildren<CircleCollider2D>()[1].radius = springLength / 3f;

                points[i * (height + 1) + j] = point.GetComponent<Point>();
            }

        int n = 0;

        // Springs: -
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height + 1; j++)
            {
                Spring spring = Instantiate(springPrefab, transform).GetComponent<Spring>();
                spring.setPoints(getPoint(i, j), getPoint(i + 1, j), springConst, dampingFactor, springLength);

                springs[n++] = spring;

                if (displayStructure)
                    spring.visualise();
            }

        // Springs: |
        for (int i = 0; i < width + 1; i++)
            for (int j = 0; j < height; j++)
            {
                Spring spring = Instantiate(springPrefab, transform).GetComponent<Spring>();
                spring.setPoints(getPoint(i, j), getPoint(i, j + 1), springConst, dampingFactor, springLength);
                
                springs[n++] = spring;

                if (displayStructure)
                    spring.visualise();
            }

        // Springs: \ and /
        for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
            {
                Spring springL = Instantiate(springPrefab, transform).GetComponent<Spring>();
                Spring springR = Instantiate(springPrefab, transform).GetComponent<Spring>();

                springL.setPoints(getPoint(i, j), getPoint(i + 1, j + 1), springConst, dampingFactor, springLength * 1.4142f);
                springR.setPoints(getPoint(i, j + 1), getPoint(i + 1, j), springConst, dampingFactor, springLength * 1.4142f);

                springs[n++] = springL;
                springs[n++] = springR;

                if (displayStructure)
                {
                    springL.visualise();
                    springR.visualise();
                }
            }

        buildMesh();
    }

    private void buildMesh()
    {   
        
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();

        if (displayStructure)
        {
            meshRenderer.enabled = false;
            return;
        }

        meshRenderer.enabled = true;

        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[(width + 1) * (height + 1)];

        for (int i = 0; i < points.Length; i++)
            vertices[i] = points[i].transform.localPosition;
           
        mesh.SetVertices(vertices);
            
        mesh.SetUVs(0, vertices.Select(e => new Vector2(e.x, e.y)).ToArray());

        int[] triangles = new int[width * height * 2 * 3];

        for (int i = 0; i < width * height; i++)
        {
            int bottomLeft = i + i / height;
            triangles[i * 6]     = bottomLeft;
            triangles[i * 6 + 1] = bottomLeft + 1;
            triangles[i * 6 + 2] = bottomLeft + height + 1;
            triangles[i * 6 + 3] = bottomLeft + 1;
            triangles[i * 6 + 4] = bottomLeft + height + 2;
            triangles[i * 6 + 5] = bottomLeft + height + 1;
        }

        mesh.SetTriangles(triangles, 0);

        meshFilter.mesh = mesh;
    }
    
    private void Update() 
    {
        if (!displayStructure)
        {
            Mesh mesh = meshFilter.mesh;
            
            Vector3[] vertices = new Vector3[(width + 1) * (height + 1)];

            for (int i = 0; i < points.Length; i++)
                vertices[i] = points[i].transform.localPosition;       

            mesh.SetVertices(vertices);
        }
    }
}