using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideoScene : MonoBehaviour
{
    public VideoPlayer video;

    private void Start()
    {
        video.GetComponent<VideoPlayer>();
        video.Play();
        video.loopPointReached += CheckOver;
    }

    private void CheckOver(UnityEngine.Video.VideoPlayer vp)
    {
        SceneManager.LoadScene("Level1");
    }
    
}
