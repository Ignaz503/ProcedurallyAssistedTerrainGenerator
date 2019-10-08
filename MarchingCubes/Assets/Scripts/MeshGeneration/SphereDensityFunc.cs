using UnityEngine;

public class SphereDensityFunc : IDensityFunc
{
    float rad;
    Vector3 center;

    public SphereDensityFunc(float rad, Vector3 center)
    {
        this.rad = rad;
        this.center = center;
    }

    public float Evaluate(SamplePointVariables x, SamplePointVariables y, SamplePointVariables z)
    {
        return Evaluate(new Vector3(x.ValueWorld, y.ValueWorld, z.ValueWorld));
    }

    public float Evaluate(Vector3 point)
    {
        return -(((point - center).magnitude - rad));//+ (Mathf.PerlinNoise(point.x,point.y));
    }
}


