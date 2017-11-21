using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace UnityGLTF {

	/// <summary>
	/// Component to load a GLTF scene with
	/// </summary>
	class GLTFComponent : MonoBehaviour
	{
		public string Url;
		public bool Multithreaded = true;
		public bool UseStream = false;

		public int MaximumLod = 300;

		public Shader GLTFStandard;
        public Shader GLTFStandardSpecular;
        public Shader GLTFConstant;

        public bool addColliders = false;

		public IEnumerator Load(Action CompleteCallback)
		{
			GLTFSceneImporter loader = null;
			FileStream gltfStream = null;
			if (UseStream)
			{
				var fullPath = Application.streamingAssetsPath + Url;
				gltfStream = File.OpenRead(fullPath);
                Stream str = gltfStream;
#if TESTING
                MemoryStream ms = new MemoryStream();
                var br = new BinaryReader(gltfStream);
                var str = Convert.ToBase64String(br.ReadBytes((int)gltfStream.Length));

                // Return a fixed model in byte array format..
                var ba = Convert.FromBase64String(str);

                var msw = new BinaryWriter(ms);
                msw.Write(ba);

                ms.Position = 0;
                str = ms;

#endif

                loader = new GLTFSceneImporter(
					fullPath,
					str,
					gameObject.transform,
                    addColliders
					);
			}
			else
			{
				loader = new GLTFSceneImporter(
					Url,
					gameObject.transform,
                    addColliders
					);
			}

            loader.SetShaderForMaterialType(GLTFSceneImporter.MaterialType.PbrMetallicRoughness, GLTFStandard);
            loader.SetShaderForMaterialType(GLTFSceneImporter.MaterialType.KHR_materials_pbrSpecularGlossiness, GLTFStandardSpecular);
            loader.SetShaderForMaterialType(GLTFSceneImporter.MaterialType.CommonConstant, GLTFConstant);
			loader.MaximumLod = MaximumLod;
			yield return loader.Load(-1, Multithreaded);
            CompleteCallback();
			if(gltfStream != null)
			{
#if WINDOWS_UWP
				gltfStream.Dispose();
#else
				gltfStream.Close();
#endif
			}
		}
	}
}
