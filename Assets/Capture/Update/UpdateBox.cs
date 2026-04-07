using TMPro;
using UnityEngine;

public class UpdateBox : MonoBehaviour
{
    public GitHubUpdateChecker gitHubUpdateChecker;
    public TMP_Text versionText;

    public void OnEnable()
    {
        versionText.text = $"{gitHubUpdateChecker.getCurrentVersion()} to {gitHubUpdateChecker.latestVersion}";
        Cursor.visible = true;
    }
    public void onButtonClickClose()
    {
        closeUpdateWindow();
    }
    
    public void downloadFromGitHub()
    {
        Application.OpenURL("https://github.com/ImmerNochNoah/VideoGameCapture/releases");
    }

    public void downloadFromItchIo()
    {
        Application.OpenURL("https://immernochnoah.itch.io/videogamecapture/devlog");
    }

    public void onButtonClickNeverCheckForUpdates()
    {
        vgcSettings vgcSettings = gitHubUpdateChecker.videoGameCaptureController.saveSystem.getSetting();
        vgcSettings.checkForUpdates = false;
        gitHubUpdateChecker.videoGameCaptureController.saveSystem.setSetting(vgcSettings);
        closeUpdateWindow();
        Debug.Log("User selected never check for updates.");
    }


    public void closeUpdateWindow()
    {
        gitHubUpdateChecker.updateBox.SetActive(false);
        Debug.Log("Closing update window!");
    }
}
