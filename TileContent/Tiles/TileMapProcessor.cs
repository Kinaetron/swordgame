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
  
    [ContentProcessor(DisplayName = "Tile Map Processor")]
    public class TileMapProcessor : ContentProcessor<
        XmlDocument, TileMapContent>
    {
        public override TileMapContent Process(
            XmlDocument input, ContentProcessorContext context)
        {
            TileMapContent map = new TileMapContent();

            XmlNodeList colLayers = input.GetElementsByTagName("CollisionLayer");
            foreach (XmlNode layer in colLayers)
            {
                map.CollisionLayers.Add(
                    context.BuildAsset<XmlDocument,
                CollisionLayerContent>(
                new ExternalReference<XmlDocument>(layer.InnerText),
                "CollisionLayerProcessor"));
            }

            //System.Diagnostics.Debugger.Launch();

            XmlNodeList tileLayers = input.GetElementsByTagName("TileLayer");
            foreach (XmlNode layer in tileLayers)
            {
                map.TileLayers.Add(
                    context.BuildAsset<XmlDocument, TileLayerContent>(
                    new ExternalReference<XmlDocument>(layer.InnerText),
                    "TileLayerProcessor"));
            }
                return map;
        }
    }
}