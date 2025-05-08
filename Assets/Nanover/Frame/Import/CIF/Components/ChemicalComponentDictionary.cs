using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Nanover.Frame.Import.CIF.Components
{
    /// <summary>
    /// A global <see cref="ScriptableObject" /> which holds the mmCIF component
    /// dictionary.
    /// </summary>
    public class ChemicalComponentDictionary : ScriptableObject,
                                                 ISerializationCallbackReceiver
    {
        private static ChemicalComponentDictionary _instance = null;

        /// <summary>
        /// The chemical component dictionary.
        /// </summary>
        public static ChemicalComponentDictionary Instance
        {
            get
            {
                if (_instance == null)
                    LoadInstance();
                return _instance;
            }
        }

        /// <summary>
        /// Load the dictionary from a file, creating it if it is missing.
        /// </summary>
        private static void LoadInstance()
        {
            _instance = new ChemicalComponentDictionary();
            _instance.compressedData = Resources.Load<TextAsset>(InstanceFile).text;

            if (_instance == null)
            {
                if (_instance == null)
                    throw new InvalidOperationException(
                        $"Cannot find mmCIF chemical component dictionary at {InstanceFile}");
            }
        }

        

#if UNITY_EDITOR
        [MenuItem("Nanover/Load mmCIF Chemical Component Dictionary")]
        private static void LoadChemicalComponentDictionary()
        {
            var filename =
                EditorUtility.OpenFilePanel("Select mmCIF Chemical Component Dictionary", "",
                                            "cif");
            Instance.LoadChemicalComponentDictionaryFromFile(filename);
        }

        private void LoadChemicalComponentDictionaryFromFile(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                return;

            var file = File.OpenRead(filename);

            chemicalComponents.Clear();

            var stream = new StreamReader(file);
            foreach (var component in CifChemicalComponentBondsImport.ImportMultiple(stream))
                chemicalComponents[component.ResId] = component;

            Debug.Log($"<b>Nanover</b>: Loaded {chemicalComponents.Count} CIF components.");

            SerializeDictionary();

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }

#endif

        private const string InstanceFile = "components";

        public ChemicalComponent GetResidue(string residueName)
        {
            return chemicalComponents.TryGetValue(residueName, out var value) ? value : null;
        }

        public void OnBeforeSerialize()
        {
        }

        private string compressedData = "";

        private void SerializeDictionary()
        {
            using (var origStream = new MemoryStream())
            using (var stream = new DeflateStream(origStream, CompressionMode.Compress))
            {
                var writer = new BinaryWriter(stream);

                writer.Write((int) chemicalComponents.Count);
                foreach (var component in chemicalComponents.Values)
                {
                    writer.Write(component.ResId);
                    writer.Write((int) component.Bonds.Count());
                    foreach (var bond in component.bonds)
                    {
                        writer.Write(bond.a);
                        writer.Write(bond.b);
                        writer.Write((byte) bond.order);
                    }
                }

                compressedData = Convert.ToBase64String(origStream.ToArray());
            }
        }

        private void DeserializeDictionary()
        {
            if (string.IsNullOrEmpty(compressedData))
            {
                return;
            }

            var bytes = Convert.FromBase64String(compressedData);
            using (var origStream = new MemoryStream(bytes))
            using (var stream = new DeflateStream(origStream, CompressionMode.Decompress))
            {
                try
                {
                    var reader = new BinaryReader(stream);

                    var count = reader.ReadInt32();

                    for (var i = 0; i < count; i++)
                    {
                        var resId = reader.ReadString();
                        var component = new ChemicalComponent()
                        {
                            ResId = resId
                        };
                        var bondCount = reader.ReadInt32();
                        for (var j = 0; j < bondCount; j++)
                        {
                            var a = reader.ReadString();
                            var b = reader.ReadString();
                            var order = reader.ReadByte();
                            component.bonds.Add(new ChemicalComponent.Bond()
                            {
                                a = a,
                                b = b,
                                order = order
                            });
                        }

                        chemicalComponents[component.ResId] = component;
                    }
                }
                catch (EndOfStreamException)
                {
                }
            }
        }

        public void OnAfterDeserialize()
        {
            DeserializeDictionary();
        }

        private readonly Dictionary<string, ChemicalComponent> chemicalComponents =
            new Dictionary<string, ChemicalComponent>();
    }
}
