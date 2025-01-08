namespace Cola.Utils.Helper;

public class FolderCopier
{
    public static void CopyFolder(string sourceFolder, string destinationFolder)
    {
        // 如果目标文件夹不存在，则创建
        if (!Directory.Exists(destinationFolder))
        {
            Directory.CreateDirectory(destinationFolder);
        }
 
        // 获取源文件夹中的所有文件和子文件夹
        foreach (string filePath in Directory.GetFiles(sourceFolder))
        {
            // 获取文件名
            string fileName = Path.GetFileName(filePath);
            // 将文件复制到目标文件夹
            string destFilePath = Path.Combine(destinationFolder, fileName);
            File.Copy(filePath, destFilePath, true); // true 表示如果目标文件存在，则覆盖它
        }
 
        // 递归复制子文件夹
        foreach (string folder in Directory.GetDirectories(sourceFolder))
        {
            string newFolderName = folder.Replace(sourceFolder, destinationFolder);
            CopyFolder(folder, newFolderName);
        }
    }
}