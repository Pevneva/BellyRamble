using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class Participant : MonoBehaviour
{
    [SerializeField] private int _score;
    [SerializeField] private ParticipantDataView _view;
    [SerializeField] private GameObject _crown;
    [SerializeField] private GameObject _lefLegParticipant;
    [SerializeField] private GameObject _rightLegParticipant;
    [SerializeField] private GameObject _backParticipant;

    public event UnityAction<int> ScoreChanged;
    public event UnityAction<Food> FoodEatenByBot;

    private Vector3 _scale;
    private Rigidbody _rigidbody;
    private float _collisionCounter;
    private float _collisionDelay;
    private GameObject _collisionGameObject;
    private BattleController _battleController;

    public int Score
    {
        get { return _score; }
    }

    protected void Start()
    {
        _collisionCounter = 0;
        _collisionDelay = 0.75f;
        _rigidbody = GetComponent<Rigidbody>();
        _view.Render(this);
        _scale = transform.localScale;
        _rigidbody.isKinematic = true;
        ScoreChanged += OnScoreChanged;
        _battleController = FindObjectOfType<BattleController>();
    }

    private void Update()
    {
        _view.Render(this);
    }

    private void OnScoreChanged(int score)
    {
        SetScale(score); //to improve
    }

    private void SetScale(int score)
    {
        _scale += new Vector3(1f, 0.1f, 1f) * 0.01f / (float) score;
        transform.localScale = _scale;
    }

    private void AddScore(int score)
    {
        _score += score;
        _view.Render(this);
        ScoreChanged?.Invoke(score);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Food food))
        {
            Destroy(other.gameObject);
            AddScore(food.Reward);

            if (this is Bot)
            {
                FoodEatenByBot?.Invoke(food);
            }
        }

        if (other.gameObject.TryGetComponent(out Bot bot))
        {
            var mover = gameObject.GetComponent<ParticipantMover>();
            var otherMover = other.gameObject.GetComponent<ParticipantMover>();

            if (mover.IsFlying || otherMover.IsFlying)
                return;
            
            if (mover.IsBoosting || otherMover.IsBoosting)
            {
                bot.GetComponent<ParticipantMover>().StopBoost();
                GetComponent<ParticipantMover>().StopBoost();

                _battleController.DoImpact(bot, this); //uncomment to do

                return;
            } 

            Rigidbody otherRigidbody = other.gameObject.GetComponent<Rigidbody>();
            otherRigidbody.isKinematic = false;
            _rigidbody.isKinematic = false;

            StartCoroutine(DoKinematic(0.5f, otherRigidbody, _rigidbody));
        }
    }
    
    private IEnumerator DoKinematic(float delay, Rigidbody rigidbody1, Rigidbody rigidbody2)
    {
        yield return new WaitForSeconds(delay);
        rigidbody1.isKinematic = true;
        rigidbody2.isKinematic = true;
    }

    private Participant GetLoser(Participant first, Participant second)
    {
        return first.Score < second.Score ? first : second;
    }

    private void OnDestroy()
    {
        Destroy(_view.gameObject);
    }
    
    public void PlayParticipantEffects()
    {
        _lefLegParticipant.SetActive(true);
        _rightLegParticipant.SetActive(true);
        _backParticipant.SetActive(true);
    }

    public void StopParticipantEffects()
    {
        _lefLegParticipant.SetActive(false);
        _rightLegParticipant.SetActive(false);
        _backParticipant.SetActive(false);        
    }

    public void TurnOnCrown()
    {
        _crown.SetActive(true);
    }
    public void TurnOffCrown()
    {
        _crown.SetActive(false);
    }
}