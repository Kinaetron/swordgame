using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Xml;
using System.Reflection;
using Microsoft.Xna.Framework.Content;

namespace SwordGame
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TileLayerSavedPropertyAttribute : Attribute
    {
    }

    public class TemperTexture
    {
        public string Texture;
        public int Index;
    }

    public class TileLayer
    {
        List<Texture2D> tileTextures = new List<Texture2D>();
        int[,] map;
        float alpha = 1f;

        [TileLayerSavedProperty]
        public float Alpha
        {
            get { return alpha; }
            set
            {
                alpha = MathHelper.Clamp(value, 0f, 1f);
            }
        }

        public int WidthInPixels
        {
            get
            {
                return Width * Engine.TileWidth;
            }
        }

        public int HeightInPixels
        {
            get
            {
                return Height * Engine.TileHeight;
            }
        }

        public int Width
        {
            get { return map.GetLength(1); }
        }


        public int Height
        {
            get { return map.GetLength(0); }
        }

        public TileLayer(int width, int height)
        {
            map = new int[height, width];

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    map[y, x] = -1;

        }

        public TileLayer(int[,] existingMap)
        {
            map = (int[,])existingMap.Clone();
        }

        public int isUsingTexture(Texture2D texture)
        {
            if (tileTextures.Contains(texture))
                return tileTextures.IndexOf(texture);

            return -1;
        }

        public void Save(string filename, string[] textureNames)
        {
            XmlDocument doc = new XmlDocument();

            XmlElement rootElement = doc.CreateElement("TileLayer");
            doc.AppendChild(rootElement);

            XmlElement textureElement = doc.CreateElement("Textures");
            rootElement.AppendChild(textureElement);

            for (int i = 0; i < textureNames.Length; i++)
            {
                string t = textureNames[i];

                XmlElement tElement = doc.CreateElement("Texture");
                XmlAttribute tAttr = doc.CreateAttribute("File");
                tAttr.Value = t;
                XmlAttribute tAttr2 = doc.CreateAttribute("ID");
                tAttr2.Value = i.ToString();

                tElement.Attributes.Append(tAttr);
                tElement.Attributes.Append(tAttr2);

                textureElement.AppendChild(tElement);
            }

            PropertyInfo[] properties = typeof(TileLayer).GetProperties();

            List<PropertyInfo> propertiesToSave = new List<PropertyInfo>();

            foreach (PropertyInfo p in properties)
            {
                object[] attributes = p.GetCustomAttributes(
                    typeof(TileLayerSavedPropertyAttribute), false);

                if (attributes.Length > 0)
                    propertiesToSave.Add(p);
            }

            XmlElement propertiesElement = doc.CreateElement("Properties");
            rootElement.AppendChild(propertiesElement);

            foreach (PropertyInfo p in propertiesToSave)
            {
                XmlElement pElement = doc.CreateElement(p.Name);
                pElement.InnerText = p.GetValue(this, null).ToString();

                propertiesElement.AppendChild(pElement);
            }

            XmlElement layoutElement = doc.CreateElement("Layout");

            string line = string.Empty;

            for (int y = 0; y < Height; y++)
            {
                string formatString = string.Empty;

                for (int x = 0; x < Width; x++)
                {
                    formatString += map[y, x].ToString() + " ";
                }
                if (y == 0)
                    line += System.Environment.NewLine + formatString + '\r' + '\n';
                else
                    line += formatString + '\r' + '\n';
            }

            layoutElement.InnerText = line;

            XmlAttribute widthAttribute = doc.CreateAttribute("Width");
            XmlAttribute heightAttribute = doc.CreateAttribute("Height");

            widthAttribute.Value = Width.ToString();
            heightAttribute.Value = Height.ToString();

            layoutElement.Attributes.Append(widthAttribute);
            layoutElement.Attributes.Append(heightAttribute);

            rootElement.AppendChild(layoutElement);

            doc.Save(filename);
        }

        public static TileLayer FromFile(string filename, out string[] textureNameArray)
        {
            TileLayer tileLayer;

            List<string> textureNames = new List<string>();

            tileLayer = ProcessFile(filename, textureNames);

            textureNameArray = textureNames.ToArray();

            return tileLayer;
        }

        private static TileLayer ProcessFile(string filename, List<string> textureNames)
        {
            TileLayer tileLayer;
            List<List<int>> tempLayout = new List<List<int>>();
            Dictionary<string, string> properties = new Dictionary<string, string>();

            XmlDocument input = new XmlDocument();

            input.Load(filename);

            int width = 0;
            int height = 0;

            tileLayer = new TileLayer(width, height);

            foreach (XmlNode node in input.DocumentElement.ChildNodes)
            {
                if (node.Name == "Textures")
                {
                    List<TemperTexture> textures = new List<TemperTexture>();

                    foreach (XmlNode textureNode in node.ChildNodes)
                    {

                        TemperTexture t = new TemperTexture();

                        t.Texture = textureNode.Attributes["File"].Value;
                        t.Index = int.Parse(textureNode.Attributes["ID"].Value);

                        textures.Add(t);
                    }

                    textures.Sort(delegate(TemperTexture a, TemperTexture b)
                    {
                        return a.Index.CompareTo(b.Index);
                    });

                    foreach (TemperTexture t in textures)
                        textureNames.Add(t.Texture);
                }
                else if (node.Name == "Layout")
                {
                    width = int.Parse(node.Attributes["Width"].Value);
                    height = int.Parse(node.Attributes["Height"].Value);

                    tileLayer = new TileLayer(width, height);

                    string layout = node.InnerText;

                    string[] lines = layout.Split('\r', '\n');

                    int row = 0;

                    foreach (string line in lines)
                    {
                        string realLine = line.Trim();

                        if (string.IsNullOrEmpty(line))
                            continue;

                        string[] cells = realLine.Split(' ');

                        for (int x = 0; x < width; x++)
                        {
                            int cellIndex = int.Parse(cells[x]);
                            tileLayer.SetCellIndex(x, row, cellIndex);
                        }

                        row++;
                    }
                }
                else if (node.Name == "Properties")
                {
                    foreach (XmlNode propertyNode in node.ChildNodes)
                    {
                        string key = propertyNode.Name;
                        string value = propertyNode.InnerText;

                        properties.Add(key, value);
                    }
                }
            }

            foreach (KeyValuePair<string, string> property in properties)
            {
                switch (property.Key)
                {
                    case "Alpha":
                        tileLayer.Alpha = float.Parse(property.Value);
                        break;
                }
            }
            return tileLayer;
        }

        public static TileLayer FromFile(ContentManager content, string filename)
        {
            TileLayer tileLayer;

            List<string> textureNames = new List<string>();

            tileLayer = ProcessFile(filename, textureNames);

            tileLayer.LoadTileTextures(content, textureNames.ToArray());

            return tileLayer;
        }

        public void LoadTileTextures(ContentManager content, params string[] textureNames)
        {
            Texture2D texture;

            foreach (string textureName in textureNames)
            {
                texture = content.Load<Texture2D>(textureName);
                tileTextures.Add(texture);
            }
        }

        public void AddTexture(Texture2D texture)
        {
            tileTextures.Add(texture);
        }

        public void RemoveTexture(Texture2D texture)
        {
            RemoveIndex(tileTextures.IndexOf(texture));
            tileTextures.Remove(texture);
        }

        public void SetCellIndex(int x, int y, int cellIndex)
        {
            map[y, x] = cellIndex;
        }

        public void SetCellIndex(Point point, int cellIndex)
        {
            map[point.Y, point.X] = cellIndex;
        }

        public int GetCellIndex(int x, int y)
        {
            return map[y, x];
        }

        public int GetCellIndex(Point point)
        {
            return map[point.Y, point.X];
        }


        public void RemoveIndex(int existingIndex)
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (map[y, x] == existingIndex)
                        map[y, x] = -1;
                    else if (map[y, x] > existingIndex)
                        map[y, x]--;
                }
            }
        }

        public void ReplaceIndex(int existingIndex, int newIndex)
        {
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    if (map[y, x] == existingIndex)
                        map[y, x] = newIndex;
        }

        public void DrawEditor(SpriteBatch spriteBatch, Camera camera)
        {

            spriteBatch.Begin(SpriteSortMode.Texture,
                   BlendState.AlphaBlend,
                   null, null, null, null,
                   camera.TransformMatrix);


            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    int textureIndex = map[y, x];

                    if (textureIndex == -1)
                        continue;

                    Texture2D texture = tileTextures[textureIndex];
                    spriteBatch.Draw(
                        texture,
                        new Rectangle(x * Engine.TileWidth - (int)camera.Position.X,
                                      y * Engine.TileHeight - (int)camera.Position.Y,
                                      Engine.TileWidth,
                                      Engine.TileHeight),
                        new Color(new Vector4(1f, 1f, 1f, Alpha)));
                }
            }

            spriteBatch.End();
        }

        public void Draw(SpriteBatch spriteBatch, Camera camera)
        {

            spriteBatch.Begin(SpriteSortMode.Texture,
                   BlendState.AlphaBlend,
                   SamplerState.PointClamp, null, null, null,
                   camera.TransformMatrix);


            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    int textureIndex = map[y, x];

                    if (textureIndex == -1)
                        continue;

                    Texture2D texture = tileTextures[textureIndex];
                    spriteBatch.Draw(
                        texture,
                        new Rectangle(x * Engine.TileWidth,
                                      y * Engine.TileHeight,
                                      Engine.TileWidth,
                                      Engine.TileHeight),
                        new Color(new Vector4(1f, 1f, 1f, Alpha)));
                }
            }

            spriteBatch.End();
        }

        public void Draw(SpriteBatch spriteBatch, Camera camera, Point min, Point max)
        {

            spriteBatch.Begin(SpriteSortMode.Texture,
                   BlendState.AlphaBlend,
                   null, null, null, null,
                   camera.TransformMatrix);


            min.X = (int)Math.Max(min.X, 0);
            min.Y = (int)Math.Max(min.Y, 0);

            max.X = (int)Math.Min(max.X, Width);
            max.Y = (int)Math.Min(max.Y, Height);

            for (int x = min.X; x < max.X; x++)
            {
                for (int y = min.Y; y < max.Y; y++)
                {
                    int textureIndex = map[y, x];

                    if (textureIndex == -1)
                        continue;

                    Texture2D texture = tileTextures[textureIndex];
                    spriteBatch.Draw(
                        texture,
                        new Rectangle(x * Engine.TileWidth,
                                      y * Engine.TileHeight,
                                      Engine.TileWidth,
                                      Engine.TileHeight),
                        new Color(new Vector4(1f, 1f, 1f, Alpha)));
                }
            }

            spriteBatch.End();
        }
    }
}
