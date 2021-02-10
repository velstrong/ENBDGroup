using System.ComponentModel.DataAnnotations;
using ENBDGroup.Foundation.Common.Core.Models;

namespace ENBDGroup.Foundation.Common.Core.Services
{
    public interface IMediatorService
    {
        MediatorResponse<T> GetMediatorResponse<T>(string code, T viewModel = default(T),
            ValidationResult validationResult = null, object parameters = null, string message = null);
    }
}
