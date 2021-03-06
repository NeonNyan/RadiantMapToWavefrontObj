﻿using System.Collections.Generic;
using System.IO;

namespace RadiantMapToWavefrontObj
{
    public class WavefrontObj
    {
        public ObjObject[] Objects { get; private set; }

        // Constructor for an entire wavefront .obj file object.
        public WavefrontObj(ObjObject[] objects)
        {
            Objects = objects;
            Cleanup();
        }

        // Removes all faces containing a texture listed in the filter from all subobjects.
        public void FilterTextures(string[] filter)
        {
            foreach (ObjObject obj in Objects)
            {
                obj.FilterTextures(filter);
            }
            Cleanup();
        }

        // Returns .obj formatted text of this object.
        public string ToCode(double scale)
        {
            string res = "# Exported using Wesley Baartman's RadiantMapToWavefrontObj software.\n\n";

            int faceOffset = 0;

            // Adds code for each object contained.
            foreach (ObjObject obj in Objects)
            {
                res += obj.ToCode(scale, faceOffset) + "\n";
                faceOffset += obj.Vertices.Length;
            }

            return res;
        }

        // Saves this object to an .obj file.
        public void SaveFile(string path, double scale)
        {
            File.WriteAllText(path, ToCode(scale));
        }

        // Removes objects that lack faces or vertices.
        private void Cleanup()
        {
            List<ObjObject> newObjects = new List<ObjObject>();

            foreach (ObjObject obj in Objects)
            {
                if (obj.Faces != null && obj.Faces.Length > 0 && obj.Vertices.Length > 0)
                    newObjects.Add(obj);
            }

            Objects = newObjects.ToArray();

        }

        // Converts a RadiantMap object to a WavefrontObj object.
        public static WavefrontObj CreateFromRadiantMap(RadiantMap map)
        {
            List<ObjObject> objects = new List<ObjObject>();

            for (int i = 0; i < map.Brushes.Length; ++i)
            {
                Brush brush = map.Brushes[i];
                ObjObject obj = ObjObject.CreateFromBrush("Brush_" + i, brush);
                objects.Add(obj);
            }

            for (int i = 0; i < map.Patches.Length; ++i)
            {
                Patch patch = map.Patches[i];
                ObjObject obj = ObjObject.CreateFromPatch("Patch_" + i, patch);
                objects.Add(obj);
            }

            return new WavefrontObj(objects.ToArray());
        }
    }
}
