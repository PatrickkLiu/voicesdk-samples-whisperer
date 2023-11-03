// Pcx - Point cloud importer & renderer for Unity
// https://github.com/keijiro/Pcx

using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Pcx
{
    //[UnityEditor.AssetImporters.ScriptedImporter(1, "ply")] //custom asset importer (priority when multiple importer are involved-lower number means higher priority, file extension filter)
    public class CustomizedPlyImporter : MonoBehaviour
    {
        #region ScriptedImporter implementation  
        //  is a code comment pragma used in C# to create a collapsible region in code editors, such as Visual Studio
        
        //public enum ContainerType { Mesh, ComputeBuffer, Texture  }
        //Enums (enumeration) are used to define a set of named constants(), which can be assigned to variables 
        // each named value corresponds to an integer, starting from 0 for the first value and incrementing by 1 for each subsequent value by default.
        //  In this case, Mesh represents the value 0, by default.

        //[SerializeField] ContainerType _containerType = ContainerType.Texture;

        /*
        public override void OnImportAsset(UnityEditor.AssetImporters.AssetImportContext context)
        // override: the method is accessible from outside the class
        // void: does not return any value
        // context: 
            //context.assetPath: Provides the path to the asset being imported.
            //context.assetName: Provides the name of the asset being imported.

        {
            
            if (_containerType == ContainerType.Mesh)
            {
                // Mesh container
                // Create a prefab with MeshFilter/MeshRenderer.
                var gameObject = new GameObject();
                var mesh = ImportAsMesh(context.assetPath);

                var meshFilter = gameObject.AddComponent<MeshFilter>();
                meshFilter.sharedMesh = mesh;

                var meshRenderer = gameObject.AddComponent<MeshRenderer>();
                meshRenderer.sharedMaterial = GetDefaultMaterial();

                context.AddObjectToAsset("prefab", gameObject);
                
                if (mesh != null) context.AddObjectToAsset("mesh", mesh);
                //context.AddObjectToAsset(name, obj): Allows you to add objects as 'string identifier' to the asset being imported.

                context.SetMainObject(gameObject);
                // Sets the main object for the asset. This is typically the primary object that represents the asset when viewing. it's also the root of prefab, and a reference point for scripting
            }
            else if (_containerType == ContainerType.ComputeBuffer)
            {
                // ComputeBuffer container
                // Create a prefab with PointCloudRenderer.
                var gameObject = new GameObject();
                var data = ImportAsPointCloudData(context.assetPath);

                var renderer = gameObject.AddComponent<PointCloudRenderer>();
                renderer.sourceData = data;

                context.AddObjectToAsset("prefab", gameObject);
                if (data != null) context.AddObjectToAsset("data", data);

                context.SetMainObject(gameObject);
            }
            else // _containerType == ContainerType.Texture
            {
                // Texture container
                // No prefab is available for this type.
                var data = ImportAsBakedPointCloud(context.assetPath);
                if (data != null)
                {
                    context.AddObjectToAsset("container", data);
                    context.AddObjectToAsset("position", data.positionMap);
                    context.AddObjectToAsset("color", data.colorMap);
                    context.SetMainObject(data);
                }
            }
            

            if (_containerType == ContainerType.Texture)
            {
                var data = ImportAsBakedPointCloud(context.assetPath); // data = blob data
           
                if (data != null)
                {
                    context.AddObjectToAsset("container", data);
                    context.AddObjectToAsset("position", data.positionMap);
                    context.AddObjectToAsset("color", data.colorMap);
                    context.SetMainObject(data);
                }
            }

        } */

        #endregion

        #region Internal utilities
        /*
        static Material GetDefaultMaterial()
        {
            // Via package manager
            var path_upm = "Packages/jp.keijiro.pcx/Editor/Default Point.mat";
            // Via project asset database
            var path_prj = "Assets/Pcx/Editor/Default Point.mat";
            return AssetDatabase.LoadAssetAtPath<Material>(path_upm) ??
                   AssetDatabase.LoadAssetAtPath<Material>(path_prj);
        }
        */

        #endregion

        #region Internal data structure
        // utility that's used in Reader Implementation

        enum DataProperty {
            Invalid,
            R8, G8, B8, A8,
            R16, G16, B16, A16,
            SingleX, SingleY, SingleZ,
            DoubleX, DoubleY, DoubleZ,
            Data8, Data16, Data32, Data64
        }

        static int GetPropertySize(DataProperty p)
        {
            switch (p)
            {
                case DataProperty.R8: return 1; //an 8-bit color value, which is 1 byte in size.
                case DataProperty.G8: return 1;
                case DataProperty.B8: return 1;
                case DataProperty.A8: return 1;
                case DataProperty.R16: return 2; //a 16-bit color value, which is 2 byte in size. smoother gradients and more accurate color representation.
                case DataProperty.G16: return 2;
                case DataProperty.B16: return 2;
                case DataProperty.A16: return 2;
                case DataProperty.SingleX: return 4; //SingleX for 32-bit single-precision floating-point X-coordinate, 4 byte in size. Single-precision floats are typically sufficient for representing 3D coordinates and transformations in real-time graphics 
                case DataProperty.SingleY: return 4;
                case DataProperty.SingleZ: return 4;
                case DataProperty.DoubleX: return 8; //DoubleX for 64-bit double-precision floating-point X-coordinate, 8 byte
                case DataProperty.DoubleY: return 8;
                case DataProperty.DoubleZ: return 8;
                case DataProperty.Data8: return 1;
                case DataProperty.Data16: return 2;
                case DataProperty.Data32: return 4;
                case DataProperty.Data64: return 8;
            }
            return 0;
        }

        class DataHeader
        {
            // return data type for readDataHeader
            public List<DataProperty> properties = new List<DataProperty>();
            public int vertexCount = -1;
        }

        class DataBody
        {
            // return data type for readDataBody
            public List<Vector3> vertices;
            public List<Color32> colors;

            public DataBody(int vertexCount)
            {
                vertices = new List<Vector3>(vertexCount);
                colors = new List<Color32>(vertexCount);
            }

            public void AddPoint(
                float x, float y, float z,
                byte r, byte g, byte b, byte a
            )
            {
                vertices.Add(new Vector3(x, y, z));
                colors.Add(new Color32(r, g, b, a));
            }
        }

        #endregion

        #region Reader implementation
        //parsing the binary data 
        /*
        Mesh ImportAsMesh(string path)
        {
            try
            {
                var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                var header = ReadDataHeader(new StreamReader(stream));
                var body = ReadDataBody(header, new BinaryReader(stream));

                var mesh = new Mesh();
                mesh.name = Path.GetFileNameWithoutExtension(path);

                mesh.indexFormat = header.vertexCount > 65535 ?
                    IndexFormat.UInt32 : IndexFormat.UInt16;

                mesh.SetVertices(body.vertices);
                mesh.SetColors(body.colors);

                mesh.SetIndices(
                    Enumerable.Range(0, header.vertexCount).ToArray(),
                    MeshTopology.Points, 0
                );

                mesh.UploadMeshData(true);
                return mesh;
            }
            catch (Exception e)
            {
                Debug.LogError("Failed importing " + path + ". " + e.Message);
                return null;
            }
        }

        PointCloudData ImportAsPointCloudData(string path)
        {
            try
            {
                var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                var header = ReadDataHeader(new StreamReader(stream));
                var body = ReadDataBody(header, new BinaryReader(stream));
                var data = ScriptableObject.CreateInstance<PointCloudData>();
                data.Initialize(body.vertices, body.colors);
                data.name = Path.GetFileNameWithoutExtension(path);
                return data;
            }
            catch (Exception e)
            {
                Debug.LogError("Failed importing " + path + ". " + e.Message);
                return null;
            }
        }
        */
        public BakedPointCloud ImportAsCustomPointCloud(MemoryStream stream)
        {
            try
            {
                //var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                //control how the file is opened:specifying that it should be opened for reading and that it can be accessed and shared for reading 
                var header = ReadDataHeader();
                //StreamReader  is a class in .NET for reading text files, such as plain text, CSV, JSON, or configuration files.
                var body = ReadDataBody(header, new BinaryReader(stream));
                //Binaryreader is for non-text data, such as images, audio, 3D model data, or custom binary file formats.
                var data = ScriptableObject.CreateInstance<BakedPointCloud>();
                data.Initialize(body.vertices, body.colors);
                //data.name = Path.GetFileNameWithoutExtension(path);
                data.name = "test";
                return data;
            }
            catch (Exception e)
            {
                Debug.LogError("Failed importing " + ". " + e.Message);
                return null;
            }
        }

        //The header section of a PLY file contains information about the structure and properties of the data that follows in the body of the file.
         /*
ply
format binary_little_endian 1.0
element vertex 4096
property float x
property float y
property float z
property uchar red
property uchar green
property uchar blue
end_header            
         */    
        DataHeader ReadDataHeader() // it returns DataHeader, and takes in streamReader
        {
            
            var data = new DataHeader();
            /*
            //to store information parsed from the PLY file header.
            var readCount = 0;

            // Magic number line ("ply")
            var line = reader.ReadLine();
            //StreamReader starts by reading the PLY file's magic number, which is typically the first line and should be "ply."
            readCount += line.Length + 1;
            if (line != "ply")
                throw new ArgumentException("Magic number ('ply') mismatch.");

            // Data format: check if it's binary/little endian.
            line = reader.ReadLine();
            // second line should be specifying binary/little endian format
            readCount += line.Length + 1;
            if (line != "format binary_little_endian 1.0")
                throw new ArgumentException(
                    "Invalid data format ('" + line + "'). " +
                    "Should be binary/little endian.");

            // Read header contents.

            for (var skip = false;;)
            {
                // Read a line and split it into an array of strings with white space.
                line = reader.ReadLine();
                readCount += line.Length + 1;
                if (line == "end_header") break;
                var col = line.Split();


                //The function iterates through the header lines, which can include elements (e.g., vertices, faces), property declarations (e.g., vertex positions, colors)

                // Element declaration (unskippable)
                if (col[0] == "element") //if the first word in the line is element
                {
                    if (col[1] == "vertex") // if the second word in the line is vertex
                    {
                        data.vertexCount = Convert.ToInt32(col[2]); //convert the third word into integer format, and assign it to the vertexCount variable for data
                        skip = false;
                    }
                    else
                    {
                        // Don't read elements other than vertices.
                        skip = true;
                    }
                }

                if (skip) continue; //we scanned all the vertex line already, and only true left, and we advance to the next iteration

                // Property declaration line
                if (col[0] == "property")
                {
                    var prop = DataProperty.Invalid;
                    //DataProperty was defined as enum -- a set of constants, including name entry of R8 G8 B8 A8, and  single XYZ

                    // Parse the property name entry.
                    switch (col[2])
                    {
                        case "red"  : prop = DataProperty.R8; break;
                        case "green": prop = DataProperty.G8; break;
                        case "blue" : prop = DataProperty.B8; break;
                        case "alpha": prop = DataProperty.A8; break;
                        case "x"    : prop = DataProperty.SingleX; break;
                        case "y"    : prop = DataProperty.SingleY; break;
                        case "z"    : prop = DataProperty.SingleZ; break;
                    }
                    //now we at least noew its color or corordinate, but we don't know the size. we need the second colomn to know

                    // Check the property type.
                    // if 8, then as defalt 8 bit color, no prob. only check if it's invalid
                    if (col[1] == "char" || col[1] == "uchar" ||
                        col[1] == "int8" || col[1] == "uint8")
                    {
                        if (prop == DataProperty.Invalid)
                            prop = DataProperty.Data8;
                        else if (GetPropertySize(prop) != 1)
                            throw new ArgumentException("Invalid property type ('" + line + "').");
                    }
                    //if 16, convert 8 bit color to 16 bit color
                    else if (col[1] == "short" || col[1] == "ushort" ||
                             col[1] == "int16" || col[1] == "uint16")
                    {

                        switch (prop)
                        {
                            case DataProperty.Invalid: prop = DataProperty.Data16; break;
                            case DataProperty.R8: prop = DataProperty.R16; break;
                            case DataProperty.G8: prop = DataProperty.G16; break;
                            case DataProperty.B8: prop = DataProperty.B16; break;
                            case DataProperty.A8: prop = DataProperty.A16; break;
                        }
                        if (GetPropertySize(prop) != 2)
                            throw new ArgumentException("Invalid property type ('" + line + "').");
                    }
                    // if 32, then as defalt single float, no prob. only check if it's invalid
                    else if (col[1] == "int"   || col[1] == "uint"   || col[1] == "float" ||
                             col[1] == "int32" || col[1] == "uint32" || col[1] == "float32")
                    {
                        if (prop == DataProperty.Invalid)
                            prop = DataProperty.Data32;
                        else if (GetPropertySize(prop) != 4)
                            throw new ArgumentException("Invalid property type ('" + line + "').");
                    }
                    // if 64, convert single float to double float
                    else if (col[1] == "int64"  || col[1] == "uint64" ||
                             col[1] == "double" || col[1] == "float64")
                    {
                        switch (prop) 
                        {
                            
                            case DataProperty.Invalid: prop = DataProperty.Data64; break;
                            case DataProperty.SingleX: prop = DataProperty.DoubleX; break;
                            case DataProperty.SingleY: prop = DataProperty.DoubleY; break;
                            case DataProperty.SingleZ: prop = DataProperty.DoubleZ; break;
                        }
                        if (GetPropertySize(prop) != 8)
                            throw new ArgumentException("Invalid property type ('" + line + "').");
                    }
                    else
                    {
                        throw new ArgumentException("Unsupported property type ('" + line + "').");
                    }

                    data.properties.Add(prop);
                }
            }

            */
            data.vertexCount = 4096;
            data.properties.Add(DataProperty.SingleX);
            data.properties.Add(DataProperty.SingleY);
            data.properties.Add(DataProperty.SingleZ);
            data.properties.Add(DataProperty.R8);
            data.properties.Add(DataProperty.G8);
            data.properties.Add(DataProperty.B8);


            //maybe this would work too
            //data.properties = new new List<DataProperty>{DataProperty.R8, DataProperty.R8, DataProperty.R8, DataProperty.SingleX, DataProperty.SingleX, DataProperty.SingleX};


            // Rewind the stream back to the exact position of the reader.
            //reader.BaseStream.Position = readCount;

            return data;
        }

        DataBody ReadDataBody(DataHeader header, BinaryReader reader)
        {
            //vertex count = 4096
            var data = new DataBody(header.vertexCount);

            float x = 0, y = 0, z = 0;
            Byte r = 255, g = 255, b = 255, a = 255;

            for (var i = 0; i < header.vertexCount; i++)
            {
                foreach (var prop in header.properties)
                {
                    switch (prop)
                    {
                        //8 bit color
                        case DataProperty.R8: r = reader.ReadByte(); break;
                        //reader.ReadByte read only one byte (8 bits) at a time, and then return an interger ranging from 0 to 255
                        case DataProperty.G8: g = reader.ReadByte(); break;
                        case DataProperty.B8: b = reader.ReadByte(); break;
                        case DataProperty.A8: a = reader.ReadByte(); break;

                        case DataProperty.R16: r = (byte)(reader.ReadUInt16() >> 8); break;
                        case DataProperty.G16: g = (byte)(reader.ReadUInt16() >> 8); break;
                        case DataProperty.B16: b = (byte)(reader.ReadUInt16() >> 8); break;
                        case DataProperty.A16: a = (byte)(reader.ReadUInt16() >> 8); break;

                        // single float
                        case DataProperty.SingleX: x = reader.ReadSingle(); break;
                        //it read the next 4 byte (32 bit) and return a single-precision float
                        case DataProperty.SingleY: y = reader.ReadSingle(); break;
                        case DataProperty.SingleZ: z = reader.ReadSingle(); break;

                        case DataProperty.DoubleX: x = (float)reader.ReadDouble(); break;
                        case DataProperty.DoubleY: y = (float)reader.ReadDouble(); break;
                        case DataProperty.DoubleZ: z = (float)reader.ReadDouble(); break;

                        case DataProperty.Data8: reader.ReadByte(); break;
                        case DataProperty.Data16: reader.BaseStream.Position += 2; break;
                        case DataProperty.Data32: reader.BaseStream.Position += 4; break;
                        case DataProperty.Data64: reader.BaseStream.Position += 8; break;

                        // the order of the cases matters in terms of execution sequence.
                    }
                }

                data.AddPoint(x, y, z, r, g, b, a);
                //store a (x,y,z) in a postion array, and a (r,g,b,a) in a color32 array
            }

            return data;
        }
    }

    #endregion
    //  the end of the region definition.
}
