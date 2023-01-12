using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
        
public class PlayerAnimator : MonoBehaviour, IAnimationStateReader
{
    public static readonly int IsStanding = Animator.StringToHash("IsStanding");
    public static readonly int IsJumping = Animator.StringToHash("IsJumping");
    public static readonly int OnGround = Animator.StringToHash("OnGround");

    public readonly int IdleState = Animator.StringToHash("Idle");
    public readonly int RunState = Animator.StringToHash("Run");
    public readonly int JumpState = Animator.StringToHash("Jump");

    private Animator _animator;
    private Controller2D _controller2D;

    public event Action<AnimatorState> StateEntered;
    public event Action<AnimatorState> StateExited;
    
    public AnimatorState State { get; private set; }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _controller2D = GetComponent<Controller2D>();
    }

    private void Update()
    {
        _animator.SetBool(IsStanding, !(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A)));
        _animator.SetBool(IsJumping, Input.GetKeyDown(KeyCode.W));
        _animator.SetBool(OnGround, _controller2D.OnGround);
    }

    public void PlayJumpAnimation()
    {
        _animator.SetBool(IsJumping, true);
    }
    
    public void PlayRunAnimation()
    {
        _animator.SetBool(IsStanding, false);
    }
    
    public void PlayIdleAnimation()
    {
        _animator.SetBool(IsStanding, true);
    }

    public void EnteredState(int stateHash)
    {
        State = StateFor(stateHash);
        StateEntered?.Invoke(State);
    }
    
    public void ExitedState(int stateHash) => 
        StateExited?.Invoke(StateFor(stateHash));
    
    private AnimatorState StateFor(int stateHash)
    {
        AnimatorState state;
        if (stateHash == IdleState)
            state = AnimatorState.Idle;
        else if (stateHash == RunState)
            state = AnimatorState.Run;
        else if (stateHash == RunState)
            state = AnimatorState.Died;
        else
            state = AnimatorState.Unknown;
      
        return state;
    }
}
}