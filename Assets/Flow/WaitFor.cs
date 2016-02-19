/**
@file WaitFor.cs
@author t-sakai
@date 2016/02/19
*/
using System.Collections;

namespace Flow
{
    public static class WaitFor
    {
        public static IEnumerator seconds(float seconds)
        {
            while(0.0f<seconds){
                yield return null;
                seconds -= UnityEngine.Time.deltaTime;
            }
        }

        public static IEnumerator seconds(float seconds, float timeScale)
        {
            while(0.0f<seconds){
                yield return null;
                seconds -= timeScale * UnityEngine.Time.deltaTime;
            }
        }
    }
}
