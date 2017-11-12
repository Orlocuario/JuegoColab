using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;

public class SceneAnimator : MonoBehaviour
{

    private Dictionary<string, float> animLengths;
    private Animator[] animators;

    void Start()
    {
        animators = GameObject.FindObjectsOfType<Animator>();
        LoadAnimatorsData();
    }

    #region Common

    public IEnumerator StartAnimation(string animName, GameObject gameObject)
    {
        Animator animator = FindAnimator(gameObject.name);

        if (animator)
        {
            animator.SetBool(animName, true);

            float animLength = animLengths[animName];
            yield return new WaitForSeconds(animLength);

            animator.SetBool(animName, false);
        }
    }

    #endregion

    #region Utils

    private void LoadAnimatorsData()
    {
        foreach (Animator animator in animators)
        {
            RuntimeAnimatorController ac = animator.runtimeAnimatorController;

            foreach (AnimationClip clip in ac.animationClips)
            {
                animLengths.Add(clip.name, clip.length);
            }
        }
    }

    private Animator FindAnimator(string name)
    {
        Animator animator = null;

        for (int i = 0; i < animators.Length; i++)
        {
            if (animators[i].gameObject.name == name)
            {
                animator = animators[i];
            }
        }

        return animator;
    }

    #endregion

}
