using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attach to any UI Text GameObject to make it blink on and off.
/// </summary>
public class BlinkText : MonoBehaviour
{
    [Tooltip("Seconds the text stays visible.")]
    public float onDuration = 0.8f;

    [Tooltip("Seconds the text stays hidden.")]
    public float offDuration = 0.4f;

    private Text _text;
    private float _timer;
    private bool _visible = true;

    void Awake()
    {
        _text = GetComponent<Text>();
        _timer = onDuration;
    }

    void Update()
    {
        _timer -= Time.deltaTime;

        if (_timer <= 0f)
        {
            _visible = !_visible;
            _text.enabled = _visible;
            _timer = _visible ? onDuration : offDuration;
        }
    }
}
