using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using System.Reflection;
using Microsoft.Xna.Framework.Graphics;

namespace SwordGame
{
    public class TileLayerReader : ContentTypeReader<TileLayer>
    {
        protected override TileLayer Read(ContentReader input, TileLayer existingInstance)
        {
            int height = input.ReadInt32();
            int width = input.ReadInt32();

            TileLayer layer = new TileLayer(width, height);

            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    layer.SetCellIndex(x, y, input.ReadInt32());

            List<TempTexture> textures = new List<TempTexture>();

            int numTexture = input.ReadInt32();
            for (int i = 0; i < numTexture; i++)
            {
                TempTexture t = new TempTexture();
                t.Texture = input.ReadExternalReference<Texture2D>();
                t.Index = input.ReadInt32();
                textures.Add(t);
            }

            textures.Sort(delegate(TempTexture a, TempTexture b)
            {
                return a.Index.CompareTo(b.Index);
            });

            foreach (TempTexture t in textures)
                layer.AddTexture(t.Texture);

            int numProperties = input.ReadInt32();
            for (int i = 0; i < numProperties; i++)
            {
                string name = input.ReadString();
                string value = input.ReadString();

                PropertyInfo propInfo = typeof(TileLayer).GetProperty(name);
                object realValue = null;

                if (propInfo.PropertyType == typeof(float))
                    realValue = float.Parse(value);
                else if (propInfo.PropertyType == typeof(int))
                    realValue = int.Parse(value);
                else if (propInfo.PropertyType == typeof(double))
                    realValue = double.Parse(value);
                else if (propInfo.PropertyType == typeof(string))
                    realValue = value;

                propInfo.SetValue(layer, realValue, null);
            }

            return layer;
        }
    }

    class TempTexture
    {
        public Texture2D Texture;
        public int Index;
    }
}
