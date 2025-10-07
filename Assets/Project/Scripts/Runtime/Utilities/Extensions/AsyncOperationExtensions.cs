using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

public static class AsyncOperationExtensions {
    /// <summary>
    /// Extension method that converts an AsyncOperation into a Task.
    /// </summary>
    /// <param name="asyncOperation">The AsyncOperation to convert.</param>
    /// <returns>A Task that represents the completion of the AsyncOperation.</returns>
    public static Task AsTask(this AsyncOperation asyncOperation) {
        var tcs = new TaskCompletionSource<bool>();
        asyncOperation.completed += _ => tcs.SetResult(true);
        return tcs.Task;
    }

    public static async Task<T> IsComplete<T>(this T t) where T : Tween {
        var completionSource = new TaskCompletionSource<T>();
        t.OnComplete(() => completionSource.SetResult(t));
        return await completionSource.Task;
    }
}