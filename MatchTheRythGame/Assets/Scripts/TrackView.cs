
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(VerticalLayoutGroup))]
[RequireComponent(typeof(ContentSizeFitter))]
[RequireComponent(typeof(RectTransform))]
public class TrackView : MonoBehaviour
{
    public enum Trigger{Missed, Right, Wrong }

    [SerializeField] RectTransform _empty;
    [SerializeField] RectTransform _up;
    [SerializeField] RectTransform _down;
    [SerializeField] RectTransform _right;
    [SerializeField] RectTransform _left;

    RectTransform _rTransform;
    List<Image> _beatViews=new List<Image>();

    float _beatViewSize;
    float _spacing;

    Vector2 _position;
    public float Position
    {
        get { return _position.y; }
        set 
        {
            if (value != _position.y) _position.y = value;
            _rTransform.anchoredPosition = _position;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        InitializeTracView(GamePlayController.Instance.Track);
    }

    private void Update()
    {
        Position -= (_beatViewSize+_spacing)*Time.deltaTime *GamePlayController.Instance.beatsPerSecond;
    }

    public void InitializeTracView(Track track)
    {
        _rTransform = (RectTransform)this.transform;
        _position = _rTransform.anchoredPosition;
        _spacing = GetComponent<VerticalLayoutGroup>().spacing;
        _beatViewSize = _empty.rect.height;

        foreach (int b in track.beats)
        {
            GameObject g;
            switch (b)
            {
                case 0:
                    g = _left.gameObject;
                    break;
                case 1:
                    g = _down.gameObject;
                    break;
                case 2:
                    g = _up.gameObject;
                    break;
                case 3:
                    g = _right.gameObject;
                    break;
                default:
                    g = _empty.gameObject;
                    break;
            }

            //Since track is inverted, we move each bit to the top after instantiation.
            Transform view = GameObject.Instantiate(g, this.transform).transform;
            view.SetAsFirstSibling();

            //Populate the list with the images that represent each beat in the track.
            _beatViews.Add(view.GetComponent<Image>());
            
        }
    }

    public void TriggerBeatView(int beatIndex, Trigger trigger)
    {
        switch (trigger)
        {
            case Trigger.Missed:
                _beatViews[beatIndex].color = Color.grey;
                //Debug.Break();
                break;
            case Trigger.Right:
                _beatViews[beatIndex].color = Color.yellow;
                break;
            case Trigger.Wrong:
                _beatViews[beatIndex].color = Color.cyan;
                break;
            default: 
                break;
        }
    }
}
