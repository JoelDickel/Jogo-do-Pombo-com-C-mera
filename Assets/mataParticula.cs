using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mataParticula : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("mataParticle", 1, 3.0f);
    }

    // Update is called once per frame
    void mataParticle()
    {
        Destroy(this.gameObject);
    }
}
