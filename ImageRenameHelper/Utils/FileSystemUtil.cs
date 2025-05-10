using System;
using System.Collections.Generic;
using System.IO;
using ImageRenameHelper.Models;

namespace ImageRenameHelper.Utils
{
    public class FileSystemUtil
    {
        /// <summary>
        /// 受け取った２つのリストのうち、 b の中のファイル名を a と同じファイル名に変更します。<br/>
        /// </summary>
        /// <param name="a">変更名のソース。</param>
        /// <param name="b">ファイル名を変更するファイルのリスト。</param>
        /// <exception cref="ArgumentException">パラメーターに入力された２つのリストの要素数が異なる場合にスローされます。</exception>
        public static void RenameFiles(List<FileListItem> a, List<FileListItem> b)
        {
            if (a.Count != b.Count)
            {
                throw new ArgumentException("Lists a and b must have the same number of elements.");
            }

            var tempNames = new Dictionary<FileListItem, string>();

            // ファイル変更途中の衝突を防ぐため、一時ファイル名に変更
            foreach (var targetFile in b)
            {
                var directoryName = targetFile.DirectoryName;
                directoryName ??= string.Empty;

                var tempFileName = Path.Combine(directoryName, Guid.NewGuid() + targetFile.Extension);

                try
                {
                    File.Move(targetFile.FullName, tempFileName);
                    tempNames[targetFile] = tempFileName;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to rename {targetFile.Name} to temporary name: {ex.Message}");
                    return; // ここで失敗したら処理を中断
                }
            }

            // 最終ファイル名に変更
            for (var i = 0; i < a.Count; i++)
            {
                var directoryName = b[i].DirectoryName;
                directoryName ??= string.Empty;

                var newFileName = Path.Combine(directoryName, a[i].Name);
                var tempFilePath = tempNames[b[i]];

                try
                {
                    File.Move(tempFilePath, newFileName);
                    Console.WriteLine($"Renamed: {tempFilePath} -> {newFileName}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to rename {tempFilePath} to final name {newFileName}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// 指定ファイルをコピーします。コピー先のファイル名はメソッド内で自動的に設定されます。
        /// </summary>
        /// <param name="targetFilePath">コピーする対象ファイルのフルパス</param>
        /// <param name="destDirectoryPath">コピー先のディレクトリのパス</param>
        public static void CopyFile(string targetFilePath, string destDirectoryPath)
        {
            if (!File.Exists(targetFilePath) || !Directory.Exists(destDirectoryPath))
            {
                return;
            }

            var fileNameWe = Path.GetFileNameWithoutExtension(targetFilePath);
            var extension = Path.GetExtension(targetFilePath);

            var counter = 1;
            var destPath = Path.Combine(destDirectoryPath, fileNameWe + $"_copy_{counter:000}" + extension);
            while (File.Exists(destPath))
            {
                destPath = Path.Combine(destDirectoryPath, fileNameWe + $"_copy_{counter:000}" + extension);
                counter++;
            }

            File.Copy(targetFilePath, destPath, true);
        }
    }
}