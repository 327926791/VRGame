using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class Gun : MonoBehaviour {

    public GameObject end, start; // The gun start and end point
    public GameObject gun;
    public Animator animator;
    
    public GameObject spine;
    public GameObject handMag;
    public GameObject gunMag;

    public GameObject bulletHole;
    public GameObject muzzleFlash;
    public GameObject shootSound;
    public GameObject reloadSound;
    public GameObject lootSound;

    float gunShotTime = 0.1f;
    float gunReloadTime = 1.0f;
    Quaternion previousRotation;
    public float health = 100;
    public bool isDead;
 

    public Text magBullets;
    public Text remainingBullets;
    public Text remainingHealth;

    int magBulletsVal = 30;
    int remainingBulletsVal = 90;
    int magSize = 30;
    public GameObject headMesh;
    public static bool leftHanded { get; private set; }
    public int show;

    public bool being_shot = false;
    // Use this for initialization
    void Start() {
        if (gameObject.tag == "Player")
        {
            headMesh.GetComponent<SkinnedMeshRenderer>().enabled = false; // Hiding player character head to avoid bugs :)
        }
        //player = GameObject.Find("player");
    }

    // Update is called once per frame
    void Update() {
        if (isDead)
            return;
        if(health <= 0)
        {
            isDead = true;
            Enemy enemy_script = transform.GetComponent<Enemy>();
            if (enemy_script)//call ally
            {
                if (enemy_script.Allys.Length > 0)
                {
                    for (int i = 0; i < enemy_script.Allys.Length; i++)
                    {
                        enemy_script.Allys[i].GetComponent<Enemy>().Alerted = true;
                    }
                }
            }

            animator.StopPlayback();
            animator.SetBool("dead", true);
            animator.SetBool("reloadAfterFire", false);
            headMesh.GetComponent<SkinnedMeshRenderer>().enabled = true;

            //seperate the guns
            GameObject parent = GameObject.Find("Guns");
            gun.gameObject.GetComponent<GunAmmo>().SetAmmo(magBulletsVal + remainingBulletsVal);
            gun.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            gun.gameObject.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;
            gun.transform.SetParent(parent.transform);



            //var controller = transform.gameObject.GetComponent<CharacterController>();
            //controller.SimpleMove(vel);
        }
        //player_pos = player.transform.position;
        //print(gameObject.tag+"  " + (magBulletsVal+ magBulletsVal));
        // Cool down times
        if (gunShotTime >= 0.0f)
        {
            gunShotTime -= Time.deltaTime;
        }
        if (gunReloadTime >= 0.0f)
        {
            gunReloadTime -= Time.deltaTime;
        }


        if (gameObject.tag == "Player")
        {
            if ((Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)) && gunShotTime <= 0 && gunReloadTime <= 0.0f && magBulletsVal > 0 && !isDead)
            {
                Ray ray = new Ray(end.transform.position, (end.transform.position - start.transform.position).normalized);
                shotDetection(ray); // Should be completed

                addEffects(ray); // Should be completed

                animator.SetBool("fire", true);
                gunShotTime = 0.5f;

                // Instantiating the muzzle prefab and shot sound

                magBulletsVal = magBulletsVal - 1;
                if (magBulletsVal <= 0 && remainingBulletsVal > 0)
                {
                    animator.SetBool("reloadAfterFire", true);
                    Destroy(Instantiate(reloadSound, end.transform.position, end.transform.rotation), 2.5f);
                    gunReloadTime = 2.5f;
                    Invoke("reloaded", 2.5f);
                }
            }
            else
            {
                animator.SetBool("fire", false);
            }

            if ((Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.R)) && gunReloadTime <= 0.0f && gunShotTime <= 0.1f && remainingBulletsVal > 0 && magBulletsVal < magSize && !isDead)
            {
                animator.SetBool("reload", true);
                Destroy(Instantiate(reloadSound, end.transform.position, end.transform.rotation), 2.5f);
                gunReloadTime = 2.5f;
                Invoke("reloaded", 2.0f);
            }
            else
            {
                animator.SetBool("reload", false);
            }
            updateText();
        }
        else
        {
            if (detectPlayer())
            {
                transform.gameObject.GetComponent<Enemy>().Alerted = true;
                if (gunShotTime <= 0 && gunReloadTime <= 0.0f && magBulletsVal > 0 && !isDead && !GameObject.Find("player").GetComponent<Gun>().isDead)
                {
                    GameObject player = GameObject.Find("player");
                    //print("fire, " + gunShotTime + "  " + gunReloadTime);
                    Vector2 noise = new Vector2(NextGaussian(0, 0.8f, -0.12f, 0.12f), NextGaussian(0, 0.8f, -0.12f, 0.12f));
                    //print(noise);                    
                    //Vector3 direction = (end.transform.position - start.transform.position).normalized + end.transform.rotation * new Vector3(noise.x, noise.y, 0);

                    Vector3 player_direction = player.transform.position;
                    player_direction.y = end.transform.position.y;
                    Vector3 direction = (player_direction - end.transform.position).normalized + end.transform.rotation * new Vector3(noise.x, noise.y, 0);
                    Ray ray = new Ray(end.transform.position, direction);
                    Debug.DrawRay(end.transform.position, direction, Color.blue, 3);
                    Debug.DrawRay(end.transform.position, (player_direction - end.transform.position).normalized, Color.yellow,1);                  
                    
                    float ShootingAngle = 45;
                    Vector3 ShootingDirection = player.transform.position - transform.position;
                    float degree = Vector3.Angle(ShootingDirection, transform.forward);
                    float distance = (transform.position - player.transform.position).magnitude;

                    RaycastHit hit;
                    Physics.Raycast(end.transform.position, (player_direction - end.transform.position).normalized, out hit, distance);  //if wall between enemy and player
                    bool wall = hit.collider && hit.collider.gameObject.layer == 8;

                    if (degree < ShootingAngle / 2 && degree > -ShootingAngle / 2 && distance <= 10 && !wall) //if playing in a range of shooting angel, then shoot
                    {

                        shotDetection(ray);
                        addEffects(ray);

                        animator.SetBool("fire", true);
                        gunShotTime = 0.5f;

                        magBulletsVal = magBulletsVal - 1;
                        if (magBulletsVal <= 0 && remainingBulletsVal > 0)
                        {
                            //print("reload, " + gunShotTime + "  " + gunReloadTime);
                            animator.SetBool("reloadAfterFire", true);                            
                            gunReloadTime = 2.5f;
                            Invoke("reloaded", 2.5f);
                        }
                    }
                }
                else
                {
                    animator.SetBool("fire", false);
                }
            }
            else
            {
                animator.SetBool("fire", false);
                //moving along the path
            }

        }
        
       
    }

  

    public void Being_shot(float damage) // getting hit from enemy
    {
        if (!isDead)
        {
            being_shot = true;
            health = health - damage;
            //print("being shot, " + transform.tag + "  " + health);
            if (remainingHealth)
            {
                if (health <= 0){
                    health = 0;
                }
                remainingHealth.text = health.ToString();
            }
        }

    }

    public void ReloadEvent(int eventNumber) // appearing and disappearing the handMag and gunMag
    {
        if (isDead)
        {
            return;
        }
        if (eventNumber == 1){
            gunMag.GetComponent<SkinnedMeshRenderer>().enabled = false;
            handMag.GetComponent<SkinnedMeshRenderer>().enabled = true;
        }            
        else if (eventNumber == 2)
        {
            gunMag.GetComponent<SkinnedMeshRenderer>().enabled = true;
            handMag.GetComponent<SkinnedMeshRenderer>().enabled = false;
        }


    }

    void reloaded()
    {
        if (isDead)
        {
            return;
        }
        int newMagBulletsVal = Mathf.Min(remainingBulletsVal + magBulletsVal, magSize);
        int addedBullets = newMagBulletsVal - magBulletsVal;
        magBulletsVal = newMagBulletsVal;
        remainingBulletsVal = Mathf.Max(0, remainingBulletsVal - addedBullets);
        //if (gameObject.tag != "Player")
        //{
         //   remainingBulletsVal = 90;
        //}
        animator.SetBool("reloadAfterFire", false);
    }

    void updateText()
    {
        magBullets.text = magBulletsVal.ToString() ;
        remainingBullets.text = remainingBulletsVal.ToString();
    }

    bool detectPlayer()
    {
        GameObject player = GameObject.Find("player");
        float distance = (transform.position - player.transform.position).magnitude;
        float SightAngle = 135;
        //float ShootingAngle = 45;

        Vector3 direction = player.transform.position - transform.position;
        float degree = Vector3.Angle(direction, transform.forward);

        RaycastHit hit;
        Vector3 player_pos = player.transform.position;
        player_pos.y = end.transform.position.y;
        Physics.Raycast(end.transform.position, (player_pos - end.transform.position).normalized, out hit, distance);  //if wall between enemy and player
        //print(hit);

        Debug.DrawRay(end.transform.position, (player_pos - end.transform.position).normalized, Color.red);
        bool PlayerInDetectRange = false;
        PlayerInDetectRange = !player.gameObject.GetComponent<Gun>().isDead && distance < 15 && degree < SightAngle / 2 && degree > -SightAngle / 2 &&
            hit.collider && hit.collider.gameObject.layer != 8;
        //print("distance: "+distance);
        if (being_shot || PlayerInDetectRange)
        {
            return true;           

        }       

        return false;
    }

    void shotDetection(Ray ray) // Detecting the object which player shot 
    {
        RaycastHit rayHit;
        if (Physics.Raycast(ray, out rayHit, 100.0f))
        {
            //print("hit on " + rayHit + "   " + rayHit.collider + "   " + rayHit.transform);
            //if (rayHit.collider.transform.tag == "Player" || rayHit.collider.transform.tag == "enemy")

            //{
            Gun script = rayHit.collider.transform.gameObject.GetComponentInParent<Gun>();
            if (script)
            {
                int dmg = 0;
                switch (rayHit.collider.transform.tag)
                {
                    case "arm":
                        dmg = 10;
                        break;
                    case "leg":
                        dmg = 20;
                        break;
                    case "body":
                        dmg = 30;
                        break;
                    case "head":
                        dmg = 100;
                        break;
                    default:
                        dmg = 20;
                        break;
                }
                 
                script.Being_shot(dmg);
            }
            //}
        }
    }

    void addEffects(Ray ray) // Adding muzzle flash, shoot sound and bullet hole on the wall
    {
        RaycastHit rayHit;
        int layerMask = 1 << 8;
        
        if (Physics.Raycast(ray, out rayHit, 100.0f, layerMask))
        {
            GameObject bulletHoleObject = Instantiate(bulletHole, rayHit.point + rayHit.collider.transform.up * 0.01f, rayHit.collider.transform.rotation);
            Destroy(bulletHoleObject, 2.0f);
        }

        GameObject muzzleFlashObject = Instantiate(muzzleFlash, end.transform.position, end.transform.rotation);
        muzzleFlashObject.GetComponent<ParticleSystem>().Play();
        Destroy(muzzleFlashObject, 1.0f);

        Destroy(Instantiate(shootSound, end.transform.position, end.transform.rotation), 1.0f);
    }

    static float NextGaussian(float mean, float variance, float min, float max)
    {
        float x;
        do
        {
            x = NextGaussian(mean, variance);
        } while (x < min || x > max);
        return x;
    }

    static float NextGaussian(float mean, float standard_deviation)
    {
        return mean + NextGaussian() * standard_deviation;
    }

    static float NextGaussian()
    {
        float v1, v2, s;
        do
        {
            v1 = 2.0f * Random.Range(0f, 1f) - 1.0f;
            v2 = 2.0f * Random.Range(0f, 1f) - 1.0f;
            s = v1 * v1 + v2 * v2;
        } while (s >= 1.0f || s == 0f);
        s = Mathf.Sqrt((-2.0f * Mathf.Log(s)) / s);
        return v1 * s;
    }

    public void AddAmmo(int ammo)
    {
        remainingBulletsVal = Mathf.Min(remainingBulletsVal + ammo, 90);
        updateText();
        Destroy(Instantiate(lootSound, end.transform.position, end.transform.rotation), 1f);
    }

    public int GetRemainingBullets()
    {
        return remainingBulletsVal;
    }

}
