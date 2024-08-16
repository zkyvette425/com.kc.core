using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using static System.String;

namespace KC
{
    [CustomEditor(typeof(ReferenceView))]
    public class ReferencePoolViewEditor : Editor
    {
        private readonly Dictionary<string, List<ReferencePoolInfo>> _referencePoolInfos = new(StringComparer.Ordinal);
        private readonly HashSet<string> _openedItems = new HashSet<string>();
        private bool _isShowFullClassName;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();

            if (!EditorApplication.isPlaying )
            {
                return;
            }

            ReferenceView referenceView = (ReferenceView)target;

            EditorGUILayout.LabelField("Reference Pool Count", ReferencePool.Count.ToString());

            _isShowFullClassName = EditorGUILayout.Toggle("Show Full Class Name", _isShowFullClassName);
            _referencePoolInfos.Clear();

            var poolInfos = ReferencePool.GetAllReferencePoolInfos();
            foreach (ReferencePoolInfo referencePoolInfo in poolInfos)
            {
                string assemblyName = referencePoolInfo.Type.Assembly.GetName().Name;
                if (!_referencePoolInfos.TryGetValue(assemblyName, out var results))
                {
                    results = new List<ReferencePoolInfo>();
                    _referencePoolInfos.Add(assemblyName, results);
                }

                results.Add(referencePoolInfo);
            }

            foreach (KeyValuePair<string, List<ReferencePoolInfo>> assemblyReferencePoolInfo in _referencePoolInfos)
            {
                bool lastState = _openedItems.Contains(assemblyReferencePoolInfo.Key);
                bool currentState = EditorGUILayout.Foldout(lastState, assemblyReferencePoolInfo.Key);
                if (currentState != lastState)
                {
                    if (currentState)
                    {
                        _openedItems.Add(assemblyReferencePoolInfo.Key);
                    }
                    else
                    {
                        _openedItems.Remove(assemblyReferencePoolInfo.Key);
                    }
                }

                if (currentState)
                {
                    EditorGUILayout.BeginVertical("box");
                    {
                        EditorGUILayout.LabelField(_isShowFullClassName ? "Full Class Name" : "Class Name",
                            "Unused\tUsing\tAcquire\tRelease\tAdd\tRemove");
                        assemblyReferencePoolInfo.Value.Sort(Comparison);
                        foreach (ReferencePoolInfo referencePoolInfo in assemblyReferencePoolInfo.Value)
                        {
                            DrawReferencePoolInfo(referencePoolInfo);
                        }

                        if (GUILayout.Button("Export CSV Data"))
                        {
                            string exportFileName = EditorUtility.SaveFilePanel("Export CSV Data", Empty,
                                Format("Reference Pool Data - {0}.csv", assemblyReferencePoolInfo.Key),
                                Empty);
                            if (!IsNullOrEmpty(exportFileName))
                            {
                                try
                                {
                                    int index = 0;
                                    string[] data = new string[assemblyReferencePoolInfo.Value.Count + 1];
                                    data[index++] =
                                        "Class Name,Full Class Name,Unused,Using,Acquire,Release,Add,Remove";
                                    foreach (ReferencePoolInfo referencePoolInfo in assemblyReferencePoolInfo.Value)
                                    {
                                        data[index++] = Format("{0},{1},{2},{3},{4},{5},{6},{7}",
                                            referencePoolInfo.Type.Name, referencePoolInfo.Type.FullName,
                                            referencePoolInfo.UnusedReferenceCount,
                                            referencePoolInfo.UsingReferenceCount,
                                            referencePoolInfo.AcquireReferenceCount,
                                            referencePoolInfo.ReleaseReferenceCount,
                                            referencePoolInfo.AddReferenceCount,
                                            referencePoolInfo.RemoveReferenceCount);
                                    }

                                    File.WriteAllLines(exportFileName, data, Encoding.UTF8);
                                    Debug.Log(Format("Export reference pool CSV data to '{0}' success.",
                                        exportFileName));
                                }
                                catch (Exception exception)
                                {
                                    Debug.LogError(Format(
                                        "Export reference pool CSV data to '{0}' failure, exception is '{1}'.",
                                        exportFileName, exception));
                                }
                            }
                        }
                    }
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.Separator();
                }
            }

            serializedObject.ApplyModifiedProperties();

            Repaint();
        }

        private void DrawReferencePoolInfo(ReferencePoolInfo referencePoolInfo)
        {
            EditorGUILayout.LabelField(
                _isShowFullClassName ? referencePoolInfo.Type.FullName : referencePoolInfo.Type.Name,
                Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}", referencePoolInfo.UnusedReferenceCount,
                    referencePoolInfo.UsingReferenceCount, referencePoolInfo.AcquireReferenceCount,
                    referencePoolInfo.ReleaseReferenceCount, referencePoolInfo.AddReferenceCount,
                    referencePoolInfo.RemoveReferenceCount));
        }

        private int Comparison(ReferencePoolInfo a, ReferencePoolInfo b)
        {
            return _isShowFullClassName
                ? Compare(a.Type.FullName, b.Type.FullName, StringComparison.Ordinal)
                : Compare(a.Type.Name, b.Type.Name, StringComparison.Ordinal);
        }
    }
}