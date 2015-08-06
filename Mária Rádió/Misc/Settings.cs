using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace Maria_Radio.Misc
{
    public class Settings: ErrorEvent
    {
        protected Format format;
        protected FileInfo filePath;
        protected Dictionary<string, Variant> settingsMap;
        protected const string ROOT_ELEMENT = "Settings";
        protected const string ATTR_TYPE = "type";

        public enum Format
        {
            Xml
        };

        public Settings(string filePath): this(filePath, Format.Xml)
        {   
        }

        public Settings(string filePath, Format format)
        {
            this.filePath = new FileInfo(filePath);
            this.format = format;

            if (!Directory.Exists(this.filePath.DirectoryName))
            {
                Directory.CreateDirectory(this.filePath.DirectoryName);
            }

            settingsMap = new Dictionary<string, Variant>();
        }

        public virtual void load()
        {
            switch (format)
            {
                default:
                    loadXml();
                    break;
            }
        }

        public virtual void save()
        {
            switch (format)
            {
                default:
                    saveXml();
                    break;
            }
        }

        private void loadXml()
        {
            try
            {
                XmlTextReader xmlReader = new XmlTextReader(filePath.FullName);
                string lastKey = null;
                string lastType = null;


                while (xmlReader.Read())
                {
                    switch (xmlReader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (xmlReader.Name != ROOT_ELEMENT)
                            {
                                lastKey = xmlReader.Name;
                                lastType = xmlReader.GetAttribute(ATTR_TYPE);
                            }
                            break;

                        case XmlNodeType.EndElement:
                            lastKey = null;
                            lastType = null;
                            break;
                        case XmlNodeType.Text:
                            if (lastKey != null && lastType != null)
                            {
                                setValue(lastKey, Variant.create(xmlReader.Value, Type.GetType(lastType)));
                            }
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                OnError(e);
            }
        }

        private void saveXml()
        {
            try
            {
                XmlTextWriter xmlWriter = new XmlTextWriter(filePath.FullName, Encoding.UTF8);

                xmlWriter.Formatting = Formatting.Indented;
                xmlWriter.WriteStartDocument();

                xmlWriter.WriteComment("Saved: " + DateTime.Now.ToString(CultureInfo.CurrentCulture));

                xmlWriter.WriteStartElement(ROOT_ELEMENT);

                foreach (KeyValuePair<string, Variant> pair in settingsMap)
                {
                    xmlWriter.WriteStartElement(pair.Key);

                    Type t = pair.Value.GetType();

                    if (t.Namespace.Equals("System"))
                    {
                        xmlWriter.WriteAttributeString(ATTR_TYPE, t.FullName);
                    }
                    else
                    {
                        xmlWriter.WriteAttributeString(ATTR_TYPE,
                            t.FullName + ", " +
                            t.Namespace + ", " +
                            "Version=" + Environment.Version + ", " +
                            "Culture=neutral, " +
                            "PublicKeyToken=b03f5f7f11d50a3a"
                        );
                    }
                    
                    xmlWriter.WriteString(pair.Value.ToString());

                    xmlWriter.WriteEndElement();
                }

                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndDocument();

                xmlWriter.Close();
            }
            catch (Exception e)
            {
                OnError(e);
            }
        }

        public void setValue(string key, object value)
        {
            Variant variant = new Variant(value);

            if (!settingsMap.ContainsKey(key))
            {
                settingsMap.Add(key, variant);
            }
            else
            {
                settingsMap[key] = variant;
            }

            variant.onError += OnError;
        }

        public T getValue<T>(string key, T defaultValue)
        {
            if (settingsMap.ContainsKey(key))
            {
                return settingsMap[key].Get<T>();
            }
            return defaultValue;
        }
    }
}
