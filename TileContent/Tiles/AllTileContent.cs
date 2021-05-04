using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace TileContent.Tiles
{
    public class TileMapContent
    {
        public List <ExternalReference<CollisionLayerContent>> CollisionLayers =
            new List<ExternalReference<CollisionLayerContent>>();

        public List<ExternalReference<TileLayerContent>> TileLayers =
            new List<ExternalReference<TileLayerContent>>();
    }

    public class CollisionLayerContent
    {
        public string[,] Layout;
    }

    public class TileLayerContent
    {
        public Collection<TileLayerTextureContent> Textures =
            new Collection<TileLayerTextureContent>();


        public Collection<TileLayerPropertyContent> Properties =
            new Collection<TileLayerPropertyContent>();

        public int[,] Layout;
     }

    public class TileLayerTextureContent
    {
        public ExternalReference<TextureContent> Texture;
        public int Index; 
    }

    public class TileLayerPropertyContent
    {
        public string Name;
        public string Value;
    }

    public class CameraReference
    {
        public int Index;
        public int Start;
        public int End;
    }
}
