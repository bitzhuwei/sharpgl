﻿using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using CelShadingSample;
using GlmNet;
using NUnit.Framework;
using SharpGL.Enumerations;
using SharpGL.RenderContextProviders;
using SharpGL.Shaders;
using SharpGL.Tests.Contexts;
using SharpGL.Tests.Helpers;
using SharpGL.Version;
using SharpGL.VertexBuffers;

namespace SharpGL.Tests.BasicShaders
{
    [TestFixture(
        Description = 
            "This test ensures we can create an FBO Render Context Provider for OpenGL 3.0. " +
            "It tests that we can create a shader program, vertex shader, fragment shader, " +
            "compile the program, build geometry in a VBA, set attributes and render using " +
            "the shader.")]
    class BasicShadersTest : RenderingTest
    {
        private const int Width = 1024;
        private const int Height = 768;

        [Test]
        [Ignore("It is impossible to let this test run until we can successfully demand a specific OpenGL version.")]
        public void CanPerformBasicRendering()
        {
            //  Create an OpenGL instance.
            var gl = new OpenGL();
            
            //var context = new OffscreenContext();
            //try
            //{
            //    context.Create(OpenGLVersion.OpenGL3_0, gl, 800, 600, 32);
            //}
            //catch (OpenGLVersionException versionException)
            //{
            //    Assert.Inconclusive(versionException.Message);
            //}

            gl.Create(OpenGLVersion.OpenGL3_0, RenderContextType.FBO, 800, 600, 32, null);

            gl.Viewport(0, 0, Width, Height);
            Assert.AreEqual(ErrorCode.NoError, gl.GetErrorCode(), "OpenGL error during render context setup.");

            //  TODO: Make sure we've got at least OpenGL 3.0.


            //  Give our attributes codes (how we refer to them in C#) and
            //  names (how we load them as 'in' data in the shader).
            const uint positionAttribute = 0;
            const uint normalAttribute = 1;
            var attributeLocations = new Dictionary<uint, string>
            {
                {positionAttribute, "Position"},
                {normalAttribute, "Normal"},
            };

            //  Create a Shader Program.
            var program = new ShaderProgram();
            program.Create(gl,
                ManifestResourceLoader.LoadTextFile(@"BasicShaders.PerPixel.vert"),
                ManifestResourceLoader.LoadTextFile(@"BasicShaders.PerPixel.frag"), attributeLocations);

            Assert.AreEqual(ErrorCode.NoError, gl.GetErrorCode(), "OpenGL error during shader compilation.");

            //  Create raw torus geometry.
            var torus = new TorusGeometry(1f, .3f, 24, 18);

            //  Create a VBA.
            var vertexBufferArray = new VertexBufferArray();
            vertexBufferArray.Create(gl);
            vertexBufferArray.Bind(gl);

            //  Bind the vertices and normals.
            var vertexBuffer = new VertexBuffer();
            vertexBuffer.Create(gl);
            vertexBuffer.Bind(gl);
            vertexBuffer.SetData(gl, positionAttribute, torus.vertices.SelectMany(v => v.to_array()).ToArray(), false, 3);

            var normalBuffer = new VertexBuffer();
            normalBuffer.Create(gl);
            normalBuffer.Bind(gl);
            normalBuffer.SetData(gl, normalAttribute, torus.normals.SelectMany(v => v.to_array()).ToArray(), false, 3);

            var indexBuffer = new IndexBuffer();
            indexBuffer.Create(gl);
            indexBuffer.Bind(gl);
            indexBuffer.SetData(gl, torus.triangles.Select(u => (ushort)u).ToArray());

            vertexBufferArray.Unbind(gl);

            Assert.AreEqual(ErrorCode.NoError, gl.GetErrorCode(), "OpenGL error during VBA setup.");

            //  TODO: I can't quite get this right. Need to revisit the geometry and also the 
            //  projection matrix (perhaps a simple sample that does geometry/projection/shader).

            //  Create the projection matrix for our screen size.
            const float S = 0.46f;
            float H = S * Height / Width;
            var projectionMatrix = glm.frustum(-S, S, -H, H, 1, 100);
//            var projectionMatrix = glm.perspective(glm.radians(60f), Height/Width, 1, 100);
            var modelView = glm.lookAt(new vec3(4f, 4f, 4f), new vec3(0f, 0f, 0f), new vec3(0f, 1f, 0f));

            //  Use the shader program.
            program.Bind(gl);

            //  Set the variables for the shader program.
            program.SetUniform3(gl, "DiffuseMaterial", 0f, 0.75f, 0.75f);
            program.SetUniform3(gl, "AmbientMaterial", 0.04f, 0.04f, 0.04f);
            program.SetUniform3(gl, "SpecularMaterial", 0.5f, 0.5f, 0.5f);
            program.SetUniform1(gl, "Shininess", 50f);

            //  Set the light position.
            program.SetUniform3(gl, "LightPosition", 0.25f, 0.25f, 1f);

            //  Set the matrices.
            program.SetUniformMatrix4(gl, "Projection", projectionMatrix.to_array());
            program.SetUniformMatrix4(gl, "Modelview", modelView.to_array());
            program.SetUniformMatrix3(gl, "NormalMatrix", mat3.identity().to_array());

            //  We can now test the ability to access uniforms.
            int bufferSize = 64;
            int length;
            int size;
            uint type;
            string name;
            gl.GetActiveUniform(program.ShaderProgramObject, normalAttribute, bufferSize, out length, out size, out type, out name);
  


            //  Bind the vertex buffer array.
            vertexBufferArray.Bind(gl);

            //  Draw the elements.
            gl.DrawElements(OpenGL.GL_TRIANGLES, torus.triangles.Length, OpenGL.GL_UNSIGNED_SHORT, IntPtr.Zero);

            //  Unbind the shader.
            program.Unbind(gl);

            Assert.AreEqual(ErrorCode.NoError, gl.GetErrorCode(), "OpenGL error during rendering of geometry.");

            ((FBORenderContextProvider)gl.RenderContextProvider).ReadBuffer();
            //context.ReadBuffer(gl);

            var dibSection = ((FBORenderContextProvider) gl.RenderContextProvider).InternalDIBSection;

            //  Get the rendered scene as an image.
            using (var renderedScene = CreateComparibleBitmap(dibSection.HBitmap))
            {
                if (ImageCompare.Compare(renderedScene, LoadReferenceBitmap()) == false)
                {
                    //  If they do not match, save the rendered scene and fail.
                    var path = Path.GetTempPath() + Guid.NewGuid().ToString() + ".png";
                    renderedScene.Save(path, ImageFormat.Png);

                    //  Fail the test.
                    Assert.Fail("The rendered scene does not match the reference image. The rendered scene has been saved to: '{0}'.", path);
                }
            }

            //context.Destroy(gl);
        }
    }
}