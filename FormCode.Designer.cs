//------------------------------------------------------------------------------
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
//------------------------------------------------------------------------------

namespace apfm
{
    /// 
    [Microsoft.VisualStudio.Tools.Applications.Contract.EntryPointAttribute(0)]
    public sealed partial class FormCode : Microsoft.Office.InfoPath.XmlFormHostItem
    {

        internal Microsoft.Office.InfoPath.Application Application;

        internal Microsoft.Office.InfoPath.EventManager EventManager;

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        public FormCode(System.AddIn.Contract.Collections.IRemoteArgumentArrayContract initArgs) :
            base(initArgs)
        {
        }

        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        protected override string PrimaryCookie
        {
            get
            {
                return "XmlForm";
            }
        }

        public event System.EventHandler Startup;

        public event System.EventHandler Shutdown;

        /// 
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        protected override void OnStartup()
        {
            base.OnStartup();
            Application = ((Microsoft.Office.InfoPath.Application)(this.GetHostObject("Microsoft.Office.InfoPath.Application", "Application")));
            EventManager = ((Microsoft.Office.InfoPath.EventManager)(this.GetHostObject("Microsoft.Office.InfoPath.EventManager", "EventManager")));
        }

        /// 
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        protected override void FinishInitialization()
        {
            base.FinishInitialization();
            this.InternalStartup();
            if ((this.Startup != null))
            {
                this.Startup(this, System.EventArgs.Empty);
            }
        }

        /// 
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        protected override void OnShutdown()
        {
            if ((this.Shutdown != null))
            {
                this.Shutdown(this, System.EventArgs.Empty);
            }
            base.OnShutdown();
        }
    }
}
