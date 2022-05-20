using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyDmgPU : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 0.6f);
        transform.localPosition += new Vector3(0, 0.2f, 0);
    }
}
