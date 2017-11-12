using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;

public class SceneAnimator : MonoBehaviour
{
    #region Attributes


    #endregion

    #region Start

    void Start()
    {
    }

    #endregion

    #region Common

    public IEnumerator StartAnimation(string animName, GameObject gameObject)
    {
        Animator animator = gameObject.GetComponent<Animator>();

        Debug.Log(gameObject.name + " animator is " + animator);

        if (animator)
        {
            animator.SetBool(animName, true);

            float animLength = FindAnimLength(animator, animName);
            if (animLength != -1)
            {
                yield return new WaitForSeconds(animLength);

                Debug.Log("Setting animation " + animName + " to false");

                animator.SetBool(animName, false);
            }
            else
            {
                Debug.Log(animName + " animation was not found in " + animator);
            }
        }
    }



    #endregion

    #region Utils

    private float FindAnimLength(Animator animator, string clipName)
    {
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;

        if (ac)
        {
            foreach (AnimationClip clip in ac.animationClips)
            {
                if (clip.name == clipName)
                {
                    return clip.length;
                }
            }
        }

        return -1;
    }

    #endregion

}
