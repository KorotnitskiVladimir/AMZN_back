using AMZN.Data;
using AMZN.Data.Entities;
using AMZN.DTOs.Products.Questions;
using Microsoft.EntityFrameworkCore;

namespace AMZN.Repositories.Products.Questions
{
    public class ProductQuestionRepository : IProductQuestionRepository
    {
        private readonly DataContext _db;
        public ProductQuestionRepository(DataContext db) 
        {
            _db = db;
        }

        public Task SaveChangesAsync()
        {
            return _db.SaveChangesAsync();
        }

        public Task<ProductQuestion?> GetQuestionByIdAsync(Guid productId, Guid questionId)
        {
            return _db.ProductQuestions.FirstOrDefaultAsync(x => x.ProductId == productId && x.Id == questionId);
        }

        public Task<int> CountQuestionsAsync(Guid productId)
        {
            return _db.ProductQuestions
                .AsNoTracking()
                .CountAsync(x => x.ProductId == productId);
        }

        public Task<List<ProductQuestionDto>> GetQuestionsPageAsync(Guid productId, int skip, int take)
        {
            return BuildQuestionDtoQuery(productId)
                .OrderByDescending(x => x.CreatedAt)
                .ThenByDescending(x => x.Id)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<Dictionary<Guid, List<ProductQuestionAnswerDto>>> GetPreviewAnswersMapAsync(List<Guid> questionIds, int takePerQuestion)
        {
            if (questionIds.Count == 0 || takePerQuestion <= 0)
                return new Dictionary<Guid, List<ProductQuestionAnswerDto>>();

            var previewAnswerItems = await _db.ProductQuestionAnswers
                .AsNoTracking()
                .Where(x => questionIds.Contains(x.QuestionId))
                .OrderBy(x => x.QuestionId)
                .ThenBy(x => x.CreatedAt)
                .ThenBy(x => x.Id)
                .Select(x => new
                {
                    x.QuestionId,
                    Dto = new ProductQuestionAnswerDto
                    {
                        Id = x.Id,
                        Text = x.Text,
                        AuthorName = x.User.FirstName + " " + x.User.LastName,
                        IsSellerAnswer = x.UserId == x.Question.Product.SellerId,
                        CreatedAt = x.CreatedAt,
                        UpdatedAt = x.UpdatedAt
                    }
                })
                .ToListAsync();

            return previewAnswerItems
                .GroupBy(x => x.QuestionId)
                .ToDictionary(
                    g => g.Key,
                    g => g
                        .Take(takePerQuestion)
                        .Select(x => x.Dto)
                        .ToList()
                );
        }

        public Task<ProductQuestionDto?> GetQuestionDtoByIdAsync(Guid productId, Guid questionId)
        {
            return BuildQuestionDtoQuery(productId).FirstOrDefaultAsync(x => x.Id == questionId);
        }

        public void AddQuestion(ProductQuestion question)
        {
            _db.ProductQuestions.Add(question);
        }

        public Task<int> CountAnswersAsync(Guid questionId)
        {
            return _db.ProductQuestionAnswers
                .AsNoTracking()
                .CountAsync(x => x.QuestionId == questionId);
        }

        public Task<List<ProductQuestionAnswerDto>> GetAnswersPageAsync(Guid questionId, int skip, int take)
        {
            return BuildAnswerDtoQuery(questionId)
                .OrderBy(x => x.CreatedAt)
                .ThenBy(x => x.Id)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public Task<ProductQuestionAnswerDto?> GetAnswerDtoByIdAsync(Guid questionId, Guid answerId)
        {
            return BuildAnswerDtoQuery(questionId).FirstOrDefaultAsync(x => x.Id == answerId);
        }

        public void AddAnswer(ProductQuestionAnswer answer)
        {
            _db.ProductQuestionAnswers.Add(answer);
        }


        // Helpers
        private IQueryable<ProductQuestionDto> BuildQuestionDtoQuery(Guid productId)
        {
            return _db.ProductQuestions
                .AsNoTracking()
                .Where(x => x.ProductId == productId)
                .Select(x => new ProductQuestionDto
                {
                    Id = x.Id,
                    Text = x.Text,
                    AuthorName = x.User.FirstName + " " + x.User.LastName,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    AnswersCount = x.Answers.Count()
                });
        }

        private IQueryable<ProductQuestionAnswerDto> BuildAnswerDtoQuery(Guid questionId)
        {
            return _db.ProductQuestionAnswers
                .AsNoTracking()
                .Where(x => x.QuestionId == questionId)
                .Select(x => new ProductQuestionAnswerDto
                {
                    Id = x.Id,
                    Text = x.Text,
                    AuthorName = x.User.FirstName + " " + x.User.LastName,
                    IsSellerAnswer = x.UserId == x.Question.Product.SellerId,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt
                });
        }


    }


}
