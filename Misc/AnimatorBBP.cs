using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NullscapeinBB.Misc
{
    public struct Animation {
        public Sprite[] animation;
        public float speed;
    }
    public class AnimatorBBP : MonoBehaviour
    {
        public SpriteRenderer renderer;
        public float framesPerSeconds = 0;
        public float currentTime = 0f;
        public const string DefaultAnimation = "Default";
        public Sprite[] CurrentAnimation;

        private Dictionary<string, Animation> AnimationsStorage = new();

        private Queue<string> QueuedAnimations = new();

        public void AddAnimationToStorage(string name, Sprite[] Animation, float speed) {
            AnimationsStorage.Add(name, new()
            {
                animation = Animation,
                speed = speed
            });
        }

        public void OnAnimationFinished() {
            if (QueuedAnimations.Count > 0) {
                
                SwitchAnimation(QueuedAnimations.Dequeue());
            }
        }

        public void Update() {
            currentTime = Time.deltaTime * framesPerSeconds;
            if (Mathf.Round(currentTime) >= CurrentAnimation.Length) {
                currentTime = 0;
                OnAnimationFinished();
            }
            renderer.sprite = CurrentAnimation[Mathf.Clamp(Mathf.RoundToInt(currentTime), 0, CurrentAnimation.Length)];
        }

        public void SwitchAnimation(string animation) {
            var anim = AnimationsStorage[animation];
            CurrentAnimation = anim.animation;
            framesPerSeconds = anim.speed;

        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="playnow">Plays first and others after</param>
        /// <param name="ClearQueue"></param>
        public void QueueAnimation(string name, bool playnow = false, bool ClearQueue = false) {
            if (ClearQueue) QueuedAnimations.Clear();
            if (playnow) SwitchAnimation(name);

            QueuedAnimations.Enqueue(name);
        }

        public static AnimatorBBP Setup(SpriteRenderer renderer, Sprite[] DefaultAnimation, float framesPerSeconds) {
            var anim = renderer.gameObject.AddComponent<AnimatorBBP>();
            anim.CurrentAnimation = DefaultAnimation;
            anim.framesPerSeconds = framesPerSeconds;

            anim.AnimationsStorage.Add("Default", new() {
                speed = framesPerSeconds,
                animation = DefaultAnimation
            });
            return anim;
        }

    }
}
