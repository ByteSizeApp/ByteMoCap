using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

namespace ByteScript.MoCap.Util
{
    public class FileManager : MonoBehaviour
    {
        /// <summary>
        /// Serializes an object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serializableObject"></param>
        /// <param name="fileName"></param>
        public static void SerializeObject<T>(T serializableObject, MoCapManager.Group group, string prefix, string fileName)
        {
            if (serializableObject == null) { return; }

            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                XmlSerializer serializer = new XmlSerializer(serializableObject.GetType());

                string folderPath = FolderPath() + "/" + group;
                folderPath = String.IsNullOrEmpty(prefix) ? folderPath : folderPath + "/" + prefix;
                if (!Directory.Exists(folderPath))
                {
                    Debug.Log("Creating folderPath: " + folderPath);
                    Directory.CreateDirectory(folderPath);
                }

                using (FileStream stream = new FileStream(folderPath + "/" + fileName, FileMode.Create))
                {
                    serializer.Serialize(stream, serializableObject);
                    stream.Position = 0;
                    xmlDocument.Load(stream);
                    xmlDocument.Save(fileName);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("cannot save object" + ex);
            }
        }

        /// <summary>
        /// Deserializes an xml file into an object list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static T DeSerializeObject<T>(MoCapManager.Group group, string prefix, string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) { return default(T); }

            T objectOut = default(T);

            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                string folderPath = FolderPath() + "/" + group;
                folderPath = String.IsNullOrEmpty(prefix) ? folderPath : folderPath + "/" + prefix;

                Debug.Log("Openning " + folderPath + "/" + fileName);
                using (FileStream stream = new FileStream(folderPath + "/" + fileName, FileMode.Open))
                {
                    stream.Position = 0;
                    xmlDocument.Load(stream);
                    xmlDocument.Load(fileName);
                }
                string xmlString = xmlDocument.OuterXml;

                using (StringReader read = new StringReader(xmlString))
                {
                    Type outType = typeof(T);

                    XmlSerializer serializer = new XmlSerializer(outType);
                    using (XmlReader reader = new XmlTextReader(read))
                    {
                        objectOut = (T)serializer.Deserialize(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("cannot load object" + ex);
            }

            return objectOut;
        }

        static string folderPath;
        private static string FolderPath()
        {
            if (folderPath == null)
            {
                folderPath = Application.streamingAssetsPath + "/ByteMocap";
            }
            return folderPath;
        }
    }
}