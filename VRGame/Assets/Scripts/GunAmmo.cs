using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GunAmmo : MonoBehaviour
{

    int remainingAmmo = 0;
    void Start()
    {

    }

    void Update()
    {
        //Rigidbody rigidbody = transform.GetComponent<Rigidbody>();
        //if(rigidbody.isKinematic == false)
        //{

        //}
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponentInParent<CharacterMovement>())
        {
            Rigidbody rigidbody = transform.GetComponent<Rigidbody>();
            if (rigidbody.isKinematic == false)
            {
             //   print("collide with player "+ GameObject.Find("player").GetComponent<Gun>().GetRemainingBullets());
                if (GameObject.Find("player").GetComponent<Gun>().GetRemainingBullets() < 90)
                    GameObject.Find("player").GetComponent<Gun>().AddAmmo(LootAmmo());
            }
                
        }
    }

    public void SetAmmo(int ammo)
    {
        remainingAmmo = ammo;
    }

    public int LootAmmo()
    {
        Destroy(transform.gameObject);
        return remainingAmmo;
    }
}
