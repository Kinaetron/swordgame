using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace SwordGame
{
    public class TileMapReader : ContentTypeReader<TileMap>
    {
        protected override TileMap Read(ContentReader input, TileMap existingInstance)
        {
            TileMap map = new TileMap();

            int collNumLayers = input.ReadInt32();
            for (int i = 0; i < collNumLayers; i++)
                map.colLayers.Add(input.ReadExternalReference<CollisionLayer>());

            int numLayers = input.ReadInt32();
            for (int i = 0; i < numLayers; i++)
                map.Layers.Add(input.ReadExternalReference<TileLayer>());

            return map;
        }
    }
}
