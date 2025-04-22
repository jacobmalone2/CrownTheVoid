using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CutsceneControl : MonoBehaviour
{
    [Header("List of Cameras")]
    [SerializeField] List<GameObject> vCam;
    [NonSerialized] GameObject EndingCam;
    [Header("Timings")]
    [SerializeField] float waitTime = 3f;
    private PlayerController pc;
    int increment = 0;
    public void Start()
    {
        // Pause player controls while cutscene is playing
        pc = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        pc.isCutscenePlaying = true;
        pc.isPaused = true;

        EndingCam = GameObject.FindWithTag("vCam");
        StartCoroutine(Phase(vCam, vCam[0], waitTime, increment));
    }

    IEnumerator Phase(List<GameObject> vCam, GameObject activeCam, float waitTime, int increment)
    {
        activeCam.SetActive(true);
        yield return new WaitForSeconds(waitTime);
        increment++;

        if(vCam.Count > increment)
        {
            vCam[increment].SetActive(true);
            activeCam.SetActive(false);
            activeCam = vCam[increment];
            StartCoroutine(Phase(vCam, activeCam, waitTime, increment)); 
        }
        else
        {
            CloseCams(vCam);
            EndingCam.SetActive(true);

            // Resume player control when cutscene is over
            pc.isCutscenePlaying = false;
            pc.isPaused = false;
        }
    }

    void CloseCams(List<GameObject> vCam)
    {
        foreach(GameObject cam in vCam)
        {
            cam.SetActive(false);
        }
    }
}
