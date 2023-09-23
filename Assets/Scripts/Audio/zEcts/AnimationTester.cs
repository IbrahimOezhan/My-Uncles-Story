/*using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

public class AnimationTester : EditorWindow
{
    [MenuItem("Tools/Animator State Tester")]
    private static void OpenWindow()
    {
        GetWindow<AnimationTester>();
    }

    private GameObject targetGO;
    private Animator animator;
    private AnimatorController controller;

    private void OnGUI()
    {
        EditorGUI.BeginChangeCheck();
        targetGO = (GameObject)EditorGUILayout.ObjectField("Target Character: ", targetGO, typeof(GameObject), true);
        if (targetGO == null)
        {
            EditorGUILayout.HelpBox("Select a gameObject to work with", MessageType.Info);
            animator = null;
            controller = null;
        }


        if (targetGO == null)
        {
            return;
        }

        Debug.Log("Change hit");
        animator = targetGO.GetComponent<Animator>();
        Debug.Log(animator.input);

        if (animator == null)
        {
            EditorGUILayout.HelpBox("No Animator found on the selected GameObject", MessageType.Error);
            return;
        }

        controller = animator.runtimeAnimatorController as AnimatorController;
        if (controller == null)
        {
            return;
        }

        foreach (AnimatorControllerLayer treeLayer in controller.layers)
        {
            foreach (ChildAnimatorState cas in treeLayer.stateMachine.states)
            {
                if (GUILayout.Button(cas.state.input))
                {
                    animator.Play(cas.state.nameHash);
                }
            }
        }
    }
}*/
