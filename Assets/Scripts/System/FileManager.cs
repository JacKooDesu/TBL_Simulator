using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace TBL
{
    // A class to save & load file
    public static class FileManager
    {
        public static void SaveJson<T>(T target, string path, string fileName, string fileType)
        {
            var serializeData = JsonUtility.ToJson(target);
            var filePath = Application.dataPath + path;

            Directory.CreateDirectory(filePath);
            File.WriteAllText(filePath + fileName + "." + fileType, serializeData);
        }

        public static T LoadJson<T>(string path, string fileName, string fileType)
        {
            var filePath = Application.dataPath + path + fileName + "." + fileType;
            var deserializeData = (string)(null);

            try
            {
                deserializeData = File.ReadAllText(filePath);
            }
            catch (System.IO.FileNotFoundException)
            {
                return default(T);
            }
            catch (System.IO.DirectoryNotFoundException)
            {
                return default(T);
            }

            return JsonUtility.FromJson<T>(deserializeData);
        }

        public static void SaveImage(byte[] bytes, string path, string fileName, string fileType)
        {
            var filePath = Application.dataPath + path;
            Directory.CreateDirectory(filePath);
            File.WriteAllBytes(filePath + fileName + "." + fileType, bytes);
        }

        public static Texture2D LoadImage(string path, string fileName, string fileType)
        {
            var filePath = Application.dataPath + path + fileName + "." + fileType;
            var deserializeData = (byte[])(null);
            Texture2D texture = new Texture2D(2, 2);

            try
            {
                deserializeData = File.ReadAllBytes(filePath);
            }
            catch (System.IO.FileNotFoundException)
            {
                return default(Texture2D);
            }
            catch (System.IO.DirectoryNotFoundException)
            {
                return default(Texture2D);
            }

            texture.LoadImage(File.ReadAllBytes(filePath), false);
            return texture;
        }

        public static List<string> LoadDirFiles(string dir, string type)
        {
            DirectoryInfo di = new DirectoryInfo(Application.dataPath + dir);
            List<string> fileList = new List<string>();

            if (di.GetFiles(type).Length == 0)
            {
                return fileList;
            }
            else
            {
                foreach (var f in di.GetFiles(type))
                {
                    fileList.Add(f.Name);
                }
            }

            return fileList;
        }

    }
}


