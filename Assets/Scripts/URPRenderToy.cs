using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ShaderToyPassFeature : ScriptableRendererFeature
{

    [Tooltip("ShaderToy渲染用的Shader")]
    public Shader ShaderToyShader;

    class CustomRenderPass : ScriptableRenderPass
    {
        private string renderTag;
        private Material mat;
        private RenderTargetHandle source;
        private RenderTargetHandle destination;

        public CustomRenderPass(string tag, Shader s)
        {
            renderTag = tag;
            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
            mat = CoreUtils.CreateEngineMaterial(s);
        }

        public void Setup()
        {
            this.source.Init("_CameraColorTexture");
            this.destination.Init("_ShaderToyTemp");
        }
        
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            cmd.GetTemporaryRT(destination.id, renderingData.cameraData.cameraTargetDescriptor);
        }
        
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get(renderTag);
            cmd.Blit(source.Identifier(), destination.Identifier(), mat);
            cmd.Blit(destination.Identifier(), source.Identifier());
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
        
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(destination.id);
        }
    }


    CustomRenderPass m_ScriptablePass;

    public override void Create()
    {
        m_ScriptablePass = new CustomRenderPass(name, ShaderToyShader);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        m_ScriptablePass.Setup();
        renderer.EnqueuePass(m_ScriptablePass);
    }
}