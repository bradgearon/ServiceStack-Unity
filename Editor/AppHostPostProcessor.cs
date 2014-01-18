using ServiceStack.Text;
using ServiceStack.WebHost.Endpoints;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad, CustomEditor(typeof(StartHost), true))] 
public class AppHostPostProcessor : Editor
{
    void OnEnable()
    {
        Debug.Log("Postprocess running");

        if (EditorApplication.isCompiling)
        {
            var apphosts = MonoImporter.GetAllRuntimeMonoScripts();
            foreach (var monoScript in apphosts) 
            {
                if (typeof (AppHostHttpListenerBase).IsAssignableFrom(monoScript.GetClass()))
                {
                    Debug.Log("AppHost found");
                }
            }
            Debug.Log(apphosts.ToJson());
        }

    }



}