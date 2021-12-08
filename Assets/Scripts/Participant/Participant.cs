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
    public event UnityAction RopeTouchStarted;
    public event UnityAction RopeTouchEnded;
    public event UnityAction<Participant> ParticipantsTouched;
    public bool IsRopeTouching;
    
    private Vector3 _scale;
    private Rigidbody _rigidbody;
    private float _collisionCounter;
    private float _collisionDelay;
    private GameObject _collisionGameObject;

    public int Score { get { return _score; } }
    
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
        _scale += new Vector3(1f,0.1f,1f) *  0.01f/(float)score;
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
        }
        
        if (other.gameObject.TryGetComponent(out BorderPrev borderPrev))
        {
            Debug.Log("AAA-135 Border Prev Enter");
            GetComponent<Rigidbody>().isKinematic = false;
        } 
        
        if (other.gameObject.TryGetComponent(out Border border))
        {
            Debug.Log("AAA-135 Border Enter " + Time.time );
            RopeTouchStarted?.Invoke();
            IsRopeTouching = true;
            _collisionCounter = 0;
        } 
        
        if (other.gameObject.TryGetComponent(out Bot bot))
        {
            Debug.Log("AAA-138 BOT !!!");
            ParticipantsTouched?.Invoke(GetLoser(bot, this));
            Debug.Log("AAA-138 WINNER : " + GetLoser(bot, this));
        }
    }

    private void OnTriggerStay(Collider other)
    {
                
        if (other.gameObject.TryGetComponent(out BorderPrev2 borderPrev2))
        {
            // Debug.Log("AAA-135 Border Prev 222 Stay");
            GetComponent<Rigidbody>().isKinematic = true;
        }   
        
        if (other.gameObject.TryGetComponent(out Border border))
        {
            _collisionCounter += Time.deltaTime;

            if (_collisionCounter >= _collisionDelay)
            {
                _collisionGameObject = other.gameObject;
                _collisionGameObject.SetActive(false);
                StartCoroutine(DoActiveCollisionObject(0.75f));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Border border))
        {
            Debug.Log("AAA-135 Border Exit");
            IsRopeTouching = false;
            RopeTouchEnded?.Invoke();
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

    // private void OnCollisionEnter(Collision other)
    // {
    //     if (other.gameObject.TryGetComponent(out Rope rope))
    //     {
    //         RopeTouchStarted?.Invoke();
    //     }
    // }
    //
    // private void OnCollisionExit(Collision other)
    // {
    //     if (other.gameObject.TryGetComponent(out Rope rope))
    //     {
    //         RopeTouchEnded?.Invoke();
    //     }        
    // }


    // private void OnTriggerExit(Collider other)
    // {
    //     // Debug.Log("AAA-13 Touch exit");
    //     if (other.gameObject.TryGetComponent(out Border border))
    //     {
    //         // Debug.Log("AAA-13 Touch border ENDED");
    //         // _rigidbody.AddForce(new Vector3(1,0,0), ForceMode.VelocityChange);
    //         // _rigidbody.isKinematic = true;
    //         RopeTouchEnded?.Invoke();
    //     }
    //     else if (other.gameObject.TryGetComponent(out Rope rope))
    //     {
    //         Debug.Log("AAA-11 Touch rope ENDED");
    //         // _rigidbody.AddForce(new Vector3(1,0,0), ForceMode.VelocityChange);
    //         // _rigidbody.isKinematic = true;
    //         RopeTouchEnded?.Invoke();
    //     }   
    // }
    // private void OnTrigge(Collision other)
    // {
    //     if (other.gameObject.TryGetComponent(out Rope rope))
    //     {
    //         Debug.Log("AAA-11 Touch rope ENDED");
    //         // _rigidbody.AddForce(new Vector3(1,0,0), ForceMode.VelocityChange);
    //         // _rigidbody.isKinematic = true;
    //         RopeTouchEnded?.Invoke();
    //     }        
    // }
}
