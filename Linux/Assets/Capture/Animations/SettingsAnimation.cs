using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * This is the OLD Animation class that is currently used for the Normal Settings UI. In the future it gets replaced by the new Animation class
 */
public class SettingsAnimation : MonoBehaviour
{
    public void showSettings(bool show)
    {
        if (!show)
        {
            stopAnimation();
            return;
        }
        startAnimation();
    }

    private void startAnimation()
    {
        LeanTween.cancel(gameObject);
        gameObject.SetActive(true);
        Vector3 vector3 = transform.position;
        vector3.x = -512f;
        transform.position = vector3;

        LeanTween.moveX(gameObject, 0f, 0.15f);
    }

    private void stopAnimation()
    {
        LeanTween.cancel(gameObject);
        LeanTween.moveX(gameObject, -512f, 0.15f).setOnComplete(hideSettingsMenu);
    }

    private void hideSettingsMenu()
    {
        gameObject.SetActive(false);
    }
}
