using AMZN.Data.Entities;
using AMZN.DTOs.Products.Questions;

namespace AMZN.Repositories.Products.Questions
{
    public interface IProductQuestionRepository
    {
        Task SaveChangesAsync();

        Task<ProductQuestion?> GetQuestionByIdAsync(Guid productId, Guid questionId);

        Task<int> CountQuestionsAsync(Guid productId);
        Task<List<ProductQuestionDto>> GetQuestionsPageAsync(Guid productId, int skip, int take);
        Task<Dictionary<Guid, List<ProductQuestionAnswerDto>>> GetPreviewAnswersMapAsync(List<Guid> questionIds, int takePerQuestion);
        Task<ProductQuestionDto?> GetQuestionDtoByIdAsync(Guid productId, Guid questionId);

        void AddQuestion(ProductQuestion question);

        Task<int> CountAnswersAsync(Guid questionId);
        Task<List<ProductQuestionAnswerDto>> GetAnswersPageAsync(Guid questionId, int skip, int take);
        Task<ProductQuestionAnswerDto?> GetAnswerDtoByIdAsync(Guid questionId, Guid answerId);

        void AddAnswer(ProductQuestionAnswer answer);
    }

}
