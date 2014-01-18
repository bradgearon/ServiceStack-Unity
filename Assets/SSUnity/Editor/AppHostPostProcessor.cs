using System.IO;
using System.IO.Compression;
using System.Linq;
using ServiceStack.Common;
using ServiceStack.Common.Web;
using ServiceStack.ServiceInterface.Testing;
using ServiceStack.Text;
using ServiceStack.VirtualPath;
using ServiceStack.WebHost.Endpoints;
using ServiceStack.WebHost.Endpoints.Support.Markdown;
using UnityEngine;
using UnityEditor;
using ServiceStack.WebHost.Endpoints.Formats;
using System.Collections.Generic;
using StreamExtensions = ServiceStack.Common.StreamExtensions;

/// <summary>
/// Copies AppHost compiled Evaluator to build output
/// </summary>
public class AppHostPostProcessor : AssetPostprocessor
{

    static void OnPostprocessAllAssets(string[] imported, string[] deleted, string[] moved, string[] movedFromAssetPaths)
    {
        if (imported.Any(s => s.Contains("AppHost")))
        {
            if (EditorApplication.isCompiling)
            {
                var startHosts = MonoBehaviour.FindObjectsOfType(typeof(StartHostBehavior));
                foreach (var startHost in startHosts)
                {
                    var monoScript = MonoScript.FromMonoBehaviour(startHost as MonoBehaviour);
                    if (typeof(StartHostBehavior).IsAssignableFrom(monoScript.GetClass()))
                    {
                        Debug.Log(monoScript.ToJsv());
                        var host = startHost as StartHost;
                        var hostPath = Path.Combine(
                            Directory.GetCurrentDirectory(), host.webrootPath);
                        var mf = new MarkdownFormat
                        {
                            VirtualPathProvider = new FileSystemVirtualPathProvider(
                                new TestAppHost   (), hostPath)
                        };
                        var mp = mf.FindMarkdownPages("/");
                        var output = new Dictionary<string, string>();

                        foreach (var markdownPage in mp)
                        {
                            markdownPage.Compile();
                            //var view = new Dictionary<string, object>()  {{ "examples", examples }};
                            output.Add(markdownPage.FilePath, markdownPage.RenderToString(new Dictionary<string, object>() { }, true));
                        }

                        Debug.Log(hostPath);
                        foreach (var outputPage in output)
                        {
                            var outputPath = hostPath + outputPage.Key.Replace('/', '\\') + ".html";
                            Debug.Log(outputPath);
                            using (var outputStream = File.Create(outputPath))
                            {

                                outputStream.Write(outputPage.Value);
                            }

                        }

                    }
                }
            }
        }

    }



}