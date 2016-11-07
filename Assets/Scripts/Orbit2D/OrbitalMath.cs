using System;
using UnityEngine;

public static class OrbitalMath
{
    private enum Quadron
    {
        Fisrt,
        Second,
        Third,
        Fourth
    }

    private static Quadron CalculateQuadron(Vector2 position)
    {
        if(position.x > 0 && position.y > 0)
        {
            return Quadron.Fisrt;
        }
        if(position.x < 0 && position.y > 0)
        {
            return Quadron.Second;
        }
        if(position.x < 0 && position.y < 0)
        {
            return Quadron.Third;
        }

        return Quadron.Fourth;
    }

    public static float CalculateAngle(Vector2 position)
    {
        var quadron = CalculateQuadron(position);
        switch (quadron)
        {
            case Quadron.Fisrt:
                return GetRadiantAngle(position);
            case Quadron.Second:
                return GetRadiantAngle(position) + 180f;
            case Quadron.Third:
                return GetRadiantAngle(position) + 180f;
            case Quadron.Fourth:
                return GetRadiantAngle(position);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private static float GetRadiantAngle(Vector2 position)
    {
        var rad = Mathf.Atan(position.y / position.x);
        var degrees = rad * 180f / Mathf.PI;

        return degrees;
    }

    public static Vector2 GetVectorFromMagnitudeAndAngle(float magnitude, float angle)
    {
        return new Vector2(
            magnitude*Mathf.Cos(angle * Mathf.PI / 180f),
            magnitude*Mathf.Sin(angle * Mathf.PI / 180f)
            );
    }

    public static float CalculateVelocity(float mass, float orbitRadius)
    {
        return Mathf.Sqrt(SceneManager.GravitationalConstant * mass / orbitRadius);
    }
}