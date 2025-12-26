using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ezFFmpeg.Models.Common
{
    class JsonData
    {
        private readonly string _filePath;
        private JsonObject? _root;

        /// <summary>
        /// JSON ファイルを指定して読み込む
        /// </summary>
        /// <param name="filePath">JSON ファイルパス</param>
        public JsonData(string filePath)
        {
            _filePath = filePath;
            Load();
        }

        /// <summary>
        /// JSON を読み込む
        /// </summary>
        public void Load()
        {
            if (!File.Exists(_filePath))
            {
                _root = [];
                return;
            }

            string json = File.ReadAllText(_filePath);
            _root = JsonNode.Parse(json)?.AsObject() ?? [];
        }

        /// <summary>
        /// JSON を保存する
        /// </summary>
        public void Save()
        {
            if (_root == null) return;
            string json = _root.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }

        /// <summary>
        /// プロパティの取得（文字列）
        /// </summary>
        public string GetString(string key, string defaultValue = "")
        {
            return _root?[key]?.GetValue<string>() ?? defaultValue;
        }

        /// <summary>
        /// プロパティの設定（文字列）
        /// </summary>
        public void SetString(string key, string value)
        {
            if (_root == null) return;
            _root[key] = value;
        }

        /// <summary>
        /// 配列の取得
        /// </summary>
        public JsonArray? GetArray(string key)
        {
            return _root?[key]?.AsArray();
        }

        /// <summary>
        /// 配列の設定
        /// </summary>
        public void SetArray(string key, JsonArray array)
        {
            if (_root == null) return;
            _root[key] = array;
        }

        /// <summary>
        /// 汎用で JsonNode を取得
        /// </summary>
        public JsonNode? GetNode(string key)
        {
            return _root?[key];
        }

        /// <summary>
        /// 汎用で JsonNode を設定
        /// </summary>
        public void SetNode(string key, JsonNode node)
        {
            if (_root == null) return;
            _root[key] = node;
        }
    }
}
