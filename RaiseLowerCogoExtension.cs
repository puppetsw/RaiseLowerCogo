using Autodesk.AutoCAD.Runtime;

namespace RaiseLowerCogo
{
    public class RaiseLowerCogoExtension : IExtensionApplication
    {
        public void Initialize()
        {
            RaiseLowerMenu.Attach();
        }

        public void Terminate()
        {
	        RaiseLowerMenu.Detach();
        }
    }
}
