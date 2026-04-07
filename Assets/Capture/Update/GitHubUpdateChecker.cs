using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;
using Unity.VisualScripting;

[Serializable]
public class GitHubRelease // Hilfsklasse für das JSON-Parsing
{
    public string tag_name;
}

public class GitHubUpdateChecker : MonoBehaviour
{
    public VideoGameCaptureController videoGameCaptureController;

    string currentVersion = "0.0.12";
    public string githubUser = "ImmerNochNoah";
    public string repoName = "VideoGameCapture";

    public string latestVersion;

    public GameObject updateBox;

    public IEnumerator CheckForUpdates()
    {
        yield return new WaitForSeconds(1F);
        if (videoGameCaptureController.saveSystem.getSetting().checkForUpdates)
        {
            Debug.Log("Checking for updates!");
            string url = $"https://api.github.com/repos/{githubUser}/{repoName}/releases/latest";

            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                // GitHub verlangt oft einen User-Agent Header
                webRequest.SetRequestHeader("User-Agent", "Unity-Update-Checker");

                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    // JSON parsen
                    GitHubRelease latestRelease = JsonUtility.FromJson<GitHubRelease>(webRequest.downloadHandler.text);
                    latestVersion = latestRelease.tag_name.Replace("v", ""); // "v1.1" -> "1.1"

                    if (IsNewer(latestVersion, currentVersion))
                    {
                        Debug.Log($"Update! now: {currentVersion}, New: {latestVersion}");
                        showUpdateBox(true);
                    }
                    else
                    {
                        Debug.Log("VGC is Uptodate!");
                    }
                }
                else
                {
                    Debug.LogError("Error: " + webRequest.error);
                }
            }
        }
        else
        {
            Debug.Log("User has checking for updates off!");
        }
    }

    public string getCurrentVersion()
    {
        return currentVersion;
    }

    public void showUpdateBox(bool show)
    {
        updateBox.SetActive(show);
    }
    bool IsNewer(string latest, string current)
    {
        // Einfacher Vergleich von Versions-Strings
        Version vLatest = new Version(latest);
        Version vCurrent = new Version(current);
        return vLatest > vCurrent;
    }
}
