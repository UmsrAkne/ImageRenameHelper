using ImageRenameHelper.Models;
using ImageRenameHelper.Utils;

namespace ImageRenameHelperTest.Utils
{
    [TestFixture]
    public class RenameFilesTests
    {
        private string testDirectory1;
        private string testDirectory2;
        private List<FileListItem> originalFiles;
        private List<FileListItem> targetFiles;

        [SetUp]
        public void Setup()
        {
            testDirectory1 = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            testDirectory2 = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(testDirectory1);
            Directory.CreateDirectory(testDirectory2);

            originalFiles = new List<FileListItem>();
            targetFiles = new List<FileListItem>();

            for (var i = 0; i < 3; i++)
            {
                var originalFilePath = Path.Combine(testDirectory1, $"OriginalFile{i}.txt");
                var targetFilePath = Path.Combine(testDirectory2, $"TargetFile{i}.txt");

                File.WriteAllText(originalFilePath, "Test Content");
                File.WriteAllText(targetFilePath, "Test Content");

                originalFiles.Add(new FileListItem(new FileInfo(originalFilePath)));
                targetFiles.Add(new FileListItem(new FileInfo(targetFilePath)));
            }
        }

        [TearDown]
        public void Cleanup()
        {
            Directory.Delete(testDirectory1, true);
        }

        [Test]
        public void RenameFiles_ShouldRenameTargetFilesToOriginalNames()
        {
            FileSystemUtil.RenameFiles(originalFiles, targetFiles);

            for (var i = 0; i < originalFiles.Count; i++)
            {
                System.Diagnostics.Debug.WriteLine($"count = {i}(FileRenameUtilTest : 51)");
                var expectedPath = Path.Combine(testDirectory2, originalFiles[i].Name);
                Assert.That(File.Exists(expectedPath), Is.True, $"Expected file {expectedPath} does not exist");
            }
        }
    }
}