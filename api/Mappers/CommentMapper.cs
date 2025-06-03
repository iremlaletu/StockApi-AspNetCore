

using api.Dtos.Comment;
using api.Models;

namespace api.Mappers
{
    public static class CommentMapper
    {
        public static CommentDto ToCommentDto(this Comment commentModel) // Converts a Comment entity (model) to a CommentDto to be returned to the client.
        {
            return new CommentDto
            {
                Id = commentModel.Id,
                Title = commentModel.Title,
                Content = commentModel.Content,
                CreatedOn = commentModel.CreatedOn,
                CreatedBy = commentModel.AppUser.UserName ?? "Anonymous",
                StockId = commentModel.StockId
            };
        }

        public static Comment ToCommentFromCreateDto(this CreateCommentDto commentDto, int stockId) // Converts the CreateCommentDto received from the client into a Comment entity. EFCore take care of the rest (id, createdOn, etc.)
        {
            return new Comment
            {
                Title = commentDto.Title,
                Content = commentDto.Content,
                StockId = stockId,
            };
        }
        
        public static Comment ToCommentFromUpdateDto(this UpdateCommentRequestDto commentDto) // Converts the CreateCommentDto received from the client into a Comment entity. EFCore take care of the rest (id, createdOn, etc.)
        {
            return new Comment
            {
                Title = commentDto.Title,
                Content = commentDto.Content,
            };
        }
    }
}