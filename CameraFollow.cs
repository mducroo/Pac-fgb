using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    //The script looks at the pos. of pac man and sets cam pos. accordinly and smoothly.

    public Transform target;
    private float _distX, _distZ;

    private Vector3 _pos;

    public float speed = 1f;
    // Start is called before the first frame update
    void Start()
    {
        _distX = transform.position.x - target.position.x;
        _distZ = transform.position.z - target.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        _pos.x = target.position.x + _distX;
        _pos.y = transform.position.y;
        _pos.z = target.position.z + _distZ;

        transform.position = Vector3.Lerp(transform.position, _pos, speed * Time.deltaTime);
    }
}
 