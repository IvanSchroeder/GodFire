namespace SaveSystem {
    public interface IDataService {
        bool SaveData<T>(T Data, string directory, string relativePath);
        T LoadData<T>(string directory, string relativePath);
        bool DeleteData(string directory, string relativePath);
        bool CheckData(string path);
    }
}
