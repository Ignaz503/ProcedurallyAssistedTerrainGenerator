using NoiseTest;
using System;

public static class SimplexNoise
{
    static OpenSimplexNoise simplexNoise;

    public static void SeedNoise(string seed)
    {
        simplexNoise = new OpenSimplexNoise(seed.GetHashCode());
    }

    public static void EnsureInitialization()
    {
        if (simplexNoise == null)
            SeedNoise(DateTime.Now.Ticks.ToString());
    }

    public static float Evaulate(float x, float y)
    {
        EnsureInitialization();
        return (float)simplexNoise.Evaluate(x, y);
    }

    public static float Evaulate(float x, float y, float z)
    {
        EnsureInitialization();
        return (float)simplexNoise.Evaluate(x, y, z);
    }

    public static float Evaulate(float x, float y, float z, float w)
    {
        EnsureInitialization();
        return (float)simplexNoise.Evaluate(x, y, z, w);
    }
}