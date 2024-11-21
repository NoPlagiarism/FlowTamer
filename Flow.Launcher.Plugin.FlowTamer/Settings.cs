namespace Flow.Launcher.Plugin.FlowTamer
{
    public enum IconTypes
    {
        BTLike,
        SegoeIcon,
        BrowserIcon
    }
    public class Settings
    {
        public string BTPath { get; set; }
        public IconTypes Icon { get; set; }
    }
}
