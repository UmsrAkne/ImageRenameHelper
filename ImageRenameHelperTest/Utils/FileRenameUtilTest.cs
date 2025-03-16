using ImageRenameHelper.Models;
using ImageRenameHelper.Utils;

namespace ImageRenameHelperTest.Utils
{
    [TestFixture]
    public class FileRenameUtilTest
    {
        [TestFixture]
        public class RenameFilesTests
        {
            private string testDirectory;
            private List<FileListItem> originalFiles;
            private List<FileListItem> targetFiles;

            [SetUp]
            public void Setup()
            {
                testDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(testDirectory);

                originalFiles = new List<FileListItem>();
                targetFiles = new List<FileListItem>();

                for (var i = 0; i < 3; i++)
                {
                    var originalFilePath = Path.Combine(testDirectory, $"OriginalFile{i}.txt");
                    var targetFilePath = Path.Combine(testDirectory, $"TargetFile{i}.txt");

                    File.WriteAllText(originalFilePath, "Test Content");
                    File.WriteAllText(targetFilePath, "Test Content");

                    originalFiles.Add(new FileListItem(new FileInfo(originalFilePath)));
                    targetFiles.Add(new FileListItem(new FileInfo(targetFilePath)));
                }
            }

            [TearDown]
            public void Cleanup()
            {
                Directory.Delete(testDirectory, true);
            }

            [Test]
            public void RenameFiles_ShouldRenameTargetFilesToOriginalNames()
            {
                FileRenameUtil.RenameFiles(originalFiles, targetFiles);

                for (var i = 0; i < originalFiles.Count; i++)
                {
                    System.Diagnostics.Debug.WriteLine($"count = {i}(FileRenameUtilTest : 51)");
                    var expectedPath = Path.Combine(testDirectory, originalFiles[i].Name);
                    Assert.IsTrue(File.Exists(expectedPath), $"Expected file {expectedPath} does not exist");
                }
            }
        }
    }
}