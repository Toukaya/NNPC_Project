namespace Character.NPC
{
    public record NPCAction(string Text, string Action, string Emotion)
    {
        public string Text {get; set;} = Text;
        public string Action {get; set;} = Action;
        public string Emotion {get; set;} = Emotion;
    }
}