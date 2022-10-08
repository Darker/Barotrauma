namespace Barotrauma.Items.Components
{
    public struct ThrottledSignalDispatch
    {
        public readonly string name;
        public readonly float cooldownDuration;
        public readonly float randomDeviation;
        public readonly bool resetOnChange;

        private float cooldownTimer;
        private string lastValue;
        private Item lastItem;

        internal ThrottledSignalDispatch(string name, float cooldownDuration, float randomDeviation = 0, bool resetOnChange = true) : this()
        {
            this.name = name;
            this.cooldownDuration = cooldownDuration;
            this.resetOnChange = resetOnChange;
        }

        internal bool Send(Item item, string value, float deltaTime)
        {
            if((value != lastValue && resetOnChange) || item != lastItem || cooldownTimer <= 0)
            {
                lastItem = item;
                lastValue = value;
                cooldownTimer += cooldownDuration * (1.0f + Rand.Range(-randomDeviation, randomDeviation));

                // the addition instead of assignment above is deliberate to prevent the expected signal rate from floating
                // in one direction because it is not divisible by dt. However, if the game gets stuck or something, this behavior is capped
                // at a single re-send the next frame by preventing it to go lower than -coolDownDuration
                if(cooldownTimer < 0)
                {
                    cooldownTimer = 0;
                }

                item.SendSignal(name, value);
                return true;
            }
            else
            {
                cooldownTimer -= deltaTime;
                return false;
            }
        }
    }
}
