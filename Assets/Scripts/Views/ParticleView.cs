using UnityEngine;

public class ParticleView : MonoBehaviour
{
    [SerializeField] private RectTransform thisTransform;
    [SerializeField] private ParticleSystem thisParticle;
    
    public void SetPosition(Vector2 pos)
    {
        thisTransform.anchoredPosition = pos;
        thisParticle.Play();
    }

    public bool IsAlive()
    {
        return thisParticle.IsAlive();
    }
}
