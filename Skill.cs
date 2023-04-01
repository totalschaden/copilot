namespace CoPilot
{
    internal class Skill
    {
        public Skill(string buffname = "", ushort id = 0, float cooldown = 0, int isBlessing = 0, int lastUsed = 0)
        {
            BuffName = buffname;
            Id = id;
            Cooldown = cooldown;
            IsBlessing = isBlessing;
        }

        public string BuffName { get; set; }

        public ushort Id { get; set; }

        public float Cooldown { get; set; }
 
        public int IsBlessing { get; set; }
    }
}