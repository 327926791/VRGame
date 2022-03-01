using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class Enemy : MonoBehaviour
{
    public Animator animator;
    public GameObject spine;
    public Transform[] target;
    public bool isDead;
    public bool Alerted;
    bool firstAlert = false;

    private int i = 1;
    public int speed = 2;
    public float delta = 1f;
    float time = 0;
    public GameObject[] Allys;

    void Start()
    {
        time = 1;
        animator = GetComponent<Animator>();
        Alerted = false;
    }

    void Update()
    {
        isDead = transform.gameObject.GetComponent<Gun>().isDead;
        if (isDead)
        {
            return;
        }

        if (Alerted)
        {
            if(Allys.Length > 0)
            {
                for(int i=0; i<Allys.Length; i++)
                {
                    //Allys[i].GetComponent<Gun>().being_shot = true;
                    Allys[i].GetComponent<Enemy>().Alerted = true;
                }
            }
            animator.SetFloat("walk_forward", 0f);
            animator.SetFloat("animation_speed", 0f);
            //move towards player and maintain distance
            GameObject player = GameObject.Find("player");

            if (player.transform.gameObject.GetComponent<Gun>().isDead) { 
                animator.SetFloat("walk_forward", -1f);
                animator.SetFloat("animation_speed", 0f);
                return;
            }

            //turn to the player
            Quaternion rotation = Quaternion.LookRotation(player.transform.position - GetComponent<Gun>().end.transform.position);
            rotation.x = 0;
            rotation.y *= 1.08f;
            rotation.z = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 3.0f * Time.deltaTime);
            if (!firstAlert)
            {
                time = 1;
                firstAlert = true;
            }
            moveToPlayer();
        }
        else
        {
            //move along the path
            moveToTarget();
        }
    }

    private void LateUpdate()
    {

    }

    public void moveToTarget()
    {
        if (target.Length > 0)
        {
            //print(gameObject + "move at target: " + i + " " + target[i]);
            target[i].position = new Vector3(target[i].position.x, target[i].position.y, target[i].position.z);

            Quaternion rotation = Quaternion.LookRotation(target[i].transform.position - transform.position);
            rotation.x = 0;
            rotation.z = 0;
            Quaternion vec = Quaternion.Slerp(transform.rotation, rotation, 1.0f * Time.deltaTime);
            //print(vec);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 1.0f * Time.deltaTime);

            if (time > 0)
            {
                time -= Time.deltaTime;
                return;
            }

            if (transform.position.x > target[i].position.x - delta &&
                transform.position.x < target[i].position.x + delta &&
                transform.position.z > target[i].position.z - delta &&
                transform.position.z < target[i].position.z + delta)
            {
               // print(gameObject + "move at next target: " + i );
                i = (i + 1) % target.Length;
                //print(gameObject + "move at next target: " + i);
                time = 1;
                animator.SetFloat("walk_forward", -1f);
                animator.SetFloat("animation_speed", 0f);
            }
            else
            {
                //move forward
                animator.SetFloat("walk_forward", speed);
                // Setting animation running speed
                animator.SetFloat("animation_speed", 1.5f);

            }
        }

    }

    public void moveToPlayer()
    {
        if (time > 0)
        {
            time -= Time.deltaTime;
            return;
        }

        GameObject player = GameObject.Find("player");
        float distance = (transform.position - player.transform.position).magnitude;
        if (distance > 10)
        {
            //print("move forwards");
            //move forward
            animator.SetFloat("walk_forward", speed);
            // Setting animation running speed
            animator.SetFloat("animation_speed", 2f);
        }
        else if (distance < 2)
        {

            //print("move backwards");
            animator.SetFloat("walk_backward", speed);
            animator.SetFloat("animation_speed", 1f);
        }
        else
        {
            animator.SetFloat("walk_backward", -1f);
            animator.SetFloat("walk_forward", -1f);
            animator.SetFloat("animation_speed", 0f);
        }

    }

}
