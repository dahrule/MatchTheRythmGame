using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayController : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] KeyCode _left;
    [SerializeField] KeyCode _down;
    [SerializeField] KeyCode _right;
    [SerializeField] KeyCode _up;

    [Header("Track")]
    [Tooltip("Beats Track to play")]
    [SerializeField] Track _track;
    /// <summary>
    /// The current track
    /// </summary>
    public Track Track{get { return _track; }}

    public float secondsPerBeat { get; private set; }
    public float beatsPerSecond { get; private set; }
    private bool _completed;
    private bool _played;
    TrackView _trackView;

    private WaitForSeconds waitAndStop;

    private static GamePlayController _instance;
    public static GamePlayController Instance
    {
        get
        {
            if (_instance == null)
                _instance = (GamePlayController)GameObject.FindObjectOfType(typeof(GamePlayController));
            return _instance;
        }
    }

    private int _current;
    public int Current
    {
        get { return _current; }
        set
        {
            if (value != _current)
            {
                _current = value;
                //Stop track playing(moving) at last beat
                if (_current == _track.beats.Count)
                {
                    CancelInvoke("NextBeat");
                    _completed = true;
                    StartCoroutine(WaitAndStop());
                }
            }
        }
    }

    private void OnDestroy()
    {
        _instance = null;
    }
    private void Awake()
    {
        _instance = this;

        beatsPerSecond = _track.bpm / 60f;
        secondsPerBeat = 60f / _track.bpm;
        waitAndStop = new WaitForSeconds(secondsPerBeat*2);

        _trackView = FindObjectOfType<TrackView>();
        if (!_trackView) Debug.LogWarning("No track view found in scene");
    }
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("NextBeat",0f,secondsPerBeat);
    }

    // Update is called once per frame
    void Update()
    {
        if (_played || _completed) return;

        if (Input.GetKeyDown(_left)) PlayBeat(0);
        if (Input.GetKeyDown(_down)) PlayBeat(1);
        if (Input.GetKeyDown(_up)) PlayBeat(2);
        if (Input.GetKeyDown(_right)) PlayBeat(3);
    }

    void PlayBeat(int beatInput)
    {
        _played = true;

        if(_track.beats[Current]==-1) //case beat played untimely.
        {
            return;
        }
        else if (_track.beats[Current] == beatInput) //case beat played correctly.
        {
            _trackView.TriggerBeatView(Current, TrackView.Trigger.Right);
        }
        else
        {
            _trackView.TriggerBeatView(Current, TrackView.Trigger.Wrong); // case beat played wrong.
        }
    }
    void NextBeat()
    {
        //not played and not equals an empty beat.
        if (!_played && _track.beats[Current] != -1)
        {
            _trackView.TriggerBeatView(Current, TrackView.Trigger.Missed);
        }
        
        _played = false;
        Current++;
    }

    private IEnumerator WaitAndStop()
    {
        yield return waitAndStop;
        enabled = false;
    }
}
