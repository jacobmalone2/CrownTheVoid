using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ArrowCountUI : MonoBehaviour
{
    private readonly Color32 AROW_COUNT_COLOR = new Color32(115, 72, 0, 255);
    private readonly Color32 RELOAD_COLOR = new Color32(200, 82, 82, 255);

    private TextMeshProUGUI m_arrowCountText;

    // Start is called before the first frame update
    void Start()
    {
        m_arrowCountText = GetComponentInChildren<TextMeshProUGUI>();
    }

    // Sets the arrow count text to remaining arrows
    public void SetArrowCount(int arrows)
    {
        m_arrowCountText.text = arrows + "/3";
        m_arrowCountText.color = AROW_COUNT_COLOR;
        m_arrowCountText.fontSize = 80f;
    }

    // Sets the text to a reload tip
    public void SetReloadText()
    {
        m_arrowCountText.text = "(R) Reload";
        m_arrowCountText.color = RELOAD_COLOR;
        m_arrowCountText.fontSize = 32f;
    }
}
