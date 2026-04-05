using UnityEngine;

/*
 * This class is the new Animator for the CaptureCardSettings and Infp UI.
 * i will also add this class to the Normal Settings UI but right now im to lazy tbh
 * 
 * i use the LeanTween library for animation
 */
public class ScreenAnimation : MonoBehaviour
{
    public GameObject startAnimationPoint;
    public GameObject endAnimationPoint;

    public float time = 0.15f;
    public void show(bool show)
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
        vector3.y = startAnimationPoint.transform.position.y;
        transform.position = vector3;

        LeanTween.move(gameObject, endAnimationPoint.transform.position, time);
    }

    private void stopAnimation()
    {
        if (gameObject.active)
        {
            LeanTween.cancel(gameObject);
            LeanTween.move(gameObject, startAnimationPoint.transform.position, time).setOnComplete(hideSettingsMenu);
        }
    }

    private void hideSettingsMenu()
    {
        gameObject.SetActive(false);
    }
}

