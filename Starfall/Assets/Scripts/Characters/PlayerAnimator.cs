using UnityEngine;

namespace Characters
{
    public class PlayerAnimator
    {
        private readonly Animator _animator;

        // Define static hashes for the animator parameters to reduce the number of string lookups at runtime
        private static readonly int Left = Animator.StringToHash("Left");
        private static readonly int Right = Animator.StringToHash("Right");
        private static readonly int Up = Animator.StringToHash("Up");
        private static readonly int Down = Animator.StringToHash("Down");

        public PlayerAnimator(Animator animator)
        {
            _animator = animator;
        }

        public void AnimatePlayer(Vector2 rawInput)
        {
            // Check the player's horizontal input
            switch (rawInput.x)
            {
                case -1:
                    _animator.SetBool(Left, true);
                    _animator.SetBool(Right, false);
                    _animator.SetBool(Up, false);
                    _animator.SetBool(Down, false);
                    break;
                case 1:
                    _animator.SetBool(Right, true);
                    _animator.SetBool(Left, false);
                    _animator.SetBool(Up, false);
                    _animator.SetBool(Down, false);
                    break;
                default:
                    _animator.SetBool(Right, false);
                    _animator.SetBool(Left, false);
                    break;
            }

            // Check the player's vertical input
            switch (rawInput.y)
            {
                case -1:
                    _animator.SetBool(Up, false);
                    _animator.SetBool(Down, true);
                    _animator.SetBool(Right, false);
                    _animator.SetBool(Left, false);
                    break;
                case 1:
                    _animator.SetBool(Up, true);
                    _animator.SetBool(Down, false);
                    _animator.SetBool(Right, false);
                    _animator.SetBool(Left, false);
                    break;
                default:
                    _animator.SetBool(Up, false);
                    _animator.SetBool(Down, false);
                    break;
            }
        }
    }
}