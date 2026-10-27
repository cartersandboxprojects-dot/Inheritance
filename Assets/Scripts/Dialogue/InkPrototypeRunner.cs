using Inheritance.State;
using UnityEngine;

namespace Inheritance.Dialogue
{
    // Temporary graybox harness; replace with the session owner and dialogue UI later.
    public sealed class InkPrototypeRunner : MonoBehaviour
    {
        [SerializeField] private Ink.UnityIntegration.InkFile compiledStory;
        public GameState State { get; private set; }
        public ElenaInkConversation Conversation { get; private set; }
        public string Transcript { get; private set; }

        private void Start()
        {
            State = FuneralNightContent.CreateState();
            Conversation = new ElenaInkConversation(compiledStory.storyJson, State);
            Transcript = Conversation.ContinueToChoice();
        }

        public void SelectChoice(int index)
        {
            Transcript += "\n" + Conversation.Choose(index);
        }

        private void OnGUI()
        {
            if (Conversation == null) return;
            GUILayout.BeginArea(new Rect(12, 12, 440, 240), GUI.skin.box);
            GUILayout.Label("Ink prototype - Elena");
            GUILayout.Label(Transcript);
            for (int i = 0; i < Conversation.Choices.Count; i++)
                if (GUILayout.Button(Conversation.Choices[i].text)) SelectChoice(i);
            GUILayout.Label("BRECKEN_COMFORTED_ELENA: " +
                State.GetFact(FuneralNightContent.BreckenComfortedElena));
            GUILayout.EndArea();
        }
    }
}

