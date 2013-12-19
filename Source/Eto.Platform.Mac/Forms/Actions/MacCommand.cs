using Eto.Forms;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;

namespace Eto.Platform.Mac.Forms.Actions
{
	public class MacCommand : Command
	{
		public Selector Selector { get; private set; }
		
		public MacCommand(string id, string text, string selector)
		{
			ID = id;
			MenuText = ToolBarText = text;
			Selector = new Selector(selector);
		}
	}
}

