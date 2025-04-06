using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CutsceneControl : MonoBehaviour
{
    [Header("List of Cameras")]
    [SerializeField] List<GameObject> vCam;
    [SerializeField] GameObject EndingCam;
    [Header("Timings")]
    [SerializeField] float waitTime = 3f;
    int increment = 0;
    public void Start()
    {
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
