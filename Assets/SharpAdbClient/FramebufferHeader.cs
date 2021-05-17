// <copyright file="FramebufferHeader.cs" company="The Android Open Source Project, Ryan Conrad, Quamotion">
// Copyright (c) The Android Open Source Project, Ryan Conrad, Quamotion. All rights reserved.
// </copyright>

namespace SharpAdbClient
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;

    /// <summary>
    /// Whenever the <c>framebuffer:</c> service is invoked, the adb server responds with the contents
    /// of the framebuffer, prefixed with a <see cref="FramebufferHeader"/> object that contains more
    /// information about the framebuffer.
    /// </summary>
    public struct FramebufferHeader
    {
        /// <summary>
        /// Gets or sets the version of the framebuffer sturcture.
        /// </summary>
        public uint Version { get; set; }

        /// <summary>
        /// Gets or sets the number of bytes per pixel. Usual values include 32 or 24.
        /// </summary>
        public uint Bpp { get; set; }

        /// <summary>
        /// Gets or sets the color space. Only available starting with <see cref="Version"/> 2.
        /// </summary>
        public uint ColorSpace { get; set; }

        /// <summary>
        /// Gets or sets the total size, in bits, of the framebuffer.
        /// </summary>
        public uint Size { get; set; }

        /// <summary>
        /// Gets or sets the width, in pixels, of the framebuffer.
        /// </summary>
        public uint Width { get; set; }

        /// <summary>
        /// Gets or sets the height, in pixels, of the framebuffer.
        /// </summary>
        public uint Height { get; set; }

        /// <summary>
        /// Gets or sets information about the red color channel.
        /// </summary>
        public ColorData Red { get; set; }

        /// <summary>
        /// Gets or sets information about the blue color channel.
        /// </summary>
        public ColorData Blue { get; set; }

        /// <summary>
        /// Gets or sets information about the green color channel.
        /// </summary>
        public ColorData Green { get; set; }

        /// <summary>
        /// Gets or sets information about the alpha channel.
        /// </summary>
        public ColorData Alpha { get; set; }

        /// <summary>
        /// Creates a new <see cref="FramebufferHeader"/> object based on a byte arra which contains the data.
        /// </summary>
        /// <param name="data">
        /// The data that feeds the <see cref="FramebufferHeader"/> structure.
        /// </param>
        /// <returns>
        /// A new <see cref="FramebufferHeader"/> object.
        /// </returns>
        public static FramebufferHeader Read(byte[] data)
        {
            // as defined in https://android.googlesource.com/platform/system/core/+/master/adb/framebuffer_service.cpp
            FramebufferHeader header = default(FramebufferHeader);

            // Read the data from a MemoryStream so we can use the BinaryReader to process the data.
            using (MemoryStream stream = new MemoryStream(data))
            using (BinaryReader reader = new BinaryReader(stream, Encoding.ASCII, leaveOpen: true))
            {
                header.Version = reader.ReadUInt32();

                if (header.Version > 2)
                {
                    // Technically, 0 is not a supported version either; we assume version 0 indicates
                    // an empty framebuffer.
                    throw new InvalidOperationException($"Framebuffer version {header.Version} is not supported");
                }

                header.Bpp = reader.ReadUInt32();

                if (header.Version >= 2)
                {
                    header.ColorSpace = reader.ReadUInt32();
                }

                header.Size = reader.ReadUInt32();
                header.Width = reader.ReadUInt32();
                header.Height = reader.ReadUInt32();
                header.Red = new ColorData()
                {
                    Offset = reader.ReadUInt32(),
                    Length = reader.ReadUInt32()
                };

                header.Blue = new ColorData()
                {
                    Offset = reader.ReadUInt32(),
                    Length = reader.ReadUInt32()
                };

                header.Green = new ColorData()
                {
                    Offset = reader.ReadUInt32(),
                    Length = reader.ReadUInt32()
                };

                header.Alpha = new ColorData()
                {
                    Offset = reader.ReadUInt32(),
                    Length = reader.ReadUInt32()
                };
            }

            return header;
        }

    }
}
