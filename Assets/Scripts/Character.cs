using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Character : MonoBehaviour
{

    [SerializeField] private float Chspeed = 5.0f;
    [SerializeField] private float horizontal;

    [SerializeField] private float groundRadius = 0.5f;

    private SpriteRenderer srr;
    private Rigidbody2D rb;
    [SerializeField] private Transform gizmo;

    private void Awake()
    {
        srr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }
    // Start is called before the first frame update
    void Start()
    {
    
    }

    
    // Update is called once per frame
    void Update()
    {
    
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontal *Chspeed , rb.velocity.y);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(gizmo.position, groundRadius);
    }
}
