using Cysharp.Threading.Tasks;

namespace SaveSystem {
    public interface IDataHandler {
        UniTask CreateData();
        UniTask SaveData();
        UniTask LoadData();
        UniTask DeleteData();
    }
}