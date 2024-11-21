using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Windows.Controls;

namespace Flow.Launcher.Plugin.FlowTamer
{
    public class FlowTamer : IPlugin, ISettingProvider
    {
        private PluginInitContext _context;
        private BTApi BT;
        private Settings settings;

        public Control CreateSettingPanel()
        {
            return new SettingsView(settings);
        }

        public void Init(PluginInitContext context)
        {
            _context = context;
            settings = _context.API.LoadSettingJsonStorage<Settings>();

            InitBT();
        }

        private void InitBT(bool forceChange = false)
        {
            if (forceChange || string.IsNullOrEmpty(settings.BTPath))
            {
                settings.BTPath = BTApi.FindBT();
                _context.API.SavePluginSettings();
            }
            BT = new BTApi(settings.BTPath);
        }


        public void Open(Query query, bool withPicker)
        {
            try { BT.Open(query.Search, withPicker); }
            catch (Win32Exception e)
            {
                Retry(query, withPicker);
            }
        }

        private void Retry(Query query, bool withPicker)
        {
            try { InitBT(true); Open(query, withPicker); }
            catch (Win32Exception e)
            {
                _context.API.LogInfo("FlowTamer", $"Win32Exception ({e.ErrorCode})");
                // TODO: think how to handle only FileNotFound
                _context.API.ShowMsg("bt.exe not found", "Change path in the settings");
            }
        }

        public List<Result> Query(Query query)
        {
            return new List<Result>() {
                new Result {
                    Title = $"Open with picker",
                    SubTitle = $"Open {query.Search}",
                    Action = (_) => { Open(query, true); return true; },
                    IcoPath = (settings.Icon == IconTypes.BTLike) ? "Images\\logoSmall.png" : "Images\\Browser.png",
                    Glyph = (settings.Icon == IconTypes.SegoeIcon) ? new GlyphInfo("/Resources/#Segoe Fluent Icons", "\uf6fa") : null,
                },
                new Result {
                    Title = $"Open",
                    SubTitle = $"Open {query.Search}",
                    Action = (_) => { Open(query, false); return true; },
                    IcoPath = (settings.Icon == IconTypes.BTLike) ? "Images\\logoSmall.png" : "Images\\Browser.png",
                    Glyph = (settings.Icon == IconTypes.SegoeIcon) ? new GlyphInfo("/Resources/#Segoe Fluent Icons", "\uf6fa") : null,
                },
            };
        }
    }
}