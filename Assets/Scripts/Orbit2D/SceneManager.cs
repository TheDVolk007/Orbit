using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using Random = UnityEngine.Random;

public class SceneManager : MonoBehaviour
{
    public static GameObject particlePrefab;

    public float ParticlesToSpawn = 10f;
    public float SpinForce = 10f;
    public float SpawnDistance = 10f;
    public bool BalancedSystem = true;

    public static readonly float GravitationalConstant = 6.67408f * Mathf.Pow(10, -3);
    
    public static List<SpaceParticleCachedData> cachedParticlesData = new List<SpaceParticleCachedData>();

    public static float PlaneRadius = 50f;
    private GameObject plane;

    private bool isRunning;

    private void Start ()
	{
	    particlePrefab = Resources.Load<GameObject>("Prefabs/Particle");
        plane = GameObject.Find("Plane");
	}
	
    private void Update ()
    {
        if(Input.GetMouseButtonUp(0))
            SpawnParticle();

        if(Input.GetMouseButtonUp(1))
            SpawnCirclingParticles();

        plane.transform.localScale = new Vector3(PlaneRadius, PlaneRadius);
    }

    private void FixedUpdate()
    {
        if(!isRunning)
        {
            isRunning = true;
            try
            {
                StartCoroutine(RunParallelAttractionCalculation());
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                isRunning = false;
            }
        }
    }

    private static IEnumerator RunParallelAttractionCalculation()
    {
        while(true)
        {
            var currentParticlesData = cachedParticlesData.Where(p => !p.Particle.isDestroyed).ToList();

            const int maxCalculationsPerStep = 20000;
            var breakPoints = MathHelper.FindBreakingPointsInArithmeticSequence(currentParticlesData.Count, -1, currentParticlesData.Count, maxCalculationsPerStep);

            for (var i = 0; i < currentParticlesData.Count; i++)
            {
                if(breakPoints.Contains(i))
                    yield return new WaitForFixedUpdate();

                for (var j = i + 1; j < currentParticlesData.Count; j++)
                {
                    AttractBodies(currentParticlesData[i], currentParticlesData[j], breakPoints.Count);
                }
            }

            var particlesToDestroy = cachedParticlesData.Where(p => p.Particle.isDestroyed).ToList();
            particlesToDestroy.ForEach(p => Destroy(p.GameObject));
            cachedParticlesData = cachedParticlesData.Where(p => p.GameObject != null).ToList();

            yield return new WaitForFixedUpdate();
        }
    }

    private static void AttractBodies(SpaceParticleCachedData firstBody, SpaceParticleCachedData secondBody, int forceMultiplyer)
    {
        //virtually make first point center of space, so the distance to the center for the second is the distance to the first (it's simoultaneously the negative of the new virtual position of second point)
        var distanceToSecondParticle = firstBody.Transform.position - secondBody.Transform.position;
        //make sure it has no overlap, and if it does, virtually transport the particle
        distanceToSecondParticle = MathHelper.TransportPointInRespectToPlaneBorders(distanceToSecondParticle, PlaneRadius);
        var force = GravitationalConstant * (1 + forceMultiplyer) * firstBody.Rigidbody2D.mass * secondBody.Rigidbody2D.mass / distanceToSecondParticle.sqrMagnitude;
        firstBody.Rigidbody2D.AddForce(-distanceToSecondParticle.normalized * force, ForceMode2D.Force);
        secondBody.Rigidbody2D.AddForce(distanceToSecondParticle.normalized * force, ForceMode2D.Force);
    }

    private static void RunPreciseAttractionCalculation()
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