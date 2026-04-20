using UnityEngine;
using UnityEngine.UI;

//
// UI표시
//
public class ScoreHud : MonoBehaviour
{
    [SerializeField] Text label;

    void OnEnable()
    {
        GameScore.Changed += OnChanged;
        OnChanged(GameScore.Current);
    }

    void OnDisable()
    {
        GameScore.Changed -= OnChanged;
    }

    void OnChanged(int v)
    {
        if (label != null)
            label.text = "점수: " + v;
    }

    public void Bind(Text t) => label = t;
}
