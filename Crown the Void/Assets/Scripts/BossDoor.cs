using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossDoor : MonoBehaviour, IInteractable
{
    private readonly bool m_isItem = false;
    private readonly bool m_isKeyItem = false;
    private readonly int m_interactPriority = 0;
    private string m_interactionPrompt = "Find 3 Keys";
    private string m_foundKeysPrompt = "Proceed";
    private bool m_hasInteractedWith = false;

    private InteractionPopUpBehavior m_popUp;
    private KeyManager m_keyManager;

    public GameObject Player;
    public string SceneName;

    public int InteractPriority => m_interactPriority;
    public string InteractionPrompt => m_interactionPrompt;
    public bool HasInteractedWith => m_hasInteractedWith;
    public bool IsItem => m_isItem;
    public bool IsKeyItem => m_isKeyItem;

    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindWithTag("Player");
        m_popUp = GetComponentInChildren<InteractionPopUpBehavior>();
        m_keyManager = Player.GetComponent<KeyManager>();
    }

    public bool Interact(Interactor interactor)
    {
        return m_hasInteractedWith;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == Player)
        {
            if (m_keyManager.HasShadowKey && m_keyManager.HasBloodKey && m_keyManager.HasVoidKey)
            {
                SceneManager.LoadScene(SceneName);
                Player.transform.position = new Vector3(0, 2, -15);
            }
        }
    }

    public void ShowPopUp(string prompt)
    {
        if (m_keyManager.HasShadowKey && m_keyManager.HasBloodKey && m_keyManager.HasVoidKey)
        {
            m_popUp.ShowPopUp(m_foundKeysPrompt);
        }
        else
        {
            m_popUp.ShowPopUp(prompt);
        }
    }
}