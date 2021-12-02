using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Participant : MonoBehaviour
{
    [SerializeField] private int _score;
    [SerializeField] private PlayerDataView _view;

    public event UnityAction<int> ScoreChanged;
    private Vector3 _scale;

    public int Score { get { return _score; } }

    
    private void Start()
    {
        _view.Render(this);
        _scale = transform.localScale;
        // Debug.Log("AAA-7 _scale : " + _scale);
    }

    private void Update()
    {
        _view.Render(this);
        // SetScale(Score);
    }
    
    private void SetScale(int score)
    {
        // Debug.Log("AAA-7-1 OnScoreChanged : " + score);
        // Debug.Log("AAA-7-1a score/(float)10 : " + score/(float)10);
        // Debug.Log("AAA-7-1b _scale * score/(float)10 : " + _scale * score/(float)10);
        _scale += _scale * score/(float)200;
        // Debug.Log("AAA-7-2 OnScoreChanged : " + score);
        // Debug.Log("AAA-7-3 _scale : " + _scale);
        transform.localScale = _scale;
    }

    private void AddScore(int score)
    {
        _score += score;
        _view.Render(this);
        // Debug.Log("SCORE: " + Score);
        // ScoreChanged?.Invoke(score);
        SetScale(score);
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.TryGetComponent(out Food food))
        {
            Destroy(other.gameObject);
            AddScore(food.Reward);
        }
    }
}
