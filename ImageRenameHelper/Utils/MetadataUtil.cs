using System;
using System.IO;
using System.Text.Json;
using ImageRenameHelper.Models;
using MetadataExtractor;

namespace ImageRenameHelper.Utils
{
    public class MetadataUtil
    {
        /// <summary>
        /// reads metadata from the specified png file and converts it into a `prompt`.
        /// </summary>
        /// <param name="filepath">png file path.</param>
        /// <returns>A Prompt object extracted and converted from the PNG file metadata.</returns>
        /// /// <remarks>
        /// This method assumes that the metadata format of the input PNG file is the same as that used in the debug environment.
        /// It may not work correctly if the metadata format differs.
        /// </remarks>
        public static Prompt ExtractMetadata(string filepath)
        {
            var directories = ImageMetadataReader.ReadMetadata(filepath);
            var result = new Prompt();

            foreach (var directory in directories)
            {
                if (directory.Name != "PNG-tEXt")
                {
                    continue;
                }

                foreach (var tag in directory.Tags)
                {
                    if (tag.Name.StartsWith("Textual Data"))
                    {
                        string[] delimiter = { "parameters: ", "Negative prompt: ", "Steps: ", };
                        var description = tag.Description ?? string.Empty;
                        var parts = description.Split(delimiter, StringSplitOptions.None);
                        if (parts.Length < 3)
                        {
                            continue;
                        }

                        result.Positive = parts[1];
                        result.Negative = parts[2];
                    }
                }
            }

            return result;
        }

        public static void SavePromptToJsonFile(Prompt prompt, string filePath)
        {
            var options = new JsonSerializerOptions { WriteIndented = true, };
            var json = JsonSerializer.Serialize(prompt, options);
            File.WriteAllText(filePath, json);
        }

        public static Prompt LoadPromptFromJsonFile(string filePath)
        {
            var json = File.ReadAllText(filePath);
            var prompt = JsonSerializer.Deserialize<Prompt>(json);
            return prompt ?? new Prompt();
        }
    }
}