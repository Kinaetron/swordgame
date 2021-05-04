using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using System.Xml;


namespace TileContent.Tiles
{
    [ContentProcessor(DisplayName = "Tile Layer Processor")]
    public class TileLayerProcessor : ContentProcessor<XmlDocument, TileLayerContent>
    {
        public override TileLayerContent Process(
            XmlDocument input, ContentProcessorContext context)
        {
            TileLayerContent layer = new TileLayerContent();


            foreach (XmlNode node in input.DocumentElement.ChildNodes)
            {
                if (node.Name == "Textures")
                {
                    foreach (XmlNode textureNode in node.ChildNodes)
                    {
                        string file = textureNode.Attributes["File"].Value;
                        int index = int.Parse(textureNode.Attributes["ID"].Value);

                        TileLayerTextureContent textureContent = new TileLayerTextureContent();
                        textureContent.Texture = context.BuildAsset<TextureContent, TextureContent>(
                            new ExternalReference<TextureContent>(file),
                            "TextureProcessor");
                        textureContent.Index = index;

                        layer.Textures.Add(textureContent);
                    }
                }
                else if (node.Name == "Properties")
                {
                    foreach (XmlNode propertyNode in node.ChildNodes)
                    {
                        TileLayerPropertyContent propConetent = new TileLayerPropertyContent();
                        propConetent.Name = propertyNode.Name;
                        propConetent.Value = propertyNode.InnerText;
                        layer.Properties.Add(propConetent);
                    }
                }
                else if (node.Name == "Layout")
                {
                    int width = int.Parse(node.Attributes["Width"].Value);
                    int height = int.Parse(node.Attributes["Height"].Value);

                    layer.Layout = new int[height, width];

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
                            layer.Layout[row, x] = cellIndex;
                        }

                        row++;
                    }
                }

            }

            return layer;
        }
    }
}