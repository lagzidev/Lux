﻿// Taken from Nathan Hoad: https://gist.github.com/nathanhoad/bf9ddac2e13a5aaa922182005f3da6e5
// Very special thanks to Noel Berry
// A lot of this is borrowed from https://gist.github.com/NoelFB/778d190e5d17f1b86ebf39325346fcc5

using System.Collections.Generic;
using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Linq;
using Microsoft.Xna.Framework;

namespace LuxEngine
{
    public enum Modes
    {
        Indexed = 1,
        Grayscale = 2,
        RGBA = 3
    }


    public class Aseprite
    {
        public Modes Mode;
        public int Width;
        public int Height;

        public List<AsepriteFrame> Frames;
        public List<AsepriteLayer> Layers;
        public List<AsepriteTag> Tags;
        public List<AsepriteSlice> Slices;


        private enum Chunks
        {
            OldPaletteA = 0x0004,
            OldPaletteB = 0x0011,
            Layer = 0x2004,
            Cel = 0x2005,
            CelExtra = 0x2006,
            Mask = 0x2016,
            Path = 0x2017,
            FrameTags = 0x2018,
            Palette = 0x2019,
            UserData = 0x2020,
            Slice = 0x2022
        }

        private enum CelTypes
        {
            RawCel = 0,
            LinkedCel = 1,
            CompressedImage = 2
        }

        /// <summary>
        /// Parses Aseprite files.
        /// </summary>
        /// <param name="filename">Filename</param>
        /// <param name="logger">Logger</param>
        public Aseprite(string filename)
        {
            Frames = new List<AsepriteFrame>();
            Layers = new List<AsepriteLayer>();
            Tags = new List<AsepriteTag>();
            Slices = new List<AsepriteSlice>();

            using (var stream = File.OpenRead(filename))
            {
                var reader = new BinaryReader(stream);

                // Helpers for translating the Aseprite file format reference
                // See: https://github.com/aseprite/aseprite/blob/master/docs/ase-file-specs.md
                byte BYTE() { return reader.ReadByte(); }
                ushort WORD() { return reader.ReadUInt16(); }
                short SHORT() { return reader.ReadInt16(); }
                uint DWORD() { return reader.ReadUInt32(); }
                long LONG() { return reader.ReadInt32(); }
                string STRING() { return Encoding.UTF8.GetString(BYTES(WORD())); }
                byte[] BYTES(int number) { return reader.ReadBytes(number); }
                void SEEK(int number) { reader.BaseStream.Position += number; }

                int frameCount;

                // Consume header
                {
                    DWORD();

                    // Magic number
                    var magic = WORD();
                    if (magic != 0xA5e0)
                    {
                        throw new Exception("File doesn't appear to be from Aseprite.");
                    }

                    // Basic info
                    frameCount = WORD();
                    Width = WORD();
                    Height = WORD();
                    Mode = (Modes)(WORD() / 8);

                    // Ignore a bunch of stuff
                    DWORD(); // Flags
                    WORD(); // Speed (deprecated)
                    DWORD(); // 0
                    DWORD(); // 0
                    BYTE(); // Palette entry 
                    SEEK(3); // Ignore these bytes
                    WORD(); // Number of colors (0 means 256 for old sprites)
                    BYTE(); // Pixel width
                    BYTE(); // Pixel height
                    SEEK(92); // For Future
                }

                // Some temporary holders
                var colorBuffer = new byte[Width * Height * (int)Mode];
                var palette = new Color[256];

                IUserData lastUserData = null;

                for (int i = 0; i < frameCount; i++)
                {
                    var frame = new AsepriteFrame();
                    Frames.Add(frame);

                    long frameStart;
                    long frameEnd;
                    int chunkCount;

                    // Frame header
                    {
                        frameStart = reader.BaseStream.Position;
                        frameEnd = frameStart + DWORD();
                        WORD(); // Magic number (always 0xF1FA)
                        chunkCount = WORD();
                        frame.Duration = WORD() / 1000f;
                        SEEK(6); // For future (set to zero)
                    }

                    for (int j = 0; j < chunkCount; j++)
                    {
                        long chunkStart;
                        long chunkEnd;
                        Chunks chunkType;

                        // Chunk header
                        {
                            chunkStart = reader.BaseStream.Position;
                            chunkEnd = chunkStart + DWORD();
                            chunkType = (Chunks)WORD();
                        }

                        // Layer
                        if (chunkType == Chunks.Layer)
                        {
                            var layer = new AsepriteLayer();
                            layer.Flag = (AsepriteLayer.Flags)WORD();
                            layer.Type = (AsepriteLayer.Types)WORD();
                            layer.ChildLevel = WORD();
                            WORD(); // width
                            WORD(); // height
                            layer.BlendMode = (AsepriteLayer.BlendModes)WORD();
                            layer.Opacity = BYTE() / 255f;
                            SEEK(3);
                            layer.Name = STRING();

                            lastUserData = layer;
                            Layers.Add(layer);
                        }

                        // Cel
                        else if (chunkType == Chunks.Cel)
                        {
                            var cel = new AsepriteCel();

                            var layerIndex = WORD();
                            cel.Layer = Layers[layerIndex]; // Layer is row (Frame is column)
                            cel.X = SHORT();
                            cel.Y = SHORT();
                            cel.Opacity = BYTE() / 255f;

                            var celType = (CelTypes)WORD();
                            SEEK(7);
                            if (celType == CelTypes.RawCel || celType == CelTypes.CompressedImage)
                            {
                                cel.Width = WORD();
                                cel.Height = WORD();

                                var byteCount = cel.Width * cel.Height * (int)Mode;

                                if (celType == CelTypes.RawCel)
                                {
                                    reader.Read(colorBuffer, 0, byteCount);
                                }
                                else
                                {
                                    SEEK(2);
                                    var deflate = new DeflateStream(reader.BaseStream, CompressionMode.Decompress);
                                    deflate.Read(colorBuffer, 0, byteCount);
                                }

                                cel.Pixels = new Color[cel.Width * cel.Height];
                                ConvertBytesToPixels(colorBuffer, cel.Pixels, palette);
                            }
                            else if (celType == CelTypes.LinkedCel)
                            {
                                var targetFrame = WORD(); // Frame position to link with

                                // Grab the cel from a previous frame
                                var targetCel = Frames[targetFrame].Cels.Where(c => c.Layer == Layers[layerIndex]).First();
                                cel.Width = targetCel.Width;
                                cel.Height = targetCel.Height;
                                cel.Pixels = targetCel.Pixels;
                            }

                            lastUserData = cel;
                            frame.Cels.Add(cel);
                        }

                        // Palette
                        else if (chunkType == Chunks.Palette)
                        {
                            var size = DWORD();
                            var start = DWORD();
                            var end = DWORD();
                            SEEK(8);

                            for (int c = 0; c < (end - start); c++)
                            {
                                var hasName = LuxCommon.IsBitSet(WORD(), 0);
                                palette[start + c] = Color.FromNonPremultiplied(BYTE(), BYTE(), BYTE(), BYTE());
                                if (hasName)
                                {
                                    STRING(); // Color name
                                }
                            }
                        }

                        // User data
                        else if (chunkType == Chunks.UserData)
                        {
                            if (lastUserData != null)
                            {
                                var flags = DWORD();

                                if (LuxCommon.IsBitSet(flags, 0))
                                {
                                    lastUserData.UserDataText = STRING();
                                }
                                else if (LuxCommon.IsBitSet(flags, 1))
                                {
                                    lastUserData.UserDataColor = Color.FromNonPremultiplied(BYTE(), BYTE(), BYTE(), BYTE());
                                }
                            }
                        }

                        // Tag (animation reference)
                        else if (chunkType == Chunks.FrameTags)
                        {
                            var tagsCount = WORD();
                            SEEK(8);

                            for (int t = 0; t < tagsCount; t++)
                            {
                                var tag = new AsepriteTag();

                                tag.From = WORD();
                                tag.To = WORD();
                                tag.LoopDirection = (AsepriteTag.LoopDirections)BYTE();
                                SEEK(8);
                                tag.Color = Color.FromNonPremultiplied(BYTE(), BYTE(), BYTE(), 255);
                                SEEK(1);
                                tag.Name = STRING();

                                Tags.Add(tag);
                            }
                        }

                        // Slice
                        else if (chunkType == Chunks.Slice)
                        {
                            var slicesCount = DWORD();
                            var flags = DWORD();
                            DWORD();
                            var name = STRING();

                            for (int s = 0; s < slicesCount; s++)
                            {
                                var slice = new AsepriteSlice();
                                slice.Name = name;
                                slice.Frame = (int)DWORD();
                                slice.OriginX = (int)LONG();
                                slice.OriginY = (int)LONG();
                                slice.Width = (int)DWORD();
                                slice.Height = (int)DWORD();

                                // 9 slice
                                if (LuxCommon.IsBitSet(flags, 0))
                                {
                                    LONG(); // Center X position (relative to slice bounds)
                                    LONG(); // Center Y position
                                    DWORD(); // Center width
                                    DWORD(); // Center height
                                }

                                // Pivot
                                else if (LuxCommon.IsBitSet(flags, 1))
                                {
                                    slice.Pivot = new Point((int)DWORD(), (int)DWORD());
                                }

                                lastUserData = slice;
                                Slices.Add(slice);
                            }
                        }

                        reader.BaseStream.Position = chunkEnd;
                    }

                    reader.BaseStream.Position = frameEnd;
                }
            }
        }


        private void ConvertBytesToPixels(byte[] bytes, Color[] pixels, Color[] palette)
        {
            int length = pixels.Length;

            if (Mode == Modes.RGBA)
            {
                for (int pixel = 0, b = 0; pixel < length; pixel++, b += 4)
                {
                    pixels[pixel].R = (byte)(bytes[b + 0] * bytes[b + 3] / 255);
                    pixels[pixel].G = (byte)(bytes[b + 1] * bytes[b + 3] / 255);
                    pixels[pixel].B = (byte)(bytes[b + 2] * bytes[b + 3] / 255);
                    pixels[pixel].A = bytes[b + 3];
                }
            }
            else if (Mode == Modes.Grayscale)
            {
                for (int pixel = 0, b = 0; pixel < length; pixel++, b += 2)
                {
                    pixels[pixel].R = pixels[pixel].G = pixels[pixel].B = (byte)(bytes[b + 0] * bytes[b + 1] / 255);
                    pixels[pixel].A = bytes[b + 1];
                }
            }
            else if (Mode == Modes.Indexed)
            {
                for (int pixel = 0, paletteIndex = 0; pixel < length; pixel++, paletteIndex += 1)
                {
                    pixels[pixel] = palette[paletteIndex];
                }
            }
        }
    }


    // UserData are extended chunks that get attached
    // to other chunks
    public interface IUserData
    {
        string UserDataText { get; set; }
        Color UserDataColor { get; set; }
    }


    // A layer stores just the meta info for a row
    // of cels
    public class AsepriteLayer : IUserData
    {
        [Flags]
        public enum Flags
        {
            Visible = 1,
            Editable = 2,
            LockMovement = 4,
            Background = 8,
            PreferLinkedCels = 16,
            Collapsed = 32,
            Reference = 64
        }

        public enum Types
        {
            Normal = 0,
            Group = 1
        }

        public enum BlendModes
        {
            Normal = 0,
            Multiply = 1,
            Screen = 2,
            Overlay = 3,
            Darken = 4,
            Lighten = 5,
            ColorDodge = 6,
            ColorBurn = 7,
            HardLight = 8,
            SoftLight = 9,
            Difference = 10,
            Exclusion = 11,
            Hue = 12,
            Saturation = 13,
            Color = 14,
            Luminosity = 15,
            Addition = 16,
            Subtract = 17,
            Divide = 18
        }

        public Flags Flag;
        public Types Type;

        public string Name;
        public float Opacity;
        public BlendModes BlendMode;
        public int ChildLevel;

        string IUserData.UserDataText { get; set; }
        Color IUserData.UserDataColor { get; set; }
    }


    // A frame is a column of cels
    public class AsepriteFrame
    {
        public float Duration;
        public List<AsepriteCel> Cels;


        public AsepriteFrame()
        {
            Cels = new List<AsepriteCel>();
        }
    }


    // Tags are animation references
    public class AsepriteTag
    {
        public enum LoopDirections
        {
            Forward = 0,
            Reverse = 1,
            PingPong = 2
        }

        public string Name;
        public LoopDirections LoopDirection;
        public int From;
        public int To;
        public Color Color;
    }


    public struct AsepriteSlice : IUserData
    {
        public int Frame;
        public string Name;
        public int OriginX;
        public int OriginY;
        public int Width;
        public int Height;
        public Point? Pivot;

        string IUserData.UserDataText { get; set; }
        Color IUserData.UserDataColor { get; set; }
    }


    // Cels are just pixel grids
    public class AsepriteCel : IUserData
    {
        public AsepriteLayer Layer;

        public Color[] Pixels;

        public int X;
        public int Y;
        public int Width;
        public int Height;
        public float Opacity;

        public string UserDataText { get; set; }
        public Color UserDataColor { get; set; }
    }
}