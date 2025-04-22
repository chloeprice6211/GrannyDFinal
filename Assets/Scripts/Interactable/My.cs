using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cinemachine.CinemachinePathBase;

namespace chloeprice
{
    public class My
    {
        public static void PlayReversedAnim(Animation _animation, string clipName)
        {
            float _animCurrentTime = 0;

            if (_animation.isPlaying)
            {
                _animCurrentTime = _animation[clipName].time;
                _animation.Stop();
            }
            else
            {
                _animCurrentTime = _animation[clipName].length;
            }

            _animation[clipName].time = _animCurrentTime;
            _animation[clipName].speed = -1;
            _animation.Play(clipName);
        }
    }
}

