using System.ComponentModel.DataAnnotations;

namespace TongBuilder.AI.Contract.Enums
{
    /// <summary>
    /// AI类型
    /// </summary>
    public enum AIType
    {
        [Display(Name = "Open AI")]
        OpenAI = 1,

        [Display(Name = "Azure Open AI")]
        AzureOpenAI = 2,

        [Display(Name = "LLama本地模型")]
        LLamaSharp = 3,

        [Display(Name = "星火大模型")]
        SparkDesk = 4,

        [Display(Name = "灵积大模型")]
        DashScope = 5,

        [Display(Name = "LLamaFactory")]
        LLamaFactory = 6,

        [Display(Name = "Bge Embedding")]
        BgeEmbedding = 7,

        [Display(Name = "Bge Rerank")]
        BgeRerank = 8,

        [Display(Name = "StableDiffusion")]
        StableDiffusion = 9,

        [Display(Name = "模拟输出")]
        Mock = 100,

    }
}
