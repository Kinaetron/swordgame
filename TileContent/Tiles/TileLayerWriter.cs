using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace TileContent.Tiles
{
    [ContentTypeWriter]
    public class TileLayerWriter : ContentTypeWriter<TileLayerContent>
    {
        protected override void Write(ContentWriter output, TileLayerContent value)
        {
            output.Write(value.Layout.GetLength(0));
            output.Write(value.Layout.GetLength(1));
            for (int y = 0; y < value.Layout.GetLength(0); y++)
                for (int x = 0; x < value.Layout.GetLength(1); x++)
                    output.Write(value.Layout[y, x]);

            output.Write(value.Textures.Count);

            foreach (TileLayerTextureContent textContent in value.Textures)
            {
                output.WriteExternalReference<TextureContent>(textContent.Texture);
                output.Write(textContent.Index);
            }

            output.Write(value.Properties.Count);

            foreach (TileLayerPropertyContent propContent in value.Properties)
            {
                output.Write(propContent.Name);
                output.Write(propContent.Value);
            }
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "SwordGame.TileLayerReader, SwordGame";
        }
    }
}
