using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject playerObject;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("StartingRoom");
        Instantiate(playerObject, new Vector3(0, 0, 0), Quaternion.identity).name = "PlayerObj";
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
