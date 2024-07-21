using System;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows;
using Autodesk.Civil.DatabaseServices;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace RaiseLowerCogo;

public static class RaiseLowerMenu
{
	private static ContextMenuExtension cme;

	public static void Attach()
	{
		cme = new ContextMenuExtension();
		MenuItem mi = new MenuItem("Raise/Lower CogoPoint");
		mi.Click += OnRaiseLower;
		cme.MenuItems.Add(mi);
		RXClass rxc = RXObject.GetClass(typeof(CogoPoint));
		Autodesk.AutoCAD.ApplicationServices.Application.AddObjectContextMenuExtension(rxc, cme);
	}

	public static void Detach()
	{
		RXClass rxc = RXObject.GetClass(typeof(CogoPoint));
		Autodesk.AutoCAD.ApplicationServices.Application.RemoveObjectContextMenuExtension(rxc, cme);
	}

	private static void OnRaiseLower(object o, EventArgs e)
	{
		Document doc = Application.DocumentManager.MdiActiveDocument;
		doc.SendStringToExecute("_.RAISELOWERCOGO ", true, false, false);
	}
}
