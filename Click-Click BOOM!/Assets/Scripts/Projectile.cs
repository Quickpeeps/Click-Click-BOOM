using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Variables")]    
    [SerializeField] private float projectileTimer;
    [SerializeField] private float projectileDmg;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private Camera mainCam;
    private Rigidbody2D rb;
    private Vector3 mousePos;
    // Start is called before the first frame update
    void Start()
    {
        //Gets the main camera, sets the directions and gives it some force
        mainCam = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePos- transform.position;
        Vector3 rotation = transform.position - mousePos;
        rb.velocity = new Vector2(direction.x, direction.y).normalized * projectileSpeed;
        float rot = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rot + 90);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
}
