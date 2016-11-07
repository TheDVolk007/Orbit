﻿using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public class SceneManager : MonoBehaviour
{
    public static GameObject particlePrefab;

    public float ParticlesToSpawn = 10f;
    public float SpinForce = 10f;
    public float SpawnDistance = 10f;
    public bool BalancedSystem = true;

    public static readonly float GravitationalConstant = 6.67408f * Mathf.Pow(10, -3);
    
    public static List<SpaceParticleCachedData> cachedParticlesData = new List<SpaceParticleCachedData> ();

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
        cachedParticlesData = cachedParticlesData.Where(p => p.GameObject != null).ToList();
        
        for (var i = 0; i < cachedParticlesData.Count; i++)
        {
            for (var j = i + 1; j < cachedParticlesData.Count; j++)
            {
                var vector = cachedParticlesData[i].Transform.position - cachedParticlesData[j].Transform.position;
                var force = GravitationalConstant * cachedParticlesData[i].Rigidbody2D.mass * cachedParticlesData[j].Rigidbody2D.mass / vector.sqrMagnitude;
                cachedParticlesData[i].Rigidbody2D.AddForce(-vector.normalized * force, ForceMode2D.Force);
                cachedParticlesData[j].Rigidbody2D.AddForce(vector.normalized * force, ForceMode2D.Force);
            }
        }
    }

    private static void SpawnParticle()
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

            AddSpinToParticle(particle.transform.position, rb2D, BalancedSystem ? (42f / SpawnDistance) :  SpinForce, true);
        }
    }

    private static GameObject InstantiateParticle(Vector2 particlePosition)
    {
        return (GameObject)Instantiate(particlePrefab, particlePosition, Quaternion.identity);
    }

    private static void AddSpinToParticle(Vector2 position, Rigidbody2D rb2D, float forceMagnitude, bool clockwise)
    {
        var angleOfForce = OrbitalMath.CalculateAngle(position) + (clockwise ? (-90) : 90);
        var forceVector = OrbitalMath.GetVectorFromMagnitudeAndAngle(forceMagnitude * position.magnitude, angleOfForce);
        rb2D.AddForce(forceVector, ForceMode2D.Force);
    }
}