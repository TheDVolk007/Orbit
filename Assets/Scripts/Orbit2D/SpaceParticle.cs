using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpaceParticle : MonoBehaviour
{
    private const float Scale = 0.1f;

    private static SceneManager sceneManager;

    private Rigidbody2D rigidbody2D;

    private readonly List<string> immuneToCollisionWithTags = new List<string>();

    private string Tag = string.Empty;

    public bool isDestroyed;

    private void Start()
    {
        rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        SceneManager.cachedParticlesData.Add(new SpaceParticleCachedData
        {
            GameObject = gameObject,
            Rigidbody2D = rigidbody2D,
            Transform = transform,
            Particle = this
        });
    }

    private void FixedUpdate()
    {
        CalculateRadius();
    }
    
    private void CalculateRadius()
    {
        var radius = Mathf.Pow(rigidbody2D.mass, 1f/3f) * Scale;
        transform.localScale = new Vector2(radius, radius);
    }

    private void OnCollisionEnter2D(Collision2D coll)
    {
        var particle = coll.gameObject.GetComponent<SpaceParticle>();
        if(isDestroyed || particle == null || particle.isDestroyed || immuneToCollisionWithTags.Contains(particle.Tag))
            return;

        var rb2D = coll.gameObject.GetComponent<Rigidbody2D>();
        if(rigidbody2D.mass < rb2D.mass)
            return;

        var summMass = rigidbody2D.mass + rb2D.mass;
        var proportion = rigidbody2D.mass / summMass;
        rigidbody2D.velocity = rigidbody2D.velocity * proportion + rb2D.velocity * (1 - proportion); 
        rigidbody2D.mass = summMass;
        
        particle.rigidbody2D.mass = float.Epsilon;
        particle.CalculateRadius();
        particle.isDestroyed = true;
    }

    public void BlowUp(float force)
    {
        if(rigidbody2D.mass < 50f)
            return;

        const float immuneToTagRemovalDelay = 1f;

        var coreMass = Random.Range(0.1f, 0.3f) * rigidbody2D.mass;
        var debrisMass = rigidbody2D.mass - coreMass;
        var debrisCount = Random.Range(50, 100);
        var singleDebrisMass = debrisMass / debrisCount;

        var debrisSpawnZone = transform.localScale.x;
        Tag = Guid.NewGuid().ToString();
        immuneToCollisionWithTags.Add(Tag);
        rigidbody2D.mass = coreMass;
        CalculateRadius();
        
        for(var i = 0; i < debrisCount; i++)
        {
            var position = Random.insideUnitCircle * debrisSpawnZone + (Vector2)transform.position;
            var debrisParticleGameObject = (GameObject)Instantiate(SceneManager.particlePrefab, position, Quaternion.identity);
            var debrisParticle = debrisParticleGameObject.GetComponent<SpaceParticle>();
            debrisParticle.Tag = Tag;
            debrisParticle.immuneToCollisionWithTags.Add(Tag);
            //StartCoroutine(debrisParticle.SetMassAndAddForceDelayed(position.normalized * force * singleDebrisMass, singleDebrisMass));
            var rb2D = debrisParticleGameObject.GetComponent<Rigidbody2D>();
            rb2D.mass = singleDebrisMass;
            rb2D.AddForce(position.normalized * force * (float)Math.Ceiling(singleDebrisMass));
            StartCoroutine(debrisParticle.RemoveImmuneTag(Tag, immuneToTagRemovalDelay));
        }
        StartCoroutine(RemoveImmuneTag(Tag, immuneToTagRemovalDelay));
    }

    private IEnumerator SetMassAndAddForceDelayed(Vector2 force, float mass)
    {
        yield return new WaitForFixedUpdate();
        rigidbody2D.mass = mass;
        rigidbody2D.AddForce(force);
    }

    private IEnumerator RemoveImmuneTag(string tag, float delay)
    {
        yield return new WaitForSeconds(delay);

        immuneToCollisionWithTags.Remove(tag);
    }
}