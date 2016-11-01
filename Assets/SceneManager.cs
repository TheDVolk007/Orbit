using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
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

    public enum Quadron
    {
        Fisrt,
        Second,
        Third,
        Fourth
    }

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

    private void FixedUpdate()
    {
        //foreach(var other in SceneManager.particles.Where(p => p != gameObject && p != null))
        //{
        //    var rb2D = other.GetComponent<Rigidbody2D>();
        //    var vector = transform.position - other.transform.position;
        //    var force = SceneManager.GravitationalConstant * rigidbody2D.mass * rb2D.mass / vector.magnitude;
        //    rb2D.AddForce(vector.normalized * force, ForceMode2D.Force);
        //}

        //for(var i = 0; i < particles.Count; i++)
        //{
        //    for(var j = i + 1; j < particles.Count; j++)
        //    {
        //        var first = particles[i];
        //        var second = particles[j];
        //        var rb2D1 = first.GetComponent<Rigidbody2D>();
        //        var rb2D2 = second.GetComponent<Rigidbody2D>();
        //        var vector = transform.position - second.transform.position;
        //        var force = GravitationalConstant * rb2D1.mass * rb2D2.mass / vector.magnitude;
        //        rb2D1.AddForce(-vector.normalized * force, ForceMode2D.Force);
        //        rb2D2.AddForce(vector.normalized * force, ForceMode2D.Force);
        //    }
        //}
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
            var rb2D = particle.GetComponent<Rigidbody2D>();

            rb2D.mass = Random.Range(1f, 3f);

            AddSpinToParticle(particle.transform.position, rb2D, BalancedSystem ? (42f * 2.4f / SpawnDistance) :  SpinForce, true);
        }
    }

    private static GameObject InstantiateParticle(Vector2 particlePosition)
    {
        return (GameObject)Instantiate(particlePrefab, particlePosition, Quaternion.identity);
    }

    private static void AddSpinToParticle(Vector2 position, Rigidbody2D rb2D, float forceMagnitude, bool clockwise)
    {
        var angleOfForce = CalculateAngle(position) + (clockwise ? (-90) : 90);
        var forceVector = GetVectorFromMagnitudeAndAngle(forceMagnitude * position.magnitude, angleOfForce);
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
