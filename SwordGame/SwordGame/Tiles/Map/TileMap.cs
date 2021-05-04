using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Xml;
using Microsoft.Xna.Framework.Graphics;

namespace SwordGame
{
    public class TileMap
    {
        public List<TileLayer> Layers = new List<TileLayer>();
        public List<CollisionLayer> colLayers = new List<CollisionLayer>();

        public List<string> coll = new List<string>();
        public List<string> tile = new List<string>();

        public int GetWidthInPixels()
        {
            return GetWidth() * Engine.TileWidth;
        }

        public int GetHeightInPixels()
        {
            return GetHeight() * Engine.TileHeight;
        }

        public int GetWidth()
        {
            int width = -1000;

            foreach (TileLayer layer in Layers)
                width = (int)Math.Max(width, layer.Width);

            return width;
        }

        /// <summary>
        /// Gets the bounding rectangle of a tile in world space.
        /// </summary>        
        public Rectangle GetBounds(int x, int y)
        {
            return new Rectangle(x * Engine.TileWidth, y * Engine.TileHeight, Engine.TileWidth, Engine.TileHeight);
        }

        public int GetHeight()
        {
            int height = -1000;

            foreach (TileLayer layer in Layers)
                height = (int)Math.Max(height, layer.Height);

            return height;
        }

        public void OpenTileFile(string filename)
        {
            XmlDocument theFile = new XmlDocument();
            theFile.Load(filename);
            //string[] tilePaths = new string[2];


            foreach (XmlNode node in theFile.DocumentElement.ChildNodes)
            { 
               if (node.Name == "CollisionLayer")
                    coll.Add(node.InnerText);

                if (node.Name == "TileLayer")
                    tile.Add(node.InnerText);
            }

            //Dictionary<string, string> pathsDict = new Dictionary<string, string>();
            //pathsDict.Add(coll.ToString(), tile.ToString());
            //return pathsDict;
        }

        public void Save(string MapName, string[] collisionLayerName, string[] tileLayerName)
        {
            XmlDocument doc = new XmlDocument();

            XmlElement rootElement = doc.CreateElement("Map");
            doc.AppendChild(rootElement);

            for (int i = 0; i < tileLayerName.Length; i++)
            {
                XmlElement tileElement = doc.CreateElement("TileLayer");
                tileElement.InnerText = "Layers/" + tileLayerName[i];
                rootElement.AppendChild(tileElement);
            }

            for (int j = 0; j < collisionLayerName.Length; j++)
            {
                XmlElement collisionElement = doc.CreateElement("CollisionLayer");
                collisionElement.InnerText = "Layers/" + collisionLayerName[j];
                rootElement.AppendChild(collisionElement);
            }

            doc.Save(MapName);
        }

        public void Draw(SpriteBatch spritebatch, Camera camera)
        {
            Point min = Engine.ConvertPositionToCell(camera.Position);
            Point max = Engine.ConvertPositionToCell(
                camera.Position + new Vector2(
                    spritebatch.GraphicsDevice.Viewport.Width + Engine.TileWidth,
                    spritebatch.GraphicsDevice.Viewport.Height + Engine.TileHeight));

            foreach (TileLayer layer in Layers)
                layer.Draw(spritebatch, camera, min, max);
        }
    }
}
