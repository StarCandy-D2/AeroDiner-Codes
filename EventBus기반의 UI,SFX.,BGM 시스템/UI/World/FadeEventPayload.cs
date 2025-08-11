public class FadeEventPayload
{
    public float targetAlpha = 1f;
    public float duration = -1f;
    public string targetScene = null;
    public bool autoFadeInAfterSceneLoad = false; // 추가됨

    public FadeEventPayload(float alpha = 1f, float duration = -1f, string scene = null, bool autoFade = false)
    {
        this.targetAlpha = alpha;
        this.duration = duration;
        this.targetScene = scene;
        this.autoFadeInAfterSceneLoad = autoFade;
    }
}