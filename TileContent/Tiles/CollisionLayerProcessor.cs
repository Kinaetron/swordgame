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
    [ContentProcessor(DisplayName = "Collision Layer Processor")]
    public class CollisionLayerProcessor : ContentProcessor<XmlDocument, CollisionLayerContent>
    {
        public override CollisionLayerContent Process(
            XmlDocument input, ContentProcessorContext context)
        {
            CollisionLayerContent layer = new CollisionLayerContent();


            foreach (XmlNode node in input.DocumentElement.ChildNodes)
            {
                if (node.Name == "Layout")
                {
                    int width = int.Parse(node.Attributes["Width"].Value);
                    int height = int.Parse(node.Attributes["Height"].Value);

                    layer.Layout = new string[height, width];

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