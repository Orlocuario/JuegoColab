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
		animLengths = new Dictionary<string, float>(); 
        animators = GameObject.FindObjectsOfType<Animator>();
		Debug.Log ("Found " + animators.Length + " animators");
		LoadAnimatorsData();
    }

    #region Common

    public IEnumerator StartAnimation(string animName, GameObject gameObject)
    {
        Animator animator = FindAnimator(gameObject.name);

		Debug.Log (gameObject.name + " animator is " + animator);

        if (animator)
        {
            animator.SetBool(animName, true);

			float animLength = animLengths[gameObject.GetInstanceID () + "/" + animName];
            yield return new WaitForSeconds(animLength);

			Debug.Log ("Setting animation " + animName + " to false"); 

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

			if (ac) 
				{
					foreach (AnimationClip clip in ac.animationClips) {
					animLengths.Add (animator.gameObject.GetInstanceID () + "/" + clip.name, clip.length);
				}
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
