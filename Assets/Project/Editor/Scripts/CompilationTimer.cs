using UnityEditor;
using UnityEditor.Compilation;
using System.Diagnostics;

[InitializeOnLoad]
public class CompilationTimer {
    private static Stopwatch stopwatch;

    static CompilationTimer() {
        CompilationPipeline.compilationStarted += OnCompilationStarted;
        CompilationPipeline.compilationFinished += OnCompilationFinished;
    }

    private static void OnCompilationStarted(object obj) {
        stopwatch = Stopwatch.StartNew();
        UnityEngine.Debug.Log("Compilation started...");
    }

    private static void OnCompilationFinished(object obj) {
        stopwatch.Stop();
        UnityEngine.Debug.Log($"Compilation finished. Time taken: {stopwatch.ElapsedMilliseconds} ms");
    }
}
