using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using Random = UnityEngine.Random;

public class SceneManager : MonoBehaviour
{
    private static GameObject particlePrefab;

    public float ParticlesToSpawn = 10f;
    public float SpinForce = 10f;
    public float SpawnDistance = 10f;
    public bool BalancedSystem = true;

    public static readonly float GravitationalConstant = 6.67408f * Mathf.Pow(10, -3);

    public static List<GameObject> particles = new List<GameObject>();

    private void Start ()
	{
	    particlePrefab = Resources.Load<GameObject>("Prefabs/Particle");
	}
	
    private void Update ()
    {
        if(Input.GetMouseButtonUp(0))
            SpawnParticle();

        if(Input.GetMouseButtonUp(1))
            SpawnCirclingParticles();
    }

    private void LateUpdate()
    {
        particles = particles.Where(p => p != null).ToList();
    }

    private void SpawnParticle()
    {
        var cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var particlePosition = new Vector2(cursorPosition.x, cursorPosition.y);

        InstantiateParticle(particlePosition);
    }

    private void SpawnCirclingParticles()
    {
        for (var i = 0; i < ParticlesToSpawn; i++)
        {
            var particle = InstantiateParticle(Random.insideUnitCircle * SpawnDistance);
            AddSpinToParticle(particle, BalancedSystem ? (42f / SpawnDistance) :  SpinForce, true);
        }
    }

    private static GameObject InstantiateParticle(Vector2 particlePosition)
    {
        return (GameObject)Instantiate(particlePrefab, particlePosition, Quaternion.identity);
    }

    private void AddSpinToParticle(GameObject particle, float forceMagnitude, bool clockwise)
    {
        var position = particle.transform.position;
        var rb2D = particle.GetComponent<Rigidbody2D>();
        var distanceToCenter = position.magnitude;
        var angleOfForce = CalculateAngle(position) + (clockwise ? (-90) : 90);
        var forceVector = GetVectorFromMagnitudeAndAngle(forceMagnitude * distanceToCenter, angleOfForce);
        rb2D.AddForce(forceVector, ForceMode2D.Force);
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

    private static float CalculateAngle(Vector2 position)
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

    private static Vector2 GetVectorFromMagnitudeAndAngle(float magnitude, float angle)
    {
        return new Vector2(
            magnitude*Mathf.Cos(angle * Mathf.PI / 180f),
            magnitude*Mathf.Sin(angle * Mathf.PI / 180f)
            );
    }

    private static float CalculateVelocity(float mass, float orbitRadius)
    {
        return Mathf.Sqrt(GravitationalConstant * mass / orbitRadius);
    }
}

public enum Quadron
{
    Fisrt,
    Second,
    Third,
    Fourth
}
