using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using System.Xml;



namespace TileContent.Tiles
{
    [ContentImporter(".map", DisplayName = "Tile Map Importer", DefaultProcessor = 
        "TileMapProcessor")]
    public class TileMapImporter : ContentImporter<XmlDocument>
    {
        public override XmlDocument Import(string filename, ContentImporterContext context)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);
            return doc;
        }
    }
}
