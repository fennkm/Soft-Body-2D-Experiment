using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : MonoBehaviour
{
    public Point start;
    public Point end;

    public float springConst;

    public float restingLength;
    public float length;

    public float dampingFactor;

    private LineRenderer lineRenderer;

    public void setPoints(Point start, Point end, float springConst, float dampingFactor)
    {
        setPoints(start, end, springConst, dampingFactor, Vector3.Distance(start.transform.position, end.transform.position));
    }

    public void setPoints(Point start, Point end, float springConst, float dampingFactor, float restingLength)
    {
        this.restingLength = restingLength;
        this.length = restingLength;
        this.springConst = springConst;
        this.dampingFactor = dampingFactor;
        this.start = start;
        this.end = end;
    }

    public void visualise() 
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = true;
        displayLine();
    }

    public void displayLine()
    {
        lineRenderer.SetPosition(0, start.transform.position);
        lineRenderer.SetPosition(1, end.transform.position);

        float stretch = getSpringForce() * 0.04f;
        Color col;

        if (stretch <= 0)
            col = new Color(0f, -1f/(stretch-1f), 1f/(stretch-1f)+1f);
        else
            col = new Color(-1f/(stretch+1f)+1f, 1f/(stretch+1f), 0f);

        lineRenderer.startColor = col;
        lineRenderer.endColor = col;
    }

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if(lineRenderer.enabled == true)
            displayLine();
    }

    void FixedUpdate()
    {
        length = Vector3.Distance(start.transform.position, end.transform.position);

        float force = getSpringForce() + getDampingForce();

        Vector3 forceVec = (end.transform.position - start.transform.position).normalized;

        start.rb.AddForce(forceVec * force);
        end.rb.AddForce(-forceVec * force);
    }

    public float getSpringForce()
    {
        return (length - restingLength) * springConst;
    }

    public float getDampingForce()
    {
        return Vector3.Dot((end.transform.position - start.transform.position).normalized, end.rb.velocity - start.rb.velocity) * dampingFactor;
    }
}
