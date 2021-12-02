namespace GleyTrafficSystem
{
    public struct WindowProperties
    {
        public readonly WindowType type;
        public readonly string name;
        public readonly string tutorialLink;
        public readonly bool showBack;
        public readonly bool showTitle;
        public readonly bool showTop;
        public readonly bool showScroll;
        public readonly bool showBottom;
        public readonly bool blockClicks;


        public WindowProperties(WindowType type, string name, bool showBack, bool showTitle, bool showTop, bool showScroll, bool showBottom, bool blockClicks, string tutorialLink)
        {
            this.type = type;
            this.name = name;
            this.showBack = showBack;
            this.showTitle = showTitle;
            this.showTop = showTop;
            this.showScroll = showScroll;
            this.showBottom = showBottom;
            this.blockClicks = blockClicks;
            this.tutorialLink = tutorialLink;
        }
    }
}
