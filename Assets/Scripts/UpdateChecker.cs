using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class UpdateChecker : MonoBehaviour
{
    const string Api = "https://api.github.com/repos/itayka112bel-sketch/zero-Hour/releases/latest";

    [SerializeField] GameObject updatePanel;
    [SerializeField] TMP_Text infoText;
    string releaseUrl;

    [Serializable]
    class Release
    {
        public string tag_name;
        public string html_url;
        public string body;
    }

    IEnumerator Start()
    {
        updatePanel.SetActive(false);

        using var req = UnityWebRequest.Get(Api);
        req.SetRequestHeader("User-Agent", "ZeroHour-Game");
        req.SetRequestHeader("Accept", "application/vnd.github+json");
        req.timeout = 10;
        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success) yield break;

        var release = JsonUtility.FromJson<Release>(req.downloadHandler.text);
        if (release == null || string.IsNullOrEmpty(release.tag_name)) yield break;

        if (IsNewer(release.tag_name, Application.version))
        {
            releaseUrl = release.html_url;
            infoText.text = $"Update available: {release.tag_name}\nYour version: V{Application.version}";
            updatePanel.SetActive(true);
        }
    }

    static bool IsNewer(string remoteTag, string localVersion)
    {
        return Version.TryParse(remoteTag.TrimStart('V', 'v'), out var remote)
            && Version.TryParse(localVersion.TrimStart('V', 'v'), out var local)
            && remote > local;
    }

    public void OnDownloadClicked() => Application.OpenURL(releaseUrl);

    public void OnLaterClicked() => updatePanel.SetActive(false);
}
