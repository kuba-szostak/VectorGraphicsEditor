using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using VectorGraphicsEditor.Models;

namespace VectorGraphicsEditor.State
{
    public sealed class FileManager
    {
        private static readonly JsonSerializerOptions _options = BuildOptions();

        private static JsonSerializerOptions BuildOptions()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true,
            };
            return options;
        }


        public void Save(Scene scene, string filePath)
        {
            string json = JsonSerializer.Serialize<List<IShape>>(scene.Shapes, _options);
            File.WriteAllText(filePath, json);
        }

        public bool Load(Scene scene, string filePath)
        {
            string json = File.ReadAllText(filePath, System.Text.Encoding.UTF8);
            if (string.IsNullOrWhiteSpace(json)) return false;

            var shapes = JsonSerializer.Deserialize<List<IShape>>(json, _options);
            if (shapes == null || shapes.Count == 0) return false;

            scene.ReplaceAll(shapes);
            return true;
        }

    }
}
