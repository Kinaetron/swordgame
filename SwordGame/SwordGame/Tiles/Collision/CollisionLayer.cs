using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using System.Xml;
using Microsoft.Xna.Framework;

namespace SwordGame
{
    public class CollisionLayer
    {
        List<Texture2D> collisionTextures = new List<Texture2D>();

        string[,] map;

        public int Width
        {
            get { return map.GetLength(1); }
        }


        public int Height
        {
            get { return map.GetLength(0); }
        }

        public CollisionLayer(int width, int height)
        {
            map = new string[height, width];

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    map[y, x] = "0";
        }


        public void Save(string filename)
        {
            XmlDocument doc = new XmlDocument();

            XmlElement rootElement = doc.CreateElement("CollisionLayer");
            doc.AppendChild(rootElement);

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


        public static CollisionLayer FromFile(string filename)
        {
            CollisionLayer tileLayer;
            List<List<int>> tempLayout = new List<List<int>>();

            Dictionary<int, string> tempHold = new Dictionary<int, string>();

            XmlDocument input = new XmlDocument();

            input.Load(filename);

            int width = 0;
            int height = 0;

            tileLayer = new CollisionLayer(width, height);

            foreach (XmlNode node in input.DocumentElement.ChildNodes)
            {
               if (node.Name == "Layout")
                {
                    width = int.Parse(node.Attributes["Width"].Value);
                    height = int.Parse(node.Attributes["Height"].Value);

                    tileLayer = new CollisionLayer(width, height);

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
                            string cellIndex = cells[x];
                            tileLayer.SetCellIndex(x, row, cellIndex);
                        }

                        row++;
                    }
                }
            }
            return tileLayer;
        }

        public void SetCellIndex(int x, int y, string cellIndex)
        {
            map[y, x] = cellIndex;
        }

        public void SetCellIndex(Point point, string cellIndex)
        {
            map[point.Y, point.X] = cellIndex;
        }

        public string GetCellIndex(int x, int y)
        {
            return map[y, x];
        }

        public string GetCellIndex(Point point)
        {
            return map[point.Y, point.X];
        }

        public void ReplaceIndex(string existingIndex, string newIndex)
        {
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    if (map[y, x] == existingIndex)
                        map[y, x] = newIndex;
        }
    }
}
