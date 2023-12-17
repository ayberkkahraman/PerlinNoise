using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FpsCounter : MonoBehaviour
{
    private int _count;

    private TMP_Text text;
    private IEnumerator Start()
    {
        text = GetComponent<TMP_Text>();
        while (true)
        {
            _count = (int)(1f / Time.unscaledDeltaTime);
            yield return new WaitForSeconds(0.1f);
            text.text = $"Fps : {_count}";
        }
    }
}
