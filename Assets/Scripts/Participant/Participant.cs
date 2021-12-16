using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class Participant : MonoBehaviour
{
    [SerializeField] private int _score;
    [SerializeField] private ParticipantDataView _view;

    public event UnityAction<int> ScoreChanged;
    public event UnityAction RopeTouchDuringMoving;
    public event UnityAction<Food> FoodEatenByBot;
    public event UnityAction RopeTouchEnded;
    public event UnityAction<Participant> ParticipantsTouched;
    public bool IsRopeTouching;

    private Vector3 _scale;
    private Rigidbody _rigidbody;
    private float _collisionCounter;
    private float _collisionDelay;
    private GameObject _collisionGameObject;

    public int Score
    {
        get { return _score; }
    }

    private void Start()
    {
        IsRopeTouching = false;
        _collisionCounter = 0;
        _collisionDelay = 0.75f;
        _rigidbody = GetComponent<Rigidbody>();
        _view.Render(this);
        _scale = transform.localScale;
        _rigidbody.isKinematic = true;
        ScoreChanged += OnScoreChanged;
    }

    private void Update()
    {
        _view.Render(this);
    }

    private void OnScoreChanged(int score)
    {
        // SetScale(score); //to improve
    }

    private void SetScale(int score)
    {
        // _scale += _scale * score/(float)450;
        _scale += new Vector3(1f, 0.1f, 1f) * 0.01f / (float) score;
        transform.localScale = _scale;
    }

    private void AddScore(int score)
    {
        _score += score;
        _view.Render(this);
        // Debug.Log("SCORE: " + Score);
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
                Debug.Log("AAA I'm bot!");
                FoodEatenByBot?.Invoke(food);
            }
        }

        if (other.gameObject.TryGetComponent(out Bot bot))
        {
            Debug.Log("AAA-138 BOT !!!");
            ParticipantsTouched?.Invoke(GetLoser(bot, this));
            Debug.Log("AAA-138 LOSER : " + GetLoser(bot, this));
        }
    }

    private IEnumerator DoActiveCollisionObject(float delay)
    {
        yield return new WaitForSeconds(delay);
        _collisionGameObject.SetActive(true);
    }

    private Participant GetLoser(Participant first, Participant second)
    {
        return first.Score < second.Score ? first : second;
    }
}