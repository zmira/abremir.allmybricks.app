namespace abremir.AllMyBricks.Device.Interfaces
{
    public interface IFile
    {
        void WriteAllBytes(string path, byte[] bytes);
    }
}