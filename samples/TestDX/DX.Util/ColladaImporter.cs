using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using SharpDX;

namespace SharpHelper
{
    /// <summary>
    /// Importer for collada
    /// </summary>
    public static class ColladaImporter
    {
        /// <summary>
        /// Namespace
        /// </summary>
        public const string ColladaNamespace = @"http://www.collada.org/2005/11/COLLADASchema";

        internal enum ChannelCode
        {
            Position,
            Normal,
            Tangent,
            Binormal,
            TexCoord1,
            TexCoord2,
            Joint,
            Weight,
            None
        }

        /// <summary>
        /// Define custom Semantic Associations
        /// </summary>
        public class SemanticAssociations
        {
            /// <summary>
            /// Position
            /// </summary>
            public string Position { get; set; }

            /// <summary>
            /// Normal
            /// </summary>
            public string Normal { get; set; }

            /// <summary>
            /// Tangent
            /// </summary>
            public string Tangent { get; set; }

            /// <summary>
            /// Binormal
            /// </summary>
            public string Binormal { get; set; }

            /// <summary>
            /// Coordinate texture 1
            /// </summary>
            public string TexCoord1 { get; set; }

            /// <summary>
            /// Cordinate texture 2
            /// </summary>
            public string TexCoord2 { get; set; }

            /// <summary>
            /// Joint
            /// </summary>
            public string Joint { get; set; }

            /// <summary>
            /// Weight
            /// </summary>
            public string Weight { get; set; }

            /// <summary>
            /// Constructor
            /// </summary>
            public SemanticAssociations()
            {
                Position = "VERTEX";
                Normal = "NORMAL";
                Tangent = "TEXTANGENT";
                Binormal = "TEXBINORMAL";
                TexCoord1 = "TEXCOORD";
                TexCoord2 = "TEXCOORD";
                Joint = "JOINT";
                Weight = "WEIGHT";
            }

            internal ChannelCode this[string semantic]
            {
                get
                {
                    if (semantic == Position)
                        return ChannelCode.Position;
                    if (semantic == Normal)
                        return ChannelCode.Normal;
                    if (semantic == Tangent)
                        return ChannelCode.Tangent;
                    if (semantic == Binormal)
                        return ChannelCode.Binormal;
                    if (semantic == TexCoord1)
                        return ChannelCode.TexCoord1;
                    if (semantic == TexCoord2)
                        return ChannelCode.TexCoord2;
                    if (semantic == Joint)
                        return ChannelCode.Joint;
                    if (semantic == Weight)
                        return ChannelCode.Weight;
                    return ChannelCode.None;

                }
            }

        }
         
 
        private static Vector4 GetColor(XElement element, string elementName)
        {
            Vector4 color = new Vector4();
            XElement elementList = element.GetNode(elementName);
            if (elementList != null)
            {
                XElement res = elementList.GetNode("color");
                if (res != null)
                {
                    var data = GetFloatList(res.Value);
                    color.X = data[0];
                    color.Y = data[1];
                    color.Z = data[2];
                    if (data.Count > 3)
                        color.W = data[3];
                }
            }

            return color;
        }

        private static float GetValue(XElement element, string elementName)
        {
            XElement elementList = element.GetNode(elementName);
            if (elementList != null)
            {
                XElement res = elementList.GetNode("float");
                if (res != null)
                {
                    var data = GetFloatList(res.Value);
                    return data[0];
                }
            }
            return 0;
        }

        private static string GetTextureName(XElement element, string elementName)
        {
            XElement elementList = element.GetNode(elementName);
            if (elementList == null)
                return "";

            XElement res = elementList.GetNode("texture");
            if (res == null)
                return "";

            //Get Texture Sampler
            string texSampler = res.Attribute("texture").Value;
            res = element.GetNodes("newparam").Where(t => t.Attribute("sid").Value == texSampler).FirstOrDefault();
            if (res == null)
                return "";

            //Get Source
            string origin = res.GetNode("source").Value;
            res = element.GetNodes("newparam").Where(t => t.Attribute("sid").Value == origin).FirstOrDefault();
            if (res == null)
                return "";

            //Get Image
            string immagine = res.GetNode("init_from").Value;
            XElement tex = element.Document.GetNodes("image").Where(t => t.Attribute("id").Value == immagine).FirstOrDefault();

            if (tex == null)
                return "";

            //Get Texture Path
            return tex.GetNode("init_from").Value;

        }

      
        #region Utilities

        private static XElement GetByID(this XDocument document, string id)
        {
            return document.Descendants().Where(n => n.Attribute("id") != null && n.Attribute("id").Value == id).FirstOrDefault();
        }

        private static XElement GetReference(this XDocument document, XElement reference)
        {
            string url = reference.Attribute("url").Value.Replace("#", "");
            return document.Descendants().Where(n => n.Attribute("id") != null && n.Attribute("id").Value == url).FirstOrDefault();
        }

        private static XElement GetSource(this XDocument document, XElement reference)
        {
            string url = reference.Attribute("source").Value.Replace("#", "");
            return document.Descendants().Where(n => n.Attribute("id") != null && n.Attribute("id").Value == url).FirstOrDefault();
        }

        private static XElement GetReference(this XElement reference)
        {
            string url = reference.Attribute("url").Value.Replace("#", "");
            return reference.Document.Descendants().Where(n => n.Attribute("id") != null && n.Attribute("id").Value == url).FirstOrDefault();
        }

        private static XElement GetSource(this XElement reference)
        {
            string url = reference.Attribute("source").Value.Replace("#", "");
            return reference.Document.Descendants().Where(n => n.Attribute("id") != null && n.Attribute("id").Value == url).FirstOrDefault();
        }


        private static XElement GetNode(this XElement element, string name)
        {
            return element.Descendants(XName.Get(name, ColladaNamespace)).FirstOrDefault();
        }

        private static IEnumerable<XElement> GetNodes(this XElement element, string name)
        {
            return element.Descendants(XName.Get(name, ColladaNamespace));
        }

        private static XElement GetNode(this XDocument document, string name)
        {
            return document.Descendants(XName.Get(name, ColladaNamespace)).FirstOrDefault();
        }

        private static IEnumerable<XElement> GetNodes(this XDocument document, string name)
        {
            return document.Descendants(XName.Get(name, ColladaNamespace));
        }

        private static XElement GetChild(this XElement element, string name)
        {
            return element.Element(XName.Get(name, ColladaNamespace));
        }

        private static IEnumerable<XElement> GetChildren(this XElement element, string name)
        {
            return element.Elements(XName.Get(name, ColladaNamespace));
        }

        private static string GetAttribute(this XElement element, string name)
        {
            var res = element.Attribute(name);
            if (res == null)
                return string.Empty;

            return res.Value;
        }

        private static Vector3 Vector3FromString(string array)
        {
            array = array.Replace('\n', ' ');
            string[] val = array.Split(new char[] { ' ' });
            System.Globalization.NumberFormatInfo info = new System.Globalization.NumberFormatInfo();
            info.NumberDecimalSeparator = ".";
            return new Vector3()
            {
                X = float.Parse(val[0], info),
                Y = float.Parse(val[1], info),
                Z = float.Parse(val[2], info)
            };
        }

        private static Vector4 Vector4FromString(string array)
        {
            array = array.Replace('\n', ' ');
            string[] val = array.Split(new char[] { ' ' });
            System.Globalization.NumberFormatInfo info = new System.Globalization.NumberFormatInfo();
            info.NumberDecimalSeparator = ".";
            return new Vector4()
            {
                X = float.Parse(val[0], info),
                Y = float.Parse(val[1], info),
                Z = float.Parse(val[2], info),
                W = float.Parse(val[3], info),
            };
        }

        private static Matrix MatrixFromString(string array)
        {
            array = array.Replace('\n', ' ');
            string[] val = array.Split(new char[] { ' ' });
            System.Globalization.NumberFormatInfo info = new System.Globalization.NumberFormatInfo();
            info.NumberDecimalSeparator = ".";
            Matrix mat = new Matrix();

            mat.M11 = float.Parse(val[0], info);
            mat.M21 = float.Parse(val[1], info);
            mat.M31 = float.Parse(val[2], info);
            mat.M41 = float.Parse(val[3], info);

            mat.M12 = float.Parse(val[4], info);
            mat.M22 = float.Parse(val[5], info);
            mat.M32 = float.Parse(val[6], info);
            mat.M42 = float.Parse(val[7], info);

            mat.M13 = float.Parse(val[8], info);
            mat.M23 = float.Parse(val[9], info);
            mat.M33 = float.Parse(val[10], info);
            mat.M43 = float.Parse(val[11], info);

            mat.M14 = float.Parse(val[12], info);
            mat.M24 = float.Parse(val[13], info);
            mat.M34 = float.Parse(val[14], info);
            mat.M44 = float.Parse(val[15], info);

            return mat;
        }

        private static List<float> GetFloatList(string array)
        {
            array = array.Replace('\n', ' ');
            List<float> data = new List<float>();

            string[] number = array.Split(new char[] { ' ' });

            System.Globalization.NumberFormatInfo info = new System.Globalization.NumberFormatInfo();
            info.NumberDecimalSeparator = ".";

            for (int i = 0; i < number.Length; i++)
            {
                if (number[i] != "")
                    data.Add(float.Parse(number[i], info));
            }

            return data;
        }

        private static List<int> GetIntList(string array)
        {
            array = array.Replace('\n', ' ');
            List<int> data = new List<int>();

            string[] number = array.Split(new char[] { ' ' });

            for (int i = 0; i < number.Length; i++)
            {
                if (number[i] != "")
                    data.Add(int.Parse(number[i]));
            }

            return data;
        }

        private static Matrix ToTranspose(this Matrix matrix)
        {
            return Matrix.Transpose(matrix);
        }
        #endregion

        #region Support

        private class ChannelData
        {
            public int IndexOffset { get; set; }

            public string Semantic { get; set; }

            public List<float> Data { get; set; }

            public int Stride { get; set; }

            public int Offset { get; set; }

            public ChannelData(XElement element)
            {
                Semantic = element.Attribute("semantic").Value;

                //get vertices
                var temp = element.GetSource();
                if (temp.Name.LocalName.ToLower() == "vertices")
                {
                    temp = temp.GetNode("input").GetSource();
                }

                //Get stride
                XElement teck = temp.GetNode("technique_common");
                if (teck != null)
                {
                    int s = 1;
                    int.TryParse(temp.GetNode("accessor").Attribute("stride").Value, out s);
                    Stride = s;
                }
                Data = GetFloatList(temp.GetNode("float_array").Value);
            }

            public float[] GetChannel(int pos)
            {
                float[] res = new float[Stride];
                for (int i = 0; i < Stride; i++)
                {
                    res[i] = Data[pos * Stride + i];
                }
                return res;
            }

        }
        
        #endregion

    }
}
