using System.Threading.Tasks;

namespace z.Report.Services
{
    public interface IRenderService
    {
        Task<byte[]> RenderAsync(RenderRequest request); 
    }
}
