using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonClickAnimation : MonoBehaviour
{
    public GameObject button;
    public void onButtonClickAnimation()
    {
        LeanTween.cancel(gameObject);
        button.transform.localScale = Vector3.zero;
    }
}
