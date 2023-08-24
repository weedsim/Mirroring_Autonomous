using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformPhase2to3 : MonoBehaviour
{

    /*GameObject FrontWall;
    GameObject BackWall;
    GameObject LeftWall;
    GameObject RightWall;*/

    void Start()
    {
        /*FrontWall = GameObject.Find("WallFront");
        BackWall = GameObject.Find("WallBack");
        LeftWall = GameObject.Find("WallLeft");
        RightWall = GameObject.Find("WallRight");*/
        
        /*SmokeParticle.Play();*/

        GetComponent<Rigidbody>().isKinematic = true;

        Phase2To3 tmp = transform.parent.parent.GetComponent<Phase2To3>();

        tmp.BreakWall -= ExplosionWall;
        tmp.BreakWall += ExplosionWall;
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetMouseButtonDown(0)) {

            Invoke("ExplosionWall", 3.5f);
            Invoke("DestroyWall", 10f);
            *//*Invoke("SmokeParticle.Play()", 3f);*/
            /*GetComponent<Rigidbody>().isKinematic = false;*/
            /*Invoke("FrontWallExplosion(FrontWall)", 10f);
            Invoke("BackWallExplosion(BackWall)", 10f);*/

            /*FrontWallExplosion(FrontWall);
            BackWallExplosion(BackWall);
            LeftWallExplosion(LeftWall);
            RightWallExplosion(RightWall);*//*
        }*/
    }
    /*
        public void FrontWallExplosion(GameObject _ob)
        {
            Vector3 speed = new Vector3(-100, 0, 0);
            GetComponent<Rigidbody>().AddForce(speed);
        }

        public void BackWallExplosion(GameObject _ob)
        {
            Vector3 speed = new Vector3(100, 0, 0);
            GetComponent<Rigidbody>().AddForce(speed);
        }
        public void LeftWallExplosion(GameObject _ob)
        {
            Vector3 speed = new Vector3(0, 0, -100);
            GetComponent<Rigidbody>().AddForce(speed);
        }

        public void RightWallExplosion(GameObject _ob)
        {
            Vector3 speed = new Vector3(0, 0, 100);
            GetComponent<Rigidbody>().AddForce(speed);
        }
    */
    public void ExplosionWall()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.AddForce(-transform.forward * 200);
    }
}
