using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.DatabaseServices;
using RaiseLowerCogo;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;
using Entity = Autodesk.AutoCAD.DatabaseServices.Entity;

[assembly: CommandClass(typeof(RaiseLowerCogoCommand))]
namespace RaiseLowerCogo;

public class RaiseLowerCogoCommand
{
	private readonly Editor _editor;
	private readonly Database _database;

	public RaiseLowerCogoCommand()
	{
		_editor = Application.DocumentManager.MdiActiveDocument.Editor;
		_database = Application.DocumentManager.MdiActiveDocument.Database;
	}

	[CommandMethod("WMS", "_RaiseLowerCogo", CommandFlags.Modal | CommandFlags.UsePickSet)]
	public void DoRaiseLowerCogo()
	{
		if (!TryGetImpliedSelectionOfType<CogoPoint>(out var pointIds) &&
		    !TryGetSelectionOfType<CogoPoint>("\nSelect CogoPoints to raise/lower: ", "", out pointIds))
			return;

		// var entityType = RXObject.GetClass(typeof(CogoPoint));
		// var typedValues = new TypedValue[] { new((int)DxfCode.Start, entityType.DxfName) };
		// var selectionFilter = new SelectionFilter(typedValues);
		// var pso = new PromptSelectionOptions { MessageForAdding = "\nSelect CogoPoints to raise/lower: " };
		// var selection = _editor.GetSelection(pso, selectionFilter);

		// if (selection.Status != PromptStatus.OK)
		// 	return;
		//
		// if (selection.Value.Count < 1)
		// 	return;

		var raiseLowerAmount = _editor.GetDouble("\nAmount to raise/lower by: ");

		if (raiseLowerAmount.Status != PromptStatus.OK)
			return;

		using var tr = new TransactAndForget(true);

		//foreach (SelectedObject selectedObject in selection.Value)
		foreach (ObjectId objectId in pointIds)
		{
			var cogoPoint = tr.GetObject<CogoPoint>(objectId, OpenMode.ForWrite);
			cogoPoint.Elevation += raiseLowerAmount.Value;
		}
	}

	/// <summary>
	/// Gets a implied selection of type T.
	/// </summary>
	/// <param name="objectIds">Collection of <see cref="ObjectId"/>s obtained from the selection set.</param>
	/// <typeparam name="T">Type of <see cref="Entity"/></typeparam>
	/// <returns><c>true</c> if the selection was successful, otherwise <c>false</c>.</returns>
	/// <remarks>Will filter out any entities not of type T.</remarks>
	public bool TryGetImpliedSelectionOfType<T>(out ObjectIdCollection objectIds) where T : Entity
	{
		var psr = _editor.SelectImplied();
		objectIds = new ObjectIdCollection();

		if (psr.Status != PromptStatus.OK)
			return false;

		var entityType = RXObject.GetClass(typeof(T));
		foreach (var objectId in psr.Value.GetObjectIds())
		{
			// check that the objectId type matches the entityType
			if (objectId.ObjectClass.Equals(entityType))
			{
				objectIds.Add(objectId);
			}
		}

		return true;
	}

	/// <summary>
	/// Gets the type of the entities of.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="addMessage">The add message.</param>
	/// <param name="removeMessage">The remove message.</param>
	/// <param name="objectIds">The object ids.</param>
	/// <returns><c>true</c> if successfully got a selection, <c>false</c> otherwise.</returns>
	public bool TryGetSelectionOfType<T>(string addMessage, string removeMessage, out ObjectIdCollection objectIds) where T : Entity
	{
		var entityType = RXObject.GetClass(typeof(T));

		objectIds = new ObjectIdCollection();

		TypedValue[] typedValues = { new TypedValue((int)DxfCode.Start, entityType.DxfName) };
		var ss = new SelectionFilter(typedValues);
		var pso = new PromptSelectionOptions
		{
			MessageForAdding = addMessage,
			MessageForRemoval = removeMessage
		};

		var result = _editor.GetSelection(pso, ss);

		if (result.Status != PromptStatus.OK)
			return false;

		objectIds = new ObjectIdCollection(result.Value.GetObjectIds());

		return true;
	}
}