using UnityEngine;
using System.Linq;

public class SpaceParticle : MonoBehaviour
{
    private const float Scale = 0.1f;

    private static SceneManager sceneManager;

    private Rigidbody2D rigidbody2D;

    private void Start()
    {
        SceneManager.particles.Add(gameObject);
        rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Attract();
        CalculateRadius();
    }
    
    private void CalculateRadius()
    {
        var radius = Mathf.Pow(rigidbody2D.mass, 1f/3f) * Scale;
        transform.localScale = new Vector2(radius, radius);
    }

    private void Attract()
    {
        foreach(var other in SceneManager.particles.Where(p => p != gameObject && p != null))
        {
            var rb2D = other.GetComponent<Rigidbody2D>();
            var vector = transform.position - other.transform.position;
            var force = SceneManager.GravitationalConstant * rigidbody2D.mass * rb2D.mass / vector.magnitude;
            rb2D.AddForce(vector.normalized * force, ForceMode2D.Force);
        }
    }

    private void OnCollisionEnter2D(Collision2D coll)
    {
        var rb2D = coll.gameObject.GetComponent<Rigidbody2D>();
        if(rigidbody2D.mass < rb2D.mass)
            return;

        rigidbody2D.mass += rb2D.mass;
        Destroy(coll.gameObject);
    }
}
