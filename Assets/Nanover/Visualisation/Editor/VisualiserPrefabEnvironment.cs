using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Nanover.Frame;
using Nanover.Frame.Import.CIF;
using Nanover.Frame.Import.CIF.Components;
using Nanover.Visualisation.Components;
using Nanover.Visualisation.Node.Adaptor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Nanover.Visualisation.Editor
{
    [InitializeOnLoad]
    internal static class VisualiserPrefabEnvironment
    {
        static VisualiserPrefabEnvironment()
        {
            PrefabStage.prefabStageOpened += OnPrefabStageOpened;
            SceneView.duringSceneGui += SceneViewOnDuringSceneGui;
            EditorUtility.ClearProgressBar();
        }
        
        private static string structureId = "";
        private static string currentMoleculeFilepath = "";
        private static ImmutableFrameSource currentMolecule = null;

        /// <summary>
        /// Show a progress message.
        /// </summary>
        /// <remarks>
        /// If called from another thread, <paramref name="threadSafe" /> should be false.
        /// This will queue showing the update until the next time the UI is refreshed, as
        /// <see cref="EditorUtility.DisplayProgressBar" /> can only be called from the
        /// main thread.
        /// </remarks>
        private static void ShowProgressMessage(string message, bool threadSafe)
        {
            if (threadSafe)
            {
                EditorUtility.DisplayProgressBar("Visualisation Preview", message, 0.5f);
                queuedProgressMessage = null;
            }
            else
            {
                queuedProgressMessage = message;
            }
        }

        /// <summary>
        /// Hide the progress bar
        /// </summary>
        private static void HideProgressMessage()
        {
            queuedProgressMessage = null;
            EditorUtility.ClearProgressBar();
        }

        /// <summary>
        /// Callback for drawing a GUI during a prefab view
        /// </summary>
        private static void SceneViewOnDuringSceneGui(SceneView obj)
        {
            if (PrefabStageUtility.GetCurrentPrefabStage() == null)
                return;

            if (!hasOnPrefabStageOpenedBeenCalled)
                OnPrefabStageOpened(PrefabStageUtility.GetCurrentPrefabStage());

            if (!string.IsNullOrEmpty(queuedProgressMessage))
            {
                ShowProgressMessage(queuedProgressMessage, true);
            }

            if (!isValidPrefabForVisualisation)
                return;

            DrawGUI();
        }

        private static void DrawGUI()
        {
            Handles.BeginGUI();

            GUILayout.BeginArea(
                new Rect(4, 4, 2 + 48 + 2 + 36 + 4 + 76 + 4 + 4 + 76 + 4 + 46, 32),
                GUIContent.none,
                EditorStyles.toolbar);
            GUI.Label(new Rect(2, 0, 48, 28),
                      "Preview",
                      EditorStyles.miniBoldLabel);
            structureId = GUI.TextField(new Rect(2 + 48 + 2, 2, 36, 28),
                                        structureId,
                                        EditorStyles.toolbarTextField);
            structureId = structureId.Trim().ToUpper();
            if (GUI.Button(new Rect(2 + 48 + 2 + 36 + 4, 0, 76 + 40, 32),
                           "Load Structure",
                           EditorStyles.toolbarButton))
            {
                LoadCifFileFromRcsb(structureId);
            }

            var x = 2 + 48 + 2 + 36 + 4 + 72 + 4 + 40;

            GUI.enabled = currentMolecule != null;

            if (GUI.Button(new Rect(x, 0, 76, 32),
                           "Show File",
                           EditorStyles.toolbarButton))
            {
                EditorUtility.RevealInFinder(currentMoleculeFilepath);
            }

            GUI.enabled = true;

            GUILayout.EndArea();
            Handles.EndGUI();
        }

        /// <summary>
        /// Is the current prefab a valid visualiser.
        /// </summary>
        private static bool isValidPrefabForVisualisation = false;

        /// <summary>
        /// Flag to indicate that <see cref="OnPrefabStageOpened" /> has been called.
        /// </summary>
        /// <remarks>
        /// When recompiling, <see cref="OnPrefabStageOpened" /> is not called and hence needs to be called on the first time the UI is refreshed.
        /// </remarks>
        private static bool hasOnPrefabStageOpenedBeenCalled = false;

        private static void OnPrefabStageOpened(PrefabStage prefabStage)
        {
            hasOnPrefabStageOpenedBeenCalled = true;

            if (prefabStage.prefabContentsRoot?.GetComponent<IFrameConsumer>() == null
                && prefabStage.prefabContentsRoot?.GetVisualisationNode<ParentedAdaptorNode>() == null)
            {
                isValidPrefabForVisualisation = false;
                return;
            }

            isValidPrefabForVisualisation = true;
            
            try
            {
                if (currentMolecule != null)
                {
                    UpdateRenderer(currentMolecule);
                }
                else
                {
                    currentMoleculeFilepath = EditorPrefs.GetString("visualiser.prefab.file");
                    if (currentMoleculeFilepath != null && File.Exists(currentMoleculeFilepath))
                    {
                        LoadFile(currentMoleculeFilepath);
                    }
                }
            }
            catch (Exception e)
            {
                // Need to catch all exceptions here, otherwise Editor can bug out.
                CancelPrefabView();
                ShowExceptionAndLog(e);
            }
        }

        /// <summary>
        /// Clearup the progress message.
        /// </summary>
        private static void CancelPrefabView()
        {
            HideProgressMessage();
        }

        private static void LoadCifFileFromRcsb(string id)
        {
            if (string.IsNullOrEmpty(id))
                return;

            currentMoleculeFilepath = $"{Application.temporaryCachePath}/{id}.cif";

            if (File.Exists(currentMoleculeFilepath))
            {
                try
                {
                    LoadFile(currentMoleculeFilepath);
                }
                catch (Exception e)
                {
                    CancelPrefabView();
                    ShowExceptionAndThrow(e);
                }
            }
            else
            {
                var client = new WebClient();
                client.DownloadFileCompleted += ClientOnDownloadFileCompleted;
                client.CancelAsync();
                client.DownloadFileAsync(new Uri($"https://files.rcsb.org/download/{id}.cif"),
                                         currentMoleculeFilepath);
                ShowProgressMessage("Downloading file", true);
            }
        }

        private static void ClientOnDownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                ShowProgressMessage("Importing file", true);
                try
                {
                    LoadFile(currentMoleculeFilepath);
                }
                catch (Exception exception)
                {
                    CancelPrefabView();
                    ShowExceptionAndThrow(exception);
                }
            }
            else
            {
                CancelPrefabView();
                ShowExceptionAndThrow(e.Error);
            }
        }

        private static void ShowExceptionAndThrow(Exception e)
        {
            EditorUtility.DisplayDialog("Exception", e.Message, "OK");
            throw e;
        }

        private static void ShowExceptionAndLog(Exception e)
        {
            EditorUtility.DisplayDialog("Exception", e.Message, "OK");
            Debug.LogException(e);
        }

        private class Progress : IProgress<string>
        {
            public void Report(string value)
            {
                ShowProgressMessage(value, false);
            }
        }

        private static string queuedProgressMessage = "";

        private static Task<Frame.Frame> ImportCifFile(
            string filename,
            ChemicalComponentDictionary dictionary)
        {
            using (var file = File.OpenRead(filename))
            using (var reader = new StreamReader(file))
            {
                if (Path.GetExtension(filename).Contains("cif"))
                    return Task.FromResult(CifImport.Import(reader, dictionary, new Progress()));
            }

            return Task.FromResult<Frame.Frame>(null);
        }

        private static async void LoadFile(string filename)
        {
            ShowProgressMessage("Loading File", true);

            var dictionary = ChemicalComponentDictionary.Instance;

            var frame = await Task.Run(() => ImportCifFile(filename, dictionary));


            frame.RecenterAroundOrigin();
            currentMolecule = new ImmutableFrameSource(frame);

            UpdateRenderer(currentMolecule);

            EditorPrefs.SetString("visualiser.prefab.file", currentMoleculeFilepath);

            structureId = Path.GetFileNameWithoutExtension(currentMoleculeFilepath);
            HideProgressMessage();
        }

        /// <summary>
        /// Update the prefab with the provided molecule
        /// </summary>
        private static void UpdateRenderer(ITrajectorySnapshot molecule)
        {
            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            var root = prefabStage.prefabContentsRoot;
            var renderer = root.GetComponent<IFrameConsumer>();
            if (renderer != null)
            {
                renderer.FrameSource = molecule;
            }
            else
            {
                var adaptor = root.GetVisualisationNode<ParentedAdaptorNode>();
                if (adaptor != null)
                {
                    var frameAdaptor = new FrameAdaptorNode();
                    frameAdaptor.FrameSource = molecule;
                    adaptor.ParentAdaptor.Value = frameAdaptor;
                    adaptor.Refresh();
                }
            }
           
            root.gameObject.SendMessage("Update");
        }

        private class ImmutableFrameSource : ITrajectorySnapshot
        {
            public ImmutableFrameSource(Frame.Frame frame)
            {
                CurrentFrame = frame;
            }

            public Frame.Frame CurrentFrame { get; }
            
            // Ignore warning that FrameChanged is never used--this is expected.
            #pragma warning disable 0067
            public event FrameChanged FrameChanged;
            #pragma warning restore 0067
        }
    }
}