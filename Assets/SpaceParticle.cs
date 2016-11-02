using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpaceParticle : MonoBehaviour
{
    private const float Scale = 0.1f;

    private static SceneManager sceneManager;

    private Rigidbody2D rigidbody2D;

    private void Start()
    {
        rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        SceneManager.cachedParticlesData.Add(new SpaceParticleCachedData
        {
            GameObject = gameObject,
            Rigidbody2D = rigidbody2D,
            Transform = transform
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
        var rb2D = coll.gameObject.GetComponent<Rigidbody2D>();
        if(rigidbody2D.mass < rb2D.mass)
            return;

        var summMass = rigidbody2D.mass + rb2D.mass;
        var proportion = rigidbody2D.mass / summMass;
        rigidbody2D.velocity = rigidbody2D.velocity * proportion + rb2D.velocity * (1 - proportion); 
        rigidbody2D.mass = summMass;
        Destroy(coll.gameObject);
    }
}