using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioBtn : MonoBehaviour, IInteractableHandler
{
    public Interactable Interactable;

    public TextMesh textMesh;

    public string GroupName;

    public static Dictionary<string, Interactable> ActiveInterable { get; private set; } = new Dictionary<string, Interactable>();

    // Start is called before the first frame update
    void Start()
    {
        if (!ActiveInterable.ContainsKey(GroupName))
        {
            ActiveInterable.Add(GroupName, null);
        }

        //Pressable.TouchBegin.AddListener(HandleRadioSelected);
        Interactable.AddHandler(this);
    }   

    private void OnDestroy()
    {
        //Pressable.TouchBegin.RemoveListener(HandleRadioSelected);
        Interactable.RemoveHandler(this);
    }


    public void OnStateChange(InteractableStates state, Interactable source) { }

    public void OnVoiceCommand(InteractableStates state, Interactable source, string command, int index = 0, int length = 1) { }

    public void OnClick(InteractableStates state, Interactable source, IMixedRealityPointer pointer = null)
    {
        if (source.IsToggled)
        {
            var _interactable = ActiveInterable[GroupName];
            // Don't cause null ref + don't set itself to false when turning on
            if (_interactable != null && _interactable != this.Interactable)
            {
                _interactable.IsToggled = false;
                _interactable.GetComponentInChildren<TextMesh>().text = "Off";
            }

            // Assign new active interactable
            ActiveInterable[GroupName] = this.Interactable;
            textMesh.text = "On";
            textMesh.color = Color.green;
        }
        else
        {
            textMesh.text = "Off";
            textMesh.color = Color.white;
        }
    }
}
